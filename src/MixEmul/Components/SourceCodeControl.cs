using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixAssembler;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Settings;
using MixLib.Misc;

namespace MixGui.Components
{
	public class SourceCodeControl : UserControl
	{
		private const string LineNumberSeparator = " │ ";
		private static readonly Color[] _findingColors = 
		{
			GuiSettings.GetColor(GuiSettings.DebugText),
			GuiSettings.GetColor(GuiSettings.InfoText),
			GuiSettings.GetColor(GuiSettings.WarningText),
			GuiSettings.GetColor(GuiSettings.ErrorText)
		};

		private Color _addressColor;
		private Color _commentColor;
		private bool _findingsColored;
		private readonly List<ProcessedSourceLine> _instructions;
		private Color _lineNumberColor;
		private int _lineNumberLength;
		private Color _lineNumberSeparatorColor;
		private Color _locColor;
		private AssemblyFinding _markedFinding;
		private Color _opColor;
		private readonly RichTextBox _sourceBox = new();

		public SourceCodeControl()
		{
			SuspendLayout();

			_sourceBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			_sourceBox.BorderStyle = BorderStyle.FixedSingle;
			_sourceBox.Location = new Point(0, 0);
			_sourceBox.Name = "mSourceBox";
			_sourceBox.ReadOnly = true;
			_sourceBox.Size = Size;
			_sourceBox.TabIndex = 0;
			_sourceBox.Text = "";
			_sourceBox.DetectUrls = false;

			Controls.Add(_sourceBox);
			Name = "SourceCodeControl";
			ResumeLayout(false);

			UpdateLayout();

			_instructions = new List<ProcessedSourceLine>();
			_lineNumberLength = 0;
			_findingsColored = false;
		}

		private void AddLine(ParsedSourceLine sourceLine)
		{
			int count = _instructions.Count;
			if (_sourceBox.TextLength != 0)
				_sourceBox.AppendText(Environment.NewLine);

			var lineNumberText = (count + 1).ToString();
			_sourceBox.AppendText(new string(' ', _lineNumberLength - lineNumberText.Length) + lineNumberText + LineNumberSeparator);

			var processedLine = new ProcessedSourceLine(sourceLine, _sourceBox.TextLength);
			_instructions.Add(processedLine);

			if (sourceLine.IsCommentLine)
			{
				if (sourceLine.Comment.Length > 0)
					_sourceBox.AppendText(sourceLine.Comment);
			}
			else
			{
				_sourceBox.AppendText(sourceLine.LocationField + new string(' ', (processedLine.LocTextLength - sourceLine.LocationField.Length) + Parser.FieldSpacing));
				_sourceBox.AppendText(sourceLine.OpField + new string(' ', (processedLine.OpTextLength - sourceLine.OpField.Length) + Parser.FieldSpacing));
				_sourceBox.AppendText(sourceLine.AddressField + new string(' ', (processedLine.AddressTextLength - sourceLine.AddressField.Length) + Parser.FieldSpacing));

				if (sourceLine.Comment.Length > 0)
					_sourceBox.AppendText(sourceLine.Comment);
			}

			ApplySyntaxColoring(processedLine);
		}

