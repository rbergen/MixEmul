using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public class FullWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private readonly Label _equalsLabel;
		private IFullWord _fullWord;
		private bool _readOnly;
		private readonly MixByteCollectionCharTextBox _wordCharTextBox;
		private readonly WordValueEditor _wordValueEditor;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public FullWordEditor(IFullWord fullWord = null)
		{
			_fullWord = fullWord ?? new FullWord();
			_readOnly = false;
			_wordValueEditor = new WordValueEditor(_fullWord);
			_wordCharTextBox = new MixByteCollectionCharTextBox(_fullWord);
			_equalsLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> _wordCharTextBox.Focused ? FieldTypes.Chars : _wordValueEditor.FocusedField;

		public int? CaretIndex
			=> FocusedField == FieldTypes.Chars ? _wordCharTextBox.CaretIndex : _wordValueEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> field == FieldTypes.Chars ? _wordCharTextBox.Focus(field, index) : _wordValueEditor.Focus(field, index);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void InitializeComponent()
		{
			SuspendLayout();

			_wordValueEditor.Location = new Point(0, 0);
			_wordValueEditor.Name = "mWordValueEditor";
			_wordValueEditor.TabIndex = 2;
			_wordValueEditor.ValueChanged += MWordValueEditor_ValueChanged;
			_wordValueEditor.NavigationKeyDown += This_KeyDown;

			_equalsLabel.Location = new Point(_wordValueEditor.Right, _wordValueEditor.Top + 2);
			_equalsLabel.Name = "mFirstEqualsLabel";
			_equalsLabel.Size = new Size(10, _wordValueEditor.Height - 2);
			_equalsLabel.TabIndex = 3;
			_equalsLabel.Text = "=";

			_wordCharTextBox.Location = new Point(_equalsLabel.Right, _wordValueEditor.Top);
			_wordCharTextBox.Name = "mWordCharTextBox";
			_wordCharTextBox.Size = new Size(45, _wordValueEditor.Height);
			_wordCharTextBox.TabIndex = 4;
			_wordCharTextBox.ValueChanged += MWordCharTextBox_ValueChanged;
			_wordCharTextBox.NavigationKeyDown += This_KeyDown;

			Controls.Add(_wordValueEditor);
			Controls.Add(_equalsLabel);
			Controls.Add(_wordCharTextBox);
			Name = "FullWordEditor";
			Size = new Size(_wordCharTextBox.Right, _wordCharTextBox.Height);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes editorField = FieldTypes.Chars;
			int? index = _wordCharTextBox.SelectionStart + _wordCharTextBox.SelectionLength;

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
					if (sender == _wordValueEditor)
						_wordCharTextBox.Focus();

					else if (sender == _wordCharTextBox && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;

				case Keys.Left:
					if (sender == _wordCharTextBox)
						_wordValueEditor.Focus(FieldTypes.Value, null);

					else if (sender == _wordValueEditor && NavigationKeyDown != null)
						NavigationKeyDown(this, e);

					break;
			}
		}

		private void MWordCharTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			_wordValueEditor.Update();
			OnValueChanged(new WordEditorValueChangedEventArgs((Word)args.OldValue, (Word)args.NewValue));
		}

		private void MWordValueEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			_wordCharTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			_wordValueEditor.Update();
			_wordCharTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			_wordValueEditor.UpdateLayout();
			_wordCharTextBox.UpdateLayout();
		}

		public IFullWord FullWord
		{
			get => _fullWord;
			set
			{
				_fullWord = value ?? throw new ArgumentNullException(nameof(value), "FullWord may not be set to null");
				_wordValueEditor.WordValue = _fullWord;
				_wordCharTextBox.MixByteCollectionValue = _fullWord;
			}
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly == value)
					return;

				_readOnly = value;
				_wordValueEditor.ReadOnly = _readOnly;
				_wordCharTextBox.ReadOnly = _readOnly;
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
				_wordCharTextBox.Select(start, length);

			else
				_wordValueEditor.Select(start, length);
		}
	}
}
