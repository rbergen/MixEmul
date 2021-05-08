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

		private byte byteValue;
		private Color editingTextColor;
		private bool editMode;
		private int lastCaretPos;
		private string lastValidText;
		private Color renderedTextColor;
		private bool updating;

		public int Index { get; private set; }

		public event ValueChangedEventHandler ValueChanged;

		public MixByteTextBox(int index = 0)
		{
			Index = index;
			this.byteValue = 0;

			this.lastValidText = this.byteValue.ToString("D2");
			this.lastCaretPos = SelectionStart + SelectionLength;

			SuspendLayout();

			BorderStyle = BorderStyle.FixedSingle;
			Location = new Point(0, 0);
			MaxLength = 2;
			Name = "mByteBox";
			Size = new Size(UseWidth, UseHeight);
			TabIndex = 0;
			Text = this.byteValue.ToString("D2");

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
			this.editMode = false;

			if (byteValue > byte.MaxValue)
				byteValue = byte.MaxValue;

			byte oldByteValue = this.byteValue;
			this.byteValue = byteValue;

			this.updating = true;

			ForeColor = this.renderedTextColor;
			this.lastValidText = this.byteValue.ToString("D2");
			base.Text = this.lastValidText;
			this.lastCaretPos = SelectionStart + SelectionLength;
			Select(0, TextLength);

			this.updating = false;

			if (oldByteValue != this.byteValue)
				OnValueChanged(new ValueChangedEventArgs(this.byteValue, this.byteValue));
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
					CheckAndUpdateValue(this.byteValue);

					return;
			}

			e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
		}

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (this.updating)
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
				this.updating = true;
				base.Text = this.lastValidText;
				Select(this.lastCaretPos, 0);
				this.updating = false;
			}
			else
			{
				this.lastValidText = base.Text;
				this.lastCaretPos = SelectionStart + SelectionLength;

				if (!this.editMode)
				{
					ForeColor = this.editingTextColor;
					this.editMode = true;
				}
			}
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			this.renderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			this.editingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = this.renderedTextColor;

			ResumeLayout();
		}

		public MixByte MixByteValue
		{
			get => this.byteValue;
			set
			{
				this.byteValue = value;
				CheckAndUpdateValue(this.byteValue);
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
