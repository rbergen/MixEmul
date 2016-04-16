using System;
using System.Collections.Generic;
using System.IO;
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

		private const int maxLoadRetryCount = 5;

		private FileBasedDevice mDevice;
		private bool mChangesPending;
		private List<IMixByteCollection> mReadBytes;
		private List<IMixByteCollection> mEditBytes;
		private delayedIOOperation mDelayedIOOperation;
		private int mLoadRetryCount;
		private int mDeviceRecordCount;
		private int mDeviceBytesPerRecord;
		private OpenStreamCallback mOpenStream;
		private ReadMixByteCallback mReadDeviceBytes;
		private WriteMixByteCallback mWriteDeviceBytes;
		private int mReadOnlyGap;
		private int mBottomButtonsGap;
		private bool mInitialized;
		private bool mShowReadOnly;
		private string mRecordLabelFormatString;
		private string mRecordName;
		private int mIndexCharCount;
		private DateTime mLastLoadTime;
		private bool mLoading;

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

			setDeviceRecordCount(1);

			mInitialized = false;
			mLoading = false;
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
			mFirstRecordTextBox.MaxValue = mDeviceRecordCount - 1;

			mMixByteCollectionEditorList.MaxIndex = (int)mDeviceRecordCount - 1;
			mMixByteCollectionEditorList.CreateEditor = createMixByteCollectionEditor;
			mMixByteCollectionEditorList.LoadEditor = loadMixByteCollectionEditor;

			mChangesPending = false;

			mLoadRetryCount = 0;

			processReadOnlySettings();
			processVisibility();
		}

		public string RecordName
		{
			get
			{
				return mRecordName;
			}
			set
			{
				mRecordName = value;

				mFirstRecordLabel.Text = string.Format(mRecordLabelFormatString, mRecordName);
			}
		}

		public bool SupportsAppending
		{
			get
			{
				return !mReadOnlyCheckBox.Checked;
			}
		}

		private void processSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			mAppendButton.Enabled = supportsAppending;
			mTruncateButton.Enabled = supportsAppending ? mDeviceRecordCount > 1 : false;
		}

		public bool ShowReadOnly
		{
			get
			{
				return mShowReadOnly;
			}
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

				processReadOnlySettings();
			}
		}

		private void processReadOnlySettings()
		{
			bool showBottomButtons = !mMixByteCollectionEditorList.ReadOnly || mShowReadOnly;

			mAppendButton.Visible = showBottomButtons;
			mTruncateButton.Visible = showBottomButtons;
			mRevertButton.Visible = showBottomButtons;
			mSaveButton.Visible = showBottomButtons;
			mSetClipboardLabel.Visible = showBottomButtons;
			mMixCharButtons.Visible = showBottomButtons;

			mMixByteCollectionEditorList.Height = showBottomButtons ? (Height - mMixByteCollectionEditorList.Top - mBottomButtonsGap) : (Height - mMixByteCollectionEditorList.Top);

			processSupportsAppending();
		}

		public long DeviceRecordCount
		{
			get
			{
				return mDeviceRecordCount;
			}
		}

		private void setDeviceRecordCount(int recordCount)
		{
			if (recordCount < 1)
			{
				recordCount = 1;
			}

			mDeviceRecordCount = recordCount;

			mMixByteCollectionEditorList.MaxIndex = mDeviceRecordCount - 1;

			adaptListToRecordCount(mEditBytes);

			mFirstRecordTextBox.MaxValue = mDeviceRecordCount - 1;

			mIndexCharCount = (recordCount - 1).ToString().Length;
			foreach (DeviceMixByteCollectionEditor editor in mMixByteCollectionEditorList)
			{
				editor.IndexCharCount = mIndexCharCount;
			}

			processSupportsAppending();

			Update();
		}

		private void adaptListToRecordCount(List<IMixByteCollection> list)
		{
			while (list.Count < mDeviceRecordCount)
			{
				list.Add(new MixByteCollection(mDeviceBytesPerRecord));
			}

			while (list.Count > mDeviceRecordCount)
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		private IMixByteCollectionEditor createMixByteCollectionEditor(int index)
		{
			DeviceMixByteCollectionEditor editor = new DeviceMixByteCollectionEditor(index == int.MinValue ? new MixByteCollection(mDeviceBytesPerRecord) : mEditBytes[index]);
			editor.MixByteCollectionIndex = index;
			editor.IndexCharCount = mIndexCharCount;
			editor.ValueChanged += new MixGui.Events.MixByteCollectionEditorValueChangedEventHandler(editor_ValueChanged);

			return editor;
		}

		private void processVisibility()
		{
			if (Visible)
			{
				if (mDevice != null)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				loadRecords();
			}
			else
			{
				if (changesPending)
				{
					writeRecords();
				}

				if (mDevice != null)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}
			}

		}

		private void editor_ValueChanged(IMixByteCollectionEditor sender, MixGui.Events.MixByteCollectionEditorValueChangedEventArgs args)
		{
			changesPending = true;
		}

		private void loadMixByteCollectionEditor(IMixByteCollectionEditor editor, int index)
		{
			DeviceMixByteCollectionEditor deviceEditor = (DeviceMixByteCollectionEditor)editor;
			deviceEditor.DeviceMixByteCollection = index >= 0 ? mEditBytes[index] : new MixByteCollection(mDeviceBytesPerRecord);
			deviceEditor.MixByteCollectionIndex = index;
		}

		public FileBasedDevice Device
		{
			get
			{
				return mDevice;
			}
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || mDevice == device)
			{
				return true;
			}

			mDeviceFileWatcher.EnableRaisingEvents = false;

			FileBasedDevice oldDevice = mDevice;
			mDevice = device;
			initDeviceFileWatcher();

			bool success = loadRecords(loadErrorHandlingMode.AbortOnError);

			if (!success)
			{
				mDevice = oldDevice;
				initDeviceFileWatcher();
			}
			else
			{
				processSupportsAppending();
			}

			return success;
		}

		private void initDeviceFileWatcher()
		{
			if (mDevice != null)
			{
				mDeviceFileWatcher.Path = Path.GetDirectoryName(mDevice.FilePath);
				mDeviceFileWatcher.Filter = Path.GetFileName(mDevice.FilePath);
				if (Visible)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}
			}
		}

		private bool changesPending
		{
			get
			{
				return mChangesPending;
			}
			set
			{
				mChangesPending = value;

				mRevertButton.Enabled = mChangesPending;
				mSaveButton.Enabled = mChangesPending;
			}
		}

		private bool loadRecords()
		{
			return loadRecords(loadErrorHandlingMode.AbortOnError);
		}

		private bool loadRecords(loadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
			{
				return true;
			}

			if (mDevice == null)
			{
				return false;
			}

			mLoading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

			try
			{
				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}

				List<IMixByteCollection> readBytes = null;

				if (File.Exists(mDevice.FilePath))
				{
					stream = mOpenStream(mDevice.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				stream = null;

				mReadBytes.Clear();
				mEditBytes.Clear();

				foreach (IMixByteCollection item in readBytes)
				{
					mReadBytes.Add(item);
					mEditBytes.Add((IMixByteCollection)item.Clone());
				}

				setDeviceRecordCount(mReadBytes.Count);

				changesPending = false;

				mLastLoadTime = DateTime.Now;
				mLoadRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == loadErrorHandlingMode.RetryOnError || (mLoadRetryCount > 0 && mLoadRetryCount <= maxLoadRetryCount))
				{
					mDelayedIOOperation = loadRecords;
					mLoadRetryCount++;
					mIODelayTimer.Start();

					return false;
				}
				else if (mLoadRetryCount > maxLoadRetryCount)
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
				if (stream != null)
				{
					try
					{
						stream.Close();
					}
					catch (Exception)
					{
					}
				}

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				mLoading = false;
			}

			return true;
		}

		public bool writeRecords()
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
				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}

				stream = mOpenStream(mDevice.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
				stream.SetLength(0);

				mWriteDeviceBytes(stream, mDeviceBytesPerRecord, mEditBytes);

				stream.Close();

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				stream = null;

				mReadBytes.Clear();

				foreach (IMixByteCollection item in mEditBytes)
				{
					mReadBytes.Add((IMixByteCollection)item.Clone());
				}

				changesPending = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to write device data: " + ex.Message, "Error writing data", MessageBoxButtons.OK, MessageBoxIcon.Hand);

				return false;
			}
			finally
			{
				if (stream != null)
				{
					try
					{
						stream.Close();
					}
					catch (Exception)
					{
					}
				}

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}
			}

			return true;
		}

		public new void Update()
		{
			mMixByteCollectionEditorList.Update();
		}

		public void UpdateSettings()
		{
			initDeviceFileWatcher();

			if (Visible)
			{
				loadRecords();
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

		private void mReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mMixByteCollectionEditorList.ReadOnly = mReadOnlyCheckBox.Checked;

			if (mReadOnlyCheckBox.Checked && mRevertButton.Enabled)
			{
				revert();
			}

			processReadOnlySettings();
		}

		public bool Save()
		{
			return writeRecords();
		}

		private void mFirstRecordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			mMixByteCollectionEditorList.FirstVisibleIndex = (int)args.NewValue;
		}

		private void mMixByteCollectionEditorList_FirstVisibleIndexChanged(EditorList<IMixByteCollectionEditor> sender, MixByteCollectionEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			mFirstRecordTextBox.LongValue = mMixByteCollectionEditorList.FirstVisibleIndex;
		}

		private void mSaveButton_Click(object sender, EventArgs e)
		{
			writeRecords();
		}

		private void mRevertButton_Click(object sender, EventArgs e)
		{
			revert();
		}

		private void revert()
		{
			mEditBytes.Clear();

			foreach (IMixByteCollection item in mReadBytes)
			{
				mEditBytes.Add((IMixByteCollection)item.Clone());
			}

			setDeviceRecordCount(mReadBytes.Count);

			changesPending = false;

			Update();
		}

		public bool ReadOnly
		{
			get
			{
				return mReadOnlyCheckBox.Checked;
			}
			set
			{
				mReadOnlyCheckBox.Checked = value;
			}
		}

		public bool ResizeInProgress
		{
			get
			{
				return mMixByteCollectionEditorList.ResizeInProgress;
			}
			set
			{
				mMixByteCollectionEditorList.ResizeInProgress = value;
			}
		}

		private void mDeviceFileWatcher_Event(object sender, FileSystemEventArgs e)
		{
			handleFileWatcherEvent();
		}

		private void handleFileWatcherEvent()
		{
			if (mLoading || mIODelayTimer.Enabled)
			{
				return;
			}

			if ((DateTime.Now - mLastLoadTime).TotalMilliseconds > mIODelayTimer.Interval)
			{
				loadRecords(loadErrorHandlingMode.RetryOnError);
			}
			else
			{
				mDelayedIOOperation = loadRecords;
				mIODelayTimer.Start();
			}
		}

		private void DeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			processVisibility();
		}

		private void mDeviceFileWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			handleFileWatcherEvent();
		}

		private void mIODelayTimer_Tick(object sender, EventArgs e)
		{
			mIODelayTimer.Stop();

			mDelayedIOOperation();
		}

		private enum loadErrorHandlingMode
		{
			AbortOnError,
			RetryOnError
		}

		private void mAppendButton_Click(object sender, EventArgs e)
		{
			setDeviceRecordCount(mDeviceRecordCount + 1);

			changesPending = true;
		}

		private void mTruncateButton_Click(object sender, EventArgs e)
		{
			setDeviceRecordCount(mDeviceRecordCount - 1);

			changesPending = true;
		}

		public void DeleteDeviceFile()
		{
			if (mDevice != null && File.Exists(mDevice.FilePath))
			{
				mDevice.CloseStream();

				try
				{
					File.Delete(mDevice.FilePath);

					mDevice.Reset();
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to delete device file: " + ex.Message, "Error deleting file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}

				loadRecords();
			}
		}

		internal void LoadDeviceFile(string filePath)
		{
			if (mDevice != null)
			{
				mDevice.CloseStream();

				try
				{
					File.Copy(filePath, mDevice.FilePath, true);

					mDevice.Reset();
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to load device file: " + ex.Message, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}

				loadRecords();
			}
		}
	}
}
