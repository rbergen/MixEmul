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
      if (disposing)
      {
        components?.Dispose();
      }
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
			this.mFirstRecordLabel = new System.Windows.Forms.Label();
			this.mSetClipboardLabel = new System.Windows.Forms.Label();
			this.mReadOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this.mReadOnlyLabel = new System.Windows.Forms.Label();
			this.mRevertButton = new System.Windows.Forms.Button();
			this.mSaveButton = new System.Windows.Forms.Button();
			this.mDeviceFileWatcher = new System.IO.FileSystemWatcher();
			this.mMixByteCollectionEditorList = new MixGui.Components.MixByteCollectionEditorList();
			this.mMixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this.mFirstRecordTextBox = new MixGui.Components.LongValueTextBox();
			this.mIODelayTimer = new System.Windows.Forms.Timer(this.components);
			this.mAppendButton = new System.Windows.Forms.Button();
			this.mTruncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.mDeviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mFirstRecordLabel
			// 
			this.mFirstRecordLabel.AutoSize = true;
			this.mFirstRecordLabel.Location = new System.Drawing.Point(0, 23);
			this.mFirstRecordLabel.Name = "mFirstRecordLabel";
			this.mFirstRecordLabel.Size = new System.Drawing.Size(46, 13);
			this.mFirstRecordLabel.TabIndex = 5;
			this.mFirstRecordLabel.Text = "First {0}:";
			// 
			// mSetClipboardLabel
			// 
			this.mSetClipboardLabel.AutoSize = true;
			this.mSetClipboardLabel.Location = new System.Drawing.Point(128, 23);
			this.mSetClipboardLabel.Name = "mSetClipboardLabel";
			this.mSetClipboardLabel.Size = new System.Drawing.Size(87, 13);
			this.mSetClipboardLabel.TabIndex = 7;
			this.mSetClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mReadOnlyCheckBox
			// 
			this.mReadOnlyCheckBox.AutoSize = true;
			this.mReadOnlyCheckBox.Location = new System.Drawing.Point(72, 1);
			this.mReadOnlyCheckBox.Name = "mReadOnlyCheckBox";
			this.mReadOnlyCheckBox.Size = new System.Drawing.Size(15, 14);
			this.mReadOnlyCheckBox.TabIndex = 1;
			this.mReadOnlyCheckBox.UseVisualStyleBackColor = true;
			this.mReadOnlyCheckBox.CheckedChanged += new System.EventHandler(this.mReadOnlyCheckBox_CheckedChanged);
			// 
			// mReadOnlyLabel
			// 
			this.mReadOnlyLabel.AutoSize = true;
			this.mReadOnlyLabel.Location = new System.Drawing.Point(0, 0);
			this.mReadOnlyLabel.Name = "mReadOnlyLabel";
			this.mReadOnlyLabel.Size = new System.Drawing.Size(61, 13);
			this.mReadOnlyLabel.TabIndex = 0;
			this.mReadOnlyLabel.Text = "Read-only: ";
			// 
			// mRevertButton
			// 
			this.mRevertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mRevertButton.Location = new System.Drawing.Point(223, 80);
			this.mRevertButton.Name = "mRevertButton";
			this.mRevertButton.Size = new System.Drawing.Size(55, 23);
			this.mRevertButton.TabIndex = 10;
			this.mRevertButton.Text = "&Revert";
			this.mRevertButton.UseVisualStyleBackColor = true;
			this.mRevertButton.Click += new System.EventHandler(this.mRevertButton_Click);
			// 
			// mSaveButton
			// 
			this.mSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mSaveButton.Location = new System.Drawing.Point(162, 80);
			this.mSaveButton.Name = "mSaveButton";
			this.mSaveButton.Size = new System.Drawing.Size(55, 23);
			this.mSaveButton.TabIndex = 10;
			this.mSaveButton.Text = "&Save";
			this.mSaveButton.UseVisualStyleBackColor = true;
			this.mSaveButton.Click += new System.EventHandler(this.mSaveButton_Click);
			// 
			// mDeviceFileWatcher
			// 
			this.mDeviceFileWatcher.EnableRaisingEvents = true;
			this.mDeviceFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
			this.mDeviceFileWatcher.SynchronizingObject = this;
			this.mDeviceFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.mDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Created += new System.IO.FileSystemEventHandler(this.mDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.mDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.mDeviceFileWatcher_Renamed);
			// 
			// mMixByteCollectionEditorList
			// 
			this.mMixByteCollectionEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mMixByteCollectionEditorList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mMixByteCollectionEditorList.CreateEditor = null;
			this.mMixByteCollectionEditorList.FirstVisibleIndex = 0;
			this.mMixByteCollectionEditorList.LoadEditor = null;
			this.mMixByteCollectionEditorList.Location = new System.Drawing.Point(0, 43);
			this.mMixByteCollectionEditorList.MaxIndex = -1;
			this.mMixByteCollectionEditorList.Name = "mMixByteCollectionEditorList";
			this.mMixByteCollectionEditorList.ReadOnly = false;
			this.mMixByteCollectionEditorList.ResizeInProgress = false;
			this.mMixByteCollectionEditorList.Size = new System.Drawing.Size(278, 32);
			this.mMixByteCollectionEditorList.TabIndex = 9;
			this.mMixByteCollectionEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IMixByteCollectionEditor>.FirstVisibleIndexChangedHandler(this.mMixByteCollectionEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this.mMixCharButtons.Location = new System.Drawing.Point(215, 20);
			this.mMixCharButtons.Name = "mMixCharButtons";
			this.mMixCharButtons.Size = new System.Drawing.Size(63, 23);
			this.mMixCharButtons.TabIndex = 8;
			// 
			// mFirstRecordTextBox
			// 
			this.mFirstRecordTextBox.BackColor = System.Drawing.Color.White;
			this.mFirstRecordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mFirstRecordTextBox.ForeColor = System.Drawing.Color.Black;
			this.mFirstRecordTextBox.Location = new System.Drawing.Point(71, 21);
			this.mFirstRecordTextBox.LongValue = ((long)(0));
			this.mFirstRecordTextBox.Magnitude = ((long)(0));
			this.mFirstRecordTextBox.MaxLength = 2;
			this.mFirstRecordTextBox.MaxValue = ((long)(99));
			this.mFirstRecordTextBox.MinValue = ((long)(0));
			this.mFirstRecordTextBox.Name = "mFirstRecordTextBox";
			this.mFirstRecordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mFirstRecordTextBox.Size = new System.Drawing.Size(35, 20);
			this.mFirstRecordTextBox.SupportNegativeZero = false;
			this.mFirstRecordTextBox.TabIndex = 6;
			this.mFirstRecordTextBox.Text = "0";
			this.mFirstRecordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.mFirstRecordTextBox_ValueChanged);
			// 
			// mIODelayTimer
			// 
			this.mIODelayTimer.Interval = 500;
			this.mIODelayTimer.Tick += new System.EventHandler(this.mIODelayTimer_Tick);
			// 
			// mAppendButton
			// 
			this.mAppendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mAppendButton.Location = new System.Drawing.Point(0, 80);
			this.mAppendButton.Name = "mAppendButton";
			this.mAppendButton.Size = new System.Drawing.Size(61, 23);
			this.mAppendButton.TabIndex = 11;
			this.mAppendButton.Text = "A&ppend";
			this.mAppendButton.UseVisualStyleBackColor = true;
			this.mAppendButton.Click += new System.EventHandler(this.mAppendButton_Click);
			// 
			// mTruncateButton
			// 
			this.mTruncateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTruncateButton.Location = new System.Drawing.Point(67, 80);
			this.mTruncateButton.Name = "mTruncateButton";
			this.mTruncateButton.Size = new System.Drawing.Size(61, 23);
			this.mTruncateButton.TabIndex = 11;
			this.mTruncateButton.Text = "&Truncate";
			this.mTruncateButton.UseVisualStyleBackColor = true;
			this.mTruncateButton.Click += new System.EventHandler(this.mTruncateButton_Click);
			// 
			// TextFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mTruncateButton);
			this.Controls.Add(this.mAppendButton);
			this.Controls.Add(this.mSaveButton);
			this.Controls.Add(this.mRevertButton);
			this.Controls.Add(this.mReadOnlyLabel);
			this.Controls.Add(this.mReadOnlyCheckBox);
			this.Controls.Add(this.mMixByteCollectionEditorList);
			this.Controls.Add(this.mMixCharButtons);
			this.Controls.Add(this.mSetClipboardLabel);
			this.Controls.Add(this.mFirstRecordTextBox);
			this.Controls.Add(this.mFirstRecordLabel);
			this.MinimumSize = new System.Drawing.Size(275, 103);
			this.Name = "TextFileDeviceEditor";
			this.Size = new System.Drawing.Size(278, 103);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.mDeviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label mFirstRecordLabel;
    private global::MixGui.Components.LongValueTextBox mFirstRecordTextBox;
    private MixCharClipboardButtonControl mMixCharButtons;
    private System.Windows.Forms.Label mSetClipboardLabel;
    private MixByteCollectionEditorList mMixByteCollectionEditorList;
    private System.Windows.Forms.CheckBox mReadOnlyCheckBox;
    private System.Windows.Forms.Label mReadOnlyLabel;
    private System.Windows.Forms.Button mRevertButton;
    private System.Windows.Forms.Button mSaveButton;
    private System.IO.FileSystemWatcher mDeviceFileWatcher;
    private System.Windows.Forms.Timer mIODelayTimer;
    private System.Windows.Forms.Button mTruncateButton;
    private System.Windows.Forms.Button mAppendButton;
  }
}
