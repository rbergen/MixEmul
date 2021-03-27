using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class TextFileDeviceEditor : UserControl
	{
		public delegate FileStream OpenStreamCallback(string FilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
		public delegate List<IMixByteCollection> ReadMixByteCallback(Stream stream, int bytesPerRecord);
		public delegate void WriteMixByteCallback(Stream stream, int bytesPerRecord, List<IMixByteCollection> bytes);

		private delegate bool delayedIOOperation();

		private const int MaxLoadRetryCount = 5;

		private bool _changesPending;
		private readonly List<IMixByteCollection> _readBytes;
		private readonly List<IMixByteCollection> _editBytes;
		private delayedIOOperation _delayedIOOperation;
		private int _loadRetryCount;
		private int _deviceBytesPerRecord;
		private OpenStreamCallback _openStream;
		private ReadMixByteCallback _readDeviceBytes;
		private WriteMixByteCallback _writeDeviceBytes;
		private readonly int _readOnlyGap;
		private int _bottomButtonsGap;
		private readonly bool _initialized;
		private bool _showReadOnly;
		private readonly string _recordLabelFormatString;
		private string _recordName;
		private int _indexCharCount;
		private DateTime _lastLoadTime;
		private bool _loading;

		public FileBasedDevice Device { get; private set; }
		public long DeviceRecordCount { get; private set; }

		public TextFileDeviceEditor()
		{
			InitializeComponent();

			_mixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);

			_readOnlyGap = _firstRecordTextBox.Top;
			_bottomButtonsGap = Height - _mixByteCollectionEditorList.Bottom;
			_showReadOnly = true;
			_recordLabelFormatString = _firstRecordLabel.Text;
			RecordName = "record";
			_lastLoadTime = DateTime.Now;
			_ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			_readBytes = new List<IMixByteCollection>();
			_editBytes = new List<IMixByteCollection>();

			SetDeviceRecordCount(1);

			_initialized = false;
			_loading = false;
		}

		public bool SupportsAppending 
			=> !_readOnlyCheckBox.Checked;

		private bool LoadRecords() 
			=> LoadRecords(LoadErrorHandlingMode.AbortOnError);

		public new void Update() 
			=> _mixByteCollectionEditorList.Update();

		public bool Save() 
			=> WriteRecords();

		private void SaveButton_Click(object sender, EventArgs e) 
			=> WriteRecords();

		private void RevertButton_Click(object sender, EventArgs e) 
			=> Revert();

		private void DeviceFileWatcher_Event(object sender, FileSystemEventArgs e) 
			=> HandleFileWatcherEvent();

		private void DeviceEditor_VisibleChanged(object sender, EventArgs e) 
			=> ProcessVisibility();

		private void DeviceFileWatcher_Renamed(object sender, RenamedEventArgs e) 
			=> HandleFileWatcherEvent();

		public void Initialize(int bytesPerRecord, OpenStreamCallback openStream, ReadMixByteCallback readBytes, WriteMixByteCallback writeBytes)
		{
			if (_initialized)
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");

			_deviceBytesPerRecord = bytesPerRecord;
			_openStream = openStream;
			_readDeviceBytes = readBytes;
			_writeDeviceBytes = writeBytes;

			_firstRecordTextBox.ClearZero = false;
			_firstRecordTextBox.MinValue = 0L;
			_firstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			_mixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;
			_mixByteCollectionEditorList.CreateEditor = CreateMixByteCollectionEditor;
			_mixByteCollectionEditorList.LoadEditor = LoadMixByteCollectionEditor;

			_changesPending = false;

			_loadRetryCount = 0;

			ProcessReadOnlySettings();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => _recordName;
			set
			{
				_recordName = value;

				_firstRecordLabel.Text = string.Format(_recordLabelFormatString, _recordName);
			}
		}

		private void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			_appendButton.Enabled = supportsAppending;
			_truncateButton.Enabled = supportsAppending && DeviceRecordCount > 1;
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
					_firstRecordLabel.Top += _readOnlyGap;
					_firstRecordTextBox.Top += _readOnlyGap;
					_setClipboardLabel.Top += _readOnlyGap;
					_mixCharButtons.Top += _readOnlyGap;
					_mixByteCollectionEditorList.Top += _readOnlyGap;
					_mixByteCollectionEditorList.Height -= _readOnlyGap;
					_bottomButtonsGap -= _readOnlyGap;
				}
				else
				{
					_firstRecordLabel.Top -= _readOnlyGap;
					_firstRecordTextBox.Top -= _readOnlyGap;
					_setClipboardLabel.Top -= _readOnlyGap;
					_mixCharButtons.Top -= _readOnlyGap;
					_mixByteCollectionEditorList.Top -= _readOnlyGap;
					_mixByteCollectionEditorList.Height += _readOnlyGap;
					_bottomButtonsGap += _readOnlyGap;
				}

				ProcessReadOnlySettings();
			}
		}

		private void ProcessReadOnlySettings()
		{
			bool showBottomButtons = !_mixByteCollectionEditorList.ReadOnly || _showReadOnly;

			_appendButton.Visible = showBottomButtons;
			_truncateButton.Visible = showBottomButtons;
			_revertButton.Visible = showBottomButtons;
			_saveButton.Visible = showBottomButtons;
			_setClipboardLabel.Visible = showBottomButtons;
			_mixCharButtons.Visible = showBottomButtons;

			_mixByteCollectionEditorList.Height = showBottomButtons ? (Height - _mixByteCollectionEditorList.Top - _bottomButtonsGap) : (Height - _mixByteCollectionEditorList.Top);

			ProcessSupportsAppending();
		}

		private void SetDeviceRecordCount(int recordCount)
		{
			if (recordCount < 1)
				recordCount = 1;

			DeviceRecordCount = recordCount;

			_mixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;

			AdaptListToRecordCount(_editBytes);

			_firstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			_indexCharCount = (recordCount - 1).ToString().Length;

			foreach (DeviceMixByteCollectionEditor editor in _mixByteCollectionEditorList)
				editor.IndexCharCount = _indexCharCount;

			ProcessSupportsAppending();

			Update();
		}

		private void AdaptListToRecordCount(List<IMixByteCollection> list)
		{
			while (list.Count < DeviceRecordCount)
				list.Add(new MixByteCollection(_deviceBytesPerRecord));

			while (list.Count > DeviceRecordCount)
				list.RemoveAt(list.Count - 1);
		}

		private IMixByteCollectionEditor CreateMixByteCollectionEditor(int index)
		{
			var editor = new DeviceMixByteCollectionEditor(index == int.MinValue ? new MixByteCollection(_deviceBytesPerRecord) : _editBytes[index])
			{
				MixByteCollectionIndex = index,
				IndexCharCount = _indexCharCount
			};

			editor.ValueChanged += Editor_ValueChanged;

			return editor;
		}

		private void ProcessVisibility()
		{
			if (Visible)
			{
				_deviceFileWatcher.EnableRaisingEvents |= Device != null;

				LoadRecords();
			}
			else
			{
				if (ChangesPending)
					WriteRecords();

				_deviceFileWatcher.EnableRaisingEvents &= Device == null;
			}

		}

		private void Editor_ValueChanged(IMixByteCollectionEditor sender, MixGui.Events.MixByteCollectionEditorValueChangedEventArgs args) => ChangesPending = true;

		private void LoadMixByteCollectionEditor(IMixByteCollectionEditor editor, int index)
		{
			var deviceEditor = (DeviceMixByteCollectionEditor)editor;
			deviceEditor.DeviceMixByteCollection = index >= 0 ? _editBytes[index] : new MixByteCollection(_deviceBytesPerRecord);
			deviceEditor.MixByteCollectionIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || Device == device)
				return true;

			_deviceFileWatcher.EnableRaisingEvents = false;

			FileBasedDevice oldDevice = Device;
			Device = device;
			InitDeviceFileWatcher();

			var success = LoadRecords(LoadErrorHandlingMode.AbortOnError);

			if (!success)
			{
				Device = oldDevice;
				InitDeviceFileWatcher();
			}
			else
				ProcessSupportsAppending();

			return success;
		}

		private void InitDeviceFileWatcher()
		{
			if (Device != null)
			{
				_deviceFileWatcher.Path = Path.GetDirectoryName(Device.FilePath);
				_deviceFileWatcher.Filter = Path.GetFileName(Device.FilePath);
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

		private bool LoadRecords(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
				return true;

			if (Device == null)
				return false;

			_loading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

			try
			{
				_deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				List<IMixByteCollection> readBytes = null;

				if (File.Exists(Device.FilePath))
				{
					stream = _openStream(Device.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					readBytes = _readDeviceBytes(stream, _deviceBytesPerRecord);

					stream.Close();
				}
				else
					readBytes = new List<IMixByteCollection>();

				if (readBytes.Count == 0)
					readBytes.Add(new MixByteCollection(_deviceBytesPerRecord));

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				_readBytes.Clear();
				_editBytes.Clear();

				foreach (IMixByteCollection item in readBytes)
				{
					_readBytes.Add(item);
					_editBytes.Add((IMixByteCollection)item.Clone());
				}

				SetDeviceRecordCount(_readBytes.Count);

				ChangesPending = false;

				_lastLoadTime = DateTime.Now;
				_loadRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (_loadRetryCount > 0 && _loadRetryCount <= MaxLoadRetryCount))
				{
					_delayedIOOperation = LoadRecords;
					_loadRetryCount++;
					_ioDelayTimer.Start();

					return false;
				}

				if (_loadRetryCount > MaxLoadRetryCount)
				{
					_delayedIOOperation = null;
					_loadRetryCount = 0;

					return false;
				}

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

		public bool WriteRecords()
		{
			if (!_changesPending)
				return true;

			if (_readOnlyCheckBox.Checked)
				return false;

			FileStream stream = null;
			bool watcherRaisesEvents = _deviceFileWatcher.EnableRaisingEvents;

			try
			{
				_deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				stream = _openStream(Device.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
				stream.SetLength(0);

				_writeDeviceBytes(stream, _deviceBytesPerRecord, _editBytes);

				stream.Close();

				_deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				_readBytes.Clear();

				foreach (IMixByteCollection item in _editBytes)
					_readBytes.Add((IMixByteCollection)item.Clone());

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

		public void UpdateSettings()
		{
			InitDeviceFileWatcher();

			if (Visible)
				LoadRecords();

			_ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		public void UpdateLayout()
		{
			_firstRecordTextBox.UpdateLayout();
			_mixCharButtons.UpdateLayout();
			_mixByteCollectionEditorList.UpdateLayout();
			_mixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
		}

		private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_mixByteCollectionEditorList.ReadOnly = _readOnlyCheckBox.Checked;

			if (_readOnlyCheckBox.Checked && _revertButton.Enabled)
				Revert();

			ProcessReadOnlySettings();
		}

		private void FirstRecordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args) 
			=> _mixByteCollectionEditorList.FirstVisibleIndex = (int)args.NewValue;

		private void MixByteCollectionEditorList_FirstVisibleIndexChanged(EditorList<IMixByteCollectionEditor> sender, MixByteCollectionEditorList.FirstVisibleIndexChangedEventArgs args) 
			=> _firstRecordTextBox.LongValue = _mixByteCollectionEditorList.FirstVisibleIndex;

		private void Revert()
		{
			_editBytes.Clear();

			foreach (IMixByteCollection item in _readBytes)
				_editBytes.Add((IMixByteCollection)item.Clone());

			SetDeviceRecordCount(_readBytes.Count);

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
			get => _mixByteCollectionEditorList.ResizeInProgress;
			set => _mixByteCollectionEditorList.ResizeInProgress = value;
		}

		private void HandleFileWatcherEvent()
		{
			if (_loading || _ioDelayTimer.Enabled)
				return;

			if ((DateTime.Now - _lastLoadTime).TotalMilliseconds > _ioDelayTimer.Interval)
				LoadRecords(LoadErrorHandlingMode.RetryOnError);

			else
			{
				_delayedIOOperation = LoadRecords;
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
			AbortOnError,
			RetryOnError
		}

		private void AppendButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount((int)DeviceRecordCount + 1);

			ChangesPending = true;
		}

		private void TruncateButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount((int)DeviceRecordCount - 1);

			ChangesPending = true;
		}

		public void DeleteDeviceFile()
		{
			if (Device == null || !File.Exists(Device.FilePath))
				return;

			Device.CloseStream();

			try
			{
				File.Delete(Device.FilePath);

				Device.Reset();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to delete device file: " + ex.Message, "Error deleting file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}

			LoadRecords();
		}

		internal void LoadDeviceFile(string filePath)
		{
			if (Device == null)
				return;

			Device.CloseStream();

			try
			{
				File.Copy(filePath, Device.FilePath, true);

				Device.Reset();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to load device file: " + ex.Message, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}

			LoadRecords();
		}
	}
}
