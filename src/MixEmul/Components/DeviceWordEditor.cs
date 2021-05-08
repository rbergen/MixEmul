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
		private readonly Label wordIndexLabel;
		private readonly FullWordEditor fullWordEditor;
		private int wordIndex;
		private IFullWord deviceWord;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public DeviceWordEditor() : this(null) { }

		public DeviceWordEditor(IFullWord deviceWord)
		{
			this.deviceWord = deviceWord ?? new FullWord();
			this.fullWordEditor = new FullWordEditor(this.deviceWord);
			this.wordIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> this.fullWordEditor.FocusedField;

		public int? CaretIndex
			=> this.fullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> this.fullWordEditor.Focus(field, index);

		public void Select(int start, int length)
			=> this.fullWordEditor.Select(start, length);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void MFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args) => OnValueChanged(args);

		private void InitializeComponent()
		{
			SuspendLayout();

			this.wordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.wordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.wordIndexLabel.Location = new Point(0, 4);
			this.wordIndexLabel.Name = "mWordIndexLabel";
			this.wordIndexLabel.Size = new Size(30, 13);
			this.wordIndexLabel.TabIndex = 0;
			this.wordIndexLabel.Text = "00:";

			this.fullWordEditor.Location = new Point(this.wordIndexLabel.Right, 2);
			this.fullWordEditor.Name = "mFullWordEditor";
			this.fullWordEditor.TabIndex = 1;
			this.fullWordEditor.ValueChanged += MFullWordEditor_ValueChanged;
			this.fullWordEditor.NavigationKeyDown += This_KeyDown;

			Controls.Add(this.wordIndexLabel);
			Controls.Add(this.fullWordEditor);
			Name = "DeviceWordEditor";
			Size = new Size(this.fullWordEditor.Right + 2, this.fullWordEditor.Height + 3);
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
			this.fullWordEditor.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			this.wordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.wordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.fullWordEditor.UpdateLayout();
		}

		public int WordIndex
		{
			get => this.wordIndex;
			set
			{
				this.wordIndex = value;
				this.wordIndexLabel.Text = value.ToString("D2") + ":";
			}
		}

		public IFullWord DeviceWord
		{
			get => this.deviceWord;
			set
			{
				this.deviceWord = value ?? throw new ArgumentNullException(nameof(value), "DeviceWord may not be set to null");
				this.fullWordEditor.WordValue = this.deviceWord;
			}
		}

		public bool ReadOnly
		{
			get => this.fullWordEditor.ReadOnly;
			set => this.fullWordEditor.ReadOnly = value;
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
