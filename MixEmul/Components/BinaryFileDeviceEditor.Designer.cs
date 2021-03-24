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
			this.mIODelayTimer = new System.Timers.Timer();
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
			this.mRecordLabel.Location = new System.Drawing.Point(0, 30);
			this.mRecordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.mRecordLabel.Name = "mRecordLabel";
			this.mRecordLabel.Size = new System.Drawing.Size(56, 15);
			this.mRecordLabel.TabIndex = 2;
			this.mRecordLabel.Text = "Show {0}:";
			// 
			// mFirstWordLabel
			// 
			this.mFirstWordLabel.AutoSize = true;
			this.mFirstWordLabel.Location = new System.Drawing.Point(0, 60);
			this.mFirstWordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.mFirstWordLabel.Name = "mFirstWordLabel";
			this.mFirstWordLabel.Size = new System.Drawing.Size(62, 15);
			this.mFirstWordLabel.TabIndex = 5;
			this.mFirstWordLabel.Text = "First word:";
			// 
			// mRecordTrackBar
			// 
			this.mRecordTrackBar.LargeChange = 256;
			this.mRecordTrackBar.Location = new System.Drawing.Point(144, 25);
			this.mRecordTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mRecordTrackBar.Maximum = 4095;
			this.mRecordTrackBar.Name = "mRecordTrackBar";
			this.mRecordTrackBar.Size = new System.Drawing.Size(177, 45);
			this.mRecordTrackBar.TabIndex = 4;
			this.mRecordTrackBar.TickFrequency = 256;
			this.mRecordTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.mRecordTrackBar.ValueChanged += new System.EventHandler(this.RecordTrackBar_ValueChanged);
			// 
			// mSetClipboardLabel
			// 
			this.mSetClipboardLabel.AutoSize = true;
			this.mSetClipboardLabel.Location = new System.Drawing.Point(149, 60);
			this.mSetClipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.mSetClipboardLabel.Name = "mSetClipboardLabel";
			this.mSetClipboardLabel.Size = new System.Drawing.Size(96, 15);
			this.mSetClipboardLabel.TabIndex = 7;
			this.mSetClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mRecordUpDown
			// 
			this.mRecordUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mRecordUpDown.Location = new System.Drawing.Point(83, 28);
			this.mRecordUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mRecordUpDown.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
			this.mRecordUpDown.Name = "mRecordUpDown";
			this.mRecordUpDown.Size = new System.Drawing.Size(61, 23);
			this.mRecordUpDown.TabIndex = 3;
			this.mRecordUpDown.ValueChanged += new System.EventHandler(this.RecordUpDown_ValueChanged);
			// 
			// mReadOnlyCheckBox
			// 
			this.mReadOnlyCheckBox.AutoSize = true;
			this.mReadOnlyCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mReadOnlyCheckBox.Location = new System.Drawing.Point(84, 1);
			this.mReadOnlyCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mReadOnlyCheckBox.Name = "mReadOnlyCheckBox";
			this.mReadOnlyCheckBox.Size = new System.Drawing.Size(12, 11);
			this.mReadOnlyCheckBox.TabIndex = 1;
			this.mReadOnlyCheckBox.UseVisualStyleBackColor = true;
			this.mReadOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ReadOnlyCheckBox_CheckedChanged);
			// 
			// mReadOnlyLabel
			// 
			this.mReadOnlyLabel.AutoSize = true;
			this.mReadOnlyLabel.Location = new System.Drawing.Point(0, 0);
			this.mReadOnlyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.mReadOnlyLabel.Name = "mReadOnlyLabel";
			this.mReadOnlyLabel.Size = new System.Drawing.Size(67, 15);
			this.mReadOnlyLabel.TabIndex = 0;
			this.mReadOnlyLabel.Text = "Read-only: ";
			// 
			// mRevertButton
			// 
			this.mRevertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mRevertButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mRevertButton.Location = new System.Drawing.Point(260, 225);
			this.mRevertButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mRevertButton.Name = "mRevertButton";
			this.mRevertButton.Size = new System.Drawing.Size(64, 27);
			this.mRevertButton.TabIndex = 10;
			this.mRevertButton.Text = "&Revert";
			this.mRevertButton.UseVisualStyleBackColor = true;
			this.mRevertButton.Click += new System.EventHandler(this.RevertButton_Click);
			// 
			// mSaveButton
			// 
			this.mSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mSaveButton.Location = new System.Drawing.Point(189, 225);
			this.mSaveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mSaveButton.Name = "mSaveButton";
			this.mSaveButton.Size = new System.Drawing.Size(64, 27);
			this.mSaveButton.TabIndex = 10;
			this.mSaveButton.Text = "&Save";
			this.mSaveButton.UseVisualStyleBackColor = true;
			this.mSaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// mDeviceFileWatcher
			// 
			this.mDeviceFileWatcher.EnableRaisingEvents = true;
			this.mDeviceFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
			this.mDeviceFileWatcher.SynchronizingObject = this;
			this.mDeviceFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Created += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this.mDeviceFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.DeviceFileWatcher_Renamed);
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
			this.mWordEditorList.Location = new System.Drawing.Point(0, 83);
			this.mWordEditorList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mWordEditorList.MaxIndex = -1;
			this.mWordEditorList.MinIndex = 0;
			this.mWordEditorList.Name = "mWordEditorList";
			this.mWordEditorList.ReadOnly = false;
			this.mWordEditorList.ResizeInProgress = false;
			this.mWordEditorList.Size = new System.Drawing.Size(324, 136);
			this.mWordEditorList.TabIndex = 9;
			this.mWordEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IWordEditor>.FirstVisibleIndexChangedHandler(this.MWordEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this.mMixCharButtons.Location = new System.Drawing.Point(251, 57);
			this.mMixCharButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mMixCharButtons.Name = "mMixCharButtons";
			this.mMixCharButtons.Size = new System.Drawing.Size(74, 27);
			this.mMixCharButtons.TabIndex = 8;
			// 
			// mFirstWordTextBox
			// 
			this.mFirstWordTextBox.BackColor = System.Drawing.Color.White;
			this.mFirstWordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mFirstWordTextBox.ClearZero = true;
			this.mFirstWordTextBox.ForeColor = System.Drawing.Color.Black;
			this.mFirstWordTextBox.Location = new System.Drawing.Point(83, 58);
			this.mFirstWordTextBox.LongValue = ((long)(0));
			this.mFirstWordTextBox.Magnitude = ((long)(0));
			this.mFirstWordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mFirstWordTextBox.MaxLength = 2;
			this.mFirstWordTextBox.MaxValue = ((long)(99));
			this.mFirstWordTextBox.MinValue = ((long)(0));
			this.mFirstWordTextBox.Name = "mFirstWordTextBox";
			this.mFirstWordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mFirstWordTextBox.Size = new System.Drawing.Size(40, 23);
			this.mFirstWordTextBox.SupportNegativeZero = false;
			this.mFirstWordTextBox.TabIndex = 6;
			this.mFirstWordTextBox.Text = "0";
			this.mFirstWordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.FirstWordTextBox_ValueChanged);
			// 
			// mIODelayTimer
			// 
			this.mIODelayTimer.Interval = 500;
			this.mIODelayTimer.Elapsed += this.IODelayTimer_Tick;
			// 
			// mAppendButton
			// 
			this.mAppendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mAppendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mAppendButton.Location = new System.Drawing.Point(0, 225);
			this.mAppendButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mAppendButton.Name = "mAppendButton";
			this.mAppendButton.Size = new System.Drawing.Size(71, 27);
			this.mAppendButton.TabIndex = 11;
			this.mAppendButton.Text = "A&ppend";
			this.mAppendButton.UseVisualStyleBackColor = true;
			this.mAppendButton.Click += new System.EventHandler(this.AppendButton_Click);
			// 
			// mTruncateButton
			// 
			this.mTruncateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTruncateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mTruncateButton.Location = new System.Drawing.Point(78, 225);
			this.mTruncateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mTruncateButton.Name = "mTruncateButton";
			this.mTruncateButton.Size = new System.Drawing.Size(71, 27);
			this.mTruncateButton.TabIndex = 11;
			this.mTruncateButton.Text = "&Truncate";
			this.mTruncateButton.UseVisualStyleBackColor = true;
			this.mTruncateButton.Click += new System.EventHandler(this.TruncateButton_Click);
			// 
			// BinaryFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
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
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MinimumSize = new System.Drawing.Size(321, 152);
			this.Name = "BinaryFileDeviceEditor";
			this.Size = new System.Drawing.Size(324, 252);
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
    private System.Timers.Timer mIODelayTimer;
    private System.Windows.Forms.Button mTruncateButton;
    private System.Windows.Forms.Button mAppendButton;
  }
}
