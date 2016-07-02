using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
    public class WordValueEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
        Label mEqualsLabel;
        bool mIncludeSign;
        readonly LongValueTextBox mLongValueTextBox;
        bool mReadOnly;
        int mTextBoxWidth;
        readonly WordEditor mWordEditor;

        public event KeyEventHandler NavigationKeyDown;

		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordValueEditor()
			: this(FullWord.ByteCount, true)
		{
		}

		public WordValueEditor(IWord word)
			: this(word, true)
		{
		}

		public WordValueEditor(int byteCount)
			: this(byteCount, true)
		{
		}

		public WordValueEditor(IWord word, bool includeSign)
			: this(word.ByteCount, includeSign)
		{
			WordValue = word;
		}

		public WordValueEditor(int byteCount, bool includeSign)
		{
			mIncludeSign = includeSign;
			mReadOnly = false;
			mTextBoxWidth = 80;

			mWordEditor = new WordEditor(byteCount, includeSign);
			mEqualsLabel = new Label();
			mLongValueTextBox = new LongValueTextBox(includeSign);

			mWordEditor.Name = "WordEditor";
			mWordEditor.Location = new Point(0, 0);
			mWordEditor.ReadOnly = mReadOnly;
			mWordEditor.TabIndex = 0;
			mWordEditor.TabStop = true;
			mWordEditor.ValueChanged += wordEditor_ValueChanged;
			mWordEditor.NavigationKeyDown += keyDown;

			mEqualsLabel.Name = "EqualsLabel";
			mEqualsLabel.TabIndex = 1;
			mEqualsLabel.TabStop = false;
			mEqualsLabel.Text = "=";
			mEqualsLabel.Size = new Size(10, 19);

			mLongValueTextBox.Name = "LongValueTextBox";
			mLongValueTextBox.ReadOnly = mReadOnly;
			mLongValueTextBox.SupportNegativeZero = true;
			mLongValueTextBox.TabIndex = 2;
			mLongValueTextBox.TabStop = true;
			mLongValueTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			mLongValueTextBox.ValueChanged += textBox_ValueChanged;
			mLongValueTextBox.NavigationKeyDown += keyDown;

			SuspendLayout();
			Controls.Add(mWordEditor);
			Controls.Add(mEqualsLabel);
			Controls.Add(mLongValueTextBox);
			Name = "WordValueEditor";
			KeyDown += keyDown;

			sizeComponent();
		}

        public Control EditorControl => this;

        public FieldTypes? FocusedField => mLongValueTextBox.Focused ? FieldTypes.Value : mWordEditor.FocusedField;

        public int? CaretIndex => 
            FocusedField == FieldTypes.Value ? mLongValueTextBox.SelectionStart + mLongValueTextBox.SelectionLength : mWordEditor.CaretIndex;

        public bool Focus(FieldTypes? field, int? index) => 
            field == FieldTypes.Value ? mLongValueTextBox.FocusWithIndex(index) : mWordEditor.Focus(field, index);

        protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        protected override void Dispose(bool disposing)
		{
		}

        void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers != Keys.None) return;

            FieldTypes editorField = FieldTypes.Word;
            int? index = null;

            if (sender == mLongValueTextBox)
            {
                editorField = FieldTypes.Value;
                index = mLongValueTextBox.SelectionLength + mLongValueTextBox.SelectionStart;
            }

            switch (e.KeyCode)
            {
                case Keys.Prior:
                case Keys.Next:
                case Keys.Up:
                case Keys.Down:
                    NavigationKeyDown?.Invoke(this, new FieldKeyEventArgs(e.KeyData, editorField, index));
                    break;

                case Keys.Right:
                    if (sender == mWordEditor)
                    {
                        mLongValueTextBox.Focus();
                    }
                    else if (sender == mLongValueTextBox && NavigationKeyDown != null)
                    {
                        NavigationKeyDown(this, e);
                    }

                    break;

                case Keys.Left:
                    if (sender == mLongValueTextBox)
                    {
                        mWordEditor.Focus(FieldTypes.LastByte, null);
                    }
                    else if (sender == mWordEditor && NavigationKeyDown != null)
                    {
                        NavigationKeyDown(this, e);
                    }

                    break;
            }
        }

        void sizeComponent()
        {
            SuspendLayout();

            mEqualsLabel.Location = new Point(mWordEditor.Width, 2);
            mLongValueTextBox.MinValue = IncludeSign ? -mWordEditor.WordValue.MaxMagnitude : 0L;
            mLongValueTextBox.MaxValue = mWordEditor.WordValue.MaxMagnitude;
            mLongValueTextBox.Location = new Point(mEqualsLabel.Left + mEqualsLabel.Width, 0);
            mLongValueTextBox.Size = new Size(TextBoxWidth, 21);
            mLongValueTextBox.Magnitude = mWordEditor.WordValue.MagnitudeLongValue;
            mLongValueTextBox.Sign = mWordEditor.WordValue.Sign;
            Size = new Size(mLongValueTextBox.Left + TextBoxWidth, 21);

            ResumeLayout(false);
        }

        void textBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
        {
            IWord wordValue = mWordEditor.WordValue;
            IWord oldValue = new Word(wordValue.Magnitude, wordValue.Sign);
            wordValue.MagnitudeLongValue = mLongValueTextBox.Magnitude;
            wordValue.Sign = mLongValueTextBox.Sign;
            mWordEditor.Update();

            OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new Word(wordValue.Magnitude, wordValue.Sign)));
        }

        public new void Update()
		{
			mWordEditor.Update();
			mLongValueTextBox.Magnitude = mWordEditor.WordValue.MagnitudeLongValue;
			mLongValueTextBox.Sign = mWordEditor.WordValue.Sign;
			base.Update();
		}

		public void UpdateLayout()
		{
			mWordEditor.UpdateLayout();
			mLongValueTextBox.UpdateLayout();
		}

        void wordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
        {
            mLongValueTextBox.Magnitude = mWordEditor.WordValue.MagnitudeLongValue;
            mLongValueTextBox.Sign = mWordEditor.WordValue.Sign;
            OnValueChanged(args);
        }

        public int ByteCount
		{
			get
			{
				return mWordEditor.ByteCount;
			}
			set
			{
				if (ByteCount != value)
				{
					mWordEditor.ByteCount = value;
					sizeComponent();
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
					mWordEditor.IncludeSign = mIncludeSign;
					sizeComponent();
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
					mWordEditor.ReadOnly = mReadOnly;
					mLongValueTextBox.ReadOnly = mReadOnly;
				}
			}
		}

		public int TextBoxWidth
		{
			get
			{
				return mTextBoxWidth;
			}
			set
			{
				if (mTextBoxWidth != value)
				{
					mTextBoxWidth = value;
					sizeComponent();
				}
			}
		}

		public IWord WordValue
		{
			get
			{
				return mWordEditor.WordValue;
			}
			set
			{
				int byteCount = ByteCount;
				mWordEditor.WordValue = value;

				if (byteCount == value.ByteCount)
				{
					Update();
				}
				else
				{
					sizeComponent();
				}
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Value)
			{
				mLongValueTextBox.Select(start, length);
			}
			else
			{
				mWordEditor.Select(start, length);
			}
		}
	}
}
