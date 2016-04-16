using System;
using System.IO;
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

		private const int maxLoadSectorRetryCount = 5;

		private FileBasedDevice mDevice;
		private bool mChangesPending;
		private IFullWord[] mRecordReadWords;
		private IFullWord[] mRecordEditWords;
		private long mLastReadRecord;
		private delayedIOOperation mDelayedIOOperation;
		private int mLoadRecordRetryCount;
		private long mDeviceRecordCount;
		private long mDeviceWordsPerRecord;
		private OpenStreamCallback mOpenStream;
		private CalculateBytePositionCallback mCalculateBytePosition;
		private ReadWordsCallback mReadWords;
		private WriteWordsCallback mWriteWords;
		private CalculateRecordCountCallback mCalculateRecordCount;
		private int mReadOnlyGap;
		private bool mInitialized;
		private bool mShowReadOnly;
		private int mEditorListWithButtonsHeight;
		private string mRecordLabelFormatString;
		private string mRecordName;
		private DateTime mLastLoadTime;
		private bool mLoading;


		public BinaryFileDeviceEditor()
		{
			InitializeComponent();

			mReadOnlyGap = mRecordUpDown.Top;
			mEditorListWithButtonsHeight = mWordEditorList.Height;
			mRecordLabelFormatString = mRecordLabel.Text;
			mCalculateRecordCount = null;
			RecordName = "record";
			mShowReadOnly = true;
			mLastLoadTime = DateTime.Now;
			setDeviceRecordCount(1);
			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			mInitialized = false;
		}

		public void Initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
		{
			initialize(recordCount, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, null);
		}

		public void Initialize(CalculateRecordCountCallback calculateRecordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
		{
			initialize(1, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, calculateRecordCount);
		}

		private void initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords, CalculateRecordCountCallback calculateRecordCount)
		{
			if (mInitialized)
			{
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");
			}

			mDeviceWordsPerRecord = wordsPerRecord;
			mOpenStream = openStream;
			mCalculateBytePosition = calculateBytePosition;
			mReadWords = readWords;
			mWriteWords = writeWords;
			mCalculateRecordCount = calculateRecordCount;

			mRecordUpDown.Minimum = 0;
			mRecordTrackBar.Minimum = 0;
			setDeviceRecordCount(recordCount);

			mFirstWordTextBox.ClearZero = false;
			mFirstWordTextBox.MinValue = 0L;
			mFirstWordTextBox.MaxValue = mDeviceWordsPerRecord - 1;

			mRecordReadWords = new IFullWord[mDeviceWordsPerRecord];
			mRecordEditWords = new IFullWord[mDeviceWordsPerRecord];
			for (int i = 0; i < mDeviceWordsPerRecord; i++)
			{
				mRecordEditWords[i] = new FullWord();
			}

			mWordEditorList.MaxIndex = (int)mDeviceWordsPerRecord - 1;
			mWordEditorList.CreateEditor = createWordEditor;
			mWordEditorList.LoadEditor = loadWordEditor;

			mChangesPending = false;

			mLoadRecordRetryCount = 0;

			processSupportsAppending();
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

				mRecordLabel.Text = string.Format(mRecordLabelFormatString, mRecordName);
			}
		}

		public bool SupportsAppending
		{
			get
			{
				return mCalculateRecordCount != null && !mReadOnlyCheckBox.Checked;
			}
		}

		private void processSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			mAppendButton.Visible = supportsAppending;
			mTruncateButton.Visible = supportsAppending;
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
					mRecordLabel.Top += mReadOnlyGap;
					mRecordUpDown.Top += mReadOnlyGap;
					mRecordTrackBar.Top += mReadOnlyGap;
					mFirstWordLabel.Top += mReadOnlyGap;
					mFirstWordTextBox.Top += mReadOnlyGap;
					mSetClipboardLabel.Top += mReadOnlyGap;
					mMixCharButtons.Top += mReadOnlyGap;
					mWordEditorList.Top += mReadOnlyGap;
					mWordEditorList.Height -= mReadOnlyGap;
					mEditorListWithButtonsHeight -= mReadOnlyGap;
				}
				else
				{
					mRecordLabel.Top -= mReadOnlyGap;
					mRecordUpDown.Top -= mReadOnlyGap;
					mRecordTrackBar.Top -= mReadOnlyGap;
					mFirstWordLabel.Top -= mReadOnlyGap;
					mFirstWordTextBox.Top -= mReadOnlyGap;
					mSetClipboardLabel.Top -= mReadOnlyGap;
					mMixCharButtons.Top -= mReadOnlyGap;
					mWordEditorList.Top -= mReadOnlyGap;
					mWordEditorList.Height += mReadOnlyGap;
					mEditorListWithButtonsHeight += mReadOnlyGap;
				}

				processReadOnlySettings();
			}
		}

		private void processReadOnlySettings()
		{
			bool showSaveRevertButtons = !mWordEditorList.ReadOnly || mShowReadOnly;

			mRevertButton.Visible = showSaveRevertButtons;
			mSaveButton.Visible = showSaveRevertButtons;

			processSupportsAppending();
			processButtonVisibility();
		}

		private void processButtonVisibility()
		{
			bool anyButtonShown = mAppendButton.Visible || mTruncateButton.Visible || mRevertButton.Visible || mSaveButton.Visible;

			mWordEditorList.Height = anyButtonShown ? mEditorListWithButtonsHeight : (Height - mWordEditorList.Top);
		}

		public long DeviceRecordCount
		{
			get
			{
				return mDeviceRecordCount;
			}
		}

		private void setDeviceRecordCount(long recordCount)
		{
			if (recordCount < 1)
			{
				recordCount = 1;
			}

			mDeviceRecordCount = recordCount;

			mRecordUpDown.Maximum = mDeviceRecordCount - 1;
			mRecordTrackBar.Maximum = (int)(mDeviceRecordCount - 1);
			mRecordTrackBar.LargeChange = (int)(mDeviceRecordCount / 20);

			setTruncateEnabledState();
		}

		private void setTruncateEnabledState()
		{
			mTruncateButton.Enabled = mLastReadRecord == mDeviceRecordCount - 1 && mDeviceRecordCount > 1;
		}

		private IWordEditor createWordEditor(int index)
		{
			DeviceWordEditor editor = new DeviceWordEditor(index == -1 ? new FullWord(0) : mRecordEditWords[index]);
			editor.WordIndex = index;
			editor.ValueChanged += new MixGui.Events.WordEditorValueChangedEventHandler(editor_ValueChanged);

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

				loadRecord();
			}
			else
			{
				if (changesPending)
				{
					writeRecord();
				}

				if (mDevice != null)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}
			}

		}

		private void editor_ValueChanged(IWordEditor sender, MixGui.Events.WordEditorValueChangedEventArgs args)
		{
			changesPending = true;
		}

		private void loadWordEditor(IWordEditor editor, int index)
		{
			DeviceWordEditor deviceEditor = (DeviceWordEditor)editor;
			deviceEditor.DeviceWord = index >= 0 ? mRecordEditWords[index] : new FullWord(0L);
			deviceEditor.WordIndex = index;
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

			bool success = applyRecordCount() && loadRecord(loadErrorHandlingMode.ReportAlways);

			if (!success)
			{
				mDevice = oldDevice;
				initDeviceFileWatcher();
			}
			else
			{
				setTruncateEnabledState();
			}

			return success;
		}

		public void UpdateSettings()
		{
			initDeviceFileWatcher();

			if (Visible)
			{
				loadRecord();
			}

			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		private bool applyRecordCount()
		{
			if (!SupportsAppending)
			{
				return true;
			}

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

			try
			{
				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}

				long recordCount = 0;

				if (File.Exists(mDevice.FilePath))
				{
					stream = mOpenStream(mDevice.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					recordCount = mCalculateRecordCount(stream);

					stream.Close();
				}

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				stream = null;

				setDeviceRecordCount(recordCount);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to determine file length: " + ex.Message, "Error determining file length", MessageBoxButtons.OK, MessageBoxIcon.Hand);

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

		private bool loadRecord()
		{
			return loadRecord(loadErrorHandlingMode.SilentIfSameRecord);
		}

		private bool loadRecord(loadErrorHandlingMode errorHandlingMode)
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
				long record = (long)mRecordUpDown.Value;

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = false;
				}

				stream = mOpenStream(mDevice.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = mCalculateBytePosition(record);

				IFullWord[] readWords = mReadWords(stream, mDevice.RecordWordCount);

				stream.Close();

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				stream = null;

				for (int i = 0; i < readWords.Length; i++)
				{
					mRecordReadWords[i] = readWords[i];
					mRecordEditWords[i].LongValue = readWords[i].LongValue;
				}

				changesPending = false;

				mLastReadRecord = record;
				mLastLoadTime = DateTime.Now;

				mLoadRecordRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == loadErrorHandlingMode.RetryOnError || (mLoadRecordRetryCount > 0 && mLoadRecordRetryCount <= maxLoadSectorRetryCount))
				{
					mDelayedIOOperation = loadRecord;
					mLoadRecordRetryCount++;
					mIODelayTimer.Start();

					return false;
				}
				else if (mLoadRecordRetryCount > maxLoadSectorRetryCount)
				{
					mDelayedIOOperation = null;
					mLoadRecordRetryCount = 0;

					return false;
				}

				if (errorHandlingMode == loadErrorHandlingMode.ReportAlways || mRecordUpDown.Value != mLastReadRecord)
				{
					MessageBox.Show(this, "Unable to load device data: " + ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}

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

		public bool writeRecord()
		{
			return writeRecord(false);
		}

		public bool writeRecord(bool force)
		{
			if (!mChangesPending)
			{
				return true;
			}

			if (mReadOnlyCheckBox.Checked || (!force && mLastReadRecord == mRecordUpDown.Value))
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

				stream = mOpenStream(mDevice.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = mCalculateBytePosition(mLastReadRecord);

				mWriteWords(stream, mDevice.RecordWordCount, mRecordEditWords);

				stream.Close();

				if (watcherRaisesEvents)
				{
					mDeviceFileWatcher.EnableRaisingEvents = true;
				}

				stream = null;

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
			mWordEditorList.Update();
		}

		public void UpdateLayout()
		{
			mFirstWordTextBox.UpdateLayout();
			mMixCharButtons.UpdateLayout();
			mWordEditorList.UpdateLayout();
		}

		private void mReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mWordEditorList.ReadOnly = mReadOnlyCheckBox.Checked;

			if (mReadOnlyCheckBox.Checked && mRevertButton.Enabled)
			{
				revert();
			}

			processReadOnlySettings();
		}

		private void mRecordTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (mLastReadRecord == mRecordTrackBar.Value)
			{
				return;
			}

			if (processRecordSelectionChange())
			{
				mRecordUpDown.Value = mRecordTrackBar.Value;
				setTruncateEnabledState();
			}
			else
			{
				mRecordTrackBar.Value = (int)mLastReadRecord;
			}
		}

		public bool SaveCurrentSector()
		{
			return writeRecord(true);
		}

		private void mRecordUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (mLastReadRecord == mRecordUpDown.Value)
			{
				return;
			}

			if (processRecordSelectionChange())
			{
				mRecordTrackBar.Value = (int)mRecordUpDown.Value;
				setTruncateEnabledState();
			}
			else
			{
				mRecordUpDown.Value = mLastReadRecord;
			}
		}

		private bool processRecordSelectionChange()
		{
			return writeRecord() && loadRecord();
		}

		private void mFirstWordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			mWordEditorList.FirstVisibleIndex = (int)args.NewValue;
		}

		private void mWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			mFirstWordTextBox.LongValue = mWordEditorList.FirstVisibleIndex;
		}

		private void mSaveButton_Click(object sender, EventArgs e)
		{
			writeRecord(true);
		}

		private void mRevertButton_Click(object sender, EventArgs e)
		{
			revert();
		}

		private void revert()
		{
			for (int i = 0; i < mRecordReadWords.Length; i++)
			{
				mRecordEditWords[i].LongValue = mRecordReadWords[i].LongValue;
			}

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
				return mWordEditorList.ResizeInProgress;
			}
			set
			{
				mWordEditorList.ResizeInProgress = value;
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
				loadRecord(loadErrorHandlingMode.RetryOnError);
			}
			else
			{
				mDelayedIOOperation = loadRecord;
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
			ReportAlways,
			SilentIfSameRecord,
			RetryOnError
		}

		private void mAppendButton_Click(object sender, EventArgs e)
		{
			setDeviceRecordCount(mDeviceRecordCount + 1);

			mRecordUpDown.Value = mDeviceRecordCount - 1;

			if (mRecordUpDown.Value != mDeviceRecordCount - 1)
			{
				setDeviceRecordCount(mDeviceRecordCount - 1);
			}
		}

		private void mTruncateButton_Click(object sender, EventArgs e)
		{
			mRecordUpDown.Value = mDeviceRecordCount - 2;

			if (mRecordUpDown.Value == mDeviceRecordCount - 2)
			{
				bool success = false;

				FileStream stream = null;
				bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

				try
				{
					long recordCount = mDeviceRecordCount - 1;

					if (watcherRaisesEvents)
					{
						mDeviceFileWatcher.EnableRaisingEvents = false;
					}

					stream = mOpenStream(mDevice.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					stream.SetLength(mCalculateBytePosition(recordCount));
					stream.Close();

					if (watcherRaisesEvents)
					{
						mDeviceFileWatcher.EnableRaisingEvents = true;
					}

					stream = null;

					success = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to truncate device file: " + ex.Message, "Error truncating file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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

				if (success)
				{
					setDeviceRecordCount(mDeviceRecordCount - 1);
				}
			}

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

				mRecordUpDown.Value = 0;

				applyRecordCount();
				loadRecord();
			}
		}
	}
}
