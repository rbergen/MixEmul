using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class LongValueTextBox : TextBox, IEscapeConsumer, INavigableControl
	{
		private Color editingTextColor;
		private bool editMode;
		private int lastCaretPos;
		private string lastValidText;
		private long magnitude;
		private Word.Signs sign;
		private long maxValue;
		private long minValue;
		private Color renderedTextColor;
		private bool supportNegativeZero;
		private bool updating;

		public bool SupportSign { get; private set; }
		public bool ClearZero { get; set; }

		public event ValueChangedEventHandler ValueChanged;
		public event KeyEventHandler NavigationKeyDown;

		public LongValueTextBox() : this(long.MinValue, long.MaxValue, 0L) { }

		public LongValueTextBox(long minValue, long maxValue) : this(minValue, maxValue, 0L) { }

		public LongValueTextBox(long minValue, long maxValue, long value)
			: this(minValue, maxValue, value.GetSign(), value.GetMagnitude()) { }

		public LongValueTextBox(long minValue, long maxValue, Word.Signs sign, long magnitude)
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

			this.updating = false;
			this.editMode = false;

			ClearZero = true;

			this.lastCaretPos = SelectionStart + SelectionLength;
			this.lastValidText = this.magnitude.ToString();
		}

		private void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude)
			=> CheckAndUpdateValue(newSign, newMagnitude, false);

		protected virtual void OnValueChanged(ValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void This_Leave(object sender, EventArgs e)
			=> CheckAndUpdateValue(Text);

		private void LongValueTextBox_Enter(object sender, EventArgs e)
		{
			if (ClearZero && this.magnitude == 0 && this.sign.IsPositive())
				base.Text = string.Empty;
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
				return;
			}

			if (e.Modifiers != Keys.None)
				return;

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
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (SelectionStart + SelectionLength == 0 && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;
			}
		}

		private void CheckAndUpdateMaxLength()
			=> MaxLength = Math.Max(this.minValue.ToString().Length, this.maxValue.ToString().Length);

		private void CheckAndUpdateValue(string newValue)
		{
			try
			{
				long magnitude = 0;
				Word.Signs sign = Word.Signs.Positive;

				if (newValue == string.Empty || newValue == "-")
				{
					if (MinValue > 0)
						magnitude = MinValue;
				}
				else
				{
					int offset = 0;

					if (newValue[0] == '-')
					{
						sign = Word.Signs.Negative;
						offset = 1;
					}

					magnitude = long.Parse(newValue[offset..]);
				}

				CheckAndUpdateValue(sign, magnitude);
			}
			catch (FormatException)
			{
				CheckAndUpdateValue(this.lastValidText);
			}
		}

		private void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude, bool suppressEvent)
		{
			this.editMode = false;
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
				newMagnitude = MaxValue;
				if (newMagnitude < 0)
				{
					newSign = Word.Signs.Negative;
					newMagnitude = -newMagnitude;
				}
			}

			this.updating = true;
			SuspendLayout();

			ForeColor = this.renderedTextColor;
			this.lastValidText = newMagnitude.ToString();

			if (newSign == Word.Signs.Negative && (this.supportNegativeZero || newMagnitude != 0))
				this.lastValidText = '-' + this.lastValidText;

			base.Text = this.lastValidText;
			this.lastCaretPos = SelectionStart + SelectionLength;
			Select(TextLength, 0);

			ResumeLayout();
			this.updating = false;

			if (newMagnitude != this.magnitude || newSign != this.sign)
			{
				long magnitude = this.magnitude;
				this.magnitude = newMagnitude;
				Word.Signs sign = this.sign;
				this.sign = newSign;

				if (!suppressEvent)
					OnValueChanged(new ValueChangedEventArgs(sign, magnitude, newSign, newMagnitude));
			}
		}

		private void SetMaxValue(long value)
		{
			this.maxValue = value;

			if (!SupportSign && (this.maxValue < 0L))
				this.maxValue = -this.maxValue;

			if (this.minValue > this.maxValue)
				this.minValue = this.maxValue;
		}

		private void SetMinValue(long value)
		{
			this.minValue = value;

			if (this.minValue > this.maxValue)
				this.maxValue = this.minValue;

			SupportSign = this.minValue < 0L;
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
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
					CheckAndUpdateValue(this.sign, this.magnitude);

					return;
			}

			if (keyChar == '-' || (keyChar == '+' && Text.Length > 0 && Text[0] == '-'))
				ChangeSign();

			e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
		}

		private void ChangeSign()
		{
			int selectionStart = SelectionStart;
			int selectionLength = SelectionLength;

			if (!SupportSign)
				return;

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
				base.Text = Text[1..];

				if (selectionStart > 0)
					selectionStart--;

				if (selectionLength > TextLength)
					selectionLength--;

				SelectionLength = selectionLength;
				SelectionStart = selectionStart;
			}
		}

		public Word.Signs Sign
		{
			get => this.sign;
			set
			{
				if (this.sign != value)
					CheckAndUpdateValue(value, this.magnitude, true);
			}
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (this.updating)
				return;

			bool textIsValid = true;

			try
			{
				string text = Text;
				if (text != string.Empty && (!SupportSign || text != "-"))
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
				this.updating = true;

				base.Text = this.lastValidText;
				Select(this.lastCaretPos, 0);

				this.updating = false;

				return;
			}

			this.lastValidText = base.Text;
			this.lastCaretPos = SelectionStart + SelectionLength;

			if (!this.editMode)
			{
				ForeColor = this.editingTextColor;
				this.editMode = true;
			}
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			this.renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			this.editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = this.renderedTextColor;

			ResumeLayout();
		}

		public long LongValue
		{
			get => this.sign.ApplyTo(this.magnitude);
			set
			{
				Word.Signs sign = Word.Signs.Positive;

				if (value < 0)
				{
					sign = Word.Signs.Negative;
					value = -value;
				}

				if (sign != this.sign || value != this.magnitude)
					CheckAndUpdateValue(sign, value, true);
			}
		}

		public long Magnitude
		{
			get => this.magnitude;
			set
			{
				value = value.GetMagnitude();

				if (this.magnitude != value)
					CheckAndUpdateValue(this.sign, value, true);
			}
		}

		public long MaxValue
		{
			get => this.maxValue;
			set
			{
				if (this.maxValue != value)
				{
					SetMaxValue(value);
					CheckAndUpdateValue(this.sign, this.magnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public long MinValue
		{
			get => this.minValue;
			set
			{
				if (this.minValue != value)
				{
					SetMinValue(value);
					CheckAndUpdateValue(this.sign, this.magnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public bool SupportNegativeZero
		{
			get => this.supportNegativeZero;
			set
			{
				if (this.supportNegativeZero != value)
				{
					this.supportNegativeZero = value;
					CheckAndUpdateValue(this.sign, this.magnitude);
				}
			}
		}

		public override string Text
		{
			get => base.Text;
			set => CheckAndUpdateValue(value);
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
				=> NewSign.ApplyTo(NewMagnitude);

			public long OldValue
				=> OldSign.ApplyTo(OldMagnitude);
		}

		public delegate void ValueChangedEventHandler(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs e);
	}
}
