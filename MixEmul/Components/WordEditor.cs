using MixGui.Events;
using MixLib.Type;
using System;
using System.Drawing;
using System.Windows.Forms;

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
		readonly Button mSignButton;
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

			mSignButton = new Button
			{
				Location = new Point(0, 0),
				Name = "SignButton",
				Size = new Size(18, 21),
				TabIndex = 0,
				TabStop = false,
				Text = "" + mWord.Sign.ToChar(),
				FlatStyle = FlatStyle.Flat,
				Enabled = !ReadOnly
			};
			mSignButton.Click += MSignButton_Click;
			mSignButton.KeyPress += Editor_KeyPress;
			mSignButton.KeyDown += This_KeyDown;

			InitializeComponent();
		}

		public int? CaretIndex => null;

		public Control EditorControl => this;

		public bool Focus(FieldTypes? field, int? index)
		{
			return ByteCount > 0 ? mByteTextBoxes[field == FieldTypes.LastByte ? ByteCount - 1 : 0].Focus() : mSignButton.Focus();
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			ValueChanged?.Invoke(this, args);
		}

		void ByteValueChanged(MixByteTextBox textBox, MixByteTextBox.ValueChangedEventArgs args)
		{
			var oldValue = new Word(mWord.Magnitude, mWord.Sign);
			mWord[textBox.Index] = args.NewValue;
			mLastRenderedMagnitude = mWord.MagnitudeLongValue;
			mLastRenderedSign = mWord.Sign;

			var newValue = new Word(mWord.Magnitude, mWord.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}


		void Editor_Click(object sender, EventArgs e)
		{
			var box = (MixByteTextBox)sender;

			if (box.SelectionLength == 0)
			{
				box.Select(0, 2);
			}
		}

		void Editor_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;

			if (IncludeSign && (keyChar == '-' || (keyChar == '+' && mWord.Sign.IsNegative())))
			{
				NegateSign();
			}
		}

		void Editor_Leave(object sender, EventArgs e)
		{
			var box = (MixByteTextBox)sender;
			mFocusByte = box.Index;
			mFocusByteSelStart = box.SelectionStart;
			mFocusByteSelLength = box.SelectionLength;
		}

		void InitializeComponent()
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
				var box = new MixByteTextBox(i)
				{
					Location = new Point(width, 0),
					Name = "MixByteEditor" + i,
					MixByteValue = mWord[i],
					ReadOnly = ReadOnly,
					TabIndex = tabIndex
				};
				box.ValueChanged += ByteValueChanged;
				box.Click += Editor_Click;
				box.Leave += Editor_Leave;
				box.KeyPress += Editor_KeyPress;
				box.KeyDown += This_KeyDown;

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

		void This_KeyDown(object sender, KeyEventArgs e)
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
						var byteIndex = Array.IndexOf(mByteTextBoxes, box);
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
						var byteIndex = Array.IndexOf(mByteTextBoxes, box);
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

		void MSignButton_Click(object sender, EventArgs e)
		{
			NegateSign();
			if (mByteTextBoxes.Length > 0)
			{
				mByteTextBoxes[mFocusByte].Focus();
				mByteTextBoxes[mFocusByte].SelectionStart = mFocusByteSelStart;
				mByteTextBoxes[mFocusByte].SelectionLength = mFocusByteSelLength;
			}
		}

		void NegateSign()
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

		void ReinitializeComponent()
		{
			Controls.Clear();

			foreach (MixByteTextBox box in mByteTextBoxes)
			{
				box.Dispose();
			}

			InitializeComponent();
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
			get => mWord.ByteCount;
			set
			{
				if (ByteCount != value)
				{
					mWord = new Word(value);
					ReinitializeComponent();
				}
			}
		}

		public bool IncludeSign
		{
			get => mIncludeSign;
			set
			{
				if (mIncludeSign != value)
				{
					mIncludeSign = value;
					ReinitializeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get => mReadOnly;
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
			get => mWord;
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
					ReinitializeComponent();
				}
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

		public void Select(int start, int length) { }
	}
}