		private void ApplyFindingColoring(AssemblyFinding finding, MarkOperation mark)
		{
			if (finding == null || finding.LineNumber == int.MinValue || finding.LineNumber < 0 || finding.LineNumber >= _instructions.Count)
				return;

			ProcessedSourceLine processedLine = _instructions[finding.LineNumber];
			int lineTextIndex = processedLine.LineTextIndex;
			int length = 0;

			switch (finding.LineSection)
			{
				case LineSection.LocationField:
					if (finding.Length <= 0)
					{
						lineTextIndex = processedLine.LocTextIndex;
						length = processedLine.SourceLine.LocationField.Length;

						break;
					}

					lineTextIndex = processedLine.LocTextIndex + finding.StartCharIndex;
					length = finding.Length;

					break;

				case LineSection.OpField:
					if (finding.Length <= 0)
					{
						lineTextIndex = processedLine.OpTextIndex;
						length = processedLine.SourceLine.OpField.Length;

						break;
					}

					lineTextIndex = processedLine.OpTextIndex + finding.StartCharIndex;
					length = finding.Length;

					break;

				case LineSection.AddressField:
					if (finding.Length <= 0)
					{
						lineTextIndex = processedLine.AddressTextIndex;
						length = processedLine.SourceLine.AddressField.Length;

						break;
					}

					lineTextIndex = processedLine.AddressTextIndex + finding.StartCharIndex;
					length = finding.Length;

					break;

				case LineSection.CommentField:
					if (finding.Length <= 0)
					{
						lineTextIndex = processedLine.CommentTextIndex;
						length = processedLine.SourceLine.Comment.Length;

						break;
					}

					lineTextIndex = processedLine.CommentTextIndex + finding.StartCharIndex;
					length = finding.Length;

					break;

				case LineSection.EntireLine:
					length = processedLine.LineTextLength;

					break;
			}

			_sourceBox.Select(lineTextIndex, length);

			if (length != 0)
			{
				if (mark == MarkOperation.Mark)
				{
					Font font = _sourceBox.Font;

					_sourceBox.SelectionFont = new Font(font.Name, font.Size, FontStyle.Underline, font.Unit, font.GdiCharSet);
					_sourceBox.Focus();
					_sourceBox.ScrollToCaret();
				}
				else if (mark == MarkOperation.Unmark)
				{
					_sourceBox.SelectionFont = _sourceBox.Font;
				}

				_sourceBox.SelectionColor = _findingColors[(int)finding.Severity];
				_sourceBox.SelectionLength = 0;
			}
		}

		private void ApplyFindingColoring(AssemblyFindingCollection findings, Severity severity)
		{
			foreach (AssemblyFinding finding in findings)
			{
				if (finding.Severity == severity)
					ApplyFindingColoring(finding, MarkOperation.None);
			}
		}

		private void ApplySyntaxColoring(ProcessedSourceLine processedLine)
		{
			_sourceBox.SelectionStart = (processedLine.LineTextIndex - LineNumberSeparator.Length) - _lineNumberLength;
			_sourceBox.SelectionLength = _lineNumberLength;
			_sourceBox.SelectionColor = _lineNumberColor;
			_sourceBox.SelectionStart += _lineNumberLength;
			_sourceBox.SelectionLength = LineNumberSeparator.Length;
			_sourceBox.SelectionColor = _lineNumberSeparatorColor;

			if (!processedLine.SourceLine.IsCommentLine)
			{
				if (processedLine.SourceLine.LocationField.Length > 0)
				{
					_sourceBox.SelectionStart = processedLine.LocTextIndex;
					_sourceBox.SelectionLength = processedLine.SourceLine.LocationField.Length;
					_sourceBox.SelectionColor = _locColor;
				}

				if (processedLine.SourceLine.OpField.Length > 0)
				{
					_sourceBox.SelectionStart = processedLine.OpTextIndex;
					_sourceBox.SelectionLength = processedLine.SourceLine.OpField.Length;
					_sourceBox.SelectionColor = _opColor;
				}

				if (processedLine.SourceLine.AddressField.Length > 0)
				{
					_sourceBox.SelectionStart = processedLine.AddressTextIndex;
					_sourceBox.SelectionLength = processedLine.SourceLine.AddressField.Length;
					_sourceBox.SelectionColor = _addressColor;
				}
			}

			if (processedLine.SourceLine.Comment.Length > 0)
			{
				_sourceBox.SelectionStart = processedLine.CommentTextIndex;
				_sourceBox.SelectionLength = processedLine.SourceLine.Comment.Length;
				_sourceBox.SelectionColor = _commentColor;
			}
		}

		public void UpdateLayout()
		{
			_sourceBox.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_sourceBox.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			_lineNumberColor = GuiSettings.GetColor(GuiSettings.LineNumberText);
			_lineNumberSeparatorColor = GuiSettings.GetColor(GuiSettings.LineNumberSeparator);
			_locColor = GuiSettings.GetColor(GuiSettings.LocationFieldText);
			_opColor = GuiSettings.GetColor(GuiSettings.OpFieldText);
			_addressColor = GuiSettings.GetColor(GuiSettings.AddressFieldText);
			_commentColor = GuiSettings.GetColor(GuiSettings.CommentFieldText);
		}

