using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public class WordEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
		private const long unrendered = long.MinValue;

		private Container components = null;
		private MixByteTextBox[] mByteTextBoxes;
		private int mFocusByte;
		private int mFocusByteSelLength;
		private int mFocusByteSelStart;
		private bool mIncludeSign;
		private long mLastRenderedMagnitude;
		private Word.Signs mLastRenderedSign;
		private bool mReadOnly;
		private Button mSignButton;
		private IWord mWord;

		public event KeyEventHandler NavigationKeyDown;

		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordEditor()
			: this(FullWord.ByteCount)
		{
		}

		public WordEditor(IWord word)
			: this(word, true)
		{
		}

		public WordEditor(int byteCount)
			: this(byteCount, true)
		{
		}

		public WordEditor(IWord word, bool includeSign)
			: this(word.ByteCount, includeSign)
		{
			WordValue = word;
		}

		public WordEditor(int byteCount, bool includeSign)
		{
			mWord = new Word(byteCount);
			mIncludeSign = includeSign;
			mReadOnly = false;
			mLastRenderedMagnitude = unrendered;
			mLastRenderedSign = Word.Signs.Positive;

			mSignButton = new Button();
			mSignButton.Location = new Point(0, 0);
			mSignButton.Name = "SignButton";
			mSignButton.Size = new Size(18, 21);
			mSignButton.TabIndex = 0;
			mSignButton.TabStop = false;
			mSignButton.Text = "" + mWord.Sign.ToChar();
			mSignButton.FlatStyle = FlatStyle.Flat;
			mSignButton.Enabled = !ReadOnly;
			mSignButton.Click += new EventHandler(mSignButton_Click);
			mSignButton.KeyPress += new KeyPressEventHandler(editor_keyPress);
			mSignButton.KeyDown += new KeyEventHandler(keyDown);

			initializeComponent();
		}

		private void byteValueChanged(MixByteTextBox textBox, MixByteTextBox.ValueChangedEventArgs args)
		{
			Word oldValue = new Word(mWord.Magnitude, mWord.Sign);
			mWord[textBox.Index] = args.NewValue;
			mLastRenderedMagnitude = mWord.MagnitudeLongValue;
			mLastRenderedSign = mWord.Sign;

			Word newValue = new Word(mWord.Magnitude, mWord.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void editor_Click(object sender, EventArgs e)
		{
			MixByteTextBox box = (MixByteTextBox)sender;

			if (box.SelectionLength == 0)
			{
				box.Select(0, 2);
			}
		}

		private void editor_keyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;

			if (IncludeSign && (keyChar == '-' || (keyChar == '+' && mWord.Sign.IsNegative())))
			{
				negateSign();
			}
		}

		private void editor_Leave(object sender, EventArgs e)
		{
			MixByteTextBox box = (MixByteTextBox)sender;
			mFocusByte = box.Index;
			mFocusByteSelStart = box.SelectionStart;
			mFocusByteSelLength = box.SelectionLength;
		}

		public bool Focus(FieldTypes? field, int? index)
		{
			if (ByteCount > 0)
			{
				return mByteTextBoxes[field == FieldTypes.LastByte ? ByteCount - 1 : 0].Focus();
			}
			else
			{
				return mSignButton.Focus();
			}
		}

		private void initializeComponent()
		{
			mFocusByte = 0;
			mFocusByteSelStart = 0;
			mFocusByteSelLength = 0;
			int tabIndex = 0;
			int width = 0;

			base.SuspendLayout();
			if (IncludeSign)
			{
				base.Controls.Add(mSignButton);
				tabIndex++;
				width += mSignButton.Width - 1;
			}

			mByteTextBoxes = new MixByteTextBox[ByteCount];
			for (int i = 0; i < ByteCount; i++)
			{
				MixByteTextBox box = new MixByteTextBox(i);
				box.Location = new Point(width, 0);
				box.Name = "MixByteEditor" + i;
				box.MixByteValue = mWord[i];
				box.ReadOnly = ReadOnly;
				box.TabIndex = tabIndex;
				box.ValueChanged += new MixByteTextBox.ValueChangedEventHandler(byteValueChanged);
				box.Click += new EventHandler(editor_Click);
				box.Leave += new EventHandler(editor_Leave);
				box.KeyPress += new KeyPressEventHandler(editor_keyPress);
				box.KeyDown += new KeyEventHandler(keyDown);

				mByteTextBoxes[i] = box;

				base.Controls.Add(box);

				tabIndex++;
				width += box.Width - 1;
			}

			base.Name = "WordEditor";
			base.Size = new Size(width + 1, 21);
			base.ResumeLayout(false);
			Update();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			MixByteTextBox box;

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					if (NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}
					break;

				case Keys.Right:
					if (sender == mSignButton && ByteCount > 0)
					{
						mByteTextBoxes[0].Focus();
						mByteTextBoxes[0].Select(0, 2);
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == box.TextLength)
					{

						int byteIndex = Array.IndexOf(mByteTextBoxes, box);
						if (byteIndex < ByteCount - 1)
						{
							mByteTextBoxes[byteIndex + 1].Focus();
							mByteTextBoxes[byteIndex + 1].Select(0, 2);
						}
						else if (NavigationKeyDown != null)
						{
							NavigationKeyDown(this, e);
						}
					}

					break;

				case Keys.Left:
					if (sender == mSignButton)
					{
						if (NavigationKeyDown != null)
						{
							NavigationKeyDown(this, e);
						}

						e.Handled = true;
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == 0)
					{
						int byteIndex = Array.IndexOf(mByteTextBoxes, box);
						if (byteIndex > 0)
						{
							mByteTextBoxes[byteIndex - 1].Focus();
							mByteTextBoxes[byteIndex - 1].Select(0, 2);
						}
						else
						{
							mSignButton.Focus();
						}
					}

					break;
			}
		}

		private void mSignButton_Click(object sender, EventArgs e)
		{
			negateSign();
			if (mByteTextBoxes.Length > 0)
			{
				mByteTextBoxes[mFocusByte].Focus();
				mByteTextBoxes[mFocusByte].SelectionStart = mFocusByteSelStart;
				mByteTextBoxes[mFocusByte].SelectionLength = mFocusByteSelLength;
			}
		}

		private void negateSign()
		{
			Word.Signs sign = mWord.Sign;
			mWord.InvertSign();
			mLastRenderedMagnitude = mWord.MagnitudeLongValue;
			mLastRenderedSign = mWord.Sign;
            mSignButton.Text = "" + mWord.Sign.ToChar();

			Word oldValue = new Word(mWord.Magnitude, sign);
			Word newValue = new Word(mWord.Magnitude, mWord.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
			}
		}

		private void reinitializeComponent()
		{
			base.Controls.Clear();

			foreach (MixByteTextBox box in mByteTextBoxes)
			{
				box.Dispose();
			}

			initializeComponent();
		}

		public new void Update()
		{
			if (mLastRenderedMagnitude != mWord.MagnitudeLongValue || mLastRenderedSign != mWord.Sign || mLastRenderedMagnitude == unrendered)
			{
                mSignButton.Text = "" + mWord.Sign.ToChar();

				foreach (MixByteTextBox box in mByteTextBoxes)
				{
					box.MixByteValue = mWord[box.Index];
				}

				base.Update();
				mLastRenderedMagnitude = mWord.MagnitudeLongValue;
				mLastRenderedSign = mWord.Sign;
			}
		}

		public void UpdateLayout()
		{
			foreach (MixByteTextBox box in mByteTextBoxes)
			{
				box.UpdateLayout();
			}
		}

		public int ByteCount
		{
			get
			{
				return mWord.ByteCount;
			}
			set
			{
				if (ByteCount != value)
				{
					mWord = new Word(value);
					reinitializeComponent();
				}
			}
		}

		public bool IncludeSign
		{
			get
			{
				return mIncludeSign;
			}
			set
			{
				if (mIncludeSign != value)
				{
					mIncludeSign = value;
					reinitializeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get
			{
				return mReadOnly;
			}
			set
			{
				if (ReadOnly != value)
				{
					mReadOnly = value;
					mSignButton.Enabled = !ReadOnly;

					foreach (MixByteTextBox box in mByteTextBoxes)
					{
						box.ReadOnly = ReadOnly;
					}
				}
			}
		}

		public IWord WordValue
		{
			get
			{
				return mWord;
			}
			set
			{
				int byteCount = ByteCount;
				mWord = value;

				if (byteCount == value.ByteCount)
				{
					Update();
				}
				else
				{
					reinitializeComponent();
				}
			}
		}

		public Control EditorControl
		{
			get
			{
				return this;
			}
		}

		public FieldTypes? FocusedField
		{
			get
			{
				if (mSignButton.Focused)
				{
					return FieldTypes.Word;
				}

				foreach (MixByteTextBox byteBox in mByteTextBoxes)
				{
					if (byteBox.Focused)
					{
						return FieldTypes.Word;
					}
				}

				return null;
			}
		}

		public int? CaretIndex
		{
			get
			{
				return null;
			}
		}

		public void Select(int start, int length) { }
	}
}
