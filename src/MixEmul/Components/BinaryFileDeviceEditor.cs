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

		private delegate bool delayedIOOperation();

		private const int MaxLoadSectorRetryCount = 5;

		private FileBasedDevice _device;
		private bool _changesPending;
		private IFullWord[] _recordReadWords;
		private IFullWord[] _recordEditWords;
		private long _lastReadRecord;
		private delayedIOOperation _delayedIOOperation;
		private int _loadRecordRetryCount;
		private long _deviceRecordCount;
		private long _deviceWordsPerRecord;
		private OpenStreamCallback _openStream;
		private CalculateBytePositionCallback _calculateBytePosition;
		private ReadWordsCallback _readWords;
		private WriteWordsCallback _writeWords;
		private CalculateRecordCountCallback _calculateRecordCount;
		private readonly int _readOnlyGap;
		private readonly bool _initialized;
		private bool _showReadOnly;
		private int _editorListWithButtonsHeight;
		private readonly string _recordLabelFormatString;
		private string _recordName;
		private DateTime _lastLoadTime;
		private bool _loading;

		public BinaryFileDeviceEditor()
		{
			InitializeComponent();

			_readOnlyGap = _recordUpDown.Top;
			_editorListWithButtonsHeight = _wordEditorList.Height;
			_recordLabelFormatString = _recordLabel.Text;
			_calculateRecordCount = null;
			RecordName = "record";
			_showReadOnly = true;
			_lastLoadTime = DateTime.Now;
			SetDeviceRecordCount(1);
			_ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			_initialized = false;
		}

		public long DeviceRecordCount
			=> _deviceRecordCount;

		public bool SupportsAppending
			=> _calculateRecordCount != null && !_readOnlyCheckBox.Checked;

		private bool LoadRecord()
			=> LoadRecord(LoadErrorHandlingMode.SilentIfSameRecord);

		public bool WriteRecord()
			=> WriteRecord(false);

		public new void Update()
			=> _wordEditorList.Update();

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
			if (_initialized)
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");

			_deviceWordsPerRecord = wordsPerRecord;
			_openStream = openStream;
			_calculateBytePosition = calculateBytePosition;
			_readWords = readWords;
			_writeWords = writeWords;
			_calculateRecordCount = calculateRecordCount;

			_recordUpDown.Minimum = 0;
			_recordTrackBar.Minimum = 0;
			SetDeviceRecordCount(recordCount);

			_firstWordTextBox.ClearZero = false;
			_firstWordTextBox.MinValue = 0L;
			_firstWordTextBox.MaxValue = _deviceWordsPerRecord - 1;

			_recordReadWords = new IFullWord[_deviceWordsPerRecord];
			_recordEditWords = new IFullWord[_deviceWordsPerRecord];

			for (int i = 0; i < _deviceWordsPerRecord; i++)
				_recordEditWords[i] = new FullWord();

			_wordEditorList.MaxIndex = (int)_deviceWordsPerRecord - 1;
			_wordEditorList.CreateEditor = CreateWordEditor;
			_wordEditorList.LoadEditor = LoadWordEditor;

			_changesPending = false;

			_loadRecordRetryCount = 0;

			ProcessSupportsAppending();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => _recordName;
			set
			{
				_recordName = value;

				_recordLabel.Text = string.Format(_recordLabelFormatString, _recordName);
			}
		}

		private void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			_appendButton.Visible = supportsAppending;
			_truncateButton.Visible = supportsAppending;
		}

		public bool ShowReadOnly
		{
			get => _showReadOnly;
			set
			{
				if (_showReadOnly == value)
					return;

				_showReadOnly = value;

				_readOnlyLabel.Visible = _showReadOnly;
				_readOnlyCheckBox.Visible = _showReadOnly;

				if (_showReadOnly)
				{
					_recordLabel.Top += _readOnlyGap;
					_recordUpDown.Top += _readOnlyGap;
					_recordTrackBar.Top += _readOnlyGap;
					_firstWordLabel.Top += _readOnlyGap;
					_firstWordTextBox.Top += _readOnlyGap;
					_setClipboardLabel.Top += _readOnlyGap;
					_mixCharButtons.Top += _readOnlyGap;
					_wordEditorList.Top += _readOnlyGap;
					_wordEditorList.Height -= _readOnlyGap;
					_editorListWithButtonsHeight -= _readOnlyGap;
				}
				else
				{
					_recordLabel.Top -= _readOnlyGap;
					_recordUpDown.Top -= _readOnlyGap;
					_recordTrackBar.Top -= _readOnlyGap;
					_firstWordLabel.Top -= _readOnlyGap;
					_firstWordTextBox.Top -= _readOnlyGap;
					_setClipboardLabel.Top -= _readOnlyGap;
					_mixCharButtons.Top -= _readOnlyGap;
					_wordEditorList.Top -= _readOnlyGap;
					_wordEditorList.Height += _readOnlyGap;
					_editorListWithButtonsHeight += _readOnlyGap;
				}

				ProcessReadOnlySettings();
			}
		}

		private void ProcessReadOnlySettings()
		{
			bool showSaveRevertButtons = !_wordEditorList.ReadOnly || _showReadOnly;

			_revertButton.Visible = showSaveRevertButtons;
			_saveButton.Visible = showSaveRevertButtons;

			ProcessSupportsAppending();
			ProcessButtonVisibility();
		}

		private void ProcessButtonVisibility()
			=> _wordEditorList.Height = _appendButton.Visible || _truncateButton.Visible || _revertButton.Visible || _saveButton.Visible
			? _editorListWithButtonsHeight
			: (Height - _wordEditorList.Top);

		private void SetDeviceRecordCount(long recordCount)
		{
			if (recordCount < 1)
				recordCount = 1;

			_deviceRecordCount = recordCount;

			_recordUpDown.Maximum = _deviceRecordCount - 1;
			_recordTrackBar.Maximum = (int)(_deviceRecordCount - 1);
			_recordTrackBar.LargeChange = (int)(_deviceRecordCount / 20);

			SetTruncateEnabledState();
		}

		private void SetTruncateEnabledState()
			=> _truncateButton.Enabled = _lastReadRecord == _deviceRecordCount - 1 && _deviceRecordCount > 1;

		private IWordEditor CreateWordEditor(int index)
		{
			var editor = new DeviceWordEditor(index == -1 ? new FullWord(0) : _recordEditWords[index])
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
				_deviceFileWatcher.EnableRaisingEvents |= _device != null;

				LoadRecord();
			}
			else
			{
				if (ChangesPending)
					WriteRecord();

				_deviceFileWatcher.EnableRaisingEvents &= _device == null;
			}

		}

		private void Editor_ValueChanged(IWordEditor sender, Events.WordEditorValueChangedEventArgs args)
			=> ChangesPending = true;

		private void LoadWordEditor(IWordEditor editor, int index)
		{
			var deviceEditor = (DeviceWordEditor)editor;
			deviceEditor.DeviceWord = index >= 0 ? _recordEditWords[index] : new FullWord(0L);
			deviceEditor.WordIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || _device == device)
				return true;

			_deviceFileWatcher.EnableRaisingEvents = false;

			FileBasedDevice oldDevice = _device;
			_device = device;
			InitDeviceFileWatcher();

			bool success = ApplyRecordCount() && LoadRecord(LoadErrorHandlingMode.ReportAlways);

			if (!success)
			{
				_device = oldDevice;
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

			_ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		private bool ApplyRecordCount()
		{
			if (!SupportsAppending)
				return true;

			FileStream stream = null;
			bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

			try
			{
				_deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				long recordCount = 0;

				if (File.Exists(_device.FilePath))
				{
					stream = _openStream(_device.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					recordCount = _calculateRecordCount(stream);

					stream.Close();
				}

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		private void InitDeviceFileWatcher()
		{
			if (_device != null)
			{
				_deviceFileWatcher.Path = Path.GetDirectoryName(_device.FilePath);
				_deviceFileWatcher.Filter = Path.GetFileName(_device.FilePath);

				_deviceFileWatcher.EnableRaisingEvents |= Visible;
			}
		}

		private bool ChangesPending
		{
			get => _changesPending;
			set
			{
				_changesPending = value;

				_revertButton.Enabled = _changesPending;
				_saveButton.Enabled = _changesPending;
			}
		}

		private bool LoadRecord(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
				return true;

			if (_device == null)
				return false;

			_loading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

			try
			{
				var record = (long)_recordUpDown.Value;

				if (watcherRaisesEvents)
					_deviceFileWatcher.EnableRaisingEvents = false;

				stream = _openStream(_device.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = _calculateBytePosition(record);

				var readWords = _readWords(stream, _device.RecordWordCount);

				stream.Close();

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				for (int i = 0; i < readWords.Length; i++)
				{
					_recordReadWords[i] = readWords[i];
					_recordEditWords[i].LongValue = readWords[i].LongValue;
				}

				ChangesPending = false;

				_lastReadRecord = record;
				_lastLoadTime = DateTime.Now;

				_loadRecordRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (_loadRecordRetryCount > 0 && _loadRecordRetryCount <= MaxLoadSectorRetryCount))
				{
					_delayedIOOperation = LoadRecord;
					_loadRecordRetryCount++;
					_ioDelayTimer.Start();

					return false;
				}

				if (_loadRecordRetryCount > MaxLoadSectorRetryCount)
				{
					_delayedIOOperation = null;
					_loadRecordRetryCount = 0;

					return false;
				}

				if (errorHandlingMode == LoadErrorHandlingMode.ReportAlways || _recordUpDown.Value != _lastReadRecord)
					MessageBox.Show(this, "Unable to load device data: " + ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				stream?.Close();

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				_loading = false;
			}

			return true;
		}

		public bool WriteRecord(bool force)
		{
			if (!_changesPending)
				return true;

			if (_readOnlyCheckBox.Checked || (!force && _lastReadRecord == _recordUpDown.Value))
				return false;

			FileStream stream = null;
			bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

			try
			{
				_deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				stream = _openStream(_device.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = _calculateBytePosition(_lastReadRecord);

				_writeWords(stream, _device.RecordWordCount, _recordEditWords);

				stream.Close();

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		public void UpdateLayout()
		{
			_firstWordTextBox.UpdateLayout();
			_mixCharButtons.UpdateLayout();
			_wordEditorList.UpdateLayout();
		}

		private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_wordEditorList.ReadOnly = _readOnlyCheckBox.Checked;

			if (_readOnlyCheckBox.Checked && _revertButton.Enabled)
				Revert();

			ProcessReadOnlySettings();
		}

		private void RecordTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (_lastReadRecord == _recordTrackBar.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				_recordUpDown.Value = _recordTrackBar.Value;
				SetTruncateEnabledState();
			}
			else
			{
				_recordTrackBar.Value = (int)_lastReadRecord;
			}
		}

		private void RecordUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_lastReadRecord == _recordUpDown.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				_recordTrackBar.Value = (int)_recordUpDown.Value;
				SetTruncateEnabledState();
			}
			else
			{
				_recordUpDown.Value = _lastReadRecord;
			}
		}

		private void FirstWordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
			=> _wordEditorList.FirstVisibleIndex = (int)args.NewValue;

		private void WordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
			=> _firstWordTextBox.LongValue = _wordEditorList.FirstVisibleIndex;

		private void Revert()
		{
			for (int i = 0; i < _recordReadWords.Length; i++)
				_recordEditWords[i].LongValue = _recordReadWords[i].LongValue;

			ChangesPending = false;

			Update();
		}

		public bool ReadOnly
		{
			get => _readOnlyCheckBox.Checked;
			set => _readOnlyCheckBox.Checked = value;
		}

		public bool ResizeInProgress
		{
			get => _wordEditorList.ResizeInProgress;
			set => _wordEditorList.ResizeInProgress = value;
		}

		private void HandleFileWatcherEvent()
		{
			if (_loading || _ioDelayTimer.Enabled)
				return;

			if ((DateTime.Now - _lastLoadTime).TotalMilliseconds > _ioDelayTimer.Interval)
			{
				LoadRecord(LoadErrorHandlingMode.RetryOnError);
			}
			else
			{
				_delayedIOOperation = LoadRecord;
				_ioDelayTimer.Start();
			}
		}

		private void IODelayTimer_Tick(object sender, ElapsedEventArgs e)
		{
			_ioDelayTimer.Stop();

			if (InvokeRequired)
				Invoke(_delayedIOOperation);

			else
				_delayedIOOperation();
		}

		private enum LoadErrorHandlingMode
		{
			ReportAlways,
			SilentIfSameRecord,
			RetryOnError
		}

		private void AppendButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount(_deviceRecordCount + 1);

			_recordUpDown.Value = _deviceRecordCount - 1;

			if (_recordUpDown.Value != _deviceRecordCount - 1)
				SetDeviceRecordCount(_deviceRecordCount - 1);
		}

		private void TruncateButton_Click(object sender, EventArgs e)
		{
			_recordUpDown.Value = _deviceRecordCount - 2;

			if (_recordUpDown.Value == _deviceRecordCount - 2)
			{
				bool success = false;

				FileStream stream = null;
				bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

				try
				{
					long recordCount = _deviceRecordCount - 1;

					if (watcherRaisesEvents)
						_deviceFileWatcher.EnableRaisingEvents = false;

					stream = _openStream(_device.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					stream.SetLength(_calculateBytePosition(recordCount));
					stream.Close();

					_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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

					_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
				}

				if (success)
					SetDeviceRecordCount(_deviceRecordCount - 1);
			}

		}

		public void DeleteDeviceFile()
		{
			if (_device == null || !File.Exists(_device.FilePath))
				return;

			_device.CloseStream();

			try
			{
				File.Delete(_device.FilePath);

				_device.Reset();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to delete device file: " + ex.Message, "Error deleting file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}

			_recordUpDown.Value = 0;

			ApplyRecordCount();
			LoadRecord();
		}
	}
}
