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
			this._recordLabel = new System.Windows.Forms.Label();
			this._firstWordLabel = new System.Windows.Forms.Label();
			this._recordTrackBar = new System.Windows.Forms.TrackBar();
			this._setClipboardLabel = new System.Windows.Forms.Label();
			this._recordUpDown = new System.Windows.Forms.NumericUpDown();
			this._readOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this._readOnlyLabel = new System.Windows.Forms.Label();
			this._revertButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._deviceFileWatcher = new System.IO.FileSystemWatcher();
			this._wordEditorList = new MixGui.Components.WordEditorList();
			this._mixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this._firstWordTextBox = new MixGui.Components.LongValueTextBox();
			this._ioDelayTimer = new System.Timers.Timer();
			this._appendButton = new System.Windows.Forms.Button();
			this._truncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._recordTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._recordUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._deviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mRecordLabel
			// 
			this._recordLabel.AutoSize = true;
			this._recordLabel.Location = new System.Drawing.Point(0, 30);
			this._recordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._recordLabel.Name = "mRecordLabel";
			this._recordLabel.Size = new System.Drawing.Size(56, 15);
			this._recordLabel.TabIndex = 2;
			this._recordLabel.Text = "Show {0}:";
			// 
			// mFirstWordLabel
			// 
			this._firstWordLabel.AutoSize = true;
			this._firstWordLabel.Location = new System.Drawing.Point(0, 60);
			this._firstWordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._firstWordLabel.Name = "mFirstWordLabel";
			this._firstWordLabel.Size = new System.Drawing.Size(62, 15);
			this._firstWordLabel.TabIndex = 5;
			this._firstWordLabel.Text = "First word:";
			// 
			// mRecordTrackBar
			// 
			this._recordTrackBar.LargeChange = 256;
			this._recordTrackBar.Location = new System.Drawing.Point(144, 25);
			this._recordTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._recordTrackBar.Maximum = 4095;
			this._recordTrackBar.Name = "mRecordTrackBar";
			this._recordTrackBar.Size = new System.Drawing.Size(177, 45);
			this._recordTrackBar.TabIndex = 4;
			this._recordTrackBar.TickFrequency = 256;
			this._recordTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this._recordTrackBar.ValueChanged += new System.EventHandler(this.RecordTrackBar_ValueChanged);
			// 
			// mSetClipboardLabel
			// 
			this._setClipboardLabel.AutoSize = true;
			this._setClipboardLabel.Location = new System.Drawing.Point(149, 60);
			this._setClipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._setClipboardLabel.Name = "mSetClipboardLabel";
			this._setClipboardLabel.Size = new System.Drawing.Size(96, 15);
			this._setClipboardLabel.TabIndex = 7;
			this._setClipboardLabel.Text = "Set clipboard to: ";
			// 
			// mRecordUpDown
			// 
			this._recordUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._recordUpDown.Location = new System.Drawing.Point(83, 28);
			this._recordUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._recordUpDown.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
			this._recordUpDown.Name = "mRecordUpDown";
			this._recordUpDown.Size = new System.Drawing.Size(61, 23);
			this._recordUpDown.TabIndex = 3;
			this._recordUpDown.ValueChanged += new System.EventHandler(this.RecordUpDown_ValueChanged);
			// 
			// mReadOnlyCheckBox
			// 
			this._readOnlyCheckBox.AutoSize = true;
			this._readOnlyCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._readOnlyCheckBox.Location = new System.Drawing.Point(84, 1);
			this._readOnlyCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._readOnlyCheckBox.Name = "mReadOnlyCheckBox";
			this._readOnlyCheckBox.Size = new System.Drawing.Size(12, 11);
			this._readOnlyCheckBox.TabIndex = 1;
			this._readOnlyCheckBox.UseVisualStyleBackColor = true;
			this._readOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ReadOnlyCheckBox_CheckedChanged);
			// 
			// mReadOnlyLabel
			// 
			this._readOnlyLabel.AutoSize = true;
			this._readOnlyLabel.Location = new System.Drawing.Point(0, 0);
			this._readOnlyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._readOnlyLabel.Name = "mReadOnlyLabel";
			this._readOnlyLabel.Size = new System.Drawing.Size(67, 15);
			this._readOnlyLabel.TabIndex = 0;
			this._readOnlyLabel.Text = "Read-only: ";
			// 
			// mRevertButton
			// 
			this._revertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._revertButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._revertButton.Location = new System.Drawing.Point(260, 225);
			this._revertButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._revertButton.Name = "mRevertButton";
			this._revertButton.Size = new System.Drawing.Size(64, 27);
			this._revertButton.TabIndex = 10;
			this._revertButton.Text = "&Revert";
			this._revertButton.UseVisualStyleBackColor = true;
			this._revertButton.Click += new System.EventHandler(this.RevertButton_Click);
			// 
			// mSaveButton
			// 
			this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._saveButton.Location = new System.Drawing.Point(189, 225);
			this._saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._saveButton.Name = "mSaveButton";
			this._saveButton.Size = new System.Drawing.Size(64, 27);
			this._saveButton.TabIndex = 10;
			this._saveButton.Text = "&Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// mDeviceFileWatcher
			// 
			this._deviceFileWatcher.EnableRaisingEvents = true;
			this._deviceFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
			this._deviceFileWatcher.SynchronizingObject = this;
			this._deviceFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this._deviceFileWatcher.Created += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this._deviceFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.DeviceFileWatcher_Event);
			this._deviceFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.DeviceFileWatcher_Renamed);
			// 
			// mWordEditorList
			// 
			this._wordEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._wordEditorList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._wordEditorList.CreateEditor = null;
			this._wordEditorList.FirstVisibleIndex = 0;
			this._wordEditorList.LoadEditor = null;
			this._wordEditorList.Location = new System.Drawing.Point(0, 83);
			this._wordEditorList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._wordEditorList.MaxIndex = -1;
			this._wordEditorList.MinIndex = 0;
			this._wordEditorList.Name = "mWordEditorList";
			this._wordEditorList.ReadOnly = false;
			this._wordEditorList.ResizeInProgress = false;
			this._wordEditorList.Size = new System.Drawing.Size(324, 136);
			this._wordEditorList.TabIndex = 9;
			this._wordEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IWordEditor>.FirstVisibleIndexChangedHandler(this.WordEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this._mixCharButtons.Location = new System.Drawing.Point(251, 57);
			this._mixCharButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._mixCharButtons.Name = "mMixCharButtons";
			this._mixCharButtons.Size = new System.Drawing.Size(74, 27);
			this._mixCharButtons.TabIndex = 8;
			// 
			// mFirstWordTextBox
			// 
			this._firstWordTextBox.BackColor = System.Drawing.Color.White;
			this._firstWordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._firstWordTextBox.ClearZero = true;
			this._firstWordTextBox.ForeColor = System.Drawing.Color.Black;
			this._firstWordTextBox.Location = new System.Drawing.Point(83, 58);
			this._firstWordTextBox.LongValue = ((long)(0));
			this._firstWordTextBox.Magnitude = ((long)(0));
			this._firstWordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._firstWordTextBox.MaxLength = 2;
			this._firstWordTextBox.MaxValue = ((long)(99));
			this._firstWordTextBox.MinValue = ((long)(0));
			this._firstWordTextBox.Name = "mFirstWordTextBox";
			this._firstWordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this._firstWordTextBox.Size = new System.Drawing.Size(40, 23);
			this._firstWordTextBox.SupportNegativeZero = false;
			this._firstWordTextBox.TabIndex = 6;
			this._firstWordTextBox.Text = "0";
			this._firstWordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.FirstWordTextBox_ValueChanged);
			// 
			// mIODelayTimer
			// 
			this._ioDelayTimer.Interval = 500;
			this._ioDelayTimer.Elapsed += this.IODelayTimer_Tick;
			// 
			// mAppendButton
			// 
			this._appendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._appendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._appendButton.Location = new System.Drawing.Point(0, 225);
			this._appendButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._appendButton.Name = "mAppendButton";
			this._appendButton.Size = new System.Drawing.Size(71, 27);
			this._appendButton.TabIndex = 11;
			this._appendButton.Text = "A&ppend";
			this._appendButton.UseVisualStyleBackColor = true;
			this._appendButton.Click += new System.EventHandler(this.AppendButton_Click);
			// 
			// mTruncateButton
			// 
			this._truncateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._truncateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._truncateButton.Location = new System.Drawing.Point(78, 225);
			this._truncateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._truncateButton.Name = "mTruncateButton";
			this._truncateButton.Size = new System.Drawing.Size(71, 27);
			this._truncateButton.TabIndex = 11;
			this._truncateButton.Text = "&Truncate";
			this._truncateButton.UseVisualStyleBackColor = true;
			this._truncateButton.Click += new System.EventHandler(this.TruncateButton_Click);
			// 
			// BinaryFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._truncateButton);
			this.Controls.Add(this._appendButton);
			this.Controls.Add(this._saveButton);
			this.Controls.Add(this._revertButton);
			this.Controls.Add(this._readOnlyLabel);
			this.Controls.Add(this._readOnlyCheckBox);
			this.Controls.Add(this._wordEditorList);
			this.Controls.Add(this._recordUpDown);
			this.Controls.Add(this._mixCharButtons);
			this.Controls.Add(this._setClipboardLabel);
			this.Controls.Add(this._recordTrackBar);
			this.Controls.Add(this._firstWordTextBox);
			this.Controls.Add(this._firstWordLabel);
			this.Controls.Add(this._recordLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MinimumSize = new System.Drawing.Size(321, 152);
			this.Name = "BinaryFileDeviceEditor";
			this.Size = new System.Drawing.Size(324, 252);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this._recordTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._recordUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._deviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _recordLabel;
    private System.Windows.Forms.Label _firstWordLabel;
    private global::MixGui.Components.LongValueTextBox _firstWordTextBox;
    private System.Windows.Forms.TrackBar _recordTrackBar;
    private MixCharClipboardButtonControl _mixCharButtons;
    private System.Windows.Forms.Label _setClipboardLabel;
    private System.Windows.Forms.NumericUpDown _recordUpDown;
    private WordEditorList _wordEditorList;
    private System.Windows.Forms.CheckBox _readOnlyCheckBox;
    private System.Windows.Forms.Label _readOnlyLabel;
    private System.Windows.Forms.Button _revertButton;
    private System.Windows.Forms.Button _saveButton;
    private System.IO.FileSystemWatcher _deviceFileWatcher;
    private System.Timers.Timer _ioDelayTimer;
    private System.Windows.Forms.Button _truncateButton;
    private System.Windows.Forms.Button _appendButton;
  }
}