		public AssemblyFindingCollection Findings
		{
			set
			{
				ApplyFindingColoring(_markedFinding, MarkOperation.Unmark);
				_markedFinding = null;

				if (_findingsColored)
				{
					foreach (ProcessedSourceLine line in _instructions)
						ApplySyntaxColoring(line);
				}

				if (value.ContainsErrors)
					ApplyFindingColoring(value, Severity.Error);

				else if (value.ContainsWarnings)
					ApplyFindingColoring(value, Severity.Warning);

				else if (value.ContainsInfos)
					ApplyFindingColoring(value, Severity.Info);

				else if (value.ContainsDebugs)
					ApplyFindingColoring(value, Severity.Debug);

				_findingsColored = true;
			}
		}

		public PreInstruction[] Instructions
		{
			set
			{
				_markedFinding = null;
				_sourceBox.Text = "";
				_instructions.Clear();
				_findingsColored = false;

				if (value == null)
				{
					_lineNumberLength = 0;
					return;
				}

				int lineCount = value.Length;
				_lineNumberLength = lineCount.ToString().Length;

				var parsedLines = new SortedList<int, ParsedSourceLine>();

				foreach (PreInstruction instruction in value)
				{
					if (instruction is ParsedSourceLine parsedLine)
					{
						parsedLines.Add(parsedLine.LineNumber, parsedLine);
					}
				}

				foreach (ParsedSourceLine parsedLine in parsedLines.Values)
				{
					while (parsedLine.LineNumber > _instructions.Count)
					{
						AddLine(new ParsedSourceLine(_instructions.Count, ""));
					}

					AddLine(parsedLine);
				}

				_sourceBox.SelectionStart = 0;
				_sourceBox.SelectionLength = 0;
			}
		}

		public AssemblyFinding MarkedFinding
		{
			get => _markedFinding;
			set
			{
				ApplyFindingColoring(_markedFinding, MarkOperation.Unmark);

				_markedFinding = value;

				ApplyFindingColoring(_markedFinding, MarkOperation.Mark);
			}
		}

		private enum MarkOperation
		{
			Unmark,
			None,
			Mark
		}

		private class ProcessedSourceLine
		{
			public ParsedSourceLine SourceLine { get; private set; }
			public int LineTextIndex { get; private set; }

			public ProcessedSourceLine(ParsedSourceLine sourceLine, int lineTextIndex)
			{
				SourceLine = sourceLine;
				LineTextIndex = lineTextIndex;
			}

			public int CommentTextIndex =>
					!SourceLine.IsCommentLine ? AddressTextIndex + AddressTextLength + Parser.FieldSpacing : LineTextIndex;

			public int LineTextLength =>
					(SourceLine.Comment == "" ? AddressTextIndex + AddressTextLength : CommentTextIndex + SourceLine.Comment.Length) - LineTextIndex;

			public int LocTextIndex 
				=> LineTextIndex;

			public int LocTextLength 
				=> !SourceLine.IsCommentLine ? Math.Max(Parser.MinLocLength, SourceLine.LocationField.Length) : 0;

			public int OpTextIndex 
				=> !SourceLine.IsCommentLine ? LocTextIndex + LocTextLength + Parser.FieldSpacing : LineTextIndex;

			public int OpTextLength 
				=> !SourceLine.IsCommentLine ? Math.Max(Parser.MinOpLength, SourceLine.OpField.Length) : 0;

			public int AddressTextIndex 
				=> !SourceLine.IsCommentLine ? OpTextIndex + OpTextLength + Parser.FieldSpacing : LineTextIndex;

			public int AddressTextLength
			{
				get
				{
					if (SourceLine.IsCommentLine)
						return 0;

					if (SourceLine.Comment != "")
						return Math.Max(Parser.MinAddressLength, SourceLine.AddressField.Length);

					return SourceLine.AddressField.Length;
				}
			}
		}
	}
}
