using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Modules.Settings;
using MixLib.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class PreferencesForm : Form
	{
		Button mCancelButton;
		Button mColorDefaultButton;
		ColorDialog mColorDialog;
		ComboBox mColorSelectionBox;
		Button mColorSetButton;
		GroupBox mColorsGroup;
		Configuration mConfiguration;
		string mDefaultDirectory;
		Button mDefaultsButton;
		TextBox mDeviceDirectoryBox;
		Button mDeviceDirectoryDefaultButton;
		Label mDeviceDirectoryLabel;
		Button mDeviceDirectorySetButton;
		TextBox mDeviceFileBox;
		Button mDeviceFileDefaultButton;
		string mDeviceFilesDirectory;
		ComboBox mDeviceFileSelectionBox;
		Button mDeviceFileSetButton;
		FolderBrowserDialog mDeviceFilesFolderBrowserDialog;
		GroupBox mDeviceFilesGroup;
		Devices mDevices;
		GroupBox mDeviceTickCountsGroup;
		Button mOkButton;
		SaveFileDialog mSelectDeviceFileDialog;
		Label mShowColorLabel;
		LongValueTextBox mTickCountBox;
		Button mTickCountDefaultButton;
		ComboBox mTickCountSelectionBox;
		GroupBox mMiscGroupBox;
		Button mDeviceReloadIntervalDefaultButton;
		Label mDeviceReloadIntervalLabel;
		ComboBox mDeviceReloadIntervalComboBox;
		Button mTickCountSetButton;
		GroupBox mLoaderCardsGroupBox;
		Panel mLoaderCardsPanel;
		MixByteCollectionCharTextBox mLoaderCard2TextBox;
		MixByteCollectionCharTextBox mLoaderCard1TextBox;
		MixByteCollectionCharTextBox mLoaderCard3TextBox;
		MixByteCollectionCharTextBox[] mLoaderCardTextBoxes;
		Button mLoaderCardsDefaultButton;
		GroupBox mFloatingPointGroupBox;
		LongValueTextBox mFloatingPointMemoryWordCountBox;
		Label mFloatingPointMemoryWordCountLabel;
		Button mFloatingPointMemoryWordCountDefaultButton;
		int mDeviceReloadInterval;
		CheckBox mColorProfilingCountsCheckBox;
		int? mFloatingPointMemoryWordCount;

		public PreferencesForm(Configuration configuration, Devices devices, string defaultDirectory)
		{
			InitializeComponent();
			mShowColorLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);

			FillDeviceReloadIntervalSelectionBox();

			mConfiguration = configuration;
			mDevices = devices;
			mDefaultDirectory = defaultDirectory;
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			InitializeLoaderCardTextBoxes();

		}

		string GetCurrentDeviceFilesDirectory() => mDeviceFilesDirectory ?? DeviceSettings.DefaultDeviceFilesDirectory;

		public void UpdateLayout() => mTickCountBox.UpdateLayout();

		void MColorSelectionBox_SelectionChangeCommited(object sender, EventArgs e) =>
				UpdateColorControls((ColorComboBoxItem)mColorSelectionBox.SelectedItem);

		void MDeviceFileSelectionBox_SelectionChangeCommitted(object sender, EventArgs e) =>
				UpdateDeviceFileControls((DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);

		void MTickCountSelectionBox_SelectionChangeCommitted(object sender, EventArgs e) =>
				UpdateTickCountControls((TickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);

		void MTickCountSetButton_Click(object sender, EventArgs e) => SetTickCountValue();

		void MLoaderCardsDefaultButton_Click(object sender, EventArgs e) => LoadDefaultLoaderCards();

		void InitializeLoaderCardTextBoxes()
		{
			mLoaderCardTextBoxes = new MixByteCollectionCharTextBox[] { mLoaderCard1TextBox, mLoaderCard2TextBox, mLoaderCard3TextBox };

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = mLoaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.NavigationKeyDown += This_KeyDown;
				loaderCardTextBox.ValueChanged += LoaderCardTextBox_ValueChanged;
			}
		}

		void LoaderCardTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			mLoaderCardsDefaultButton.Enabled = true;
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is MixByteCollectionCharTextBox) || e.Modifiers != Keys.None) return;

			var senderTextBox = (MixByteCollectionCharTextBox)sender;
			MixByteCollectionCharTextBox targetTextBox;
			int index;

			int? caretPos = (e is IndexKeyEventArgs) ? ((IndexKeyEventArgs)e).Index : null;

			switch (e.KeyCode)
			{
				case Keys.Up:
					index = Array.IndexOf(mLoaderCardTextBoxes, senderTextBox);
					if (index > 0)
					{
						targetTextBox = mLoaderCardTextBoxes[index - 1];
						targetTextBox.Focus();
						if (caretPos.HasValue) targetTextBox.Select(caretPos.Value, 0);
					}

					break;

				case Keys.Down:
					index = Array.IndexOf(mLoaderCardTextBoxes, senderTextBox);
					if (index < mLoaderCardTextBoxes.Length - 1)
					{
						targetTextBox = mLoaderCardTextBoxes[index + 1];
						targetTextBox.Focus();
						if (caretPos.HasValue) targetTextBox.Select(caretPos.Value, 0);
					}

					break;
			}
		}

		void FillDeviceReloadIntervalSelectionBox()
		{
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(250));
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(500));
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(750));
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1000));
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(1500));
			mDeviceReloadIntervalComboBox.Items.Add(new DeviceReloadIntervalComboBoxItem(2000));
		}

		void FillColorSelectionBox()
		{
			mColorSelectionBox.Items.Clear();
			string[] knownColorNames = GuiSettings.KnownColorNames;

			var list = new List<ColorComboBoxItem>();
			foreach (string colorName in knownColorNames)
			{
				var comboBoxItem = new ColorComboBoxItem(colorName);
				if (mConfiguration.Colors.ContainsKey(colorName))
				{
					comboBoxItem.Color = mConfiguration.Colors[colorName];
				}
				list.Add(comboBoxItem);
			}

			mColorSelectionBox.Items.AddRange(list.ToArray());
			if (mColorSelectionBox.Items.Count > 0)
			{
				mColorSelectionBox.SelectedIndex = 0;
			}

			UpdateColorControls((ColorComboBoxItem)mColorSelectionBox.SelectedItem);
		}

		void FillDeviceFileSelectionBox()
		{
			mDeviceFileSelectionBox.Items.Clear();
			var list = new List<DeviceFileComboBoxItem>();
			foreach (MixDevice device in mDevices)
			{
				if (!(device is FileBasedDevice)) continue;

				var item = new DeviceFileComboBoxItem((FileBasedDevice)device);

				if (mConfiguration.DeviceFilePaths.ContainsKey(device.Id))
				{
					item.FilePath = mConfiguration.DeviceFilePaths[device.Id];
				}

				list.Add(item);
			}

			mDeviceFileSelectionBox.Items.AddRange(list.ToArray());

			if (mDeviceFileSelectionBox.Items.Count > 0)
			{
				mDeviceFileSelectionBox.SelectedIndex = 0;
			}

			UpdateDeviceFileControls((DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
		}

		void FillTickCountSelectionBox()
		{
			mTickCountSelectionBox.Items.Clear();

			string[] knownTickCountNames = DeviceSettings.KnownTickCountNames;

			var list = new List<TickCountComboBoxItem>();
			foreach (string tickCountName in knownTickCountNames)
			{
				var item = new TickCountComboBoxItem(tickCountName);

				if (mConfiguration.TickCounts.ContainsKey(tickCountName))
				{
					item.TickCount = mConfiguration.TickCounts[tickCountName];
				}

				list.Add(item);
			}

			mTickCountSelectionBox.Items.AddRange(list.ToArray());

			if (mTickCountSelectionBox.Items.Count > 0)
			{
				mTickCountSelectionBox.SelectedIndex = 0;
			}

			UpdateTickCountControls((TickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);
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

		Color GetMatchingColor(string name, bool wantBackground)
		{
			string matchingColorName;
			if (wantBackground)
			{
				if (name.EndsWith("Text", StringComparison.Ordinal))
				{
					matchingColorName = name.Substring(0, name.Length - 4);
				}
				else
				{
					matchingColorName = name;
				}

				if (!GuiSettings.IsKnownColor(matchingColorName))
				{
					matchingColorName = matchingColorName + "Background";
				}
			}
			else if (name.EndsWith("Background", StringComparison.Ordinal))
			{
				matchingColorName = name.Substring(0, name.Length - 10) + "Text";
			}
			else
			{
				matchingColorName = name + "Text";
			}

			if (!GuiSettings.IsKnownColor(matchingColorName))
			{
				matchingColorName = wantBackground ? "EditorBackground" : "RenderedText";
			}

			foreach (ColorComboBoxItem item in mColorSelectionBox.Items)
			{
				if (item.Name == matchingColorName) return item.Color;
			}

			return GuiSettings.GetDefaultColor(name);
		}

		void InitializeComponent()
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
			mColorsGroup = new GroupBox();
			mColorSetButton = new Button();
			mShowColorLabel = new Label();
			mColorSelectionBox = new ComboBox();
			mColorDefaultButton = new Button();
			mDeviceFilesGroup = new GroupBox();
			mDeviceDirectoryBox = new TextBox();
			mDeviceDirectoryLabel = new Label();
			mDeviceDirectorySetButton = new Button();
			mDeviceDirectoryDefaultButton = new Button();
			mDeviceFileSelectionBox = new ComboBox();
			mDeviceFileBox = new TextBox();
			mDeviceFileSetButton = new Button();
			mDeviceFileDefaultButton = new Button();
			mOkButton = new Button();
			mCancelButton = new Button();
			mDeviceTickCountsGroup = new GroupBox();
			mTickCountDefaultButton = new Button();
			mTickCountSetButton = new Button();
			mTickCountSelectionBox = new ComboBox();
			mDefaultsButton = new Button();
			mMiscGroupBox = new GroupBox();
			mDeviceReloadIntervalComboBox = new ComboBox();
			mDeviceReloadIntervalLabel = new Label();
			mDeviceReloadIntervalDefaultButton = new Button();
			mLoaderCardsGroupBox = new GroupBox();
			mLoaderCardsDefaultButton = new Button();
			mLoaderCardsPanel = new Panel();
			mFloatingPointGroupBox = new GroupBox();
			mFloatingPointMemoryWordCountLabel = new Label();
			mFloatingPointMemoryWordCountDefaultButton = new Button();
			mFloatingPointMemoryWordCountBox = new LongValueTextBox();
			mLoaderCard3TextBox = new MixByteCollectionCharTextBox();
			mLoaderCard2TextBox = new MixByteCollectionCharTextBox();
			mLoaderCard1TextBox = new MixByteCollectionCharTextBox();
			mTickCountBox = new LongValueTextBox();
			mColorProfilingCountsCheckBox = new CheckBox();
			mColorsGroup.SuspendLayout();
			mDeviceFilesGroup.SuspendLayout();
			mDeviceTickCountsGroup.SuspendLayout();
			mMiscGroupBox.SuspendLayout();
			mLoaderCardsGroupBox.SuspendLayout();
			mLoaderCardsPanel.SuspendLayout();
			mFloatingPointGroupBox.SuspendLayout();
			SuspendLayout();
			// 
			// mColorsGroup
			// 
			mColorsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mColorsGroup.Controls.Add(mColorProfilingCountsCheckBox);
			mColorsGroup.Controls.Add(mColorSetButton);
			mColorsGroup.Controls.Add(mShowColorLabel);
			mColorsGroup.Controls.Add(mColorSelectionBox);
			mColorsGroup.Controls.Add(mColorDefaultButton);
			mColorsGroup.Location = new Point(12, 12);
			mColorsGroup.Name = "mColorsGroup";
			mColorsGroup.Size = new Size(366, 70);
			mColorsGroup.TabIndex = 1;
			mColorsGroup.TabStop = false;
			mColorsGroup.Text = "Colors";
			// 
			// mColorSetButton
			// 
			mColorSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mColorSetButton.Location = new Point(254, 15);
			mColorSetButton.Name = "mColorSetButton";
			mColorSetButton.Size = new Size(40, 23);
			mColorSetButton.TabIndex = 2;
			mColorSetButton.Text = "&Set...";
			mColorSetButton.Click += MColorSetButton_Click;
			// 
			// mShowColorLabel
			// 
			mShowColorLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mShowColorLabel.BorderStyle = BorderStyle.FixedSingle;
			mShowColorLabel.Location = new Point(198, 16);
			mShowColorLabel.Name = "mShowColorLabel";
			mShowColorLabel.Size = new Size(48, 21);
			mShowColorLabel.TabIndex = 1;
			mShowColorLabel.Text = "Color";
			mShowColorLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// mColorSelectionBox
			// 
			mColorSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mColorSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			mColorSelectionBox.Location = new Point(8, 16);
			mColorSelectionBox.Name = "mColorSelectionBox";
			mColorSelectionBox.Size = new Size(182, 21);
			mColorSelectionBox.Sorted = true;
			mColorSelectionBox.TabIndex = 0;
			mColorSelectionBox.SelectionChangeCommitted += MColorSelectionBox_SelectionChangeCommited;
			// 
			// mColorDefaultButton
			// 
			mColorDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mColorDefaultButton.Location = new Point(302, 15);
			mColorDefaultButton.Name = "mColorDefaultButton";
			mColorDefaultButton.Size = new Size(56, 23);
			mColorDefaultButton.TabIndex = 3;
			mColorDefaultButton.Text = "Default";
			mColorDefaultButton.Click += MColorDefaultButton_Click;
			// 
			// mDeviceFilesGroup
			// 
			mDeviceFilesGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mDeviceFilesGroup.Controls.Add(mDeviceDirectoryBox);
			mDeviceFilesGroup.Controls.Add(mDeviceDirectoryLabel);
			mDeviceFilesGroup.Controls.Add(mDeviceDirectorySetButton);
			mDeviceFilesGroup.Controls.Add(mDeviceDirectoryDefaultButton);
			mDeviceFilesGroup.Controls.Add(mDeviceFileSelectionBox);
			mDeviceFilesGroup.Controls.Add(mDeviceFileBox);
			mDeviceFilesGroup.Controls.Add(mDeviceFileSetButton);
			mDeviceFilesGroup.Controls.Add(mDeviceFileDefaultButton);
			mDeviceFilesGroup.Location = new Point(12, 131);
			mDeviceFilesGroup.Name = "mDeviceFilesGroup";
			mDeviceFilesGroup.Size = new Size(366, 80);
			mDeviceFilesGroup.TabIndex = 3;
			mDeviceFilesGroup.TabStop = false;
			mDeviceFilesGroup.Text = "Device files";
			// 
			// mDeviceDirectoryBox
			// 
			mDeviceDirectoryBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mDeviceDirectoryBox.Location = new Point(100, 16);
			mDeviceDirectoryBox.Name = "mDeviceDirectoryBox";
			mDeviceDirectoryBox.ReadOnly = true;
			mDeviceDirectoryBox.Size = new Size(146, 20);
			mDeviceDirectoryBox.TabIndex = 1;
			// 
			// mDeviceDirectoryLabel
			// 
			mDeviceDirectoryLabel.Location = new Point(6, 16);
			mDeviceDirectoryLabel.Name = "mDeviceDirectoryLabel";
			mDeviceDirectoryLabel.Size = new Size(96, 21);
			mDeviceDirectoryLabel.TabIndex = 0;
			mDeviceDirectoryLabel.Text = "Default directory:";
			mDeviceDirectoryLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mDeviceDirectorySetButton
			// 
			mDeviceDirectorySetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mDeviceDirectorySetButton.Location = new Point(254, 15);
			mDeviceDirectorySetButton.Name = "mDeviceDirectorySetButton";
			mDeviceDirectorySetButton.Size = new Size(40, 23);
			mDeviceDirectorySetButton.TabIndex = 2;
			mDeviceDirectorySetButton.Text = "S&et...";
			mDeviceDirectorySetButton.Click += MDeviceDirectorySetButton_Click;
			// 
			// mDeviceDirectoryDefaultButton
			// 
			mDeviceDirectoryDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mDeviceDirectoryDefaultButton.Location = new Point(302, 15);
			mDeviceDirectoryDefaultButton.Name = "mDeviceDirectoryDefaultButton";
			mDeviceDirectoryDefaultButton.Size = new Size(56, 23);
			mDeviceDirectoryDefaultButton.TabIndex = 3;
			mDeviceDirectoryDefaultButton.Text = "Default";
			mDeviceDirectoryDefaultButton.Click += MDeviceDirectoryDefaultButton_Click;
			// 
			// mDeviceFileSelectionBox
			// 
			mDeviceFileSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			mDeviceFileSelectionBox.Location = new Point(6, 48);
			mDeviceFileSelectionBox.Name = "mDeviceFileSelectionBox";
			mDeviceFileSelectionBox.Size = new Size(88, 21);
			mDeviceFileSelectionBox.Sorted = true;
			mDeviceFileSelectionBox.TabIndex = 4;
			mDeviceFileSelectionBox.SelectionChangeCommitted += MDeviceFileSelectionBox_SelectionChangeCommitted;
			// 
			// mDeviceFileBox
			// 
			mDeviceFileBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mDeviceFileBox.Location = new Point(100, 48);
			mDeviceFileBox.Name = "mDeviceFileBox";
			mDeviceFileBox.ReadOnly = true;
			mDeviceFileBox.Size = new Size(146, 20);
			mDeviceFileBox.TabIndex = 5;
			// 
			// mDeviceFileSetButton
			// 
			mDeviceFileSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mDeviceFileSetButton.Location = new Point(254, 47);
			mDeviceFileSetButton.Name = "mDeviceFileSetButton";
			mDeviceFileSetButton.Size = new Size(40, 23);
			mDeviceFileSetButton.TabIndex = 6;
			mDeviceFileSetButton.Text = "Se&t...";
			mDeviceFileSetButton.Click += MDeviceFileSetButton_Click;
			// 
			// mDeviceFileDefaultButton
			// 
			mDeviceFileDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mDeviceFileDefaultButton.Location = new Point(302, 47);
			mDeviceFileDefaultButton.Name = "mDeviceFileDefaultButton";
			mDeviceFileDefaultButton.Size = new Size(56, 23);
			mDeviceFileDefaultButton.TabIndex = 7;
			mDeviceFileDefaultButton.Text = "Default";
			mDeviceFileDefaultButton.Click += MDeviceFileDefaultButton_Click;
			// 
			// mOkButton
			// 
			mOkButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			mOkButton.DialogResult = DialogResult.OK;
			mOkButton.Location = new Point(224, 382);
			mOkButton.Name = "mOkButton";
			mOkButton.Size = new Size(75, 23);
			mOkButton.TabIndex = 8;
			mOkButton.Text = "&OK";
			mOkButton.Click += MOkButton_Click;
			// 
			// mCancelButton
			// 
			mCancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			mCancelButton.DialogResult = DialogResult.Cancel;
			mCancelButton.Location = new Point(304, 382);
			mCancelButton.Name = "mCancelButton";
			mCancelButton.Size = new Size(75, 23);
			mCancelButton.TabIndex = 9;
			mCancelButton.Text = "&Cancel";
			// 
			// mDeviceTickCountsGroup
			// 
			mDeviceTickCountsGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mDeviceTickCountsGroup.Controls.Add(mTickCountBox);
			mDeviceTickCountsGroup.Controls.Add(mTickCountDefaultButton);
			mDeviceTickCountsGroup.Controls.Add(mTickCountSetButton);
			mDeviceTickCountsGroup.Controls.Add(mTickCountSelectionBox);
			mDeviceTickCountsGroup.Location = new Point(12, 211);
			mDeviceTickCountsGroup.Name = "mDeviceTickCountsGroup";
			mDeviceTickCountsGroup.Size = new Size(366, 48);
			mDeviceTickCountsGroup.TabIndex = 4;
			mDeviceTickCountsGroup.TabStop = false;
			mDeviceTickCountsGroup.Text = "Device tick counts";
			// 
			// mTickCountDefaultButton
			// 
			mTickCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mTickCountDefaultButton.Location = new Point(302, 16);
			mTickCountDefaultButton.Name = "mTickCountDefaultButton";
			mTickCountDefaultButton.Size = new Size(56, 23);
			mTickCountDefaultButton.TabIndex = 3;
			mTickCountDefaultButton.Text = "Default";
			mTickCountDefaultButton.Click += MTickCountDefaultButton_Click;
			// 
			// mTickCountSetButton
			// 
			mTickCountSetButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mTickCountSetButton.Location = new Point(246, 16);
			mTickCountSetButton.Name = "mTickCountSetButton";
			mTickCountSetButton.Size = new Size(48, 23);
			mTickCountSetButton.TabIndex = 2;
			mTickCountSetButton.Text = "&Apply";
			mTickCountSetButton.Click += MTickCountSetButton_Click;
			// 
			// mTickCountSelectionBox
			// 
			mTickCountSelectionBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mTickCountSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
			mTickCountSelectionBox.Location = new Point(6, 16);
			mTickCountSelectionBox.Name = "mTickCountSelectionBox";
			mTickCountSelectionBox.Size = new Size(182, 21);
			mTickCountSelectionBox.Sorted = true;
			mTickCountSelectionBox.TabIndex = 0;
			mTickCountSelectionBox.SelectionChangeCommitted += MTickCountSelectionBox_SelectionChangeCommitted;
			// 
			// mDefaultsButton
			// 
			mDefaultsButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			mDefaultsButton.Location = new Point(144, 382);
			mDefaultsButton.Name = "mDefaultsButton";
			mDefaultsButton.Size = new Size(75, 23);
			mDefaultsButton.TabIndex = 7;
			mDefaultsButton.Text = "&Defaults";
			mDefaultsButton.Click += MDefaultsButton_Click;
			// 
			// mMiscGroupBox
			// 
			mMiscGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mMiscGroupBox.Controls.Add(mDeviceReloadIntervalComboBox);
			mMiscGroupBox.Controls.Add(mDeviceReloadIntervalLabel);
			mMiscGroupBox.Controls.Add(mDeviceReloadIntervalDefaultButton);
			mMiscGroupBox.Location = new Point(12, 327);
			mMiscGroupBox.Name = "mMiscGroupBox";
			mMiscGroupBox.Size = new Size(366, 48);
			mMiscGroupBox.TabIndex = 6;
			mMiscGroupBox.TabStop = false;
			mMiscGroupBox.Text = "Miscellaneous";
			// 
			// mDeviceReloadIntervalComboBox
			// 
			mDeviceReloadIntervalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			mDeviceReloadIntervalComboBox.FormattingEnabled = true;
			mDeviceReloadIntervalComboBox.Location = new Point(156, 15);
			mDeviceReloadIntervalComboBox.Name = "mDeviceReloadIntervalComboBox";
			mDeviceReloadIntervalComboBox.Size = new Size(73, 21);
			mDeviceReloadIntervalComboBox.TabIndex = 5;
			mDeviceReloadIntervalComboBox.SelectedIndexChanged += MDeviceReloadIntervalComboBox_SelectedIndexChanged;
			// 
			// mDeviceReloadIntervalLabel
			// 
			mDeviceReloadIntervalLabel.AutoSize = true;
			mDeviceReloadIntervalLabel.Location = new Point(6, 18);
			mDeviceReloadIntervalLabel.Name = "mDeviceReloadIntervalLabel";
			mDeviceReloadIntervalLabel.Size = new Size(145, 13);
			mDeviceReloadIntervalLabel.TabIndex = 4;
			mDeviceReloadIntervalLabel.Text = "Device editor reload interval: ";
			// 
			// mDeviceReloadIntervalDefaultButton
			// 
			mDeviceReloadIntervalDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mDeviceReloadIntervalDefaultButton.Location = new Point(302, 15);
			mDeviceReloadIntervalDefaultButton.Name = "mDeviceReloadIntervalDefaultButton";
			mDeviceReloadIntervalDefaultButton.Size = new Size(56, 23);
			mDeviceReloadIntervalDefaultButton.TabIndex = 3;
			mDeviceReloadIntervalDefaultButton.Text = "Default";
			mDeviceReloadIntervalDefaultButton.Click += MDeviceReloadIntervalDefaultButton_Click;
			// 
			// mLoaderCardsGroupBox
			// 
			mLoaderCardsGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mLoaderCardsGroupBox.Controls.Add(mLoaderCardsDefaultButton);
			mLoaderCardsGroupBox.Controls.Add(mLoaderCardsPanel);
			mLoaderCardsGroupBox.Location = new Point(12, 259);
			mLoaderCardsGroupBox.Name = "mLoaderCardsGroupBox";
			mLoaderCardsGroupBox.Size = new Size(366, 68);
			mLoaderCardsGroupBox.TabIndex = 5;
			mLoaderCardsGroupBox.TabStop = false;
			mLoaderCardsGroupBox.Text = "Loader instruction cards";
			// 
			// mLoaderCardsDefaultButton
			// 
			mLoaderCardsDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mLoaderCardsDefaultButton.Location = new Point(302, 16);
			mLoaderCardsDefaultButton.Name = "mLoaderCardsDefaultButton";
			mLoaderCardsDefaultButton.Size = new Size(56, 23);
			mLoaderCardsDefaultButton.TabIndex = 4;
			mLoaderCardsDefaultButton.Text = "Default";
			mLoaderCardsDefaultButton.Click += MLoaderCardsDefaultButton_Click;
			// 
			// mLoaderCardsPanel
			// 
			mLoaderCardsPanel.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mLoaderCardsPanel.BorderStyle = BorderStyle.FixedSingle;
			mLoaderCardsPanel.Controls.Add(mLoaderCard3TextBox);
			mLoaderCardsPanel.Controls.Add(mLoaderCard2TextBox);
			mLoaderCardsPanel.Controls.Add(mLoaderCard1TextBox);
			mLoaderCardsPanel.Location = new Point(8, 16);
			mLoaderCardsPanel.Name = "mLoaderCardsPanel";
			mLoaderCardsPanel.Size = new Size(286, 44);
			mLoaderCardsPanel.TabIndex = 0;
			// 
			// mFloatingPointGroupBox
			// 
			mFloatingPointGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mFloatingPointGroupBox.Controls.Add(mFloatingPointMemoryWordCountBox);
			mFloatingPointGroupBox.Controls.Add(mFloatingPointMemoryWordCountLabel);
			mFloatingPointGroupBox.Controls.Add(mFloatingPointMemoryWordCountDefaultButton);
			mFloatingPointGroupBox.Location = new Point(12, 83);
			mFloatingPointGroupBox.Name = "mFloatingPointGroupBox";
			mFloatingPointGroupBox.Size = new Size(366, 48);
			mFloatingPointGroupBox.TabIndex = 2;
			mFloatingPointGroupBox.TabStop = false;
			mFloatingPointGroupBox.Text = "Floating point module";
			// 
			// mFloatingPointMemoryWordCountLabel
			// 
			mFloatingPointMemoryWordCountLabel.AutoSize = true;
			mFloatingPointMemoryWordCountLabel.Location = new Point(6, 18);
			mFloatingPointMemoryWordCountLabel.Name = "mFloatingPointMemoryWordCountLabel";
			mFloatingPointMemoryWordCountLabel.Size = new Size(181, 13);
			mFloatingPointMemoryWordCountLabel.TabIndex = 0;
			mFloatingPointMemoryWordCountLabel.Text = "Memory word count (requires restart):";
			// 
			// mFloatingPointMemoryWordCountDefaultButton
			// 
			mFloatingPointMemoryWordCountDefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mFloatingPointMemoryWordCountDefaultButton.Location = new Point(302, 15);
			mFloatingPointMemoryWordCountDefaultButton.Name = "mFloatingPointMemoryWordCountDefaultButton";
			mFloatingPointMemoryWordCountDefaultButton.Size = new Size(56, 23);
			mFloatingPointMemoryWordCountDefaultButton.TabIndex = 2;
			mFloatingPointMemoryWordCountDefaultButton.Text = "Default";
			mFloatingPointMemoryWordCountDefaultButton.Click += MFloatingPointMemoryWordCountDefaultButton_Click;
			// 
			// mFloatingPointMemoryWordCountBox
			// 
			mFloatingPointMemoryWordCountBox.BackColor = Color.White;
			mFloatingPointMemoryWordCountBox.BorderStyle = BorderStyle.FixedSingle;
			mFloatingPointMemoryWordCountBox.ClearZero = true;
			mFloatingPointMemoryWordCountBox.ForeColor = Color.Black;
			mFloatingPointMemoryWordCountBox.Location = new Point(199, 16);
			mFloatingPointMemoryWordCountBox.LongValue = 200;
			mFloatingPointMemoryWordCountBox.Magnitude = 200;
			mFloatingPointMemoryWordCountBox.MaxLength = 5;
			mFloatingPointMemoryWordCountBox.MaxValue = 99999;
			mFloatingPointMemoryWordCountBox.MinValue = 1;
			mFloatingPointMemoryWordCountBox.Name = "mFloatingPointMemoryWordCountBox";
			mFloatingPointMemoryWordCountBox.Sign = Word.Signs.Positive;
			mFloatingPointMemoryWordCountBox.Size = new Size(40, 20);
			mFloatingPointMemoryWordCountBox.SupportNegativeZero = false;
			mFloatingPointMemoryWordCountBox.TabIndex = 1;
			mFloatingPointMemoryWordCountBox.Text = "200";
			mFloatingPointMemoryWordCountBox.ValueChanged += MFloatingPointMemoryWordCountBox_ValueChanged;
			// 
			// mLoaderCard3TextBox
			// 
			mLoaderCard3TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mLoaderCard3TextBox.BackColor = Color.White;
			mLoaderCard3TextBox.BorderStyle = BorderStyle.None;
			mLoaderCard3TextBox.CharacterCasing = CharacterCasing.Upper;
			mLoaderCard3TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			mLoaderCard3TextBox.ForeColor = Color.Blue;
			mLoaderCard3TextBox.Location = new Point(0, 28);
			mLoaderCard3TextBox.Margin = new Padding(0);
			fullWord1.LongValue = 0;
			fullWord1.Magnitude = new MixByte[] {
				mixByte1,
				mixByte2,
				mixByte3,
				mixByte4,
				mixByte5};
			fullWord1.MagnitudeLongValue = 0;
			fullWord1.Sign = Word.Signs.Positive;
			mLoaderCard3TextBox.MixByteCollectionValue = fullWord1;
			mLoaderCard3TextBox.Name = "mLoaderCard3TextBox";
			mLoaderCard3TextBox.Size = new Size(284, 14);
			mLoaderCard3TextBox.TabIndex = 2;
			mLoaderCard3TextBox.Text = "     ";
			mLoaderCard3TextBox.UseEditMode = true;
			// 
			// mLoaderCard2TextBox
			// 
			mLoaderCard2TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mLoaderCard2TextBox.BackColor = Color.White;
			mLoaderCard2TextBox.BorderStyle = BorderStyle.None;
			mLoaderCard2TextBox.CharacterCasing = CharacterCasing.Upper;
			mLoaderCard2TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			mLoaderCard2TextBox.ForeColor = Color.Blue;
			mLoaderCard2TextBox.Location = new Point(0, 14);
			mLoaderCard2TextBox.Margin = new Padding(0);
			fullWord2.LongValue = 0;
			fullWord2.Magnitude = new MixByte[] {
				mixByte6,
				mixByte7,
				mixByte8,
				mixByte9,
				mixByte10};
			fullWord2.MagnitudeLongValue = 0;
			fullWord2.Sign = Word.Signs.Positive;
			mLoaderCard2TextBox.MixByteCollectionValue = fullWord2;
			mLoaderCard2TextBox.Name = "mLoaderCard2TextBox";
			mLoaderCard2TextBox.Size = new Size(284, 14);
			mLoaderCard2TextBox.TabIndex = 1;
			mLoaderCard2TextBox.Text = "     ";
			mLoaderCard2TextBox.UseEditMode = true;
			// 
			// mLoaderCard1TextBox
			// 
			mLoaderCard1TextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			mLoaderCard1TextBox.BackColor = Color.White;
			mLoaderCard1TextBox.BorderStyle = BorderStyle.None;
			mLoaderCard1TextBox.CharacterCasing = CharacterCasing.Upper;
			mLoaderCard1TextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			mLoaderCard1TextBox.ForeColor = Color.Blue;
			mLoaderCard1TextBox.Location = new Point(0, 0);
			mLoaderCard1TextBox.Margin = new Padding(0);
			fullWord3.LongValue = 0;
			fullWord3.Magnitude = new MixByte[] {
				mixByte11,
				mixByte12,
				mixByte13,
				mixByte14,
				mixByte15};
			fullWord3.MagnitudeLongValue = 0;
			fullWord3.Sign = Word.Signs.Positive;
			mLoaderCard1TextBox.MixByteCollectionValue = fullWord3;
			mLoaderCard1TextBox.Name = "mLoaderCard1TextBox";
			mLoaderCard1TextBox.Size = new Size(284, 14);
			mLoaderCard1TextBox.TabIndex = 0;
			mLoaderCard1TextBox.Text = "     ";
			mLoaderCard1TextBox.UseEditMode = true;
			// 
			// mTickCountBox
			// 
			mTickCountBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			mTickCountBox.BackColor = Color.White;
			mTickCountBox.BorderStyle = BorderStyle.FixedSingle;
			mTickCountBox.ClearZero = true;
			mTickCountBox.ForeColor = Color.Black;
			mTickCountBox.Location = new Point(198, 17);
			mTickCountBox.LongValue = 1;
			mTickCountBox.Magnitude = 1;
			mTickCountBox.MaxLength = 5;
			mTickCountBox.MaxValue = 99999;
			mTickCountBox.MinValue = 1;
			mTickCountBox.Name = "mTickCountBox";
			mTickCountBox.Sign = Word.Signs.Positive;
			mTickCountBox.Size = new Size(40, 20);
			mTickCountBox.SupportNegativeZero = false;
			mTickCountBox.TabIndex = 1;
			mTickCountBox.Text = "1";
			mTickCountBox.ValueChanged += MTickCountBox_ValueChanged;
			// 
			// mColorProfilingCountsCheckBox
			// 
			mColorProfilingCountsCheckBox.AutoSize = true;
			mColorProfilingCountsCheckBox.Checked = true;
			mColorProfilingCountsCheckBox.CheckState = CheckState.Checked;
			mColorProfilingCountsCheckBox.Location = new Point(8, 43);
			mColorProfilingCountsCheckBox.Name = "mColorProfilingCountsCheckBox";
			mColorProfilingCountsCheckBox.Size = new Size(119, 17);
			mColorProfilingCountsCheckBox.TabIndex = 4;
			mColorProfilingCountsCheckBox.Text = "Color profiler counts";
			mColorProfilingCountsCheckBox.UseVisualStyleBackColor = true;
			// 
			// PreferencesForm
			// 
			ClientSize = new Size(384, 412);
			Controls.Add(mFloatingPointGroupBox);
			Controls.Add(mLoaderCardsGroupBox);
			Controls.Add(mMiscGroupBox);
			Controls.Add(mDefaultsButton);
			Controls.Add(mDeviceTickCountsGroup);
			Controls.Add(mOkButton);
			Controls.Add(mDeviceFilesGroup);
			Controls.Add(mColorsGroup);
			Controls.Add(mCancelButton);
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
			mColorsGroup.ResumeLayout(false);
			mColorsGroup.PerformLayout();
			mDeviceFilesGroup.ResumeLayout(false);
			mDeviceFilesGroup.PerformLayout();
			mDeviceTickCountsGroup.ResumeLayout(false);
			mDeviceTickCountsGroup.PerformLayout();
			mMiscGroupBox.ResumeLayout(false);
			mMiscGroupBox.PerformLayout();
			mLoaderCardsGroupBox.ResumeLayout(false);
			mLoaderCardsPanel.ResumeLayout(false);
			mLoaderCardsPanel.PerformLayout();
			mFloatingPointGroupBox.ResumeLayout(false);
			mFloatingPointGroupBox.PerformLayout();
			ResumeLayout(false);

		}

		void MColorDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (ColorComboBoxItem)mColorSelectionBox.SelectedItem;

			selectedItem.ResetDefault();
			UpdateColorControls(selectedItem);
			mColorSelectionBox.Focus();
		}

		void MColorSetButton_Click(object sender, EventArgs e)
		{
			if (mColorDialog == null)
			{
				mColorDialog = new ColorDialog
				{
					AnyColor = true,
					SolidColorOnly = true,
					AllowFullOpen = true
				};
			}

			var selectedItem = (ColorComboBoxItem)mColorSelectionBox.SelectedItem;
			mColorDialog.CustomColors = new int[] { selectedItem.Color.B << 16 | selectedItem.Color.G << 8 | selectedItem.Color.R };
			mColorDialog.Color = selectedItem.Color;

			if (mColorDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.Color = mColorDialog.Color;
				UpdateColorControls(selectedItem);
			}
		}

		void MDefaultsButton_Click(object sender, EventArgs e)
		{
			foreach (ColorComboBoxItem colorItem in mColorSelectionBox.Items) colorItem.ResetDefault();

			mColorProfilingCountsCheckBox.Checked = true;

			foreach (DeviceFileComboBoxItem deviceFileItem in mDeviceFileSelectionBox.Items) deviceFileItem.ResetDefault();

			foreach (TickCountComboBoxItem tickCountItem in mTickCountSelectionBox.Items) tickCountItem.ResetDefault();

			mDeviceFilesDirectory = null;
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			UpdateColorControls((ColorComboBoxItem)mColorSelectionBox.SelectedItem);
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			UpdateTickCountControls((TickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);
			UpdateDeviceReloadIntervalControls();
			mFloatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();

			LoadDefaultLoaderCards();
		}

		void UpdateFloatingPointControls()
		{
			mFloatingPointMemoryWordCountBox.LongValue = mFloatingPointMemoryWordCount ?? ModuleSettings.FloatingPointMemoryWordCountDefault;
			mFloatingPointMemoryWordCountDefaultButton.Enabled = mFloatingPointMemoryWordCount != null;
		}

		void MDeviceDirectoryDefaultButton_Click(object sender, EventArgs e)
		{
			mDeviceFilesDirectory = null;
			UpdateDeviceDirectoryControls();
			UpdateDeviceFileControls((DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			mDeviceDirectoryBox.Focus();
		}

		void MDeviceDirectorySetButton_Click(object sender, EventArgs e)
		{
			if (mDeviceFilesFolderBrowserDialog == null)
			{
				mDeviceFilesFolderBrowserDialog = new FolderBrowserDialog
				{
					Description = "Please select the default directory for device files."
				};
			}

			mDeviceFilesFolderBrowserDialog.SelectedPath = GetCurrentDeviceFilesDirectory();

			if (mDeviceFilesFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				mDeviceFilesDirectory = mDeviceFilesFolderBrowserDialog.SelectedPath;
				UpdateDeviceDirectoryControls();
				UpdateDeviceFileControls((DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			}
		}

		void MDeviceFileDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateDeviceFileControls(selectedItem);
			mDeviceFileSelectionBox.Focus();
		}

		void MDeviceFileSetButton_Click(object sender, EventArgs e)
		{
			if (mSelectDeviceFileDialog == null)
			{
				mSelectDeviceFileDialog = new SaveFileDialog
				{
					CreatePrompt = true,
					DefaultExt = "mixdev",
					Filter = "MIX device files|*.mixdev|All files|*.*",
					OverwritePrompt = false
				};
			}

			var selectedItem = (DeviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem;
			mSelectDeviceFileDialog.FileName = Path.Combine(GetCurrentDeviceFilesDirectory(), selectedItem.FilePath);

			if (mSelectDeviceFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.FilePath = mSelectDeviceFileDialog.FileName;
				UpdateDeviceFileControls(selectedItem);
			}
		}

		void MOkButton_Click(object sender, EventArgs e)
		{
			mConfiguration.Colors.Clear();
			foreach (ColorComboBoxItem colorItem in mColorSelectionBox.Items)
			{
				if (!colorItem.IsDefault) mConfiguration.Colors.Add(colorItem.Name, colorItem.Color);
			}

			mConfiguration.ColorProfilingCounts = mColorProfilingCountsCheckBox.Checked;

			mConfiguration.DeviceFilePaths.Clear();
			foreach (DeviceFileComboBoxItem deviceFileItem in mDeviceFileSelectionBox.Items)
			{
				if (!deviceFileItem.IsDefault)
				{
					mConfiguration.DeviceFilePaths.Add(deviceFileItem.Id, deviceFileItem.FilePath);
				}
			}

			mConfiguration.TickCounts.Clear();
			foreach (TickCountComboBoxItem tickCountItem in mTickCountSelectionBox.Items)
			{
				if (!tickCountItem.IsDefault)
				{
					mConfiguration.TickCounts.Add(tickCountItem.Name, tickCountItem.TickCount);
				}
			}

			mConfiguration.DeviceFilesDirectory = mDeviceFilesDirectory;
			mConfiguration.DeviceReloadInterval = mDeviceReloadInterval;

			mConfiguration.FloatingPointMemoryWordCount = mFloatingPointMemoryWordCount;

			mConfiguration.LoaderCards = mLoaderCardsDefaultButton.Enabled ? mLoaderCardTextBoxes.Select(box => box.MixByteCollectionValue.ToString(true)).ToArray() : null;

			try
			{
				mConfiguration.Save(mDefaultDirectory);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, "Unable to save preferences: " + exception.Message, "Error saving preferences", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				DialogResult = DialogResult.None;
			}

		}

		void MTickCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args) => SetTickCountValue();

		void MTickCountDefaultButton_Click(object sender, EventArgs e)
		{
			var selectedItem = (TickCountComboBoxItem)mTickCountSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			UpdateTickCountControls(selectedItem);
			mTickCountSelectionBox.Focus();
		}

		void SetTickCountValue()
		{
			var selectedItem = (TickCountComboBoxItem)mTickCountSelectionBox.SelectedItem;
			selectedItem.TickCount = (int)mTickCountBox.LongValue;
			UpdateTickCountControls(selectedItem);
		}

		void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape && !(GetFocusedControl() is IEscapeConsumer))
			{
				DialogResult = DialogResult.Cancel;
				Hide();
			}
		}

		void This_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				FillColorSelectionBox();
				mColorProfilingCountsCheckBox.Checked = mConfiguration.ColorProfilingCounts;
				mDeviceFilesDirectory = mConfiguration.DeviceFilesDirectory;
				UpdateDeviceDirectoryControls();
				FillDeviceFileSelectionBox();
				FillTickCountSelectionBox();
				mDeviceReloadInterval = mConfiguration.DeviceReloadInterval;
				UpdateDeviceReloadIntervalControls();
				mFloatingPointMemoryWordCount = mConfiguration.FloatingPointMemoryWordCount;
				UpdateFloatingPointControls();
				LoadLoaderCards();
			}
		}

		void LoadLoaderCards()
		{
			string[] loaderCards = mConfiguration.LoaderCards ?? CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = mLoaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.MixByteCollectionValue.Load(index < loaderCards.Length ? loaderCards[index] : string.Empty);
				loaderCardTextBox.Update();
			}

			mLoaderCardsDefaultButton.Enabled = mConfiguration.LoaderCards != null;
		}

		void UpdateColorControls(ColorComboBoxItem item)
		{
			if (item == null)
			{
				mShowColorLabel.ForeColor = Color.Black;
				mShowColorLabel.BackColor = Color.White;

				mColorSetButton.Enabled = false;
				mColorDefaultButton.Enabled = false;
			}
			else
			{
				if (item.IsBackground)
				{
					mShowColorLabel.ForeColor = GetMatchingColor(item.Name, false);
					mShowColorLabel.BackColor = item.Color;
				}
				else
				{
					mShowColorLabel.ForeColor = item.Color;
					mShowColorLabel.BackColor = GetMatchingColor(item.Name, true);
				}

				mColorSetButton.Enabled = true;
				mColorDefaultButton.Enabled = !item.IsDefault;
			}
		}

		void UpdateDeviceReloadIntervalControls()
		{
			if (mDeviceReloadInterval == DeviceSettings.UnsetDeviceReloadInterval)
			{
				SelectDeviceReloadInterval(DeviceSettings.DefaultDeviceReloadInterval);
				mDeviceReloadIntervalDefaultButton.Enabled = false;
			}
			else
			{
				SelectDeviceReloadInterval(mDeviceReloadInterval);
				mDeviceReloadIntervalDefaultButton.Enabled = true;
			}
		}

		void SelectDeviceReloadInterval(int interval)
		{
			foreach (DeviceReloadIntervalComboBoxItem item in mDeviceReloadIntervalComboBox.Items)
			{
				if (item.MilliSeconds >= interval)
				{
					mDeviceReloadIntervalComboBox.SelectedItem = item;
					return;
				}
			}
		}

		void UpdateDeviceDirectoryControls()
		{
			if (mDeviceFilesDirectory == null)
			{
				mDeviceDirectoryBox.Text = DeviceSettings.DefaultDeviceFilesDirectory;
				mDeviceDirectoryDefaultButton.Enabled = false;
			}
			else
			{
				mDeviceDirectoryBox.Text = mDeviceFilesDirectory;
				mDeviceDirectoryDefaultButton.Enabled = true;
			}
		}

		void UpdateDeviceFileControls(DeviceFileComboBoxItem item)
		{
			if (item == null)
			{
				mDeviceFileBox.Text = "";
				mDeviceFileSetButton.Enabled = false;
				mDeviceFileDefaultButton.Enabled = false;
			}
			else
			{
				mDeviceFileBox.Text = Path.Combine(GetCurrentDeviceFilesDirectory(), item.FilePath);
				mDeviceFileSetButton.Enabled = true;
				mDeviceFileDefaultButton.Enabled = !item.IsDefault;
			}
		}

		void UpdateTickCountControls(TickCountComboBoxItem item)
		{
			if (item == null)
			{
				mTickCountBox.LongValue = 1L;
				mTickCountSetButton.Enabled = false;
				mTickCountDefaultButton.Enabled = false;
			}
			else
			{
				mTickCountBox.LongValue = item.TickCount;
				mTickCountSetButton.Enabled = true;
				mTickCountDefaultButton.Enabled = !item.IsDefault;
			}
		}

		void MDeviceReloadIntervalDefaultButton_Click(object sender, EventArgs e)
		{
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			UpdateDeviceReloadIntervalControls();
		}

		void MDeviceReloadIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			mDeviceReloadInterval = ((DeviceReloadIntervalComboBoxItem)mDeviceReloadIntervalComboBox.SelectedItem).MilliSeconds;
			mDeviceReloadIntervalDefaultButton.Enabled = true;
		}

		void LoadDefaultLoaderCards()
		{
			string[] defaultLoaderCards = CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				mLoaderCardTextBoxes[index].MixByteCollectionValue.Load(index < defaultLoaderCards.Length ? defaultLoaderCards[index] : string.Empty);
				mLoaderCardTextBoxes[index].Update();
			}

			mLoaderCardsDefaultButton.Enabled = false;
		}

		void MFloatingPointMemoryWordCountDefaultButton_Click(object sender, EventArgs e)
		{
			mFloatingPointMemoryWordCount = null;
			UpdateFloatingPointControls();
		}

		void MFloatingPointMemoryWordCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			mFloatingPointMemoryWordCount = (int)mFloatingPointMemoryWordCountBox.LongValue;
			UpdateFloatingPointControls();
		}

		/// <summary>
		/// Instances of this class are used to store the color settings 
		/// </summary>
		class ColorComboBoxItem
		{
			string mDisplayName;
			public bool IsDefault { get; private set; }
			public string Name { get; private set; }
			Color mSetColor;

			public ColorComboBoxItem(string name)
			{
				Name = name;
				mDisplayName = GetDisplayName(name);
				IsDefault = true;
			}

			public bool IsBackground => !Name.EndsWith("Text", StringComparison.Ordinal);

			public override string ToString() => mDisplayName;

			static string GetDisplayName(string name)
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
					{
						builder.Append(name[i]);
					}
				}

				return builder.ToString();
			}

			public void ResetDefault()
			{
				IsDefault = true;
			}

			public Color Color
			{
				get
				{
					return !IsDefault ? mSetColor : GuiSettings.GetDefaultColor(Name);
				}
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
		class DeviceFileComboBoxItem
		{
			readonly FileBasedDevice mDevice;
			string mFilePath;

			public DeviceFileComboBoxItem(FileBasedDevice device)
			{
				mDevice = device;
			}

			public int Id => mDevice.Id;

			public bool IsDefault => mFilePath == null;

			public override string ToString() => mDevice.Id.ToString("D2") + ": " + mDevice.ShortName;

			public void ResetDefault()
			{
				mFilePath = null;
			}

			public string FilePath
			{
				get
				{
					return mFilePath ?? mDevice.DefaultFileName;
				}
				set
				{
					mFilePath = value;
				}
			}
		}

		/// <summary>
		/// Instances of this class are used to store the tick count settings 
		/// </summary>
		class TickCountComboBoxItem
		{
			string mDisplayName;
			bool mIsDefault;
			readonly string mName;
			int mSetTickCount;

			public TickCountComboBoxItem(string name)
			{
				mName = name;
				mDisplayName = GetDisplayName(name);
				mIsDefault = true;
			}

			public bool IsDefault => mIsDefault;

			public string Name => mName;

			public override string ToString() => mDisplayName;

			static string GetDisplayName(string name)
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
					{
						builder.Append(name[i]);
					}
				}

				return builder.ToString();
			}

			public void ResetDefault()
			{
				mIsDefault = true;
			}

			public int TickCount
			{
				get
				{
					return !mIsDefault ? mSetTickCount : DeviceSettings.GetDefaultTickCount(mName);
				}
				set
				{
					mSetTickCount = value;
					mIsDefault = false;
				}
			}
		}

		class DeviceReloadIntervalComboBoxItem
		{
			public int MilliSeconds { get; set; }

			public DeviceReloadIntervalComboBoxItem(int milliSeconds)
			{
				MilliSeconds = milliSeconds;
			}

			public override string ToString()
			{
				if (MilliSeconds < 1000) return MilliSeconds.ToString("##0 ms");

				float seconds = MilliSeconds / 1000f;
				return seconds.ToString("0.## s");
			}
		}
	}
}
