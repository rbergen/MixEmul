using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MixAssembler;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Type;

namespace MixGui.Components
{
	public class InstructionInstanceTextBox : RichTextBox, IWordEditor, IEscapeConsumer
	{
		private const long Unrendered = long.MinValue;

		private Color editingTextColor;
		private Color errorTextColor;
		private Color immutableTextColor;
		private Color renderedTextColor;
		private Color editorBackgroundColor;
		private Color loadedInstructionTextColor;
		private Color loadedInstructionBackgroundColor;

		private bool editMode;
		private IFullWord instructionWord;
		private long lastRenderedMagnitude;
		private Word.Signs lastRenderedSign;
		private ToolTip toolTip;
		private bool noInstructionRendered;
		private bool updating;
		private string lastRenderedSourceLine;
		private string caption;
		private bool showShourceLineToolTip;
		private bool lastRenderedShowSourceLineToolTip;
		private readonly ToolStripMenuItem showAddressMenuItem;
		private readonly ToolStripMenuItem showIndexedAddressMenuItem;
		private readonly ContextMenuStrip contextMenu;

		public int MemoryMinIndex { get; set; }
		public int MemoryMaxIndex { get; set; }
		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback { get; set; } = null;
		public SymbolCollection Symbols { get; set; }
		public int MemoryAddress { get; set; }


		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public InstructionInstanceTextBox(IFullWord instructionWord = null)
		{
			this.instructionWord = instructionWord ?? new FullWord();

			UpdateLayout();

			DetectUrls = false;
			TextChanged += This_TextChanged;
			KeyPress += This_KeyPress;
			KeyDown += This_KeyDown;
			LostFocus += This_LostFocus;
			Enter += This_Enter;
			ReadOnlyChanged += This_ReadOnlyChanged;

			this.showAddressMenuItem = new ToolStripMenuItem("Show address", null, ShowAddressMenuItem_Click);

			this.showIndexedAddressMenuItem = new ToolStripMenuItem("Show indexed address", null, ShowIndexedAddressMenuItem_Click);

			this.contextMenu = new ContextMenuStrip();
			this.contextMenu.Items.Add(this.showAddressMenuItem);
			this.contextMenu.Items.Add(this.showIndexedAddressMenuItem);

			ContextMenuStrip = this.contextMenu;

			this.updating = false;
			this.editMode = false;
			MemoryAddress = 0;
			this.lastRenderedMagnitude = Unrendered;
			this.lastRenderedSign = Word.Signs.Positive;
			this.lastRenderedSourceLine = null;
			this.noInstructionRendered = true;
			this.showShourceLineToolTip = true;

			Update();
		}

		public bool ShowSourceLineToolTip 
		{
			get => this.showShourceLineToolTip;
			set
			{
				if (this.showShourceLineToolTip == value)
					return;

				this.showShourceLineToolTip = value;
				Update();
			}
		}


		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> FieldTypes.Instruction;

		public int? CaretIndex
			=> SelectionStart + SelectionLength;

		public bool Focus(FieldTypes? field, int? index)
			=> this.FocusWithIndex(index);

		protected void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		protected void OnAddressSelected(AddressSelectedEventArgs args)
			=> AddressSelected?.Invoke(this, args);

		private void This_Enter(object sender, EventArgs e)
			=> CheckContentsEditable();

		private void ShowIndexedAddressMenuItem_Click(object sender, EventArgs e)
			=> OnAddressSelected(new AddressSelectedEventArgs(IndexedAddressCalculatorCallback(GetAddress(), this.instructionWord[MixInstruction.IndexByte])));

		private void ShowAddressMenuItem_Click(object sender, EventArgs e)
			=> OnAddressSelected(new AddressSelectedEventArgs(GetAddress()));

		private void AddTextElement(string element, bool markAsError)
		{
			if (element == "")
				return;

			int textLength = TextLength;
			AppendText(element);
			Select(textLength, element.Length);
			SelectionColor = markAsError ? this.errorTextColor : ForeColor;
		}

		private string GetAssemblyErrorsCaption(ParsedSourceLine line, AssemblyFindingCollection findings)
		{
			string caption = "";
			int findingNumber = 1;

			foreach (AssemblyFinding finding in findings)
			{
				string fieldLabel = "";

				if (finding.Severity == Severity.Error)
				{
					switch (finding.LineSection)
					{
						case LineSection.OpField:
							MarkAssemblyError(finding.StartCharIndex, finding.Length);
							fieldLabel = "mnemonic";

							break;

						case LineSection.AddressField:
							if (finding.Length != 0)
								MarkAssemblyError((line.OpField.Length + finding.StartCharIndex) + 1, finding.Length);

							else
								MarkAssemblyError(line.OpField.Length + 1, line.AddressField.Length);

							fieldLabel = "address";

							break;

						case LineSection.CommentField:

							continue;

						case LineSection.EntireLine:
							MarkAssemblyError(0, TextLength);
							fieldLabel = "instruction";

							break;
					}
				}

				if (findingNumber != 1)
					caption = string.Concat(caption, Environment.NewLine, findingNumber, ". ");

				if (findingNumber == 2)
					caption = "1. " + caption;

				caption += fieldLabel + ": " + finding.Message;

				findingNumber++;
			}

			return findingNumber == 2
					? string.Concat("Instruction error: ", caption)
					: string.Concat("Instruction errors:", Environment.NewLine, caption);
		}

