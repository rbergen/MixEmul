using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
	public class WordValueEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
		private readonly Label _equalsLabel;
		private bool _includeSign;
		private readonly LongValueTextBox _longValueTextBox;
		private bool _readOnly;
		private int _textBoxWidth;
		private readonly WordEditor _wordEditor;

		public event KeyEventHandler NavigationKeyDown;

		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordValueEditor() : this(FullWord.ByteCount, true) {}

		public WordValueEditor(IWord word)  : this(word, true) {}

		public WordValueEditor(int byteCount) : this(byteCount, true) {}

		public WordValueEditor(IWord word, bool includeSign)
			: this(word.ByteCount, includeSign) => WordValue = word;

		public WordValueEditor(int byteCount, bool includeSign)
		{
			_includeSign = includeSign;
			_readOnly = false;
			_textBoxWidth = 80;

			_wordEditor = new WordEditor(byteCount, includeSign);
			_equalsLabel = new Label();
			_longValueTextBox = new LongValueTextBox();

			_wordEditor.Name = "WordEditor";
			_wordEditor.Location = new Point(0, 0);
			_wordEditor.ReadOnly = _readOnly;
			_wordEditor.TabIndex = 0;
			_wordEditor.TabStop = true;
			_wordEditor.ValueChanged += WordEditor_ValueChanged;
			_wordEditor.NavigationKeyDown += This_KeyDown;

			_equalsLabel.Name = "EqualsLabel";
			_equalsLabel.TabIndex = 1;
			_equalsLabel.TabStop = false;
			_equalsLabel.Text = "=";
			_equalsLabel.Size = new Size(10, 19);

			_longValueTextBox.Name = "LongValueTextBox";
			_longValueTextBox.ReadOnly = _readOnly;
			_longValueTextBox.SupportNegativeZero = true;
			_longValueTextBox.TabIndex = 2;
			_longValueTextBox.TabStop = true;
			_longValueTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			_longValueTextBox.ValueChanged += TextBox_ValueChanged;
			_longValueTextBox.NavigationKeyDown += This_KeyDown;
			_longValueTextBox.Height = _wordEditor.Height;

			SuspendLayout();
			Controls.Add(_wordEditor);
			Controls.Add(_equalsLabel);
			Controls.Add(_longValueTextBox);
			Name = "WordValueEditor";
			KeyDown += This_KeyDown;

			SizeComponent();
		}

		public Control EditorControl 
			=> this;

		public FieldTypes? FocusedField 
			=> _longValueTextBox.Focused ? FieldTypes.Value : _wordEditor.FocusedField;

		public int? CaretIndex 
			=> FocusedField == FieldTypes.Value 
			? _longValueTextBox.SelectionStart + _longValueTextBox.SelectionLength 
			: _wordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index) 
			=> field == FieldTypes.Value 
			? _longValueTextBox.FocusWithIndex(index) 
			: _wordEditor.Focus(field, index);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) 
			=> ValueChanged?.Invoke(this, args);

		protected override void Dispose(bool disposing) 
		{
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes editorField = FieldTypes.Word;
			int? index = null;

			if (sender == _longValueTextBox)
			{
				editorField = FieldTypes.Value;
				index = _longValueTextBox.SelectionLength + _longValueTextBox.SelectionStart;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, new FieldKeyEventArgs(e.KeyData, editorField, index));
					break;

				case Keys.Right:
					if (sender == _wordEditor)
						_longValueTextBox.Focus();

					else if (sender == _longValueTextBox && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (sender == _longValueTextBox)
						_wordEditor.Focus(FieldTypes.LastByte, null);

					else if (sender == _wordEditor && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;
			}
		}

		private void SizeComponent()
		{
			SuspendLayout();

			_equalsLabel.Location = new Point(_wordEditor.Width, 2);
			_longValueTextBox.AutoSize = false;
			_longValueTextBox.MinValue = IncludeSign ? -_wordEditor.WordValue.MaxMagnitude : 0L;
			_longValueTextBox.MaxValue = _wordEditor.WordValue.MaxMagnitude;
			_longValueTextBox.Location = new Point(_equalsLabel.Left + _equalsLabel.Width, 0);
			_longValueTextBox.Size = new Size(TextBoxWidth, _wordEditor.Height);
			_longValueTextBox.Magnitude = _wordEditor.WordValue.MagnitudeLongValue;
			_longValueTextBox.Sign = _wordEditor.WordValue.Sign;
			Size = new Size(_longValueTextBox.Left + TextBoxWidth, _wordEditor.Height);

			ResumeLayout(false);
		}

		private void TextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			IWord wordValue = _wordEditor.WordValue;
			IWord oldValue = new Word(wordValue.Magnitude, wordValue.Sign);
			wordValue.MagnitudeLongValue = _longValueTextBox.Magnitude;
			wordValue.Sign = _longValueTextBox.Sign;
			_wordEditor.Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new Word(wordValue.Magnitude, wordValue.Sign)));
		}

		public new void Update()
		{
			_wordEditor.Update();
			_longValueTextBox.Magnitude = _wordEditor.WordValue.MagnitudeLongValue;
			_longValueTextBox.Sign = _wordEditor.WordValue.Sign;
			base.Update();
		}

		public void UpdateLayout()
		{
			_wordEditor.UpdateLayout();
			_longValueTextBox.UpdateLayout();
		}

		private void WordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			_longValueTextBox.Magnitude = _wordEditor.WordValue.MagnitudeLongValue;
			_longValueTextBox.Sign = _wordEditor.WordValue.Sign;
			OnValueChanged(args);
		}

		public int ByteCount
		{
			get => _wordEditor.ByteCount;
			set
			{
				if (ByteCount != value)
				{
					_wordEditor.ByteCount = value;
					SizeComponent();
				}
			}
		}

		public bool IncludeSign
		{
			get => _includeSign;
			set
			{
				if (_includeSign != value)
				{
					_includeSign = value;
					_wordEditor.IncludeSign = _includeSign;
					SizeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (ReadOnly != value)
				{
					_readOnly = value;
					_wordEditor.ReadOnly = _readOnly;
					_longValueTextBox.ReadOnly = _readOnly;
				}
			}
		}

		public int TextBoxWidth
		{
			get => _textBoxWidth;
			set
			{
				if (_textBoxWidth != value)
				{
					_textBoxWidth = value;
					SizeComponent();
				}
			}
		}

		public IWord WordValue
		{
			get => _wordEditor.WordValue;
			set
			{
				int byteCount = ByteCount;
				_wordEditor.WordValue = value;

				if (byteCount == value.ByteCount)
					Update();

				else
					SizeComponent();
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Value)
				_longValueTextBox.Select(start, length);

			else
				_wordEditor.Select(start, length);
		}
	}
}
