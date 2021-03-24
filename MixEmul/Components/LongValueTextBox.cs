using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class LongValueTextBox : TextBox, IEscapeConsumer, INavigableControl
	{
		private Color _editingTextColor;
		private bool _editMode;
		private int _lastCaretPos;
		private string _lastValidText;
		private long _magnitude;
		private Word.Signs _sign;
		private long _maxValue;
		private long _minValue;
		private Color _renderedTextColor;
		private bool _supportNegativeZero;
		private bool _updating;

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

			_updating = false;
			_editMode = false;

			ClearZero = true;

			_lastCaretPos = SelectionStart + SelectionLength;
			_lastValidText = _magnitude.ToString();
		}

		private void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude)
			=> CheckAndUpdateValue(newSign, newMagnitude, false);

		protected virtual void OnValueChanged(ValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void This_Leave(object sender, EventArgs e)
			=> CheckAndUpdateValue(Text);

		private void LongValueTextBox_Enter(object sender, EventArgs e)
		{
			if (ClearZero && _magnitude == 0 && _sign.IsPositive())
				base.Text = "";
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
			=> MaxLength = Math.Max(_minValue.ToString().Length, _maxValue.ToString().Length);

		private void CheckAndUpdateValue(string newValue)
		{
			try
			{
				long magnitude = 0;
				Word.Signs sign = Word.Signs.Positive;

				if (newValue == "" || newValue == "-")
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
				CheckAndUpdateValue(_lastValidText);
			}
		}

		private void CheckAndUpdateValue(Word.Signs newSign, long newMagnitude, bool suppressEvent)
		{
			_editMode = false;
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

			_updating = true;
			SuspendLayout();

			ForeColor = _renderedTextColor;
			_lastValidText = newMagnitude.ToString();

			if (newSign == Word.Signs.Negative && (_supportNegativeZero || newMagnitude != 0))
				_lastValidText = '-' + _lastValidText;

			base.Text = _lastValidText;
			_lastCaretPos = SelectionStart + SelectionLength;
			Select(TextLength, 0);

			ResumeLayout();
			_updating = false;

			if (newMagnitude != _magnitude || newSign != _sign)
			{
				long magnitude = _magnitude;
				_magnitude = newMagnitude;
				Word.Signs sign = _sign;
				_sign = newSign;

				if (!suppressEvent)
					OnValueChanged(new ValueChangedEventArgs(sign, magnitude, newSign, newMagnitude));
			}
		}

		private void SetMaxValue(long value)
		{
			_maxValue = value;

			if (!SupportSign && (_maxValue < 0L))
				_maxValue = -_maxValue;

			if (_minValue > _maxValue)
				_minValue = _maxValue;
		}

		private void SetMinValue(long value)
		{
			_minValue = value;

			if (_minValue > _maxValue)
				_maxValue = _minValue;

			SupportSign = _minValue < 0L;
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
					CheckAndUpdateValue(_sign, _magnitude);

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
			get => _sign;
			set
			{
				if (_sign != value)
					CheckAndUpdateValue(value, _magnitude, true);
			}
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

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
				_updating = true;

				base.Text = _lastValidText;
				Select(_lastCaretPos, 0);

				_updating = false;

				return;
			}

			_lastValidText = base.Text;
			_lastCaretPos = SelectionStart + SelectionLength;

			if (!_editMode)
			{
				ForeColor = _editingTextColor;
				_editMode = true;
			}
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			_renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			_editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = _renderedTextColor;

			ResumeLayout();
		}

		public long LongValue
		{
			get => _sign.ApplyTo(_magnitude);
			set
			{
				Word.Signs sign = Word.Signs.Positive;

				if (value < 0)
				{
					sign = Word.Signs.Negative;
					value = -value;
				}

				if (sign != _sign || value != _magnitude)
					CheckAndUpdateValue(sign, value, true);
			}
		}

		public long Magnitude
		{
			get => _magnitude;
			set
			{
				value = value.GetMagnitude();

				if (_magnitude != value)
					CheckAndUpdateValue(_sign, value, true);
			}
		}

		public long MaxValue
		{
			get => _maxValue;
			set
			{
				if (_maxValue != value)
				{
					SetMaxValue(value);
					CheckAndUpdateValue(_sign, _magnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public long MinValue
		{
			get => _minValue;
			set
			{
				if (_minValue != value)
				{
					SetMinValue(value);
					CheckAndUpdateValue(_sign, _magnitude);
					CheckAndUpdateMaxLength();
				}
			}
		}

		public bool SupportNegativeZero
		{
			get => _supportNegativeZero;
			set
			{
				if (_supportNegativeZero != value)
				{
					_supportNegativeZero = value;
					CheckAndUpdateValue(_sign, _magnitude);
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
