namespace MixGui.Components
{
  partial class DeviceEditorForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) components?.Dispose();

      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			System.Windows.Forms.Label tapePathColonLabel;
			System.Windows.Forms.Label diskPathColonLabel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceEditorForm));
			this.closeButton = new System.Windows.Forms.Button();
			this.disksTab = new System.Windows.Forms.TabPage();
			this.diskDeleteButton = new System.Windows.Forms.Button();
			this.diskPathBox = new System.Windows.Forms.TextBox();
			this.diskSelectorLabel = new System.Windows.Forms.Label();
			this.diskSelectorComboBox = new System.Windows.Forms.ComboBox();
			this.diskEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this.deviceTypeTabs = new System.Windows.Forms.TabControl();
			this.tapesTab = new System.Windows.Forms.TabPage();
			this.tapeDeleteButton = new System.Windows.Forms.Button();
			this.tapePathBox = new System.Windows.Forms.TextBox();
			this.tapeEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this.tapeSelectorComboBox = new System.Windows.Forms.ComboBox();
			this.tapeSelectorLabel = new System.Windows.Forms.Label();
			this.cardReaderTab = new System.Windows.Forms.TabPage();
			this.cardReaderLoadButton = new System.Windows.Forms.Button();
			this.cardReaderDeleteButton = new System.Windows.Forms.Button();
			this.cardReaderPathBox = new System.Windows.Forms.TextBox();
			this.cardReaderPathLabel = new System.Windows.Forms.Label();
			this.cardReaderEditor = new MixGui.Components.TextFileDeviceEditor();
			this.cardPunchTab = new System.Windows.Forms.TabPage();
			this.cardPunchDeleteButton = new System.Windows.Forms.Button();
			this.cardWriterPathBox = new System.Windows.Forms.TextBox();
			this.cardWriterPathLabel = new System.Windows.Forms.Label();
			this.cardWriterEditor = new MixGui.Components.TextFileDeviceEditor();
			this.printerTab = new System.Windows.Forms.TabPage();
			this.printerDeleteButton = new System.Windows.Forms.Button();
			this.printerPathBox = new System.Windows.Forms.TextBox();
			this.printerPathLabel = new System.Windows.Forms.Label();
			this.printerEditor = new MixGui.Components.TextFileDeviceEditor();
			this.paperTapeTab = new System.Windows.Forms.TabPage();
			this.paperTapeDeleteButton = new System.Windows.Forms.Button();
			this.paperTapePathBox = new System.Windows.Forms.TextBox();
			this.paperTapePathLabel = new System.Windows.Forms.Label();
			this.paperTapeEditor = new MixGui.Components.TextFileDeviceEditor();
			this.cardReaderLoadFileDialog = new System.Windows.Forms.OpenFileDialog();
			tapePathColonLabel = new System.Windows.Forms.Label();
			diskPathColonLabel = new System.Windows.Forms.Label();
			this.disksTab.SuspendLayout();
			this.deviceTypeTabs.SuspendLayout();
			this.tapesTab.SuspendLayout();
			this.cardReaderTab.SuspendLayout();
			this.cardPunchTab.SuspendLayout();
			this.printerTab.SuspendLayout();
			this.paperTapeTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// tapePathColonLabel
			// 
			tapePathColonLabel.AutoSize = true;
			tapePathColonLabel.Location = new System.Drawing.Point(162, 7);
			tapePathColonLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			tapePathColonLabel.Name = "tapePathColonLabel";
			tapePathColonLabel.Size = new System.Drawing.Size(10, 15);
			tapePathColonLabel.TabIndex = 4;
			tapePathColonLabel.Text = ":";
			// 
			// diskPathColonLabel
			// 
			diskPathColonLabel.AutoSize = true;
			diskPathColonLabel.Location = new System.Drawing.Point(162, 7);
			diskPathColonLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			diskPathColonLabel.Name = "diskPathColonLabel";
			diskPathColonLabel.Size = new System.Drawing.Size(10, 15);
			diskPathColonLabel.TabIndex = 5;
			diskPathColonLabel.Text = ":";
			// 
			// mCloseButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.closeButton.Location = new System.Drawing.Point(444, 400);
			this.closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.closeButton.Name = "mCloseButton";
			this.closeButton.Size = new System.Drawing.Size(65, 27);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "&Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// mDisksTab
			// 
			this.disksTab.Controls.Add(this.diskDeleteButton);
			this.disksTab.Controls.Add(diskPathColonLabel);
			this.disksTab.Controls.Add(this.diskPathBox);
			this.disksTab.Controls.Add(this.diskSelectorLabel);
			this.disksTab.Controls.Add(this.diskSelectorComboBox);
			this.disksTab.Controls.Add(this.diskEditor);
			this.disksTab.Location = new System.Drawing.Point(4, 27);
			this.disksTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.disksTab.Name = "mDisksTab";
			this.disksTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.disksTab.Size = new System.Drawing.Size(504, 365);
			this.disksTab.TabIndex = 0;
			this.disksTab.Text = "Disks";
			this.disksTab.UseVisualStyleBackColor = true;
			// 
			// mDiskDeleteButton
			// 
			this.diskDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.diskDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.diskDeleteButton.Location = new System.Drawing.Point(439, 1);
			this.diskDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.diskDeleteButton.Name = "mDiskDeleteButton";
			this.diskDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.diskDeleteButton.TabIndex = 3;
			this.diskDeleteButton.Text = "&Delete";
			this.diskDeleteButton.UseVisualStyleBackColor = true;
			this.diskDeleteButton.Click += new System.EventHandler(this.DiskDeleteButton_Click);
			// 
			// mDiskPathBox
			// 
			this.diskPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diskPathBox.BackColor = System.Drawing.SystemColors.Control;
			this.diskPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.diskPathBox.Location = new System.Drawing.Point(174, 7);
			this.diskPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.diskPathBox.Name = "mDiskPathBox";
			this.diskPathBox.ReadOnly = true;
			this.diskPathBox.Size = new System.Drawing.Size(258, 16);
			this.diskPathBox.TabIndex = 2;
			// 
			// mDiskSelectorLabel
			// 
			this.diskSelectorLabel.AutoSize = true;
			this.diskSelectorLabel.Location = new System.Drawing.Point(0, 7);
			this.diskSelectorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.diskSelectorLabel.Name = "mDiskSelectorLabel";
			this.diskSelectorLabel.Size = new System.Drawing.Size(54, 15);
			this.diskSelectorLabel.TabIndex = 0;
			this.diskSelectorLabel.Text = "Edit disk:";
			// 
			// mDiskSelectorComboBox
			// 
			this.diskSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.diskSelectorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.diskSelectorComboBox.FormattingEnabled = true;
			this.diskSelectorComboBox.Location = new System.Drawing.Point(84, 2);
			this.diskSelectorComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.diskSelectorComboBox.Name = "mDiskSelectorComboBox";
			this.diskSelectorComboBox.Size = new System.Drawing.Size(78, 23);
			this.diskSelectorComboBox.TabIndex = 1;
			this.diskSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.DiskSelectorComboBox_SelectedIndexChanged);
			// 
			// mDiskEditor
			// 
			this.diskEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diskEditor.Location = new System.Drawing.Point(0, 33);
			this.diskEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.diskEditor.MinimumSize = new System.Drawing.Size(374, 175);
			this.diskEditor.Name = "mDiskEditor";
			this.diskEditor.ReadOnly = false;
			this.diskEditor.RecordName = "sector";
			this.diskEditor.ResizeInProgress = false;
			this.diskEditor.ShowReadOnly = true;
			this.diskEditor.Size = new System.Drawing.Size(504, 332);
			this.diskEditor.TabIndex = 4;
			// 
			// mDeviceTypeTabs
			// 
			this.deviceTypeTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceTypeTabs.Controls.Add(this.tapesTab);
			this.deviceTypeTabs.Controls.Add(this.disksTab);
			this.deviceTypeTabs.Controls.Add(this.cardReaderTab);
			this.deviceTypeTabs.Controls.Add(this.cardPunchTab);
			this.deviceTypeTabs.Controls.Add(this.printerTab);
			this.deviceTypeTabs.Controls.Add(this.paperTapeTab);
			this.deviceTypeTabs.Location = new System.Drawing.Point(1, 1);
			this.deviceTypeTabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.deviceTypeTabs.Name = "mDeviceTypeTabs";
			this.deviceTypeTabs.SelectedIndex = 0;
			this.deviceTypeTabs.Size = new System.Drawing.Size(512, 396);
			this.deviceTypeTabs.TabIndex = 0;
			this.deviceTypeTabs.SelectedIndexChanged += new System.EventHandler(this.DeviceTypeTabs_SelectedIndexChanged);
			// 
			// mTapesTab
			// 
			this.tapesTab.Controls.Add(this.tapeDeleteButton);
			this.tapesTab.Controls.Add(tapePathColonLabel);
			this.tapesTab.Controls.Add(this.tapePathBox);
			this.tapesTab.Controls.Add(this.tapeEditor);
			this.tapesTab.Controls.Add(this.tapeSelectorComboBox);
			this.tapesTab.Controls.Add(this.tapeSelectorLabel);
			this.tapesTab.Location = new System.Drawing.Point(4, 27);
			this.tapesTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tapesTab.Name = "mTapesTab";
			this.tapesTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tapesTab.Size = new System.Drawing.Size(504, 365);
			this.tapesTab.TabIndex = 1;
			this.tapesTab.Text = "Tapes";
			this.tapesTab.UseVisualStyleBackColor = true;
			// 
			// mTapeDeleteButton
			// 
			this.tapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tapeDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tapeDeleteButton.Location = new System.Drawing.Point(441, 1);
			this.tapeDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tapeDeleteButton.Name = "mTapeDeleteButton";
			this.tapeDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.tapeDeleteButton.TabIndex = 3;
			this.tapeDeleteButton.Text = "&Delete";
			this.tapeDeleteButton.UseVisualStyleBackColor = true;
			this.tapeDeleteButton.Click += new System.EventHandler(this.TapeDeleteButton_Click);
			// 
			// mTapePathBox
			// 
			this.tapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tapePathBox.BackColor = System.Drawing.SystemColors.Control;
			this.tapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tapePathBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.tapePathBox.Location = new System.Drawing.Point(174, 7);
			this.tapePathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tapePathBox.Name = "mTapePathBox";
			this.tapePathBox.ReadOnly = true;
			this.tapePathBox.Size = new System.Drawing.Size(262, 16);
			this.tapePathBox.TabIndex = 2;
			// 
			// mTapeEditor
			// 
			this.tapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tapeEditor.Location = new System.Drawing.Point(0, 33);
			this.tapeEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.tapeEditor.MinimumSize = new System.Drawing.Size(374, 175);
			this.tapeEditor.Name = "mTapeEditor";
			this.tapeEditor.ReadOnly = false;
			this.tapeEditor.RecordName = "record";
			this.tapeEditor.ResizeInProgress = false;
			this.tapeEditor.ShowReadOnly = true;
			this.tapeEditor.Size = new System.Drawing.Size(504, 332);
			this.tapeEditor.TabIndex = 4;
			// 
			// mTapeSelectorComboBox
			// 
			this.tapeSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tapeSelectorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tapeSelectorComboBox.FormattingEnabled = true;
			this.tapeSelectorComboBox.Location = new System.Drawing.Point(84, 2);
			this.tapeSelectorComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tapeSelectorComboBox.Name = "mTapeSelectorComboBox";
			this.tapeSelectorComboBox.Size = new System.Drawing.Size(78, 23);
			this.tapeSelectorComboBox.TabIndex = 1;
			this.tapeSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.TapeSelectorComboBox_SelectedIndexChanged);
			// 
			// mTapeSelectorLabel
			// 
			this.tapeSelectorLabel.AutoSize = true;
			this.tapeSelectorLabel.Location = new System.Drawing.Point(0, 7);
			this.tapeSelectorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.tapeSelectorLabel.Name = "mTapeSelectorLabel";
			this.tapeSelectorLabel.Size = new System.Drawing.Size(56, 15);
			this.tapeSelectorLabel.TabIndex = 0;
			this.tapeSelectorLabel.Text = "Edit tape:";
			// 
			// mCardReaderTab
			// 
			this.cardReaderTab.Controls.Add(this.cardReaderLoadButton);
			this.cardReaderTab.Controls.Add(this.cardReaderDeleteButton);
			this.cardReaderTab.Controls.Add(this.cardReaderPathBox);
			this.cardReaderTab.Controls.Add(this.cardReaderPathLabel);
			this.cardReaderTab.Controls.Add(this.cardReaderEditor);
			this.cardReaderTab.Location = new System.Drawing.Point(4, 27);
			this.cardReaderTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardReaderTab.Name = "mCardReaderTab";
			this.cardReaderTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardReaderTab.Size = new System.Drawing.Size(504, 365);
			this.cardReaderTab.TabIndex = 2;
			this.cardReaderTab.Text = "Card Reader";
			this.cardReaderTab.UseVisualStyleBackColor = true;
			// 
			// mCardReaderLoadButton
			// 
			this.cardReaderLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cardReaderLoadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cardReaderLoadButton.Location = new System.Drawing.Point(368, 1);
			this.cardReaderLoadButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardReaderLoadButton.Name = "mCardReaderLoadButton";
			this.cardReaderLoadButton.Size = new System.Drawing.Size(63, 27);
			this.cardReaderLoadButton.TabIndex = 2;
			this.cardReaderLoadButton.Text = "&Load...";
			this.cardReaderLoadButton.UseVisualStyleBackColor = true;
			this.cardReaderLoadButton.Click += new System.EventHandler(this.CardReaderLoadButton_Click);
			// 
			// mCardReaderDeleteButton
			// 
			this.cardReaderDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cardReaderDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cardReaderDeleteButton.Location = new System.Drawing.Point(439, 1);
			this.cardReaderDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardReaderDeleteButton.Name = "mCardReaderDeleteButton";
			this.cardReaderDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.cardReaderDeleteButton.TabIndex = 3;
			this.cardReaderDeleteButton.Text = "&Delete";
			this.cardReaderDeleteButton.UseVisualStyleBackColor = true;
			this.cardReaderDeleteButton.Click += new System.EventHandler(this.CardReaderDeleteButton_Click);
			// 
			// mCardReaderPathBox
			// 
			this.cardReaderPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cardReaderPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.cardReaderPathBox.Location = new System.Drawing.Point(84, 7);
			this.cardReaderPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardReaderPathBox.Name = "mCardReaderPathBox";
			this.cardReaderPathBox.ReadOnly = true;
			this.cardReaderPathBox.Size = new System.Drawing.Size(277, 16);
			this.cardReaderPathBox.TabIndex = 1;
			// 
			// mCardReaderPathLabel
			// 
			this.cardReaderPathLabel.AutoSize = true;
			this.cardReaderPathLabel.Location = new System.Drawing.Point(0, 7);
			this.cardReaderPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cardReaderPathLabel.Name = "mCardReaderPathLabel";
			this.cardReaderPathLabel.Size = new System.Drawing.Size(55, 15);
			this.cardReaderPathLabel.TabIndex = 0;
			this.cardReaderPathLabel.Text = "File path:";
			// 
			// mCardReaderEditor
			// 
			this.cardReaderEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cardReaderEditor.Location = new System.Drawing.Point(0, 33);
			this.cardReaderEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.cardReaderEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this.cardReaderEditor.Name = "mCardReaderEditor";
			this.cardReaderEditor.ReadOnly = false;
			this.cardReaderEditor.RecordName = "card";
			this.cardReaderEditor.ResizeInProgress = false;
			this.cardReaderEditor.ShowReadOnly = true;
			this.cardReaderEditor.Size = new System.Drawing.Size(504, 332);
			this.cardReaderEditor.TabIndex = 4;
			// 
			// mCardPunchTab
			// 
			this.cardPunchTab.Controls.Add(this.cardPunchDeleteButton);
			this.cardPunchTab.Controls.Add(this.cardWriterPathBox);
			this.cardPunchTab.Controls.Add(this.cardWriterPathLabel);
			this.cardPunchTab.Controls.Add(this.cardWriterEditor);
			this.cardPunchTab.Location = new System.Drawing.Point(4, 27);
			this.cardPunchTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardPunchTab.Name = "mCardPunchTab";
			this.cardPunchTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardPunchTab.Size = new System.Drawing.Size(504, 365);
			this.cardPunchTab.TabIndex = 3;
			this.cardPunchTab.Text = "Card Punch";
			this.cardPunchTab.UseVisualStyleBackColor = true;
			// 
			// mCardPunchDeleteButton
			// 
			this.cardPunchDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cardPunchDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cardPunchDeleteButton.Location = new System.Drawing.Point(439, 1);
			this.cardPunchDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardPunchDeleteButton.Name = "mCardPunchDeleteButton";
			this.cardPunchDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.cardPunchDeleteButton.TabIndex = 2;
			this.cardPunchDeleteButton.Text = "&Delete";
			this.cardPunchDeleteButton.UseVisualStyleBackColor = true;
			this.cardPunchDeleteButton.Click += new System.EventHandler(this.CardPunchDeleteButton_Click);
			// 
			// mCardWriterPathBox
			// 
			this.cardWriterPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cardWriterPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.cardWriterPathBox.Location = new System.Drawing.Point(84, 7);
			this.cardWriterPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.cardWriterPathBox.Name = "mCardWriterPathBox";
			this.cardWriterPathBox.ReadOnly = true;
			this.cardWriterPathBox.Size = new System.Drawing.Size(347, 16);
			this.cardWriterPathBox.TabIndex = 1;
			// 
			// mCardWriterPathLabel
			// 
			this.cardWriterPathLabel.AutoSize = true;
			this.cardWriterPathLabel.Location = new System.Drawing.Point(0, 7);
			this.cardWriterPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cardWriterPathLabel.Name = "mCardWriterPathLabel";
			this.cardWriterPathLabel.Size = new System.Drawing.Size(55, 15);
			this.cardWriterPathLabel.TabIndex = 0;
			this.cardWriterPathLabel.Text = "File path:";
			// 
			// mCardWriterEditor
			// 
			this.cardWriterEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cardWriterEditor.Location = new System.Drawing.Point(0, 31);
			this.cardWriterEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.cardWriterEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this.cardWriterEditor.Name = "mCardWriterEditor";
			this.cardWriterEditor.ReadOnly = true;
			this.cardWriterEditor.RecordName = "card";
			this.cardWriterEditor.ResizeInProgress = false;
			this.cardWriterEditor.ShowReadOnly = false;
			this.cardWriterEditor.Size = new System.Drawing.Size(502, 332);
			this.cardWriterEditor.TabIndex = 3;
			// 
			// mPrinterTab
			// 
			this.printerTab.Controls.Add(this.printerDeleteButton);
			this.printerTab.Controls.Add(this.printerPathBox);
			this.printerTab.Controls.Add(this.printerPathLabel);
			this.printerTab.Controls.Add(this.printerEditor);
			this.printerTab.Location = new System.Drawing.Point(4, 27);
			this.printerTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.printerTab.Name = "mPrinterTab";
			this.printerTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.printerTab.Size = new System.Drawing.Size(504, 365);
			this.printerTab.TabIndex = 4;
			this.printerTab.Text = "Printer";
			this.printerTab.UseVisualStyleBackColor = true;
			// 
			// mPrinterDeleteButton
			// 
			this.printerDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.printerDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.printerDeleteButton.Location = new System.Drawing.Point(439, 1);
			this.printerDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.printerDeleteButton.Name = "mPrinterDeleteButton";
			this.printerDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.printerDeleteButton.TabIndex = 3;
			this.printerDeleteButton.Text = "&Delete";
			this.printerDeleteButton.UseVisualStyleBackColor = true;
			this.printerDeleteButton.Click += new System.EventHandler(this.PrinterDeleteButton_Click);
			// 
			// mPrinterPathBox
			// 
			this.printerPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.printerPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.printerPathBox.Location = new System.Drawing.Point(84, 7);
			this.printerPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.printerPathBox.Name = "mPrinterPathBox";
			this.printerPathBox.ReadOnly = true;
			this.printerPathBox.Size = new System.Drawing.Size(347, 16);
			this.printerPathBox.TabIndex = 1;
			// 
			// mPrinterPathLabel
			// 
			this.printerPathLabel.AutoSize = true;
			this.printerPathLabel.Location = new System.Drawing.Point(0, 7);
			this.printerPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.printerPathLabel.Name = "mPrinterPathLabel";
			this.printerPathLabel.Size = new System.Drawing.Size(55, 15);
			this.printerPathLabel.TabIndex = 0;
			this.printerPathLabel.Text = "File path:";
			// 
			// mPrinterEditor
			// 
			this.printerEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.printerEditor.Location = new System.Drawing.Point(0, 31);
			this.printerEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.printerEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this.printerEditor.Name = "mPrinterEditor";
			this.printerEditor.ReadOnly = true;
			this.printerEditor.RecordName = "line";
			this.printerEditor.ResizeInProgress = false;
			this.printerEditor.ShowReadOnly = false;
			this.printerEditor.Size = new System.Drawing.Size(502, 332);
			this.printerEditor.TabIndex = 2;
			// 
			// mPaperTapeTab
			// 
			this.paperTapeTab.Controls.Add(this.paperTapeDeleteButton);
			this.paperTapeTab.Controls.Add(this.paperTapePathBox);
			this.paperTapeTab.Controls.Add(this.paperTapePathLabel);
			this.paperTapeTab.Controls.Add(this.paperTapeEditor);
			this.paperTapeTab.Location = new System.Drawing.Point(4, 27);
			this.paperTapeTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.paperTapeTab.Name = "mPaperTapeTab";
			this.paperTapeTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.paperTapeTab.Size = new System.Drawing.Size(504, 365);
			this.paperTapeTab.TabIndex = 5;
			this.paperTapeTab.Text = "Paper Tape";
			this.paperTapeTab.UseVisualStyleBackColor = true;
			// 
			// mPaperTapeDeleteButton
			// 
			this.paperTapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.paperTapeDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.paperTapeDeleteButton.Location = new System.Drawing.Point(439, 1);
			this.paperTapeDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.paperTapeDeleteButton.Name = "mPaperTapeDeleteButton";
			this.paperTapeDeleteButton.Size = new System.Drawing.Size(63, 27);
			this.paperTapeDeleteButton.TabIndex = 2;
			this.paperTapeDeleteButton.Text = "&Delete";
			this.paperTapeDeleteButton.UseVisualStyleBackColor = true;
			this.paperTapeDeleteButton.Click += new System.EventHandler(this.PaperTapeDeleteButton_Click);
			// 
			// mPaperTapePathBox
			// 
			this.paperTapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paperTapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.paperTapePathBox.Location = new System.Drawing.Point(84, 7);
			this.paperTapePathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.paperTapePathBox.Name = "mPaperTapePathBox";
			this.paperTapePathBox.ReadOnly = true;
			this.paperTapePathBox.Size = new System.Drawing.Size(347, 16);
			this.paperTapePathBox.TabIndex = 1;
			// 
			// mPaperTapePathLabel
			// 
			this.paperTapePathLabel.AutoSize = true;
			this.paperTapePathLabel.Location = new System.Drawing.Point(4, 8);
			this.paperTapePathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.paperTapePathLabel.Name = "mPaperTapePathLabel";
			this.paperTapePathLabel.Size = new System.Drawing.Size(55, 15);
			this.paperTapePathLabel.TabIndex = 0;
			this.paperTapePathLabel.Text = "File path:";
			// 
			// mPaperTapeEditor
			// 
			this.paperTapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paperTapeEditor.Location = new System.Drawing.Point(4, 33);
			this.paperTapeEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.paperTapeEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this.paperTapeEditor.Name = "mPaperTapeEditor";
			this.paperTapeEditor.ReadOnly = false;
			this.paperTapeEditor.RecordName = "record";
			this.paperTapeEditor.ResizeInProgress = false;
			this.paperTapeEditor.ShowReadOnly = true;
			this.paperTapeEditor.Size = new System.Drawing.Size(498, 332);
			this.paperTapeEditor.TabIndex = 3;
			// 
			// mCardReaderLoadFileDialog
			// 
			this.cardReaderLoadFileDialog.DefaultExt = "mixdeck";
			this.cardReaderLoadFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			// 
			// DeviceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(517, 433);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.deviceTypeTabs);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeviceEditorForm";
			this.Text = "Edit devices";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceEditorForm_FormClosing);
			this.ResizeBegin += new System.EventHandler(this.DeviceEditorForm_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.DeviceEditorForm_ResizeEnd);
			this.disksTab.ResumeLayout(false);
			this.disksTab.PerformLayout();
			this.deviceTypeTabs.ResumeLayout(false);
			this.tapesTab.ResumeLayout(false);
			this.tapesTab.PerformLayout();
			this.cardReaderTab.ResumeLayout(false);
			this.cardReaderTab.PerformLayout();
			this.cardPunchTab.ResumeLayout(false);
			this.cardPunchTab.PerformLayout();
			this.printerTab.ResumeLayout(false);
			this.printerTab.PerformLayout();
			this.paperTapeTab.ResumeLayout(false);
			this.paperTapeTab.PerformLayout();
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button closeButton;
    private System.Windows.Forms.TabPage disksTab;
    private System.Windows.Forms.Label diskSelectorLabel;
    private System.Windows.Forms.ComboBox diskSelectorComboBox;
    private System.Windows.Forms.TabControl deviceTypeTabs;
    private BinaryFileDeviceEditor diskEditor;
    private System.Windows.Forms.TabPage tapesTab;
    private System.Windows.Forms.Label tapeSelectorLabel;
    private System.Windows.Forms.ComboBox tapeSelectorComboBox;
    private BinaryFileDeviceEditor tapeEditor;
    private System.Windows.Forms.TabPage cardReaderTab;
    private TextFileDeviceEditor cardReaderEditor;
    private System.Windows.Forms.TabPage cardPunchTab;
    private System.Windows.Forms.TabPage printerTab;
    private System.Windows.Forms.TabPage paperTapeTab;
    private TextFileDeviceEditor cardWriterEditor;
    private TextFileDeviceEditor printerEditor;
    private TextFileDeviceEditor paperTapeEditor;
    private System.Windows.Forms.TextBox tapePathBox;
    private System.Windows.Forms.TextBox diskPathBox;
    private System.Windows.Forms.Label paperTapePathLabel;
    private System.Windows.Forms.TextBox paperTapePathBox;
    private System.Windows.Forms.TextBox printerPathBox;
    private System.Windows.Forms.Label printerPathLabel;
    private System.Windows.Forms.TextBox cardWriterPathBox;
    private System.Windows.Forms.Label cardWriterPathLabel;
    private System.Windows.Forms.TextBox cardReaderPathBox;
    private System.Windows.Forms.Label cardReaderPathLabel;
		private System.Windows.Forms.Button tapeDeleteButton;
		private System.Windows.Forms.Button diskDeleteButton;
		private System.Windows.Forms.Button cardReaderDeleteButton;
		private System.Windows.Forms.Button cardPunchDeleteButton;
		private System.Windows.Forms.Button printerDeleteButton;
		private System.Windows.Forms.Button paperTapeDeleteButton;
		private System.Windows.Forms.Button cardReaderLoadButton;
		private System.Windows.Forms.OpenFileDialog cardReaderLoadFileDialog;
  }
}
