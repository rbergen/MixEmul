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
			this._firstRecordLabel = new System.Windows.Forms.Label();
			this._setClipboardLabel = new System.Windows.Forms.Label();
			this._readOnlyCheckBox = new System.Windows.Forms.CheckBox();
			this._readOnlyLabel = new System.Windows.Forms.Label();
			this._revertButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._deviceFileWatcher = new System.IO.FileSystemWatcher();
			this._mixByteCollectionEditorList = new MixGui.Components.MixByteCollectionEditorList();
			this._mixCharButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this._firstRecordTextBox = new MixGui.Components.LongValueTextBox();
			this._ioDelayTimer = new System.Timers.Timer();
			this._appendButton = new System.Windows.Forms.Button();
			this._truncateButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._deviceFileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// mFirstRecordLabel
			// 
			this._firstRecordLabel.AutoSize = true;
			this._firstRecordLabel.Location = new System.Drawing.Point(0, 27);
			this._firstRecordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._firstRecordLabel.Name = "mFirstRecordLabel";
			this._firstRecordLabel.Size = new System.Drawing.Size(49, 15);
			this._firstRecordLabel.TabIndex = 5;
			this._firstRecordLabel.Text = "First {0}:";
			// 
			// mSetClipboardLabel
			// 
			this._setClipboardLabel.AutoSize = true;
			this._setClipboardLabel.Location = new System.Drawing.Point(149, 27);
			this._setClipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._setClipboardLabel.Name = "mSetClipboardLabel";
			this._setClipboardLabel.Size = new System.Drawing.Size(96, 15);
			this._setClipboardLabel.TabIndex = 7;
			this._setClipboardLabel.Text = "Set clipboard to: ";
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
			this._revertButton.Location = new System.Drawing.Point(260, 170);
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
			this._saveButton.Location = new System.Drawing.Point(189, 170);
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
			// mMixByteCollectionEditorList
			// 
			this._mixByteCollectionEditorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mixByteCollectionEditorList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._mixByteCollectionEditorList.CreateEditor = null;
			this._mixByteCollectionEditorList.FirstVisibleIndex = 0;
			this._mixByteCollectionEditorList.LoadEditor = null;
			this._mixByteCollectionEditorList.Location = new System.Drawing.Point(0, 50);
			this._mixByteCollectionEditorList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._mixByteCollectionEditorList.MaxIndex = 0;
			this._mixByteCollectionEditorList.MinIndex = 0;
			this._mixByteCollectionEditorList.Name = "mMixByteCollectionEditorList";
			this._mixByteCollectionEditorList.ReadOnly = false;
			this._mixByteCollectionEditorList.ResizeInProgress = false;
			this._mixByteCollectionEditorList.Size = new System.Drawing.Size(324, 115);
			this._mixByteCollectionEditorList.TabIndex = 9;
			this._mixByteCollectionEditorList.FirstVisibleIndexChanged += new MixGui.Components.EditorList<MixGui.Components.IMixByteCollectionEditor>.FirstVisibleIndexChangedHandler(this.MixByteCollectionEditorList_FirstVisibleIndexChanged);
			// 
			// mMixCharButtons
			// 
			this._mixCharButtons.Location = new System.Drawing.Point(251, 23);
			this._mixCharButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._mixCharButtons.Name = "mMixCharButtons";
			this._mixCharButtons.Size = new System.Drawing.Size(74, 27);
			this._mixCharButtons.TabIndex = 8;
			// 
			// mFirstRecordTextBox
			// 
			this._firstRecordTextBox.BackColor = System.Drawing.Color.White;
			this._firstRecordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._firstRecordTextBox.ClearZero = true;
			this._firstRecordTextBox.ForeColor = System.Drawing.Color.Black;
			this._firstRecordTextBox.Location = new System.Drawing.Point(83, 24);
			this._firstRecordTextBox.LongValue = ((long)(0));
			this._firstRecordTextBox.Magnitude = ((long)(0));
			this._firstRecordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._firstRecordTextBox.MaxLength = 2;
			this._firstRecordTextBox.MaxValue = ((long)(99));
			this._firstRecordTextBox.MinValue = ((long)(0));
			this._firstRecordTextBox.Name = "mFirstRecordTextBox";
			this._firstRecordTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this._firstRecordTextBox.Size = new System.Drawing.Size(40, 23);
			this._firstRecordTextBox.SupportNegativeZero = false;
			this._firstRecordTextBox.TabIndex = 6;
			this._firstRecordTextBox.Text = "0";
			this._firstRecordTextBox.ValueChanged += new MixGui.Components.LongValueTextBox.ValueChangedEventHandler(this.FirstRecordTextBox_ValueChanged);
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
			this._appendButton.Location = new System.Drawing.Point(0, 170);
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
			this._truncateButton.Location = new System.Drawing.Point(78, 170);
			this._truncateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._truncateButton.Name = "mTruncateButton";
			this._truncateButton.Size = new System.Drawing.Size(71, 27);
			this._truncateButton.TabIndex = 11;
			this._truncateButton.Text = "&Truncate";
			this._truncateButton.UseVisualStyleBackColor = true;
			this._truncateButton.Click += new System.EventHandler(this.TruncateButton_Click);
			// 
			// TextFileDeviceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._truncateButton);
			this.Controls.Add(this._appendButton);
			this.Controls.Add(this._saveButton);
			this.Controls.Add(this._revertButton);
			this.Controls.Add(this._readOnlyLabel);
			this.Controls.Add(this._readOnlyCheckBox);
			this.Controls.Add(this._mixByteCollectionEditorList);
			this.Controls.Add(this._mixCharButtons);
			this.Controls.Add(this._setClipboardLabel);
			this.Controls.Add(this._firstRecordTextBox);
			this.Controls.Add(this._firstRecordLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "TextFileDeviceEditor";
			this.Size = new System.Drawing.Size(324, 197);
			this.VisibleChanged += new System.EventHandler(this.DeviceEditor_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this._deviceFileWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _firstRecordLabel;
    private global::MixGui.Components.LongValueTextBox _firstRecordTextBox;
    private MixCharClipboardButtonControl _mixCharButtons;
    private System.Windows.Forms.Label _setClipboardLabel;
    private MixByteCollectionEditorList _mixByteCollectionEditorList;
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
