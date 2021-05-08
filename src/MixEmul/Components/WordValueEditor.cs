using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
	public class WordValueEditor : UserControl, IWordEditor, INavigableControl, IEscapeConsumer
	{
		private readonly Label equalsLabel;
		private bool includeSign;
		private readonly LongValueTextBox longValueTextBox;
		private bool readOnly;
		private int textBoxWidth;
		private readonly WordEditor wordEditor;

		public event KeyEventHandler NavigationKeyDown;

		public event WordEditorValueChangedEventHandler ValueChanged;

		public WordValueEditor() : this(FullWord.ByteCount, true) { }

		public WordValueEditor(IWord word)  : this(word, true) { }

		public WordValueEditor(int byteCount) : this(byteCount, true) { }

		public WordValueEditor(IWord word, bool includeSign)
			: this(word.ByteCount, includeSign) => WordValue = word;

		public WordValueEditor(int byteCount, bool includeSign)
		{
			this.includeSign = includeSign;
			this.readOnly = false;
			this.textBoxWidth = 80;

			this.wordEditor = new WordEditor(byteCount, includeSign);
			this.equalsLabel = new Label();
			this.longValueTextBox = new LongValueTextBox();

			this.wordEditor.Name = "WordEditor";
			this.wordEditor.Location = new Point(0, 0);
			this.wordEditor.ReadOnly = this.readOnly;
			this.wordEditor.TabIndex = 0;
			this.wordEditor.TabStop = true;
			this.wordEditor.ValueChanged += WordEditor_ValueChanged;
			this.wordEditor.NavigationKeyDown += This_KeyDown;

			this.equalsLabel.Name = "EqualsLabel";
			this.equalsLabel.TabIndex = 1;
			this.equalsLabel.TabStop = false;
			this.equalsLabel.Text = "=";
			this.equalsLabel.Size = new Size(10, 19);

			this.longValueTextBox.Name = "LongValueTextBox";
			this.longValueTextBox.ReadOnly = this.readOnly;
			this.longValueTextBox.SupportNegativeZero = true;
			this.longValueTextBox.TabIndex = 2;
			this.longValueTextBox.TabStop = true;
			this.longValueTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			this.longValueTextBox.ValueChanged += TextBox_ValueChanged;
			this.longValueTextBox.NavigationKeyDown += This_KeyDown;
			this.longValueTextBox.Height = this.wordEditor.Height;

			SuspendLayout();
			Controls.Add(this.wordEditor);
			Controls.Add(this.equalsLabel);
			Controls.Add(this.longValueTextBox);
			Name = "WordValueEditor";
			KeyDown += This_KeyDown;

			SizeComponent();
		}

		public Control EditorControl 
			=> this;

		public FieldTypes? FocusedField 
			=> this.longValueTextBox.Focused ? FieldTypes.Value : this.wordEditor.FocusedField;

		public int? CaretIndex 
			=> FocusedField == FieldTypes.Value 
			? this.longValueTextBox.SelectionStart + this.longValueTextBox.SelectionLength 
			: this.wordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index) 
			=> field == FieldTypes.Value 
			? this.longValueTextBox.FocusWithIndex(index) 
			: this.wordEditor.Focus(field, index);

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

			if (sender == this.longValueTextBox)
			{
				editorField = FieldTypes.Value;
				index = this.longValueTextBox.SelectionLength + this.longValueTextBox.SelectionStart;
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
					if (sender == this.wordEditor)
						this.longValueTextBox.Focus();

					else if (sender == this.longValueTextBox && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (sender == this.longValueTextBox)
						this.wordEditor.Focus(FieldTypes.LastByte, null);

					else if (sender == this.wordEditor && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;
			}
		}

		private void SizeComponent()
		{
			SuspendLayout();

			this.equalsLabel.Location = new Point(this.wordEditor.Width, 2);
			this.longValueTextBox.AutoSize = false;
			this.longValueTextBox.MinValue = IncludeSign ? -this.wordEditor.WordValue.MaxMagnitude : 0L;
			this.longValueTextBox.MaxValue = this.wordEditor.WordValue.MaxMagnitude;
			this.longValueTextBox.Location = new Point(this.equalsLabel.Left + this.equalsLabel.Width, 0);
			this.longValueTextBox.Size = new Size(TextBoxWidth, this.wordEditor.Height);
			this.longValueTextBox.Magnitude = this.wordEditor.WordValue.MagnitudeLongValue;
			this.longValueTextBox.Sign = this.wordEditor.WordValue.Sign;
			Size = new Size(this.longValueTextBox.Left + TextBoxWidth, this.wordEditor.Height);

			ResumeLayout(false);
		}

		private void TextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			IWord wordValue = this.wordEditor.WordValue;
			IWord oldValue = new Word(wordValue.Magnitude, wordValue.Sign);
			wordValue.MagnitudeLongValue = this.longValueTextBox.Magnitude;
			wordValue.Sign = this.longValueTextBox.Sign;
			this.wordEditor.Update();

			OnValueChanged(new WordEditorValueChangedEventArgs(oldValue, new Word(wordValue.Magnitude, wordValue.Sign)));
		}

		public new void Update()
		{
			this.wordEditor.Update();
			this.longValueTextBox.Magnitude = this.wordEditor.WordValue.MagnitudeLongValue;
			this.longValueTextBox.Sign = this.wordEditor.WordValue.Sign;
			base.Update();
		}

		public void UpdateLayout()
		{
			this.wordEditor.UpdateLayout();
			this.longValueTextBox.UpdateLayout();
		}

		private void WordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			this.longValueTextBox.Magnitude = this.wordEditor.WordValue.MagnitudeLongValue;
			this.longValueTextBox.Sign = this.wordEditor.WordValue.Sign;
			OnValueChanged(args);
		}

		public int ByteCount
		{
			get => this.wordEditor.ByteCount;
			set
			{
				if (ByteCount != value)
				{
					this.wordEditor.ByteCount = value;
					SizeComponent();
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
					this.wordEditor.IncludeSign = this.includeSign;
					SizeComponent();
				}
			}
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (ReadOnly != value)
				{
					this.readOnly = value;
					this.wordEditor.ReadOnly = this.readOnly;
					this.longValueTextBox.ReadOnly = this.readOnly;
				}
			}
		}

		public int TextBoxWidth
		{
			get => this.textBoxWidth;
			set
			{
				if (this.textBoxWidth != value)
				{
					this.textBoxWidth = value;
					SizeComponent();
				}
			}
		}

		public IWord WordValue
		{
			get => this.wordEditor.WordValue;
			set
			{
				int byteCount = ByteCount;
				this.wordEditor.WordValue = value;

				if (byteCount == value.ByteCount)
					Update();

				else
					SizeComponent();
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Value)
				this.longValueTextBox.Select(start, length);

			else
				this.wordEditor.Select(start, length);
		}
	}
}
