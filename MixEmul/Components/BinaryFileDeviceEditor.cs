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

        delegate bool delayedIOOperation();

        const int maxLoadSectorRetryCount = 5;

        FileBasedDevice mDevice;
        bool mChangesPending;
        IFullWord[] mRecordReadWords;
        IFullWord[] mRecordEditWords;
        long mLastReadRecord;
        delayedIOOperation mDelayedIOOperation;
        int mLoadRecordRetryCount;
        long mDeviceRecordCount;
        long mDeviceWordsPerRecord;
        OpenStreamCallback mOpenStream;
        CalculateBytePositionCallback mCalculateBytePosition;
        ReadWordsCallback mReadWords;
        WriteWordsCallback mWriteWords;
        CalculateRecordCountCallback mCalculateRecordCount;
        int mReadOnlyGap;
        bool mInitialized;
        bool mShowReadOnly;
        int mEditorListWithButtonsHeight;
        string mRecordLabelFormatString;
        string mRecordName;
        DateTime mLastLoadTime;
        bool mLoading;


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

        public long DeviceRecordCount => mDeviceRecordCount;

        public bool SupportsAppending => mCalculateRecordCount != null && !mReadOnlyCheckBox.Checked;

        bool loadRecord() => loadRecord(loadErrorHandlingMode.SilentIfSameRecord);

        public bool writeRecord() => writeRecord(false);

        public new void Update() => mWordEditorList.Update();

        public bool SaveCurrentSector() => writeRecord(true);

        bool processRecordSelectionChange() => writeRecord() && loadRecord();

        void mSaveButton_Click(object sender, EventArgs e) => writeRecord(true);

        void mRevertButton_Click(object sender, EventArgs e) => revert();

        void mDeviceFileWatcher_Event(object sender, FileSystemEventArgs e) => handleFileWatcherEvent();

        void DeviceEditor_VisibleChanged(object sender, EventArgs e) => processVisibility();

        void mDeviceFileWatcher_Renamed(object sender, RenamedEventArgs e) => handleFileWatcherEvent();

        public void Initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
		{
			initialize(recordCount, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, null);
		}

		public void Initialize(CalculateRecordCountCallback calculateRecordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords)
		{
			initialize(1, wordsPerRecord, openStream, calculateBytePosition, readWords, writeWords, calculateRecordCount);
		}

        void initialize(long recordCount, long wordsPerRecord, OpenStreamCallback openStream, CalculateBytePositionCallback calculateBytePosition, ReadWordsCallback readWords, WriteWordsCallback writeWords, CalculateRecordCountCallback calculateRecordCount)
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

        void processSupportsAppending()
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
				if (mShowReadOnly == value) return;

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

        void processReadOnlySettings()
        {
            bool showSaveRevertButtons = !mWordEditorList.ReadOnly || mShowReadOnly;

            mRevertButton.Visible = showSaveRevertButtons;
            mSaveButton.Visible = showSaveRevertButtons;

            processSupportsAppending();
            processButtonVisibility();
        }

        void processButtonVisibility()
        {
            bool anyButtonShown = mAppendButton.Visible || mTruncateButton.Visible || mRevertButton.Visible || mSaveButton.Visible;

            mWordEditorList.Height = anyButtonShown ? mEditorListWithButtonsHeight : (Height - mWordEditorList.Top);
        }

        void setDeviceRecordCount(long recordCount)
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

        void setTruncateEnabledState()
        {
            mTruncateButton.Enabled = mLastReadRecord == mDeviceRecordCount - 1 && mDeviceRecordCount > 1;
        }

        IWordEditor createWordEditor(int index)
        {
            var editor = new DeviceWordEditor(index == -1 ? new FullWord(0) : mRecordEditWords[index]);
            editor.WordIndex = index;
            editor.ValueChanged += editor_ValueChanged;

            return editor;
        }

        void processVisibility()
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
                if (changesPending) writeRecord();

                if (mDevice != null)
                {
                    mDeviceFileWatcher.EnableRaisingEvents = false;
                }
            }

        }

        void editor_ValueChanged(IWordEditor sender, MixGui.Events.WordEditorValueChangedEventArgs args)
        {
            changesPending = true;
        }

        void loadWordEditor(IWordEditor editor, int index)
        {
            var deviceEditor = (DeviceWordEditor)editor;
            deviceEditor.DeviceWord = index >= 0 ? mRecordEditWords[index] : new FullWord(0L);
            deviceEditor.WordIndex = index;
        }

        public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || mDevice == device) return true;

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

			if (Visible) loadRecord();

			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

        bool applyRecordCount()
        {
            if (!SupportsAppending) return true;

            FileStream stream = null;
            bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

            try
            {
                mDeviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

                long recordCount = 0;

                if (File.Exists(mDevice.FilePath))
                {
                    stream = mOpenStream(mDevice.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    recordCount = mCalculateRecordCount(stream);

                    stream.Close();
                }

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
            }

            return true;
        }

        void initDeviceFileWatcher()
        {
            if (mDevice != null)
            {
                mDeviceFileWatcher.Path = Path.GetDirectoryName(mDevice.FilePath);
                mDeviceFileWatcher.Filter = Path.GetFileName(mDevice.FilePath);

                mDeviceFileWatcher.EnableRaisingEvents |= Visible;
            }
        }

        bool changesPending
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

        bool loadRecord(loadErrorHandlingMode errorHandlingMode)
        {
            if (!Visible) return true;
            if (mDevice == null) return false;

            mLoading = true;

            FileStream stream = null;
            bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

            try
            {
                var record = (long)mRecordUpDown.Value;

                if (watcherRaisesEvents)
                {
                    mDeviceFileWatcher.EnableRaisingEvents = false;
                }

                stream = mOpenStream(mDevice.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                stream.Position = mCalculateBytePosition(record);

                IFullWord[] readWords = mReadWords(stream, mDevice.RecordWordCount);

                stream.Close();

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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
                if (mLoadRecordRetryCount > maxLoadSectorRetryCount)
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
                stream?.Close();

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

                mLoading = false;
            }

            return true;
        }

        public bool writeRecord(bool force)
		{
			if (!mChangesPending) return true;
			if (mReadOnlyCheckBox.Checked || (!force && mLastReadRecord == mRecordUpDown.Value)) return false;

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

            try
            {
                mDeviceFileWatcher.EnableRaisingEvents &= !watcherRaisesEvents;

                stream = mOpenStream(mDevice.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                stream.Position = mCalculateBytePosition(mLastReadRecord);

                mWriteWords(stream, mDevice.RecordWordCount, mRecordEditWords);

                stream.Close();

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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
                stream?.Close();

                mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
            }

			return true;
		}

		public void UpdateLayout()
		{
			mFirstWordTextBox.UpdateLayout();
			mMixCharButtons.UpdateLayout();
			mWordEditorList.UpdateLayout();
		}

        void mReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mWordEditorList.ReadOnly = mReadOnlyCheckBox.Checked;

            if (mReadOnlyCheckBox.Checked && mRevertButton.Enabled) revert();

            processReadOnlySettings();
        }

        void mRecordTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (mLastReadRecord == mRecordTrackBar.Value) return;

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

        void mRecordUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (mLastReadRecord == mRecordUpDown.Value) return;

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

        void mFirstWordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
        {
            mWordEditorList.FirstVisibleIndex = (int)args.NewValue;
        }

        void mWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
        {
            mFirstWordTextBox.LongValue = mWordEditorList.FirstVisibleIndex;
        }

        void revert()
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

        void handleFileWatcherEvent()
        {
            if (mLoading || mIODelayTimer.Enabled)  return;

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

        void mIODelayTimer_Tick(object sender, EventArgs e)
        {
            mIODelayTimer.Stop();

            mDelayedIOOperation();
        }

        enum loadErrorHandlingMode
        {
            ReportAlways,
            SilentIfSameRecord,
            RetryOnError
        }

        void mAppendButton_Click(object sender, EventArgs e)
        {
            setDeviceRecordCount(mDeviceRecordCount + 1);

            mRecordUpDown.Value = mDeviceRecordCount - 1;

            if (mRecordUpDown.Value != mDeviceRecordCount - 1) setDeviceRecordCount(mDeviceRecordCount - 1);
        }

        void mTruncateButton_Click(object sender, EventArgs e)
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

                    if (watcherRaisesEvents) mDeviceFileWatcher.EnableRaisingEvents = false;

                    stream = mOpenStream(mDevice.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    stream.SetLength(mCalculateBytePosition(recordCount));
                    stream.Close();

                    mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

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

                    mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
                }

                if (success) setDeviceRecordCount(mDeviceRecordCount - 1);
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
