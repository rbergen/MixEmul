using MixGui.Settings;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class TextFileDeviceEditor : UserControl
	{
		public delegate FileStream OpenStreamCallback(string FilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
		public delegate List<IMixByteCollection> ReadMixByteCallback(Stream stream, int bytesPerRecord);
		public delegate void WriteMixByteCallback(Stream stream, int bytesPerRecord, List<IMixByteCollection> bytes);

		delegate bool delayedIOOperation();

		const int maxLoadRetryCount = 5;

		bool mChangesPending;
		readonly List<IMixByteCollection> mReadBytes;
		readonly List<IMixByteCollection> mEditBytes;
		delayedIOOperation mDelayedIOOperation;
		int mLoadRetryCount;
		int mDeviceBytesPerRecord;
		OpenStreamCallback mOpenStream;
		ReadMixByteCallback mReadDeviceBytes;
		WriteMixByteCallback mWriteDeviceBytes;
		readonly int mReadOnlyGap;
		int mBottomButtonsGap;
		readonly bool mInitialized;
		bool mShowReadOnly;
		readonly string mRecordLabelFormatString;
		string mRecordName;
		int mIndexCharCount;
		DateTime mLastLoadTime;
		bool mLoading;

		public FileBasedDevice Device { get; private set; }
		public long DeviceRecordCount { get; private set; }

		public TextFileDeviceEditor()
		{
			InitializeComponent();

			mMixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);

			mReadOnlyGap = mFirstRecordTextBox.Top;
			mBottomButtonsGap = Height - mMixByteCollectionEditorList.Bottom;
			mShowReadOnly = true;
			mRecordLabelFormatString = mFirstRecordLabel.Text;
			RecordName = "record";
			mLastLoadTime = DateTime.Now;
			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			mReadBytes = new List<IMixByteCollection>();
			mEditBytes = new List<IMixByteCollection>();

			SetDeviceRecordCount(1);

			mInitialized = false;
			mLoading = false;
		}

		public bool SupportsAppending => !mReadOnlyCheckBox.Checked;

		bool LoadRecords()
		{
			return LoadRecords(LoadErrorHandlingMode.AbortOnError);
		}

		public new void Update()
		{
			mMixByteCollectionEditorList.Update();
		}

		public bool Save()
		{
			return WriteRecords();
		}

		void MSaveButton_Click(object sender, EventArgs e)
		{
			WriteRecords();
		}

		void MRevertButton_Click(object sender, EventArgs e)
		{
			Revert();
		}

		void MDeviceFileWatcher_Event(object sender, FileSystemEventArgs e)
		{
			HandleFileWatcherEvent();
		}

		void DeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			ProcessVisibility();
		}

		void MDeviceFileWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			HandleFileWatcherEvent();
		}

		public void Initialize(int bytesPerRecord, OpenStreamCallback openStream, ReadMixByteCallback readBytes, WriteMixByteCallback writeBytes)
		{
			if (mInitialized)
			{
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");
			}

			mDeviceBytesPerRecord = bytesPerRecord;
			mOpenStream = openStream;
			mReadDeviceBytes = readBytes;
			mWriteDeviceBytes = writeBytes;

			mFirstRecordTextBox.ClearZero = false;
			mFirstRecordTextBox.MinValue = 0L;
			mFirstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			mMixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;
			mMixByteCollectionEditorList.CreateEditor = CreateMixByteCollectionEditor;
			mMixByteCollectionEditorList.LoadEditor = LoadMixByteCollectionEditor;

			mChangesPending = false;

			mLoadRetryCount = 0;

			ProcessReadOnlySettings();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => mRecordName;
			set
			{
				mRecordName = value;

				mFirstRecordLabel.Text = string.Format(mRecordLabelFormatString, mRecordName);
			}
		}

		void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			mAppendButton.Enabled = supportsAppending;
			mTruncateButton.Enabled = supportsAppending && DeviceRecordCount > 1;
		}

		public bool ShowReadOnly
		{
			get => mShowReadOnly;
			set
			{
				if (mShowReadOnly == value)
				{
					return;
				}

				mShowReadOnly = value;

				mReadOnlyLabel.Visible = mShowReadOnly;
				mReadOnlyCheckBox.Visible = mShowReadOnly;

				if (mShowReadOnly)
				{
					mFirstRecordLabel.Top += mReadOnlyGap;
					mFirstRecordTextBox.Top += mReadOnlyGap;
					mSetClipboardLabel.Top += mReadOnlyGap;
					mMixCharButtons.Top += mReadOnlyGap;
					mMixByteCollectionEditorList.Top += mReadOnlyGap;
					mMixByteCollectionEditorList.Height -= mReadOnlyGap;
					mBottomButtonsGap -= mReadOnlyGap;
				}
				else
				{
					mFirstRecordLabel.Top -= mReadOnlyGap;
					mFirstRecordTextBox.Top -= mReadOnlyGap;
					mSetClipboardLabel.Top -= mReadOnlyGap;
					mMixCharButtons.Top -= mReadOnlyGap;
					mMixByteCollectionEditorList.Top -= mReadOnlyGap;
					mMixByteCollectionEditorList.Height += mReadOnlyGap;
					mBottomButtonsGap += mReadOnlyGap;
				}

				ProcessReadOnlySettings();
			}
		}

		void ProcessReadOnlySettings()
		{
			bool showBottomButtons = !mMixByteCollectionEditorList.ReadOnly || mShowReadOnly;

			mAppendButton.Visible = showBottomButtons;
			mTruncateButton.Visible = showBottomButtons;
			mRevertButton.Visible = showBottomButtons;
			mSaveButton.Visible = showBottomButtons;
			mSetClipboardLabel.Visible = showBottomButtons;
			mMixCharButtons.Visible = showBottomButtons;

			mMixByteCollectionEditorList.Height = showBottomButtons ? (Height - mMixByteCollectionEditorList.Top - mBottomButtonsGap) : (Height - mMixByteCollectionEditorList.Top);

			ProcessSupportsAppending();
		}

		void SetDeviceRecordCount(int recordCount)
		{
			if (recordCount < 1)
			{
				recordCount = 1;
			}

			DeviceRecordCount = recordCount;

			mMixByteCollectionEditorList.MaxIndex = (int)DeviceRecordCount - 1;

			AdaptListToRecordCount(mEditBytes);

			mFirstRecordTextBox.MaxValue = DeviceRecordCount - 1;

			mIndexCharCount = (recordCount - 1).ToString().Length;
			foreach (DeviceMixByteCollectionEditor editor in mMixByteCollectionEditorList)
			{
				editor.IndexCharCount = mIndexCharCount;
			}

			ProcessSupportsAppending();

			Update();
		}

		void AdaptListToRecordCount(List<IMixByteCollection> list)
		{
			while (list.Count < DeviceRecordCount)
			{
				list.Add(new MixByteCollection(mDeviceBytesPerRecord));
			}

			while (list.Count > DeviceRecordCount)
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		IMixByteCollectionEditor CreateMixByteCollectionEditor(int index)
		{
			var editor = new DeviceMixByteCollectionEditor(index == int.MinValue ? new MixByteCollection(mDeviceBytesPerRecord) : mEditBytes[index])
			{
				MixByteCollectionIndex = index,
				IndexCharCount = mIndexCharCount
			};
			editor.ValueChanged += Editor_ValueChanged;

			return editor;
		}

		void ProcessVisibility()
		{
			if (Visible)
			{
				mDeviceFileWatcher.EnableRaisingEvents |= Device != null;

				LoadRecords();
			}
			else
			{
				if (ChangesPending)
				{
					WriteRecords();
				}

				mDeviceFileWatcher.EnableRaisingEvents &= Device == null;
			}

		}

		void Editor_ValueChanged(IMixByteCollectionEditor sender, MixGui.Events.MixByteCollectionEditorValueChangedEventArgs args)
		{
			ChangesPending = true;
		}

		void LoadMixByteCollectionEditor(IMixByteCollectionEditor editor, int index)
		{
			var deviceEditor = (DeviceMixByteCollectionEditor)editor;
			deviceEditor.DeviceMixByteCollection = index >= 0 ? mEditBytes[index] : new MixByteCollection(mDeviceBytesPerRecord);
			deviceEditor.MixByteCollectionIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || Device == device)
			{
				return true;
			}

			mDeviceFileWatcher.EnableRaisingEvents = false;

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
			{
				ProcessSupportsAppending();
			}

			return success;
		}

		void InitDeviceFileWatcher()
		{
			if (Device != null)
			{
				mDeviceFileWatcher.Path = Path.GetDirectoryName(Device.FilePath);
				mDeviceFileWatcher.Filter = Path.GetFileName(Device.FilePath);
				mDeviceFileWatcher.EnableRaisingEvents |= Visible;
			}
		}

		bool ChangesPending
		{
			get => mChangesPending;
			set
			{
				mChangesPending = value;

				mRevertButton.Enabled = mChangesPending;
				mSaveButton.Enabled = mChangesPending;
			}
		}

		bool LoadRecords(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
			{
				return true;
			}

			if (Device == null)
			{
				return false;
			}

			mLoading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

			try
			{
				mDeviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				List<IMixByteCollection> readBytes = null;

				if (File.Exists(Device.FilePath))
				{
					stream = mOpenStream(Device.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					readBytes = mReadDeviceBytes(stream, mDeviceBytesPerRecord);

					stream.Close();
				}
				else
				{
					readBytes = new List<IMixByteCollection>();
				}

				if (readBytes.Count == 0)
				{
					readBytes.Add(new MixByteCollection(mDeviceBytesPerRecord));
				}

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				mReadBytes.Clear();
				mEditBytes.Clear();

				foreach (IMixByteCollection item in readBytes)
				{
					mReadBytes.Add(item);
					mEditBytes.Add((IMixByteCollection)item.Clone());
				}

				SetDeviceRecordCount(mReadBytes.Count);

				ChangesPending = false;

				mLastLoadTime = DateTime.Now;
				mLoadRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (mLoadRetryCount > 0 && mLoadRetryCount <= maxLoadRetryCount))
				{
					mDelayedIOOperation = LoadRecords;
					mLoadRetryCount++;
					mIODelayTimer.Start();

					return false;
				}
				if (mLoadRetryCount > maxLoadRetryCount)
				{
					mDelayedIOOperation = null;
					mLoadRetryCount = 0;

					return false;
				}

				MessageBox.Show(this, "Unable to load device data: " + ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				stream?.Close();

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				mLoading = false;
			}

			return true;
		}

		public bool WriteRecords()
		{
			if (!mChangesPending)
			{
				return true;
			}

			if (mReadOnlyCheckBox.Checked)
			{
				return false;
			}

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

			try
			{
				mDeviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

				stream = mOpenStream(Device.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
				stream.SetLength(0);

				mWriteDeviceBytes(stream, mDeviceBytesPerRecord, mEditBytes);

				stream.Close();

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				mReadBytes.Clear();

				foreach (IMixByteCollection item in mEditBytes)
				{
					mReadBytes.Add((IMixByteCollection)item.Clone());
				}

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

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		public void UpdateSettings()
		{
			InitDeviceFileWatcher();

			if (Visible)
			{
				LoadRecords();
			}

			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		public void UpdateLayout()
		{
			mFirstRecordTextBox.UpdateLayout();
			mMixCharButtons.UpdateLayout();
			mMixByteCollectionEditorList.UpdateLayout();
			mMixByteCollectionEditorList.BackColor = GuiSettings.GetColor(GuiSettings.EditorBackground);
		}

		void MReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mMixByteCollectionEditorList.ReadOnly = mReadOnlyCheckBox.Checked;

			if (mReadOnlyCheckBox.Checked && mRevertButton.Enabled)
			{
				Revert();
			}

			ProcessReadOnlySettings();
		}

		void MFirstRecordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			mMixByteCollectionEditorList.FirstVisibleIndex = (int)args.NewValue;
		}

		void MMixByteCollectionEditorList_FirstVisibleIndexChanged(EditorList<IMixByteCollectionEditor> sender, MixByteCollectionEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			mFirstRecordTextBox.LongValue = mMixByteCollectionEditorList.FirstVisibleIndex;
		}

		void Revert()
		{
			mEditBytes.Clear();

			foreach (IMixByteCollection item in mReadBytes)
			{
				mEditBytes.Add((IMixByteCollection)item.Clone());
			}

			SetDeviceRecordCount(mReadBytes.Count);

			ChangesPending = false;

			Update();
		}

		public bool ReadOnly
		{
			get => mReadOnlyCheckBox.Checked;
			set => mReadOnlyCheckBox.Checked = value;
		}

		public bool ResizeInProgress
		{
			get => mMixByteCollectionEditorList.ResizeInProgress;
			set => mMixByteCollectionEditorList.ResizeInProgress = value;
		}

		void HandleFileWatcherEvent()
		{
			if (mLoading || mIODelayTimer.Enabled)
			{
				return;
			}

			if ((DateTime.Now - mLastLoadTime).TotalMilliseconds > mIODelayTimer.Interval)
			{
				LoadRecords(LoadErrorHandlingMode.RetryOnError);
			}
			else
			{
				mDelayedIOOperation = LoadRecords;
				mIODelayTimer.Start();
			}
		}

		void MIODelayTimer_Tick(object sender, ElapsedEventArgs e)
		{
			mIODelayTimer.Stop();

			if (InvokeRequired)
				Invoke(mDelayedIOOperation);
			else
				mDelayedIOOperation();
		}

		enum LoadErrorHandlingMode
		{
			AbortOnError,
			RetryOnError
		}

		void MAppendButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount((int)DeviceRecordCount + 1);

			ChangesPending = true;
		}

		void MTruncateButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount((int)DeviceRecordCount - 1);

			ChangesPending = true;
		}

		public void DeleteDeviceFile()
		{
			if (Device != null && File.Exists(Device.FilePath))
			{
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
		}

		internal void LoadDeviceFile(string filePath)
		{
			if (Device != null)
			{
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
}
