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
		private Container components = null;
		private Button mCancelButton;
		private Button mColorDefaultButton;
		private ColorDialog mColorDialog;
		private ComboBox mColorSelectionBox;
		private Button mColorSetButton;
		private GroupBox mColorsGroup;
		private Configuration mConfiguration;
		private string mDefaultDirectory;
		private Button mDefaultsButton;
		private TextBox mDeviceDirectoryBox;
		private Button mDeviceDirectoryDefaultButton;
		private Label mDeviceDirectoryLabel;
		private Button mDeviceDirectorySetButton;
		private TextBox mDeviceFileBox;
		private Button mDeviceFileDefaultButton;
		private string mDeviceFilesDirectory;
		private ComboBox mDeviceFileSelectionBox;
		private Button mDeviceFileSetButton;
		private FolderBrowserDialog mDeviceFilesFolderBrowserDialog;
		private GroupBox mDeviceFilesGroup;
		private Devices mDevices;
		private GroupBox mDeviceTickCountsGroup;
		private Button mOkButton;
		private SaveFileDialog mSelectDeviceFileDialog;
		private Label mShowColorLabel;
		private LongValueTextBox mTickCountBox;
		private Button mTickCountDefaultButton;
		private ComboBox mTickCountSelectionBox;
		private GroupBox mMiscGroupBox;
		private Button mDeviceReloadIntervalDefaultButton;
		private Label mDeviceReloadIntervalLabel;
		private ComboBox mDeviceReloadIntervalComboBox;
		private Button mTickCountSetButton;
		private GroupBox mLoaderCardsGroupBox;
		private Panel mLoaderCardsPanel;
		private MixByteCollectionCharTextBox mLoaderCard2TextBox;
		private MixByteCollectionCharTextBox mLoaderCard1TextBox;
		private MixByteCollectionCharTextBox mLoaderCard3TextBox;
		private MixByteCollectionCharTextBox[] mLoaderCardTextBoxes;
		private Button mLoaderCardsDefaultButton;
		private GroupBox mFloatingPointGroupBox;
		private LongValueTextBox mFloatingPointMemoryWordCountBox;
		private Label mFloatingPointMemoryWordCountLabel;
		private Button mFloatingPointMemoryWordCountDefaultButton;
		private int mDeviceReloadInterval;
		private CheckBox mColorProfilingCountsCheckBox;
		private int? mFloatingPointMemoryWordCount;

		public PreferencesForm(Configuration configuration, Devices devices, string defaultDirectory)
		{
			InitializeComponent();
			mShowColorLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);

			fillDeviceReloadIntervalSelectionBox();

			mConfiguration = configuration;
			mDevices = devices;
			mDefaultDirectory = defaultDirectory;
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			initializeLoaderCardTextBoxes();

		}

		private void initializeLoaderCardTextBoxes()
		{
			mLoaderCardTextBoxes = new MixByteCollectionCharTextBox[] { mLoaderCard1TextBox, mLoaderCard2TextBox, mLoaderCard3TextBox };

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = mLoaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.NavigationKeyDown += keyDown;
				loaderCardTextBox.ValueChanged += loaderCardTextBox_ValueChanged;
			}
		}

		void loaderCardTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			mLoaderCardsDefaultButton.Enabled = true;
		}

		void keyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is MixByteCollectionCharTextBox) || e.Modifiers != Keys.None)
			{
				return;
			}

			MixByteCollectionCharTextBox senderTextBox = (MixByteCollectionCharTextBox)sender;
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
						if (caretPos.HasValue)
						{
							targetTextBox.Select(caretPos.Value, 0);
						}
					}

					break;

				case Keys.Down:
					index = Array.IndexOf(mLoaderCardTextBoxes, senderTextBox);
					if (index < mLoaderCardTextBoxes.Length - 1)
					{
						targetTextBox = mLoaderCardTextBoxes[index + 1];
						targetTextBox.Focus();
						if (caretPos.HasValue)
						{
							targetTextBox.Select(caretPos.Value, 0);
						}
					}

					break;
			}
		}

		private void fillDeviceReloadIntervalSelectionBox()
		{
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(250));
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(500));
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(750));
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(1000));
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(1500));
			mDeviceReloadIntervalComboBox.Items.Add(new deviceReloadIntervalComboBoxItem(2000));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void fillColorSelectionBox()
		{
			mColorSelectionBox.Items.Clear();
			string[] knownColorNames = GuiSettings.KnownColorNames;

			List<colorComboBoxItem> list = new List<colorComboBoxItem>();
			foreach (string colorName in knownColorNames)
			{
				colorComboBoxItem comboBoxItem = new colorComboBoxItem(colorName);
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

			updateColorControls((colorComboBoxItem)mColorSelectionBox.SelectedItem);
		}

		private void fillDeviceFileSelectionBox()
		{
			mDeviceFileSelectionBox.Items.Clear();
			List<deviceFileComboBoxItem> list = new List<deviceFileComboBoxItem>();
			foreach (MixDevice device in mDevices)
			{
				if (!(device is FileBasedDevice))
				{
					continue;
				}

				deviceFileComboBoxItem item = new deviceFileComboBoxItem((FileBasedDevice)device);

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

			updateDeviceFileControls((deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
		}

		private void fillTickCountSelectionBox()
		{
			mTickCountSelectionBox.Items.Clear();

			string[] knownTickCountNames = DeviceSettings.KnownTickCountNames;

			List<tickCountComboBoxItem> list = new List<tickCountComboBoxItem>();
			foreach (string tickCountName in knownTickCountNames)
			{
				tickCountComboBoxItem item = new tickCountComboBoxItem(tickCountName);

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

			updateTickCountControls((tickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);
		}

		private string getCurrentDeviceFilesDirectory()
		{
			return mDeviceFilesDirectory != null ? mDeviceFilesDirectory : DeviceSettings.DefaultDeviceFilesDirectory;
		}

		public Control getFocusedControl()
		{
			ContainerControl container = (ContainerControl)this;
			Control control = null;

			while (container != null)
			{
				control = container.ActiveControl;
				container = control as ContainerControl;
			}

			return control;
		}

		private Color getMatchingColor(string name, bool wantBackground)
		{
			string matchingColorName;
			if (wantBackground)
			{
				if (name.EndsWith("Text"))
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
			else if (name.EndsWith("Background"))
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

			foreach (colorComboBoxItem item in mColorSelectionBox.Items)
			{
				if (item.Name == matchingColorName)
				{
					return item.Color;
				}
			}

			return GuiSettings.GetDefaultColor(name);
		}

		private void InitializeComponent()
		{
			MixLib.Type.FullWord fullWord1 = new MixLib.Type.FullWord();
			MixLib.Type.MixByte mixByte1 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte2 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte3 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte4 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte5 = new MixLib.Type.MixByte();
			MixLib.Type.FullWord fullWord2 = new MixLib.Type.FullWord();
			MixLib.Type.MixByte mixByte6 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte7 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte8 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte9 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte10 = new MixLib.Type.MixByte();
			MixLib.Type.FullWord fullWord3 = new MixLib.Type.FullWord();
			MixLib.Type.MixByte mixByte11 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte12 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte13 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte14 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte15 = new MixLib.Type.MixByte();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
			this.mColorsGroup = new System.Windows.Forms.GroupBox();
			this.mColorSetButton = new System.Windows.Forms.Button();
			this.mShowColorLabel = new System.Windows.Forms.Label();
			this.mColorSelectionBox = new System.Windows.Forms.ComboBox();
			this.mColorDefaultButton = new System.Windows.Forms.Button();
			this.mDeviceFilesGroup = new System.Windows.Forms.GroupBox();
			this.mDeviceDirectoryBox = new System.Windows.Forms.TextBox();
			this.mDeviceDirectoryLabel = new System.Windows.Forms.Label();
			this.mDeviceDirectorySetButton = new System.Windows.Forms.Button();
			this.mDeviceDirectoryDefaultButton = new System.Windows.Forms.Button();
			this.mDeviceFileSelectionBox = new System.Windows.Forms.ComboBox();
			this.mDeviceFileBox = new System.Windows.Forms.TextBox();
			this.mDeviceFileSetButton = new System.Windows.Forms.Button();
			this.mDeviceFileDefaultButton = new System.Windows.Forms.Button();
			this.mOkButton = new System.Windows.Forms.Button();
			this.mCancelButton = new System.Windows.Forms.Button();
			this.mDeviceTickCountsGroup = new System.Windows.Forms.GroupBox();
			this.mTickCountDefaultButton = new System.Windows.Forms.Button();
			this.mTickCountSetButton = new System.Windows.Forms.Button();
			this.mTickCountSelectionBox = new System.Windows.Forms.ComboBox();
			this.mDefaultsButton = new System.Windows.Forms.Button();
			this.mMiscGroupBox = new System.Windows.Forms.GroupBox();
			this.mDeviceReloadIntervalComboBox = new System.Windows.Forms.ComboBox();
			this.mDeviceReloadIntervalLabel = new System.Windows.Forms.Label();
			this.mDeviceReloadIntervalDefaultButton = new System.Windows.Forms.Button();
			this.mLoaderCardsGroupBox = new System.Windows.Forms.GroupBox();
			this.mLoaderCardsDefaultButton = new System.Windows.Forms.Button();
			this.mLoaderCardsPanel = new System.Windows.Forms.Panel();
			this.mFloatingPointGroupBox = new System.Windows.Forms.GroupBox();
			this.mFloatingPointMemoryWordCountLabel = new System.Windows.Forms.Label();
			this.mFloatingPointMemoryWordCountDefaultButton = new System.Windows.Forms.Button();
			this.mFloatingPointMemoryWordCountBox = new MixGui.Components.LongValueTextBox();
			this.mLoaderCard3TextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this.mLoaderCard2TextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this.mLoaderCard1TextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this.mTickCountBox = new MixGui.Components.LongValueTextBox();
			this.mColorProfilingCountsCheckBox = new System.Windows.Forms.CheckBox();
			this.mColorsGroup.SuspendLayout();
			this.mDeviceFilesGroup.SuspendLayout();
			this.mDeviceTickCountsGroup.SuspendLayout();
			this.mMiscGroupBox.SuspendLayout();
			this.mLoaderCardsGroupBox.SuspendLayout();
			this.mLoaderCardsPanel.SuspendLayout();
			this.mFloatingPointGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mColorsGroup
			// 
			this.mColorsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mColorsGroup.Controls.Add(this.mColorProfilingCountsCheckBox);
			this.mColorsGroup.Controls.Add(this.mColorSetButton);
			this.mColorsGroup.Controls.Add(this.mShowColorLabel);
			this.mColorsGroup.Controls.Add(this.mColorSelectionBox);
			this.mColorsGroup.Controls.Add(this.mColorDefaultButton);
			this.mColorsGroup.Location = new System.Drawing.Point(12, 12);
			this.mColorsGroup.Name = "mColorsGroup";
			this.mColorsGroup.Size = new System.Drawing.Size(366, 70);
			this.mColorsGroup.TabIndex = 1;
			this.mColorsGroup.TabStop = false;
			this.mColorsGroup.Text = "Colors";
			// 
			// mColorSetButton
			// 
			this.mColorSetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mColorSetButton.Location = new System.Drawing.Point(254, 15);
			this.mColorSetButton.Name = "mColorSetButton";
			this.mColorSetButton.Size = new System.Drawing.Size(40, 23);
			this.mColorSetButton.TabIndex = 2;
			this.mColorSetButton.Text = "&Set...";
			this.mColorSetButton.Click += new System.EventHandler(this.mColorSetButton_Click);
			// 
			// mShowColorLabel
			// 
			this.mShowColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mShowColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mShowColorLabel.Location = new System.Drawing.Point(198, 16);
			this.mShowColorLabel.Name = "mShowColorLabel";
			this.mShowColorLabel.Size = new System.Drawing.Size(48, 21);
			this.mShowColorLabel.TabIndex = 1;
			this.mShowColorLabel.Text = "Color";
			this.mShowColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mColorSelectionBox
			// 
			this.mColorSelectionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mColorSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mColorSelectionBox.Location = new System.Drawing.Point(8, 16);
			this.mColorSelectionBox.Name = "mColorSelectionBox";
			this.mColorSelectionBox.Size = new System.Drawing.Size(182, 21);
			this.mColorSelectionBox.Sorted = true;
			this.mColorSelectionBox.TabIndex = 0;
			this.mColorSelectionBox.SelectionChangeCommitted += new System.EventHandler(this.mColorSelectionBox_SelectionChangeCommited);
			// 
			// mColorDefaultButton
			// 
			this.mColorDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mColorDefaultButton.Location = new System.Drawing.Point(302, 15);
			this.mColorDefaultButton.Name = "mColorDefaultButton";
			this.mColorDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mColorDefaultButton.TabIndex = 3;
			this.mColorDefaultButton.Text = "Default";
			this.mColorDefaultButton.Click += new System.EventHandler(this.mColorDefaultButton_Click);
			// 
			// mDeviceFilesGroup
			// 
			this.mDeviceFilesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceDirectoryBox);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceDirectoryLabel);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceDirectorySetButton);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceDirectoryDefaultButton);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceFileSelectionBox);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceFileBox);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceFileSetButton);
			this.mDeviceFilesGroup.Controls.Add(this.mDeviceFileDefaultButton);
			this.mDeviceFilesGroup.Location = new System.Drawing.Point(12, 131);
			this.mDeviceFilesGroup.Name = "mDeviceFilesGroup";
			this.mDeviceFilesGroup.Size = new System.Drawing.Size(366, 80);
			this.mDeviceFilesGroup.TabIndex = 3;
			this.mDeviceFilesGroup.TabStop = false;
			this.mDeviceFilesGroup.Text = "Device files";
			// 
			// mDeviceDirectoryBox
			// 
			this.mDeviceDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceDirectoryBox.Location = new System.Drawing.Point(100, 16);
			this.mDeviceDirectoryBox.Name = "mDeviceDirectoryBox";
			this.mDeviceDirectoryBox.ReadOnly = true;
			this.mDeviceDirectoryBox.Size = new System.Drawing.Size(146, 20);
			this.mDeviceDirectoryBox.TabIndex = 1;
			// 
			// mDeviceDirectoryLabel
			// 
			this.mDeviceDirectoryLabel.Location = new System.Drawing.Point(6, 16);
			this.mDeviceDirectoryLabel.Name = "mDeviceDirectoryLabel";
			this.mDeviceDirectoryLabel.Size = new System.Drawing.Size(96, 21);
			this.mDeviceDirectoryLabel.TabIndex = 0;
			this.mDeviceDirectoryLabel.Text = "Default directory:";
			this.mDeviceDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mDeviceDirectorySetButton
			// 
			this.mDeviceDirectorySetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceDirectorySetButton.Location = new System.Drawing.Point(254, 15);
			this.mDeviceDirectorySetButton.Name = "mDeviceDirectorySetButton";
			this.mDeviceDirectorySetButton.Size = new System.Drawing.Size(40, 23);
			this.mDeviceDirectorySetButton.TabIndex = 2;
			this.mDeviceDirectorySetButton.Text = "S&et...";
			this.mDeviceDirectorySetButton.Click += new System.EventHandler(this.mDeviceDirectorySetButton_Click);
			// 
			// mDeviceDirectoryDefaultButton
			// 
			this.mDeviceDirectoryDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceDirectoryDefaultButton.Location = new System.Drawing.Point(302, 15);
			this.mDeviceDirectoryDefaultButton.Name = "mDeviceDirectoryDefaultButton";
			this.mDeviceDirectoryDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mDeviceDirectoryDefaultButton.TabIndex = 3;
			this.mDeviceDirectoryDefaultButton.Text = "Default";
			this.mDeviceDirectoryDefaultButton.Click += new System.EventHandler(this.mDeviceDirectoryDefaultButton_Click);
			// 
			// mDeviceFileSelectionBox
			// 
			this.mDeviceFileSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mDeviceFileSelectionBox.Location = new System.Drawing.Point(6, 48);
			this.mDeviceFileSelectionBox.Name = "mDeviceFileSelectionBox";
			this.mDeviceFileSelectionBox.Size = new System.Drawing.Size(88, 21);
			this.mDeviceFileSelectionBox.Sorted = true;
			this.mDeviceFileSelectionBox.TabIndex = 4;
			this.mDeviceFileSelectionBox.SelectionChangeCommitted += new System.EventHandler(this.mDeviceFileSelectionBox_SelectionChangeCommitted);
			// 
			// mDeviceFileBox
			// 
			this.mDeviceFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceFileBox.Location = new System.Drawing.Point(100, 48);
			this.mDeviceFileBox.Name = "mDeviceFileBox";
			this.mDeviceFileBox.ReadOnly = true;
			this.mDeviceFileBox.Size = new System.Drawing.Size(146, 20);
			this.mDeviceFileBox.TabIndex = 5;
			// 
			// mDeviceFileSetButton
			// 
			this.mDeviceFileSetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceFileSetButton.Location = new System.Drawing.Point(254, 47);
			this.mDeviceFileSetButton.Name = "mDeviceFileSetButton";
			this.mDeviceFileSetButton.Size = new System.Drawing.Size(40, 23);
			this.mDeviceFileSetButton.TabIndex = 6;
			this.mDeviceFileSetButton.Text = "Se&t...";
			this.mDeviceFileSetButton.Click += new System.EventHandler(this.mDeviceFileSetButton_Click);
			// 
			// mDeviceFileDefaultButton
			// 
			this.mDeviceFileDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceFileDefaultButton.Location = new System.Drawing.Point(302, 47);
			this.mDeviceFileDefaultButton.Name = "mDeviceFileDefaultButton";
			this.mDeviceFileDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mDeviceFileDefaultButton.TabIndex = 7;
			this.mDeviceFileDefaultButton.Text = "Default";
			this.mDeviceFileDefaultButton.Click += new System.EventHandler(this.mDeviceFileDefaultButton_Click);
			// 
			// mOkButton
			// 
			this.mOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mOkButton.Location = new System.Drawing.Point(224, 382);
			this.mOkButton.Name = "mOkButton";
			this.mOkButton.Size = new System.Drawing.Size(75, 23);
			this.mOkButton.TabIndex = 8;
			this.mOkButton.Text = "&OK";
			this.mOkButton.Click += new System.EventHandler(this.mOkButton_Click);
			// 
			// mCancelButton
			// 
			this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCancelButton.Location = new System.Drawing.Point(304, 382);
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.Size = new System.Drawing.Size(75, 23);
			this.mCancelButton.TabIndex = 9;
			this.mCancelButton.Text = "&Cancel";
			// 
			// mDeviceTickCountsGroup
			// 
			this.mDeviceTickCountsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceTickCountsGroup.Controls.Add(this.mTickCountBox);
			this.mDeviceTickCountsGroup.Controls.Add(this.mTickCountDefaultButton);
			this.mDeviceTickCountsGroup.Controls.Add(this.mTickCountSetButton);
			this.mDeviceTickCountsGroup.Controls.Add(this.mTickCountSelectionBox);
			this.mDeviceTickCountsGroup.Location = new System.Drawing.Point(12, 211);
			this.mDeviceTickCountsGroup.Name = "mDeviceTickCountsGroup";
			this.mDeviceTickCountsGroup.Size = new System.Drawing.Size(366, 48);
			this.mDeviceTickCountsGroup.TabIndex = 4;
			this.mDeviceTickCountsGroup.TabStop = false;
			this.mDeviceTickCountsGroup.Text = "Device tick counts";
			// 
			// mTickCountDefaultButton
			// 
			this.mTickCountDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mTickCountDefaultButton.Location = new System.Drawing.Point(302, 16);
			this.mTickCountDefaultButton.Name = "mTickCountDefaultButton";
			this.mTickCountDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mTickCountDefaultButton.TabIndex = 3;
			this.mTickCountDefaultButton.Text = "Default";
			this.mTickCountDefaultButton.Click += new System.EventHandler(this.mTickCountDefaultButton_Click);
			// 
			// mTickCountSetButton
			// 
			this.mTickCountSetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mTickCountSetButton.Location = new System.Drawing.Point(246, 16);
			this.mTickCountSetButton.Name = "mTickCountSetButton";
			this.mTickCountSetButton.Size = new System.Drawing.Size(48, 23);
			this.mTickCountSetButton.TabIndex = 2;
			this.mTickCountSetButton.Text = "&Apply";
			this.mTickCountSetButton.Click += new System.EventHandler(this.mTickCountSetButton_Click);
			// 
			// mTickCountSelectionBox
			// 
			this.mTickCountSelectionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mTickCountSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mTickCountSelectionBox.Location = new System.Drawing.Point(6, 16);
			this.mTickCountSelectionBox.Name = "mTickCountSelectionBox";
			this.mTickCountSelectionBox.Size = new System.Drawing.Size(182, 21);
			this.mTickCountSelectionBox.Sorted = true;
			this.mTickCountSelectionBox.TabIndex = 0;
			this.mTickCountSelectionBox.SelectionChangeCommitted += new System.EventHandler(this.mTickCountSelectionBox_SelectionChangeCommitted);
			// 
			// mDefaultsButton
			// 
			this.mDefaultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mDefaultsButton.Location = new System.Drawing.Point(144, 382);
			this.mDefaultsButton.Name = "mDefaultsButton";
			this.mDefaultsButton.Size = new System.Drawing.Size(75, 23);
			this.mDefaultsButton.TabIndex = 7;
			this.mDefaultsButton.Text = "&Defaults";
			this.mDefaultsButton.Click += new System.EventHandler(this.mDefaultsButton_Click);
			// 
			// mMiscGroupBox
			// 
			this.mMiscGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mMiscGroupBox.Controls.Add(this.mDeviceReloadIntervalComboBox);
			this.mMiscGroupBox.Controls.Add(this.mDeviceReloadIntervalLabel);
			this.mMiscGroupBox.Controls.Add(this.mDeviceReloadIntervalDefaultButton);
			this.mMiscGroupBox.Location = new System.Drawing.Point(12, 327);
			this.mMiscGroupBox.Name = "mMiscGroupBox";
			this.mMiscGroupBox.Size = new System.Drawing.Size(366, 48);
			this.mMiscGroupBox.TabIndex = 6;
			this.mMiscGroupBox.TabStop = false;
			this.mMiscGroupBox.Text = "Miscellaneous";
			// 
			// mDeviceReloadIntervalComboBox
			// 
			this.mDeviceReloadIntervalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mDeviceReloadIntervalComboBox.FormattingEnabled = true;
			this.mDeviceReloadIntervalComboBox.Location = new System.Drawing.Point(156, 15);
			this.mDeviceReloadIntervalComboBox.Name = "mDeviceReloadIntervalComboBox";
			this.mDeviceReloadIntervalComboBox.Size = new System.Drawing.Size(73, 21);
			this.mDeviceReloadIntervalComboBox.TabIndex = 5;
			this.mDeviceReloadIntervalComboBox.SelectedIndexChanged += new System.EventHandler(this.mDeviceReloadIntervalComboBox_SelectedIndexChanged);
			// 
			// mDeviceReloadIntervalLabel
			// 
			this.mDeviceReloadIntervalLabel.AutoSize = true;
			this.mDeviceReloadIntervalLabel.Location = new System.Drawing.Point(6, 18);
			this.mDeviceReloadIntervalLabel.Name = "mDeviceReloadIntervalLabel";
			this.mDeviceReloadIntervalLabel.Size = new System.Drawing.Size(145, 13);
			this.mDeviceReloadIntervalLabel.TabIndex = 4;
			this.mDeviceReloadIntervalLabel.Text = "Device editor reload interval: ";
			// 
			// mDeviceReloadIntervalDefaultButton
			// 
			this.mDeviceReloadIntervalDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceReloadIntervalDefaultButton.Location = new System.Drawing.Point(302, 15);
			this.mDeviceReloadIntervalDefaultButton.Name = "mDeviceReloadIntervalDefaultButton";
			this.mDeviceReloadIntervalDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mDeviceReloadIntervalDefaultButton.TabIndex = 3;
			this.mDeviceReloadIntervalDefaultButton.Text = "Default";
			this.mDeviceReloadIntervalDefaultButton.Click += new System.EventHandler(this.mDeviceReloadIntervalDefaultButton_Click);
			// 
			// mLoaderCardsGroupBox
			// 
			this.mLoaderCardsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCardsGroupBox.Controls.Add(this.mLoaderCardsDefaultButton);
			this.mLoaderCardsGroupBox.Controls.Add(this.mLoaderCardsPanel);
			this.mLoaderCardsGroupBox.Location = new System.Drawing.Point(12, 259);
			this.mLoaderCardsGroupBox.Name = "mLoaderCardsGroupBox";
			this.mLoaderCardsGroupBox.Size = new System.Drawing.Size(366, 68);
			this.mLoaderCardsGroupBox.TabIndex = 5;
			this.mLoaderCardsGroupBox.TabStop = false;
			this.mLoaderCardsGroupBox.Text = "Loader instruction cards";
			// 
			// mLoaderCardsDefaultButton
			// 
			this.mLoaderCardsDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCardsDefaultButton.Location = new System.Drawing.Point(302, 16);
			this.mLoaderCardsDefaultButton.Name = "mLoaderCardsDefaultButton";
			this.mLoaderCardsDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mLoaderCardsDefaultButton.TabIndex = 4;
			this.mLoaderCardsDefaultButton.Text = "Default";
			this.mLoaderCardsDefaultButton.Click += new System.EventHandler(this.mLoaderCardsDefaultButton_Click);
			// 
			// mLoaderCardsPanel
			// 
			this.mLoaderCardsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCardsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mLoaderCardsPanel.Controls.Add(this.mLoaderCard3TextBox);
			this.mLoaderCardsPanel.Controls.Add(this.mLoaderCard2TextBox);
			this.mLoaderCardsPanel.Controls.Add(this.mLoaderCard1TextBox);
			this.mLoaderCardsPanel.Location = new System.Drawing.Point(8, 16);
			this.mLoaderCardsPanel.Name = "mLoaderCardsPanel";
			this.mLoaderCardsPanel.Size = new System.Drawing.Size(286, 44);
			this.mLoaderCardsPanel.TabIndex = 0;
			// 
			// mFloatingPointGroupBox
			// 
			this.mFloatingPointGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mFloatingPointGroupBox.Controls.Add(this.mFloatingPointMemoryWordCountBox);
			this.mFloatingPointGroupBox.Controls.Add(this.mFloatingPointMemoryWordCountLabel);
			this.mFloatingPointGroupBox.Controls.Add(this.mFloatingPointMemoryWordCountDefaultButton);
			this.mFloatingPointGroupBox.Location = new System.Drawing.Point(12, 83);
			this.mFloatingPointGroupBox.Name = "mFloatingPointGroupBox";
			this.mFloatingPointGroupBox.Size = new System.Drawing.Size(366, 48);
			this.mFloatingPointGroupBox.TabIndex = 2;
			this.mFloatingPointGroupBox.TabStop = false;
			this.mFloatingPointGroupBox.Text = "Floating point module";
			// 
			// mFloatingPointMemoryWordCountLabel
			// 
			this.mFloatingPointMemoryWordCountLabel.AutoSize = true;
			this.mFloatingPointMemoryWordCountLabel.Location = new System.Drawing.Point(6, 18);
			this.mFloatingPointMemoryWordCountLabel.Name = "mFloatingPointMemoryWordCountLabel";
			this.mFloatingPointMemoryWordCountLabel.Size = new System.Drawing.Size(181, 13);
			this.mFloatingPointMemoryWordCountLabel.TabIndex = 0;
			this.mFloatingPointMemoryWordCountLabel.Text = "Memory word count (requires restart):";
			// 
			// mFloatingPointMemoryWordCountDefaultButton
			// 
			this.mFloatingPointMemoryWordCountDefaultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mFloatingPointMemoryWordCountDefaultButton.Location = new System.Drawing.Point(302, 15);
			this.mFloatingPointMemoryWordCountDefaultButton.Name = "mFloatingPointMemoryWordCountDefaultButton";
			this.mFloatingPointMemoryWordCountDefaultButton.Size = new System.Drawing.Size(56, 23);
			this.mFloatingPointMemoryWordCountDefaultButton.TabIndex = 2;
			this.mFloatingPointMemoryWordCountDefaultButton.Text = "Default";
			this.mFloatingPointMemoryWordCountDefaultButton.Click += new System.EventHandler(this.mFloatingPointMemoryWordCountDefaultButton_Click);
			// 
			// mFloatingPointMemoryWordCountBox
			// 
			this.mFloatingPointMemoryWordCountBox.BackColor = System.Drawing.Color.White;
			this.mFloatingPointMemoryWordCountBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mFloatingPointMemoryWordCountBox.ClearZero = true;
			this.mFloatingPointMemoryWordCountBox.ForeColor = System.Drawing.Color.Black;
			this.mFloatingPointMemoryWordCountBox.Location = new System.Drawing.Point(199, 16);
			this.mFloatingPointMemoryWordCountBox.LongValue = ((long)(200));
			this.mFloatingPointMemoryWordCountBox.Magnitude = ((long)(200));
			this.mFloatingPointMemoryWordCountBox.MaxLength = 5;
			this.mFloatingPointMemoryWordCountBox.MaxValue = ((long)(99999));
			this.mFloatingPointMemoryWordCountBox.MinValue = ((long)(1));
			this.mFloatingPointMemoryWordCountBox.Name = "mFloatingPointMemoryWordCountBox";
			this.mFloatingPointMemoryWordCountBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mFloatingPointMemoryWordCountBox.Size = new System.Drawing.Size(40, 20);
			this.mFloatingPointMemoryWordCountBox.SupportNegativeZero = false;
			this.mFloatingPointMemoryWordCountBox.TabIndex = 1;
			this.mFloatingPointMemoryWordCountBox.Text = "200";
			this.mFloatingPointMemoryWordCountBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.mFloatingPointMemoryWordCountBox_ValueChanged);
			// 
			// mLoaderCard3TextBox
			// 
			this.mLoaderCard3TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCard3TextBox.BackColor = System.Drawing.Color.White;
			this.mLoaderCard3TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mLoaderCard3TextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mLoaderCard3TextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mLoaderCard3TextBox.ForeColor = System.Drawing.Color.Blue;
			this.mLoaderCard3TextBox.Location = new System.Drawing.Point(0, 28);
			this.mLoaderCard3TextBox.Margin = new System.Windows.Forms.Padding(0);
			fullWord1.LongValue = ((long)(0));
			fullWord1.Magnitude = new MixLib.Type.MixByte[] {
        mixByte1,
        mixByte2,
        mixByte3,
        mixByte4,
        mixByte5};
			fullWord1.MagnitudeLongValue = ((long)(0));
			fullWord1.Sign = MixLib.Type.Word.Signs.Positive;
			this.mLoaderCard3TextBox.MixByteCollectionValue = fullWord1;
			this.mLoaderCard3TextBox.Name = "mLoaderCard3TextBox";
			this.mLoaderCard3TextBox.Size = new System.Drawing.Size(284, 14);
			this.mLoaderCard3TextBox.TabIndex = 2;
			this.mLoaderCard3TextBox.Text = "     ";
			this.mLoaderCard3TextBox.UseEditMode = true;
			// 
			// mLoaderCard2TextBox
			// 
			this.mLoaderCard2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCard2TextBox.BackColor = System.Drawing.Color.White;
			this.mLoaderCard2TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mLoaderCard2TextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mLoaderCard2TextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mLoaderCard2TextBox.ForeColor = System.Drawing.Color.Blue;
			this.mLoaderCard2TextBox.Location = new System.Drawing.Point(0, 14);
			this.mLoaderCard2TextBox.Margin = new System.Windows.Forms.Padding(0);
			fullWord2.LongValue = ((long)(0));
			fullWord2.Magnitude = new MixLib.Type.MixByte[] {
        mixByte6,
        mixByte7,
        mixByte8,
        mixByte9,
        mixByte10};
			fullWord2.MagnitudeLongValue = ((long)(0));
			fullWord2.Sign = MixLib.Type.Word.Signs.Positive;
			this.mLoaderCard2TextBox.MixByteCollectionValue = fullWord2;
			this.mLoaderCard2TextBox.Name = "mLoaderCard2TextBox";
			this.mLoaderCard2TextBox.Size = new System.Drawing.Size(284, 14);
			this.mLoaderCard2TextBox.TabIndex = 1;
			this.mLoaderCard2TextBox.Text = "     ";
			this.mLoaderCard2TextBox.UseEditMode = true;
			// 
			// mLoaderCard1TextBox
			// 
			this.mLoaderCard1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLoaderCard1TextBox.BackColor = System.Drawing.Color.White;
			this.mLoaderCard1TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mLoaderCard1TextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mLoaderCard1TextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mLoaderCard1TextBox.ForeColor = System.Drawing.Color.Blue;
			this.mLoaderCard1TextBox.Location = new System.Drawing.Point(0, 0);
			this.mLoaderCard1TextBox.Margin = new System.Windows.Forms.Padding(0);
			fullWord3.LongValue = ((long)(0));
			fullWord3.Magnitude = new MixLib.Type.MixByte[] {
        mixByte11,
        mixByte12,
        mixByte13,
        mixByte14,
        mixByte15};
			fullWord3.MagnitudeLongValue = ((long)(0));
			fullWord3.Sign = MixLib.Type.Word.Signs.Positive;
			this.mLoaderCard1TextBox.MixByteCollectionValue = fullWord3;
			this.mLoaderCard1TextBox.Name = "mLoaderCard1TextBox";
			this.mLoaderCard1TextBox.Size = new System.Drawing.Size(284, 14);
			this.mLoaderCard1TextBox.TabIndex = 0;
			this.mLoaderCard1TextBox.Text = "     ";
			this.mLoaderCard1TextBox.UseEditMode = true;
			// 
			// mTickCountBox
			// 
			this.mTickCountBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mTickCountBox.BackColor = System.Drawing.Color.White;
			this.mTickCountBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mTickCountBox.ClearZero = true;
			this.mTickCountBox.ForeColor = System.Drawing.Color.Black;
			this.mTickCountBox.Location = new System.Drawing.Point(198, 17);
			this.mTickCountBox.LongValue = ((long)(1));
			this.mTickCountBox.Magnitude = ((long)(1));
			this.mTickCountBox.MaxLength = 5;
			this.mTickCountBox.MaxValue = ((long)(99999));
			this.mTickCountBox.MinValue = ((long)(1));
			this.mTickCountBox.Name = "mTickCountBox";
			this.mTickCountBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mTickCountBox.Size = new System.Drawing.Size(40, 20);
			this.mTickCountBox.SupportNegativeZero = false;
			this.mTickCountBox.TabIndex = 1;
			this.mTickCountBox.Text = "1";
			this.mTickCountBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.mTickCountBox_ValueChanged);
			// 
			// mColorProfilingCountsCheckBox
			// 
			this.mColorProfilingCountsCheckBox.AutoSize = true;
			this.mColorProfilingCountsCheckBox.Checked = true;
			this.mColorProfilingCountsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mColorProfilingCountsCheckBox.Location = new System.Drawing.Point(8, 43);
			this.mColorProfilingCountsCheckBox.Name = "mColorProfilingCountsCheckBox";
			this.mColorProfilingCountsCheckBox.Size = new System.Drawing.Size(119, 17);
			this.mColorProfilingCountsCheckBox.TabIndex = 4;
			this.mColorProfilingCountsCheckBox.Text = "Color profiler counts";
			this.mColorProfilingCountsCheckBox.UseVisualStyleBackColor = true;
			// 
			// PreferencesForm
			// 
			this.ClientSize = new System.Drawing.Size(384, 412);
			this.Controls.Add(this.mFloatingPointGroupBox);
			this.Controls.Add(this.mLoaderCardsGroupBox);
			this.Controls.Add(this.mMiscGroupBox);
			this.Controls.Add(this.mDefaultsButton);
			this.Controls.Add(this.mDeviceTickCountsGroup);
			this.Controls.Add(this.mOkButton);
			this.Controls.Add(this.mDeviceFilesGroup);
			this.Controls.Add(this.mColorsGroup);
			this.Controls.Add(this.mCancelButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 450);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 450);
			this.Name = "PreferencesForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.VisibleChanged += new System.EventHandler(this.this_VisibleChanged);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.this_KeyPress);
			this.mColorsGroup.ResumeLayout(false);
			this.mColorsGroup.PerformLayout();
			this.mDeviceFilesGroup.ResumeLayout(false);
			this.mDeviceFilesGroup.PerformLayout();
			this.mDeviceTickCountsGroup.ResumeLayout(false);
			this.mDeviceTickCountsGroup.PerformLayout();
			this.mMiscGroupBox.ResumeLayout(false);
			this.mMiscGroupBox.PerformLayout();
			this.mLoaderCardsGroupBox.ResumeLayout(false);
			this.mLoaderCardsPanel.ResumeLayout(false);
			this.mLoaderCardsPanel.PerformLayout();
			this.mFloatingPointGroupBox.ResumeLayout(false);
			this.mFloatingPointGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		private void mColorDefaultButton_Click(object sender, EventArgs e)
		{
			colorComboBoxItem selectedItem = (colorComboBoxItem)mColorSelectionBox.SelectedItem;

			selectedItem.ResetDefault();
			updateColorControls(selectedItem);
			mColorSelectionBox.Focus();
		}

		private void mColorSelectionBox_SelectionChangeCommited(object sender, EventArgs e)
		{
			updateColorControls((colorComboBoxItem)mColorSelectionBox.SelectedItem);
		}

		private void mColorSetButton_Click(object sender, EventArgs e)
		{
			if (mColorDialog == null)
			{
				mColorDialog = new ColorDialog();
				mColorDialog.AnyColor = true;
				mColorDialog.SolidColorOnly = true;
				mColorDialog.AllowFullOpen = true;
			}

			colorComboBoxItem selectedItem = (colorComboBoxItem)mColorSelectionBox.SelectedItem;
			mColorDialog.CustomColors = new int[] { selectedItem.Color.B << 16 | selectedItem.Color.G << 8 | selectedItem.Color.R };
			mColorDialog.Color = selectedItem.Color;

			if (mColorDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.Color = mColorDialog.Color;
				updateColorControls(selectedItem);
			}
		}

		private void mDefaultsButton_Click(object sender, EventArgs e)
		{
			foreach (colorComboBoxItem colorItem in mColorSelectionBox.Items)
			{
				colorItem.ResetDefault();
			}

			mColorProfilingCountsCheckBox.Checked = true;

			foreach (deviceFileComboBoxItem deviceFileItem in mDeviceFileSelectionBox.Items)
			{
				deviceFileItem.ResetDefault();
			}

			foreach (tickCountComboBoxItem tickCountItem in mTickCountSelectionBox.Items)
			{
				tickCountItem.ResetDefault();
			}

			mDeviceFilesDirectory = null;
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;

			updateColorControls((colorComboBoxItem)mColorSelectionBox.SelectedItem);
			updateDeviceDirectoryControls();
			updateDeviceFileControls((deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			updateTickCountControls((tickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);
			updateDeviceReloadIntervalControls();
			mFloatingPointMemoryWordCount = null;
			updateFloatingPointControls();

			loadDefaultLoaderCards();
		}

		private void updateFloatingPointControls()
		{
			mFloatingPointMemoryWordCountBox.LongValue = mFloatingPointMemoryWordCount ?? ModuleSettings.FloatingPointMemoryWordCountDefault;
			mFloatingPointMemoryWordCountDefaultButton.Enabled = mFloatingPointMemoryWordCount != null;
		}

		private void mDeviceDirectoryDefaultButton_Click(object sender, EventArgs e)
		{
			mDeviceFilesDirectory = null;
			updateDeviceDirectoryControls();
			updateDeviceFileControls((deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			mDeviceDirectoryBox.Focus();
		}

		private void mDeviceDirectorySetButton_Click(object sender, EventArgs e)
		{
			if (mDeviceFilesFolderBrowserDialog == null)
			{
				mDeviceFilesFolderBrowserDialog = new FolderBrowserDialog();
				mDeviceFilesFolderBrowserDialog.Description = "Please select the default directory for device files.";
			}

			mDeviceFilesFolderBrowserDialog.SelectedPath = getCurrentDeviceFilesDirectory();

			if (mDeviceFilesFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				mDeviceFilesDirectory = mDeviceFilesFolderBrowserDialog.SelectedPath;
				updateDeviceDirectoryControls();
				updateDeviceFileControls((deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
			}
		}

		private void mDeviceFileDefaultButton_Click(object sender, EventArgs e)
		{
			deviceFileComboBoxItem selectedItem = (deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			updateDeviceFileControls(selectedItem);
			mDeviceFileSelectionBox.Focus();
		}

		private void mDeviceFileSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			updateDeviceFileControls((deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem);
		}

		private void mDeviceFileSetButton_Click(object sender, EventArgs e)
		{
			if (mSelectDeviceFileDialog == null)
			{
				mSelectDeviceFileDialog = new SaveFileDialog();
				mSelectDeviceFileDialog.CreatePrompt = true;
				mSelectDeviceFileDialog.DefaultExt = "mixdev";
				mSelectDeviceFileDialog.Filter = "MIX device files|*.mixdev|All files|*.*";
				mSelectDeviceFileDialog.OverwritePrompt = false;
			}

			deviceFileComboBoxItem selectedItem = (deviceFileComboBoxItem)mDeviceFileSelectionBox.SelectedItem;
			mSelectDeviceFileDialog.FileName = Path.Combine(getCurrentDeviceFilesDirectory(), selectedItem.FilePath);

			if (mSelectDeviceFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedItem.FilePath = mSelectDeviceFileDialog.FileName;
				updateDeviceFileControls(selectedItem);
			}
		}

		private void mOkButton_Click(object sender, EventArgs e)
		{
			mConfiguration.Colors.Clear();
			foreach (colorComboBoxItem colorItem in mColorSelectionBox.Items)
			{
				if (!colorItem.IsDefault)
				{
					mConfiguration.Colors.Add(colorItem.Name, colorItem.Color);
				}
			}

			mConfiguration.ColorProfilingCounts = mColorProfilingCountsCheckBox.Checked;

			mConfiguration.DeviceFilePaths.Clear();
			foreach (deviceFileComboBoxItem deviceFileItem in mDeviceFileSelectionBox.Items)
			{
				if (!deviceFileItem.IsDefault)
				{
					mConfiguration.DeviceFilePaths.Add(deviceFileItem.Id, deviceFileItem.FilePath);
				}
			}

			mConfiguration.TickCounts.Clear();
			foreach (tickCountComboBoxItem tickCountItem in mTickCountSelectionBox.Items)
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
				base.DialogResult = DialogResult.None;
			}

		}

		private void mTickCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			setTickCountValue();
		}

		private void mTickCountDefaultButton_Click(object sender, EventArgs e)
		{
			tickCountComboBoxItem selectedItem = (tickCountComboBoxItem)mTickCountSelectionBox.SelectedItem;
			selectedItem.ResetDefault();
			updateTickCountControls(selectedItem);
			mTickCountSelectionBox.Focus();
		}

		private void mTickCountSelectionBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			updateTickCountControls((tickCountComboBoxItem)mTickCountSelectionBox.SelectedItem);
		}

		private void mTickCountSetButton_Click(object sender, EventArgs e)
		{
			setTickCountValue();
		}

		private void setTickCountValue()
		{
			tickCountComboBoxItem selectedItem = (tickCountComboBoxItem)mTickCountSelectionBox.SelectedItem;
			selectedItem.TickCount = (int)mTickCountBox.LongValue;
			updateTickCountControls(selectedItem);
		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape && !(getFocusedControl() is IEscapeConsumer))
			{
				base.DialogResult = DialogResult.Cancel;
				base.Hide();
			}
		}

		private void this_VisibleChanged(object sender, EventArgs e)
		{
			if (base.Visible)
			{
				fillColorSelectionBox();
				mColorProfilingCountsCheckBox.Checked = mConfiguration.ColorProfilingCounts;
				mDeviceFilesDirectory = mConfiguration.DeviceFilesDirectory;
				updateDeviceDirectoryControls();
				fillDeviceFileSelectionBox();
				fillTickCountSelectionBox();
				mDeviceReloadInterval = mConfiguration.DeviceReloadInterval;
				updateDeviceReloadIntervalControls();
				mFloatingPointMemoryWordCount = mConfiguration.FloatingPointMemoryWordCount;
				updateFloatingPointControls();
				loadLoaderCards();
			}
		}

		private void loadLoaderCards()
		{
			string[] loaderCards = mConfiguration.LoaderCards != null ? mConfiguration.LoaderCards : CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				MixByteCollectionCharTextBox loaderCardTextBox = mLoaderCardTextBoxes[index];
				loaderCardTextBox.MixByteCollectionValue = new MixByteCollection(CardReaderDevice.BytesPerRecord);
				loaderCardTextBox.MixByteCollectionValue.Load(index < loaderCards.Length ? loaderCards[index] : string.Empty);
				loaderCardTextBox.Update();
			}

			mLoaderCardsDefaultButton.Enabled = mConfiguration.LoaderCards != null;
		}

		private void updateColorControls(colorComboBoxItem item)
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
					mShowColorLabel.ForeColor = getMatchingColor(item.Name, false);
					mShowColorLabel.BackColor = item.Color;
				}
				else
				{
					mShowColorLabel.ForeColor = item.Color;
					mShowColorLabel.BackColor = getMatchingColor(item.Name, true);
				}

				mColorSetButton.Enabled = true;
				mColorDefaultButton.Enabled = !item.IsDefault;
			}
		}

		private void updateDeviceReloadIntervalControls()
		{
			if (mDeviceReloadInterval == DeviceSettings.UnsetDeviceReloadInterval)
			{
				selectDeviceReloadInterval(DeviceSettings.DefaultDeviceReloadInterval);
				mDeviceReloadIntervalDefaultButton.Enabled = false;
			}
			else
			{
				selectDeviceReloadInterval(mDeviceReloadInterval);
				mDeviceReloadIntervalDefaultButton.Enabled = true;
			}
		}

		private void selectDeviceReloadInterval(int interval)
		{
			foreach (deviceReloadIntervalComboBoxItem item in mDeviceReloadIntervalComboBox.Items)
			{
				if (item.MilliSeconds >= interval)
				{
					mDeviceReloadIntervalComboBox.SelectedItem = item;
					return;
				}
			}
		}

		private void updateDeviceDirectoryControls()
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

		private void updateDeviceFileControls(deviceFileComboBoxItem item)
		{
			if (item == null)
			{
				mDeviceFileBox.Text = "";
				mDeviceFileSetButton.Enabled = false;
				mDeviceFileDefaultButton.Enabled = false;
			}
			else
			{
				mDeviceFileBox.Text = Path.Combine(getCurrentDeviceFilesDirectory(), item.FilePath);
				mDeviceFileSetButton.Enabled = true;
				mDeviceFileDefaultButton.Enabled = !item.IsDefault;
			}
		}

		public void UpdateLayout()
		{
			mTickCountBox.UpdateLayout();
		}

		private void updateTickCountControls(tickCountComboBoxItem item)
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

		/// <summary>
		/// Instances of this class are used to store the color settings 
		/// </summary>
		private class colorComboBoxItem
		{
			private string mDisplayName;
			private bool mIsDefault;
			private string mName;
			private System.Drawing.Color mSetColor;

			public colorComboBoxItem(string name)
			{
				mName = name;
				mDisplayName = getDisplayName(name);
				mIsDefault = true;
			}

			private static string getDisplayName(string name)
			{
				StringBuilder builder = new StringBuilder(name[0].ToString());

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

			public override string ToString()
			{
				return mDisplayName;
			}

			public System.Drawing.Color Color
			{
				get
				{
					return !mIsDefault ? mSetColor : GuiSettings.GetDefaultColor(mName);
				}
				set
				{
					mSetColor = value;
					mIsDefault = false;
				}
			}

			public bool IsBackground
			{
				get
				{
					return !mName.EndsWith("Text");
				}
			}

			public bool IsDefault
			{
				get
				{
					return mIsDefault;
				}
			}

			public string Name
			{
				get
				{
					return mName;
				}
			}
		}

		/// <summary>
		/// Instances of this class are used to store the device file settings 
		/// </summary>
		private class deviceFileComboBoxItem
		{
			private FileBasedDevice mDevice;
			private string mFilePath;

			public deviceFileComboBoxItem(FileBasedDevice device)
			{
				mDevice = device;
			}

			public void ResetDefault()
			{
				mFilePath = null;
			}

			public override string ToString()
			{
				return (mDevice.Id.ToString("D2") + ": " + mDevice.ShortName);
			}

			public string FilePath
			{
				get
				{
					return mFilePath != null ? mFilePath : mDevice.DefaultFileName;
				}
				set
				{
					mFilePath = value;
				}
			}

			public int Id
			{
				get
				{
					return mDevice.Id;
				}
			}

			public bool IsDefault
			{
				get
				{
					return (mFilePath == null);
				}
			}
		}

		/// <summary>
		/// Instances of this class are used to store the tick count settings 
		/// </summary>
		private class tickCountComboBoxItem
		{
			private string mDisplayName;
			private bool mIsDefault;
			private string mName;
			private int mSetTickCount;

			public tickCountComboBoxItem(string name)
			{
				mName = name;
				mDisplayName = getDisplayName(name);
				mIsDefault = true;
			}

			private static string getDisplayName(string name)
			{
				StringBuilder builder = new StringBuilder(name[0].ToString());

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

			public override string ToString()
			{
				return mDisplayName;
			}

			public bool IsDefault
			{
				get
				{
					return mIsDefault;
				}
			}

			public string Name
			{
				get
				{
					return mName;
				}
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

		private class deviceReloadIntervalComboBoxItem
		{
			private int mMilliSeconds;

			public deviceReloadIntervalComboBoxItem(int milliSeconds)
			{
				mMilliSeconds = milliSeconds;
			}

			public int MilliSeconds
			{
				get
				{
					return mMilliSeconds;
				}
				set
				{
					mMilliSeconds = value;
				}
			}

			public override string ToString()
			{
				if (mMilliSeconds < 1000)
				{
					return mMilliSeconds.ToString("##0 ms");
				}

				float seconds = mMilliSeconds / 1000f;
				return seconds.ToString("0.## s");
			}
		}

		private void mDeviceReloadIntervalDefaultButton_Click(object sender, EventArgs e)
		{
			mDeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			updateDeviceReloadIntervalControls();
		}

		private void mDeviceReloadIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			mDeviceReloadInterval = ((deviceReloadIntervalComboBoxItem)mDeviceReloadIntervalComboBox.SelectedItem).MilliSeconds;
			mDeviceReloadIntervalDefaultButton.Enabled = true;
		}

		private void mLoaderCardsDefaultButton_Click(object sender, EventArgs e)
		{
			loadDefaultLoaderCards();
		}

		private void loadDefaultLoaderCards()
		{
			string[] defaultLoaderCards = CardDeckExporter.DefaultLoaderCards;

			for (int index = 0; index < mLoaderCardTextBoxes.Length; index++)
			{
				mLoaderCardTextBoxes[index].MixByteCollectionValue.Load(index < defaultLoaderCards.Length ? defaultLoaderCards[index] : string.Empty);
				mLoaderCardTextBoxes[index].Update();
			}

			mLoaderCardsDefaultButton.Enabled = false;
		}

		private void mFloatingPointMemoryWordCountDefaultButton_Click(object sender, EventArgs e)
		{
			mFloatingPointMemoryWordCount = null;
			updateFloatingPointControls();
		}

		private void mFloatingPointMemoryWordCountBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			mFloatingPointMemoryWordCount = (int)mFloatingPointMemoryWordCountBox.LongValue;
			updateFloatingPointControls();
		}
	}
}
