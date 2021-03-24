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
		private Color _editingTextColor;
		private bool _editMode;
		private int _lastCaretPos;
		private MixByte[] _lastRenderedBytes;
		private string _lastValidText;
		private Color _renderedTextColor;
		private bool _updating;
		private IMixByteCollection _byteCollection;

		public bool UseEditMode { get; set; }

		public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;
		public event KeyEventHandler NavigationKeyDown;

		public MixByteCollectionCharTextBox(IMixByteCollection byteCollection = null)
		{
			_byteCollection = byteCollection ?? new FullWord();
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

			_updating = false;
			_editMode = false;
			_lastRenderedBytes = null;
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
			if (_editMode)
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
			if (_updating)
				return;

			string validText = "";

			foreach (var c in Text.Where(c => MixByte.MixChars.Contains(c)))
				validText += c;

			if (validText.Length > _byteCollection.MaxByteCount)
				validText = validText.Substring(0, _byteCollection.MaxByteCount);

			if (validText.Length > 0 || TextLength == 0)
			{
				_lastValidText = validText;
				_lastCaretPos = SelectionStart + SelectionLength;

				if (!UseEditMode)
				{
					UpdateValue();
				}
				else if (!_editMode)
				{
					ForeColor = _editingTextColor;
					_editMode = true;
				}
			}

			_updating = true;

			base.Text = _lastValidText;
			Select(_lastCaretPos, 0);

			_updating = false;
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
			var byteCollectionBytes = _byteCollection.ToArray();

			if (!_editMode && _lastRenderedBytes != null && ArraysEqual(_lastRenderedBytes, byteCollectionBytes))
				return;

			_editMode = false;
			_lastRenderedBytes = byteCollectionBytes;

			var textValue = _byteCollection.ToString(true).TrimEnd();

			_updating = true;
			SuspendLayout();

			ForeColor = _renderedTextColor;
			_lastValidText = textValue;
			_lastCaretPos = SelectionStart + SelectionLength;
			Text = _lastValidText;
			Select(_lastCaretPos, 0);

			ResumeLayout();
			_updating = false;
			base.Update();
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

		private void UpdateValue()
		{
			int currentCharIndex = 0;
			var oldValue = (IMixByteCollection)_byteCollection.Clone();

			bool valueDiffers = false;

			while (currentCharIndex < Math.Min(TextLength, _byteCollection.MaxByteCount))
			{
				if (_byteCollection[currentCharIndex].CharValue != Text[currentCharIndex])
				{
					valueDiffers = true;

					if (Text[currentCharIndex] != MixByte.MixChars[^1])
						_byteCollection[currentCharIndex] = Text[currentCharIndex];
				}

				currentCharIndex++;
			}

			while (currentCharIndex < _byteCollection.MaxByteCount)
			{
				if (_byteCollection[currentCharIndex].CharValue != ' ')
				{
					valueDiffers = true;
					_byteCollection[currentCharIndex] = ' ';
				}

				currentCharIndex++;
			}

			if (valueDiffers)
			{
				var newValue = (IMixByteCollection)_byteCollection.Clone();
				OnValueChanged(new MixByteCollectionEditorValueChangedEventArgs(oldValue, newValue));
			}

			Update();
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get => _byteCollection;
			set
			{
				if (value != null)
				{
					_byteCollection = value;

					if (TextLength > _byteCollection.MaxByteCount)
						Text = Text.Substring(0, _byteCollection.MaxByteCount);

					Select(TextLength, 0);

					Update();
				}
			}
		}
	}
}
