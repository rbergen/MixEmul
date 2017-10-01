using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class MixByteTextBox : TextBox, IEscapeConsumer
	{
		public const int UseHeight = 21;
		public const int UseWidth = 18;

        byte mByteValue;
        Color mEditingTextColor;
        bool mEditMode;
        int mLastCaretPos;
        string mLastValidText;
        Color mRenderedTextColor;
        bool mUpdating;

        public int Index { get; private set; }

        public event ValueChangedEventHandler ValueChanged;

		public MixByteTextBox()
			: this(0)
		{
		}

		public MixByteTextBox(int index)
		{
			Index = index;
			mByteValue = 0;

			mLastValidText = mByteValue.ToString("D2");
			mLastCaretPos = SelectionStart + SelectionLength;

			SuspendLayout();

			BorderStyle = BorderStyle.FixedSingle;
			Location = new Point(0, 0);
			MaxLength = 2;
			Name = "mByteBox";
			Size = new Size(UseWidth, UseHeight);
			TabIndex = 0;
			Text = mByteValue.ToString("D2");

			ResumeLayout(false);

			UpdateLayout();

			Leave += This_Leave;
			Enter += This_Enter;
			KeyPress += This_KeyPress;
			KeyDown += This_KeyDown;
			TextChanged += This_TextChanged;
		}

        protected virtual void OnValueChanged(ValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        void This_Enter(object sender, EventArgs e) => Select(0, TextLength);

        void This_Leave(object sender, EventArgs e) => CheckAndUpdateValue(Text);

        void CheckAndUpdateValue(byte byteValue)
        {
            mEditMode = false;

            if (byteValue > Byte.MaxValue)
            {
                byteValue = Byte.MaxValue;
            }

            byte oldByteValue = mByteValue;
            mByteValue = byteValue;

            mUpdating = true;

            ForeColor = mRenderedTextColor;
            mLastValidText = mByteValue.ToString("D2");
            base.Text = mLastValidText;
            mLastCaretPos = SelectionStart + SelectionLength;
            Select(0, TextLength);

            mUpdating = false;

            if (oldByteValue != mByteValue) OnValueChanged(new ValueChangedEventArgs(mByteValue, mByteValue));
        }

        void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        void CheckAndUpdateValue(string newValue)
        {
            try
            {
                CheckAndUpdateValue(newValue == "" ? (byte)0 : byte.Parse(newValue));
            }
            catch (FormatException)
            {
            }
        }

        void This_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            switch ((Keys)keyChar)
            {
                case Keys.Enter:
                    e.Handled = true;
                    CheckAndUpdateValue(Text);

                    return;

                case Keys.Escape:
                    e.Handled = true;
                    CheckAndUpdateValue(mByteValue);

                    return;
            }

            e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
        }

        void This_TextChanged(object sender, EventArgs e)
        {
            if (!mUpdating)
            {
                bool textIsValid = true;

                try
                {
                    string text = Text;

                    if (text != "")
                    {
                        var byteValue = byte.Parse(text);
                        textIsValid = byteValue >= 0 && byteValue <= MixByte.MaxValue;
                    }
                }
                catch (FormatException)
                {
                    textIsValid = false;
                }

                if (!textIsValid)
                {
                    mUpdating = true;
                    base.Text = mLastValidText;
                    Select(mLastCaretPos, 0);
                    mUpdating = false;
                }
                else
                {
                    mLastValidText = base.Text;
                    mLastCaretPos = SelectionStart + SelectionLength;

                    if (!mEditMode)
                    {
                        ForeColor = mEditingTextColor;
                        mEditMode = true;
                    }
                }
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

		public MixByte MixByteValue
		{
			get
			{
				return mByteValue;
			}
			set
			{
				mByteValue = value;
				CheckAndUpdateValue(mByteValue);
			}
		}

		public sealed override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				CheckAndUpdateValue(value);
			}
		}

		public class ValueChangedEventArgs : EventArgs
		{
			public MixByte NewValue { get; private set; }
			public MixByte OldValue { get; private set; }

			public ValueChangedEventArgs(MixByte oldValue, MixByte newValue)
			{
				OldValue = oldValue;
				NewValue = newValue;
			}
		}

		public delegate void ValueChangedEventHandler(MixByteTextBox sender, MixByteTextBox.ValueChangedEventArgs e);
	}
}
