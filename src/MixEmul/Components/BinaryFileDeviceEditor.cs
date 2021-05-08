using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class BinaryFileDeviceEditor : UserControl
	{
		public delegate FileStream OpenStreamCallback(string FilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
		public delegate long CalculateBytePositionCallback(long record);
		public delegate IFullWord[] ReadWordsCallback(Stream stream, int wordCount);
		public delegate void WriteWordsCallback(Stream stream, int wordCount, IFullWord[] words);
		public delegate long CalculateRecordCountCallback(FileStream stream);

		private delegate bool DelayedIOOperation();

		private const int MaxLoadSectorRetryCount = 5;

		private FileBasedDevice device;
		private bool changesPending;
		private IFullWord[] recordReadWords;
		private IFullWord[] recordEditWords;
		private long lastReadRecord;
		private DelayedIOOperation delayedIOOperation;
		private int loadRecordRetryCount;
		private long deviceRecordCount;
		private long deviceWordsPerRecord;
		private OpenStreamCallback openStream;
		private CalculateBytePositionCallback calculateBytePosition;
		private ReadWordsCallback readWords;
		private WriteWordsCallback writeWords;
		private CalculateRecordCountCallback calculateRecordCount;
		private readonly int readOnlyGap;
		private readonly bool initialized;
		private bool showReadOnly;
		private int editorListWithButtonsHeight;
		private readonly string recordLabelFormatString;
		private string recordName;
		private DateTime lastLoadTime;
		private bool loading;

		public BinaryFileDeviceEditor()
		{
			InitializeComponent();

			this.readOnlyGap = this.recordUpDown.Top;
			this.editorListWithButtonsHeight = this.wordEditorList.Height;
			this.recordLabelFormatString = this.recordLabel.Text;
			this.calculateRecordCount = null;
			RecordName = "record";
			this.showReadOnly = true;
			this.lastLoadTime = DateTime.Now;
			SetDeviceRecordCount(1);
			this.ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			this.initialized = false;
		}

		public long DeviceRecordCount
			=> this.deviceRecordCount;

		public bool SupportsAppending
			=> this.calculateRecordCount != null && !this.readOnlyCheckBox.Checked;

		private bool LoadRecord()
			=> LoadRecord(LoadErrorHandlingMode.SilentIfSameRecord);

		public bool WriteRecord()
			=> WriteRecord(false);

		public new void Update()
			=> this.wordEditorList.Update();

		public bool SaveCurrentSector()
			=> WriteRecord(true);

		private bool ProcessRecordSelectionChange()
			=> WriteRecord() && LoadRecord();

		private void SaveButton_Click(object sender, EventArgs e)
			=> WriteRecord(true);

		private void RevertButton_Click(object sender, EventArgs e)
			=> Revert();

		private void DeviceFileWatcher_Event(object sender, FileSystemEventArgs e)
			=> HandleFileWatcherEvent();

		private void DeviceEditor_VisibleChanged(object sender, EventArgs e)
			=> ProcessVisibility();

		private void DeviceFileWatcher_Renamed(object sender, RenamedEventArgs e)
			=> HandleFileWatcherEvent();

		public void Initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
			=> Initialize(recordCount, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, null);

		public void Initialize(CalculateRecordCountCallback calculateRecordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
			=> Initialize(1, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, calculateRecordCount);

		private void Initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords, CalculateRecordCountCallback calculateRecordCount)
		{
			if (this.initialized)
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");

			this.deviceWordsPerRecord = wordsPerRecord;
			this.openStream = openStream;
			this.calculateBytePosition = calculateBytePosition;
			this.readWords = readWords;
			this.writeWords = writeWords;
			this.calculateRecordCount = calculateRecordCount;

			this.recordUpDown.Minimum = 0;
			this.recordTrackBar.Minimum = 0;
			SetDeviceRecordCount(recordCount);

			this.firstWordTextBox.ClearZero = false;
			this.firstWordTextBox.MinValue = 0L;
			this.firstWordTextBox.MaxValue = this.deviceWordsPerRecord - 1;

			this.recordReadWords = new IFullWord[this.deviceWordsPerRecord];
			this.recordEditWords = new IFullWord[this.deviceWordsPerRecord];

			for (int i = 0; i < this.deviceWordsPerRecord; i++)
				this.recordEditWords[i] = new FullWord();

			this.wordEditorList.MaxIndex = (int)this.deviceWordsPerRecord - 1;
			this.wordEditorList.CreateEditor = CreateWordEditor;
			this.wordEditorList.LoadEditor = LoadWordEditor;

			this.changesPending = false;

			this.loadRecordRetryCount = 0;

			ProcessSupportsAppending();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => this.recordName;
			set
			{
				this.recordName = value;

				this.recordLabel.Text = string.Format(this.recordLabelFormatString, this.recordName);
			}
		}

		private void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			this.appendButton.Visible = supportsAppending;
			this.truncateButton.Visible = supportsAppending;
		}

		public bool ShowReadOnly
		{
			get => this.showReadOnly;
			set
			{
				if (this.showReadOnly == value)
					return;

				this.showReadOnly = value;

				this.readOnlyLabel.Visible = this.showReadOnly;
				this.readOnlyCheckBox.Visible = this.showReadOnly;

				if (this.showReadOnly)
				{
					this.recordLabel.Top += this.readOnlyGap;
					this.recordUpDown.Top += this.readOnlyGap;
					this.recordTrackBar.Top += this.readOnlyGap;
					this.firstWordLabel.Top += this.readOnlyGap;
					this.firstWordTextBox.Top += this.readOnlyGap;
					this.setClipboardLabel.Top += this.readOnlyGap;
					this.mixCharButtons.Top += this.readOnlyGap;
					this.wordEditorList.Top += this.readOnlyGap;
					this.wordEditorList.Height -= this.readOnlyGap;
					this.editorListWithButtonsHeight -= this.readOnlyGap;
				}
				else
				{
					this.recordLabel.Top -= this.readOnlyGap;
					this.recordUpDown.Top -= this.readOnlyGap;
					this.recordTrackBar.Top -= this.readOnlyGap;
					this.firstWordLabel.Top -= this.readOnlyGap;
					this.firstWordTextBox.Top -= this.readOnlyGap;
					this.setClipboardLabel.Top -= this.readOnlyGap;
					this.mixCharButtons.Top -= this.readOnlyGap;
					this.wordEditorList.Top -= this.readOnlyGap;
					this.wordEditorList.Height += this.readOnlyGap;
					this.editorListWithButtonsHeight += this.readOnlyGap;
				}

				ProcessReadOnlySettings();
			}
		}

		private void ProcessReadOnlySettings()
		{
			bool showSaveRevertButtons = !this.wordEditorList.ReadOnly || this.showReadOnly;

			this.revertButton.Visible = showSaveRevertButtons;
			this.saveButton.Visible = showSaveRevertButtons;

			ProcessSupportsAppending();
			ProcessButtonVisibility();
		}

		private void ProcessButtonVisibility()
			=> this.wordEditorList.Height = this.appendButton.Visible || this.truncateButton.Visible || this.revertButton.Visible || this.saveButton.Visible
			? this.editorListWithButtonsHeight
			: (Height - this.wordEditorList.Top);

		private void SetDeviceRecordCount(long recordCount)
		{
			if (recordCount < 1)
				recordCount = 1;

			this.deviceRecordCount = recordCount;

			this.recordUpDown.Maximum = this.deviceRecordCount - 1;
			this.recordTrackBar.Maximum = (int)(this.deviceRecordCount - 1);
			this.recordTrackBar.LargeChange = (int)(this.deviceRecordCount / 20);

			SetTruncateEnabledState();
		}

		private void SetTruncateEnabledState()
			=> this.truncateButton.Enabled = this.lastReadRecord == this.deviceRecordCount - 1 && this.deviceRecordCount > 1;

		private IWordEditor CreateWordEditor(int index)
		{
			var editor = new DeviceWordEditor(index == -1 ? new FullWord(0) : this.recordEditWords[index])
			{
				WordIndex = index
			};

			editor.ValueChanged += Editor_ValueChanged;

			return editor;
		}

		private void ProcessVisibility()
		{
			if (Visible)
			{
				this.deviceFileWatcher.EnableRaisingEvents |= this.device != null;

				LoadRecord();
			}
			else
			{
				if (ChangesPending)
					WriteRecord();

				this.deviceFileWatcher.EnableRaisingEvents &= this.device == null;
			}

		}

		private void Editor_ValueChanged(IWordEditor sender, Events.WordEditorValueChangedEventArgs args)
			=> ChangesPending = true;

		private void LoadWordEditor(IWordEditor editor, int index)
		{
			var deviceEditor = (DeviceWordEditor)editor;
			deviceEditor.DeviceWord = index >= 0 ? this.recordEditWords[index] : new FullWord(0L);
			deviceEditor.WordIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || this.device == device)
				return true;

			this.deviceFileWatcher.EnableRaisingEvents = false;

			FileBasedDevice oldDevice = this.device;
			this.device = device;
			InitDeviceFileWatcher();

			bool success = ApplyRecordCount() && LoadRecord(LoadErrorHandlingMode.ReportAlways);

			if (!success)
			{
				this.device = oldDevice;
				InitDeviceFileWatcher();
			}
			else
			{
				SetTruncateEnabledState();
			}

			return success;
		}

		public void UpdateSettings()
		{
			InitDeviceFileWatcher();

			if (Visible)
				LoadRecord();

			this.ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		private bool ApplyRecordCount()
		{
			if (!SupportsAppending)
				return true;

			FileStream stream = null;
			bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

			try
			{
				this.deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				long recordCount = 0;

				if (File.Exists(this.device.FilePath))
				{
					stream = this.openStream(this.device.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					recordCount = this.calculateRecordCount(stream);

					stream.Close();
				}

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				SetDeviceRecordCount(recordCount);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to determine file length: " + ex.Message, "Error determining file length", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				stream?.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		private void InitDeviceFileWatcher()
		{
			if (this.device != null)
			{
				this.deviceFileWatcher.Path = Path.GetDirectoryName(this.device.FilePath);
				this.deviceFileWatcher.Filter = Path.GetFileName(this.device.FilePath);

				this.deviceFileWatcher.EnableRaisingEvents |= Visible;
			}
		}

		private bool ChangesPending
		{
			get => this.changesPending;
			set
			{
				this.changesPending = value;

				this.revertButton.Enabled = this.changesPending;
				this.saveButton.Enabled = this.changesPending;
			}
		}

		private bool LoadRecord(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
				return true;

			if (this.device == null)
				return false;

			this.loading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

			try
			{
				var record = (long)this.recordUpDown.Value;

				if (watcherRaisesEvents)
					this.deviceFileWatcher.EnableRaisingEvents = false;

				stream = this.openStream(this.device.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = this.calculateBytePosition(record);

				var readWords = this.readWords(stream, this.device.RecordWordCount);

				stream.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				for (int i = 0; i < readWords.Length; i++)
				{
					this.recordReadWords[i] = readWords[i];
					this.recordEditWords[i].LongValue = readWords[i].LongValue;
				}

				ChangesPending = false;

				this.lastReadRecord = record;
				this.lastLoadTime = DateTime.Now;

				this.loadRecordRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (this.loadRecordRetryCount > 0 && this.loadRecordRetryCount <= MaxLoadSectorRetryCount))
				{
					this.delayedIOOperation = LoadRecord;
					this.loadRecordRetryCount++;
					this.ioDelayTimer.Start();

					return false;
				}

				if (this.loadRecordRetryCount > MaxLoadSectorRetryCount)
				{
					this.delayedIOOperation = null;
					this.loadRecordRetryCount = 0;

					return false;
				}

				if (errorHandlingMode == LoadErrorHandlingMode.ReportAlways || this.recordUpDown.Value != this.lastReadRecord)
					MessageBox.Show(this, "Unable to load device data: " + ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				stream?.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				this.loading = false;
			}

			return true;
		}

		public bool WriteRecord(bool force)
		{
			if (!this.changesPending)
				return true;

			if (this.readOnlyCheckBox.Checked || (!force && this.lastReadRecord == this.recordUpDown.Value))
				return false;

			FileStream stream = null;
			bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

			try
			{
				this.deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				stream = this.openStream(this.device.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = this.calculateBytePosition(this.lastReadRecord);

				this.writeWords(stream, this.device.RecordWordCount, this.recordEditWords);

				stream.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				ChangesPending = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to write device data: " + ex.Message, "Error writing data", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				stream?.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		public void UpdateLayout()
		{
			this.firstWordTextBox.UpdateLayout();
			this.mixCharButtons.UpdateLayout();
			this.wordEditorList.UpdateLayout();
		}

		private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.wordEditorList.ReadOnly = this.readOnlyCheckBox.Checked;

			if (this.readOnlyCheckBox.Checked && this.revertButton.Enabled)
				Revert();

			ProcessReadOnlySettings();
		}

		private void RecordTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (this.lastReadRecord == this.recordTrackBar.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				this.recordUpDown.Value = this.recordTrackBar.Value;
				SetTruncateEnabledState();
			}
			else
			{
				this.recordTrackBar.Value = (int)this.lastReadRecord;
			}
		}

		private void RecordUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (this.lastReadRecord == this.recordUpDown.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				this.recordTrackBar.Value = (int)this.recordUpDown.Value;
				SetTruncateEnabledState();
			}
			else
			{
				this.recordUpDown.Value = this.lastReadRecord;
			}
		}

		private void FirstWordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
			=> this.wordEditorList.FirstVisibleIndex = (int)args.NewValue;

		private void WordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
			=> this.firstWordTextBox.LongValue = this.wordEditorList.FirstVisibleIndex;

		private void Revert()
		{
			for (int i = 0; i < this.recordReadWords.Length; i++)
				this.recordEditWords[i].LongValue = this.recordReadWords[i].LongValue;

			ChangesPending = false;

			Update();
		}

		public bool ReadOnly
		{
			get => this.readOnlyCheckBox.Checked;
			set => this.readOnlyCheckBox.Checked = value;
		}

		public bool ResizeInProgress
		{
			get => this.wordEditorList.ResizeInProgress;
			set => this.wordEditorList.ResizeInProgress = value;
		}

		private void HandleFileWatcherEvent()
		{
			if (this.loading || this.ioDelayTimer.Enabled)
				return;

			if ((DateTime.Now - this.lastLoadTime).TotalMilliseconds > this.ioDelayTimer.Interval)
			{
				LoadRecord(LoadErrorHandlingMode.RetryOnError);
			}
			else
			{
				this.delayedIOOperation = LoadRecord;
				this.ioDelayTimer.Start();
			}
		}

		private void IODelayTimer_Tick(object sender, ElapsedEventArgs e)
		{
			this.ioDelayTimer.Stop();

			if (InvokeRequired)
				Invoke(this.delayedIOOperation);

			else
				this.delayedIOOperation();
		}

		private enum LoadErrorHandlingMode
		{
			ReportAlways,
			SilentIfSameRecord,
			RetryOnError
		}

		private void AppendButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount(this.deviceRecordCount + 1);

			this.recordUpDown.Value = this.deviceRecordCount - 1;

			if (this.recordUpDown.Value != this.deviceRecordCount - 1)
				SetDeviceRecordCount(this.deviceRecordCount - 1);
		}

		private void TruncateButton_Click(object sender, EventArgs e)
		{
			this.recordUpDown.Value = this.deviceRecordCount - 2;

			if (this.recordUpDown.Value == this.deviceRecordCount - 2)
			{
				bool success = false;

				FileStream stream = null;
				bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

				try
				{
					long recordCount = this.deviceRecordCount - 1;

					if (watcherRaisesEvents)
						this.deviceFileWatcher.EnableRaisingEvents = false;

					stream = this.openStream(this.device.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					stream.SetLength(this.calculateBytePosition(recordCount));
					stream.Close();

					this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

					stream = null;

					success = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to truncate device file: " + ex.Message, "Error truncating file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				finally
				{
					stream?.Close();

					this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
				}

				if (success)
					SetDeviceRecordCount(this.deviceRecordCount - 1);
			}

		}

		public void DeleteDeviceFile()
		{
			if (this.device == null || !File.Exists(this.device.FilePath))
				return;

			this.device.CloseStream();

			try
			{
				File.Delete(this.device.FilePath);

				this.device.Reset();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to delete device file: " + ex.Message, "Error deleting file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}

			this.recordUpDown.Value = 0;

			ApplyRecordCount();
			LoadRecord();
		}
	}
}
