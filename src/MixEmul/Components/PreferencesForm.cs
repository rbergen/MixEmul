using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Modules.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class PreferencesForm : Form
	{
		private Button _cancelButton;
		private Button _colorDefaultButton;
		private ColorDialog _colorDialog;
		private ComboBox _colorSelectionBox;
		private Button _colorSetButton;
		private GroupBox _colorsGroup;
		private readonly Configuration _configuration;
		private readonly string _defaultDirectory;
		private Button _defaultsButton;
		private TextBox _deviceDirectoryBox;
		private Button _deviceDirectoryDefaultButton;
		private Label _deviceDirectoryLabel;
		private Button _deviceDirectorySetButton;
		private TextBox _deviceFileBox;
		private Button _deviceFileDefaultButton;
		private string _deviceFilesDirectory;
		private ComboBox _deviceFileSelectionBox;
		private Button _deviceFileSetButton;
		private FolderBrowserDialog _deviceFilesFolderBrowserDialog;
		private GroupBox _deviceFilesGroup;
		private readonly Devices _devices;
		private GroupBox _deviceTickCountsGroup;
		private Button _okButton;
		private SaveFileDialog _selectDeviceFileDialog;
		private Label _showColorLabel;
		private LongValueTextBox _tickCountBox;
		private Button _tickCountDefaultButton;
		private ComboBox _tickCountSelectionBox;
		private GroupBox _miscGroupBox;
		private Button _deviceReloadIntervalDefaultButton;
		private Label _deviceReloadIntervalLabel;
		private ComboBox _deviceReloadIntervalComboBox;
		private Button _tickCountSetButton;
		private GroupBox _loaderCardsGroupBox;
		private Panel _loaderCardsPanel;
		private MixByteCollectionCharTextBox _loaderCard2TextBox;
		private MixByteCollectionCharTextBox _loaderCard1TextBox;
		private MixByteCollectionCharTextBox _loaderCard3TextBox;
		private MixByteCollectionCharTextBox[] _loaderCardTextBoxes;
		private Button _loaderCardsDefaultButton;
		private GroupBox _floatingPointGroupBox;
		private LongValueTextBox _floatingPointMemoryWordCountBox;
		private Label _floatingPointMemoryWordCountLabel;
		private Button _floatingPointMemoryWordCountDefaultButton;
		private int _deviceReloadInterval;
		private CheckBox _colorProfilingCountsCheckBox;
		private int? _floatingPointMemoryWordCount;

		public PreferencesForm(Configuration configuration, Devices devices, string defaultDirectory)
		{
			InitializeComponent();
			_showColorLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);

			FillDeviceReloadIntervalSelectionBox();

			_configuration = configuration;
			_devices = devices;
			_defaultDirectory = defaultDirectory;
			_deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			InitializeLoaderCardTextBoxes();
		}

		private string GetCurrentDeviceFilesDirectory()
			=> _deviceFilesDirectory ?? DeviceSettings.DefaultDeviceFilesDirectory;

		public void UpdateLayout()
			=> _tickCountBox.UpdateLayout();

		private void ColorSelectionBox_SelectionChangeCommited(object sender, EventArgs e)
			=> UpdateColorControls((ColorComboBoxItem)_colorSelectionBox.SelectedItem);

		private void DeviceFileSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
			=> UpdateDeviceFileControls((DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem);

		private void TickCountSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
			=> UpdateTickCountControls((TickCountComboBoxItem)_tickCountSelectionBox.SelectedItem);

		private void TickCountSetButton_Click(object sender, EventArgs e)
			=> SetTickCountValue();

		private void LoaderCardsDefaultButton_Click(object sender, EventArgs e)
			=> LoadDefaultLoaderCards();

		private void InitializeLoaderCardTextBoxes()
		{
			_loaderCardTextBoxes = new MixByteCollectionCharTextBox[] { _loaderCard1TextBox, _loaderCard2TextBox, _loaderCard3TextBox };

			for (int index = 0; index < _loaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = _loaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.NavigationKeyDown += This_KeyDown;
				loaderCardTextBox.ValueChanged += LoaderCardTextBox_ValueChanged;
			}
		}

		private void LoaderCardTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
			=> _loaderCardsDefaultButton.Enabled = true;

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (sender is not MixByteCollectionCharTextBox || e.Modifiers != Keys.None)
				return;

			var senderTextBox = (MixByteCollectionCharTextBox)sender;
			MixByteCollectionCharTextBox targetTextBox;
			int index;

			int? caretPos = e is IndexKeyEventArgs args ? args.Index : null;

			switch (e.KeyCode)
			{
				case Keys.Up:
					index = Array.IndexOf(_loaderCardTextBoxes, senderTextBox);

					if (index <= 0)
						break;

					targetTextBox = _loaderCardTextBoxes[index - 1];
					targetTextBox.Focus();

					if (caretPos.HasValue)
						targetTextBox.Select(caretPos.Value, 0);

					break;

				case Keys.Down:
					index = Array.IndexOf(_loaderCardTextBoxes, senderTextBox);

					if (index >= _loaderCardTextBoxes.Length - 1)
						break;

					targetTextBox = _loaderCardTextBoxes[index + 1];
					targetTextBox.Focus();

					if (caretPos.HasValue)
						targetTextBox.Select(caretPos.Value, 0);

					break;
			}
		}

		private void FillDeviceReloadIntervalSelectionBox()
		{
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(250));
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(500));
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(750));
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1000));
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1500));
			_deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(2000));
		}

		private void FillColorSelectionBox()
		{
			_colorSelectionBox.Items.Clear();
			string[] knownColorNames = GuiSettings.KnownColorNames;

			var list = new List<ColorComboBoxItem>();
			foreach (string colorName in knownColorNames)
			{
				var comboBoxItem = new ColorComboBoxItem(colorName);

				if (_configuration.Colors.ContainsKey(colorName))
					comboBoxItem.Color = _configuration.Colors[colorName];

				list.Add(comboBoxItem);
			}

			_colorSelectionBox.Items.AddRange(list.ToArray());

			if (_colorSelectionBox.Items.Count > 0)
				_colorSelectionBox.SelectedIndex = 0;

			UpdateColorControls((ColorComboBoxItem)_colorSelectionBox.SelectedItem);
		}

		private void FillDeviceFileSelectionBox()
		{
			_deviceFileSelectionBox.Items.Clear();
			var list = new List<DeviceFileComboBoxItem>();
			foreach (MixDevice device in _devices)
			{
				if (!(device is FileBasedDevice))
					continue;

				var item = new DeviceFileComboBoxItem((FileBasedDevice)device);

				if (_configuration.DeviceFilePaths.ContainsKey(device.Id))
					item.FilePath = _configuration.DeviceFilePaths[device.Id];

				list.Add(item);
			}

			_deviceFileSelectionBox.Items.AddRange(list.ToArray());

			if (_deviceFileSelectionBox.Items.Count > 0)
				_deviceFileSelectionBox.SelectedIndex = 0;

			UpdateDeviceFileControls((DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem);
		}

		private void FillTickCountSelectionBox()
		{
			_tickCountSelectionBox.Items.Clear();

			string[] knownTickCountNames = DeviceSettings.KnownTickCountNames;

			var list = new List<TickCountComboBoxItem>();
			foreach (string tickCountName in knownTickCountNames)
			{
				var item = new TickCountComboBoxItem(tickCountName);

				if (_configuration.TickCounts.ContainsKey(tickCountName))
					item.TickCount = _configuration.TickCounts[tickCountName];

				list.Add(item);
			}

			_tickCountSelectionBox.Items.AddRange(list.ToArray());

			if (_tickCountSelectionBox.Items.Count > 0)
	  		_tickCountSelectionBox.SelectedIndex = 0;

			UpdateTickCountControls((TickCountComboBoxItem)_tickCountSelectionBox.SelectedItem);
		}

		public Control GetFocusedControl()
		{
			ContainerControl container = this;
			Control control = null;

			while (container != null)
			{
				control = container.ActiveControl;
				container = control as ContainerControl;
			}

			return control;
		}

		private Color GetMatchingColor(string name, bool wantBackground)
		{
			string matchingColorName;
			if (wantBackground)
			{
				if (name.EndsWith("Text", StringComparison.Ordinal))
					matchingColorName = name[..^4];

				else
					matchingColorName = name;

				if (!GuiSettings.IsKnownColor(matchingColorName))
					matchingColorName += "Background";
			}
			else if (name.EndsWith("Background", StringComparison.Ordinal))
				matchingColorName = name[..^10] + "Text";

			else
				matchingColorName = name + "Text";

			if (!GuiSettings.IsKnownColor(matchingColorName))
				matchingColorName = wantBackground ? "EditorBackground" : "RenderedText";

			foreach (ColorComboBoxItem item in _colorSelectionBox.Items)
			{
				if (item.Name == matchingColorName)
					return item.Color;
			}

			return GuiSettings.GetDefaultColor(name);
		}

		private void InitializeComponent()
		{
			var fullWord1 = new FullWord();
			var mixByte1 = new MixByte();
			var mixByte2 = new MixByte();
			var mixByte3 = new MixByte();
			var mixByte4 = new MixByte();
			var mixByte5 = new MixByte();
			var fullWord2 = new FullWord();
			var mixByte6 = new MixByte();
			var mixByte7 = new MixByte();
			var mixByte8 = new MixByte();
			var mixByte9 = new MixByte();
			var mixByte10 = new MixByte();
			var fullWord3 = new FullWord();
			var mixByte11 = new MixByte();
			var mixByte12 = new MixByte();
			var mixByte13 = new MixByte();
			var mixByte14 = new MixByte();
			var mixByte15 = new MixByte();
			var resources = new ComponentResourceManager(typeof(PreferencesForm));
			_colorsGroup = new GroupBox();
			_colorSetButton = new Button();
			_showColorLabel = new Label();
			_colorSelectionBox = new ComboBox();
			_colorDefaultButton = new Button();
			_deviceFilesGroup = new GroupBox();
			_deviceDirectoryBox = new TextBox();
			_deviceDirectoryLabel = new Label();
			_deviceDirectorySetButton = new Button();
			_deviceDirectoryDefaultButton = new Button();
			_deviceFileSelectionBox = new ComboBox();
			_deviceFileBox = new TextBox();
			_deviceFileSetButton = new Button();
			_deviceFileDefaultButton = new Button();
			_okButton = new Button();
			_cancelButton = new Button();
			_deviceTickCountsGroup = new GroupBox();
			_tickCountDefaultButton = new Button();
			_tickCountSetButton = new Button();
			_tickCountSelectionBox = new ComboBox();
			_defaultsButton = new Button();
			_miscGroupBox = new GroupBox();
			_deviceReloadIntervalComboBox = new ComboBox();
			_deviceReloadIntervalLabel = new Label();
			_deviceReloadIntervalDefaultButton = new Button();
			_loaderCardsGroupBox = new GroupBox();
			_loaderCardsDefaultButton = new Button();
			_loaderCardsPanel = new Panel();
			_floatingPointGroupBox = new GroupBox();
			_floatingPointMemoryWordCountLabel = new Label();
			_floatingPointMemoryWordCountDefaultButton = new Button();
			_floatingPointMemoryWordCountBox = new LongValueTextBox();
			_loaderCard3TextBox = new MixByteCollectionCharTextBox();
			_loaderCard2TextBox = new MixByteCollectionCharTextBox();
			_loaderCard1TextBox = new MixByteCollectionCharTextBox();
			_tickCountBox = new LongValueTextBox();
			_colorProfilingCountsCheckBox = new CheckBox();
			_colorsGroup.SuspendLayout();
			_deviceFilesGroup.SuspendLayout();
			_deviceTickCountsGroup.SuspendLayout();
			_miscGroupBox.SuspendLayout();
			_loaderCardsGroupBox.SuspendLayout();
			_loaderCardsPanel.SuspendLayout();
			_floatingPointGroupBox.SuspendLayout();
			SuspendLayout();
			// 
			// mColorsGroup
			// 
			_colorsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_colorsGroup.Controls.Add(_colorProfilingCountsCheckBox);
			_colorsGroup.Controls.Add(_colorSetButton);
			_colorsGroup.Controls.Add(_showColorLabel);
			_colorsGroup.Controls.Add(_colorSelectionBox);
			_colorsGroup.Controls.Add(_colorDefaultButton);
			_colorsGroup.Location = new Point(12, 12);
			_colorsGroup.Name = "mColorsGroup";
			_colorsGroup.Size = new Size(366, 70);
			_colorsGroup.TabIndex = 1;
			_colorsGroup.TabStop = false;
			_colorsGroup.Text = "Colors";
			// 
			// mColorSetButton
			// 
			_colorSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_colorSetButton.Location = new Point(254, 15);
			_colorSetButton.Name = "mColorSetButton";
			_colorSetButton.Size = new Size(40, 23);
			_colorSetButton.TabIndex = 2;
			_colorSetButton.Text = "&Set...";
			_colorSetButton.FlatStyle = FlatStyle.Flat;
			_colorSetButton.Click += ColorSetButton_Click;
			// 
			// mShowColorLabel
			// 
			_showColorLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_showColorLabel.BorderStyle = BorderStyle.FixedSingle;
			_showColorLabel.Location = new Point(198, 16);
			_showColorLabel.Name = "mShowColorLabel";
			_showColorLabel.Size = new Size(48, 21);
			_showColorLabel.TabIndex = 1;
			_showColorLabel.Text = "Color";
			_showColorLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// mColorSelectionBox
			// 
			_colorSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_colorSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			_colorSelectionBox.Location = new Point(8, 16);
			_colorSelectionBox.Name = "mColorSelectionBox";
			_colorSelectionBox.Size = new Size(182, 21);
			_colorSelectionBox.Sorted = true;
			_colorSelectionBox.TabIndex = 0;
			_colorSelectionBox.FlatStyle = FlatStyle.Flat;
			_colorSelectionBox.SelectionChangeCommitted += ColorSelectionBox_SelectionChangeCommited;
			// 
			// mColorDefaultButton
			// 
			_colorDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_colorDefaultButton.Location = new Point(302, 15);
			_colorDefaultButton.Name = "mColorDefaultButton";
			_colorDefaultButton.Size = new Size(56, 23);
			_colorDefaultButton.TabIndex = 3;
			_colorDefaultButton.Text = "Default";
			_colorDefaultButton.FlatStyle = FlatStyle.Flat;
			_colorDefaultButton.Click += ColorDefaultButton_Click;
			// 
			// mDeviceFilesGroup
			// 
			_deviceFilesGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_deviceFilesGroup.Controls.Add(_deviceDirectoryBox);
			_deviceFilesGroup.Controls.Add(_deviceDirectoryLabel);
			_deviceFilesGroup.Controls.Add(_deviceDirectorySetButton);
			_deviceFilesGroup.Controls.Add(_deviceDirectoryDefaultButton);
			_deviceFilesGroup.Controls.Add(_deviceFileSelectionBox);
			_deviceFilesGroup.Controls.Add(_deviceFileBox);
			_deviceFilesGroup.Controls.Add(_deviceFileSetButton);
			_deviceFilesGroup.Controls.Add(_deviceFileDefaultButton);
			_deviceFilesGroup.Location = new Point(12, 131);
			_deviceFilesGroup.Name = "mDeviceFilesGroup";
			_deviceFilesGroup.Size = new Size(366, 80);
			_deviceFilesGroup.TabIndex = 3;
			_deviceFilesGroup.TabStop = false;
			_deviceFilesGroup.Text = "Device files";
			// 
			// mDeviceDirectoryBox
			// 
			_deviceDirectoryBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_deviceDirectoryBox.Location = new Point(104, 16);
			_deviceDirectoryBox.Name = "mDeviceDirectoryBox";
			_deviceDirectoryBox.ReadOnly = true;
			_deviceDirectoryBox.Size = new Size(142, 20);
			_deviceDirectoryBox.TabIndex = 1;
			_deviceDirectoryBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mDeviceDirectoryLabel
			// 
			_deviceDirectoryLabel.Location = new Point(6, 16);
			_deviceDirectoryLabel.Name = "mDeviceDirectoryLabel";
			_deviceDirectoryLabel.Size = new Size(100, 21);
			_deviceDirectoryLabel.TabIndex = 0;
			_deviceDirectoryLabel.Text = "Default directory:";
			_deviceDirectoryLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mDeviceDirectorySetButton
			// 
			_deviceDirectorySetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceDirectorySetButton.Location = new Point(254, 15);
			_deviceDirectorySetButton.Name = "mDeviceDirectorySetButton";
			_deviceDirectorySetButton.Size = new Size(40, 23);
			_deviceDirectorySetButton.TabIndex = 2;
			_deviceDirectorySetButton.Text = "S&et...";
			_deviceDirectorySetButton.FlatStyle = FlatStyle.Flat;
			_deviceDirectorySetButton.Click += DeviceDirectorySetButton_Click;
			// 
			// mDeviceDirectoryDefaultButton
			// 
			_deviceDirectoryDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceDirectoryDefaultButton.Location = new Point(302, 15);
			_deviceDirectoryDefaultButton.Name = "mDeviceDirectoryDefaultButton";
			_deviceDirectoryDefaultButton.Size = new Size(56, 23);
			_deviceDirectoryDefaultButton.TabIndex = 3;
			_deviceDirectoryDefaultButton.Text = "Default";
			_deviceDirectoryDefaultButton.FlatStyle = FlatStyle.Flat;
			_deviceDirectoryDefaultButton.Click += DeviceDirectoryDefaultButton_Click;
			// 
			// mDeviceFileSelectionBox
			// 
			_deviceFileSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			_deviceFileSelectionBox.Location = new Point(6, 48);
			_deviceFileSelectionBox.Name = "mDeviceFileSelectionBox";
			_deviceFileSelectionBox.Size = new Size(92, 21);
			_deviceFileSelectionBox.Sorted = true;
			_deviceFileSelectionBox.TabIndex = 4;
			_deviceFileSelectionBox.FlatStyle = FlatStyle.Flat;
			_deviceFileSelectionBox.SelectionChangeCommitted += DeviceFileSelectionBox_SelectionChangeCommitted;
			// 
			// mDeviceFileBox
			// 
			_deviceFileBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_deviceFileBox.Location = new Point(104, 48);
			_deviceFileBox.Name = "mDeviceFileBox";
			_deviceFileBox.ReadOnly = true;
			_deviceFileBox.Size = new Size(142, 20);
			_deviceFileBox.BorderStyle = BorderStyle.FixedSingle;
			_deviceFileBox.TabIndex = 5;
			// 
			// mDeviceFileSetButton
			// 
			_deviceFileSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceFileSetButton.Location = new Point(254, 47);
			_deviceFileSetButton.Name = "mDeviceFileSetButton";
			_deviceFileSetButton.Size = new Size(40, 23);
			_deviceFileSetButton.TabIndex = 6;
			_deviceFileSetButton.Text = "Se&t...";
			_deviceFileSetButton.FlatStyle = FlatStyle.Flat;
			_deviceFileSetButton.Click += DeviceFileSetButton_Click;
			// 
			// mDeviceFileDefaultButton
			// 
			_deviceFileDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceFileDefaultButton.Location = new Point(302, 47);
			_deviceFileDefaultButton.Name = "mDeviceFileDefaultButton";
			_deviceFileDefaultButton.Size = new Size(56, 23);
			_deviceFileDefaultButton.TabIndex = 7;
			_deviceFileDefaultButton.Text = "Default";
			_deviceFileDefaultButton.FlatStyle = FlatStyle.Flat;
			_deviceFileDefaultButton.Click += DeviceFileDefaultButton_Click;
			// 
			// mOkButton
			// 
			_okButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			_okButton.DialogResult = DialogResult.OK;
			_okButton.Location = new Point(224, 382);
			_okButton.Name = "mOkButton";
			_okButton.Size = new Size(75, 23);
			_okButton.TabIndex = 8;
			_okButton.Text = "&OK";
			_okButton.FlatStyle = FlatStyle.Flat;
			_okButton.Click += OkButton_Click;
			// 
			// mCancelButton
			// 
			_cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			_cancelButton.DialogResult = DialogResult.Cancel;
			_cancelButton.Location = new Point(304, 382);
			_cancelButton.Name = "mCancelButton";
			_cancelButton.Size = new Size(75, 23);
			_cancelButton.TabIndex = 9;
			_cancelButton.FlatStyle = FlatStyle.Flat;
			_cancelButton.Text = "&Cancel";
			// 
			// mDeviceTickCountsGroup
			// 
			_deviceTickCountsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_deviceTickCountsGroup.Controls.Add(_tickCountBox);
			_deviceTickCountsGroup.Controls.Add(_tickCountDefaultButton);
			_deviceTickCountsGroup.Controls.Add(_tickCountSetButton);
			_deviceTickCountsGroup.Controls.Add(_tickCountSelectionBox);
			_deviceTickCountsGroup.Location = new Point(12, 211);
			_deviceTickCountsGroup.Name = "mDeviceTickCountsGroup";
			_deviceTickCountsGroup.Size = new Size(366, 48);
			_deviceTickCountsGroup.TabIndex = 4;
			_deviceTickCountsGroup.TabStop = false;
			_deviceTickCountsGroup.Text = "Device tick counts";
			// 
			// mTickCountDefaultButton
			// 
			_tickCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_tickCountDefaultButton.Location = new Point(302, 16);
			_tickCountDefaultButton.Name = "mTickCountDefaultButton";
			_tickCountDefaultButton.Size = new Size(56, 23);
			_tickCountDefaultButton.TabIndex = 3;
			_tickCountDefaultButton.Text = "Default";
			_tickCountDefaultButton.FlatStyle = FlatStyle.Flat;
			_tickCountDefaultButton.Click += TickCountDefaultButton_Click;
			// 
			// mTickCountSetButton
			// 
			_tickCountSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_tickCountSetButton.Location = new Point(246, 16);
			_tickCountSetButton.Name = "mTickCountSetButton";
			_tickCountSetButton.Size = new Size(48, 23);
			_tickCountSetButton.TabIndex = 2;
			_tickCountSetButton.Text = "&Apply";
			_tickCountSetButton.FlatStyle = FlatStyle.Flat;
			_tickCountSetButton.Click += TickCountSetButton_Click;
			// 
			// mTickCountSelectionBox
			// 
			_tickCountSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_tickCountSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			_tickCountSelectionBox.Location = new Point(6, 16);
			_tickCountSelectionBox.Name = "mTickCountSelectionBox";
			_tickCountSelectionBox.Size = new Size(182, 21);
			_tickCountSelectionBox.Sorted = true;
			_tickCountSelectionBox.TabIndex = 0;
			_tickCountSelectionBox.FlatStyle = FlatStyle.Flat;
			_tickCountSelectionBox.SelectionChangeCommitted += TickCountSelectionBox_SelectionChangeCommitted;
			// 
			// mDefaultsButton
			// 
			_defaultsButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			_defaultsButton.Location = new Point(144, 382);
			_defaultsButton.Name = "mDefaultsButton";
			_defaultsButton.Size = new Size(75, 23);
			_defaultsButton.TabIndex = 7;
			_defaultsButton.Text = "&Defaults";
			_defaultsButton.FlatStyle = FlatStyle.Flat;
			_defaultsButton.Click += DefaultsButton_Click;
			// 
			// mMiscGroupBox
			// 
			_miscGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_miscGroupBox.Controls.Add(_deviceReloadIntervalComboBox);
			_miscGroupBox.Controls.Add(_deviceReloadIntervalLabel);
			_miscGroupBox.Controls.Add(_deviceReloadIntervalDefaultButton);
			_miscGroupBox.Location = new Point(12, 327);
			_miscGroupBox.Name = "mMiscGroupBox";
			_miscGroupBox.Size = new Size(366, 48);
			_miscGroupBox.TabIndex = 6;
			_miscGroupBox.TabStop = false;
			_miscGroupBox.Text = "Miscellaneous";
			// 
			// mDeviceReloadIntervalComboBox
			// 
			_deviceReloadIntervalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			_deviceReloadIntervalComboBox.FormattingEnabled = true;
			_deviceReloadIntervalComboBox.Location = new Point(156, 15);
			_deviceReloadIntervalComboBox.Name = "mDeviceReloadIntervalComboBox";
			_deviceReloadIntervalComboBox.Size = new Size(73, 21);
			_deviceReloadIntervalComboBox.TabIndex = 5;
			_deviceReloadIntervalComboBox.FlatStyle = FlatStyle.Flat;
			_deviceReloadIntervalComboBox.SelectedIndexChanged += DeviceReloadIntervalComboBox_SelectedIndexChanged;
			// 
			// mDeviceReloadIntervalLabel
			// 
			_deviceReloadIntervalLabel.AutoSize = true;
			_deviceReloadIntervalLabel.Location = new Point(6, 18);
			_deviceReloadIntervalLabel.Name = "mDeviceReloadIntervalLabel";
			_deviceReloadIntervalLabel.Size = new Size(145, 13);
			_deviceReloadIntervalLabel.TabIndex = 4;
			_deviceReloadIntervalLabel.Text = "Device editor reload interval: ";
			// 
			// mDeviceReloadIntervalDefaultButton
			// 
			_deviceReloadIntervalDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceReloadIntervalDefaultButton.Location = new Point(302, 15);
			_deviceReloadIntervalDefaultButton.Name = "mDeviceReloadIntervalDefaultButton";
			_deviceReloadIntervalDefaultButton.Size = new Size(56, 23);
			_deviceReloadIntervalDefaultButton.TabIndex = 3;
			_deviceReloadIntervalDefaultButton.Text = "Default";
			_deviceReloadIntervalDefaultButton.FlatStyle = FlatStyle.Flat;
			_deviceReloadIntervalDefaultButton.Click += DeviceReloadIntervalDefaultButton_Click;
			// 
			// mLoaderCardsGroupBox
			// 
			_loaderCardsGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_loaderCardsGroupBox.Controls.Add(_loaderCardsDefaultButton);
			_loaderCardsGroupBox.Controls.Add(_loaderCardsPanel);
			_loaderCardsGroupBox.Location = new Point(12, 259);
			_loaderCardsGroupBox.Name = "mLoaderCardsGroupBox";
			_loaderCardsGroupBox.Size = new Size(366, 68);
			_loaderCardsGroupBox.TabIndex = 5;
			_loaderCardsGroupBox.TabStop = false;
			_loaderCardsGroupBox.Text = "Loader instruction cards";
			// 
			// mLoaderCardsDefaultButton
			// 
			_loaderCardsDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_loaderCardsDefaultButton.Location = new Point(302, 16);
			_loaderCardsDefaultButton.Name = "mLoaderCardsDefaultButton";
			_loaderCardsDefaultButton.Size = new Size(56, 23);
			_loaderCardsDefaultButton.TabIndex = 4;
			_loaderCardsDefaultButton.Text = "Default";
			_loaderCardsDefaultButton.FlatStyle = FlatStyle.Flat;
			_loaderCardsDefaultButton.Click += LoaderCardsDefaultButton_Click;
			// 
			// mLoaderCardsPanel
			// 
			_loaderCardsPanel.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_loaderCardsPanel.BorderStyle = BorderStyle.FixedSingle;
			_loaderCardsPanel.Controls.Add(_loaderCard3TextBox);
			_loaderCardsPanel.Controls.Add(_loaderCard2TextBox);
			_loaderCardsPanel.Controls.Add(_loaderCard1TextBox);
			_loaderCardsPanel.Location = new Point(8, 16);
			_loaderCardsPanel.Name = "mLoaderCardsPanel";
			_loaderCardsPanel.Size = new Size(286, 44);
			_loaderCardsPanel.TabIndex = 0;
			// 
			// mFloatingPointGroupBox
			// 
			_floatingPointGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_floatingPointGroupBox.Controls.Add(_floatingPointMemoryWordCountBox);
			_floatingPointGroupBox.Controls.Add(_floatingPointMemoryWordCountLabel);
			_floatingPointGroupBox.Controls.Add(_floatingPointMemoryWordCountDefaultButton);
			_floatingPointGroupBox.Location = new Point(12, 83);
			_floatingPointGroupBox.Name = "mFloatingPointGroupBox";
			_floatingPointGroupBox.Size = new Size(366, 48);
			_floatingPointGroupBox.TabIndex = 2;
			_floatingPointGroupBox.TabStop = false;
			_floatingPointGroupBox.Text = "Floating point module";
			// 
			// mFloatingPointMemoryWordCountLabel
			// 
			_floatingPointMemoryWordCountLabel.AutoSize = true;
			_floatingPointMemoryWordCountLabel.Location = new Point(6, 18);
			_floatingPointMemoryWordCountLabel.Name = "mFloatingPointMemoryWordCountLabel";
			_floatingPointMemoryWordCountLabel.Size = new Size(181, 13);
			_floatingPointMemoryWordCountLabel.TabIndex = 0;
			_floatingPointMemoryWordCountLabel.Text = "Memory word count (requires restart):";
			// 
			// mFloatingPointMemoryWordCountDefaultButton
			// 
			_floatingPointMemoryWordCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_floatingPointMemoryWordCountDefaultButton.Location = new Point(302, 15);
			_floatingPointMemoryWordCountDefaultButton.Name = "mFloatingPointMemoryWordCountDefaultButton";
			_floatingPointMemoryWordCountDefaultButton.Size = new Size(56, 23);
			_floatingPointMemoryWordCountDefaultButton.TabIndex = 2;
			_floatingPointMemoryWordCountDefaultButton.Text = "Default";
			_floatingPointMemoryWordCountDefaultButton.FlatStyle = FlatStyle.Flat;
			_floatingPointMemoryWordCountDefaultButton.Click += FloatingPointMemoryWordCountDefaultButton_Click;
			// 
			// mFloatingPointMemoryWordCountBox
			// 
			_floatingPointMemoryWordCountBox.BackColor = Color.White;
			_floatingPointMemoryWordCountBox.BorderStyle = BorderStyle.FixedSingle;
			_floatingPointMemoryWordCountBox.ClearZero = true;
			_floatingPointMemoryWordCountBox.ForeColor = Color.Black;
			_floatingPointMemoryWordCountBox.Location = new Point(199, 16);
			_floatingPointMemoryWordCountBox.LongValue = 200;
			_floatingPointMemoryWordCountBox.Magnitude = 200;
			_floatingPointMemoryWordCountBox.MaxLength = 5;
			_floatingPointMemoryWordCountBox.MaxValue = 99999;
			_floatingPointMemoryWordCountBox.MinValue = 1;
			_floatingPointMemoryWordCountBox.Name = "mFloatingPointMemoryWordCountBox";
			_floatingPointMemoryWordCountBox.Sign = Word.Signs.Positive;
			_floatingPointMemoryWordCountBox.Size = new Size(40, 20);
			_floatingPointMemoryWordCountBox.SupportNegativeZero = false;
			_floatingPointMemoryWordCountBox.TabIndex = 1;
			_floatingPointMemoryWordCountBox.Text = "200";
			_floatingPointMemoryWordCountBox.ValueChanged += FloatingPointMemoryWordCountBox_ValueChanged;
			// 
			// mLoaderCard3TextBox
			// 
			_loaderCard3TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_loaderCard3TextBox.BackColor = Color.White;
			_loaderCard3TextBox.BorderStyle = BorderStyle.None;
			_loaderCard3TextBox.CharacterCasing = CharacterCasing.Upper;
			_loaderCard3TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			_loaderCard3TextBox.ForeColor = Color.Blue;
			_loaderCard3TextBox.Location = new Point(0, 28);
			_loaderCard3TextBox.Margin = new Padding(0);
			fullWord1.LongValue = 0;
			fullWord1.Magnitude = new MixByte[] {
				mixByte1,
				mixByte2,
				mixByte3,
				mixByte4,
				mixByte5};
			fullWord1.MagnitudeLongValue = 0;
			fullWord1.Sign = Word.Signs.Positive;
			_loaderCard3TextBox.MixByteCollectionValue = fullWord1;
			_loaderCard3TextBox.Name = "mLoaderCard3TextBox";
			_loaderCard3TextBox.Size = new Size(284, 14);
			_loaderCard3TextBox.TabIndex = 2;
			_loaderCard3TextBox.Text = "     ";
			_loaderCard3TextBox.UseEditMode = true;
			// 
			// mLoaderCard2TextBox
			// 
			_loaderCard2TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_loaderCard2TextBox.BackColor = Color.White;
			_loaderCard2TextBox.BorderStyle = BorderStyle.None;
			_loaderCard2TextBox.CharacterCasing = CharacterCasing.Upper;
			_loaderCard2TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			_loaderCard2TextBox.ForeColor = Color.Blue;
			_loaderCard2TextBox.Location = new Point(0, 14);
			_loaderCard2TextBox.Margin = new Padding(0);
			fullWord2.LongValue = 0;
			fullWord2.Magnitude = new MixByte[] {
				mixByte6,
				mixByte7,
				mixByte8,
				mixByte9,
				mixByte10};
			fullWord2.MagnitudeLongValue = 0;
			fullWord2.Sign = Word.Signs.Positive;
			_loaderCard2TextBox.MixByteCollectionValue = fullWord2;
			_loaderCard2TextBox.Name = "mLoaderCard2TextBox";
			_loaderCard2TextBox.Size = new Size(284, 14);
			_loaderCard2TextBox.TabIndex = 1;
			_loaderCard2TextBox.Text = "     ";
			_loaderCard2TextBox.UseEditMode = true;
			// 
			// mLoaderCard1TextBox
			// 
			_loaderCard1TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_loaderCard1TextBox.BackColor = Color.White;
			_loaderCard1TextBox.BorderStyle = BorderStyle.None;
			_loaderCard1TextBox.CharacterCasing = CharacterCasing.Upper;
			_loaderCard1TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			_loaderCard1TextBox.ForeColor = Color.Blue;
			_loaderCard1TextBox.Location = new Point(0, 0);
			_loaderCard1TextBox.Margin = new Padding(0);
			fullWord3.LongValue = 0;
			fullWord3.Magnitude = new MixByte[] {
				mixByte11,
				mixByte12,
				mixByte13,
				mixByte14,
				mixByte15};
			fullWord3.MagnitudeLongValue = 0;
			fullWord3.Sign = Word.Signs.Positive;
			_loaderCard1TextBox.MixByteCollectionValue = fullWord3;
			_loaderCard1TextBox.Name = "mLoaderCard1TextBox";
			_loaderCard1TextBox.Size = new Size(284, 14);
			_loaderCard1TextBox.TabIndex = 0;
			_loaderCard1TextBox.Text = "     ";
			_loaderCard1TextBox.UseEditMode = true;
			// 
			// mTickCountBox
			// 
			_tickCountBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_tickCountBox.BackColor = Color.White;
			_tickCountBox.BorderStyle = BorderStyle.FixedSingle;
			_tickCountBox.ClearZero = true;
			_tickCountBox.ForeColor = Color.Black;
			_tickCountBox.Location = new Point(198, 17);
			_tickCountBox.LongValue = 1;
			_tickCountBox.Magnitude = 1;
			_tickCountBox.MaxLength = 5;
			_tickCountBox.MaxValue = 99999;
			_tickCountBox.MinValue = 1;
			_tickCountBox.Name = "mTickCountBox";
			_tickCountBox.Sign = Word.Signs.Positive;
			_tickCountBox.Size = new Size(40, 20);
			_tickCountBox.SupportNegativeZero = false;
			_tickCountBox.TabIndex = 1;
			_tickCountBox.Text = "1";
			_tickCountBox.ValueChanged += TickCountBox_ValueChanged;
			// 
			// mColorProfilingCountsCheckBox
			// 
			_colorProfilingCountsCheckBox.AutoSize = true;
			_colorProfilingCountsCheckBox.Checked = true;
			_colorProfilingCountsCheckBox.CheckState = CheckState.Checked;
			_colorProfilingCountsCheckBox.Location = new Point(8, 43);
			_colorProfilingCountsCheckBox.Name = "mColorProfilingCountsCheckBox";
			_colorProfilingCountsCheckBox.Size = new Size(119, 17);
			_colorProfilingCountsCheckBox.TabIndex = 4;
			_colorProfilingCountsCheckBox.Text = "Color profiler counts";
			_colorProfilingCountsCheckBox.UseVisualStyleBackColor = true;
			_colorProfilingCountsCheckBox.FlatStyle = FlatStyle.Flat;
			// 
			// PreferencesForm
			// 
			ClientSize = new Size(384, 412);
			Controls.Add(_floatingPointGroupBox);
			Controls.Add(_loaderCardsGroupBox);
			Controls.Add(_miscGroupBox);
			Controls.Add(_defaultsButton);
			Controls.Add(_deviceTickCountsGroup);
			Controls.Add(_okButton);
			Controls.Add(_deviceFilesGroup);
			Controls.Add(_colorsGroup);
			Controls.Add(_cancelButton);
			Icon = ((Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			MaximizeBox = false;
			MaximumSize = new Size(800, 450);
			MinimizeBox = false;
			MinimumSize = new Size(400, 450);
			Name = "PreferencesForm";
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Preferences";
			VisibleChanged += This_VisibleChanged;
			KeyPress += This_KeyPress;
			_colorsGroup.ResumeLayout(false);
			_colorsGroup.PerformLayout();
			_deviceFilesGroup.ResumeLayout(false);
			_deviceFilesGroup.PerformLayout();
			_deviceTickCountsGroup.ResumeLayout(false);
			_deviceTickCountsGroup.PerformLayout();
			_miscGroupBox.ResumeLayout(false);
			_miscGroupBox.PerformLayout();
			_loaderCardsGroupBox.ResumeLayout(false);
			_loaderCardsPanel.ResumeLayout(false);
			_loaderCardsPanel.PerformLayout();
			_floatingPointGroupBox.ResumeLayout(false);
			_floatingPointGroupBox.PerformLayout();
			ResumeLayout(false);

		}

		private void ColorDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (ColorComboBoxItem)_colorSelectionBox.SelectedItem;

			selectedItem.ResetDefault();
			UpdateColorControls(selectedItem);
			_colorSelectionBox.Focus();
		}

		private void ColorSetButton_Click(object sender, EventArgs e)
		{
			if (_colorDialog == null)
			{
				_colorDialog = new ColorDialog
				{
					AnyColor = true,
					SolidColorOnly = true,
					AllowFullOpen = true
				};
			}

			var selectedItem = (ColorComboBoxItem)_colorSelectionBox.SelectedItem;
			_colorDialog.CustomColors = new int[] { selectedItem.Color.B << 16 | selectedItem.Color.G << 8 | selectedItem.Color.R };
			_colorDialog.Color = selectedItem.Color;

			if (_colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.Color = _colorDialog.Color;
				UpdateColorControls(selectedItem);
			}
		}

		private void DefaultsButton_Click(object sender, EventArgs e)
		{
			foreach (ColorComboBoxItem colorItem in _colorSelectionBox.Items)
				colorItem.ResetDefault();

			_colorProfilingCountsCheckBox.Checked = true;

			foreach (DeviceFileComboBoxItem deviceFileItem in _deviceFileSelectionBox.Items)
				deviceFileItem.ResetDefault();

			foreach (TickCountComboBoxItem tickCountItem in _tickCountSelectionBox.Items)
				tickCountItem.ResetDefault();

			_deviceFilesDirectory = null;
			_deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			UpdateColorControls((ColorComboBoxItem)_colorSelectionBox.SelectedItem);
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem);
			UpdateTickCountControls((TickCountComboBoxItem)_tickCountSelectionBox.SelectedItem);
			UpdateDeviceReloadIntervalControls();
			_floatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();

			LoadDefaultLoaderCards();
		}

		private void UpdateFloatingPointControls()
		{
			_floatingPointMemoryWordCountBox.LongValue = _floatingPointMemoryWordCount ?? ModuleSettings.FloatingPointMemoryWordCountDefault;
			_floatingPointMemoryWordCountDefaultButton.Enabled = _floatingPointMemoryWordCount != null;
		}

		private void DeviceDirectoryDefaultButton_Click(object sender, EventArgs e)
		{
			_deviceFilesDirectory = null;
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem);
			_deviceDirectoryBox.Focus();
		}

		private void DeviceDirectorySetButton_Click(object sender, EventArgs e)
		{
			if (_deviceFilesFolderBrowserDialog == null)
			{
				_deviceFilesFolderBrowserDialog = new FolderBrowserDialog
				{
					Description = "Please select the default directory for device files."
				};
			}

			_deviceFilesFolderBrowserDialog.SelectedPath = GetCurrentDeviceFilesDirectory();

			if (_deviceFilesFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				_deviceFilesDirectory = _deviceFilesFolderBrowserDialog.SelectedPath;
				UpdateDeviceDirectoryControls();
				UpdateDeviceFileControls((DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem);
			}
		}

		private void DeviceFileDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateDeviceFileControls(selectedItem);
			_deviceFileSelectionBox.Focus();
		}

		private void DeviceFileSetButton_Click(object sender, EventArgs e)
		{
			if (_selectDeviceFileDialog == null)
			{
				_selectDeviceFileDialog = new SaveFileDialog
				{
					CreatePrompt = true,
					DefaultExt = "mixdev",
					Filter = "MIX device files|*.mixdev|All files|*.*",
					OverwritePrompt = false
				};
			}

			var selectedItem = (DeviceFileComboBoxItem)_deviceFileSelectionBox.SelectedItem;
			_selectDeviceFileDialog.FileName = Path.Combine(GetCurrentDeviceFilesDirectory(), selectedItem.FilePath);

			if (_selectDeviceFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.FilePath = _selectDeviceFileDialog.FileName;
				UpdateDeviceFileControls(selectedItem);
			}
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			_configuration.Colors.Clear();
			foreach (ColorComboBoxItem colorItem in _colorSelectionBox.Items)
			{
				if (!colorItem.IsDefault)
					_configuration.Colors.Add(colorItem.Name, colorItem.Color);
			}

			_configuration.ColorProfilingCounts = _colorProfilingCountsCheckBox.Checked;

			_configuration.DeviceFilePaths.Clear();
			foreach (DeviceFileComboBoxItem deviceFileItem in _deviceFileSelectionBox.Items)
			{
				if (!deviceFileItem.IsDefault)
					_configuration.DeviceFilePaths.Add(deviceFileItem.Id, deviceFileItem.FilePath);
			}

			_configuration.TickCounts.Clear();
			foreach (TickCountComboBoxItem tickCountItem in _tickCountSelectionBox.Items)
			{
				if (!tickCountItem.IsDefault)
					_configuration.TickCounts.Add(tickCountItem.Name, tickCountItem.TickCount);
			}

			_configuration.DeviceFilesDirectory = _deviceFilesDirectory;
			_configuration.DeviceReloadInterval = _deviceReloadInterval;

			_configuration.FloatingPointMemoryWordCount = _floatingPointMemoryWordCount;

			_configuration.LoaderCards = _loaderCardsDefaultButton.Enabled ? _loaderCardTextBoxes.Select(box => box.MixByteCollectionValue.ToString(true)).ToArray() : null;

			try
			{
				_configuration.Save(_defaultDirectory);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, "Unable to save preferences: " + exception.Message, "Error saving preferences", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}

			DialogResult = DialogResult.OK;
		}

		private void TickCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args) 
      => SetTickCountValue();

		private void TickCountDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (TickCountComboBoxItem)_tickCountSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateTickCountControls(selectedItem);
			_tickCountSelectionBox.Focus();
		}

		private void SetTickCountValue()
		{
			var selectedItem = (TickCountComboBoxItem)_tickCountSelectionBox.SelectedItem;
			selectedItem.TickCount = (int)_tickCountBox.LongValue;
			UpdateTickCountControls(selectedItem);
		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape && !(GetFocusedControl() is IEscapeConsumer))
			{
				DialogResult = DialogResult.Cancel;
				Hide();
			}
		}

		private void This_VisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				return;

			FillColorSelectionBox();
			_colorProfilingCountsCheckBox.Checked = _configuration.ColorProfilingCounts;
			_deviceFilesDirectory = _configuration.DeviceFilesDirectory;
			UpdateDeviceDirectoryControls();
			FillDeviceFileSelectionBox();
			FillTickCountSelectionBox();
			_deviceReloadInterval = _configuration.DeviceReloadInterval;
			UpdateDeviceReloadIntervalControls();
			_floatingPointMemoryWordCount = _configuration.FloatingPointMemoryWordCount;
			UpdateFloatingPointControls();
			LoadLoaderCards();
		}

		private void LoadLoaderCards()
		{
			string[] loaderCards = _configuration.LoaderCards ?? CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < _loaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = _loaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.MixByteCollectionValue.Load(index < loaderCards.Length ? loaderCards[index] : string.Empty);
				loaderCardTextBox.Update();
			}

			_loaderCardsDefaultButton.Enabled = _configuration.LoaderCards != null;
		}

		private void UpdateColorControls(ColorComboBoxItem item)
		{
			if (item == null)
			{
				_showColorLabel.ForeColor = Color.Black;
				_showColorLabel.BackColor = Color.White;

				_colorSetButton.Enabled = false;
				_colorDefaultButton.Enabled = false;
			}
			else
			{
				if (item.IsBackground)
				{
					_showColorLabel.ForeColor = GetMatchingColor(item.Name, false);
					_showColorLabel.BackColor = item.Color;
				}
				else
				{
					_showColorLabel.ForeColor = item.Color;
					_showColorLabel.BackColor = GetMatchingColor(item.Name, true);
				}

				_colorSetButton.Enabled = true;
				_colorDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void UpdateDeviceReloadIntervalControls()
		{
			if (_deviceReloadInterval == DeviceSettings.UnsetDeviceReloadInterval)
			{
				SelectDeviceReloadInterval(DeviceSettings.DefaultDeviceReloadInterval);
				_deviceReloadIntervalDefaultButton.Enabled = false;
			}
			else
			{
				SelectDeviceReloadInterval(_deviceReloadInterval);
				_deviceReloadIntervalDefaultButton.Enabled = true;
			}
		}

		private void SelectDeviceReloadInterval(int interval)
		{
			foreach (DeviceReloadIntervalComboBoxItem item in _deviceReloadIntervalComboBox.Items)
			{
				if (item.MilliSeconds >= interval)
				{
					_deviceReloadIntervalComboBox.SelectedItem = item;
					return;
				}
			}
		}

		private void UpdateDeviceDirectoryControls()
		{
			if (_deviceFilesDirectory == null)
			{
				_deviceDirectoryBox.Text = DeviceSettings.DefaultDeviceFilesDirectory;
				_deviceDirectoryDefaultButton.Enabled = false;
			}
			else
			{
				_deviceDirectoryBox.Text = _deviceFilesDirectory;
				_deviceDirectoryDefaultButton.Enabled = true;
			}
		}

		private void UpdateDeviceFileControls(DeviceFileComboBoxItem item)
		{
			if (item == null)
			{
				_deviceFileBox.Text = "";
				_deviceFileSetButton.Enabled = false;
				_deviceFileDefaultButton.Enabled = false;
			}
			else
			{
				_deviceFileBox.Text = Path.Combine(GetCurrentDeviceFilesDirectory(), item.FilePath);
				_deviceFileSetButton.Enabled = true;
				_deviceFileDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void UpdateTickCountControls(TickCountComboBoxItem item)
		{
			if (item == null)
			{
				_tickCountBox.LongValue = 1L;
				_tickCountSetButton.Enabled = false;
				_tickCountDefaultButton.Enabled = false;
			}
			else
			{
				_tickCountBox.LongValue = item.TickCount;
				_tickCountSetButton.Enabled = true;
				_tickCountDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void DeviceReloadIntervalDefaultButton_Click(object sender, EventArgs e)
		{
			_deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			UpdateDeviceReloadIntervalControls();
		}

		private void DeviceReloadIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			_deviceReloadInterval = ((DeviceReloadIntervalComboBoxItem)_deviceReloadIntervalComboBox.SelectedItem).MilliSeconds;
			_deviceReloadIntervalDefaultButton.Enabled = true;
		}

		private void LoadDefaultLoaderCards()
		{
			string[] defaultLoaderCards = CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < _loaderCardTextBoxes.Length; index++)
			{
				_loaderCardTextBoxes[index].MixByteCollectionValue.Load(index < defaultLoaderCards.Length ? defaultLoaderCards[index] : string.Empty);
				_loaderCardTextBoxes[index].Update();
			}

			_loaderCardsDefaultButton.Enabled = false;
		}

		private void FloatingPointMemoryWordCountDefaultButton_Click(object sender, EventArgs e)
		{
			_floatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();
		}

		private void FloatingPointMemoryWordCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			_floatingPointMemoryWordCount = (int)_floatingPointMemoryWordCountBox.LongValue;
			UpdateFloatingPointControls();
		}

		/// <summary>
		/// Instances of this class are used to store the color settings 
		/// </summary>
		private class ColorComboBoxItem
		{
			private readonly string mDisplayName;
			public bool IsDefault { get; private set; }
			public string Name { get; private set; }

			private Color mSetColor;

			public ColorComboBoxItem(string name)
			{
				Name = name;
				mDisplayName = GetDisplayName(name);
				IsDefault = true;
			}

			public bool IsBackground => !Name.EndsWith("Text", StringComparison.Ordinal);

			public override string ToString() => mDisplayName;

			private static string GetDisplayName(string name)
			{
				var builder = new StringBuilder(name[0].ToString());

				for (int i = 1; i < name.Length; i++)
				{
					if (char.IsUpper(name, i))
					{
						builder.Append(' ');
						builder.Append(char.ToLower(name[i]));
					}
					else
						builder.Append(name[i]);
				}

				return builder.ToString();
			}

			public void ResetDefault() => IsDefault = true;

			public Color Color
			{
				get => !IsDefault ? mSetColor : GuiSettings.GetDefaultColor(Name);
				set
				{
					mSetColor = value;
					IsDefault = false;
				}
			}
		}

		/// <summary>
		/// Instances of this class are used to store the device file settings 
		/// </summary>
		private class DeviceFileComboBoxItem
		{
			private readonly FileBasedDevice mDevice;
			private string mFilePath;

			public DeviceFileComboBoxItem(FileBasedDevice device) => mDevice = device;

			public int Id => mDevice.Id;

			public bool IsDefault => mFilePath == null;

			public override string ToString() => mDevice.Id.ToString("D2") + ": " + mDevice.ShortName;

			public void ResetDefault() => mFilePath = null;

			public string FilePath
			{
				get => mFilePath ?? mDevice.DefaultFileName;
				set => mFilePath = value;
			}
		}

		/// <summary>
		/// Instances of this class are used to store the tick count settings 
		/// </summary>
		private class TickCountComboBoxItem
		{
			private readonly string mDisplayName;
			private bool mIsDefault;
			private readonly string mName;
			private int mSetTickCount;

			public TickCountComboBoxItem(string name)
			{
				mName = name;
				mDisplayName = GetDisplayName(name);
				mIsDefault = true;
			}

			public bool IsDefault => mIsDefault;

			public string Name => mName;

			public override string ToString() => mDisplayName;

			private static string GetDisplayName(string name)
			{
				var builder = new StringBuilder(name[0].ToString());

				for (int i = 1; i < name.Length; i++)
				{
					if (char.IsUpper(name, i))
					{
						builder.Append(' ');
						builder.Append(char.ToLower(name[i]));
					}
					else
						builder.Append(name[i]);
				}

				return builder.ToString();
			}

			public void ResetDefault() => mIsDefault = true;

			public int TickCount
			{
				get => !mIsDefault ? mSetTickCount : DeviceSettings.GetDefaultTickCount(mName);
				set
				{
					mSetTickCount = value;
					mIsDefault = false;
				}
			}
		}

		private class DeviceReloadIntervalComboBoxItem
		{
			public int MilliSeconds { get; set; }

			public DeviceReloadIntervalComboBoxItem(int milliSeconds) => MilliSeconds = milliSeconds;

			public override string ToString()
			{
				if (MilliSeconds < 1000)
					return MilliSeconds.ToString("##0 ms");

				float seconds = MilliSeconds / 1000f;
				return seconds.ToString("0.## s");
			}
		}
	}
}
