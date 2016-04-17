using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class LongValueTextBox : TextBox, IEscapeConsumer, INavigableControl
	{
		private Color mEditingTextColor;
		private bool mEditMode;
		private int mLastCaretPos;
		private string mLastValidText;
		private long mMagnitude;
		private Word.Signs mSign;
		private long mMaxValue;
		private long mMinValue;
		private Color mRenderedTextColor;
		private bool mSupportSign;
		private bool mSupportNegativeZero;
		private bool mUpdating;

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
			mSupportSign = true;

			base.BorderStyle = BorderStyle.FixedSingle;

			UpdateLayout();

			base.KeyDown += this_KeyDown;
			base.KeyPress += this_KeyPress;
			base.Leave += this_Leave;
			base.TextChanged += this_TextChanged;
			base.Enter += LongValueTextBox_Enter;
			setMinValue(minValue);
			setMaxValue(maxValue);

			checkAndUpdateValue(sign, magnitude);
			checkAndUpdateMaxLength();

			mUpdating = false;
			mEditMode = false;

			ClearZero = true;

			mLastCaretPos = base.SelectionStart + SelectionLength;
			mLastValidText = mMagnitude.ToString();
		}

		public bool ClearZero
		{
			get;
			set;
		}

		void LongValueTextBox_Enter(object sender, EventArgs e)
		{
			if (ClearZero && mMagnitude == 0 && mSign.IsPositive())
			{
				base.Text = "";
			}
		}

		void this_KeyDown(object sender, KeyEventArgs e)
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
					if (NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}
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

		private void checkAndUpdateMaxLength()
		{
			MaxLength = Math.Max(mMinValue.ToString().Length, mMaxValue.ToString().Length);
		}

		private void checkAndUpdateValue(Word.Signs newSign, long newMagnitude)
		{
			checkAndUpdateValue(newSign, newMagnitude, false);
		}

		private void checkAndUpdateValue(string newValue)
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

				checkAndUpdateValue(sign, magnitude);
			}
			catch (FormatException)
			{
				checkAndUpdateValue(mLastValidText);
			}
		}

		private void checkAndUpdateValue(Word.Signs newSign, long newMagnitude, bool suppressEvent)
		{
			mEditMode = false;
			long newValue = newSign.ApplyTo(newMagnitude);

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

			if (!mSupportSign && (newValue < 0L))
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
			base.SuspendLayout();

			ForeColor = mRenderedTextColor;
			mLastValidText = newMagnitude.ToString();
			if (newSign == Word.Signs.Negative && (mSupportNegativeZero || newMagnitude != 0))
			{
				mLastValidText = '-' + mLastValidText;
			}

			base.Text = mLastValidText;
			mLastCaretPos = base.SelectionStart + SelectionLength;
			base.Select(TextLength, 0);

			base.ResumeLayout();
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

		protected virtual void OnValueChanged(ValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
			}
		}

		private void setMaxValue(long value)
		{
			mMaxValue = value;

			if (!mSupportSign && (mMaxValue < 0L))
			{
				mMaxValue = -mMaxValue;
			}

			if (mMinValue > mMaxValue)
			{
				mMinValue = mMaxValue;
			}
		}

		private void setMinValue(long value)
		{
			mMinValue = value;
			if (mMinValue > mMaxValue)
			{
				mMaxValue = mMinValue;
			}

			mSupportSign = mMinValue < 0L;
		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;

			switch ((Keys)keyChar)
			{
				case Keys.Enter:
					e.Handled = true;
					checkAndUpdateValue(Text);

					return;

				case Keys.Escape:
					e.Handled = true;
					checkAndUpdateValue(mSign, mMagnitude);

					return;
			}

			if (keyChar == '-')
			{
				changeSign();
			}
			else if (keyChar == '+' && Text.Length > 0 && Text[0] == '-')
			{
				changeSign();
			}

			e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
		}

		private void changeSign()
		{
			int selectionStart = base.SelectionStart;
			int selectionLength = SelectionLength;

			if (!SupportSign)
			{
				return;
			}

			if (TextLength == 0)
			{
				base.Text = "-";
				base.SelectionStart = 1;

				return;
			}

			if (Text[0] != '-')
			{
				base.Text = '-' + Text;
				base.SelectionStart = selectionStart + 1;
				SelectionLength = selectionLength;
			}
			else
			{
				base.Text = Text.Substring(1);
				if (selectionStart > 0)
				{
					selectionStart--;
				}
				if (selectionLength > TextLength)
				{
					selectionLength--;
				}
				SelectionLength = selectionLength;
				base.SelectionStart = selectionStart;
			}
		}

		private void this_Leave(object sender, EventArgs e)
		{
			checkAndUpdateValue(Text);
		}

		public Word.Signs Sign
		{
			get
			{
				return mSign;
			}
			set
			{
				if (mSign != value)
				{
					checkAndUpdateValue(value, mMagnitude, true);
				}
			}
		}

		private void this_TextChanged(object sender, EventArgs e)
		{
			if (mUpdating)
			{
				return;
			}

			bool textIsValid = true;

			try
			{
				string text = Text;
				if (text != "" && (!SupportSign || text != "-"))
				{
					long num = long.Parse(text);
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
				base.Select(mLastCaretPos, 0);

				mUpdating = false;

				return;
			}

			mLastValidText = base.Text;
			mLastCaretPos = base.SelectionStart + SelectionLength;

			if (!mEditMode)
			{
				ForeColor = mEditingTextColor;
				mEditMode = true;
			}
		}

		public void UpdateLayout()
		{
			base.SuspendLayout();

			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = mRenderedTextColor;

			base.ResumeLayout();
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

				if (sign != mSign || value != mMagnitude)
				{
					checkAndUpdateValue(sign, value, true);
				}
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
				if (value < 0)
				{
					value = -value;
				}

				if (mMagnitude != value)
				{
					checkAndUpdateValue(mSign, value, true);
				}
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
					setMaxValue(value);
					checkAndUpdateValue(mSign, mMagnitude);
					checkAndUpdateMaxLength();
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
					setMinValue(value);
					checkAndUpdateValue(mSign, mMagnitude);
					checkAndUpdateMaxLength();
				}
			}
		}

		public bool SupportSign
		{
			get
			{
				return mSupportSign;
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
					checkAndUpdateValue(mSign, mMagnitude);
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
				checkAndUpdateValue(value);
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

            public long NewValue
			{
				get
				{
					return NewSign.ApplyTo(NewMagnitude);
				}
			}

            public long OldValue
			{
				get
				{
					return OldSign.ApplyTo(OldMagnitude);
				}
			}
		}

		public delegate void ValueChangedEventHandler(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs e);
	}
}
