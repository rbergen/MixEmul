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

		private byte _byteValue;
		private Color _editingTextColor;
		private bool _editMode;
		private int _lastCaretPos;
		private string _lastValidText;
		private Color _renderedTextColor;
		private bool _updating;

		public int Index { get; private set; }

		public event ValueChangedEventHandler ValueChanged;

		public MixByteTextBox(int index = 0)
		{
			Index = index;
			_byteValue = 0;

			_lastValidText = _byteValue.ToString("D2");
			_lastCaretPos = SelectionStart + SelectionLength;

			SuspendLayout();

			BorderStyle = BorderStyle.FixedSingle;
			Location = new Point(0, 0);
			MaxLength = 2;
			Name = "mByteBox";
			Size = new Size(UseWidth, UseHeight);
			TabIndex = 0;
			Text = _byteValue.ToString("D2");

			ResumeLayout(false);

			UpdateLayout();

			Leave += This_Leave;
			Enter += This_Enter;
			KeyPress += This_KeyPress;
			KeyDown += This_KeyDown;
			TextChanged += This_TextChanged;
		}

		protected virtual void OnValueChanged(ValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void This_Enter(object sender, EventArgs e)
			=> Select(0, TextLength);

		private void This_Leave(object sender, EventArgs e)
			=> CheckAndUpdateValue(Text);

		private void CheckAndUpdateValue(byte byteValue)
		{
			_editMode = false;

			if (byteValue > byte.MaxValue)
				byteValue = byte.MaxValue;

			byte oldByteValue = _byteValue;
			_byteValue = byteValue;

			_updating = true;

			ForeColor = _renderedTextColor;
			_lastValidText = _byteValue.ToString("D2");
			base.Text = _lastValidText;
			_lastCaretPos = SelectionStart + SelectionLength;
			Select(0, TextLength);

			_updating = false;

			if (oldByteValue != _byteValue)
				OnValueChanged(new ValueChangedEventArgs(_byteValue, _byteValue));
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void CheckAndUpdateValue(string newValue)
		{
			try
			{
				CheckAndUpdateValue(newValue == "" ? (byte)0 : byte.Parse(newValue));
			}
			catch (FormatException) { }
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
					CheckAndUpdateValue(_byteValue);

					return;
			}

			e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

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
				_updating = true;
				base.Text = _lastValidText;
				Select(_lastCaretPos, 0);
				_updating = false;
			}
			else
			{
				_lastValidText = base.Text;
				_lastCaretPos = SelectionStart + SelectionLength;

				if (!_editMode)
				{
					ForeColor = _editingTextColor;
					_editMode = true;
				}
			}
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			_renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			_editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = _renderedTextColor;

			ResumeLayout();
		}

		public MixByte MixByteValue
		{
			get => _byteValue;
			set
			{
				_byteValue = value;
				CheckAndUpdateValue(_byteValue);
			}
		}

		public sealed override string Text
		{
			get => base.Text;
			set => CheckAndUpdateValue(value);
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
