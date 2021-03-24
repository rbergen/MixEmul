using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Type;
using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;

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
		private readonly int mReadOnlyGap;
		private readonly bool mInitialized;
		private bool mShowReadOnly;
		private int mEditorListWithButtonsHeight;
		private readonly string mRecordLabelFormatString;
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
			SetDeviceRecordCount(1);
			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;

			mInitialized = false;
		}

		public long DeviceRecordCount 
			=> mDeviceRecordCount;

		public bool SupportsAppending 
			=> mCalculateRecordCount != null && !mReadOnlyCheckBox.Checked;

		private bool LoadRecord() 
			=> LoadRecord(LoadErrorHandlingMode.SilentIfSameRecord);

		public bool WriteRecord() 
			=> WriteRecord(false);

		public new void Update() 
			=> mWordEditorList.Update();

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
			if (mInitialized)
				throw new InvalidOperationException("FileDeviceEditor has already been initialized");

			mDeviceWordsPerRecord = wordsPerRecord;
			mOpenStream = openStream;
			mCalculateBytePosition = calculateBytePosition;
			mReadWords = readWords;
			mWriteWords = writeWords;
			mCalculateRecordCount = calculateRecordCount;

			mRecordUpDown.Minimum = 0;
			mRecordTrackBar.Minimum = 0;
			SetDeviceRecordCount(recordCount);

			mFirstWordTextBox.ClearZero = false;
			mFirstWordTextBox.MinValue = 0L;
			mFirstWordTextBox.MaxValue = mDeviceWordsPerRecord - 1;

			mRecordReadWords = new IFullWord[mDeviceWordsPerRecord];
			mRecordEditWords = new IFullWord[mDeviceWordsPerRecord];

			for (int i = 0; i < mDeviceWordsPerRecord; i++)
				mRecordEditWords[i] = new FullWord();

			mWordEditorList.MaxIndex = (int)mDeviceWordsPerRecord - 1;
			mWordEditorList.CreateEditor = CreateWordEditor;
			mWordEditorList.LoadEditor = LoadWordEditor;

			mChangesPending = false;

			mLoadRecordRetryCount = 0;

			ProcessSupportsAppending();
			ProcessVisibility();
		}

		public string RecordName
		{
			get => mRecordName;
			set
			{
				mRecordName = value;

				mRecordLabel.Text = string.Format(mRecordLabelFormatString, mRecordName);
			}
		}

		private void ProcessSupportsAppending()
		{
			bool supportsAppending = SupportsAppending;

			mAppendButton.Visible = supportsAppending;
			mTruncateButton.Visible = supportsAppending;
		}

		public bool ShowReadOnly
		{
			get => mShowReadOnly;
			set
			{
				if (mShowReadOnly == value)
					return;

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

				ProcessReadOnlySettings();
			}
		}

		private void ProcessReadOnlySettings()
		{
			bool showSaveRevertButtons = !mWordEditorList.ReadOnly || mShowReadOnly;

			mRevertButton.Visible = showSaveRevertButtons;
			mSaveButton.Visible = showSaveRevertButtons;

			ProcessSupportsAppending();
			ProcessButtonVisibility();
		}

		private void ProcessButtonVisibility() 
			=> mWordEditorList.Height = mAppendButton.Visible || mTruncateButton.Visible || mRevertButton.Visible || mSaveButton.Visible 
			? mEditorListWithButtonsHeight 
			: (Height - mWordEditorList.Top);

		private void SetDeviceRecordCount(long recordCount)
		{
			if (recordCount < 1)
				recordCount = 1;

			mDeviceRecordCount = recordCount;

			mRecordUpDown.Maximum = mDeviceRecordCount - 1;
			mRecordTrackBar.Maximum = (int)(mDeviceRecordCount - 1);
			mRecordTrackBar.LargeChange = (int)(mDeviceRecordCount / 20);

			SetTruncateEnabledState();
		}

		private void SetTruncateEnabledState() 
			=> mTruncateButton.Enabled = mLastReadRecord == mDeviceRecordCount - 1 && mDeviceRecordCount > 1;

		private IWordEditor CreateWordEditor(int index)
		{
			var editor = new DeviceWordEditor(index == -1 ? new FullWord(0) : mRecordEditWords[index])
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
				mDeviceFileWatcher.EnableRaisingEvents |= mDevice != null;

				LoadRecord();
			}
			else
			{
				if (ChangesPending)
					WriteRecord();

				mDeviceFileWatcher.EnableRaisingEvents &= mDevice == null;
			}

		}

		private void Editor_ValueChanged(IWordEditor sender, Events.WordEditorValueChangedEventArgs args) 
			=> ChangesPending = true;

		private void LoadWordEditor(IWordEditor editor, int index)
		{
			var deviceEditor = (DeviceWordEditor)editor;
			deviceEditor.DeviceWord = index >= 0 ? mRecordEditWords[index] : new FullWord(0L);
			deviceEditor.WordIndex = index;
		}

		public bool SetDevice(FileBasedDevice device)
		{
			if (device == null || mDevice == device)
				return true;

			mDeviceFileWatcher.EnableRaisingEvents = false;

			FileBasedDevice oldDevice = mDevice;
			mDevice = device;
			InitDeviceFileWatcher();

			bool success = ApplyRecordCount() && LoadRecord(LoadErrorHandlingMode.ReportAlways);

			if (!success)
			{
				mDevice = oldDevice;
				InitDeviceFileWatcher();
			}
			else
				SetTruncateEnabledState();

			return success;
		}

		public void UpdateSettings()
		{
			InitDeviceFileWatcher();

			if (Visible)
				LoadRecord();

			mIODelayTimer.Interval = DeviceSettings.DeviceReloadInterval;
		}

		private bool ApplyRecordCount()
		{
			if (!SupportsAppending)
				return true;

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

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;
			}

			return true;
		}

		private void InitDeviceFileWatcher()
		{
			if (mDevice != null)
			{
				mDeviceFileWatcher.Path = Path.GetDirectoryName(mDevice.FilePath);
				mDeviceFileWatcher.Filter = Path.GetFileName(mDevice.FilePath);

				mDeviceFileWatcher.EnableRaisingEvents |= Visible;
			}
		}

		private bool ChangesPending
		{
			get => mChangesPending;
			set
			{
				mChangesPending = value;

				mRevertButton.Enabled = mChangesPending;
				mSaveButton.Enabled = mChangesPending;
			}
		}

		private bool LoadRecord(LoadErrorHandlingMode errorHandlingMode)
		{
			if (!Visible)
				return true;

			if (mDevice == null)
				return false;

			mLoading = true;

			FileStream stream = null;
			bool watcherRaisesEvents = mDeviceFileWatcher.EnableRaisingEvents;

			try
			{
				var record = (long)mRecordUpDown.Value;

				if (watcherRaisesEvents)
					mDeviceFileWatcher.EnableRaisingEvents = false;

				stream = mOpenStream(mDevice.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream.Position = mCalculateBytePosition(record);

				var readWords = mReadWords(stream, mDevice.RecordWordCount);

				stream.Close();

				mDeviceFileWatcher.EnableRaisingEvents |= watcherRaisesEvents;

				stream = null;

				for (int i = 0; i < readWords.Length; i++)
				{
					mRecordReadWords[i] = readWords[i];
					mRecordEditWords[i].LongValue = readWords[i].LongValue;
				}

				ChangesPending = false;

				mLastReadRecord = record;
				mLastLoadTime = DateTime.Now;

				mLoadRecordRetryCount = 0;

				Update();
			}
			catch (Exception ex)
			{
				if (errorHandlingMode == LoadErrorHandlingMode.RetryOnError || (mLoadRecordRetryCount > 0 && mLoadRecordRetryCount <= maxLoadSectorRetryCount))
				{
					mDelayedIOOperation = LoadRecord;
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

				if (errorHandlingMode == LoadErrorHandlingMode.ReportAlways || mRecordUpDown.Value != mLastReadRecord)
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

		public bool WriteRecord(bool force)
		{
			if (!mChangesPending)
				return true;

			if (mReadOnlyCheckBox.Checked || (!force && mLastReadRecord == mRecordUpDown.Value))
				return false;

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

		public void UpdateLayout()
		{
			mFirstWordTextBox.UpdateLayout();
			mMixCharButtons.UpdateLayout();
			mWordEditorList.UpdateLayout();
		}

		private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mWordEditorList.ReadOnly = mReadOnlyCheckBox.Checked;

			if (mReadOnlyCheckBox.Checked && mRevertButton.Enabled)
				Revert();

			ProcessReadOnlySettings();
		}

		private void RecordTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (mLastReadRecord == mRecordTrackBar.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				mRecordUpDown.Value = mRecordTrackBar.Value;
				SetTruncateEnabledState();
			}
			else
				mRecordTrackBar.Value = (int)mLastReadRecord;
		}

		private void RecordUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (mLastReadRecord == mRecordUpDown.Value)
				return;

			if (ProcessRecordSelectionChange())
			{
				mRecordTrackBar.Value = (int)mRecordUpDown.Value;
				SetTruncateEnabledState();
			}
			else
				mRecordUpDown.Value = mLastReadRecord;
		}

		private void FirstWordTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args) 
			=> mWordEditorList.FirstVisibleIndex = (int)args.NewValue;

		private void MWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args) 
			=> mFirstWordTextBox.LongValue = mWordEditorList.FirstVisibleIndex;

		private void Revert()
		{
			for (int i = 0; i < mRecordReadWords.Length; i++)
				mRecordEditWords[i].LongValue = mRecordReadWords[i].LongValue;

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
			get => mWordEditorList.ResizeInProgress;
			set => mWordEditorList.ResizeInProgress = value;
		}

		private void HandleFileWatcherEvent()
		{
			if (mLoading || mIODelayTimer.Enabled)
				return;

			if ((DateTime.Now - mLastLoadTime).TotalMilliseconds > mIODelayTimer.Interval)
				LoadRecord(LoadErrorHandlingMode.RetryOnError);

			else
			{
				mDelayedIOOperation = LoadRecord;
				mIODelayTimer.Start();
			}
		}

		private void IODelayTimer_Tick(object sender, ElapsedEventArgs e)
		{
			mIODelayTimer.Stop();

			if (InvokeRequired)
				Invoke(mDelayedIOOperation);
			
			else
				mDelayedIOOperation();
		}

		private enum LoadErrorHandlingMode
		{
			ReportAlways,
			SilentIfSameRecord,
			RetryOnError
		}

		private void AppendButton_Click(object sender, EventArgs e)
		{
			SetDeviceRecordCount(mDeviceRecordCount + 1);

			mRecordUpDown.Value = mDeviceRecordCount - 1;

			if (mRecordUpDown.Value != mDeviceRecordCount - 1)
				SetDeviceRecordCount(mDeviceRecordCount - 1);
		}

		private void TruncateButton_Click(object sender, EventArgs e)
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
						mDeviceFileWatcher.EnableRaisingEvents = false;

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

				if (success)
					SetDeviceRecordCount(mDeviceRecordCount - 1);
			}

		}

		public void DeleteDeviceFile()
		{
			if (mDevice == null || !File.Exists(mDevice.FilePath))
				return;

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

			ApplyRecordCount();
			LoadRecord();
		}
	}
}
