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
		private readonly Label _mixByteCollectionIndexLabel;
		private readonly MixByteCollectionCharTextBox _charTextBox;
		private int _mixByteCollectionIndex;
		private IMixByteCollection _deviceMixByteCollection;
		private int _indexCharCount = 2;

		public event KeyEventHandler NavigationKeyDown;
		public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;

		public DeviceMixByteCollectionEditor(IMixByteCollection mixByteCollection = null)
		{
			_deviceMixByteCollection = mixByteCollection;
			if (_deviceMixByteCollection == null)
				_deviceMixByteCollection = new MixByteCollection(FullWord.ByteCount);

			_charTextBox = new MixByteCollectionCharTextBox(_deviceMixByteCollection);
			_mixByteCollectionIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> _charTextBox.Focused ? FieldTypes.Chars : null;

		public int? CaretIndex
			=> _charTextBox.Focused ? _charTextBox.SelectionStart + _charTextBox.SelectionLength : null;

		public void Select(int start, int length)
			=> _charTextBox.Select(start, length);

		public bool Focus(FieldTypes? field, int? index)
			=> _charTextBox.FocusWithIndex(index);

		protected virtual void OnValueChanged(MixByteCollectionEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void MixByteCollectionEditor_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
			=> OnValueChanged(args);

		private void InitializeComponent()
		{
			SuspendLayout();

			_mixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_mixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_mixByteCollectionIndexLabel.Location = new Point(0, 0);
			_mixByteCollectionIndexLabel.Name = "mMixByteCollectionIndexLabel";
			_mixByteCollectionIndexLabel.Size = new Size(30, 13);
			_mixByteCollectionIndexLabel.AutoSize = true;
			_mixByteCollectionIndexLabel.TabIndex = 0;
			_mixByteCollectionIndexLabel.Text = "00:";

			_charTextBox.Location = new Point(_mixByteCollectionIndexLabel.Right, 0);
			_charTextBox.Name = "mCharTextBox";
			_charTextBox.TabIndex = 1;
			_charTextBox.ValueChanged += MixByteCollectionEditor_ValueChanged;
			_charTextBox.NavigationKeyDown += This_KeyDown;
			_charTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			_charTextBox.BorderStyle = BorderStyle.None;
			_charTextBox.Height = 13;

			Controls.Add(_mixByteCollectionIndexLabel);
			Controls.Add(_charTextBox);
			Name = "DeviceMixByteCollectionEditor";
			Size = new Size(_charTextBox.Right, _charTextBox.Height);
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
			_charTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			_mixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_mixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_charTextBox.UpdateLayout();
		}

		public int MixByteCollectionIndex
		{
			get => _mixByteCollectionIndex;
			set
			{
				if (value < 0)
					value = 0;

				_mixByteCollectionIndex = value;
				SetIndexLabelText();
			}
		}

		private void SetIndexLabelText()
			=> _mixByteCollectionIndexLabel.Text = _mixByteCollectionIndex.ToString("D" + _indexCharCount) + ":";

		public int IndexCharCount
		{
			get => _indexCharCount;
			set
			{
				if (_indexCharCount == value)
					return;

				_indexCharCount = value;
				SetIndexLabelText();

				int oldCharBoxLeft = _charTextBox.Left;
				_charTextBox.Left = _mixByteCollectionIndexLabel.Right + 3;
				_charTextBox.Width += oldCharBoxLeft - _charTextBox.Left;
			}
		}

		public IMixByteCollection DeviceMixByteCollection
		{
			get => _deviceMixByteCollection;
			set
			{
				_deviceMixByteCollection = value ?? throw new ArgumentNullException(nameof(value), "DeviceMixByteCollection may not be set to null");
				_charTextBox.MixByteCollectionValue = _deviceMixByteCollection;
				_charTextBox.Select(0, 0);
			}
		}

		public bool ReadOnly
		{
			get => _charTextBox.ReadOnly;
			set => _charTextBox.ReadOnly = value;
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get => DeviceMixByteCollection;
			set => DeviceMixByteCollection = value;
		}
	}
}
