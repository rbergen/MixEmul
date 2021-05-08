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
		private Button cancelButton;
		private Button colorDefaultButton;
		private ColorDialog colorDialog;
		private ComboBox colorSelectionBox;
		private Button colorSetButton;
		private GroupBox colorsGroup;
		private readonly Configuration configuration;
		private readonly string defaultDirectory;
		private Button defaultsButton;
		private TextBox deviceDirectoryBox;
		private Button deviceDirectoryDefaultButton;
		private Label deviceDirectoryLabel;
		private Button deviceDirectorySetButton;
		private TextBox deviceFileBox;
		private Button deviceFileDefaultButton;
		private string deviceFilesDirectory;
		private ComboBox deviceFileSelectionBox;
		private Button deviceFileSetButton;
		private FolderBrowserDialog deviceFilesFolderBrowserDialog;
		private GroupBox deviceFilesGroup;
		private readonly Devices devices;
		private GroupBox deviceTickCountsGroup;
		private Button okButton;
		private SaveFileDialog selectDeviceFileDialog;
		private Label showColorLabel;
		private LongValueTextBox tickCountBox;
		private Button tickCountDefaultButton;
		private ComboBox tickCountSelectionBox;
		private GroupBox miscGroupBox;
		private Button deviceReloadIntervalDefaultButton;
		private Label deviceReloadIntervalLabel;
		private ComboBox deviceReloadIntervalComboBox;
		private Button tickCountSetButton;
		private GroupBox loaderCardsGroupBox;
		private Panel loaderCardsPanel;
		private MixByteCollectionCharTextBox loaderCard2TextBox;
		private MixByteCollectionCharTextBox loaderCard1TextBox;
		private MixByteCollectionCharTextBox loaderCard3TextBox;
		private MixByteCollectionCharTextBox[] loaderCardTextBoxes;
		private Button loaderCardsDefaultButton;
		private GroupBox floatingPointGroupBox;
		private LongValueTextBox floatingPointMemoryWordCountBox;
		private Label floatingPointMemoryWordCountLabel;
		private Button floatingPointMemoryWordCountDefaultButton;
		private int deviceReloadInterval;
		private CheckBox colorProfilingCountsCheckBox;
		private int? floatingPointMemoryWordCount;

		public PreferencesForm(Configuration configuration, Devices devices, string defaultDirectory)
		{
			InitializeComponent();
			this.showColorLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);

			FillDeviceReloadIntervalSelectionBox();

			this.configuration = configuration;
			this.devices = devices;
			this.defaultDirectory = defaultDirectory;
			this.deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			InitializeLoaderCardTextBoxes();
		}

		private string GetCurrentDeviceFilesDirectory()
			=> this.deviceFilesDirectory ?? DeviceSettings.DefaultDeviceFilesDirectory;

		public void UpdateLayout()
			=> this.tickCountBox.UpdateLayout();

		private void ColorSelectionBox_SelectionChangeCommited(object sender, EventArgs e)
			=> UpdateColorControls((ColorComboBoxItem)this.colorSelectionBox.SelectedItem);

		private void DeviceFileSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
			=> UpdateDeviceFileControls((DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem);

		private void TickCountSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
			=> UpdateTickCountControls((TickCountComboBoxItem)this.tickCountSelectionBox.SelectedItem);

		private void TickCountSetButton_Click(object sender, EventArgs e)
			=> SetTickCountValue();

		private void LoaderCardsDefaultButton_Click(object sender, EventArgs e)
			=> LoadDefaultLoaderCards();

		private void InitializeLoaderCardTextBoxes()
		{
			this.loaderCardTextBoxes = new MixByteCollectionCharTextBox[] { this.loaderCard1TextBox, this.loaderCard2TextBox, this.loaderCard3TextBox };

			for (int index = 0; index < this.loaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = this.loaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.NavigationKeyDown += This_KeyDown;
				loaderCardTextBox.ValueChanged += LoaderCardTextBox_ValueChanged;
			}
		}

		private void LoaderCardTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
			=> this.loaderCardsDefaultButton.Enabled = true;

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
					index = Array.IndexOf(this.loaderCardTextBoxes, senderTextBox);

					if (index <= 0)
						break;

					targetTextBox = this.loaderCardTextBoxes[index - 1];
					targetTextBox.Focus();

					if (caretPos.HasValue)
						targetTextBox.Select(caretPos.Value, 0);

					break;

				case Keys.Down:
					index = Array.IndexOf(this.loaderCardTextBoxes, senderTextBox);

					if (index >= this.loaderCardTextBoxes.Length - 1)
						break;

					targetTextBox = this.loaderCardTextBoxes[index + 1];
					targetTextBox.Focus();

					if (caretPos.HasValue)
						targetTextBox.Select(caretPos.Value, 0);

					break;
			}
		}

		private void FillDeviceReloadIntervalSelectionBox()
		{
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(250));
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(500));
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(750));
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1000));
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1500));
			this.deviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(2000));
		}

		private void FillColorSelectionBox()
		{
			this.colorSelectionBox.Items.Clear();
			string[] knownColorNames = GuiSettings.KnownColorNames;

			var list = new List<ColorComboBoxItem>();
			foreach (string colorName in knownColorNames)
			{
				var comboBoxItem = new ColorComboBoxItem(colorName);

				if (this.configuration.Colors.ContainsKey(colorName))
					comboBoxItem.Color = this.configuration.Colors[colorName];

				list.Add(comboBoxItem);
			}

			this.colorSelectionBox.Items.AddRange(list.ToArray());

			if (this.colorSelectionBox.Items.Count > 0)
				this.colorSelectionBox.SelectedIndex = 0;

			UpdateColorControls((ColorComboBoxItem)this.colorSelectionBox.SelectedItem);
		}

		private void FillDeviceFileSelectionBox()
		{
			this.deviceFileSelectionBox.Items.Clear();
			var list = new List<DeviceFileComboBoxItem>();
			foreach (MixDevice device in this.devices)
			{
				if (!(device is FileBasedDevice))
					continue;

				var item = new DeviceFileComboBoxItem((FileBasedDevice)device);

				if (this.configuration.DeviceFilePaths.ContainsKey(device.Id))
					item.FilePath = this.configuration.DeviceFilePaths[device.Id];

				list.Add(item);
			}

			this.deviceFileSelectionBox.Items.AddRange(list.ToArray());

			if (this.deviceFileSelectionBox.Items.Count > 0)
				this.deviceFileSelectionBox.SelectedIndex = 0;

			UpdateDeviceFileControls((DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem);
		}

		private void FillTickCountSelectionBox()
		{
			this.tickCountSelectionBox.Items.Clear();

			string[] knownTickCountNames = DeviceSettings.KnownTickCountNames;

			var list = new List<TickCountComboBoxItem>();
			foreach (string tickCountName in knownTickCountNames)
			{
				var item = new TickCountComboBoxItem(tickCountName);

				if (this.configuration.TickCounts.ContainsKey(tickCountName))
					item.TickCount = this.configuration.TickCounts[tickCountName];

				list.Add(item);
			}

			this.tickCountSelectionBox.Items.AddRange(list.ToArray());

			if (this.tickCountSelectionBox.Items.Count > 0)
	  		this.tickCountSelectionBox.SelectedIndex = 0;

			UpdateTickCountControls((TickCountComboBoxItem)this.tickCountSelectionBox.SelectedItem);
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

			foreach (ColorComboBoxItem item in this.colorSelectionBox.Items)
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
			this.colorsGroup = new GroupBox();
			this.colorSetButton = new Button();
			this.showColorLabel = new Label();
			this.colorSelectionBox = new ComboBox();
			this.colorDefaultButton = new Button();
			this.deviceFilesGroup = new GroupBox();
			this.deviceDirectoryBox = new TextBox();
			this.deviceDirectoryLabel = new Label();
			this.deviceDirectorySetButton = new Button();
			this.deviceDirectoryDefaultButton = new Button();
			this.deviceFileSelectionBox = new ComboBox();
			this.deviceFileBox = new TextBox();
			this.deviceFileSetButton = new Button();
			this.deviceFileDefaultButton = new Button();
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.deviceTickCountsGroup = new GroupBox();
			this.tickCountDefaultButton = new Button();
			this.tickCountSetButton = new Button();
			this.tickCountSelectionBox = new ComboBox();
			this.defaultsButton = new Button();
			this.miscGroupBox = new GroupBox();
			this.deviceReloadIntervalComboBox = new ComboBox();
			this.deviceReloadIntervalLabel = new Label();
			this.deviceReloadIntervalDefaultButton = new Button();
			this.loaderCardsGroupBox = new GroupBox();
			this.loaderCardsDefaultButton = new Button();
			this.loaderCardsPanel = new Panel();
			this.floatingPointGroupBox = new GroupBox();
			this.floatingPointMemoryWordCountLabel = new Label();
			this.floatingPointMemoryWordCountDefaultButton = new Button();
			this.floatingPointMemoryWordCountBox = new LongValueTextBox();
			this.loaderCard3TextBox = new MixByteCollectionCharTextBox();
			this.loaderCard2TextBox = new MixByteCollectionCharTextBox();
			this.loaderCard1TextBox = new MixByteCollectionCharTextBox();
			this.tickCountBox = new LongValueTextBox();
			this.colorProfilingCountsCheckBox = new CheckBox();
			this.colorsGroup.SuspendLayout();
			this.deviceFilesGroup.SuspendLayout();
			this.deviceTickCountsGroup.SuspendLayout();
			this.miscGroupBox.SuspendLayout();
			this.loaderCardsGroupBox.SuspendLayout();
			this.loaderCardsPanel.SuspendLayout();
			this.floatingPointGroupBox.SuspendLayout();
			SuspendLayout();
			// 
			// mColorsGroup
			// 
			this.colorsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.colorsGroup.Controls.Add(this.colorProfilingCountsCheckBox);
			this.colorsGroup.Controls.Add(this.colorSetButton);
			this.colorsGroup.Controls.Add(this.showColorLabel);
			this.colorsGroup.Controls.Add(this.colorSelectionBox);
			this.colorsGroup.Controls.Add(this.colorDefaultButton);
			this.colorsGroup.Location = new Point(12, 12);
			this.colorsGroup.Name = "mColorsGroup";
			this.colorsGroup.Size = new Size(366, 70);
			this.colorsGroup.TabIndex = 1;
			this.colorsGroup.TabStop = false;
			this.colorsGroup.Text = "Colors";
			// 
			// mColorSetButton
			// 
			this.colorSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.colorSetButton.Location = new Point(254, 15);
			this.colorSetButton.Name = "mColorSetButton";
			this.colorSetButton.Size = new Size(40, 23);
			this.colorSetButton.TabIndex = 2;
			this.colorSetButton.Text = "&Set...";
			this.colorSetButton.FlatStyle = FlatStyle.Flat;
			this.colorSetButton.Click += ColorSetButton_Click;
			// 
			// mShowColorLabel
			// 
			this.showColorLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.showColorLabel.BorderStyle = BorderStyle.FixedSingle;
			this.showColorLabel.Location = new Point(198, 16);
			this.showColorLabel.Name = "mShowColorLabel";
			this.showColorLabel.Size = new Size(48, 21);
			this.showColorLabel.TabIndex = 1;
			this.showColorLabel.Text = "Color";
			this.showColorLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// mColorSelectionBox
			// 
			this.colorSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.colorSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.colorSelectionBox.Location = new Point(8, 16);
			this.colorSelectionBox.Name = "mColorSelectionBox";
			this.colorSelectionBox.Size = new Size(182, 21);
			this.colorSelectionBox.Sorted = true;
			this.colorSelectionBox.TabIndex = 0;
			this.colorSelectionBox.FlatStyle = FlatStyle.Flat;
			this.colorSelectionBox.SelectionChangeCommitted += ColorSelectionBox_SelectionChangeCommited;
			// 
			// mColorDefaultButton
			// 
			this.colorDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.colorDefaultButton.Location = new Point(302, 15);
			this.colorDefaultButton.Name = "mColorDefaultButton";
			this.colorDefaultButton.Size = new Size(56, 23);
			this.colorDefaultButton.TabIndex = 3;
			this.colorDefaultButton.Text = "Default";
			this.colorDefaultButton.FlatStyle = FlatStyle.Flat;
			this.colorDefaultButton.Click += ColorDefaultButton_Click;
			// 
			// mDeviceFilesGroup
			// 
			this.deviceFilesGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.deviceFilesGroup.Controls.Add(this.deviceDirectoryBox);
			this.deviceFilesGroup.Controls.Add(this.deviceDirectoryLabel);
			this.deviceFilesGroup.Controls.Add(this.deviceDirectorySetButton);
			this.deviceFilesGroup.Controls.Add(this.deviceDirectoryDefaultButton);
			this.deviceFilesGroup.Controls.Add(this.deviceFileSelectionBox);
			this.deviceFilesGroup.Controls.Add(this.deviceFileBox);
			this.deviceFilesGroup.Controls.Add(this.deviceFileSetButton);
			this.deviceFilesGroup.Controls.Add(this.deviceFileDefaultButton);
			this.deviceFilesGroup.Location = new Point(12, 131);
			this.deviceFilesGroup.Name = "mDeviceFilesGroup";
			this.deviceFilesGroup.Size = new Size(366, 80);
			this.deviceFilesGroup.TabIndex = 3;
			this.deviceFilesGroup.TabStop = false;
			this.deviceFilesGroup.Text = "Device files";
			// 
			// mDeviceDirectoryBox
			// 
			this.deviceDirectoryBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.deviceDirectoryBox.Location = new Point(104, 16);
			this.deviceDirectoryBox.Name = "mDeviceDirectoryBox";
			this.deviceDirectoryBox.ReadOnly = true;
			this.deviceDirectoryBox.Size = new Size(142, 20);
			this.deviceDirectoryBox.TabIndex = 1;
			this.deviceDirectoryBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mDeviceDirectoryLabel
			// 
			this.deviceDirectoryLabel.Location = new Point(6, 16);
			this.deviceDirectoryLabel.Name = "mDeviceDirectoryLabel";
			this.deviceDirectoryLabel.Size = new Size(100, 21);
			this.deviceDirectoryLabel.TabIndex = 0;
			this.deviceDirectoryLabel.Text = "Default directory:";
			this.deviceDirectoryLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mDeviceDirectorySetButton
			// 
			this.deviceDirectorySetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceDirectorySetButton.Location = new Point(254, 15);
			this.deviceDirectorySetButton.Name = "mDeviceDirectorySetButton";
			this.deviceDirectorySetButton.Size = new Size(40, 23);
			this.deviceDirectorySetButton.TabIndex = 2;
			this.deviceDirectorySetButton.Text = "S&et...";
			this.deviceDirectorySetButton.FlatStyle = FlatStyle.Flat;
			this.deviceDirectorySetButton.Click += DeviceDirectorySetButton_Click;
			// 
			// mDeviceDirectoryDefaultButton
			// 
			this.deviceDirectoryDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceDirectoryDefaultButton.Location = new Point(302, 15);
			this.deviceDirectoryDefaultButton.Name = "mDeviceDirectoryDefaultButton";
			this.deviceDirectoryDefaultButton.Size = new Size(56, 23);
			this.deviceDirectoryDefaultButton.TabIndex = 3;
			this.deviceDirectoryDefaultButton.Text = "Default";
			this.deviceDirectoryDefaultButton.FlatStyle = FlatStyle.Flat;
			this.deviceDirectoryDefaultButton.Click += DeviceDirectoryDefaultButton_Click;
			// 
			// mDeviceFileSelectionBox
			// 
			this.deviceFileSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.deviceFileSelectionBox.Location = new Point(6, 48);
			this.deviceFileSelectionBox.Name = "mDeviceFileSelectionBox";
			this.deviceFileSelectionBox.Size = new Size(92, 21);
			this.deviceFileSelectionBox.Sorted = true;
			this.deviceFileSelectionBox.TabIndex = 4;
			this.deviceFileSelectionBox.FlatStyle = FlatStyle.Flat;
			this.deviceFileSelectionBox.SelectionChangeCommitted += DeviceFileSelectionBox_SelectionChangeCommitted;
			// 
			// mDeviceFileBox
			// 
			this.deviceFileBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.deviceFileBox.Location = new Point(104, 48);
			this.deviceFileBox.Name = "mDeviceFileBox";
			this.deviceFileBox.ReadOnly = true;
			this.deviceFileBox.Size = new Size(142, 20);
			this.deviceFileBox.BorderStyle = BorderStyle.FixedSingle;
			this.deviceFileBox.TabIndex = 5;
			// 
			// mDeviceFileSetButton
			// 
			this.deviceFileSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceFileSetButton.Location = new Point(254, 47);
			this.deviceFileSetButton.Name = "mDeviceFileSetButton";
			this.deviceFileSetButton.Size = new Size(40, 23);
			this.deviceFileSetButton.TabIndex = 6;
			this.deviceFileSetButton.Text = "Se&t...";
			this.deviceFileSetButton.FlatStyle = FlatStyle.Flat;
			this.deviceFileSetButton.Click += DeviceFileSetButton_Click;
			// 
			// mDeviceFileDefaultButton
			// 
			this.deviceFileDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceFileDefaultButton.Location = new Point(302, 47);
			this.deviceFileDefaultButton.Name = "mDeviceFileDefaultButton";
			this.deviceFileDefaultButton.Size = new Size(56, 23);
			this.deviceFileDefaultButton.TabIndex = 7;
			this.deviceFileDefaultButton.Text = "Default";
			this.deviceFileDefaultButton.FlatStyle = FlatStyle.Flat;
			this.deviceFileDefaultButton.Click += DeviceFileDefaultButton_Click;
			// 
			// mOkButton
			// 
			this.okButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Location = new Point(224, 382);
			this.okButton.Name = "mOkButton";
			this.okButton.Size = new Size(75, 23);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "&OK";
			this.okButton.FlatStyle = FlatStyle.Flat;
			this.okButton.Click += OkButton_Click;
			// 
			// mCancelButton
			// 
			this.cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(304, 382);
			this.cancelButton.Name = "mCancelButton";
			this.cancelButton.Size = new Size(75, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.FlatStyle = FlatStyle.Flat;
			this.cancelButton.Text = "&Cancel";
			// 
			// mDeviceTickCountsGroup
			// 
			this.deviceTickCountsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.deviceTickCountsGroup.Controls.Add(this.tickCountBox);
			this.deviceTickCountsGroup.Controls.Add(this.tickCountDefaultButton);
			this.deviceTickCountsGroup.Controls.Add(this.tickCountSetButton);
			this.deviceTickCountsGroup.Controls.Add(this.tickCountSelectionBox);
			this.deviceTickCountsGroup.Location = new Point(12, 211);
			this.deviceTickCountsGroup.Name = "mDeviceTickCountsGroup";
			this.deviceTickCountsGroup.Size = new Size(366, 48);
			this.deviceTickCountsGroup.TabIndex = 4;
			this.deviceTickCountsGroup.TabStop = false;
			this.deviceTickCountsGroup.Text = "Device tick counts";
			// 
			// mTickCountDefaultButton
			// 
			this.tickCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.tickCountDefaultButton.Location = new Point(302, 16);
			this.tickCountDefaultButton.Name = "mTickCountDefaultButton";
			this.tickCountDefaultButton.Size = new Size(56, 23);
			this.tickCountDefaultButton.TabIndex = 3;
			this.tickCountDefaultButton.Text = "Default";
			this.tickCountDefaultButton.FlatStyle = FlatStyle.Flat;
			this.tickCountDefaultButton.Click += TickCountDefaultButton_Click;
			// 
			// mTickCountSetButton
			// 
			this.tickCountSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.tickCountSetButton.Location = new Point(246, 16);
			this.tickCountSetButton.Name = "mTickCountSetButton";
			this.tickCountSetButton.Size = new Size(48, 23);
			this.tickCountSetButton.TabIndex = 2;
			this.tickCountSetButton.Text = "&Apply";
			this.tickCountSetButton.FlatStyle = FlatStyle.Flat;
			this.tickCountSetButton.Click += TickCountSetButton_Click;
			// 
			// mTickCountSelectionBox
			// 
			this.tickCountSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.tickCountSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.tickCountSelectionBox.Location = new Point(6, 16);
			this.tickCountSelectionBox.Name = "mTickCountSelectionBox";
			this.tickCountSelectionBox.Size = new Size(182, 21);
			this.tickCountSelectionBox.Sorted = true;
			this.tickCountSelectionBox.TabIndex = 0;
			this.tickCountSelectionBox.FlatStyle = FlatStyle.Flat;
			this.tickCountSelectionBox.SelectionChangeCommitted += TickCountSelectionBox_SelectionChangeCommitted;
			// 
			// mDefaultsButton
			// 
			this.defaultsButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.defaultsButton.Location = new Point(144, 382);
			this.defaultsButton.Name = "mDefaultsButton";
			this.defaultsButton.Size = new Size(75, 23);
			this.defaultsButton.TabIndex = 7;
			this.defaultsButton.Text = "&Defaults";
			this.defaultsButton.FlatStyle = FlatStyle.Flat;
			this.defaultsButton.Click += DefaultsButton_Click;
			// 
			// mMiscGroupBox
			// 
			this.miscGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.miscGroupBox.Controls.Add(this.deviceReloadIntervalComboBox);
			this.miscGroupBox.Controls.Add(this.deviceReloadIntervalLabel);
			this.miscGroupBox.Controls.Add(this.deviceReloadIntervalDefaultButton);
			this.miscGroupBox.Location = new Point(12, 327);
			this.miscGroupBox.Name = "mMiscGroupBox";
			this.miscGroupBox.Size = new Size(366, 48);
			this.miscGroupBox.TabIndex = 6;
			this.miscGroupBox.TabStop = false;
			this.miscGroupBox.Text = "Miscellaneous";
			// 
			// mDeviceReloadIntervalComboBox
			// 
			this.deviceReloadIntervalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.deviceReloadIntervalComboBox.FormattingEnabled = true;
			this.deviceReloadIntervalComboBox.Location = new Point(156, 15);
			this.deviceReloadIntervalComboBox.Name = "mDeviceReloadIntervalComboBox";
			this.deviceReloadIntervalComboBox.Size = new Size(73, 21);
			this.deviceReloadIntervalComboBox.TabIndex = 5;
			this.deviceReloadIntervalComboBox.FlatStyle = FlatStyle.Flat;
			this.deviceReloadIntervalComboBox.SelectedIndexChanged += DeviceReloadIntervalComboBox_SelectedIndexChanged;
			// 
			// mDeviceReloadIntervalLabel
			// 
			this.deviceReloadIntervalLabel.AutoSize = true;
			this.deviceReloadIntervalLabel.Location = new Point(6, 18);
			this.deviceReloadIntervalLabel.Name = "mDeviceReloadIntervalLabel";
			this.deviceReloadIntervalLabel.Size = new Size(145, 13);
			this.deviceReloadIntervalLabel.TabIndex = 4;
			this.deviceReloadIntervalLabel.Text = "Device editor reload interval: ";
			// 
			// mDeviceReloadIntervalDefaultButton
			// 
			this.deviceReloadIntervalDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceReloadIntervalDefaultButton.Location = new Point(302, 15);
			this.deviceReloadIntervalDefaultButton.Name = "mDeviceReloadIntervalDefaultButton";
			this.deviceReloadIntervalDefaultButton.Size = new Size(56, 23);
			this.deviceReloadIntervalDefaultButton.TabIndex = 3;
			this.deviceReloadIntervalDefaultButton.Text = "Default";
			this.deviceReloadIntervalDefaultButton.FlatStyle = FlatStyle.Flat;
			this.deviceReloadIntervalDefaultButton.Click += DeviceReloadIntervalDefaultButton_Click;
			// 
			// mLoaderCardsGroupBox
			// 
			this.loaderCardsGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.loaderCardsGroupBox.Controls.Add(this.loaderCardsDefaultButton);
			this.loaderCardsGroupBox.Controls.Add(this.loaderCardsPanel);
			this.loaderCardsGroupBox.Location = new Point(12, 259);
			this.loaderCardsGroupBox.Name = "mLoaderCardsGroupBox";
			this.loaderCardsGroupBox.Size = new Size(366, 68);
			this.loaderCardsGroupBox.TabIndex = 5;
			this.loaderCardsGroupBox.TabStop = false;
			this.loaderCardsGroupBox.Text = "Loader instruction cards";
			// 
			// mLoaderCardsDefaultButton
			// 
			this.loaderCardsDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.loaderCardsDefaultButton.Location = new Point(302, 16);
			this.loaderCardsDefaultButton.Name = "mLoaderCardsDefaultButton";
			this.loaderCardsDefaultButton.Size = new Size(56, 23);
			this.loaderCardsDefaultButton.TabIndex = 4;
			this.loaderCardsDefaultButton.Text = "Default";
			this.loaderCardsDefaultButton.FlatStyle = FlatStyle.Flat;
			this.loaderCardsDefaultButton.Click += LoaderCardsDefaultButton_Click;
			// 
			// mLoaderCardsPanel
			// 
			this.loaderCardsPanel.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.loaderCardsPanel.BorderStyle = BorderStyle.FixedSingle;
			this.loaderCardsPanel.Controls.Add(this.loaderCard3TextBox);
			this.loaderCardsPanel.Controls.Add(this.loaderCard2TextBox);
			this.loaderCardsPanel.Controls.Add(this.loaderCard1TextBox);
			this.loaderCardsPanel.Location = new Point(8, 16);
			this.loaderCardsPanel.Name = "mLoaderCardsPanel";
			this.loaderCardsPanel.Size = new Size(286, 44);
			this.loaderCardsPanel.TabIndex = 0;
			// 
			// mFloatingPointGroupBox
			// 
			this.floatingPointGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.floatingPointGroupBox.Controls.Add(this.floatingPointMemoryWordCountBox);
			this.floatingPointGroupBox.Controls.Add(this.floatingPointMemoryWordCountLabel);
			this.floatingPointGroupBox.Controls.Add(this.floatingPointMemoryWordCountDefaultButton);
			this.floatingPointGroupBox.Location = new Point(12, 83);
			this.floatingPointGroupBox.Name = "mFloatingPointGroupBox";
			this.floatingPointGroupBox.Size = new Size(366, 48);
			this.floatingPointGroupBox.TabIndex = 2;
			this.floatingPointGroupBox.TabStop = false;
			this.floatingPointGroupBox.Text = "Floating point module";
			// 
			// mFloatingPointMemoryWordCountLabel
			// 
			this.floatingPointMemoryWordCountLabel.AutoSize = true;
			this.floatingPointMemoryWordCountLabel.Location = new Point(6, 18);
			this.floatingPointMemoryWordCountLabel.Name = "mFloatingPointMemoryWordCountLabel";
			this.floatingPointMemoryWordCountLabel.Size = new Size(181, 13);
			this.floatingPointMemoryWordCountLabel.TabIndex = 0;
			this.floatingPointMemoryWordCountLabel.Text = "Memory word count (requires restart):";
			// 
			// mFloatingPointMemoryWordCountDefaultButton
			// 
			this.floatingPointMemoryWordCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.floatingPointMemoryWordCountDefaultButton.Location = new Point(302, 15);
			this.floatingPointMemoryWordCountDefaultButton.Name = "mFloatingPointMemoryWordCountDefaultButton";
			this.floatingPointMemoryWordCountDefaultButton.Size = new Size(56, 23);
			this.floatingPointMemoryWordCountDefaultButton.TabIndex = 2;
			this.floatingPointMemoryWordCountDefaultButton.Text = "Default";
			this.floatingPointMemoryWordCountDefaultButton.FlatStyle = FlatStyle.Flat;
			this.floatingPointMemoryWordCountDefaultButton.Click += FloatingPointMemoryWordCountDefaultButton_Click;
			// 
			// mFloatingPointMemoryWordCountBox
			// 
			this.floatingPointMemoryWordCountBox.BackColor = Color.White;
			this.floatingPointMemoryWordCountBox.BorderStyle = BorderStyle.FixedSingle;
			this.floatingPointMemoryWordCountBox.ClearZero = true;
			this.floatingPointMemoryWordCountBox.ForeColor = Color.Black;
			this.floatingPointMemoryWordCountBox.Location = new Point(199, 16);
			this.floatingPointMemoryWordCountBox.LongValue = 200;
			this.floatingPointMemoryWordCountBox.Magnitude = 200;
			this.floatingPointMemoryWordCountBox.MaxLength = 5;
			this.floatingPointMemoryWordCountBox.MaxValue = 99999;
			this.floatingPointMemoryWordCountBox.MinValue = 1;
			this.floatingPointMemoryWordCountBox.Name = "mFloatingPointMemoryWordCountBox";
			this.floatingPointMemoryWordCountBox.Sign = Word.Signs.Positive;
			this.floatingPointMemoryWordCountBox.Size = new Size(40, 20);
			this.floatingPointMemoryWordCountBox.SupportNegativeZero = false;
			this.floatingPointMemoryWordCountBox.TabIndex = 1;
			this.floatingPointMemoryWordCountBox.Text = "200";
			this.floatingPointMemoryWordCountBox.ValueChanged += FloatingPointMemoryWordCountBox_ValueChanged;
			// 
			// mLoaderCard3TextBox
			// 
			this.loaderCard3TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.loaderCard3TextBox.BackColor = Color.White;
			this.loaderCard3TextBox.BorderStyle = BorderStyle.None;
			this.loaderCard3TextBox.CharacterCasing = CharacterCasing.Upper;
			this.loaderCard3TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.loaderCard3TextBox.ForeColor = Color.Blue;
			this.loaderCard3TextBox.Location = new Point(0, 28);
			this.loaderCard3TextBox.Margin = new Padding(0);
			fullWord1.LongValue = 0;
			fullWord1.Magnitude = new MixByte[] {
				mixByte1,
				mixByte2,
				mixByte3,
				mixByte4,
				mixByte5};
			fullWord1.MagnitudeLongValue = 0;
			fullWord1.Sign = Word.Signs.Positive;
			this.loaderCard3TextBox.MixByteCollectionValue = fullWord1;
			this.loaderCard3TextBox.Name = "mLoaderCard3TextBox";
			this.loaderCard3TextBox.Size = new Size(284, 14);
			this.loaderCard3TextBox.TabIndex = 2;
			this.loaderCard3TextBox.Text = "     ";
			this.loaderCard3TextBox.UseEditMode = true;
			// 
			// mLoaderCard2TextBox
			// 
			this.loaderCard2TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.loaderCard2TextBox.BackColor = Color.White;
			this.loaderCard2TextBox.BorderStyle = BorderStyle.None;
			this.loaderCard2TextBox.CharacterCasing = CharacterCasing.Upper;
			this.loaderCard2TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.loaderCard2TextBox.ForeColor = Color.Blue;
			this.loaderCard2TextBox.Location = new Point(0, 14);
			this.loaderCard2TextBox.Margin = new Padding(0);
			fullWord2.LongValue = 0;
			fullWord2.Magnitude = new MixByte[] {
				mixByte6,
				mixByte7,
				mixByte8,
				mixByte9,
				mixByte10};
			fullWord2.MagnitudeLongValue = 0;
			fullWord2.Sign = Word.Signs.Positive;
			this.loaderCard2TextBox.MixByteCollectionValue = fullWord2;
			this.loaderCard2TextBox.Name = "mLoaderCard2TextBox";
			this.loaderCard2TextBox.Size = new Size(284, 14);
			this.loaderCard2TextBox.TabIndex = 1;
			this.loaderCard2TextBox.Text = "     ";
			this.loaderCard2TextBox.UseEditMode = true;
			// 
			// mLoaderCard1TextBox
			// 
			this.loaderCard1TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.loaderCard1TextBox.BackColor = Color.White;
			this.loaderCard1TextBox.BorderStyle = BorderStyle.None;
			this.loaderCard1TextBox.CharacterCasing = CharacterCasing.Upper;
			this.loaderCard1TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.loaderCard1TextBox.ForeColor = Color.Blue;
			this.loaderCard1TextBox.Location = new Point(0, 0);
			this.loaderCard1TextBox.Margin = new Padding(0);
			fullWord3.LongValue = 0;
			fullWord3.Magnitude = new MixByte[] {
				mixByte11,
				mixByte12,
				mixByte13,
				mixByte14,
				mixByte15};
			fullWord3.MagnitudeLongValue = 0;
			fullWord3.Sign = Word.Signs.Positive;
			this.loaderCard1TextBox.MixByteCollectionValue = fullWord3;
			this.loaderCard1TextBox.Name = "mLoaderCard1TextBox";
			this.loaderCard1TextBox.Size = new Size(284, 14);
			this.loaderCard1TextBox.TabIndex = 0;
			this.loaderCard1TextBox.Text = "     ";
			this.loaderCard1TextBox.UseEditMode = true;
			// 
			// mTickCountBox
			// 
			this.tickCountBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.tickCountBox.BackColor = Color.White;
			this.tickCountBox.BorderStyle = BorderStyle.FixedSingle;
			this.tickCountBox.ClearZero = true;
			this.tickCountBox.ForeColor = Color.Black;
			this.tickCountBox.Location = new Point(198, 17);
			this.tickCountBox.LongValue = 1;
			this.tickCountBox.Magnitude = 1;
			this.tickCountBox.MaxLength = 5;
			this.tickCountBox.MaxValue = 99999;
			this.tickCountBox.MinValue = 1;
			this.tickCountBox.Name = "mTickCountBox";
			this.tickCountBox.Sign = Word.Signs.Positive;
			this.tickCountBox.Size = new Size(40, 20);
			this.tickCountBox.SupportNegativeZero = false;
			this.tickCountBox.TabIndex = 1;
			this.tickCountBox.Text = "1";
			this.tickCountBox.ValueChanged += TickCountBox_ValueChanged;
			// 
			// mColorProfilingCountsCheckBox
			// 
			this.colorProfilingCountsCheckBox.AutoSize = true;
			this.colorProfilingCountsCheckBox.Checked = true;
			this.colorProfilingCountsCheckBox.CheckState = CheckState.Checked;
			this.colorProfilingCountsCheckBox.Location = new Point(8, 43);
			this.colorProfilingCountsCheckBox.Name = "mColorProfilingCountsCheckBox";
			this.colorProfilingCountsCheckBox.Size = new Size(119, 17);
			this.colorProfilingCountsCheckBox.TabIndex = 4;
			this.colorProfilingCountsCheckBox.Text = "Color profiler counts";
			this.colorProfilingCountsCheckBox.UseVisualStyleBackColor = true;
			this.colorProfilingCountsCheckBox.FlatStyle = FlatStyle.Flat;
			// 
			// PreferencesForm
			// 
			ClientSize = new Size(384, 412);
			Controls.Add(this.floatingPointGroupBox);
			Controls.Add(this.loaderCardsGroupBox);
			Controls.Add(this.miscGroupBox);
			Controls.Add(this.defaultsButton);
			Controls.Add(this.deviceTickCountsGroup);
			Controls.Add(this.okButton);
			Controls.Add(this.deviceFilesGroup);
			Controls.Add(this.colorsGroup);
			Controls.Add(this.cancelButton);
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
			this.colorsGroup.ResumeLayout(false);
			this.colorsGroup.PerformLayout();
			this.deviceFilesGroup.ResumeLayout(false);
			this.deviceFilesGroup.PerformLayout();
			this.deviceTickCountsGroup.ResumeLayout(false);
			this.deviceTickCountsGroup.PerformLayout();
			this.miscGroupBox.ResumeLayout(false);
			this.miscGroupBox.PerformLayout();
			this.loaderCardsGroupBox.ResumeLayout(false);
			this.loaderCardsPanel.ResumeLayout(false);
			this.loaderCardsPanel.PerformLayout();
			this.floatingPointGroupBox.ResumeLayout(false);
			this.floatingPointGroupBox.PerformLayout();
			ResumeLayout(false);

		}

		private void ColorDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (ColorComboBoxItem)this.colorSelectionBox.SelectedItem;

			selectedItem.ResetDefault();
			UpdateColorControls(selectedItem);
			this.colorSelectionBox.Focus();
		}

		private void ColorSetButton_Click(object sender, EventArgs e)
		{
			if (this.colorDialog == null)
			{
				this.colorDialog = new ColorDialog
				{
					AnyColor = true,
					SolidColorOnly = true,
					AllowFullOpen = true
				};
			}

			var selectedItem = (ColorComboBoxItem)this.colorSelectionBox.SelectedItem;
			this.colorDialog.CustomColors = new int[] { selectedItem.Color.B << 16 | selectedItem.Color.G << 8 | selectedItem.Color.R };
			this.colorDialog.Color = selectedItem.Color;

			if (this.colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.Color = this.colorDialog.Color;
				UpdateColorControls(selectedItem);
			}
		}

		private void DefaultsButton_Click(object sender, EventArgs e)
		{
			foreach (ColorComboBoxItem colorItem in this.colorSelectionBox.Items)
				colorItem.ResetDefault();

			this.colorProfilingCountsCheckBox.Checked = true;

			foreach (DeviceFileComboBoxItem deviceFileItem in this.deviceFileSelectionBox.Items)
				deviceFileItem.ResetDefault();

			foreach (TickCountComboBoxItem tickCountItem in this.tickCountSelectionBox.Items)
				tickCountItem.ResetDefault();

			this.deviceFilesDirectory = null;
			this.deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			UpdateColorControls((ColorComboBoxItem)this.colorSelectionBox.SelectedItem);
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem);
			UpdateTickCountControls((TickCountComboBoxItem)this.tickCountSelectionBox.SelectedItem);
			UpdateDeviceReloadIntervalControls();
			this.floatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();

			LoadDefaultLoaderCards();
		}

		private void UpdateFloatingPointControls()
		{
			this.floatingPointMemoryWordCountBox.LongValue = this.floatingPointMemoryWordCount ?? ModuleSettings.FloatingPointMemoryWordCountDefault;
			this.floatingPointMemoryWordCountDefaultButton.Enabled = this.floatingPointMemoryWordCount != null;
		}

		private void DeviceDirectoryDefaultButton_Click(object sender, EventArgs e)
		{
			this.deviceFilesDirectory = null;
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem);
			this.deviceDirectoryBox.Focus();
		}

		private void DeviceDirectorySetButton_Click(object sender, EventArgs e)
		{
			if (this.deviceFilesFolderBrowserDialog == null)
			{
				this.deviceFilesFolderBrowserDialog = new FolderBrowserDialog
				{
					Description = "Please select the default directory for device files."
				};
			}

			this.deviceFilesFolderBrowserDialog.SelectedPath = GetCurrentDeviceFilesDirectory();

			if (this.deviceFilesFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.deviceFilesDirectory = this.deviceFilesFolderBrowserDialog.SelectedPath;
				UpdateDeviceDirectoryControls();
				UpdateDeviceFileControls((DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem);
			}
		}

		private void DeviceFileDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateDeviceFileControls(selectedItem);
			this.deviceFileSelectionBox.Focus();
		}

		private void DeviceFileSetButton_Click(object sender, EventArgs e)
		{
			if (this.selectDeviceFileDialog == null)
			{
				this.selectDeviceFileDialog = new SaveFileDialog
				{
					CreatePrompt = true,
					DefaultExt = "mixdev",
					Filter = "MIX device files|*.mixdev|All files|*.*",
					OverwritePrompt = false
				};
			}

			var selectedItem = (DeviceFileComboBoxItem)this.deviceFileSelectionBox.SelectedItem;
			this.selectDeviceFileDialog.FileName = Path.Combine(GetCurrentDeviceFilesDirectory(), selectedItem.FilePath);

			if (this.selectDeviceFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.FilePath = this.selectDeviceFileDialog.FileName;
				UpdateDeviceFileControls(selectedItem);
			}
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			this.configuration.Colors.Clear();
			foreach (ColorComboBoxItem colorItem in this.colorSelectionBox.Items)
			{
				if (!colorItem.IsDefault)
					this.configuration.Colors.Add(colorItem.Name, colorItem.Color);
			}

			this.configuration.ColorProfilingCounts = this.colorProfilingCountsCheckBox.Checked;

			this.configuration.DeviceFilePaths.Clear();
			foreach (DeviceFileComboBoxItem deviceFileItem in this.deviceFileSelectionBox.Items)
			{
				if (!deviceFileItem.IsDefault)
					this.configuration.DeviceFilePaths.Add(deviceFileItem.Id, deviceFileItem.FilePath);
			}

			this.configuration.TickCounts.Clear();
			foreach (TickCountComboBoxItem tickCountItem in this.tickCountSelectionBox.Items)
			{
				if (!tickCountItem.IsDefault)
					this.configuration.TickCounts.Add(tickCountItem.Name, tickCountItem.TickCount);
			}

			this.configuration.DeviceFilesDirectory = this.deviceFilesDirectory;
			this.configuration.DeviceReloadInterval = this.deviceReloadInterval;

			this.configuration.FloatingPointMemoryWordCount = this.floatingPointMemoryWordCount;

			this.configuration.LoaderCards = this.loaderCardsDefaultButton.Enabled ? this.loaderCardTextBoxes.Select(box => box.MixByteCollectionValue.ToString(true)).ToArray() : null;

			try
			{
				this.configuration.Save(this.defaultDirectory);
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
			var selectedItem = (TickCountComboBoxItem)this.tickCountSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateTickCountControls(selectedItem);
			this.tickCountSelectionBox.Focus();
		}

		private void SetTickCountValue()
		{
			var selectedItem = (TickCountComboBoxItem)this.tickCountSelectionBox.SelectedItem;
			selectedItem.TickCount = (int)this.tickCountBox.LongValue;
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
			this.colorProfilingCountsCheckBox.Checked = this.configuration.ColorProfilingCounts;
			this.deviceFilesDirectory = this.configuration.DeviceFilesDirectory;
			UpdateDeviceDirectoryControls();
			FillDeviceFileSelectionBox();
			FillTickCountSelectionBox();
			this.deviceReloadInterval = this.configuration.DeviceReloadInterval;
			UpdateDeviceReloadIntervalControls();
			this.floatingPointMemoryWordCount = this.configuration.FloatingPointMemoryWordCount;
			UpdateFloatingPointControls();
			LoadLoaderCards();
		}

		private void LoadLoaderCards()
		{
			string[] loaderCards = this.configuration.LoaderCards ?? CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < this.loaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = this.loaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.MixByteCollectionValue.Load(index < loaderCards.Length ? loaderCards[index] : string.Empty);
				loaderCardTextBox.Update();
			}

			this.loaderCardsDefaultButton.Enabled = this.configuration.LoaderCards != null;
		}

		private void UpdateColorControls(ColorComboBoxItem item)
		{
			if (item == null)
			{
				this.showColorLabel.ForeColor = Color.Black;
				this.showColorLabel.BackColor = Color.White;

				this.colorSetButton.Enabled = false;
				this.colorDefaultButton.Enabled = false;
			}
			else
			{
				if (item.IsBackground)
				{
					this.showColorLabel.ForeColor = GetMatchingColor(item.Name, false);
					this.showColorLabel.BackColor = item.Color;
				}
				else
				{
					this.showColorLabel.ForeColor = item.Color;
					this.showColorLabel.BackColor = GetMatchingColor(item.Name, true);
				}

				this.colorSetButton.Enabled = true;
				this.colorDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void UpdateDeviceReloadIntervalControls()
		{
			if (this.deviceReloadInterval == DeviceSettings.UnsetDeviceReloadInterval)
			{
				SelectDeviceReloadInterval(DeviceSettings.DefaultDeviceReloadInterval);
				this.deviceReloadIntervalDefaultButton.Enabled = false;
			}
			else
			{
				SelectDeviceReloadInterval(this.deviceReloadInterval);
				this.deviceReloadIntervalDefaultButton.Enabled = true;
			}
		}

		private void SelectDeviceReloadInterval(int interval)
		{
			foreach (DeviceReloadIntervalComboBoxItem item in this.deviceReloadIntervalComboBox.Items)
			{
				if (item.MilliSeconds >= interval)
				{
					this.deviceReloadIntervalComboBox.SelectedItem = item;
					return;
				}
			}
		}

		private void UpdateDeviceDirectoryControls()
		{
			if (this.deviceFilesDirectory == null)
			{
				this.deviceDirectoryBox.Text = DeviceSettings.DefaultDeviceFilesDirectory;
				this.deviceDirectoryDefaultButton.Enabled = false;
			}
			else
			{
				this.deviceDirectoryBox.Text = this.deviceFilesDirectory;
				this.deviceDirectoryDefaultButton.Enabled = true;
			}
		}

		private void UpdateDeviceFileControls(DeviceFileComboBoxItem item)
		{
			if (item == null)
			{
				this.deviceFileBox.Text = "";
				this.deviceFileSetButton.Enabled = false;
				this.deviceFileDefaultButton.Enabled = false;
			}
			else
			{
				this.deviceFileBox.Text = Path.Combine(GetCurrentDeviceFilesDirectory(), item.FilePath);
				this.deviceFileSetButton.Enabled = true;
				this.deviceFileDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void UpdateTickCountControls(TickCountComboBoxItem item)
		{
			if (item == null)
			{
				this.tickCountBox.LongValue = 1L;
				this.tickCountSetButton.Enabled = false;
				this.tickCountDefaultButton.Enabled = false;
			}
			else
			{
				this.tickCountBox.LongValue = item.TickCount;
				this.tickCountSetButton.Enabled = true;
				this.tickCountDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void DeviceReloadIntervalDefaultButton_Click(object sender, EventArgs e)
		{
			this.deviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			UpdateDeviceReloadIntervalControls();
		}

		private void DeviceReloadIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.deviceReloadInterval = ((DeviceReloadIntervalComboBoxItem)this.deviceReloadIntervalComboBox.SelectedItem).MilliSeconds;
			this.deviceReloadIntervalDefaultButton.Enabled = true;
		}

		private void LoadDefaultLoaderCards()
		{
			string[] defaultLoaderCards = CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < this.loaderCardTextBoxes.Length; index++)
			{
				this.loaderCardTextBoxes[index].MixByteCollectionValue.Load(index < defaultLoaderCards.Length ? defaultLoaderCards[index] : string.Empty);
				this.loaderCardTextBoxes[index].Update();
			}

			this.loaderCardsDefaultButton.Enabled = false;
		}

		private void FloatingPointMemoryWordCountDefaultButton_Click(object sender, EventArgs e)
		{
			this.floatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();
		}

		private void FloatingPointMemoryWordCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			this.floatingPointMemoryWordCount = (int)this.floatingPointMemoryWordCountBox.LongValue;
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
