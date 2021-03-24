using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public class WordEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
		private const long Unrendered = long.MinValue;

		private MixByteTextBox[] _byteTextBoxes;
		private int _focusByte;
		private int _focusByteSelLength;
		private int _focusByteSelStart;
		private bool _includeSign;
		private long _lastRenderedMagnitude;
		private Word.Signs _lastRenderedSign;
		private bool _readOnly;
		private readonly Button _signButton;
		private IWord _word;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordEditor() : this(FullWord.ByteCount) {}

		public WordEditor(IWord word) : this(word, true) {}

		public WordEditor(int byteCount) : this(byteCount, true) {}

		public WordEditor(IWord word, bool includeSign) : this(word.ByteCount, includeSign) 
			=> WordValue = word;

		public WordEditor(int byteCount, bool includeSign)
		{
			_word = new Word(byteCount);
			_includeSign = includeSign;
			_readOnly = false;
			_lastRenderedMagnitude = Unrendered;
			_lastRenderedSign = Word.Signs.Positive;

			_signButton = new Button
			{
				Location = new Point(0, 0),
				Name = "SignButton",
				Size = new Size(18, 21),
				TabIndex = 0,
				TabStop = false,
				Text = "" + _word.Sign.ToChar(),
				FlatStyle = FlatStyle.Flat,
				Enabled = !ReadOnly
			};

			_signButton.Click += SignButton_Click;
			_signButton.KeyPress += Editor_KeyPress;
			_signButton.KeyDown += This_KeyDown;

			InitializeComponent();
		}

		public int? CaretIndex 
			=> null;

		public Control EditorControl 
			=> this;

		public bool Focus(FieldTypes? field, int? index) 
			=> ByteCount > 0 
			? _byteTextBoxes[field == FieldTypes.LastByte ? ByteCount - 1 : 0].Focus() 
			: _signButton.Focus();

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) 
			=> ValueChanged?.Invoke(this, args);

		private void ByteValueChanged(MixByteTextBox textBox, MixByteTextBox.ValueChangedEventArgs args)
		{
			var oldValue = new Word(_word.Magnitude, _word.Sign);
			_word[textBox.Index] = args.NewValue;
			_lastRenderedMagnitude = _word.MagnitudeLongValue;
			_lastRenderedSign = _word.Sign;

			var newValue = new Word(_word.Magnitude, _word.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}

		private void Editor_Click(object sender, EventArgs e)
		{
			var box = (MixByteTextBox)sender;

			if (box.SelectionLength == 0)
				box.Select(0, 2);
		}

		private void Editor_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;

			if (IncludeSign && (keyChar == '-' || (keyChar == '+' && _word.Sign.IsNegative())))
				NegateSign();
		}

		private void Editor_Leave(object sender, EventArgs e)
		{
			var box = (MixByteTextBox)sender;
			_focusByte = box.Index;
			_focusByteSelStart = box.SelectionStart;
			_focusByteSelLength = box.SelectionLength;
		}

		private void InitializeComponent()
		{
			_focusByte = 0;
			_focusByteSelStart = 0;
			_focusByteSelLength = 0;
			int tabIndex = 0;
			int width = 0;

			SuspendLayout();
			if (IncludeSign)
			{
				Controls.Add(_signButton);
				tabIndex++;
				width += _signButton.Width - 1;
			}

			_byteTextBoxes = new MixByteTextBox[ByteCount];
			for (int i = 0; i < ByteCount; i++)
			{
				var box = new MixByteTextBox(i)
				{
					Location = new Point(width, 0),
					Name = "MixByteEditor" + i,
					MixByteValue = _word[i],
					ReadOnly = ReadOnly,
					TabIndex = tabIndex
				};
				box.ValueChanged += ByteValueChanged;
				box.Click += Editor_Click;
				box.Leave += Editor_Leave;
				box.KeyPress += Editor_KeyPress;
				box.KeyDown += This_KeyDown;

				_byteTextBoxes[i] = box;

				Controls.Add(box);

				tabIndex++;
				width += box.Width - 1;
			}

			Name = "WordEditor";
			Size = new Size(width + 1, 21);
			ResumeLayout(false);
			Update();
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			MixByteTextBox box;

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, e);
					break;

				case Keys.Right:
					if (sender == _signButton && ByteCount > 0)
					{
						_byteTextBoxes[0].Focus();
						_byteTextBoxes[0].Select(0, 2);
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == box.TextLength)
					{
						var byteIndex = Array.IndexOf(_byteTextBoxes, box);
						if (byteIndex < ByteCount - 1)
						{
							_byteTextBoxes[byteIndex + 1].Focus();
							_byteTextBoxes[byteIndex + 1].Select(0, 2);
						}
						else
							NavigationKeyDown?.Invoke(this, e);
					}

					break;

				case Keys.Left:
					if (sender == _signButton)
					{
						NavigationKeyDown?.Invoke(this, e);

						e.Handled = true;
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == 0)
					{
						var byteIndex = Array.IndexOf(_byteTextBoxes, box);
						if (byteIndex > 0)
						{
							_byteTextBoxes[byteIndex - 1].Focus();
							_byteTextBoxes[byteIndex - 1].Select(0, 2);
						}
						else
							_signButton.Focus();
					}

					break;
			}
		}

		private void SignButton_Click(object sender, EventArgs e)
		{
			NegateSign();
			if (_byteTextBoxes.Length > 0)
			{
				_byteTextBoxes[_focusByte].Focus();
				_byteTextBoxes[_focusByte].SelectionStart = _focusByteSelStart;
				_byteTextBoxes[_focusByte].SelectionLength = _focusByteSelLength;
			}
		}

		private void NegateSign()
		{
			Word.Signs sign = _word.Sign;
			_word.InvertSign();
			_lastRenderedMagnitude = _word.MagnitudeLongValue;
			_lastRenderedSign = _word.Sign;
			_signButton.Text = "" + _word.Sign.ToChar();

			var oldValue = new Word(_word.Magnitude, sign);
			var newValue = new Word(_word.Magnitude, _word.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}

		private void ReinitializeComponent()
		{
			Controls.Clear();

			foreach (MixByteTextBox box in _byteTextBoxes)
				box.Dispose();

			InitializeComponent();
		}

		public new void Update()
		{
			if (_lastRenderedMagnitude != _word.MagnitudeLongValue || _lastRenderedSign != _word.Sign || _lastRenderedMagnitude == Unrendered)
			{
				_signButton.Text = "" + _word.Sign.ToChar();

				foreach (MixByteTextBox box in _byteTextBoxes)
					box.MixByteValue = _word[box.Index];

				base.Update();
				_lastRenderedMagnitude = _word.MagnitudeLongValue;
				_lastRenderedSign = _word.Sign;
			}
		}

		public void UpdateLayout()
		{
			foreach (MixByteTextBox box in _byteTextBoxes)
				box.UpdateLayout();
		}

		public int ByteCount
		{
			get => _word.ByteCount;
			set
			{
				if (ByteCount != value)
				{
					_word = new Word(value);
					ReinitializeComponent();
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
					ReinitializeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (ReadOnly == value)
					return;

				_readOnly = value;
				_signButton.Enabled = !ReadOnly;

				foreach (MixByteTextBox box in _byteTextBoxes)
					box.ReadOnly = ReadOnly;
			}
		}

		public IWord WordValue
		{
			get => _word;
			set
			{
				int byteCount = ByteCount;
				_word = value;

				if (byteCount == value.ByteCount)
					Update();

				else
					ReinitializeComponent();
			}
		}

		public FieldTypes? FocusedField
		{
			get
			{
				if (_signButton.Focused)
					return FieldTypes.Word;

				foreach (MixByteTextBox byteBox in _byteTextBoxes)
				{
					if (byteBox.Focused)
						return FieldTypes.Word;
				}

				return null;
			}
		}

		public void Select(int start, int length) { }
	}
}
