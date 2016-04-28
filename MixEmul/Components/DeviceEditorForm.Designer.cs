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
			this.mCloseButton = new System.Windows.Forms.Button();
			this.mDisksTab = new System.Windows.Forms.TabPage();
			this.mDiskDeleteButton = new System.Windows.Forms.Button();
			this.mDiskPathBox = new System.Windows.Forms.TextBox();
			this.mDiskSelectorLabel = new System.Windows.Forms.Label();
			this.mDiskSelectorComboBox = new System.Windows.Forms.ComboBox();
			this.mDeviceTypeTabs = new System.Windows.Forms.TabControl();
			this.mTapesTab = new System.Windows.Forms.TabPage();
			this.mTapeDeleteButton = new System.Windows.Forms.Button();
			this.mTapePathBox = new System.Windows.Forms.TextBox();
			this.mTapeSelectorComboBox = new System.Windows.Forms.ComboBox();
			this.mTapeSelectorLabel = new System.Windows.Forms.Label();
			this.mCardReaderTab = new System.Windows.Forms.TabPage();
			this.mCardReaderLoadButton = new System.Windows.Forms.Button();
			this.mCardReaderDeleteButton = new System.Windows.Forms.Button();
			this.mCardReaderPathBox = new System.Windows.Forms.TextBox();
			this.mCardReaderPathLabel = new System.Windows.Forms.Label();
			this.mCardPunchTab = new System.Windows.Forms.TabPage();
			this.mCardPunchDeleteButton = new System.Windows.Forms.Button();
			this.mCardWriterPathBox = new System.Windows.Forms.TextBox();
			this.mCardWriterPathLabel = new System.Windows.Forms.Label();
			this.mPrinterTab = new System.Windows.Forms.TabPage();
			this.mPrinterDeleteButton = new System.Windows.Forms.Button();
			this.mPrinterPathBox = new System.Windows.Forms.TextBox();
			this.mPrinterPathLabel = new System.Windows.Forms.Label();
			this.mPaperTapeTab = new System.Windows.Forms.TabPage();
			this.mPaperTapeDeleteButton = new System.Windows.Forms.Button();
			this.mPaperTapePathBox = new System.Windows.Forms.TextBox();
			this.mPaperTapePathLabel = new System.Windows.Forms.Label();
			this.mCardReaderLoadFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.mTapeEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this.mDiskEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this.mCardReaderEditor = new MixGui.Components.TextFileDeviceEditor();
			this.mCardWriterEditor = new MixGui.Components.TextFileDeviceEditor();
			this.mPrinterEditor = new MixGui.Components.TextFileDeviceEditor();
			this.mPaperTapeEditor = new MixGui.Components.TextFileDeviceEditor();
			tapePathColonLabel = new System.Windows.Forms.Label();
			diskPathColonLabel = new System.Windows.Forms.Label();
			this.mDisksTab.SuspendLayout();
			this.mDeviceTypeTabs.SuspendLayout();
			this.mTapesTab.SuspendLayout();
			this.mCardReaderTab.SuspendLayout();
			this.mCardPunchTab.SuspendLayout();
			this.mPrinterTab.SuspendLayout();
			this.mPaperTapeTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// tapePathColonLabel
			// 
			tapePathColonLabel.AutoSize = true;
			tapePathColonLabel.Location = new System.Drawing.Point(139, 6);
			tapePathColonLabel.Name = "tapePathColonLabel";
			tapePathColonLabel.Size = new System.Drawing.Size(10, 13);
			tapePathColonLabel.TabIndex = 4;
			tapePathColonLabel.Text = ":";
			// 
			// diskPathColonLabel
			// 
			diskPathColonLabel.AutoSize = true;
			diskPathColonLabel.Location = new System.Drawing.Point(139, 6);
			diskPathColonLabel.Name = "diskPathColonLabel";
			diskPathColonLabel.Size = new System.Drawing.Size(10, 13);
			diskPathColonLabel.TabIndex = 5;
			diskPathColonLabel.Text = ":";
			// 
			// mCloseButton
			// 
			this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mCloseButton.Location = new System.Drawing.Point(260, 350);
			this.mCloseButton.Name = "mCloseButton";
			this.mCloseButton.Size = new System.Drawing.Size(59, 23);
			this.mCloseButton.TabIndex = 1;
			this.mCloseButton.Text = "&Close";
			this.mCloseButton.UseVisualStyleBackColor = true;
			this.mCloseButton.Click += new System.EventHandler(this.mCloseButton_Click);
			// 
			// mDisksTab
			// 
			this.mDisksTab.Controls.Add(this.mDiskDeleteButton);
			this.mDisksTab.Controls.Add(diskPathColonLabel);
			this.mDisksTab.Controls.Add(this.mDiskPathBox);
			this.mDisksTab.Controls.Add(this.mDiskSelectorLabel);
			this.mDisksTab.Controls.Add(this.mDiskSelectorComboBox);
			this.mDisksTab.Controls.Add(this.mDiskEditor);
			this.mDisksTab.Location = new System.Drawing.Point(4, 22);
			this.mDisksTab.Name = "mDisksTab";
			this.mDisksTab.Padding = new System.Windows.Forms.Padding(3);
			this.mDisksTab.Size = new System.Drawing.Size(310, 317);
			this.mDisksTab.TabIndex = 0;
			this.mDisksTab.Text = "Disks";
			this.mDisksTab.UseVisualStyleBackColor = true;
			// 
			// mDiskDeleteButton
			// 
			this.mDiskDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDiskDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mDiskDeleteButton.Name = "mDiskDeleteButton";
			this.mDiskDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mDiskDeleteButton.TabIndex = 3;
			this.mDiskDeleteButton.Text = "&Delete";
			this.mDiskDeleteButton.UseVisualStyleBackColor = true;
			this.mDiskDeleteButton.Click += new System.EventHandler(this.mDiskDeleteButton_Click);
			// 
			// mDiskPathBox
			// 
			this.mDiskPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mDiskPathBox.BackColor = System.Drawing.SystemColors.Control;
			this.mDiskPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mDiskPathBox.Location = new System.Drawing.Point(149, 6);
			this.mDiskPathBox.Name = "mDiskPathBox";
			this.mDiskPathBox.ReadOnly = true;
			this.mDiskPathBox.Size = new System.Drawing.Size(100, 13);
			this.mDiskPathBox.TabIndex = 2;
			// 
			// mDiskSelectorLabel
			// 
			this.mDiskSelectorLabel.AutoSize = true;
			this.mDiskSelectorLabel.Location = new System.Drawing.Point(0, 6);
			this.mDiskSelectorLabel.Name = "mDiskSelectorLabel";
			this.mDiskSelectorLabel.Size = new System.Drawing.Size(50, 13);
			this.mDiskSelectorLabel.TabIndex = 0;
			this.mDiskSelectorLabel.Text = "Edit disk:";
			// 
			// mDiskSelectorComboBox
			// 
			this.mDiskSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mDiskSelectorComboBox.FormattingEnabled = true;
			this.mDiskSelectorComboBox.Location = new System.Drawing.Point(72, 2);
			this.mDiskSelectorComboBox.Name = "mDiskSelectorComboBox";
			this.mDiskSelectorComboBox.Size = new System.Drawing.Size(67, 21);
			this.mDiskSelectorComboBox.TabIndex = 1;
			this.mDiskSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.mDiskSelectorComboBox_SelectedIndexChanged);
			// 
			// mDeviceTypeTabs
			// 
			this.mDeviceTypeTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceTypeTabs.Controls.Add(this.mTapesTab);
			this.mDeviceTypeTabs.Controls.Add(this.mDisksTab);
			this.mDeviceTypeTabs.Controls.Add(this.mCardReaderTab);
			this.mDeviceTypeTabs.Controls.Add(this.mCardPunchTab);
			this.mDeviceTypeTabs.Controls.Add(this.mPrinterTab);
			this.mDeviceTypeTabs.Controls.Add(this.mPaperTapeTab);
			this.mDeviceTypeTabs.Location = new System.Drawing.Point(1, 1);
			this.mDeviceTypeTabs.Name = "mDeviceTypeTabs";
			this.mDeviceTypeTabs.SelectedIndex = 0;
			this.mDeviceTypeTabs.Size = new System.Drawing.Size(318, 343);
			this.mDeviceTypeTabs.TabIndex = 0;
			this.mDeviceTypeTabs.SelectedIndexChanged += new System.EventHandler(this.mDeviceTypeTabs_SelectedIndexChanged);
			// 
			// mTapesTab
			// 
			this.mTapesTab.Controls.Add(this.mTapeDeleteButton);
			this.mTapesTab.Controls.Add(tapePathColonLabel);
			this.mTapesTab.Controls.Add(this.mTapePathBox);
			this.mTapesTab.Controls.Add(this.mTapeEditor);
			this.mTapesTab.Controls.Add(this.mTapeSelectorComboBox);
			this.mTapesTab.Controls.Add(this.mTapeSelectorLabel);
			this.mTapesTab.Location = new System.Drawing.Point(4, 22);
			this.mTapesTab.Name = "mTapesTab";
			this.mTapesTab.Padding = new System.Windows.Forms.Padding(3);
			this.mTapesTab.Size = new System.Drawing.Size(310, 317);
			this.mTapesTab.TabIndex = 1;
			this.mTapesTab.Text = "Tapes";
			this.mTapesTab.UseVisualStyleBackColor = true;
			// 
			// mTapeDeleteButton
			// 
			this.mTapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mTapeDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mTapeDeleteButton.Name = "mTapeDeleteButton";
			this.mTapeDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mTapeDeleteButton.TabIndex = 3;
			this.mTapeDeleteButton.Text = "&Delete";
			this.mTapeDeleteButton.UseVisualStyleBackColor = true;
			this.mTapeDeleteButton.Click += new System.EventHandler(this.mTapeDeleteButton_Click);
			// 
			// mTapePathBox
			// 
			this.mTapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mTapePathBox.BackColor = System.Drawing.SystemColors.Control;
			this.mTapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mTapePathBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.mTapePathBox.Location = new System.Drawing.Point(149, 6);
			this.mTapePathBox.Name = "mTapePathBox";
			this.mTapePathBox.ReadOnly = true;
			this.mTapePathBox.Size = new System.Drawing.Size(100, 13);
			this.mTapePathBox.TabIndex = 2;
			// 
			// mTapeSelectorComboBox
			// 
			this.mTapeSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mTapeSelectorComboBox.FormattingEnabled = true;
			this.mTapeSelectorComboBox.Location = new System.Drawing.Point(72, 2);
			this.mTapeSelectorComboBox.Name = "mTapeSelectorComboBox";
			this.mTapeSelectorComboBox.Size = new System.Drawing.Size(67, 21);
			this.mTapeSelectorComboBox.TabIndex = 1;
			this.mTapeSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.mTapeSelectorComboBox_SelectedIndexChanged);
			// 
			// mTapeSelectorLabel
			// 
			this.mTapeSelectorLabel.AutoSize = true;
			this.mTapeSelectorLabel.Location = new System.Drawing.Point(0, 6);
			this.mTapeSelectorLabel.Name = "mTapeSelectorLabel";
			this.mTapeSelectorLabel.Size = new System.Drawing.Size(52, 13);
			this.mTapeSelectorLabel.TabIndex = 0;
			this.mTapeSelectorLabel.Text = "Edit tape:";
			// 
			// mCardReaderTab
			// 
			this.mCardReaderTab.Controls.Add(this.mCardReaderLoadButton);
			this.mCardReaderTab.Controls.Add(this.mCardReaderDeleteButton);
			this.mCardReaderTab.Controls.Add(this.mCardReaderPathBox);
			this.mCardReaderTab.Controls.Add(this.mCardReaderPathLabel);
			this.mCardReaderTab.Controls.Add(this.mCardReaderEditor);
			this.mCardReaderTab.Location = new System.Drawing.Point(4, 22);
			this.mCardReaderTab.Name = "mCardReaderTab";
			this.mCardReaderTab.Padding = new System.Windows.Forms.Padding(3);
			this.mCardReaderTab.Size = new System.Drawing.Size(310, 317);
			this.mCardReaderTab.TabIndex = 2;
			this.mCardReaderTab.Text = "Card Reader";
			this.mCardReaderTab.UseVisualStyleBackColor = true;
			// 
			// mCardReaderLoadButton
			// 
			this.mCardReaderLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardReaderLoadButton.Location = new System.Drawing.Point(195, 1);
			this.mCardReaderLoadButton.Name = "mCardReaderLoadButton";
			this.mCardReaderLoadButton.Size = new System.Drawing.Size(54, 23);
			this.mCardReaderLoadButton.TabIndex = 2;
			this.mCardReaderLoadButton.Text = "&Load...";
			this.mCardReaderLoadButton.UseVisualStyleBackColor = true;
			this.mCardReaderLoadButton.Click += new System.EventHandler(this.mCardReaderLoadButton_Click);
			// 
			// mCardReaderDeleteButton
			// 
			this.mCardReaderDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardReaderDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mCardReaderDeleteButton.Name = "mCardReaderDeleteButton";
			this.mCardReaderDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mCardReaderDeleteButton.TabIndex = 3;
			this.mCardReaderDeleteButton.Text = "&Delete";
			this.mCardReaderDeleteButton.UseVisualStyleBackColor = true;
			this.mCardReaderDeleteButton.Click += new System.EventHandler(this.mCardReaderDeleteButton_Click);
			// 
			// mCardReaderPathBox
			// 
			this.mCardReaderPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardReaderPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mCardReaderPathBox.Location = new System.Drawing.Point(72, 6);
			this.mCardReaderPathBox.Name = "mCardReaderPathBox";
			this.mCardReaderPathBox.ReadOnly = true;
			this.mCardReaderPathBox.Size = new System.Drawing.Size(117, 13);
			this.mCardReaderPathBox.TabIndex = 1;
			// 
			// mCardReaderPathLabel
			// 
			this.mCardReaderPathLabel.AutoSize = true;
			this.mCardReaderPathLabel.Location = new System.Drawing.Point(0, 6);
			this.mCardReaderPathLabel.Name = "mCardReaderPathLabel";
			this.mCardReaderPathLabel.Size = new System.Drawing.Size(50, 13);
			this.mCardReaderPathLabel.TabIndex = 0;
			this.mCardReaderPathLabel.Text = "File path:";
			// 
			// mCardPunchTab
			// 
			this.mCardPunchTab.Controls.Add(this.mCardPunchDeleteButton);
			this.mCardPunchTab.Controls.Add(this.mCardWriterPathBox);
			this.mCardPunchTab.Controls.Add(this.mCardWriterPathLabel);
			this.mCardPunchTab.Controls.Add(this.mCardWriterEditor);
			this.mCardPunchTab.Location = new System.Drawing.Point(4, 22);
			this.mCardPunchTab.Name = "mCardPunchTab";
			this.mCardPunchTab.Padding = new System.Windows.Forms.Padding(3);
			this.mCardPunchTab.Size = new System.Drawing.Size(310, 317);
			this.mCardPunchTab.TabIndex = 3;
			this.mCardPunchTab.Text = "Card Punch";
			this.mCardPunchTab.UseVisualStyleBackColor = true;
			// 
			// mCardPunchDeleteButton
			// 
			this.mCardPunchDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardPunchDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mCardPunchDeleteButton.Name = "mCardPunchDeleteButton";
			this.mCardPunchDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mCardPunchDeleteButton.TabIndex = 2;
			this.mCardPunchDeleteButton.Text = "&Delete";
			this.mCardPunchDeleteButton.UseVisualStyleBackColor = true;
			this.mCardPunchDeleteButton.Click += new System.EventHandler(this.mCardPunchDeleteButton_Click);
			// 
			// mCardWriterPathBox
			// 
			this.mCardWriterPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardWriterPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mCardWriterPathBox.Location = new System.Drawing.Point(72, 6);
			this.mCardWriterPathBox.Name = "mCardWriterPathBox";
			this.mCardWriterPathBox.ReadOnly = true;
			this.mCardWriterPathBox.Size = new System.Drawing.Size(177, 13);
			this.mCardWriterPathBox.TabIndex = 1;
			// 
			// mCardWriterPathLabel
			// 
			this.mCardWriterPathLabel.AutoSize = true;
			this.mCardWriterPathLabel.Location = new System.Drawing.Point(0, 6);
			this.mCardWriterPathLabel.Name = "mCardWriterPathLabel";
			this.mCardWriterPathLabel.Size = new System.Drawing.Size(50, 13);
			this.mCardWriterPathLabel.TabIndex = 0;
			this.mCardWriterPathLabel.Text = "File path:";
			// 
			// mPrinterTab
			// 
			this.mPrinterTab.Controls.Add(this.mPrinterDeleteButton);
			this.mPrinterTab.Controls.Add(this.mPrinterPathBox);
			this.mPrinterTab.Controls.Add(this.mPrinterPathLabel);
			this.mPrinterTab.Controls.Add(this.mPrinterEditor);
			this.mPrinterTab.Location = new System.Drawing.Point(4, 22);
			this.mPrinterTab.Name = "mPrinterTab";
			this.mPrinterTab.Padding = new System.Windows.Forms.Padding(3);
			this.mPrinterTab.Size = new System.Drawing.Size(310, 317);
			this.mPrinterTab.TabIndex = 4;
			this.mPrinterTab.Text = "Printer";
			this.mPrinterTab.UseVisualStyleBackColor = true;
			// 
			// mPrinterDeleteButton
			// 
			this.mPrinterDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mPrinterDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mPrinterDeleteButton.Name = "mPrinterDeleteButton";
			this.mPrinterDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mPrinterDeleteButton.TabIndex = 3;
			this.mPrinterDeleteButton.Text = "&Delete";
			this.mPrinterDeleteButton.UseVisualStyleBackColor = true;
			this.mPrinterDeleteButton.Click += new System.EventHandler(this.mPrinterDeleteButton_Click);
			// 
			// mPrinterPathBox
			// 
			this.mPrinterPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mPrinterPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mPrinterPathBox.Location = new System.Drawing.Point(72, 6);
			this.mPrinterPathBox.Name = "mPrinterPathBox";
			this.mPrinterPathBox.ReadOnly = true;
			this.mPrinterPathBox.Size = new System.Drawing.Size(177, 13);
			this.mPrinterPathBox.TabIndex = 1;
			// 
			// mPrinterPathLabel
			// 
			this.mPrinterPathLabel.AutoSize = true;
			this.mPrinterPathLabel.Location = new System.Drawing.Point(0, 6);
			this.mPrinterPathLabel.Name = "mPrinterPathLabel";
			this.mPrinterPathLabel.Size = new System.Drawing.Size(50, 13);
			this.mPrinterPathLabel.TabIndex = 0;
			this.mPrinterPathLabel.Text = "File path:";
			// 
			// mPaperTapeTab
			// 
			this.mPaperTapeTab.Controls.Add(this.mPaperTapeDeleteButton);
			this.mPaperTapeTab.Controls.Add(this.mPaperTapePathBox);
			this.mPaperTapeTab.Controls.Add(this.mPaperTapePathLabel);
			this.mPaperTapeTab.Controls.Add(this.mPaperTapeEditor);
			this.mPaperTapeTab.Location = new System.Drawing.Point(4, 22);
			this.mPaperTapeTab.Name = "mPaperTapeTab";
			this.mPaperTapeTab.Padding = new System.Windows.Forms.Padding(3);
			this.mPaperTapeTab.Size = new System.Drawing.Size(310, 317);
			this.mPaperTapeTab.TabIndex = 5;
			this.mPaperTapeTab.Text = "Paper Tape";
			this.mPaperTapeTab.UseVisualStyleBackColor = true;
			// 
			// mPaperTapeDeleteButton
			// 
			this.mPaperTapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mPaperTapeDeleteButton.Location = new System.Drawing.Point(255, 1);
			this.mPaperTapeDeleteButton.Name = "mPaperTapeDeleteButton";
			this.mPaperTapeDeleteButton.Size = new System.Drawing.Size(54, 23);
			this.mPaperTapeDeleteButton.TabIndex = 2;
			this.mPaperTapeDeleteButton.Text = "&Delete";
			this.mPaperTapeDeleteButton.UseVisualStyleBackColor = true;
			this.mPaperTapeDeleteButton.Click += new System.EventHandler(this.mPaperTapeDeleteButton_Click);
			// 
			// mPaperTapePathBox
			// 
			this.mPaperTapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mPaperTapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mPaperTapePathBox.Location = new System.Drawing.Point(72, 6);
			this.mPaperTapePathBox.Name = "mPaperTapePathBox";
			this.mPaperTapePathBox.ReadOnly = true;
			this.mPaperTapePathBox.Size = new System.Drawing.Size(177, 13);
			this.mPaperTapePathBox.TabIndex = 1;
			// 
			// mPaperTapePathLabel
			// 
			this.mPaperTapePathLabel.AutoSize = true;
			this.mPaperTapePathLabel.Location = new System.Drawing.Point(0, 6);
			this.mPaperTapePathLabel.Name = "mPaperTapePathLabel";
			this.mPaperTapePathLabel.Size = new System.Drawing.Size(50, 13);
			this.mPaperTapePathLabel.TabIndex = 0;
			this.mPaperTapePathLabel.Text = "File path:";
			// 
			// mCardReaderLoadFileDialog
			// 
			this.mCardReaderLoadFileDialog.DefaultExt = "mixdeck";
			this.mCardReaderLoadFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			// 
			// mTapeEditor
			// 
			this.mTapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mTapeEditor.Location = new System.Drawing.Point(0, 29);
			this.mTapeEditor.MinimumSize = new System.Drawing.Size(275, 130);
			this.mTapeEditor.Name = "mTapeEditor";
			this.mTapeEditor.ReadOnly = false;
			this.mTapeEditor.RecordName = "record";
			this.mTapeEditor.ResizeInProgress = false;
			this.mTapeEditor.ShowReadOnly = true;
			this.mTapeEditor.Size = new System.Drawing.Size(310, 288);
			this.mTapeEditor.TabIndex = 4;
			// 
			// mDiskEditor
			// 
			this.mDiskEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mDiskEditor.Location = new System.Drawing.Point(0, 29);
			this.mDiskEditor.MinimumSize = new System.Drawing.Size(275, 130);
			this.mDiskEditor.Name = "mDiskEditor";
			this.mDiskEditor.ReadOnly = false;
			this.mDiskEditor.RecordName = "sector";
			this.mDiskEditor.ResizeInProgress = false;
			this.mDiskEditor.ShowReadOnly = true;
			this.mDiskEditor.Size = new System.Drawing.Size(310, 288);
			this.mDiskEditor.TabIndex = 4;
			// 
			// mCardReaderEditor
			// 
			this.mCardReaderEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardReaderEditor.Location = new System.Drawing.Point(0, 29);
			this.mCardReaderEditor.MinimumSize = new System.Drawing.Size(275, 101);
			this.mCardReaderEditor.Name = "mCardReaderEditor";
			this.mCardReaderEditor.ReadOnly = false;
			this.mCardReaderEditor.RecordName = "card";
			this.mCardReaderEditor.ResizeInProgress = false;
			this.mCardReaderEditor.ShowReadOnly = true;
			this.mCardReaderEditor.Size = new System.Drawing.Size(310, 288);
			this.mCardReaderEditor.TabIndex = 4;
			// 
			// mCardWriterEditor
			// 
			this.mCardWriterEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mCardWriterEditor.Location = new System.Drawing.Point(0, 27);
			this.mCardWriterEditor.MinimumSize = new System.Drawing.Size(275, 101);
			this.mCardWriterEditor.Name = "mCardWriterEditor";
			this.mCardWriterEditor.ReadOnly = true;
			this.mCardWriterEditor.RecordName = "card";
			this.mCardWriterEditor.ResizeInProgress = false;
			this.mCardWriterEditor.ShowReadOnly = false;
			this.mCardWriterEditor.Size = new System.Drawing.Size(310, 290);
			this.mCardWriterEditor.TabIndex = 3;
			// 
			// mPrinterEditor
			// 
			this.mPrinterEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mPrinterEditor.Location = new System.Drawing.Point(0, 27);
			this.mPrinterEditor.MinimumSize = new System.Drawing.Size(275, 101);
			this.mPrinterEditor.Name = "mPrinterEditor";
			this.mPrinterEditor.ReadOnly = true;
			this.mPrinterEditor.RecordName = "line";
			this.mPrinterEditor.ResizeInProgress = false;
			this.mPrinterEditor.ShowReadOnly = false;
			this.mPrinterEditor.Size = new System.Drawing.Size(310, 290);
			this.mPrinterEditor.TabIndex = 2;
			// 
			// mPaperTapeEditor
			// 
			this.mPaperTapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mPaperTapeEditor.Location = new System.Drawing.Point(0, 29);
			this.mPaperTapeEditor.MinimumSize = new System.Drawing.Size(275, 101);
			this.mPaperTapeEditor.Name = "mPaperTapeEditor";
			this.mPaperTapeEditor.ReadOnly = false;
			this.mPaperTapeEditor.RecordName = "record";
			this.mPaperTapeEditor.ResizeInProgress = false;
			this.mPaperTapeEditor.ShowReadOnly = true;
			this.mPaperTapeEditor.Size = new System.Drawing.Size(310, 288);
			this.mPaperTapeEditor.TabIndex = 3;
			// 
			// DeviceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(319, 375);
			this.Controls.Add(this.mCloseButton);
			this.Controls.Add(this.mDeviceTypeTabs);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(292, 246);
			this.Name = "DeviceEditorForm";
			this.Text = "Edit devices";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceEditorForm_FormClosing);
			this.ResizeBegin += new System.EventHandler(this.DeviceEditorForm_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.DeviceEditorForm_ResizeEnd);
			this.mDisksTab.ResumeLayout(false);
			this.mDisksTab.PerformLayout();
			this.mDeviceTypeTabs.ResumeLayout(false);
			this.mTapesTab.ResumeLayout(false);
			this.mTapesTab.PerformLayout();
			this.mCardReaderTab.ResumeLayout(false);
			this.mCardReaderTab.PerformLayout();
			this.mCardPunchTab.ResumeLayout(false);
			this.mCardPunchTab.PerformLayout();
			this.mPrinterTab.ResumeLayout(false);
			this.mPrinterTab.PerformLayout();
			this.mPaperTapeTab.ResumeLayout(false);
			this.mPaperTapeTab.PerformLayout();
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button mCloseButton;
    private System.Windows.Forms.TabPage mDisksTab;
    private System.Windows.Forms.Label mDiskSelectorLabel;
    private System.Windows.Forms.ComboBox mDiskSelectorComboBox;
    private System.Windows.Forms.TabControl mDeviceTypeTabs;
    private BinaryFileDeviceEditor mDiskEditor;
    private System.Windows.Forms.TabPage mTapesTab;
    private System.Windows.Forms.Label mTapeSelectorLabel;
    private System.Windows.Forms.ComboBox mTapeSelectorComboBox;
    private BinaryFileDeviceEditor mTapeEditor;
    private System.Windows.Forms.TabPage mCardReaderTab;
    private TextFileDeviceEditor mCardReaderEditor;
    private System.Windows.Forms.TabPage mCardPunchTab;
    private System.Windows.Forms.TabPage mPrinterTab;
    private System.Windows.Forms.TabPage mPaperTapeTab;
    private TextFileDeviceEditor mCardWriterEditor;
    private TextFileDeviceEditor mPrinterEditor;
    private TextFileDeviceEditor mPaperTapeEditor;
    private System.Windows.Forms.TextBox mTapePathBox;
    private System.Windows.Forms.TextBox mDiskPathBox;
    private System.Windows.Forms.Label mPaperTapePathLabel;
    private System.Windows.Forms.TextBox mPaperTapePathBox;
    private System.Windows.Forms.TextBox mPrinterPathBox;
    private System.Windows.Forms.Label mPrinterPathLabel;
    private System.Windows.Forms.TextBox mCardWriterPathBox;
    private System.Windows.Forms.Label mCardWriterPathLabel;
    private System.Windows.Forms.TextBox mCardReaderPathBox;
    private System.Windows.Forms.Label mCardReaderPathLabel;
		private System.Windows.Forms.Button mTapeDeleteButton;
		private System.Windows.Forms.Button mDiskDeleteButton;
		private System.Windows.Forms.Button mCardReaderDeleteButton;
		private System.Windows.Forms.Button mCardPunchDeleteButton;
		private System.Windows.Forms.Button mPrinterDeleteButton;
		private System.Windows.Forms.Button mPaperTapeDeleteButton;
		private System.Windows.Forms.Button mCardReaderLoadButton;
		private System.Windows.Forms.OpenFileDialog mCardReaderLoadFileDialog;
  }
}