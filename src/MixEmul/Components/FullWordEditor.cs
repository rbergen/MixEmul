using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public class FullWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private readonly Label equalsLabel;
		private IFullWord fullWord;
		private bool readOnly;
		private readonly MixByteCollectionCharTextBox wordCharTextBox;
		private readonly WordValueEditor wordValueEditor;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public FullWordEditor(IFullWord fullWord = null)
		{
			this.fullWord = fullWord ?? new FullWord();
			this.readOnly = false;
			this.wordValueEditor = new WordValueEditor(this.fullWord);
			this.wordCharTextBox = new MixByteCollectionCharTextBox(this.fullWord);
			this.equalsLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> this.wordCharTextBox.Focused ? FieldTypes.Chars : this.wordValueEditor.FocusedField;

		public int? CaretIndex
			=> FocusedField == FieldTypes.Chars ? this.wordCharTextBox.CaretIndex : this.wordValueEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> field == FieldTypes.Chars ? this.wordCharTextBox.Focus(field, index) : this.wordValueEditor.Focus(field, index);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void InitializeComponent()
		{
			SuspendLayout();

			this.wordValueEditor.Location = new Point(0, 0);
			this.wordValueEditor.Name = "mWordValueEditor";
			this.wordValueEditor.TabIndex = 2;
			this.wordValueEditor.ValueChanged += MWordValueEditor_ValueChanged;
			this.wordValueEditor.NavigationKeyDown += This_KeyDown;

			this.equalsLabel.Location = new Point(this.wordValueEditor.Right, this.wordValueEditor.Top + 2);
			this.equalsLabel.Name = "mFirstEqualsLabel";
			this.equalsLabel.Size = new Size(10, this.wordValueEditor.Height - 2);
			this.equalsLabel.TabIndex = 3;
			this.equalsLabel.Text = "=";

			this.wordCharTextBox.Location = new Point(this.equalsLabel.Right, this.wordValueEditor.Top);
			this.wordCharTextBox.Name = "mWordCharTextBox";
			this.wordCharTextBox.Size = new Size(45, this.wordValueEditor.Height);
			this.wordCharTextBox.TabIndex = 4;
			this.wordCharTextBox.ValueChanged += MWordCharTextBox_ValueChanged;
			this.wordCharTextBox.NavigationKeyDown += This_KeyDown;

			Controls.Add(this.wordValueEditor);
			Controls.Add(this.equalsLabel);
			Controls.Add(this.wordCharTextBox);
			Name = "FullWordEditor";
			Size = new Size(this.wordCharTextBox.Right, this.wordCharTextBox.Height);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes editorField = FieldTypes.Chars;
			int? index = this.wordCharTextBox.SelectionStart + this.wordCharTextBox.SelectionLength;

			if (e is FieldKeyEventArgs args)
			{
				editorField = args.Field;
				index = args.Index;
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
					if (sender == this.wordValueEditor)
						this.wordCharTextBox.Focus();

					else if (sender == this.wordCharTextBox && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (sender == this.wordCharTextBox)
						this.wordValueEditor.Focus(FieldTypes.Value, null);

					else if (sender == this.wordValueEditor && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;
			}
		}

		private void MWordCharTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			this.wordValueEditor.Update();
			OnValueChanged(new WordEditorValueChangedEventArgs((Word)args.OldValue, (Word)args.NewValue));
		}

		private void MWordValueEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			this.wordCharTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			this.wordValueEditor.Update();
			this.wordCharTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			this.wordValueEditor.UpdateLayout();
			this.wordCharTextBox.UpdateLayout();
		}

		public IFullWord FullWord
		{
			get => this.fullWord;
			set
			{
				this.fullWord = value ?? throw new ArgumentNullException(nameof(value), "FullWord may not be set to null");
				this.wordValueEditor.WordValue = this.fullWord;
				this.wordCharTextBox.MixByteCollectionValue = this.fullWord;
			}
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (this.readOnly == value)
					return;

				this.readOnly = value;
				this.wordValueEditor.ReadOnly = this.readOnly;
				this.wordCharTextBox.ReadOnly = this.readOnly;
			}
		}

		public IWord WordValue
		{
			get => FullWord;
			set
			{
				if (value is not IFullWord)
					throw new ArgumentException("Value must be an IFullWord");

				FullWord = (IFullWord)value;
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Chars)
				this.wordCharTextBox.Select(start, length);

			else
				this.wordValueEditor.Select(start, length);
		}
	}
}
