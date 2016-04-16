using System;
using System.Drawing;
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
		private int mMemoryAddress;
		private bool mNoInstructionRendered;
		private ToolTip mToolTip;
		private bool mUpdating;
		private string mLastRenderedSourceLine;
		private SymbolCollection mSymbols;
		private int mMemoryMinIndex;
		private int mMemoryMaxIndex;

		private string mCaption;

		private MenuItem mShowAddressMenuItem;
		private MenuItem mShowIndexedAddressMenuItem;
		private ContextMenu mContextMenu;

		private IndexedAddressCalculatorCallback mIndexedAddressCalculatorCallback = null;

		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public InstructionInstanceTextBox()
			: this(null)
		{
		}

		public InstructionInstanceTextBox(IFullWord instructionWord)
		{
			if (instructionWord == null)
			{
				instructionWord = new FullWord();
			}

			mInstructionWord = instructionWord;

			UpdateLayout();

			base.DetectUrls = false;
			base.TextChanged += this_TextChanged;
			base.KeyPress += this_KeyPress;
			base.KeyDown += this_KeyDown;
			base.Leave += this_Leave;
			base.Enter += this_Enter;
			base.ReadOnlyChanged += this_ReadOnlyChanged;

			mShowAddressMenuItem = new MenuItem("Show address", showAddressMenuItem_Click);

			mShowIndexedAddressMenuItem = new MenuItem("Show indexed address", showIndexedAddressMenuItem_Click);

			mContextMenu = new ContextMenu();
			mContextMenu.MenuItems.Add(mShowAddressMenuItem);
			mContextMenu.MenuItems.Add(mShowIndexedAddressMenuItem);

			ContextMenu = mContextMenu;

			mUpdating = false;
			mEditMode = false;
			mMemoryAddress = 0;
			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;
			mLastRenderedSourceLine = null;
			mNoInstructionRendered = true;

			Update();
		}

		private void addTextElement(string element, bool markAsError)
		{
			if (element != "")
			{
				int textLength = TextLength;
				base.AppendText(element);
				base.Select(textLength, element.Length);
				base.SelectionColor = markAsError ? mErrorTextColor : ForeColor;
			}
		}

		private string getAssemblyErrorsCaption(ParsedSourceLine line, AssemblyFindingCollection findings)
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
							markAssemblyError(finding.StartCharIndex, finding.Length);
							fieldLabel = "mnemonic";

							break;

						case LineSection.AddressField:
							if (finding.Length != 0)
							{
								markAssemblyError((line.OpField.Length + finding.StartCharIndex) + 1, finding.Length);
							}
							else
							{
								markAssemblyError(line.OpField.Length + 1, line.AddressField.Length);
							}
							fieldLabel = "address";

							break;

						case LineSection.CommentField:

							continue;

						case LineSection.EntireLine:
							markAssemblyError(0, TextLength);
							fieldLabel = "instruction";

							break;
					}
				}

				if (findingNumber != 1)
				{
					caption = string.Concat(caption, Environment.NewLine, findingNumber, ". ");
				}

				if (findingNumber == 2)
				{
					caption = "1. " + caption;
				}

				caption = caption + fieldLabel + ": " + finding.Message;

				findingNumber++;
			}

			return findingNumber == 2
				? string.Concat("Instruction error: ", caption)
				: string.Concat("Instruction errors:", Environment.NewLine, caption);
		}

		private bool assembleInstruction(bool markErrors)
		{
			AssemblyFindingCollection findings;
			ParsedSourceLine line;
			mEditMode = false;

			InstructionInstanceBase instance = Assembler.Assemble(Text, mMemoryAddress, out line, mSymbols, out findings);

			if (instance != null && !(instance is MixInstruction.Instance))
			{
				findings.Add(new AssemblyError(0, LineSection.OpField, 0, line.OpField.Length, new ValidationError("only MIX instruction mnemonics supported")));
				instance = null;
			}

			if (markErrors && findings.Count > 0)
			{
				mUpdating = true;

				int selectionStart = base.SelectionStart;
				int selectionLength = SelectionLength;

				base.SuspendLayout();

				base.Text = line.OpField + " " + line.AddressField;

				mCaption = getAssemblyErrorsCaption(line, findings);
				if (mToolTip != null)
				{
					mToolTip.SetToolTip(this, mCaption);
				}

				base.Select(selectionStart, selectionLength);
				base.ResumeLayout();

				mUpdating = false;
			}

			if (instance == null)
			{
				return false;
			}

			FullWord oldValue = new FullWord(mInstructionWord.LongValue);
			mInstructionWord.LongValue = ((MixInstruction.Instance)instance).InstructionWord.LongValue;

			Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new FullWord(mInstructionWord.LongValue)));

			return true;
		}

		private bool checkContentsEditable()
		{
			if (!base.ReadOnly && (mNoInstructionRendered || mInstructionWord.IsEmpty))
			{
				base.Text = "";
				setEditMode();

				return false;
			}

			return true;
		}

		private bool itemInErrors(InstanceValidationError[] errors, InstanceValidationError.Sources item)
		{
			if (errors != null)
			{
				foreach (InstanceValidationError error in errors)
				{
					if (error.Source == item)
					{
						return true;
					}
				}
			}

			return false;
		}

		private void markAssemblyError(int startCharIndex, int length)
		{
			int selectionStart = base.SelectionStart;
			int selectionLength = SelectionLength;

			if (length != 0)
			{
				base.Select(startCharIndex, length);
				base.SelectionColor = mErrorTextColor;
				base.Select(selectionStart, selectionLength);
			}
		}

		protected void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
			}
		}

		protected void OnAddressSelected(AddressSelectedEventArgs args)
		{
			if (AddressSelected != null)
			{
				AddressSelected(this, args);
			}
		}

		private void setEditMode()
		{
			base.SuspendLayout();

			ForeColor = mEditingTextColor;

			if (TextLength > 0)
			{
				int selectionStart = base.SelectionStart;
				int selectionLength = SelectionLength;

				base.Select(0, TextLength);
				base.SelectionColor = ForeColor;
				base.Select(selectionStart, selectionLength);
			}

			base.ResumeLayout();

			mCaption = null;
			if (mToolTip != null)
			{
				mToolTip.SetToolTip(this, null);
			}

			ContextMenu = null;

			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;
			mEditMode = true;
		}

		private void showNonInstruction(string text)
		{
			mNoInstructionRendered = true;

			if (!Focused || checkContentsEditable())
			{
				base.SuspendLayout();
				base.Text = text;
				base.Select(0, TextLength);
				base.SelectionFont = new Font(Font.Name, Font.Size, FontStyle.Italic, Font.Unit, Font.GdiCharSet);

				ForeColor = mImmutableTextColor;
				SelectionLength = 0;

				base.Select(0, 0);
				base.ResumeLayout();
			}
		}

		private void this_Enter(object sender, EventArgs e)
		{
			checkContentsEditable();
		}

		private void this_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (mEditMode)
				{
					assembleInstruction(true);
				}
				e.Handled = true;
			}
		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				e.Handled = true;
				mEditMode = false;
				Update();
			}
		}

		private void this_Leave(object sender, EventArgs e)
		{
			if (!mEditMode || !assembleInstruction(false))
			{
				mEditMode = false;
				Update();
			}
		}

		private void this_ReadOnlyChanged(object sender, EventArgs e)
		{
			if (Focused)
			{
				checkContentsEditable();
			}
		}

		private void this_TextChanged(object sender, EventArgs e)
		{
			if (!mUpdating && !mEditMode)
			{
				setEditMode();
			}
		}

		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				if (Focused)
				{
					checkContentsEditable();
				}
			}
		}

		private void showIndexedAddressMenuItem_Click(object sender, EventArgs e)
		{
			OnAddressSelected(new AddressSelectedEventArgs(mIndexedAddressCalculatorCallback(getAddress(), mInstructionWord[MixInstruction.IndexByte])));
		}

		private int getAddress()
		{
			MixByte[] addressBytes = new MixByte[MixInstruction.AddressByteCount];

			for (int i = 0; i < MixInstruction.AddressByteCount; i++)
			{
				addressBytes[i] = mInstructionWord[i];
			}

			int address = (int)Word.BytesToLong(addressBytes);

			if (mInstructionWord.Sign == MixLib.Type.Word.Signs.Negative)
			{
				address = -address;
			}

			return address;
		}

		private void showAddressMenuItem_Click(object sender, EventArgs e)
		{
			OnAddressSelected(new AddressSelectedEventArgs(getAddress()));
		}

		private void handleNoInstructionSet()
		{
			showNonInstruction("No instructionset loaded");

			ContextMenu = null;
			mUpdating = false;
			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;
		}

		private void handleNoInstruction(string caption)
		{
			showNonInstruction("No instruction");

			mCaption = caption;
			if (mToolTip != null)
			{
				mToolTip.SetToolTip(this, caption);
			}

			ContextMenu = null;
			mUpdating = false;
		}

		public new void Update()
		{
			IMemoryFullWord memoryFullWord = mInstructionWord as IMemoryFullWord;
			string sourceLine = memoryFullWord != null ? memoryFullWord.SourceLine : null;

			if (mEditMode || (mLastRenderedMagnitude == mInstructionWord.MagnitudeLongValue
												&& mLastRenderedSign == mInstructionWord.Sign
												&& mLastRenderedSourceLine == sourceLine))
			{
				return;
			}

			mUpdating = true;
			mEditMode = false;

			mCaption = null;
			if (mToolTip != null)
			{
				mToolTip.SetToolTip(this, null);
			}

			mLastRenderedSourceLine = sourceLine;
			BackColor = mLastRenderedSourceLine != null ? mLoadedInstructionBackgroundColor : mEditorBackgroundColor;

			string sourceCaption = mLastRenderedSourceLine != null ? "Original source:   " + mLastRenderedSourceLine : null;

			mLastRenderedMagnitude = mInstructionWord.MagnitudeLongValue;
			mLastRenderedSign = mInstructionWord.Sign;
			MixInstruction instruction = InstructionSet.Instance.GetInstruction((byte)mInstructionWord[MixInstruction.OpcodeByte], new FieldSpec(mInstructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				handleNoInstruction(sourceCaption);
				return;
			}

			MixInstruction.Instance instance = instruction.CreateInstance(mInstructionWord);
			InstanceValidationError[] errors = instance.Validate();
			InstructionText text = new InstructionText(instance);

			base.SuspendLayout();
			base.Text = "";

			ForeColor = mLastRenderedSourceLine != null ? mLoadedInstructionTextColor : mRenderedTextColor;
			addTextElement(text.Mnemonic + " ", false);
			addTextElement(text.Address, itemInErrors(errors, InstanceValidationError.Sources.Address));
			addTextElement(text.Index, itemInErrors(errors, InstanceValidationError.Sources.Index));
			addTextElement(text.Field, itemInErrors(errors, InstanceValidationError.Sources.FieldSpec));

			if (TextLength > 0)
			{
				base.Select(TextLength, 0);
			}

			string caption = getAddressErrorsCaption(errors);

			if (sourceCaption != null)
			{
				caption = caption == null ? sourceCaption : string.Concat(sourceCaption, Environment.NewLine, caption);
			}

			mCaption = caption;
			if (mToolTip != null)
			{
				mToolTip.SetToolTip(this, caption);
			}

			int address = getAddress();
			mShowAddressMenuItem.Enabled = address >= mMemoryMinIndex && address <= mMemoryMaxIndex;
			mShowIndexedAddressMenuItem.Enabled = mIndexedAddressCalculatorCallback != null && mIndexedAddressCalculatorCallback(address, mInstructionWord[MixInstruction.IndexByte]) != int.MinValue;
			ContextMenu = mContextMenu;

			base.Update();
			base.ResumeLayout();

			mNoInstructionRendered = false;
			mUpdating = false;
		}

		private string getAddressErrorsCaption(InstanceValidationError[] errors)
		{
			if (errors == null)
			{
				return null;
			}

			string caption = "";
			int errorNumber = 1;

			foreach (InstanceValidationError error in errors)
			{
				if (errorNumber != 1)
				{
					caption = string.Concat(caption, Environment.NewLine, errorNumber, ". ");
				}

				if (errorNumber == 2)
				{
					caption = "1. " + caption;
				}

				caption = caption + "address: " + error.CompiledMessage;
				errorNumber++;
			}

			return errorNumber == 2
				? string.Concat("Instruction error: ", caption)
				: string.Concat("Instruction errors:", Environment.NewLine, caption);
		}


		public int MemoryMinIndex
		{
			get
			{
				return mMemoryMinIndex;
			}
			set
			{
				mMemoryMinIndex = value;
			}
		}

		public int MemoryMaxIndex
		{
			get
			{
				return mMemoryMaxIndex;
			}
			set
			{
				mMemoryMaxIndex = value;
			}
		}

		public void UpdateLayout()
		{
			base.SuspendLayout();
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
			base.ResumeLayout();
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get
			{
				return mIndexedAddressCalculatorCallback;
			}
			set
			{
				mIndexedAddressCalculatorCallback = value;
			}
		}

		public IFullWord InstructionWord
		{
			get
			{
				return mInstructionWord;
			}
			set
			{
				if (value != null)
				{
					mInstructionWord = value;
					mEditMode = false;

					Update();

					if (Focused)
					{
						checkContentsEditable();
					}
				}
			}
		}

		public int MemoryAddress
		{
			get
			{
				return mMemoryAddress;
			}
			set
			{
				mMemoryAddress = value;
			}
		}

		public SymbolCollection Symbols
		{
			get
			{
				return mSymbols;
			}
			set
			{
				mSymbols = value;
			}
		}

		public ToolTip ToolTip
		{
			get
			{
				return mToolTip;
			}
			set
			{
				mToolTip = value;
				if (mToolTip != null)
				{
					mToolTip.SetToolTip(this, mCaption);
				}
			}
		}

		public IWord WordValue
		{
			get
			{
				return mInstructionWord;
			}
			set
			{
				if (!(value is IFullWord))
				{
					throw new ArgumentException("value must be an IFullWord");
				}

				InstructionWord = (IFullWord)value;
			}
		}

		public Control EditorControl
		{
			get
			{
				return this;
			}
		}

		public bool Focus(FieldTypes? field, int? index)
		{
			return this.FocusWithIndex(index);
		}

		public FieldTypes? FocusedField
		{
			get
			{
				return FieldTypes.Instruction;
			}
		}

		public int? CaretIndex
		{
			get
			{
				return SelectionStart + SelectionLength;
			}
		}
	}
}
