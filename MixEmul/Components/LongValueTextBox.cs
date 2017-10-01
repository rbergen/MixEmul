using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class LongValueTextBox : TextBox, IEscapeConsumer, INavigableControl
	{
        Color mEditingTextColor;
        bool mEditMode;
        int mLastCaretPos;
        string mLastValidText;
        long mMagnitude;
        Word.Signs mSign;
        long mMaxValue;
        long mMinValue;
        Color mRenderedTextColor;
        bool mSupportNegativeZero;
        bool mUpdating;

        public bool SupportSign { get; private set; }
        public bool ClearZero { get; set; }

        public event ValueChangedEventHandler ValueChanged;
		public event KeyEventHandler NavigationKeyDown;

		public LongValueTextBox()
			: this(true, long.MinValue, long.MaxValue, 0L)
		{
		}

		public LongValueTextBox(bool supportsSign)
			: this(supportsSign, long.MinValue, long.MaxValue, 0L)
		{
		}

		public LongValueTextBox(bool supportsSign, long minValue, long maxValue)
			: this(supportsSign, minValue, maxValue, 0L)
		{
		}

		public LongValueTextBox(bool supportsSign, long minValue, long maxValue, long value)
			: this(supportsSign, minValue, maxValue, value.GetSign(), value.GetMagnitude())
		{
		}

		public LongValueTextBox(bool supportsSign, long minValue, long maxValue, Word.Signs sign, long magnitude)
		{
			SupportSign = true;

			BorderStyle = BorderStyle.FixedSingle;

			UpdateLayout();

			KeyDown += This_KeyDown;
			KeyPress += This_KeyPress;
			Leave += This_Leave;
			TextChanged += This_TextChanged;
			Enter += LongValueTextBox_Enter;
			SetMinValue(minValue);
			SetMaxValue(maxValue);

			CheckAndUpdateValue(sign, magnitude);
			CheckAndUpdateMaxLength();

			mUpdating = false;
			mEditMode = false;

			ClearZero = true;

			mLastCaretPos = SelectionStart + SelectionLength;
			mLastValidText = mMagnitude.ToString();
		}

        void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude) => CheckAndUpdateValue(newSign, newMagnitude, false);

        protected virtual void OnValueChanged(ValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        void This_Leave(object sender, EventArgs e) => CheckAndUpdateValue(Text);

        void LongValueTextBox_Enter(object sender, EventArgs e)
		{
			if (ClearZero && mMagnitude == 0 && mSign.IsPositive())
			{
				base.Text = "";
			}
		}

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
                    NavigationKeyDown?.Invoke(this, e);
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
			}
		}

        void CheckAndUpdateMaxLength()
        {
            MaxLength = Math.Max(mMinValue.ToString().Length, mMaxValue.ToString().Length);
        }

        void CheckAndUpdateValue(string newValue)
        {
            try
            {
                long magnitude = 0;
                Word.Signs sign = Word.Signs.Positive;

                if (newValue == "" || newValue == "-")
                {
                    if (MinValue > 0)
                    {
                        magnitude = MinValue;
                    }
                }
                else
                {
                    int offset = 0;

                    if (newValue[0] == '-')
                    {
                        sign = Word.Signs.Negative;
                        offset = 1;
                    }

                    magnitude = long.Parse(newValue.Substring(offset));
                }

                CheckAndUpdateValue(sign, magnitude);
            }
            catch (FormatException)
            {
                CheckAndUpdateValue(mLastValidText);
            }
        }

        void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude, bool suppressEvent)
        {
            mEditMode = false;
            var newValue = newSign.ApplyTo(newMagnitude);

            if (newValue < MinValue)
            {
                newValue = MinValue;
                newMagnitude = MinValue;
                if (newMagnitude < 0)
                {
                    newSign = Word.Signs.Negative;
                    newMagnitude = -newMagnitude;
                }
            }

            if (!SupportSign && (newValue < 0L))
            {
                newValue = -newValue;
                newSign = Word.Signs.Positive;
            }

            if (newValue > MaxValue)
            {
                newValue = MaxValue;
                newMagnitude = MaxValue;
                if (newMagnitude < 0)
                {
                    newSign = Word.Signs.Negative;
                    newMagnitude = -newMagnitude;
                }
            }

            mUpdating = true;
            SuspendLayout();

            ForeColor = mRenderedTextColor;
            mLastValidText = newMagnitude.ToString();
            if (newSign == Word.Signs.Negative && (mSupportNegativeZero || newMagnitude != 0))
            {
                mLastValidText = '-' + mLastValidText;
            }

            base.Text = mLastValidText;
            mLastCaretPos = SelectionStart + SelectionLength;
            Select(TextLength, 0);

            ResumeLayout();
            mUpdating = false;

            if (newMagnitude != mMagnitude || newSign != mSign)
            {
                long magnitude = mMagnitude;
                mMagnitude = newMagnitude;
                Word.Signs sign = mSign;
                mSign = newSign;

                if (!suppressEvent)
                {
                    OnValueChanged(new ValueChangedEventArgs(sign, magnitude, newSign, newMagnitude));
                }
            }
        }

        void SetMaxValue(long value)
        {
            mMaxValue = value;

            if (!SupportSign && (mMaxValue < 0L))
            {
                mMaxValue = -mMaxValue;
            }

            if (mMinValue > mMaxValue)
            {
                mMinValue = mMaxValue;
            }
        }

        void SetMinValue(long value)
        {
            mMinValue = value;
            if (mMinValue > mMaxValue)
            {
                mMaxValue = mMinValue;
            }

            SupportSign = mMinValue < 0L;
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
                    CheckAndUpdateValue(mSign, mMagnitude);

                    return;
            }

            if (keyChar == '-' || (keyChar == '+' && Text.Length > 0 && Text[0] == '-')) ChangeSign();

            e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
        }

        void ChangeSign()
        {
            int selectionStart = SelectionStart;
            int selectionLength = SelectionLength;

            if (!SupportSign) return;

            if (TextLength == 0)
            {
                base.Text = "-";
                SelectionStart = 1;

                return;
            }

            if (Text[0] != '-')
            {
                base.Text = '-' + Text;
                SelectionStart = selectionStart + 1;
                SelectionLength = selectionLength;
            }
            else
            {
                base.Text = Text.Substring(1);

                if (selectionStart > 0) selectionStart--;
                if (selectionLength > TextLength) selectionLength--;

                SelectionLength = selectionLength;
                SelectionStart = selectionStart;
            }
        }

        public Word.Signs Sign
		{
			get
			{
				return mSign;
			}
			set
			{
				if (mSign != value) CheckAndUpdateValue(value, mMagnitude, true);
			}
		}

        void This_TextChanged(object sender, EventArgs e)
        {
            if (mUpdating) return;

            bool textIsValid = true;

            try
            {
                string text = Text;
                if (text != "" && (!SupportSign || text != "-"))
                {
                    var num = long.Parse(text);
                    textIsValid = ((num >= MinValue) && (num <= MaxValue)) && (SupportSign || (num >= 0L));
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

                return;
            }

            mLastValidText = base.Text;
            mLastCaretPos = SelectionStart + SelectionLength;

            if (!mEditMode)
            {
                ForeColor = mEditingTextColor;
                mEditMode = true;
            }
        }

        public void UpdateLayout()
		{
			SuspendLayout();

			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = mRenderedTextColor;

			ResumeLayout();
		}

		public long LongValue
		{
			get
			{
				return mSign.ApplyTo(mMagnitude);
			}
			set
			{
				Word.Signs sign = Word.Signs.Positive;

				if (value < 0)
				{
					sign = Word.Signs.Negative;
					value = -value;
				}

				if (sign != mSign || value != mMagnitude) CheckAndUpdateValue(sign, value, true);
			}
		}

		public long Magnitude
		{
			get
			{
				return mMagnitude;
			}
			set
			{
				value = value.GetMagnitude();

				if (mMagnitude != value) CheckAndUpdateValue(mSign, value, true);
			}
		}

		public long MaxValue
		{
			get
			{
				return mMaxValue;
			}
			set
			{
				if (mMaxValue != value)
				{
					SetMaxValue(value);
					CheckAndUpdateValue(mSign, mMagnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public long MinValue
		{
			get
			{
				return mMinValue;
			}
			set
			{
				if (mMinValue != value)
				{
					SetMinValue(value);
					CheckAndUpdateValue(mSign, mMagnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public bool SupportNegativeZero
		{
			get
			{
				return mSupportNegativeZero;
			}
			set
			{
				if (mSupportNegativeZero != value)
				{
					mSupportNegativeZero = value;
					CheckAndUpdateValue(mSign, mMagnitude);
				}
			}
		}

		public override string Text
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
            public long OldMagnitude { get; private set; }
            public Word.Signs OldSign { get; private set; }
            public long NewMagnitude { get; private set; }
            public Word.Signs NewSign { get; private set; }

            public ValueChangedEventArgs(Word.Signs oldSign, long oldMagnitude, Word.Signs newSign, long newMagnitude)
			{
				OldMagnitude = oldMagnitude;
				NewMagnitude = newMagnitude;
				OldSign = oldSign;
				NewSign = newSign;
			}

            public long NewValue => NewSign.ApplyTo(NewMagnitude);
	
            public long OldValue => OldSign.ApplyTo(OldMagnitude);
		}

		public delegate void ValueChangedEventHandler(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs e);
	}
}
