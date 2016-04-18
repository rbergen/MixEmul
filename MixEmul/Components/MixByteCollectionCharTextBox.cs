using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
	public class MixByteCollectionCharTextBox : TextBox, IMixByteCollectionEditor, IEscapeConsumer, INavigableControl
	{
		private const long unrendered = long.MinValue;

		private Color mEditingTextColor;
		private bool mEditMode;
		private int mLastCaretPos;
		private MixByte[] mLastRenderedBytes;
		private string mLastValidText;
		private Color mRenderedTextColor;
		private bool mUpdating;
		private IMixByteCollection mByteCollection;

        public bool UseEditMode { get; set; }

        public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;
		public event KeyEventHandler NavigationKeyDown;

		public MixByteCollectionCharTextBox()
			: this(null)
		{
		}

		public MixByteCollectionCharTextBox(IMixByteCollection byteCollection)
		{
			mByteCollection = byteCollection;
			UseEditMode = true;

			if (mByteCollection == null)
			{
				mByteCollection = new FullWord();
			}

			base.SuspendLayout();
			base.CharacterCasing = CharacterCasing.Upper;
			base.Location = new Point(40, 16);
			base.Size = new Size(48, 21);
			base.BorderStyle = BorderStyle.FixedSingle;
			base.KeyDown += this_KeyDown;
			base.KeyPress += this_KeyPress;
			base.Leave += this_Leave;
			base.TextChanged += this_TextChanged;
			//      base.MaxLength = mByteCollection.MaxByteCount;
			base.ResumeLayout(false);

			UpdateLayout();

			mUpdating = false;
			mEditMode = false;
			mLastRenderedBytes = null;
			Update();
		}

        public Control EditorControl => this;

        public bool Focus(FieldTypes? field, int? index) => this.FocusWithIndex(index);

        public FieldTypes? FocusedField => FieldTypes.Chars;

        public int? CaretIndex => SelectionStart + SelectionLength;

        protected void OnValueChanged(MixByteCollectionEditorValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        private void this_Leave(object sender, EventArgs e) => updateValue();

        private void this_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
				return;
			}

			if (e.Modifiers != Keys.None)
			{
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
                    NavigationKeyDown?.Invoke(this, new IndexKeyEventArgs(e.KeyData, SelectionStart + SelectionLength));
                    break;

				case Keys.Right:
					if (SelectionStart + SelectionLength == TextLength && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}
					break;

				case Keys.Left:
					if (SelectionStart + SelectionLength == 0 && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}
					break;

				case Keys.Enter:
					e.Handled = true;
					break;
			}
		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;
			if (mEditMode)
			{
				switch ((Keys)keyChar)
				{
					case Keys.Enter:
						e.Handled = true;
						updateValue();

						return;

					case Keys.Escape:
						e.Handled = true;
						Update();

						return;
				}
			}

			int index = MixByte.MixChars.IndexOf(char.ToUpper(keyChar));

			e.Handled = (index < 0 || index == MixByte.MixChars.Length - 2) && !char.IsControl(keyChar);
		}

		private void this_TextChanged(object sender, EventArgs e)
		{
			if (!mUpdating)
			{
				string validText = "";

				for (int i = 0; i < TextLength; i++)
				{
					if (MixByte.MixChars.IndexOf(Text[i]) >= 0)
					{
						validText += Text[i];
					}
				}

				if (validText.Length > mByteCollection.MaxByteCount)
				{
					validText = validText.Substring(0, mByteCollection.MaxByteCount);
				}

				if (validText.Length > 0 || TextLength == 0)
				{
					mLastValidText = validText;
					mLastCaretPos = base.SelectionStart + SelectionLength;

					if (!UseEditMode)
					{
						updateValue();
					}
					else if (!mEditMode)
					{
						ForeColor = mEditingTextColor;
						mEditMode = true;
					}
				}

				mUpdating = true;

				base.Text = mLastValidText;
				base.Select(mLastCaretPos, 0);

				mUpdating = false;

			}
		}

		private bool arraysEqual(MixByte[] one, MixByte[] two)
		{
			if ((one == null && two != null) || (one != null && two == null))
			{
				return false;
			}

			if (one == null && two == null)
			{
				return true;
			}

			if (one.Length != two.Length)
			{
				return false;
			}

			for (int i = 0; i < one.Length; i++)
			{
				if (one[i] != two[i])
				{
					return false;
				}
			}

			return true;
		}

		public new void Update()
		{
			MixByte[] byteCollectionBytes = mByteCollection.ToArray();

			if (mEditMode || mLastRenderedBytes == null || !arraysEqual(mLastRenderedBytes, byteCollectionBytes))
			{
				mEditMode = false;
				mLastRenderedBytes = byteCollectionBytes;

				string textValue = mByteCollection.ToString(true).TrimEnd();

				mUpdating = true;
				base.SuspendLayout();

				ForeColor = mRenderedTextColor;
				mLastValidText = textValue;
				mLastCaretPos = base.SelectionStart + SelectionLength;
				Text = mLastValidText;
				base.Select(mLastCaretPos, 0);

				base.ResumeLayout();
				mUpdating = false;
				base.Update();
			}
		}

		public void UpdateLayout()
		{
			base.SuspendLayout();

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = mRenderedTextColor;

			base.ResumeLayout();
		}

		private void updateValue()
		{
			int currentCharIndex = 0;
			IMixByteCollection oldValue = (IMixByteCollection)mByteCollection.Clone();

			bool valueDiffers = false;

			while (currentCharIndex < Math.Min(TextLength, mByteCollection.MaxByteCount))
			{
				if (mByteCollection[currentCharIndex].CharValue != Text[currentCharIndex])
				{
					valueDiffers = true;

					if (Text[currentCharIndex] != MixByte.MixChars[MixByte.MixChars.Length - 1])
					{
						mByteCollection[currentCharIndex] = Text[currentCharIndex];
					}
				}

				currentCharIndex++;
			}

			while (currentCharIndex < mByteCollection.MaxByteCount)
			{
				if (mByteCollection[currentCharIndex].CharValue != ' ')
				{
					valueDiffers = true;
					mByteCollection[currentCharIndex] = ' ';
				}
				currentCharIndex++;
			}

			if (valueDiffers)
			{
				IMixByteCollection newValue = (IMixByteCollection)mByteCollection.Clone();
				OnValueChanged(new MixByteCollectionEditorValueChangedEventArgs(oldValue, newValue));
			}

			Update();
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get
			{
				return mByteCollection;
			}
			set
			{
				if (value != null)
				{
					mByteCollection = value;

					if (TextLength > mByteCollection.MaxByteCount)
					{
						Text = Text.Substring(0, mByteCollection.MaxByteCount);
					}

					Select(TextLength, 0);

					Update();
				}
			}
		}
	}
}
