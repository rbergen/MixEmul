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

		private MixByteTextBox[] byteTextBoxes;
		private int focusByte;
		private int focusByteSelLength;
		private int focusByteSelStart;
		private bool includeSign;
		private long lastRenderedMagnitude;
		private Word.Signs lastRenderedSign;
		private bool readOnly;
		private readonly Button signButton;
		private IWord word;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordEditor() : this(FullWord.ByteCount) { }

		public WordEditor(IWord word) : this(word, true) { }

		public WordEditor(int byteCount) : this(byteCount, true) { }

		public WordEditor(IWord word, bool includeSign) : this(word.ByteCount, includeSign) 
			=> WordValue = word;

		public WordEditor(int byteCount, bool includeSign)
		{
			this.word = new Word(byteCount);
			this.includeSign = includeSign;
			this.readOnly = false;
			this.lastRenderedMagnitude = Unrendered;
			this.lastRenderedSign = Word.Signs.Positive;

			this.signButton = new Button
			{
				Location = new Point(0, 0),
				Name = "SignButton",
				Size = new Size(18, 21),
				TabIndex = 0,
				TabStop = false,
				Text = this.word.Sign.ToChar().ToString(),
				FlatStyle = FlatStyle.Flat,
				Enabled = !ReadOnly
			};

			this.signButton.Click += SignButton_Click;
			this.signButton.KeyPress += Editor_KeyPress;
			this.signButton.KeyDown += This_KeyDown;

			InitializeComponent();
		}

		public int? CaretIndex 
			=> null;

		public Control EditorControl 
			=> this;

		public bool Focus(FieldTypes? field, int? index) 
			=> ByteCount > 0 
			? this.byteTextBoxes[field == FieldTypes.LastByte ? ByteCount - 1 : 0].Focus() 
			: this.signButton.Focus();

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) 
			=> ValueChanged?.Invoke(this, args);

		private void ByteValueChanged(MixByteTextBox textBox, MixByteTextBox.ValueChangedEventArgs args)
		{
			var oldValue = new Word(this.word.Magnitude, this.word.Sign);
			this.word[textBox.Index] = args.NewValue;
			this.lastRenderedMagnitude = this.word.MagnitudeLongValue;
			this.lastRenderedSign = this.word.Sign;

			var newValue = new Word(this.word.Magnitude, this.word.Sign);
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

			if (IncludeSign && (keyChar == '-' || (keyChar == '+' && this.word.Sign.IsNegative())))
				NegateSign();
		}

		private void Editor_Leave(object sender, EventArgs e)
		{
			var box = (MixByteTextBox)sender;
			this.focusByte = box.Index;
			this.focusByteSelStart = box.SelectionStart;
			this.focusByteSelLength = box.SelectionLength;
		}

		private void InitializeComponent()
		{
			this.focusByte = 0;
			this.focusByteSelStart = 0;
			this.focusByteSelLength = 0;
			int tabIndex = 0;
			int width = 0;

			SuspendLayout();
			if (IncludeSign)
			{
				Controls.Add(this.signButton);
				tabIndex++;
				width += this.signButton.Width - 1;
			}

			this.byteTextBoxes = new MixByteTextBox[ByteCount];
			for (int i = 0; i < ByteCount; i++)
			{
				var box = new MixByteTextBox(i)
				{
					Location = new Point(width, 0),
					Name = "MixByteEditor" + i,
					MixByteValue = this.word[i],
					ReadOnly = ReadOnly,
					TabIndex = tabIndex
				};
				box.ValueChanged += ByteValueChanged;
				box.Click += Editor_Click;
				box.Leave += Editor_Leave;
				box.KeyPress += Editor_KeyPress;
				box.KeyDown += This_KeyDown;

				this.byteTextBoxes[i] = box;

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
					if (sender == this.signButton && ByteCount > 0)
					{
						this.byteTextBoxes[0].Focus();
						this.byteTextBoxes[0].Select(0, 2);
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == box.TextLength)
					{
						var byteIndex = Array.IndexOf(this.byteTextBoxes, box);
						if (byteIndex < ByteCount - 1)
						{
							this.byteTextBoxes[byteIndex + 1].Focus();
							this.byteTextBoxes[byteIndex + 1].Select(0, 2);
						}
						else
							NavigationKeyDown?.Invoke(this, e);
					}

					break;

				case Keys.Left:
					if (sender == this.signButton)
					{
						NavigationKeyDown?.Invoke(this, e);

						e.Handled = true;
						break;
					}

					box = (MixByteTextBox)sender;
					if (box.SelectionStart + box.SelectionLength == 0)
					{
						var byteIndex = Array.IndexOf(this.byteTextBoxes, box);
						if (byteIndex > 0)
						{
							this.byteTextBoxes[byteIndex - 1].Focus();
							this.byteTextBoxes[byteIndex - 1].Select(0, 2);
						}
						else
							this.signButton.Focus();
					}

					break;
			}
		}

		private void SignButton_Click(object sender, EventArgs e)
		{
			NegateSign();
			if (this.byteTextBoxes.Length > 0)
			{
				this.byteTextBoxes[this.focusByte].Focus();
				this.byteTextBoxes[this.focusByte].SelectionStart = this.focusByteSelStart;
				this.byteTextBoxes[this.focusByte].SelectionLength = this.focusByteSelLength;
			}
		}

		private void NegateSign()
		{
			Word.Signs sign = this.word.Sign;
			this.word.InvertSign();
			this.lastRenderedMagnitude = this.word.MagnitudeLongValue;
			this.lastRenderedSign = this.word.Sign;
			this.signButton.Text = this.word.Sign.ToChar().ToString();

			var oldValue = new Word(this.word.Magnitude, sign);
			var newValue = new Word(this.word.Magnitude, this.word.Sign);
			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, newValue));
		}

		private void ReinitializeComponent()
		{
			Controls.Clear();

			foreach (MixByteTextBox box in this.byteTextBoxes)
				box.Dispose();

			InitializeComponent();
		}

		public new void Update()
		{
			if (this.lastRenderedMagnitude != this.word.MagnitudeLongValue || this.lastRenderedSign != this.word.Sign || this.lastRenderedMagnitude == Unrendered)
			{
				this.signButton.Text = this.word.Sign.ToChar().ToString();

				foreach (MixByteTextBox box in this.byteTextBoxes)
					box.MixByteValue = this.word[box.Index];

				base.Update();
				this.lastRenderedMagnitude = this.word.MagnitudeLongValue;
				this.lastRenderedSign = this.word.Sign;
			}
		}

		public void UpdateLayout()
		{
			foreach (MixByteTextBox box in this.byteTextBoxes)
				box.UpdateLayout();
		}

		public int ByteCount
		{
			get => this.word.ByteCount;
			set
			{
				if (ByteCount != value)
				{
					this.word = new Word(value);
					ReinitializeComponent();
				}
			}
		}

		public bool IncludeSign
		{
			get => this.includeSign;
			set
			{
				if (this.includeSign != value)
				{
					this.includeSign = value;
					ReinitializeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (ReadOnly == value)
					return;

				this.readOnly = value;
				this.signButton.Enabled = !ReadOnly;

				foreach (MixByteTextBox box in this.byteTextBoxes)
					box.ReadOnly = ReadOnly;
			}
		}

		public IWord WordValue
		{
			get => this.word;
			set
			{
				int byteCount = ByteCount;
				this.word = value;

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
				if (this.signButton.Focused)
					return FieldTypes.Word;

				foreach (MixByteTextBox byteBox in this.byteTextBoxes)
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
