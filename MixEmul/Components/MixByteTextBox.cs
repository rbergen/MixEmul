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

		private byte mByteValue;
		private Color mEditingTextColor;
		private bool mEditMode;
		private int mIndex;
		private int mLastCaretPos;
		private string mLastValidText;
		private Color mRenderedTextColor;
		private bool mUpdating;

		public event ValueChangedEventHandler ValueChanged;

		public MixByteTextBox()
			: this(0)
		{
		}

		public MixByteTextBox(int index)
		{
			mIndex = index;
			mByteValue = 0;

			mLastValidText = mByteValue.ToString("D2");
			mLastCaretPos = base.SelectionStart + SelectionLength;

			base.SuspendLayout();

			base.BorderStyle = BorderStyle.FixedSingle;
			base.Location = new Point(0, 0);
			MaxLength = 2;
			base.Name = "mByteBox";
			base.Size = new Size(UseWidth, UseHeight);
			base.TabIndex = 0;
			Text = mByteValue.ToString("D2");

			base.ResumeLayout(false);

			UpdateLayout();

			base.Leave += this_Leave;
			base.Enter += this_Enter;
			base.KeyPress += this_KeyPress;
			base.KeyDown += this_KeyDown;
			base.TextChanged += this_TextChanged;
		}

		private void checkAndUpdateValue(byte byteValue)
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
			mLastCaretPos = base.SelectionStart + SelectionLength;
			base.Select(0, TextLength);

			mUpdating = false;

			if (oldByteValue != mByteValue)
			{
				OnValueChanged(new ValueChangedEventArgs(mByteValue, mByteValue));
			}
		}

		private void this_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void checkAndUpdateValue(string newValue)
		{
			try
			{
				checkAndUpdateValue(newValue == "" ? (byte)0 : byte.Parse(newValue));
			}
			catch (FormatException)
			{
			}
		}

		protected virtual void OnValueChanged(ValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
			}
		}

		private void this_Enter(object sender, EventArgs e)
		{
			base.Select(0, TextLength);
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
					checkAndUpdateValue(mByteValue);

					return;
			}

			e.Handled = !char.IsNumber(keyChar) && !char.IsControl(keyChar);
		}

		private void this_Leave(object sender, EventArgs e)
		{
			checkAndUpdateValue(Text);
		}

		private void this_TextChanged(object sender, EventArgs e)
		{
			if (!mUpdating)
			{
				bool textIsValid = true;

				try
				{
					string text = Text;

					if (text != "")
					{
						byte byteValue = byte.Parse(text);
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
					base.Select(mLastCaretPos, 0);
					mUpdating = false;
				}
				else
				{
					mLastValidText = base.Text;
					mLastCaretPos = base.SelectionStart + SelectionLength;

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
			base.SuspendLayout();

			Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
			mRenderedTextColor = GuiSettings.GetColor(GuiSettings.RenderedText);
			mEditingTextColor = GuiSettings.GetColor(GuiSettings.EditingText);
			ForeColor = mRenderedTextColor;

			base.ResumeLayout();
		}

		public int Index
		{
			get
			{
				return mIndex;
			}
		}

		public MixByte MixByteValue
		{
			get
			{
				return mByteValue;
			}
			set
			{
				mByteValue = (byte)value;
				checkAndUpdateValue(mByteValue);
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
			private MixByte mNewValue;
			private MixByte mOldValue;

			public ValueChangedEventArgs(MixByte oldValue, MixByte newValue)
			{
				mOldValue = oldValue;
				mNewValue = newValue;
			}

			public MixByte NewValue
			{
				get
				{
					return mNewValue;
				}
			}

			public MixByte OldValue
			{
				get
				{
					return mOldValue;
				}
			}
		}

		public delegate void ValueChangedEventHandler(MixByteTextBox sender, MixByteTextBox.ValueChangedEventArgs args);
	}
}