		private bool AssembleInstruction(bool markErrors)
		{
			this.editMode = false;

			var instance = Assembler.Assemble(Text, MemoryAddress, out ParsedSourceLine line, Symbols, out AssemblyFindingCollection findings);

			if (instance != null && !(instance is MixInstruction.Instance))
			{
				findings.Add(new AssemblyError(0, LineSection.OpField, 0, line.OpField.Length, new ValidationError("only MIX instruction mnemonics supported")));
				instance = null;
			}

			if (markErrors && findings.Count > 0)
			{
				this.updating = true;

				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				SuspendLayout();

				base.Text = line.OpField + " " + line.AddressField;

				this.caption = GetAssemblyErrorsCaption(line, findings);
				this.toolTip?.SetToolTip(this, this.caption);

				Select(selectionStart, selectionLength);
				ResumeLayout();

				this.updating = false;
			}

			if (instance == null)
				return false;

			var oldValue = new FullWord(this.instructionWord.LongValue);
			this.instructionWord.LongValue = ((MixInstruction.Instance)instance).InstructionWord.LongValue;

			Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new FullWord(this.instructionWord.LongValue)));

			return true;
		}

		private bool CheckContentsEditable()
		{
			if (ReadOnly || !this.noInstructionRendered && !this.instructionWord.IsEmpty)
				return true;

			base.Text = "";
			SetEditMode();

			return false;
		}

		private static bool ItemInErrors(InstanceValidationError[] errors, InstanceValidationError.Sources item)
			=> errors?.Any(error => error.Source == item) ?? false;

		private void MarkAssemblyError(int startCharIndex, int length)
		{
			int selectionStart = SelectionStart;
			int selectionLength = SelectionLength;

			if (length == 0)
				return;

			Select(startCharIndex, length);
			SelectionColor = this.errorTextColor;
			Select(selectionStart, selectionLength);
		}

		private void SetEditMode()
		{
			SuspendLayout();

			ForeColor = this.editingTextColor;

			if (TextLength > 0)
			{
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				Select(0, TextLength);
				SelectionColor = ForeColor;
				Select(selectionStart, selectionLength);
			}

			ResumeLayout();

			this.caption = null;
			this.toolTip?.SetToolTip(this, null);

			ContextMenuStrip = null;

			this.lastRenderedMagnitude = Unrendered;
			this.lastRenderedSign = Word.Signs.Positive;
			this.editMode = true;
		}

		private void ShowNonInstruction(string text)
		{
			this.noInstructionRendered = true;

			if (Focused && !CheckContentsEditable())
				return;

			SuspendLayout();
			base.Text = text;
			Select(0, TextLength);
			SelectionFont = new Font(Font.Name, Font.Size, FontStyle.Italic, Font.Unit, Font.GdiCharSet);

			ForeColor = this.immutableTextColor;
			SelectionLength = 0;

			Select(0, 0);
			ResumeLayout();
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			if (this.editMode)
				AssembleInstruction(true);

			e.Handled = true;
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != (char)Keys.Escape)
				return;

			e.Handled = true;
			this.editMode = false;
			Update();
		}

		private void This_LostFocus(object sender, EventArgs e)
		{
			if (this.editMode && AssembleInstruction(false))
				return;

			this.editMode = false;
			Update();
		}

		private void This_ReadOnlyChanged(object sender, EventArgs e)
		{
			if (Focused)
				CheckContentsEditable();
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (!this.updating && !this.editMode)
				SetEditMode();
		}

		public new string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;

				if (Focused)
					CheckContentsEditable();
			}
		}

		private int GetAddress()
		{
			MixByte[] addressBytes = new MixByte[MixInstruction.AddressByteCount];

			for (int i = 0; i < MixInstruction.AddressByteCount; i++)
				addressBytes[i] = this.instructionWord[i];

			var address = (int)Word.BytesToLong(addressBytes);

			if (this.instructionWord.Sign.IsNegative())
				address = -address;

			return address;
		}

		private void HandleNoInstruction(string caption)
		{
			ShowNonInstruction("No instruction");

			this.caption = caption;
			this.toolTip?.SetToolTip(this, caption);

			ContextMenuStrip = null;
			this.updating = false;
		}

		public new void Update()
		{
			var memoryFullWord = this.instructionWord as IMemoryFullWord;
			string sourceLine = memoryFullWord?.SourceLine;

			if (this.editMode || (this.lastRenderedMagnitude == this.instructionWord.MagnitudeLongValue
												&& this.lastRenderedSign == this.instructionWord.Sign
												&& this.lastRenderedSourceLine == sourceLine
												&& this.lastRenderedShowSourceLineToolTip == this.showShourceLineToolTip))
			{
				return;
			}

			this.updating = true;
			this.editMode = false;

			this.caption = null;
			this.toolTip?.SetToolTip(this, null);

			this.lastRenderedSourceLine = sourceLine;
			BackColor = this.lastRenderedSourceLine != null ? this.loadedInstructionBackgroundColor : this.editorBackgroundColor;

			this.lastRenderedShowSourceLineToolTip = this.showShourceLineToolTip;
			string sourceCaption = this.lastRenderedShowSourceLineToolTip && this.lastRenderedSourceLine != null ? "Original source:   " + this.lastRenderedSourceLine : null;

			this.lastRenderedMagnitude = this.instructionWord.MagnitudeLongValue;
			this.lastRenderedSign = this.instructionWord.Sign;
			var instruction = InstructionSet.Instance.GetInstruction(this.instructionWord[MixInstruction.OpcodeByte], new FieldSpec(this.instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				HandleNoInstruction(sourceCaption);
				return;
			}

			var instance = instruction.CreateInstance(this.instructionWord);
			var errors = instance.Validate();
			var text = new InstructionText(instance);

			SuspendLayout();
			base.Text = "";

			ForeColor = this.lastRenderedSourceLine != null ? this.loadedInstructionTextColor : this.renderedTextColor;
			AddTextElement(text.Mnemonic + " ", false);
			AddTextElement(text.Address, ItemInErrors(errors, InstanceValidationError.Sources.Address));
			AddTextElement(text.Index, ItemInErrors(errors, InstanceValidationError.Sources.Index));
			AddTextElement(text.Field, ItemInErrors(errors, InstanceValidationError.Sources.FieldSpec));

			if (TextLength > 0)
				Select(TextLength, 0);

			var caption = GetAddressErrorsCaption(errors);

			if (sourceCaption != null)
				caption = caption == null ? sourceCaption : string.Concat(sourceCaption, Environment.NewLine, caption);

			this.caption = caption;
			this.toolTip?.SetToolTip(this, caption);

			var address = GetAddress();
			this.showAddressMenuItem.Enabled = address >= MemoryMinIndex && address <= MemoryMaxIndex;
			this.showIndexedAddressMenuItem.Enabled = IndexedAddressCalculatorCallback?.Invoke(address, this.instructionWord[MixInstruction.IndexByte]) != int.MinValue;
			ContextMenuStrip = this.contextMenu;

			base.Update();
			ResumeLayout();

			this.noInstructionRendered = false;
			this.updating = false;
		}

		private static string GetAddressErrorsCaption(InstanceValidationError[] errors)
		{
			if (errors == null)
				return null;

			var captionBuilder = new StringBuilder();
			int errorNumber = 1;

			foreach (InstanceValidationError error in errors)
			{
				if (errorNumber != 1)
					captionBuilder.AppendFormat("{0}{1}. ", Environment.NewLine, errorNumber);

				if (errorNumber == 2)
					captionBuilder.Insert(0, "1. ");

				captionBuilder.AppendFormat("address: {0}", error.CompiledMessage);
				errorNumber++;
			}

			if (errorNumber == 2)
				captionBuilder.Insert(0, "Instruction error: ");

			else
				captionBuilder.Insert(0, $"Instruction errors:{Environment.NewLine}");

			return captionBuilder.ToString();
		}

		public void UpdateLayout()
		{
			SuspendLayout();
			this.updating = true;

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			this.editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			this.errorTextColor = GuiSettings.GetColor(GuiSettings.ErrorText);
			this.immutableTextColor = GuiSettings.GetColor(GuiSettings.ImmutableText);
			this.editorBackgroundColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			this.loadedInstructionTextColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionText);
			this.loadedInstructionBackgroundColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionBackground);
			ForeColor = this.lastRenderedSourceLine != null ? this.loadedInstructionTextColor : this.renderedTextColor;
			BackColor = this.lastRenderedSourceLine != null ? this.loadedInstructionBackgroundColor : this.editorBackgroundColor;

			this.updating = false;
			ResumeLayout();
		}

		public IFullWord InstructionWord
		{
			get => this.instructionWord;
			set
			{
				if (value != null)
				{
					this.instructionWord = value;
					this.editMode = false;

					Update();

					if (Focused)
					{
						CheckContentsEditable();
					}
				}
			}
		}

		public ToolTip ToolTip
		{
			get => this.toolTip;
			set
			{
				this.toolTip = value;
				this.toolTip?.SetToolTip(this, this.caption);
			}
		}

		public IWord WordValue
		{
			get => this.instructionWord;
			set
			{
				if (!(value is IFullWord))
				{
					throw new ArgumentException("value must be an IFullWord");
				}

				InstructionWord = (IFullWord)value;
			}
		}
	}
}
