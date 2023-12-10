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
		private static readonly Color[] findingColors = 
		[
			GuiSettings.GetColor(GuiSettings.DebugText),
			GuiSettings.GetColor(GuiSettings.InfoText),
			GuiSettings.GetColor(GuiSettings.WarningText),
			GuiSettings.GetColor(GuiSettings.ErrorText)
		];

		private Color addressColor;
		private Color commentColor;
		private bool findingsColored;
		private readonly List<ProcessedSourceLine> instructions;
		private Color lineNumberColor;
		private int lineNumberLength;
		private Color lineNumberSeparatorColor;
		private Color locColor;
		private AssemblyFinding markedFinding;
		private Color opColor;
		private readonly RichTextBox sourceBox = new();

		public SourceCodeControl()
		{
			SuspendLayout();

			this.sourceBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			this.sourceBox.BorderStyle = BorderStyle.FixedSingle;
			this.sourceBox.Location = new Point(0, 0);
			this.sourceBox.Name = "mSourceBox";
			this.sourceBox.ReadOnly = true;
			this.sourceBox.Size = Size;
			this.sourceBox.TabIndex = 0;
			this.sourceBox.Text = string.Empty;
			this.sourceBox.DetectUrls = false;

			Controls.Add(this.sourceBox);
			Name = "SourceCodeControl";
			ResumeLayout(false);

			UpdateLayout();

			this.instructions = [];
			this.lineNumberLength = 0;
			this.findingsColored = false;
		}

		private void AddLine(ParsedSourceLine sourceLine)
		{
			int count = this.instructions.Count;
			if (this.sourceBox.TextLength != 0)
				this.sourceBox.AppendText(Environment.NewLine);

			var lineNumberText = (count + 1).ToString();
			this.sourceBox.AppendText(new string(' ', this.lineNumberLength - lineNumberText.Length) + lineNumberText + LineNumberSeparator);

			var processedLine = new ProcessedSourceLine(sourceLine, this.sourceBox.TextLength);
			this.instructions.Add(processedLine);

			if (sourceLine.IsCommentLine)
			{
				if (sourceLine.Comment.Length > 0)
					this.sourceBox.AppendText(sourceLine.Comment);
			}
			else
			{
				this.sourceBox.AppendText(sourceLine.LocationField + new string(' ', (processedLine.LocTextLength - sourceLine.LocationField.Length) + Parser.FieldSpacing));
				this.sourceBox.AppendText(sourceLine.OpField + new string(' ', (processedLine.OpTextLength - sourceLine.OpField.Length) + Parser.FieldSpacing));
				this.sourceBox.AppendText(sourceLine.AddressField + new string(' ', (processedLine.AddressTextLength - sourceLine.AddressField.Length) + Parser.FieldSpacing));

				if (sourceLine.Comment.Length > 0)
					this.sourceBox.AppendText(sourceLine.Comment);
			}

			ApplySyntaxColoring(processedLine);
		}

		private void ApplyFindingColoring(AssemblyFinding finding, MarkOperation mark)
		{
			if (finding == null || finding.LineNumber == int.MinValue || finding.LineNumber < 0 || finding.LineNumber >= this.instructions.Count)
				return;

			ProcessedSourceLine processedLine = this.instructions[finding.LineNumber];
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

			this.sourceBox.Select(lineTextIndex, length);

			if (length != 0)
			{
				if (mark == MarkOperation.Mark)
				{
					Font font = this.sourceBox.Font;

					this.sourceBox.SelectionFont = new Font(font.Name, font.Size, FontStyle.Underline, font.Unit, font.GdiCharSet);
					this.sourceBox.Focus();
					this.sourceBox.ScrollToCaret();
				}
				else if (mark == MarkOperation.Unmark)
				{
					this.sourceBox.SelectionFont = this.sourceBox.Font;
				}

				this.sourceBox.SelectionColor = findingColors[(int)finding.Severity];
				this.sourceBox.SelectionLength = 0;
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
			this.sourceBox.SelectionStart = (processedLine.LineTextIndex - LineNumberSeparator.Length) - this.lineNumberLength;
			this.sourceBox.SelectionLength = this.lineNumberLength;
			this.sourceBox.SelectionColor = this.lineNumberColor;
			this.sourceBox.SelectionStart += this.lineNumberLength;
			this.sourceBox.SelectionLength = LineNumberSeparator.Length;
			this.sourceBox.SelectionColor = this.lineNumberSeparatorColor;

			if (!processedLine.SourceLine.IsCommentLine)
			{
				if (processedLine.SourceLine.LocationField.Length > 0)
				{
					this.sourceBox.SelectionStart = processedLine.LocTextIndex;
					this.sourceBox.SelectionLength = processedLine.SourceLine.LocationField.Length;
					this.sourceBox.SelectionColor = this.locColor;
				}

				if (processedLine.SourceLine.OpField.Length > 0)
				{
					this.sourceBox.SelectionStart = processedLine.OpTextIndex;
					this.sourceBox.SelectionLength = processedLine.SourceLine.OpField.Length;
					this.sourceBox.SelectionColor = this.opColor;
				}

				if (processedLine.SourceLine.AddressField.Length > 0)
				{
					this.sourceBox.SelectionStart = processedLine.AddressTextIndex;
					this.sourceBox.SelectionLength = processedLine.SourceLine.AddressField.Length;
					this.sourceBox.SelectionColor = this.addressColor;
				}
			}

			if (processedLine.SourceLine.Comment.Length > 0)
			{
				this.sourceBox.SelectionStart = processedLine.CommentTextIndex;
				this.sourceBox.SelectionLength = processedLine.SourceLine.Comment.Length;
				this.sourceBox.SelectionColor = this.commentColor;
			}
		}

		public void UpdateLayout()
		{
			this.sourceBox.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.sourceBox.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			this.lineNumberColor = GuiSettings.GetColor(GuiSettings.LineNumberText);
			this.lineNumberSeparatorColor = GuiSettings.GetColor(GuiSettings.LineNumberSeparator);
			this.locColor = GuiSettings.GetColor(GuiSettings.LocationFieldText);
			this.opColor = GuiSettings.GetColor(GuiSettings.OpFieldText);
			this.addressColor = GuiSettings.GetColor(GuiSettings.AddressFieldText);
			this.commentColor = GuiSettings.GetColor(GuiSettings.CommentFieldText);
		}

		public AssemblyFindingCollection Findings
		{
			set
			{
				ApplyFindingColoring(this.markedFinding, MarkOperation.Unmark);
				this.markedFinding = null;

				if (this.findingsColored)
				{
					foreach (ProcessedSourceLine line in this.instructions)
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

				this.findingsColored = true;
			}
		}

		public PreInstruction[] Instructions
		{
			set
			{
				this.markedFinding = null;
				this.sourceBox.Text = string.Empty;
				this.instructions.Clear();
				this.findingsColored = false;

				if (value == null)
				{
					this.lineNumberLength = 0;
					return;
				}

				int lineCount = value.Length;
				this.lineNumberLength = lineCount.ToString().Length;

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
					while (parsedLine.LineNumber > this.instructions.Count)
					{
						AddLine(new ParsedSourceLine(this.instructions.Count, string.Empty));
					}

					AddLine(parsedLine);
				}

				this.sourceBox.SelectionStart = 0;
				this.sourceBox.SelectionLength = 0;
			}
		}

		public AssemblyFinding MarkedFinding
		{
			get => this.markedFinding;
			set
			{
				ApplyFindingColoring(this.markedFinding, MarkOperation.Unmark);

				this.markedFinding = value;

				ApplyFindingColoring(this.markedFinding, MarkOperation.Mark);
			}
		}

		private enum MarkOperation
		{
			Unmark,
			None,
			Mark
		}

		private class ProcessedSourceLine(ParsedSourceLine sourceLine, int lineTextIndex)
	{
	  public ParsedSourceLine SourceLine { get; private set; } = sourceLine;
	  public int LineTextIndex { get; private set; } = lineTextIndex;

	  public int CommentTextIndex =>
					!SourceLine.IsCommentLine ? AddressTextIndex + AddressTextLength + Parser.FieldSpacing : LineTextIndex;

			public int LineTextLength =>
					(SourceLine.Comment == string.Empty ? AddressTextIndex + AddressTextLength : CommentTextIndex + SourceLine.Comment.Length) - LineTextIndex;

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

					if (SourceLine.Comment != string.Empty)
						return Math.Max(Parser.MinAddressLength, SourceLine.AddressField.Length);

					return SourceLine.AddressField.Length;
				}
			}
		}
	}
}
