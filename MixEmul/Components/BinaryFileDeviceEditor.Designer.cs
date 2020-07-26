namespace MixGui.Components
{
  partial class BinaryFileDeviceEditor
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
			this.mRecordLabel = new System.Windows.Forms.Label();
			this.mFirstWordLabel = new System.Windows.Forms.Label();
			this.mRecordTrackBar = new System.Windows.Forms.TrackBar();
			this.mSetClipboardLabel = new System.Windows.Forms.Label();
			this.mRecordUpDown = new System.Windows.Forms.NumericUpDown();
			this.mReadOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this.mReadOnlyLabel = new System.Windows.Forms.Label();
			this.mRevertButton = new System.Windows.Forms.Button();
			this.mSaveButton = new System.Windows.Forms.Button();
			this.mDeviceFileWatcher = new System.IO.FileSystemWatcher();
			this.mWordEditorList = new MixGui.Components.WordEditorList();
			this.mMixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this.mFirstWordTextBox = new MixGui.Components.LongValueTextBox();
			this.mIODelayTimer = new System.Windows.Forms.Timer(this.components);
			this.mAppendButton = new System.Windows.Forms.Button();
			this.mTruncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.mRecordTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mRecordUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mDeviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mRecordLabel
			// 
			this.mRecordLabel.AutoSize = true;
			this.mRecordLabel.Location = new System.Drawing.Point(0, 26);
			this.mRecordLabel.Name = "mRecordLabel";
			this.mRecordLabel.Size = new System.Drawing.Size(54, 13);
			this.mRecordLabel.TabIndex = 2;
			this.mRecordLabel.Text = "Show {0}:";
			// 
			// mFirstWordLabel
			// 
			this.mFirstWordLabel.AutoSize = true;
			this.mFirstWordLabel.Location = new System.Drawing.Point(0, 52);
			this.mFirstWordLabel.Name = "mFirstWordLabel";
			this.mFirstWordLabel.Size = new System.Drawing.Size(55, 13);
			this.mFirstWordLabel.TabIndex = 5;
			this.mFirstWordLabel.Text = "First word:";
			// 
			// mRecordTrackBar
			// 
			this.mRecordTrackBar.LargeChange = 256;
			this.mRecordTrackBar.Location = new System.Drawing.Point(123, 22);
			this.mRecordTrackBar.Maximum = 4095;
			this.mRecordTrackBar.Name = "mRecordTrackBar";
			this.mRecordTrackBar.Size = new System.Drawing.Size(152, 45);
			this.mRecordTrackBar.TabIndex = 4;
			this.mRecordTrackBar.TickFrequency = 256;
			this.mRecordTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.mRecordTrackBar.ValueChanged += new System.EventHandler(this.MRecordTrackBar_ValueChanged);
			// 
			// mSetClipboardLabel
			// 
			this.mSetClipboardLabel.AutoSize = true;
			this.mSetClipboardLabel.Location = new System.Drawing.Point(128, 52);
			this.mSetClipboardLabel.Name = "mSetClipboardLabel";
			this.mSetClipboardLabel.Size = new System.Drawing.Size(87, 13);
			this.mSetClipboardLabel.TabIndex = 7;
			this.mSetClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mRecordUpDown
			// 
			this.mRecordUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mRecordUpDown.Location = new System.Drawing.Point(71, 24);
			this.mRecordUpDown.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
			this.mRecordUpDown.Name = "mRecordUpDown";
			this.mRecordUpDown.Size = new System.Drawing.Size(52, 20);
			this.mRecordUpDown.TabIndex = 3;
			this.mRecordUpDown.ValueChanged += new System.EventHandler(this.MRecordUpDown_ValueChanged);
			// 
			// mReadOnlyCheckBox
			// 
			this.mReadOnlyCheckBox.AutoSize = true;
			this.mReadOnlyCheckBox.Location = new System.Drawing.Point(72, 1);
			this.mReadOnlyCheckBox.Name = "mReadOnlyCheckBox";
			this.mReadOnlyCheckBox.Size = new System.Drawing.Size(15, 14);
			this.mReadOnlyCheckBox.TabIndex = 1;
			this.mReadOnlyCheckBox.UseVisualStyleBackColor = true;
			this.mReadOnlyCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mReadOnlyCheckBox.CheckedChanged += new System.EventHandler(this.MReadOnlyCheckBox_CheckedChanged);
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
			this.mRevertButton.Location = new System.Drawing.Point(223, 109);
			this.mRevertButton.Name = "mRevertButton";
			this.mRevertButton.Size = new System.Drawing.Size(55, 23);
			this.mRevertButton.TabIndex = 10;
			this.mRevertButton.Text = "&Revert";
			this.mRevertButton.UseVisualStyleBackColor = true;
			this.mRevertButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mRevertButton.Click += new System.EventHandler(this.MRevertButton_Click);
			// 
			// mSaveButton
			// 
			this.mSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mSaveButton.Location = new System.Drawing.Point(162, 109);
			this.mSaveButton.Name = "mSaveButton";
			this.mSaveButton.Size = new System.Drawing.Size(55, 23);
			this.mSaveButton.TabIndex = 10;
			this.mSaveButton.Text = "&Save";
			this.mSaveButton.UseVisualStyleBackColor = true;
			this.mSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mSaveButton.Click += new System.EventHandler(this.MSaveButton_Click);
			// 
			// mDeviceFileWatcher
			// 
			this.mDeviceFileWatcher.EnableRaisingEvents = true;
			this.mDeviceFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
			this.mDeviceFileWatcher.SynchronizingObject = this;
			this.mDeviceFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.MDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Created += new System.IO.FileSystemEventHandler(this.MDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.MDeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.MDeviceFileWatcher_Renamed);
			// 
			// mWordEditorList
			// 
			this.mWordEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mWordEditorList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mWordEditorList.CreateEditor = null;
			this.mWordEditorList.FirstVisibleIndex = 0;
			this.mWordEditorList.LoadEditor = null;
			this.mWordEditorList.Location = new System.Drawing.Point(0, 72);
			this.mWordEditorList.MaxIndex = -1;
			this.mWordEditorList.Name = "mWordEditorList";
			this.mWordEditorList.ReadOnly = false;
			this.mWordEditorList.ResizeInProgress = false;
			this.mWordEditorList.Size = new System.Drawing.Size(278, 32);
			this.mWordEditorList.TabIndex = 9;
			this.mWordEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IWordEditor>.FirstVisibleIndexChangedHandler(this.MWordEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this.mMixCharButtons.Location = new System.Drawing.Point(215, 49);
			this.mMixCharButtons.Name = "mMixCharButtons";
			this.mMixCharButtons.Size = new System.Drawing.Size(63, 23);
			this.mMixCharButtons.TabIndex = 8;
			// 
			// mFirstWordTextBox
			// 
			this.mFirstWordTextBox.BackColor = System.Drawing.Color.White;
			this.mFirstWordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mFirstWordTextBox.ForeColor = System.Drawing.Color.Black;
			this.mFirstWordTextBox.Location = new System.Drawing.Point(71, 50);
			this.mFirstWordTextBox.LongValue = ((long)(0));
			this.mFirstWordTextBox.Magnitude = ((long)(0));
			this.mFirstWordTextBox.MaxLength = 2;
			this.mFirstWordTextBox.MaxValue = ((long)(99));
			this.mFirstWordTextBox.MinValue = ((long)(0));
			this.mFirstWordTextBox.Name = "mFirstWordTextBox";
			this.mFirstWordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mFirstWordTextBox.Size = new System.Drawing.Size(35, 20);
			this.mFirstWordTextBox.SupportNegativeZero = false;
			this.mFirstWordTextBox.TabIndex = 6;
			this.mFirstWordTextBox.Text = "0";
			this.mFirstWordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.MFirstWordTextBox_ValueChanged);
			// 
			// mIODelayTimer
			// 
			this.mIODelayTimer.Interval = 500;
			this.mIODelayTimer.Tick += new System.EventHandler(this.MIODelayTimer_Tick);
			// 
			// mAppendButton
			// 
			this.mAppendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mAppendButton.Location = new System.Drawing.Point(0, 109);
			this.mAppendButton.Name = "mAppendButton";
			this.mAppendButton.Size = new System.Drawing.Size(61, 23);
			this.mAppendButton.TabIndex = 11;
			this.mAppendButton.Text = "A&ppend";
			this.mAppendButton.UseVisualStyleBackColor = true;
			this.mAppendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mAppendButton.Click += new System.EventHandler(this.MAppendButton_Click);
			// 
			// mTruncateButton
			// 
			this.mTruncateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTruncateButton.Location = new System.Drawing.Point(67, 109);
			this.mTruncateButton.Name = "mTruncateButton";
			this.mTruncateButton.Size = new System.Drawing.Size(61, 23);
			this.mTruncateButton.TabIndex = 11;
			this.mTruncateButton.Text = "&Truncate";
			this.mTruncateButton.UseVisualStyleBackColor = true;
			this.mTruncateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mTruncateButton.Click += new System.EventHandler(this.MTruncateButton_Click);
			// 
			// BinaryFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mTruncateButton);
			this.Controls.Add(this.mAppendButton);
			this.Controls.Add(this.mSaveButton);
			this.Controls.Add(this.mRevertButton);
			this.Controls.Add(this.mReadOnlyLabel);
			this.Controls.Add(this.mReadOnlyCheckBox);
			this.Controls.Add(this.mWordEditorList);
			this.Controls.Add(this.mRecordUpDown);
			this.Controls.Add(this.mMixCharButtons);
			this.Controls.Add(this.mSetClipboardLabel);
			this.Controls.Add(this.mRecordTrackBar);
			this.Controls.Add(this.mFirstWordTextBox);
			this.Controls.Add(this.mFirstWordLabel);
			this.Controls.Add(this.mRecordLabel);
			this.MinimumSize = new System.Drawing.Size(275, 132);
			this.Name = "BinaryFileDeviceEditor";
			this.Size = new System.Drawing.Size(278, 132);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.mRecordTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mRecordUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mDeviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label mRecordLabel;
    private System.Windows.Forms.Label mFirstWordLabel;
    private global::MixGui.Components.LongValueTextBox mFirstWordTextBox;
    private System.Windows.Forms.TrackBar mRecordTrackBar;
    private MixCharClipboardButtonControl mMixCharButtons;
    private System.Windows.Forms.Label mSetClipboardLabel;
    private System.Windows.Forms.NumericUpDown mRecordUpDown;
    private WordEditorList mWordEditorList;
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
