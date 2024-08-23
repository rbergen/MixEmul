using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		private delegate bool DelayedIOOperation();

		private const int MaxLoadRetryCount = 5;

		private bool changesPending;
		private readonly List<IMixByteCollection> readBytes;
		private readonly List<IMixByteCollection> editBytes;
		private DelayedIOOperation delayedIOOperation;
		private int loadRetryCount;
		private int deviceBytesPerRecord;
		private OpenStreamCallback openStream;
		private ReadMixByteCallback readDeviceBytes;
		private WriteMixByteCallback writeDeviceBytes;
		private readonly int readOnlyGap;
		private int bottomButtonsGap;
		private readonly bool initialized;
		private bool showReadOnly;
		private readonly string recordLabelFormatString;
		private string recordName;
		private int indexCharCount;
		private DateTime lastLoadTime;
		private bool loading;

		public FileBasedDevice Device { get; private set; }
		public long DeviceRecordCount { get; private set; }

		public TextFileDeviceEditor()
		{
			InitializeComponent();

			this.mixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);

			this.readOnlyGap = this.firstRecordTextBox.Top;
			this.bottomButtonsGap = Height - this.mixByteCollectionEditorList.Bottom;
			this.showReadOnly = true;
			this.recordLabelFormatString = this.firstRecordLabel.Text;
			RecordName = "record";
			this.lastLoadTime = DateTime.Now;
			this.ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			this.readBytes = [];
			this.editBytes = [];

			SetDeviceRecordCount(1);

			this.initialized = false;
			this.loading = false;
		}

		public bool SupportsAppending 
			=> !this.readOnlyCheckBox.Checked;

		private bool LoadRecords() 
			=> LoadRecords(LoadErrorHandlingMode.AbortOnError);

		public new void Update() 
			=> this.mixByteCollectionEditorList.Update();

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
			if (this.initialized)
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");

			this.deviceBytesPerRecord = bytesPerRecord;
			this.openStream = openStream;
			this.readDeviceBytes = readBytes;
			this.writeDeviceBytes = writeBytes;

			this.firstRecordTextBox.ClearZero = false;
			this.firstRecordTextBox.MinValue = 0L;
			this.firstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			this.mixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;
			this.mixByteCollectionEditorList.CreateEditor = CreateMixByteCollectionEditor;
			this.mixByteCollectionEditorList.LoadEditor = LoadMixByteCollectionEditor;

			this.changesPending = false;

			this.loadRetryCount = 0;

			ProcessReadOnlySettings();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => this.recordName;
			set
			{
				this.recordName = value;

				this.firstRecordLabel.Text = string.Format(this.recordLabelFormatString, this.recordName);
			}
		}

		private void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			this.appendButton.Enabled = supportsAppending;
			this.truncateButton.Enabled = supportsAppending && DeviceRecordCount > 1;
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
					this.firstRecordLabel.Top += this.readOnlyGap;
					this.firstRecordTextBox.Top += this.readOnlyGap;
					this.setClipboardLabel.Top += this.readOnlyGap;
					this.mixCharButtons.Top += this.readOnlyGap;
					this.mixByteCollectionEditorList.Top += this.readOnlyGap;
					this.mixByteCollectionEditorList.Height -= this.readOnlyGap;
					this.bottomButtonsGap -= this.readOnlyGap;
				}
				else
				{
					this.firstRecordLabel.Top -= this.readOnlyGap;
					this.firstRecordTextBox.Top -= this.readOnlyGap;
					this.setClipboardLabel.Top -= this.readOnlyGap;
					this.mixCharButtons.Top -= this.readOnlyGap;
					this.mixByteCollectionEditorList.Top -= this.readOnlyGap;
					this.mixByteCollectionEditorList.Height += this.readOnlyGap;
					this.bottomButtonsGap += this.readOnlyGap;
				}

				ProcessReadOnlySettings();
			}
		}

		private void ProcessReadOnlySettings()
		{
			bool showBottomButtons = !this.mixByteCollectionEditorList.ReadOnly || this.showReadOnly;

			this.appendButton.Visible = showBottomButtons;
			this.truncateButton.Visible = showBottomButtons;
			this.revertButton.Visible = showBottomButtons;
			this.saveButton.Visible = showBottomButtons;
			this.setClipboardLabel.Visible = showBottomButtons;
			this.mixCharButtons.Visible = showBottomButtons;

			this.mixByteCollectionEditorList.Height = showBottomButtons ? (Height - this.mixByteCollectionEditorList.Top - this.bottomButtonsGap) : (Height - this.mixByteCollectionEditorList.Top);

			ProcessSupportsAppending();
		}

		private void SetDeviceRecordCount(int recordCount)
		{
			if (recordCount < 1)
				recordCount = 1;

			DeviceRecordCount = recordCount;

			this.mixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;

			AdaptListToRecordCount(this.editBytes);

			this.firstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			this.indexCharCount = (recordCount - 1).ToString().Length;

			foreach (DeviceMixByteCollectionEditor editor in this.mixByteCollectionEditorList.Cast<DeviceMixByteCollectionEditor>())
				editor.IndexCharCount = this.indexCharCount;

			ProcessSupportsAppending();

			Update();
		}

		private void AdaptListToRecordCount(List<IMixByteCollection> list)
		{
			while (list.Count < DeviceRecordCount)
				list.Add(new MixByteCollection(this.deviceBytesPerRecord));

			while (list.Count > DeviceRecordCount)
				list.RemoveAt(list.Count - 1);
		}

		private IMixByteCollectionEditor CreateMixByteCollectionEditor(int index)
		{
			var editor = new DeviceMixByteCollectionEditor(index == int.MinValue ? new MixByteCollection(this.deviceBytesPerRecord) : this.editBytes[index])
			{
				MixByteCollectionIndex = index,
				IndexCharCount = this.indexCharCount
			};

			editor.ValueChanged += Editor_ValueChanged;

			return editor;
		}

		private void ProcessVisibility()
		{
			if (Visible)
			{
				this.deviceFileWatcher.EnableRaisingEvents |= Device != null;

				LoadRecords();
			}
			else
			{
				if (ChangesPending)
					WriteRecords();

				this.deviceFileWatcher.EnableRaisingEvents &= Device == null;
			}

		}

		private void Editor_ValueChanged(IMixByteCollectionEditor sender, MixGui.Events.MixByteCollectionEditorValueChangedEventArgs args) => ChangesPending = true;

		private void LoadMixByteCollectionEditor(IMixByteCollectionEditor editor, int index)
		{
			var deviceEditor = (DeviceMixByteCollectionEditor)editor;
			deviceEditor.DeviceMixByteCollection = index >= 0 ? this.editBytes[index] : new MixByteCollection(this.deviceBytesPerRecord);
			deviceEditor.MixByteCollectionIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || Device == device)
				return true;

			this.deviceFileWatcher.EnableRaisingEvents = false;

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
				this.deviceFileWatcher.Path = Path.GetDirectoryName(Device.FilePath);
				this.deviceFileWatcher.Filter = Path.GetFileName(Device.FilePath);
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

		private bool LoadRecords(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
				return true;

			if (Device == null)
				return false;

			this.loading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

			try
			{
				this.deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				List<IMixByteCollection> readBytes = null;

				if (File.Exists(Device.FilePath))
				{
					stream = this.openStream(Device.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					readBytes = this.readDeviceBytes(stream, this.deviceBytesPerRecord);

					stream.Close();
				}
				else
					readBytes = [];

				if (readBytes.Count == 0)
					readBytes.Add(new MixByteCollection(this.deviceBytesPerRecord));

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				this.readBytes.Clear();
				this.editBytes.Clear();

				foreach (IMixByteCollection item in readBytes)
				{
					this.readBytes.Add(item);
					this.editBytes.Add((IMixByteCollection)item.Clone());
				}

				SetDeviceRecordCount(this.readBytes.Count);

				ChangesPending = false;

				this.lastLoadTime = DateTime.Now;
				this.loadRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (this.loadRetryCount > 0 && this.loadRetryCount <= MaxLoadRetryCount))
				{
					this.delayedIOOperation = LoadRecords;
					this.loadRetryCount++;
					this.ioDelayTimer.Start();

					return false;
				}

				if (this.loadRetryCount > MaxLoadRetryCount)
				{
					this.delayedIOOperation = null;
					this.loadRetryCount = 0;

					return false;
				}

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

		public bool WriteRecords()
		{
			if (!this.changesPending)
				return true;

			if (this.readOnlyCheckBox.Checked)
				return false;

			FileStream stream = null;
			bool watcherRaisesEvents = this.deviceFileWatcher.EnableRaisingEvents;

			try
			{
				this.deviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				stream = this.openStream(Device.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
				stream.SetLength(0);

				this.writeDeviceBytes(stream, this.deviceBytesPerRecord, this.editBytes);

				stream.Close();

				this.deviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				this.readBytes.Clear();

				foreach (IMixByteCollection item in this.editBytes)
					this.readBytes.Add((IMixByteCollection)item.Clone());

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

		public void UpdateSettings()
		{
			InitDeviceFileWatcher();

			if (Visible)
				LoadRecords();

			this.ioDelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		public void UpdateLayout()
		{
			this.firstRecordTextBox.UpdateLayout();
			this.mixCharButtons.UpdateLayout();
			this.mixByteCollectionEditorList.UpdateLayout();
			this.mixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
		}

		private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.mixByteCollectionEditorList.ReadOnly = this.readOnlyCheckBox.Checked;

			if (this.readOnlyCheckBox.Checked && this.revertButton.Enabled)
				Revert();

			ProcessReadOnlySettings();
		}

		private void FirstRecordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args) 
			=> this.mixByteCollectionEditorList.FirstVisibleIndex = (int)args.NewValue;

		private void MixByteCollectionEditorList_FirstVisibleIndexChanged(EditorList<IMixByteCollectionEditor> sender, MixByteCollectionEditorList.FirstVisibleIndexChangedEventArgs args) 
			=> this.firstRecordTextBox.LongValue = this.mixByteCollectionEditorList.FirstVisibleIndex;

		private void Revert()
		{
			this.editBytes.Clear();

			foreach (IMixByteCollection item in this.readBytes)
				this.editBytes.Add((IMixByteCollection)item.Clone());

			SetDeviceRecordCount(this.readBytes.Count);

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
			get => this.mixByteCollectionEditorList.ResizeInProgress;
			set => this.mixByteCollectionEditorList.ResizeInProgress = value;
		}

		private void HandleFileWatcherEvent()
		{
			if (this.loading || this.ioDelayTimer.Enabled)
				return;

			if ((DateTime.Now - this.lastLoadTime).TotalMilliseconds > this.ioDelayTimer.Interval)
				LoadRecords(LoadErrorHandlingMode.RetryOnError);

			else
			{
				this.delayedIOOperation = LoadRecords;
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
