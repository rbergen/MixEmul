using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class DeviceWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private readonly Label _wordIndexLabel;
		private readonly FullWordEditor _fullWordEditor;
		private int _wordIndex;
		private IFullWord _deviceWord;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public DeviceWordEditor() : this(null) { }

		public DeviceWordEditor(IFullWord deviceWord)
		{
			_deviceWord = deviceWord ?? new FullWord();
			_fullWordEditor = new FullWordEditor(_deviceWord);
			_wordIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> _fullWordEditor.FocusedField;

		public int? CaretIndex
			=> _fullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> _fullWordEditor.Focus(field, index);

		public void Select(int start, int length)
			=> _fullWordEditor.Select(start, length);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void MFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args) => OnValueChanged(args);

		private void InitializeComponent()
		{
			SuspendLayout();

			_wordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_wordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_wordIndexLabel.Location = new Point(0, 4);
			_wordIndexLabel.Name = "mWordIndexLabel";
			_wordIndexLabel.Size = new Size(30, 13);
			_wordIndexLabel.TabIndex = 0;
			_wordIndexLabel.Text = "00:";

			_fullWordEditor.Location = new Point(_wordIndexLabel.Right, 2);
			_fullWordEditor.Name = "mFullWordEditor";
			_fullWordEditor.TabIndex = 1;
			_fullWordEditor.ValueChanged += MFullWordEditor_ValueChanged;
			_fullWordEditor.NavigationKeyDown += This_KeyDown;

			Controls.Add(_wordIndexLabel);
			Controls.Add(_fullWordEditor);
			Name = "DeviceWordEditor";
			Size = new Size(_fullWordEditor.Right + 2, _fullWordEditor.Height + 3);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, e);
					break;

				case Keys.Right:
					break;
			}
		}

		public new void Update()
		{
			_fullWordEditor.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			_wordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_wordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_fullWordEditor.UpdateLayout();
		}

		public int WordIndex
		{
			get => _wordIndex;
			set
			{
				_wordIndex = value;
				_wordIndexLabel.Text = value.ToString("D2") + ":";
			}
		}

		public IFullWord DeviceWord
		{
			get => _deviceWord;
			set
			{
				_deviceWord = value ?? throw new ArgumentNullException(nameof(value), "DeviceWord may not be set to null");
				_fullWordEditor.WordValue = _deviceWord;
			}
		}

		public bool ReadOnly
		{
			get => _fullWordEditor.ReadOnly;
			set => _fullWordEditor.ReadOnly = value;
		}

		public IWord WordValue
		{
			get => DeviceWord;
			set
			{
				if (value is not IFullWord)
					throw new ArgumentException("Value must be an IFullWord");

				DeviceWord = (IFullWord)value;
			}
		}
	}
}
