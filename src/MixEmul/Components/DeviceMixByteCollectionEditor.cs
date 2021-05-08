namespace MixGui.Components
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using MixGui.Events;
	using MixGui.Settings;
	using MixGui.Utils;
	using MixLib.Type;

	public class DeviceMixByteCollectionEditor : UserControl, IMixByteCollectionEditor, INavigableControl
	{
		private readonly Label mixByteCollectionIndexLabel;
		private readonly MixByteCollectionCharTextBox charTextBox;
		private int mixByteCollectionIndex;
		private IMixByteCollection deviceMixByteCollection;
		private int indexCharCount = 2;

		public event KeyEventHandler NavigationKeyDown;
		public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;

		public DeviceMixByteCollectionEditor(IMixByteCollection mixByteCollection = null)
		{
			this.deviceMixByteCollection = mixByteCollection;
			if (deviceMixByteCollection == null)
				this.deviceMixByteCollection = new MixByteCollection(FullWord.ByteCount);

			this.charTextBox = new MixByteCollectionCharTextBox(this.deviceMixByteCollection);
			this.mixByteCollectionIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> this.charTextBox.Focused ? FieldTypes.Chars : null;

		public int? CaretIndex
			=> this.charTextBox.Focused ? this.charTextBox.SelectionStart + this.charTextBox.SelectionLength : null;

		public void Select(int start, int length)
			=> this.charTextBox.Select(start, length);

		public bool Focus(FieldTypes? field, int? index)
			=> this.charTextBox.FocusWithIndex(index);

		protected virtual void OnValueChanged(MixByteCollectionEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void MixByteCollectionEditor_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
			=> OnValueChanged(args);

		private void InitializeComponent()
		{
			SuspendLayout();

			this.mixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.mixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.mixByteCollectionIndexLabel.Location = new Point(0, 0);
			this.mixByteCollectionIndexLabel.Name = "mMixByteCollectionIndexLabel";
			this.mixByteCollectionIndexLabel.Size = new Size(30, 13);
			this.mixByteCollectionIndexLabel.AutoSize = true;
			this.mixByteCollectionIndexLabel.TabIndex = 0;
			this.mixByteCollectionIndexLabel.Text = "00:";

			this.charTextBox.Location = new Point(this.mixByteCollectionIndexLabel.Right, 0);
			this.charTextBox.Name = "mCharTextBox";
			this.charTextBox.TabIndex = 1;
			this.charTextBox.ValueChanged += MixByteCollectionEditor_ValueChanged;
			this.charTextBox.NavigationKeyDown += This_KeyDown;
			this.charTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.charTextBox.BorderStyle = BorderStyle.None;
			this.charTextBox.Height = 13;

			Controls.Add(this.mixByteCollectionIndexLabel);
			Controls.Add(this.charTextBox);
			Name = "DeviceMixByteCollectionEditor";
			Size = new Size(this.charTextBox.Right, this.charTextBox.Height);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, e);
					break;
			}
		}

		public new void Update()
		{
			this.charTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			this.mixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.mixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.charTextBox.UpdateLayout();
		}

		public int MixByteCollectionIndex
		{
			get => this.mixByteCollectionIndex;
			set
			{
				if (value < 0)
					value = 0;

				this.mixByteCollectionIndex = value;
				SetIndexLabelText();
			}
		}

		private void SetIndexLabelText()
			=> this.mixByteCollectionIndexLabel.Text = this.mixByteCollectionIndex.ToString("D" + this.indexCharCount) + ":";

		public int IndexCharCount
		{
			get => this.indexCharCount;
			set
			{
				if (this.indexCharCount == value)
					return;

				this.indexCharCount = value;
				SetIndexLabelText();

				int oldCharBoxLeft = this.charTextBox.Left;
				this.charTextBox.Left = this.mixByteCollectionIndexLabel.Right + 3;
				this.charTextBox.Width += oldCharBoxLeft - this.charTextBox.Left;
			}
		}

		public IMixByteCollection DeviceMixByteCollection
		{
			get => this.deviceMixByteCollection;
			set
			{
				this.deviceMixByteCollection = value ?? throw new ArgumentNullException(nameof(value), "DeviceMixByteCollection may not be set to null");
				this.charTextBox.MixByteCollectionValue = this.deviceMixByteCollection;
				this.charTextBox.Select(0, 0);
			}
		}

		public bool ReadOnly
		{
			get => this.charTextBox.ReadOnly;
			set => this.charTextBox.ReadOnly = value;
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get => DeviceMixByteCollection;
			set => DeviceMixByteCollection = value;
		}
	}
}
