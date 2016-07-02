using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
    public class WordEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
        const long unrendered = long.MinValue;

        MixByteTextBox[] mByteTextBoxes;
        int mFocusByte;
        int mFocusByteSelLength;
        int mFocusByteSelStart;
        bool mIncludeSign;
        long mLastRenderedMagnitude;
        Word.Signs mLastRenderedSign;
        bool mReadOnly;
        Button mSignButton;
        IWord mWord;

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
			mSignButton.Click += mSignButton_Click;
			mSignButton.KeyPress += editor_keyPress;
			mSignButton.KeyDown += keyDown;

			initializeComponent();
		}

        public int? CaretIndex => null;

        public Control EditorControl => this;

        public bool Focus(FieldTypes? field, int? index) =>
            ByteCount > 0 ? mByteTextBoxes[field == FieldTypes.LastByte ? ByteCount - 1 : 0].Focus() : mSignButton.Focus();

        protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        void byteValueChanged(MixByteTextBox textBox, MixByteTextBox.ValueChangedEventArgs args)
        {
            var oldValue = new Word(mWord.Magnitude, mWord.Sign);
            mWord[textBox.Index] = args.NewValue;
            mLastRenderedMagnitude = mWord.MagnitudeLongValue;
            mLastRenderedSign = mWord.Sign;

            var newValue = new Word(mWord.Magnitude, mWord.Sign);
            OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
        }


        void editor_Click(object sender, EventArgs e)
        {
            var box = (MixByteTextBox)sender;

            if (box.SelectionLength == 0) box.Select(0, 2);
        }

        void editor_keyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (IncludeSign && (keyChar == '-' || (keyChar == '+' && mWord.Sign.IsNegative()))) negateSign();
        }

        void editor_Leave(object sender, EventArgs e)
        {
            var box = (MixByteTextBox)sender;
            mFocusByte = box.Index;
            mFocusByteSelStart = box.SelectionStart;
            mFocusByteSelLength = box.SelectionLength;
        }

        void initializeComponent()
        {
            mFocusByte = 0;
            mFocusByteSelStart = 0;
            mFocusByteSelLength = 0;
            int tabIndex = 0;
            int width = 0;

            SuspendLayout();
            if (IncludeSign)
            {
                Controls.Add(mSignButton);
                tabIndex++;
                width += mSignButton.Width - 1;
            }

            mByteTextBoxes = new MixByteTextBox[ByteCount];
            for (int i = 0; i < ByteCount; i++)
            {
                var box = new MixByteTextBox(i);
                box.Location = new Point(width, 0);
                box.Name = "MixByteEditor" + i;
                box.MixByteValue = mWord[i];
                box.ReadOnly = ReadOnly;
                box.TabIndex = tabIndex;
                box.ValueChanged += byteValueChanged;
                box.Click += editor_Click;
                box.Leave += editor_Leave;
                box.KeyPress += editor_keyPress;
                box.KeyDown += keyDown;

                mByteTextBoxes[i] = box;

                Controls.Add(box);

                tabIndex++;
                width += box.Width - 1;
            }

            Name = "WordEditor";
            Size = new Size(width + 1, 21);
            ResumeLayout(false);
            Update();
        }

        void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers != Keys.None) return;

            MixByteTextBox box;

            switch (e.KeyCode)
            {
                case Keys.Prior:
                case Keys.Next:
                case Keys.Up:
                case Keys.Down:
                    NavigationKeyDown?.Invoke(this, e);
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
                        else
                        {
                            NavigationKeyDown?.Invoke(this, e);
                        }
                    }

                    break;

                case Keys.Left:
                    if (sender == mSignButton)
                    {
                        NavigationKeyDown?.Invoke(this, e);

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

        void mSignButton_Click(object sender, EventArgs e)
        {
            negateSign();
            if (mByteTextBoxes.Length > 0)
            {
                mByteTextBoxes[mFocusByte].Focus();
                mByteTextBoxes[mFocusByte].SelectionStart = mFocusByteSelStart;
                mByteTextBoxes[mFocusByte].SelectionLength = mFocusByteSelLength;
            }
        }

        void negateSign()
        {
            Word.Signs sign = mWord.Sign;
            mWord.InvertSign();
            mLastRenderedMagnitude = mWord.MagnitudeLongValue;
            mLastRenderedSign = mWord.Sign;
            mSignButton.Text = "" + mWord.Sign.ToChar();

            var oldValue = new Word(mWord.Magnitude, sign);
            var newValue = new Word(mWord.Magnitude, mWord.Sign);
            OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
        }

        void reinitializeComponent()
        {
            Controls.Clear();

            foreach (MixByteTextBox box in mByteTextBoxes) box.Dispose();

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
			foreach (MixByteTextBox box in mByteTextBoxes) box.UpdateLayout();
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

		public FieldTypes? FocusedField
		{
			get
			{
				if (mSignButton.Focused) return FieldTypes.Word;

				foreach (MixByteTextBox byteBox in mByteTextBoxes)
				{
					if (byteBox.Focused) return FieldTypes.Word;
				}

				return null;
			}
		}

		public void Select(int start, int length) { }
	}
}
