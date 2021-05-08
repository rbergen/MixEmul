namespace MixGui.Components
{
  partial class TextFileDeviceEditor
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			this.components = new System.ComponentModel.Container();
			this.firstRecordLabel = new System.Windows.Forms.Label();
			this.setClipboardLabel = new System.Windows.Forms.Label();
			this.readOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this.readOnlyLabel = new System.Windows.Forms.Label();
			this.revertButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.deviceFileWatcher = new System.IO.FileSystemWatcher();
			this.mixByteCollectionEditorList = new MixGui.Components.MixByteCollectionEditorList();
			this.mixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this.firstRecordTextBox = new MixGui.Components.LongValueTextBox();
			this.ioDelayTimer = new System.Timers.Timer();
			this.appendButton = new System.Windows.Forms.Button();
			this.truncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.deviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mFirstRecordLabel
			// 
			this.firstRecordLabel.AutoSize = true;
			this.firstRecordLabel.Location = new System.Drawing.Point(0, 27);
			this.firstRecordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.firstRecordLabel.Name = "mFirstRecordLabel";
			this.firstRecordLabel.Size = new System.Drawing.Size(49, 15);
			this.firstRecordLabel.TabIndex = 5;
			this.firstRecordLabel.Text = "First {0}:";
			// 
			// mSetClipboardLabel
			// 
			this.setClipboardLabel.AutoSize = true;
			this.setClipboardLabel.Location = new System.Drawing.Point(149, 27);
			this.setClipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.setClipboardLabel.Name = "mSetClipboardLabel";
			this.setClipboardLabel.Size = new System.Drawing.Size(96, 15);
			this.setClipboardLabel.TabIndex = 7;
			this.setClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mReadOnlyCheckBox
			// 
			this.readOnlyCheckBox.AutoSize = true;
			this.readOnlyCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.readOnlyCheckBox.Location = new System.Drawing.Point(84, 1);
			this.readOnlyCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.readOnlyCheckBox.Name = "mReadOnlyCheckBox";
			this.readOnlyCheckBox.Size = new System.Drawing.Size(12, 11);
			this.readOnlyCheckBox.TabIndex = 1;
			this.readOnlyCheckBox.UseVisualStyleBackColor = true;
			this.readOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ReadOnlyCheckBox_CheckedChanged);
			// 
			// mReadOnlyLabel
			// 
			this.readOnlyLabel.AutoSize = true;
			this.readOnlyLabel.Location = new System.Drawing.Point(0, 0);
			this.readOnlyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.readOnlyLabel.Name = "mReadOnlyLabel";
			this.readOnlyLabel.Size = new System.Drawing.Size(67, 15);
			this.readOnlyLabel.TabIndex = 0;
			this.readOnlyLabel.Text = "Read-only: ";
			// 
			// mRevertButton
			// 
			this.revertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.revertButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.revertButton.Location = new System.Drawing.Point(260, 170);
			this.revertButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.revertButton.Name = "mRevertButton";
			this.revertButton.Size = new System.Drawing.Size(64, 27);
			this.revertButton.TabIndex = 10;
			this.revertButton.Text = "&Revert";
			this.revertButton.UseVisualStyleBackColor = true;
			this.revertButton.Click += new System.EventHandler(this.RevertButton_Click);
			// 
			// mSaveButton
			// 
			this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.saveButton.Location = new System.Drawing.Point(189, 170);
			this.saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.saveButton.Name = "mSaveButton";
			this.saveButton.Size = new System.Drawing.Size(64, 27);
			this.saveButton.TabIndex = 10;
			this.saveButton.Text = "&Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// mDeviceFileWatcher
			// 
			this.deviceFileWatcher.EnableRaisingEvents = true;
			this.deviceFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
			this.deviceFileWatcher.SynchronizingObject = this;
			this.deviceFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.deviceFileWatcher.Created += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.deviceFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.deviceFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.DeviceFileWatcher_Renamed);
			// 
			// mMixByteCollectionEditorList
			// 
			this.mixByteCollectionEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mixByteCollectionEditorList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mixByteCollectionEditorList.CreateEditor = null;
			this.mixByteCollectionEditorList.FirstVisibleIndex = 0;
			this.mixByteCollectionEditorList.LoadEditor = null;
			this.mixByteCollectionEditorList.Location = new System.Drawing.Point(0, 50);
			this.mixByteCollectionEditorList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mixByteCollectionEditorList.MaxIndex = 0;
			this.mixByteCollectionEditorList.MinIndex = 0;
			this.mixByteCollectionEditorList.Name = "mMixByteCollectionEditorList";
			this.mixByteCollectionEditorList.ReadOnly = false;
			this.mixByteCollectionEditorList.ResizeInProgress = false;
			this.mixByteCollectionEditorList.Size = new System.Drawing.Size(324, 115);
			this.mixByteCollectionEditorList.TabIndex = 9;
			this.mixByteCollectionEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IMixByteCollectionEditor>.FirstVisibleIndexChangedHandler(this.MixByteCollectionEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this.mixCharButtons.Location = new System.Drawing.Point(251, 23);
			this.mixCharButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mixCharButtons.Name = "mMixCharButtons";
			this.mixCharButtons.Size = new System.Drawing.Size(74, 27);
			this.mixCharButtons.TabIndex = 8;
			// 
			// mFirstRecordTextBox
			// 
			this.firstRecordTextBox.BackColor = System.Drawing.Color.White;
			this.firstRecordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.firstRecordTextBox.ClearZero = true;
			this.firstRecordTextBox.ForeColor = System.Drawing.Color.Black;
			this.firstRecordTextBox.Location = new System.Drawing.Point(83, 24);
			this.firstRecordTextBox.LongValue = ((long)(0));
			this.firstRecordTextBox.Magnitude = ((long)(0));
			this.firstRecordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.firstRecordTextBox.MaxLength = 2;
			this.firstRecordTextBox.MaxValue = ((long)(99));
			this.firstRecordTextBox.MinValue = ((long)(0));
			this.firstRecordTextBox.Name = "mFirstRecordTextBox";
			this.firstRecordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.firstRecordTextBox.Size = new System.Drawing.Size(40, 23);
			this.firstRecordTextBox.SupportNegativeZero = false;
			this.firstRecordTextBox.TabIndex = 6;
			this.firstRecordTextBox.Text = "0";
			this.firstRecordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.FirstRecordTextBox_ValueChanged);
			// 
			// mIODelayTimer
			// 
			this.ioDelayTimer.Interval = 500;
			this.ioDelayTimer.Elapsed += this.IODelayTimer_Tick;
			// 
			// mAppendButton
			// 
			this.appendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.appendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.appendButton.Location = new System.Drawing.Point(0, 170);
			this.appendButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.appendButton.Name = "mAppendButton";
			this.appendButton.Size = new System.Drawing.Size(71, 27);
			this.appendButton.TabIndex = 11;
			this.appendButton.Text = "A&ppend";
			this.appendButton.UseVisualStyleBackColor = true;
			this.appendButton.Click += new System.EventHandler(this.AppendButton_Click);
			// 
			// mTruncateButton
			// 
			this.truncateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.truncateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.truncateButton.Location = new System.Drawing.Point(78, 170);
			this.truncateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.truncateButton.Name = "mTruncateButton";
			this.truncateButton.Size = new System.Drawing.Size(71, 27);
			this.truncateButton.TabIndex = 11;
			this.truncateButton.Text = "&Truncate";
			this.truncateButton.UseVisualStyleBackColor = true;
			this.truncateButton.Click += new System.EventHandler(this.TruncateButton_Click);
			// 
			// TextFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.truncateButton);
			this.Controls.Add(this.appendButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.revertButton);
			this.Controls.Add(this.readOnlyLabel);
			this.Controls.Add(this.readOnlyCheckBox);
			this.Controls.Add(this.mixByteCollectionEditorList);
			this.Controls.Add(this.mixCharButtons);
			this.Controls.Add(this.setClipboardLabel);
			this.Controls.Add(this.firstRecordTextBox);
			this.Controls.Add(this.firstRecordLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "TextFileDeviceEditor";
			this.Size = new System.Drawing.Size(324, 197);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.deviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label firstRecordLabel;
    private global::MixGui.Components.LongValueTextBox firstRecordTextBox;
    private MixCharClipboardButtonControl mixCharButtons;
    private System.Windows.Forms.Label setClipboardLabel;
    private MixByteCollectionEditorList mixByteCollectionEditorList;
    private System.Windows.Forms.CheckBox readOnlyCheckBox;
    private System.Windows.Forms.Label readOnlyLabel;
    private System.Windows.Forms.Button revertButton;
    private System.Windows.Forms.Button saveButton;
    private System.IO.FileSystemWatcher deviceFileWatcher;
    private System.Timers.Timer ioDelayTimer;
    private System.Windows.Forms.Button truncateButton;
    private System.Windows.Forms.Button appendButton;
  }
}
