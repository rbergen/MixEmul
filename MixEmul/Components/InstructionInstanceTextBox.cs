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
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class InstructionInstanceTextBox : RichTextBox, IWordEditor, IEscapeConsumer
	{
		private const long unrendered = long.MinValue;

		private Color mEditingTextColor;
		private Color mErrorTextColor;
		private Color mImmutableTextColor;
		private Color mRenderedTextColor;
		private Color mEditorBackgroundColor;
		private Color mLoadedInstructionTextColor;
		private Color mLoadedInstructionBackgroundColor;

		private bool mEditMode;
		private IFullWord mInstructionWord;
		private long mLastRenderedMagnitude;
		private Word.Signs mLastRenderedSign;
		private ToolTip mToolTip;
		private bool mNoInstructionRendered;
		private bool mUpdating;
		private string mLastRenderedSourceLine;
		private string mCaption;

		private readonly ToolStripMenuItem mShowAddressMenuItem;
		private readonly ToolStripMenuItem mShowIndexedAddressMenuItem;
		private readonly ContextMenuStrip mContextMenu;

		public int MemoryMinIndex { get; set; }
		public int MemoryMaxIndex { get; set; }
		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback { get; set; } = null;
		public SymbolCollection Symbols { get; set; }
		public int MemoryAddress { get; set; }

		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public InstructionInstanceTextBox() : this(null) {}

		public InstructionInstanceTextBox(IFullWord instructionWord)
		{
			mInstructionWord = instructionWord ?? new FullWord();

			UpdateLayout();

			DetectUrls = false;
			TextChanged += This_TextChanged;
			KeyPress += This_KeyPress;
			KeyDown += This_KeyDown;
			LostFocus += This_LostFocus;
			Enter += This_Enter;
			ReadOnlyChanged += This_ReadOnlyChanged;

			mShowAddressMenuItem = new ToolStripMenuItem("Show address", null, ShowAddressMenuItem_Click);

			mShowIndexedAddressMenuItem = new ToolStripMenuItem("Show indexed address", null, ShowIndexedAddressMenuItem_Click);

			mContextMenu = new ContextMenuStrip();
			mContextMenu.Items.Add(mShowAddressMenuItem);
			mContextMenu.Items.Add(mShowIndexedAddressMenuItem);

			ContextMenuStrip = mContextMenu;

			mUpdating = false;
			mEditMode = false;
			MemoryAddress = 0;
			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;
			mLastRenderedSourceLine = null;
			mNoInstructionRendered = true;

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
			=> OnAddressSelected(new AddressSelectedEventArgs(IndexedAddressCalculatorCallback(GetAddress(), mInstructionWord[MixInstruction.IndexByte])));

		private void ShowAddressMenuItem_Click(object sender, EventArgs e) 
			=> OnAddressSelected(new AddressSelectedEventArgs(GetAddress()));

		private void AddTextElement(string element, bool markAsError)
		{
			if (element != "")
			{
				int textLength = TextLength;
				AppendText(element);
				Select(textLength, element.Length);
				SelectionColor = markAsError ? mErrorTextColor : ForeColor;
			}
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

				caption = caption + fieldLabel + ": " + finding.Message;

				findingNumber++;
			}

			return findingNumber == 2
					? string.Concat("Instruction error: ", caption)
					: string.Concat("Instruction errors:", Environment.NewLine, caption);
		}

		private bool AssembleInstruction(bool markErrors)
		{
			mEditMode = false;

			var instance = Assembler.Assemble(Text, MemoryAddress, out ParsedSourceLine line, Symbols, out AssemblyFindingCollection findings);

			if (instance != null && !(instance is MixInstruction.Instance))
			{
				findings.Add(new AssemblyError(0, LineSection.OpField, 0, line.OpField.Length, new ValidationError("only MIX instruction mnemonics supported")));
				instance = null;
			}

			if (markErrors && findings.Count > 0)
			{
				mUpdating = true;

				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				SuspendLayout();

				base.Text = line.OpField + " " + line.AddressField;

				mCaption = GetAssemblyErrorsCaption(line, findings);
				mToolTip?.SetToolTip(this, mCaption);

				Select(selectionStart, selectionLength);
				ResumeLayout();

				mUpdating = false;
			}

			if (instance == null)
				return false;

			var oldValue = new FullWord(mInstructionWord.LongValue);
			mInstructionWord.LongValue = ((MixInstruction.Instance)instance).InstructionWord.LongValue;

			Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new FullWord(mInstructionWord.LongValue)));

			return true;
		}

		private bool CheckContentsEditable()
		{
			if (ReadOnly || !mNoInstructionRendered && !mInstructionWord.IsEmpty)
				return true;

			base.Text = "";
			SetEditMode();

			return false;
		}

		static bool ItemInErrors(InstanceValidationError[] errors, InstanceValidationError.Sources item) 
			=> errors?.Any(error => error.Source == item) ?? false;

		private void MarkAssemblyError(int startCharIndex, int length)
		{
			int selectionStart = SelectionStart;
			int selectionLength = SelectionLength;

			if (length == 0)
				return;

			Select(startCharIndex, length);
			SelectionColor = mErrorTextColor;
			Select(selectionStart, selectionLength);
		}

		private void SetEditMode()
		{
			SuspendLayout();

			ForeColor = mEditingTextColor;

			if (TextLength > 0)
			{
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;

				Select(0, TextLength);
				SelectionColor = ForeColor;
				Select(selectionStart, selectionLength);
			}

			ResumeLayout();

			mCaption = null;
			mToolTip?.SetToolTip(this, null);

			ContextMenuStrip = null;

			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;
			mEditMode = true;
		}

		private void ShowNonInstruction(string text)
		{
			mNoInstructionRendered = true;

			if (Focused && !CheckContentsEditable())
				return;

			SuspendLayout();
			base.Text = text;
			Select(0, TextLength);
			SelectionFont = new Font(Font.Name, Font.Size, FontStyle.Italic, Font.Unit, Font.GdiCharSet);

			ForeColor = mImmutableTextColor;
			SelectionLength = 0;

			Select(0, 0);
			ResumeLayout();
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			if (mEditMode)
				AssembleInstruction(true);

			e.Handled = true;
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != (char)Keys.Escape)
				return;

			e.Handled = true;
			mEditMode = false;
			Update();
		}

		private void This_LostFocus(object sender, EventArgs e)
		{
			if (mEditMode && AssembleInstruction(false))
				return;

			mEditMode = false;
			Update();
		}

		private void This_ReadOnlyChanged(object sender, EventArgs e)
		{
			if (Focused)
				CheckContentsEditable();
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (!mUpdating && !mEditMode)
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
				addressBytes[i] = mInstructionWord[i];

			var address = (int)Word.BytesToLong(addressBytes);

			if (mInstructionWord.Sign.IsNegative())
				address = -address;

			return address;
		}

		private void HandleNoInstruction(string caption)
		{
			ShowNonInstruction("No instruction");

			mCaption = caption;
			mToolTip?.SetToolTip(this, caption);

			ContextMenuStrip = null;
			mUpdating = false;
		}

		public new void Update()
		{
			var memoryFullWord = mInstructionWord as IMemoryFullWord;
			string sourceLine = memoryFullWord?.SourceLine;

			if (mEditMode || (mLastRenderedMagnitude == mInstructionWord.MagnitudeLongValue
												&& mLastRenderedSign == mInstructionWord.Sign
												&& mLastRenderedSourceLine == sourceLine))
			{
				return;
			}

			mUpdating = true;
			mEditMode = false;

			mCaption = null;
			mToolTip?.SetToolTip(this, null);

			mLastRenderedSourceLine = sourceLine;
			BackColor = mLastRenderedSourceLine != null ? mLoadedInstructionBackgroundColor : mEditorBackgroundColor;

			string sourceCaption = mLastRenderedSourceLine != null ? "Original source:   " + mLastRenderedSourceLine : null;

			mLastRenderedMagnitude = mInstructionWord.MagnitudeLongValue;
			mLastRenderedSign = mInstructionWord.Sign;
			var instruction = InstructionSet.Instance.GetInstruction(mInstructionWord[MixInstruction.OpcodeByte], new FieldSpec(mInstructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				HandleNoInstruction(sourceCaption);
				return;
			}

			var instance = instruction.CreateInstance(mInstructionWord);
			var errors = instance.Validate();
			var text = new InstructionText(instance);

			SuspendLayout();
			base.Text = "";

			ForeColor = mLastRenderedSourceLine != null ? mLoadedInstructionTextColor : mRenderedTextColor;
			AddTextElement(text.Mnemonic + " ", false);
			AddTextElement(text.Address, ItemInErrors(errors, InstanceValidationError.Sources.Address));
			AddTextElement(text.Index, ItemInErrors(errors, InstanceValidationError.Sources.Index));
			AddTextElement(text.Field, ItemInErrors(errors, InstanceValidationError.Sources.FieldSpec));

			if (TextLength > 0)
				Select(TextLength, 0);

			var caption = GetAddressErrorsCaption(errors);

			if (sourceCaption != null)
				caption = caption == null ? sourceCaption : string.Concat(sourceCaption, Environment.NewLine, caption);

			mCaption = caption;
			mToolTip?.SetToolTip(this, caption);

			var address = GetAddress();
			mShowAddressMenuItem.Enabled = address >= MemoryMinIndex && address <= MemoryMaxIndex;
			mShowIndexedAddressMenuItem.Enabled = IndexedAddressCalculatorCallback?.Invoke(address, mInstructionWord[MixInstruction.IndexByte]) != int.MinValue;
			ContextMenuStrip = mContextMenu;

			base.Update();
			ResumeLayout();

			mNoInstructionRendered = false;
			mUpdating = false;
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
			mUpdating = true;

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			mErrorTextColor = GuiSettings.GetColor(GuiSettings.ErrorText);
			mImmutableTextColor = GuiSettings.GetColor(GuiSettings.ImmutableText);
			mEditorBackgroundColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mLoadedInstructionTextColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionText);
			mLoadedInstructionBackgroundColor = GuiSettings.GetColor(GuiSettings.LoadedInstructionBackground);
			ForeColor = mLastRenderedSourceLine != null ? mLoadedInstructionTextColor : mRenderedTextColor;
			BackColor = mLastRenderedSourceLine != null ? mLoadedInstructionBackgroundColor : mEditorBackgroundColor;

			mUpdating = false;
			ResumeLayout();
		}

		public IFullWord InstructionWord
		{
			get => mInstructionWord;
			set
			{
				if (value != null)
				{
					mInstructionWord = value;
					mEditMode = false;

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
			get => mToolTip;
			set
			{
				mToolTip = value;
				mToolTip?.SetToolTip(this, mCaption);
			}
		}

		public IWord WordValue
		{
			get => mInstructionWord;
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
