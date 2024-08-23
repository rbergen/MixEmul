﻿using System;
using System.Collections.Generic;
using System.Linq;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixAssembler.Symbol;
using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Type;

namespace MixAssembler
{
	public static class Assembler
	{
		public static InstructionInstanceBase[] Assemble(string[] sourceLines, out PreInstruction[] preInstructions, out SymbolCollection symbols, out AssemblyFindingCollection findings)
		{
			var list = new List<InstructionInstanceBase>();
			var status = new ParsingStatus();

			preInstructions = Parser.ParseSource(sourceLines, status);
			findings = status.Findings;
			symbols = null;

			// if errors were found during parsing, abort assembly
			if (status.Findings.ContainsErrors)
				return null;

			status.LocationCounter = 0;
			status.LineNumber = 0;

			// compose the instruction instances based on the preinstructions returned by the parser
			foreach (PreInstruction instruction in preInstructions)
			{
				if (!instruction.IsDefined)
					continue;

				var parsedSourceLine = instruction as ParsedSourceLine;

				status.LineNumber = parsedSourceLine != null ? parsedSourceLine.LineNumber : sourceLines.Length;
				var instructionInstance = instruction.CreateInstance(status);

				if (parsedSourceLine != null && instructionInstance != null)
				{
					string sourceLineText = parsedSourceLine.LocationField + new string(' ', Math.Max(Parser.MinLocLength - parsedSourceLine.LocationField.Length, 0) + Parser.FieldSpacing);
					sourceLineText += parsedSourceLine.OpField + new string(' ', Math.Max(Parser.MinOpLength - parsedSourceLine.OpField.Length, 0) + Parser.FieldSpacing);
					sourceLineText += parsedSourceLine.AddressField + new string(' ', Math.Max(Parser.MinAddressLength - parsedSourceLine.AddressField.Length, 0) + Parser.FieldSpacing);
					sourceLineText += parsedSourceLine.Comment;

					instructionInstance.SourceLine = sourceLineText;
				}

				list.Add(instructionInstance);

				if (instructionInstance != null && instructionInstance.Instruction is LoaderInstruction loaderInstruction)
				{
					if (loaderInstruction.Operation == LoaderInstruction.Operations.SetLocationCounter)
					{
						status.LocationCounter = (int)((LoaderInstruction.Instance)instructionInstance).Value.LongValue;
						continue;
					}

					// in case the PC is set by this instruction instance, prevent the location counter from being increased
					if (loaderInstruction.Operation == LoaderInstruction.Operations.SetProgramCounter)
						continue;
				}

				status.LocationCounter++;
			}

			// if errors were found during assembly, don't return the instruction instances: they would be useless anyway
			if (status.Findings.ContainsErrors)
				return null;

			symbols = [.. status.Symbols.Where(symbol => symbol is ValueSymbol)];

			status.Findings.Add(new AssemblyInfo("assembly completed successfully", int.MinValue, LineSection.EntireLine, 0, 0));
			return [.. list];
		}

		public static InstructionInstanceBase Assemble(string instructionLine, int locationCounter, out ParsedSourceLine parsedLine, SymbolCollection symbols, out AssemblyFindingCollection findings)
		{
			var status = new ParsingStatus(symbols)
			{
				LocationCounter = locationCounter,
				LineNumber = 0
			};

			parsedLine = Parser.ParseInstructionLine(instructionLine, status);
			findings = status.Findings;

			// if errors were found while parsing, abort assembly
			if (status.Findings.ContainsErrors)
				return null;

			if (!parsedLine.IsDefined)
			{
				status.Findings.Add(new AssemblyError(0, LineSection.EntireLine, 0, 0, new ValidationError("line is not an instruction")));
				return null;
			}

			status.LocationCounter = locationCounter;
			status.LineNumber = 0;
			var instructionInstance = parsedLine.CreateInstance(status);

			// if errors were found while assembling, don't return the instruction instance: it would be useless anyway
			if (status.Findings.ContainsErrors)
				return null;

			return instructionInstance;
		}
	}
}
