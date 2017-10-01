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
        const string lineNumberSeparator = " │ ";

        static readonly Color[] findingColors = {
            GuiSettings.GetColor(GuiSettings.DebugText),
            GuiSettings.GetColor(GuiSettings.InfoText),
            GuiSettings.GetColor(GuiSettings.WarningText),
            GuiSettings.GetColor(GuiSettings.ErrorText)
        };

        Color mAddressColor;
        Color mCommentColor;
        bool mFindingsColored;
        List<ProcessedSourceLine> mInstructions;
        Color mLineNumberColor;
        int mLineNumberLength;
        Color mLineNumberSeparatorColor;
        Color mLocColor;
        AssemblyFinding mMarkedFinding;
        Color mOpColor;
        RichTextBox mSourceBox = new RichTextBox();

        public SourceCodeControl()
        {
            SuspendLayout();

            mSourceBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            mSourceBox.Location = new Point(0, 0);
            mSourceBox.Name = "mSourceBox";
            mSourceBox.ReadOnly = true;
            mSourceBox.Size = Size;
            mSourceBox.TabIndex = 0;
            mSourceBox.Text = "";
            mSourceBox.DetectUrls = false;

            Controls.Add(mSourceBox);
            Name = "SourceCodeControl";
            ResumeLayout(false);

            UpdateLayout();

            mInstructions = new List<ProcessedSourceLine>();
            mLineNumberLength = 0;
            mFindingsColored = false;
        }

        void AddLine(ParsedSourceLine sourceLine)
        {
            int count = mInstructions.Count;
            if (mSourceBox.TextLength != 0) mSourceBox.AppendText(Environment.NewLine);

            var lineNumberText = (count + 1).ToString();
            mSourceBox.AppendText(new string(' ', mLineNumberLength - lineNumberText.Length) + lineNumberText + lineNumberSeparator);

            var processedLine = new ProcessedSourceLine(sourceLine, mSourceBox.TextLength);
            mInstructions.Add(processedLine);

            if (sourceLine.IsCommentLine)
            {
                if (sourceLine.Comment.Length > 0) mSourceBox.AppendText(sourceLine.Comment);
            }
            else
            {
                mSourceBox.AppendText(sourceLine.LocationField + new string(' ', (processedLine.LocTextLength - sourceLine.LocationField.Length) + Parser.FieldSpacing));
                mSourceBox.AppendText(sourceLine.OpField + new string(' ', (processedLine.OpTextLength - sourceLine.OpField.Length) + Parser.FieldSpacing));
                mSourceBox.AppendText(sourceLine.AddressField + new string(' ', (processedLine.AddressTextLength - sourceLine.AddressField.Length) + Parser.FieldSpacing));
                if (sourceLine.Comment.Length > 0) mSourceBox.AppendText(sourceLine.Comment);
            }

            ApplySyntaxColoring(processedLine);
        }

        void ApplyFindingColoring(AssemblyFinding finding, MarkOperation mark)
        {
            if (finding != null && finding.LineNumber != int.MinValue && finding.LineNumber >= 0 && finding.LineNumber < mInstructions.Count)
            {
                ProcessedSourceLine processedLine = mInstructions[finding.LineNumber];
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

                mSourceBox.Select(lineTextIndex, length);

                if (length != 0)
                {
                    if (mark == MarkOperation.Mark)
                    {
                        Font font = mSourceBox.Font;

                        mSourceBox.SelectionFont = new Font(font.Name, font.Size, FontStyle.Underline, font.Unit, font.GdiCharSet);
                        mSourceBox.Focus();
                        mSourceBox.ScrollToCaret();
                    }
                    else if (mark == MarkOperation.Unmark)
                    {
                        mSourceBox.SelectionFont = mSourceBox.Font;
                    }

                    mSourceBox.SelectionColor = findingColors[(int)finding.Severity];
                    mSourceBox.SelectionLength = 0;
                }
            }
        }

        void ApplyFindingColoring(AssemblyFindingCollection findings, Severity severity)
        {
            foreach (AssemblyFinding finding in findings)
            {
                if (finding.Severity == severity) ApplyFindingColoring(finding, MarkOperation.None);
            }
        }

        void ApplySyntaxColoring(ProcessedSourceLine processedLine)
        {
            mSourceBox.SelectionStart = (processedLine.LineTextIndex - lineNumberSeparator.Length) - mLineNumberLength;
            mSourceBox.SelectionLength = mLineNumberLength;
            mSourceBox.SelectionColor = mLineNumberColor;
            mSourceBox.SelectionStart += mLineNumberLength;
            mSourceBox.SelectionLength = lineNumberSeparator.Length;
            mSourceBox.SelectionColor = mLineNumberSeparatorColor;

            if (!processedLine.SourceLine.IsCommentLine)
            {
                if (processedLine.SourceLine.LocationField.Length > 0)
                {
                    mSourceBox.SelectionStart = processedLine.LocTextIndex;
                    mSourceBox.SelectionLength = processedLine.SourceLine.LocationField.Length;
                    mSourceBox.SelectionColor = mLocColor;
                }

                if (processedLine.SourceLine.OpField.Length > 0)
                {
                    mSourceBox.SelectionStart = processedLine.OpTextIndex;
                    mSourceBox.SelectionLength = processedLine.SourceLine.OpField.Length;
                    mSourceBox.SelectionColor = mOpColor;
                }

                if (processedLine.SourceLine.AddressField.Length > 0)
                {
                    mSourceBox.SelectionStart = processedLine.AddressTextIndex;
                    mSourceBox.SelectionLength = processedLine.SourceLine.AddressField.Length;
                    mSourceBox.SelectionColor = mAddressColor;
                }
            }

            if (processedLine.SourceLine.Comment.Length > 0)
            {
                mSourceBox.SelectionStart = processedLine.CommentTextIndex;
                mSourceBox.SelectionLength = processedLine.SourceLine.Comment.Length;
                mSourceBox.SelectionColor = mCommentColor;
            }
        }

        public void UpdateLayout()
        {
            mSourceBox.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
            mSourceBox.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
            mLineNumberColor = GuiSettings.GetColor(GuiSettings.LineNumberText);
            mLineNumberSeparatorColor = GuiSettings.GetColor(GuiSettings.LineNumberSeparator);
            mLocColor = GuiSettings.GetColor(GuiSettings.LocationFieldText);
            mOpColor = GuiSettings.GetColor(GuiSettings.OpFieldText);
            mAddressColor = GuiSettings.GetColor(GuiSettings.AddressFieldText);
            mCommentColor = GuiSettings.GetColor(GuiSettings.CommentFieldText);
        }

        public AssemblyFindingCollection Findings
        {
            set
            {
                ApplyFindingColoring(mMarkedFinding, MarkOperation.Unmark);
                mMarkedFinding = null;

                if (mFindingsColored)
                {
                    foreach (ProcessedSourceLine line in mInstructions) ApplySyntaxColoring(line);
                }

                if (value.ContainsDebugs) ApplyFindingColoring(value, Severity.Debug);
                if (value.ContainsInfos) ApplyFindingColoring(value, Severity.Info);
                if (value.ContainsWarnings) ApplyFindingColoring(value, Severity.Warning);
                if (value.ContainsErrors) ApplyFindingColoring(value, Severity.Error);

                mFindingsColored = true;
            }
        }

        public PreInstruction[] Instructions
        {
            set
            {
                mMarkedFinding = null;
                mSourceBox.Text = "";
                mInstructions.Clear();
                mFindingsColored = false;

                if (value == null)
                {
                    mLineNumberLength = 0;
                }
                else
                {
                    int lineCount = value.Length;
                    mLineNumberLength = lineCount.ToString().Length;

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
                        while (parsedLine.LineNumber > mInstructions.Count)
                        {
                            AddLine(new ParsedSourceLine(mInstructions.Count, ""));
                        }

                        AddLine(parsedLine);
                    }

                    mSourceBox.SelectionStart = 0;
                    mSourceBox.SelectionLength = 0;
                }
            }
        }

        public AssemblyFinding MarkedFinding
        {
            get
            {
                return mMarkedFinding;
            }
            set
            {
                ApplyFindingColoring(mMarkedFinding, MarkOperation.Unmark);

                mMarkedFinding = value;

                ApplyFindingColoring(mMarkedFinding, MarkOperation.Mark);
            }
        }

        enum MarkOperation
        {
            Unmark,
            None,
            Mark
        }

        class ProcessedSourceLine
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

            public int LocTextIndex => LineTextIndex;

            public int LocTextLength => !SourceLine.IsCommentLine ? Math.Max(Parser.MinLocLength, SourceLine.LocationField.Length) : 0;

            public int OpTextIndex => !SourceLine.IsCommentLine ? LocTextIndex + LocTextLength + Parser.FieldSpacing : LineTextIndex;

            public int OpTextLength => !SourceLine.IsCommentLine ? Math.Max(Parser.MinOpLength, SourceLine.OpField.Length) : 0;

            public int AddressTextIndex => !SourceLine.IsCommentLine ? OpTextIndex + OpTextLength + Parser.FieldSpacing : LineTextIndex;

            public int AddressTextLength
            {
                get
                {
                    if (SourceLine.IsCommentLine) return 0;

                    if (SourceLine.Comment != "")
                    {
                        return Math.Max(Parser.MinAddressLength, SourceLine.AddressField.Length);
                    }

                    return SourceLine.AddressField.Length;
                }
            }
        }
    }
}
