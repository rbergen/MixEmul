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

		private Color _editingTextColor;
		private Color _errorTextColor;
		private Color _immutableTextColor;
		private Color _renderedTextColor;
		private Color _editorBackgroundColor;
		private Color _loadedInstructionTextColor;
		private Color _loadedInstructionBackgroundColor;

		private bool _editMode;
		private IFullWord _instructionWord;
		private long _lastRenderedMagnitude;
		private Word.Signs _lastRenderedSign;
		private ToolTip _toolTip;
		private bool _noInstructionRendered;
		private bool _updating;
		private string _lastRenderedSourceLine;
		private string _caption;

		private readonly ToolStripMenuItem _showAddressMenuItem;
		private readonly ToolStripMenuItem _showIndexedAddressMenuItem;
		private readonly ContextMenuStrip _contextMenu;

		public int MemoryMinIndex { get; set; }
		public int MemoryMaxIndex { get; set; }
		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback { get; set; } = null;
		public SymbolCollection Symbols { get; set; }
		public int MemoryAddress { get; set; }

		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public InstructionInstanceTextBox(IFullWord instructionWord = null)
		{
			_instructionWord = instructionWord ?? new FullWord();

			UpdateLayout();

			DetectUrls = false;
			TextChanged += This_TextChanged;
			KeyPress += This_KeyPress;
			KeyDown += This_KeyDown;
			LostFocus += This_LostFocus;
			Enter += This_Enter;
			ReadOnlyChanged += This_ReadOnlyChanged;

			_showAddressMenuItem = new ToolStripMenuItem("Show address", null, ShowAddressMenuItem_Click);

			_showIndexedAddressMenuItem = new ToolStripMenuItem("Show indexed address", null, ShowIndexedAddressMenuItem_Click);

			_contextMenu = new ContextMenuStrip();
			_contextMenu.Items.Add(_showAddressMenuItem);
			_contextMenu.Items.Add(_showIndexedAddressMenuItem);

			ContextMenuStrip = _contextMenu;

			_updating = false;
			_editMode = false;
			MemoryAddress = 0;
			_lastRenderedMagnitude = Unrendered;
			_lastRenderedSign = Word.Signs.Positive;
			_lastRenderedSourceLine = null;
			_noInstructionRendered = true;

			Update();
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
			=> OnAddressSelected(new AddressSelectedEventArgs(IndexedAddressCalculatorCallback(GetAddress(), _instructionWord[MixInstruction.IndexByte])));

		private void ShowAddressMenuItem_Click(object sender, EventArgs e)
			=> OnAddressSelected(new AddressSelectedEventArgs(GetAddress()));

		private void AddTextElement(string element, bool markAsError)
		{
			if (element == "")
				return;

			int textLength = TextLength;
			AppendText(element);
			Select(textLength, element.Length);
			SelectionColor = markAsError ? _errorTextColor : ForeColor;
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
			_editMode = false;

			var instance = Assembler.Assemble(Text, MemoryAddress, out ParsedSourceLine line, Symbols, out AssemblyFindingCollection findings);

			if (instance != null && !(instance is MixInstruction.Instance))
			{
				findings.Add(new AssemblyError(0, LineSection.OpField, 0, line.OpField.Length, new ValidationError("only MIX instruction mnemonics supported")));
				instance = null;
			}

			if (markErrors && findings.Count > 0)
			{
				_updating = true;

				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				SuspendLayout();

				base.Text = line.OpField + " " + line.AddressField;

				_caption = GetAssemblyErrorsCaption(line, findings);
				_toolTip?.SetToolTip(this, _caption);

				Select(selectionStart, selectionLength);
				ResumeLayout();

				_updating = false;
			}

			if (instance == null)
				return false;

			var oldValue = new FullWord(_instructionWord.LongValue);
			_instructionWord.LongValue = ((MixInstruction.Instance)instance).InstructionWord.LongValue;

			Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new FullWord(_instructionWord.LongValue)));

			return true;
		}

		private bool CheckContentsEditable()
		{
			if (ReadOnly || !_noInstructionRendered && !_instructionWord.IsEmpty)
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
			SelectionColor = _errorTextColor;
			Select(selectionStart, selectionLength);
		}

		private void SetEditMode()
		{
			SuspendLayout();

			ForeColor = _editingTextColor;

			if (TextLength > 0)
			{
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				Select(0, TextLength);
				SelectionColor = ForeColor;
				Select(selectionStart, selectionLength);
			}

			ResumeLayout();

			_caption = null;
			_toolTip?.SetToolTip(this, null);

			ContextMenuStrip = null;

			_lastRenderedMagnitude = Unrendered;
			_lastRenderedSign = Word.Signs.Positive;
			_editMode = true;
		}

		private void ShowNonInstruction(string text)
		{
			_noInstructionRendered = true;

			if (Focused && !CheckContentsEditable())
				return;

			SuspendLayout();
			base.Text = text;
			Select(0, TextLength);
			SelectionFont = new Font(Font.Name, Font.Size, FontStyle.Italic, Font.Unit, Font.GdiCharSet);

			ForeColor = _immutableTextColor;
			SelectionLength = 0;

			Select(0, 0);
			ResumeLayout();
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			if (_editMode)
				AssembleInstruction(true);

			e.Handled = true;
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != (char)Keys.Escape)
				return;

			e.Handled = true;
			_editMode = false;
			Update();
		}

		private void This_LostFocus(object sender, EventArgs e)
		{
			if (_editMode && AssembleInstruction(false))
				return;

			_editMode = false;
			Update();
		}

		private void This_ReadOnlyChanged(object sender, EventArgs e)
		{
			if (Focused)
				CheckContentsEditable();
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (!_updating && !_editMode)
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
				addressBytes[i] = _instructionWord[i];

			var address = (int)Word.BytesToLong(addressBytes);

			if (_instructionWord.Sign.IsNegative())
				address = -address;

			return address;
		}

		private void HandleNoInstruction(string caption)
		{
			ShowNonInstruction("No instruction");

			_caption = caption;
			_toolTip?.SetToolTip(this, caption);

			ContextMenuStrip = null;
			_updating = false;
		}

		public new void Update()
		{
			var memoryFullWord = _instructionWord as IMemoryFullWord;
			string sourceLine = memoryFullWord?.SourceLine;

			if (_editMode || (_lastRenderedMagnitude == _instructionWord.MagnitudeLongValue
												&& _lastRenderedSign == _instructionWord.Sign
												&& _lastRenderedSourceLine == sourceLine))
			{
				return;
			}

			_updating = true;
			_editMode = false;

			_caption = null;
			_toolTip?.SetToolTip(this, null);

			_lastRenderedSourceLine = sourceLine;
			BackColor = _lastRenderedSourceLine != null ? _loadedInstructionBackgroundColor : _editorBackgroundColor;

			string sourceCaption = _lastRenderedSourceLine != null ? "Original source:   " + _lastRenderedSourceLine : null;

			_lastRenderedMagnitude = _instructionWord.MagnitudeLongValue;
			_lastRenderedSign = _instructionWord.Sign;
			var instruction = InstructionSet.Instance.GetInstruction(_instructionWord[MixInstruction.OpcodeByte], new FieldSpec(_instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				HandleNoInstruction(sourceCaption);
				return;
			}

			var instance = instruction.CreateInstance(_instructionWord);
			var errors = instance.Validate();
			var text = new InstructionText(instance);

			SuspendLayout();
			base.Text = "";

			ForeColor = _lastRenderedSourceLine != null ? _loadedInstructionTextColor : _renderedTextColor;
			AddTextElement(text.Mnemonic + " ", false);
			AddTextElement(text.Address, ItemInErrors(errors, InstanceValidationError.Sources.Address));
			AddTextElement(text.Index, ItemInErrors(errors, InstanceValidationError.Sources.Index));
			AddTextElement(text.Field, ItemInErrors(errors, InstanceValidationError.Sources.FieldSpec));

			if (TextLength > 0)
				Select(TextLength, 0);

			var caption = GetAddressErrorsCaption(errors);

			if (sourceCaption != null)
				caption = caption == null ? sourceCaption : string.Concat(sourceCaption, Environment.NewLine, caption);

			_caption = caption;
			_toolTip?.SetToolTip(this, caption);

			var address = GetAddress();
			_showAddressMenuItem.Enabled = address >= MemoryMinIndex && address <= MemoryMaxIndex;
			_showIndexedAddressMenuItem.Enabled = IndexedAddressCalculatorCallback?.Invoke(address, _instructionWord[MixInstruction.IndexByte]) != int.MinValue;
			ContextMenuStrip = _contextMenu;

			base.Update();
			ResumeLayout();

			_noInstructionRendered = false;
			_updating = false;
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
				captionBuilder.Insert(0, String.Format("Instruction errors:{0}", Environment.NewLine));

			return captionBuilder.ToString();
		}

		public void UpdateLayout()
		{
			SuspendLayout();
			_updating = true;

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			_editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			_errorTextColor = GuiSettings.GetColor(GuiSettings.ErrorText);
			_immutableTextColor = GuiSettings.GetColor(GuiSettings.ImmutableText);
			_editorBackgroundColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			_loadedInstructionTextColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionText);
			_loadedInstructionBackgroundColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionBackground);
			ForeColor = _lastRenderedSourceLine != null ? _loadedInstructionTextColor : _renderedTextColor;
			BackColor = _lastRenderedSourceLine != null ? _loadedInstructionBackgroundColor : _editorBackgroundColor;

			_updating = false;
			ResumeLayout();
		}

		public IFullWord InstructionWord
		{
			get => _instructionWord;
			set
			{
				if (value != null)
				{
					_instructionWord = value;
					_editMode = false;

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
			get => _toolTip;
			set
			{
				_toolTip = value;
				_toolTip?.SetToolTip(this, _caption);
			}
		}

		public IWord WordValue
		{
			get => _instructionWord;
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
