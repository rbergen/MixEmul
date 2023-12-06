using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
	public class MixByteCollectionCharTextBox : TextBox, IMixByteCollectionEditor, IEscapeConsumer, INavigableControl
	{
		private Color editingTextColor;
		private bool editMode;
		private int lastCaretPos;
		private MixByte[] lastRenderedBytes;
		private string lastValidText;
		private Color renderedTextColor;
		private bool updating;
		private IMixByteCollection byteCollection;

		public bool UseEditMode { get; set; }

		public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;
		public event KeyEventHandler NavigationKeyDown;

		public MixByteCollectionCharTextBox(IMixByteCollection byteCollection = null)
		{
			this.byteCollection = byteCollection ?? new FullWord();
			UseEditMode = true;

			SuspendLayout();

			CharacterCasing = CharacterCasing.Upper;
			Location = new Point(40, 16);
			Size = new Size(48, 21);
			BorderStyle = BorderStyle.FixedSingle;
			KeyDown += This_KeyDown;
			KeyPress += This_KeyPress;
			Leave += This_Leave;
			TextChanged += This_TextChanged;

			ResumeLayout(false);

			UpdateLayout();

			this.updating = false;
			this.editMode = false;
			this.lastRenderedBytes = null;
			Update();
		}

		public Control EditorControl
			=> this;

		public bool Focus(FieldTypes? field, int? index)
			=> this.FocusWithIndex(index);

		public FieldTypes? FocusedField
			=> FieldTypes.Chars;

		public int? CaretIndex
			=> SelectionStart + SelectionLength;

		protected void OnValueChanged(MixByteCollectionEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void This_Leave(object sender, EventArgs e)
			=> UpdateValue();

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
					NavigationKeyDown?.Invoke(this, new IndexKeyEventArgs(e.KeyData, SelectionStart + SelectionLength));
					break;

				case Keys.Right:
					if (SelectionStart + SelectionLength == TextLength && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (SelectionStart + SelectionLength == 0 && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Enter:
					e.Handled = true;
					break;
			}
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;
			if (this.editMode)
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

		private void This_TextChanged(object sender, EventArgs e)
		{
			if (this.updating)
				return;

			string validText = string.Empty;

			foreach (var c in Text.Where(c => MixByte.MixChars.Contains(c)))
				validText += c;

			if (validText.Length > this.byteCollection.MaxByteCount)
				validText = validText.Substring(0, this.byteCollection.MaxByteCount);

			if (validText.Length > 0 || TextLength == 0)
			{
				this.lastValidText = validText;
				this.lastCaretPos = SelectionStart + SelectionLength;

				if (!UseEditMode)
				{
					UpdateValue();
				}
				else if (!this.editMode)
				{
					ForeColor = this.editingTextColor;
					this.editMode = true;
				}
			}

			this.updating = true;

			base.Text = this.lastValidText;
			Select(this.lastCaretPos, 0);

			this.updating = false;
		}

		private static bool ArraysEqual(MixByte[] one, MixByte[] two)
		{
			if ((one == null && two != null) || (one != null && two == null))
				return false;

			if (one == null && two == null)
				return true;

			if (one.Length != two.Length)
				return false;

			for (int i = 0; i < one.Length; i++)
			{
				if (one[i] != two[i])
					return false;
			}

			return true;
		}

		public new void Update()
		{
			var byteCollectionBytes = this.byteCollection.ToArray();

			if (!this.editMode && this.lastRenderedBytes != null && ArraysEqual(this.lastRenderedBytes, byteCollectionBytes))
				return;

			this.editMode = false;
			this.lastRenderedBytes = byteCollectionBytes;

			var textValue = this.byteCollection.ToString(true).TrimEnd();

			this.updating = true;
			SuspendLayout();

			ForeColor = this.renderedTextColor;
			this.lastValidText = textValue;
			this.lastCaretPos = SelectionStart + SelectionLength;
			Text = this.lastValidText;
			Select(this.lastCaretPos, 0);

			ResumeLayout();
			this.updating = false;
			base.Update();
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

		private void UpdateValue()
		{
			int currentCharIndex = 0;
			var oldValue = (IMixByteCollection)this.byteCollection.Clone();

			bool valueDiffers = false;

			while (currentCharIndex < Math.Min(TextLength, this.byteCollection.MaxByteCount))
			{
				if (this.byteCollection[currentCharIndex].CharValue != Text[currentCharIndex])
				{
					valueDiffers = true;

					if (Text[currentCharIndex] != MixByte.MixChars[^1])
						this.byteCollection[currentCharIndex] = Text[currentCharIndex];
				}

				currentCharIndex++;
			}

			while (currentCharIndex < this.byteCollection.MaxByteCount)
			{
				if (this.byteCollection[currentCharIndex].CharValue != ' ')
				{
					valueDiffers = true;
					this.byteCollection[currentCharIndex] = ' ';
				}

				currentCharIndex++;
			}

			if (valueDiffers)
			{
				var newValue = (IMixByteCollection)this.byteCollection.Clone();
				OnValueChanged(new MixByteCollectionEditorValueChangedEventArgs(oldValue, newValue));
			}

			Update();
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get => this.byteCollection;
			set
			{
				if (value != null)
				{
					this.byteCollection = value;

					if (TextLength > this.byteCollection.MaxByteCount)
						Text = Text.Substring(0, this.byteCollection.MaxByteCount);

					Select(TextLength, 0);

					Update();
				}
			}
		}
	}
}
