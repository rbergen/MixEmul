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
			this.recordLabel = new System.Windows.Forms.Label();
			this.firstWordLabel = new System.Windows.Forms.Label();
			this.recordTrackBar = new System.Windows.Forms.TrackBar();
			this.setClipboardLabel = new System.Windows.Forms.Label();
			this.recordUpDown = new System.Windows.Forms.NumericUpDown();
			this.readOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this.readOnlyLabel = new System.Windows.Forms.Label();
			this.revertButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.deviceFileWatcher = new System.IO.FileSystemWatcher();
			this.wordEditorList = new MixGui.Components.WordEditorList();
			this.mixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this.firstWordTextBox = new MixGui.Components.LongValueTextBox();
			this.ioDelayTimer = new System.Timers.Timer();
			this.appendButton = new System.Windows.Forms.Button();
			this.truncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.recordTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.recordUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.deviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mRecordLabel
			// 
			this.recordLabel.AutoSize = true;
			this.recordLabel.Location = new System.Drawing.Point(0, 30);
			this.recordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.recordLabel.Name = "mRecordLabel";
			this.recordLabel.Size = new System.Drawing.Size(56, 15);
			this.recordLabel.TabIndex = 2;
			this.recordLabel.Text = "Show {0}:";
			// 
			// mFirstWordLabel
			// 
			this.firstWordLabel.AutoSize = true;
			this.firstWordLabel.Location = new System.Drawing.Point(0, 60);
			this.firstWordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.firstWordLabel.Name = "mFirstWordLabel";
			this.firstWordLabel.Size = new System.Drawing.Size(62, 15);
			this.firstWordLabel.TabIndex = 5;
			this.firstWordLabel.Text = "First word:";
			// 
			// mRecordTrackBar
			// 
			this.recordTrackBar.LargeChange = 256;
			this.recordTrackBar.Location = new System.Drawing.Point(144, 25);
			this.recordTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.recordTrackBar.Maximum = 4095;
			this.recordTrackBar.Name = "mRecordTrackBar";
			this.recordTrackBar.Size = new System.Drawing.Size(177, 45);
			this.recordTrackBar.TabIndex = 4;
			this.recordTrackBar.TickFrequency = 256;
			this.recordTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.recordTrackBar.ValueChanged += new System.EventHandler(this.RecordTrackBar_ValueChanged);
			// 
			// mSetClipboardLabel
			// 
			this.setClipboardLabel.AutoSize = true;
			this.setClipboardLabel.Location = new System.Drawing.Point(149, 60);
			this.setClipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.setClipboardLabel.Name = "mSetClipboardLabel";
			this.setClipboardLabel.Size = new System.Drawing.Size(96, 15);
			this.setClipboardLabel.TabIndex = 7;
			this.setClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mRecordUpDown
			// 
			this.recordUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.recordUpDown.Location = new System.Drawing.Point(83, 28);
			this.recordUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.recordUpDown.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
			this.recordUpDown.Name = "mRecordUpDown";
			this.recordUpDown.Size = new System.Drawing.Size(61, 23);
			this.recordUpDown.TabIndex = 3;
			this.recordUpDown.ValueChanged += new System.EventHandler(this.RecordUpDown_ValueChanged);
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
			this.revertButton.Location = new System.Drawing.Point(260, 225);
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
			this.saveButton.Location = new System.Drawing.Point(189, 225);
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
			// mWordEditorList
			// 
			this.wordEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.wordEditorList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.wordEditorList.CreateEditor = null;
			this.wordEditorList.FirstVisibleIndex = 0;
			this.wordEditorList.LoadEditor = null;
			this.wordEditorList.Location = new System.Drawing.Point(0, 83);
			this.wordEditorList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.wordEditorList.MaxIndex = -1;
			this.wordEditorList.MinIndex = 0;
			this.wordEditorList.Name = "mWordEditorList";
			this.wordEditorList.ReadOnly = false;
			this.wordEditorList.ResizeInProgress = false;
			this.wordEditorList.Size = new System.Drawing.Size(324, 136);
			this.wordEditorList.TabIndex = 9;
			this.wordEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IWordEditor>.FirstVisibleIndexChangedHandler(this.WordEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this.mixCharButtons.Location = new System.Drawing.Point(251, 57);
			this.mixCharButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.mixCharButtons.Name = "mMixCharButtons";
			this.mixCharButtons.Size = new System.Drawing.Size(74, 27);
			this.mixCharButtons.TabIndex = 8;
			// 
			// mFirstWordTextBox
			// 
			this.firstWordTextBox.BackColor = System.Drawing.Color.White;
			this.firstWordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.firstWordTextBox.ClearZero = true;
			this.firstWordTextBox.ForeColor = System.Drawing.Color.Black;
			this.firstWordTextBox.Location = new System.Drawing.Point(83, 58);
			this.firstWordTextBox.LongValue = ((long)(0));
			this.firstWordTextBox.Magnitude = ((long)(0));
			this.firstWordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.firstWordTextBox.MaxLength = 2;
			this.firstWordTextBox.MaxValue = ((long)(99));
			this.firstWordTextBox.MinValue = ((long)(0));
			this.firstWordTextBox.Name = "mFirstWordTextBox";
			this.firstWordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.firstWordTextBox.Size = new System.Drawing.Size(40, 23);
			this.firstWordTextBox.SupportNegativeZero = false;
			this.firstWordTextBox.TabIndex = 6;
			this.firstWordTextBox.Text = "0";
			this.firstWordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.FirstWordTextBox_ValueChanged);
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
			this.appendButton.Location = new System.Drawing.Point(0, 225);
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
			this.truncateButton.Location = new System.Drawing.Point(78, 225);
			this.truncateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.truncateButton.Name = "mTruncateButton";
			this.truncateButton.Size = new System.Drawing.Size(71, 27);
			this.truncateButton.TabIndex = 11;
			this.truncateButton.Text = "&Truncate";
			this.truncateButton.UseVisualStyleBackColor = true;
			this.truncateButton.Click += new System.EventHandler(this.TruncateButton_Click);
			// 
			// BinaryFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.truncateButton);
			this.Controls.Add(this.appendButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.revertButton);
			this.Controls.Add(this.readOnlyLabel);
			this.Controls.Add(this.readOnlyCheckBox);
			this.Controls.Add(this.wordEditorList);
			this.Controls.Add(this.recordUpDown);
			this.Controls.Add(this.mixCharButtons);
			this.Controls.Add(this.setClipboardLabel);
			this.Controls.Add(this.recordTrackBar);
			this.Controls.Add(this.firstWordTextBox);
			this.Controls.Add(this.firstWordLabel);
			this.Controls.Add(this.recordLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MinimumSize = new System.Drawing.Size(321, 152);
			this.Name = "BinaryFileDeviceEditor";
			this.Size = new System.Drawing.Size(324, 252);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.recordTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.recordUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.deviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label recordLabel;
    private System.Windows.Forms.Label firstWordLabel;
    private global::MixGui.Components.LongValueTextBox firstWordTextBox;
    private System.Windows.Forms.TrackBar recordTrackBar;
    private MixCharClipboardButtonControl mixCharButtons;
    private System.Windows.Forms.Label setClipboardLabel;
    private System.Windows.Forms.NumericUpDown recordUpDown;
    private WordEditorList wordEditorList;
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
