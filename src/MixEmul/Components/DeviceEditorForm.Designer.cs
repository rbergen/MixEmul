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
			this._closeButton = new System.Windows.Forms.Button();
			this._disksTab = new System.Windows.Forms.TabPage();
			this._diskDeleteButton = new System.Windows.Forms.Button();
			this._diskPathBox = new System.Windows.Forms.TextBox();
			this._diskSelectorLabel = new System.Windows.Forms.Label();
			this._diskSelectorComboBox = new System.Windows.Forms.ComboBox();
			this._diskEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this._deviceTypeTabs = new System.Windows.Forms.TabControl();
			this._tapesTab = new System.Windows.Forms.TabPage();
			this._tapeDeleteButton = new System.Windows.Forms.Button();
			this._tapePathBox = new System.Windows.Forms.TextBox();
			this._tapeEditor = new MixGui.Components.BinaryFileDeviceEditor();
			this._tapeSelectorComboBox = new System.Windows.Forms.ComboBox();
			this._tapeSelectorLabel = new System.Windows.Forms.Label();
			this._cardReaderTab = new System.Windows.Forms.TabPage();
			this._cardReaderLoadButton = new System.Windows.Forms.Button();
			this._cardReaderDeleteButton = new System.Windows.Forms.Button();
			this._cardReaderPathBox = new System.Windows.Forms.TextBox();
			this._cardReaderPathLabel = new System.Windows.Forms.Label();
			this._cardReaderEditor = new MixGui.Components.TextFileDeviceEditor();
			this._cardPunchTab = new System.Windows.Forms.TabPage();
			this._cardPunchDeleteButton = new System.Windows.Forms.Button();
			this._cardWriterPathBox = new System.Windows.Forms.TextBox();
			this._cardWriterPathLabel = new System.Windows.Forms.Label();
			this._cardWriterEditor = new MixGui.Components.TextFileDeviceEditor();
			this._printerTab = new System.Windows.Forms.TabPage();
			this._printerDeleteButton = new System.Windows.Forms.Button();
			this._printerPathBox = new System.Windows.Forms.TextBox();
			this._printerPathLabel = new System.Windows.Forms.Label();
			this._printerEditor = new MixGui.Components.TextFileDeviceEditor();
			this._paperTapeTab = new System.Windows.Forms.TabPage();
			this._paperTapeDeleteButton = new System.Windows.Forms.Button();
			this._paperTapePathBox = new System.Windows.Forms.TextBox();
			this._paperTapePathLabel = new System.Windows.Forms.Label();
			this._paperTapeEditor = new MixGui.Components.TextFileDeviceEditor();
			this._cardReaderLoadFileDialog = new System.Windows.Forms.OpenFileDialog();
			tapePathColonLabel = new System.Windows.Forms.Label();
			diskPathColonLabel = new System.Windows.Forms.Label();
			this._disksTab.SuspendLayout();
			this._deviceTypeTabs.SuspendLayout();
			this._tapesTab.SuspendLayout();
			this._cardReaderTab.SuspendLayout();
			this._cardPunchTab.SuspendLayout();
			this._printerTab.SuspendLayout();
			this._paperTapeTab.SuspendLayout();
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
			this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._closeButton.Location = new System.Drawing.Point(444, 400);
			this._closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._closeButton.Name = "mCloseButton";
			this._closeButton.Size = new System.Drawing.Size(65, 27);
			this._closeButton.TabIndex = 1;
			this._closeButton.Text = "&Close";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// mDisksTab
			// 
			this._disksTab.Controls.Add(this._diskDeleteButton);
			this._disksTab.Controls.Add(diskPathColonLabel);
			this._disksTab.Controls.Add(this._diskPathBox);
			this._disksTab.Controls.Add(this._diskSelectorLabel);
			this._disksTab.Controls.Add(this._diskSelectorComboBox);
			this._disksTab.Controls.Add(this._diskEditor);
			this._disksTab.Location = new System.Drawing.Point(4, 27);
			this._disksTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._disksTab.Name = "mDisksTab";
			this._disksTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._disksTab.Size = new System.Drawing.Size(504, 365);
			this._disksTab.TabIndex = 0;
			this._disksTab.Text = "Disks";
			this._disksTab.UseVisualStyleBackColor = true;
			// 
			// mDiskDeleteButton
			// 
			this._diskDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._diskDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._diskDeleteButton.Location = new System.Drawing.Point(439, 1);
			this._diskDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._diskDeleteButton.Name = "mDiskDeleteButton";
			this._diskDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._diskDeleteButton.TabIndex = 3;
			this._diskDeleteButton.Text = "&Delete";
			this._diskDeleteButton.UseVisualStyleBackColor = true;
			this._diskDeleteButton.Click += new System.EventHandler(this.DiskDeleteButton_Click);
			// 
			// mDiskPathBox
			// 
			this._diskPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._diskPathBox.BackColor = System.Drawing.SystemColors.Control;
			this._diskPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._diskPathBox.Location = new System.Drawing.Point(174, 7);
			this._diskPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._diskPathBox.Name = "mDiskPathBox";
			this._diskPathBox.ReadOnly = true;
			this._diskPathBox.Size = new System.Drawing.Size(258, 16);
			this._diskPathBox.TabIndex = 2;
			// 
			// mDiskSelectorLabel
			// 
			this._diskSelectorLabel.AutoSize = true;
			this._diskSelectorLabel.Location = new System.Drawing.Point(0, 7);
			this._diskSelectorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._diskSelectorLabel.Name = "mDiskSelectorLabel";
			this._diskSelectorLabel.Size = new System.Drawing.Size(54, 15);
			this._diskSelectorLabel.TabIndex = 0;
			this._diskSelectorLabel.Text = "Edit disk:";
			// 
			// mDiskSelectorComboBox
			// 
			this._diskSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._diskSelectorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._diskSelectorComboBox.FormattingEnabled = true;
			this._diskSelectorComboBox.Location = new System.Drawing.Point(84, 2);
			this._diskSelectorComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._diskSelectorComboBox.Name = "mDiskSelectorComboBox";
			this._diskSelectorComboBox.Size = new System.Drawing.Size(78, 23);
			this._diskSelectorComboBox.TabIndex = 1;
			this._diskSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.DiskSelectorComboBox_SelectedIndexChanged);
			// 
			// mDiskEditor
			// 
			this._diskEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._diskEditor.Location = new System.Drawing.Point(0, 33);
			this._diskEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._diskEditor.MinimumSize = new System.Drawing.Size(374, 175);
			this._diskEditor.Name = "mDiskEditor";
			this._diskEditor.ReadOnly = false;
			this._diskEditor.RecordName = "sector";
			this._diskEditor.ResizeInProgress = false;
			this._diskEditor.ShowReadOnly = true;
			this._diskEditor.Size = new System.Drawing.Size(504, 332);
			this._diskEditor.TabIndex = 4;
			// 
			// mDeviceTypeTabs
			// 
			this._deviceTypeTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._deviceTypeTabs.Controls.Add(this._tapesTab);
			this._deviceTypeTabs.Controls.Add(this._disksTab);
			this._deviceTypeTabs.Controls.Add(this._cardReaderTab);
			this._deviceTypeTabs.Controls.Add(this._cardPunchTab);
			this._deviceTypeTabs.Controls.Add(this._printerTab);
			this._deviceTypeTabs.Controls.Add(this._paperTapeTab);
			this._deviceTypeTabs.Location = new System.Drawing.Point(1, 1);
			this._deviceTypeTabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._deviceTypeTabs.Name = "mDeviceTypeTabs";
			this._deviceTypeTabs.SelectedIndex = 0;
			this._deviceTypeTabs.Size = new System.Drawing.Size(512, 396);
			this._deviceTypeTabs.TabIndex = 0;
			this._deviceTypeTabs.SelectedIndexChanged += new System.EventHandler(this.DeviceTypeTabs_SelectedIndexChanged);
			// 
			// mTapesTab
			// 
			this._tapesTab.Controls.Add(this._tapeDeleteButton);
			this._tapesTab.Controls.Add(tapePathColonLabel);
			this._tapesTab.Controls.Add(this._tapePathBox);
			this._tapesTab.Controls.Add(this._tapeEditor);
			this._tapesTab.Controls.Add(this._tapeSelectorComboBox);
			this._tapesTab.Controls.Add(this._tapeSelectorLabel);
			this._tapesTab.Location = new System.Drawing.Point(4, 27);
			this._tapesTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._tapesTab.Name = "mTapesTab";
			this._tapesTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._tapesTab.Size = new System.Drawing.Size(504, 365);
			this._tapesTab.TabIndex = 1;
			this._tapesTab.Text = "Tapes";
			this._tapesTab.UseVisualStyleBackColor = true;
			// 
			// mTapeDeleteButton
			// 
			this._tapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._tapeDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._tapeDeleteButton.Location = new System.Drawing.Point(441, 1);
			this._tapeDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._tapeDeleteButton.Name = "mTapeDeleteButton";
			this._tapeDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._tapeDeleteButton.TabIndex = 3;
			this._tapeDeleteButton.Text = "&Delete";
			this._tapeDeleteButton.UseVisualStyleBackColor = true;
			this._tapeDeleteButton.Click += new System.EventHandler(this.TapeDeleteButton_Click);
			// 
			// mTapePathBox
			// 
			this._tapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tapePathBox.BackColor = System.Drawing.SystemColors.Control;
			this._tapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._tapePathBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._tapePathBox.Location = new System.Drawing.Point(174, 7);
			this._tapePathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._tapePathBox.Name = "mTapePathBox";
			this._tapePathBox.ReadOnly = true;
			this._tapePathBox.Size = new System.Drawing.Size(262, 16);
			this._tapePathBox.TabIndex = 2;
			// 
			// mTapeEditor
			// 
			this._tapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tapeEditor.Location = new System.Drawing.Point(0, 33);
			this._tapeEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._tapeEditor.MinimumSize = new System.Drawing.Size(374, 175);
			this._tapeEditor.Name = "mTapeEditor";
			this._tapeEditor.ReadOnly = false;
			this._tapeEditor.RecordName = "record";
			this._tapeEditor.ResizeInProgress = false;
			this._tapeEditor.ShowReadOnly = true;
			this._tapeEditor.Size = new System.Drawing.Size(504, 332);
			this._tapeEditor.TabIndex = 4;
			// 
			// mTapeSelectorComboBox
			// 
			this._tapeSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._tapeSelectorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._tapeSelectorComboBox.FormattingEnabled = true;
			this._tapeSelectorComboBox.Location = new System.Drawing.Point(84, 2);
			this._tapeSelectorComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._tapeSelectorComboBox.Name = "mTapeSelectorComboBox";
			this._tapeSelectorComboBox.Size = new System.Drawing.Size(78, 23);
			this._tapeSelectorComboBox.TabIndex = 1;
			this._tapeSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.TapeSelectorComboBox_SelectedIndexChanged);
			// 
			// mTapeSelectorLabel
			// 
			this._tapeSelectorLabel.AutoSize = true;
			this._tapeSelectorLabel.Location = new System.Drawing.Point(0, 7);
			this._tapeSelectorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._tapeSelectorLabel.Name = "mTapeSelectorLabel";
			this._tapeSelectorLabel.Size = new System.Drawing.Size(56, 15);
			this._tapeSelectorLabel.TabIndex = 0;
			this._tapeSelectorLabel.Text = "Edit tape:";
			// 
			// mCardReaderTab
			// 
			this._cardReaderTab.Controls.Add(this._cardReaderLoadButton);
			this._cardReaderTab.Controls.Add(this._cardReaderDeleteButton);
			this._cardReaderTab.Controls.Add(this._cardReaderPathBox);
			this._cardReaderTab.Controls.Add(this._cardReaderPathLabel);
			this._cardReaderTab.Controls.Add(this._cardReaderEditor);
			this._cardReaderTab.Location = new System.Drawing.Point(4, 27);
			this._cardReaderTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardReaderTab.Name = "mCardReaderTab";
			this._cardReaderTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardReaderTab.Size = new System.Drawing.Size(504, 365);
			this._cardReaderTab.TabIndex = 2;
			this._cardReaderTab.Text = "Card Reader";
			this._cardReaderTab.UseVisualStyleBackColor = true;
			// 
			// mCardReaderLoadButton
			// 
			this._cardReaderLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cardReaderLoadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._cardReaderLoadButton.Location = new System.Drawing.Point(368, 1);
			this._cardReaderLoadButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardReaderLoadButton.Name = "mCardReaderLoadButton";
			this._cardReaderLoadButton.Size = new System.Drawing.Size(63, 27);
			this._cardReaderLoadButton.TabIndex = 2;
			this._cardReaderLoadButton.Text = "&Load...";
			this._cardReaderLoadButton.UseVisualStyleBackColor = true;
			this._cardReaderLoadButton.Click += new System.EventHandler(this.CardReaderLoadButton_Click);
			// 
			// mCardReaderDeleteButton
			// 
			this._cardReaderDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cardReaderDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._cardReaderDeleteButton.Location = new System.Drawing.Point(439, 1);
			this._cardReaderDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardReaderDeleteButton.Name = "mCardReaderDeleteButton";
			this._cardReaderDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._cardReaderDeleteButton.TabIndex = 3;
			this._cardReaderDeleteButton.Text = "&Delete";
			this._cardReaderDeleteButton.UseVisualStyleBackColor = true;
			this._cardReaderDeleteButton.Click += new System.EventHandler(this.CardReaderDeleteButton_Click);
			// 
			// mCardReaderPathBox
			// 
			this._cardReaderPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cardReaderPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._cardReaderPathBox.Location = new System.Drawing.Point(84, 7);
			this._cardReaderPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardReaderPathBox.Name = "mCardReaderPathBox";
			this._cardReaderPathBox.ReadOnly = true;
			this._cardReaderPathBox.Size = new System.Drawing.Size(277, 16);
			this._cardReaderPathBox.TabIndex = 1;
			// 
			// mCardReaderPathLabel
			// 
			this._cardReaderPathLabel.AutoSize = true;
			this._cardReaderPathLabel.Location = new System.Drawing.Point(0, 7);
			this._cardReaderPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._cardReaderPathLabel.Name = "mCardReaderPathLabel";
			this._cardReaderPathLabel.Size = new System.Drawing.Size(55, 15);
			this._cardReaderPathLabel.TabIndex = 0;
			this._cardReaderPathLabel.Text = "File path:";
			// 
			// mCardReaderEditor
			// 
			this._cardReaderEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cardReaderEditor.Location = new System.Drawing.Point(0, 33);
			this._cardReaderEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._cardReaderEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this._cardReaderEditor.Name = "mCardReaderEditor";
			this._cardReaderEditor.ReadOnly = false;
			this._cardReaderEditor.RecordName = "card";
			this._cardReaderEditor.ResizeInProgress = false;
			this._cardReaderEditor.ShowReadOnly = true;
			this._cardReaderEditor.Size = new System.Drawing.Size(504, 332);
			this._cardReaderEditor.TabIndex = 4;
			// 
			// mCardPunchTab
			// 
			this._cardPunchTab.Controls.Add(this._cardPunchDeleteButton);
			this._cardPunchTab.Controls.Add(this._cardWriterPathBox);
			this._cardPunchTab.Controls.Add(this._cardWriterPathLabel);
			this._cardPunchTab.Controls.Add(this._cardWriterEditor);
			this._cardPunchTab.Location = new System.Drawing.Point(4, 27);
			this._cardPunchTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardPunchTab.Name = "mCardPunchTab";
			this._cardPunchTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardPunchTab.Size = new System.Drawing.Size(504, 365);
			this._cardPunchTab.TabIndex = 3;
			this._cardPunchTab.Text = "Card Punch";
			this._cardPunchTab.UseVisualStyleBackColor = true;
			// 
			// mCardPunchDeleteButton
			// 
			this._cardPunchDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cardPunchDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._cardPunchDeleteButton.Location = new System.Drawing.Point(439, 1);
			this._cardPunchDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardPunchDeleteButton.Name = "mCardPunchDeleteButton";
			this._cardPunchDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._cardPunchDeleteButton.TabIndex = 2;
			this._cardPunchDeleteButton.Text = "&Delete";
			this._cardPunchDeleteButton.UseVisualStyleBackColor = true;
			this._cardPunchDeleteButton.Click += new System.EventHandler(this.CardPunchDeleteButton_Click);
			// 
			// mCardWriterPathBox
			// 
			this._cardWriterPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cardWriterPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._cardWriterPathBox.Location = new System.Drawing.Point(84, 7);
			this._cardWriterPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cardWriterPathBox.Name = "mCardWriterPathBox";
			this._cardWriterPathBox.ReadOnly = true;
			this._cardWriterPathBox.Size = new System.Drawing.Size(347, 16);
			this._cardWriterPathBox.TabIndex = 1;
			// 
			// mCardWriterPathLabel
			// 
			this._cardWriterPathLabel.AutoSize = true;
			this._cardWriterPathLabel.Location = new System.Drawing.Point(0, 7);
			this._cardWriterPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._cardWriterPathLabel.Name = "mCardWriterPathLabel";
			this._cardWriterPathLabel.Size = new System.Drawing.Size(55, 15);
			this._cardWriterPathLabel.TabIndex = 0;
			this._cardWriterPathLabel.Text = "File path:";
			// 
			// mCardWriterEditor
			// 
			this._cardWriterEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cardWriterEditor.Location = new System.Drawing.Point(0, 31);
			this._cardWriterEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._cardWriterEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this._cardWriterEditor.Name = "mCardWriterEditor";
			this._cardWriterEditor.ReadOnly = true;
			this._cardWriterEditor.RecordName = "card";
			this._cardWriterEditor.ResizeInProgress = false;
			this._cardWriterEditor.ShowReadOnly = false;
			this._cardWriterEditor.Size = new System.Drawing.Size(502, 332);
			this._cardWriterEditor.TabIndex = 3;
			// 
			// mPrinterTab
			// 
			this._printerTab.Controls.Add(this._printerDeleteButton);
			this._printerTab.Controls.Add(this._printerPathBox);
			this._printerTab.Controls.Add(this._printerPathLabel);
			this._printerTab.Controls.Add(this._printerEditor);
			this._printerTab.Location = new System.Drawing.Point(4, 27);
			this._printerTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._printerTab.Name = "mPrinterTab";
			this._printerTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._printerTab.Size = new System.Drawing.Size(504, 365);
			this._printerTab.TabIndex = 4;
			this._printerTab.Text = "Printer";
			this._printerTab.UseVisualStyleBackColor = true;
			// 
			// mPrinterDeleteButton
			// 
			this._printerDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._printerDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._printerDeleteButton.Location = new System.Drawing.Point(439, 1);
			this._printerDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._printerDeleteButton.Name = "mPrinterDeleteButton";
			this._printerDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._printerDeleteButton.TabIndex = 3;
			this._printerDeleteButton.Text = "&Delete";
			this._printerDeleteButton.UseVisualStyleBackColor = true;
			this._printerDeleteButton.Click += new System.EventHandler(this.PrinterDeleteButton_Click);
			// 
			// mPrinterPathBox
			// 
			this._printerPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._printerPathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._printerPathBox.Location = new System.Drawing.Point(84, 7);
			this._printerPathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._printerPathBox.Name = "mPrinterPathBox";
			this._printerPathBox.ReadOnly = true;
			this._printerPathBox.Size = new System.Drawing.Size(347, 16);
			this._printerPathBox.TabIndex = 1;
			// 
			// mPrinterPathLabel
			// 
			this._printerPathLabel.AutoSize = true;
			this._printerPathLabel.Location = new System.Drawing.Point(0, 7);
			this._printerPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._printerPathLabel.Name = "mPrinterPathLabel";
			this._printerPathLabel.Size = new System.Drawing.Size(55, 15);
			this._printerPathLabel.TabIndex = 0;
			this._printerPathLabel.Text = "File path:";
			// 
			// mPrinterEditor
			// 
			this._printerEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._printerEditor.Location = new System.Drawing.Point(0, 31);
			this._printerEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._printerEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this._printerEditor.Name = "mPrinterEditor";
			this._printerEditor.ReadOnly = true;
			this._printerEditor.RecordName = "line";
			this._printerEditor.ResizeInProgress = false;
			this._printerEditor.ShowReadOnly = false;
			this._printerEditor.Size = new System.Drawing.Size(502, 332);
			this._printerEditor.TabIndex = 2;
			// 
			// mPaperTapeTab
			// 
			this._paperTapeTab.Controls.Add(this._paperTapeDeleteButton);
			this._paperTapeTab.Controls.Add(this._paperTapePathBox);
			this._paperTapeTab.Controls.Add(this._paperTapePathLabel);
			this._paperTapeTab.Controls.Add(this._paperTapeEditor);
			this._paperTapeTab.Location = new System.Drawing.Point(4, 27);
			this._paperTapeTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._paperTapeTab.Name = "mPaperTapeTab";
			this._paperTapeTab.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._paperTapeTab.Size = new System.Drawing.Size(504, 365);
			this._paperTapeTab.TabIndex = 5;
			this._paperTapeTab.Text = "Paper Tape";
			this._paperTapeTab.UseVisualStyleBackColor = true;
			// 
			// mPaperTapeDeleteButton
			// 
			this._paperTapeDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._paperTapeDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._paperTapeDeleteButton.Location = new System.Drawing.Point(439, 1);
			this._paperTapeDeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._paperTapeDeleteButton.Name = "mPaperTapeDeleteButton";
			this._paperTapeDeleteButton.Size = new System.Drawing.Size(63, 27);
			this._paperTapeDeleteButton.TabIndex = 2;
			this._paperTapeDeleteButton.Text = "&Delete";
			this._paperTapeDeleteButton.UseVisualStyleBackColor = true;
			this._paperTapeDeleteButton.Click += new System.EventHandler(this.PaperTapeDeleteButton_Click);
			// 
			// mPaperTapePathBox
			// 
			this._paperTapePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._paperTapePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._paperTapePathBox.Location = new System.Drawing.Point(84, 7);
			this._paperTapePathBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._paperTapePathBox.Name = "mPaperTapePathBox";
			this._paperTapePathBox.ReadOnly = true;
			this._paperTapePathBox.Size = new System.Drawing.Size(347, 16);
			this._paperTapePathBox.TabIndex = 1;
			// 
			// mPaperTapePathLabel
			// 
			this._paperTapePathLabel.AutoSize = true;
			this._paperTapePathLabel.Location = new System.Drawing.Point(4, 8);
			this._paperTapePathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._paperTapePathLabel.Name = "mPaperTapePathLabel";
			this._paperTapePathLabel.Size = new System.Drawing.Size(55, 15);
			this._paperTapePathLabel.TabIndex = 0;
			this._paperTapePathLabel.Text = "File path:";
			// 
			// mPaperTapeEditor
			// 
			this._paperTapeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._paperTapeEditor.Location = new System.Drawing.Point(4, 33);
			this._paperTapeEditor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this._paperTapeEditor.MinimumSize = new System.Drawing.Size(374, 227);
			this._paperTapeEditor.Name = "mPaperTapeEditor";
			this._paperTapeEditor.ReadOnly = false;
			this._paperTapeEditor.RecordName = "record";
			this._paperTapeEditor.ResizeInProgress = false;
			this._paperTapeEditor.ShowReadOnly = true;
			this._paperTapeEditor.Size = new System.Drawing.Size(498, 332);
			this._paperTapeEditor.TabIndex = 3;
			// 
			// mCardReaderLoadFileDialog
			// 
			this._cardReaderLoadFileDialog.DefaultExt = "mixdeck";
			this._cardReaderLoadFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			// 
			// DeviceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(517, 433);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this._deviceTypeTabs);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeviceEditorForm";
			this.Text = "Edit devices";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceEditorForm_FormClosing);
			this.ResizeBegin += new System.EventHandler(this.DeviceEditorForm_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.DeviceEditorForm_ResizeEnd);
			this._disksTab.ResumeLayout(false);
			this._disksTab.PerformLayout();
			this._deviceTypeTabs.ResumeLayout(false);
			this._tapesTab.ResumeLayout(false);
			this._tapesTab.PerformLayout();
			this._cardReaderTab.ResumeLayout(false);
			this._cardReaderTab.PerformLayout();
			this._cardPunchTab.ResumeLayout(false);
			this._cardPunchTab.PerformLayout();
			this._printerTab.ResumeLayout(false);
			this._printerTab.PerformLayout();
			this._paperTapeTab.ResumeLayout(false);
			this._paperTapeTab.PerformLayout();
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button _closeButton;
    private System.Windows.Forms.TabPage _disksTab;
    private System.Windows.Forms.Label _diskSelectorLabel;
    private System.Windows.Forms.ComboBox _diskSelectorComboBox;
    private System.Windows.Forms.TabControl _deviceTypeTabs;
    private BinaryFileDeviceEditor _diskEditor;
    private System.Windows.Forms.TabPage _tapesTab;
    private System.Windows.Forms.Label _tapeSelectorLabel;
    private System.Windows.Forms.ComboBox _tapeSelectorComboBox;
    private BinaryFileDeviceEditor _tapeEditor;
    private System.Windows.Forms.TabPage _cardReaderTab;
    private TextFileDeviceEditor _cardReaderEditor;
    private System.Windows.Forms.TabPage _cardPunchTab;
    private System.Windows.Forms.TabPage _printerTab;
    private System.Windows.Forms.TabPage _paperTapeTab;
    private TextFileDeviceEditor _cardWriterEditor;
    private TextFileDeviceEditor _printerEditor;
    private TextFileDeviceEditor _paperTapeEditor;
    private System.Windows.Forms.TextBox _tapePathBox;
    private System.Windows.Forms.TextBox _diskPathBox;
    private System.Windows.Forms.Label _paperTapePathLabel;
    private System.Windows.Forms.TextBox _paperTapePathBox;
    private System.Windows.Forms.TextBox _printerPathBox;
    private System.Windows.Forms.Label _printerPathLabel;
    private System.Windows.Forms.TextBox _cardWriterPathBox;
    private System.Windows.Forms.Label _cardWriterPathLabel;
    private System.Windows.Forms.TextBox _cardReaderPathBox;
    private System.Windows.Forms.Label _cardReaderPathLabel;
		private System.Windows.Forms.Button _tapeDeleteButton;
		private System.Windows.Forms.Button _diskDeleteButton;
		private System.Windows.Forms.Button _cardReaderDeleteButton;
		private System.Windows.Forms.Button _cardPunchDeleteButton;
		private System.Windows.Forms.Button _printerDeleteButton;
		private System.Windows.Forms.Button _paperTapeDeleteButton;
		private System.Windows.Forms.Button _cardReaderLoadButton;
		private System.Windows.Forms.OpenFileDialog _cardReaderLoadFileDialog;
  }
}
