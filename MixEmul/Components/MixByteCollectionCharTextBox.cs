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
        const long unrendered = long.MinValue;

        Color mEditingTextColor;
        bool mEditMode;
        int mLastCaretPos;
        MixByte[] mLastRenderedBytes;
        string mLastValidText;
        Color mRenderedTextColor;
        bool mUpdating;
        IMixByteCollection mByteCollection;

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

			SuspendLayout();
			CharacterCasing = CharacterCasing.Upper;
			Location = new Point(40, 16);
			Size = new Size(48, 21);
			BorderStyle = BorderStyle.FixedSingle;
			KeyDown += This_KeyDown;
			KeyPress += This_KeyPress;
			Leave += This_Leave;
			TextChanged += This_TextChanged;
			//      base.MaxLength = mByteCollection.MaxByteCount;
			ResumeLayout(false);

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

        void This_Leave(object sender, EventArgs e) => UpdateValue();

        void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Modifiers != Keys.None) return;

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

        void This_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;
            if (mEditMode)
            {
                switch ((Keys)keyChar)
                {
                    case Keys.Enter:
                        e.Handled = true;
                        UpdateValue();

                        return;

                    case Keys.Escape:
                        e.Handled = true;
                        Update();

                        return;
                }
            }

            var index = MixByte.MixChars.IndexOf(char.ToUpper(keyChar));

            e.Handled = (index < 0 || index == MixByte.MixChars.Length - 2) && !char.IsControl(keyChar);
        }

        void This_TextChanged(object sender, EventArgs e)
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
                    mLastCaretPos = SelectionStart + SelectionLength;

                    if (!UseEditMode)
                    {
                        UpdateValue();
                    }
                    else if (!mEditMode)
                    {
                        ForeColor = mEditingTextColor;
                        mEditMode = true;
                    }
                }

                mUpdating = true;

                base.Text = mLastValidText;
                Select(mLastCaretPos, 0);

                mUpdating = false;

            }
        }

        bool ArraysEqual(MixByte[] one, MixByte[] two)
        {
            if ((one == null && two != null) || (one != null && two == null)) return false;
            if (one == null && two == null) return true;
            if (one.Length != two.Length) return false;

            for (int i = 0; i < one.Length; i++)
            {
                if (one[i] != two[i]) return false;
            }

            return true;
        }

        public new void Update()
		{
			var byteCollectionBytes = mByteCollection.ToArray();

			if (mEditMode || mLastRenderedBytes == null || !ArraysEqual(mLastRenderedBytes, byteCollectionBytes))
			{
				mEditMode = false;
				mLastRenderedBytes = byteCollectionBytes;

				var textValue = mByteCollection.ToString(true).TrimEnd();

				mUpdating = true;
				SuspendLayout();

				ForeColor = mRenderedTextColor;
				mLastValidText = textValue;
				mLastCaretPos = SelectionStart + SelectionLength;
				Text = mLastValidText;
				Select(mLastCaretPos, 0);

				ResumeLayout();
				mUpdating = false;
				base.Update();
			}
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = mRenderedTextColor;

			ResumeLayout();
		}

        void UpdateValue()
        {
            int currentCharIndex = 0;
            var oldValue = (IMixByteCollection)mByteCollection.Clone();

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
                var newValue = (IMixByteCollection)mByteCollection.Clone();
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
