using System;
using System.Windows.Forms;
using MixLib;
using MixLib.Device;
using MixLib.Device.Step;

namespace MixGui.Components
{
	public partial class DeviceEditorForm : Form
	{
        const int tapesTabIndex = 0;
        const int disksTabIndex = 1;
        const int cardReaderTabIndex = 2;
        const int cardWriterTabIndex = 3;
        const int printerTabIndex = 4;
        const int paperTapeTabIndex = 5;

        Devices mDevices;
        int mLastSelectedDisk;
        int mLastSelectedTape;
        int mLastSelectedDeviceTab;

        public DeviceEditorForm()
			: this(null)
		{
		}

		public DeviceEditorForm(Devices devices)
		{
			InitializeComponent();

			mDiskEditor.Initialize(DiskDevice.SectorCount, DiskDevice.WordsPerSector, DiskDevice.OpenStream, DiskDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			mTapeEditor.Initialize(TapeDevice.CalculateRecordCount, TapeDevice.WordsPerRecord, FileBasedDevice.OpenStream, TapeDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			mCardReaderEditor.Initialize(CardReaderDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			mCardWriterEditor.Initialize(CardWriterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			mPrinterEditor.Initialize(PrinterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			mPaperTapeEditor.Initialize(PaperTapeDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);

			Devices = devices;

			mLastSelectedDeviceTab = 0;
		}

        void mCloseButton_Click(object sender, EventArgs e) => Hide();

        void mTapeDeleteButton_Click(object sender, EventArgs e) =>
            deleteBinaryDeviceFile(mTapeSelectorComboBox.SelectedItem.ToString(), mTapeEditor);

        void mDiskDeleteButton_Click(object sender, EventArgs e) =>
            deleteBinaryDeviceFile(mDiskSelectorComboBox.SelectedItem.ToString(), mDiskEditor);

        void mCardReaderDeleteButton_Click(object sender, EventArgs e) => deleteTextDeviceFile("card reader", mCardReaderEditor);

        void mCardPunchDeleteButton_Click(object sender, EventArgs e) => deleteTextDeviceFile("card punch", mCardWriterEditor);

        void mPrinterDeleteButton_Click(object sender, EventArgs e) => deleteTextDeviceFile("printer", mPrinterEditor);

        void mPaperTapeDeleteButton_Click(object sender, EventArgs e) => deleteTextDeviceFile("paper tape", mPaperTapeEditor);

        void mDiskSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mLastSelectedDisk == mDiskSelectorComboBox.SelectedIndex)
            {
                return;
            }

            if (updateDiskControls())
            {
                mLastSelectedDisk = mDiskSelectorComboBox.SelectedIndex;
            }
            else
            {
                mDiskSelectorComboBox.SelectedIndex = mLastSelectedDisk;
            }
        }

        void mTapeSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mLastSelectedTape == mTapeSelectorComboBox.SelectedIndex)
            {
                return;
            }

            if (updateTapeControls())
            {
                mLastSelectedTape = mTapeSelectorComboBox.SelectedIndex;
            }
            else
            {
                mTapeSelectorComboBox.SelectedIndex = mLastSelectedTape;
            }

        }

        public new void Hide()
		{
			if (saveCurrentTabRecord())
			{
				base.Hide();
			}
		}

		public void ShowDevice(MixDevice device)
		{
			if (device is DiskDevice)
			{
				mDeviceTypeTabs.SelectedIndex = disksTabIndex;
				mDiskSelectorComboBox.SelectedItem = device;
			}
			else if (device is TapeDevice)
			{
				mDeviceTypeTabs.SelectedIndex = tapesTabIndex;
				mTapeSelectorComboBox.SelectedItem = device;
			}
			else if (device is CardReaderDevice)
			{
				mDeviceTypeTabs.SelectedIndex = cardReaderTabIndex;
			}
			else if (device is CardWriterDevice)
			{
				mDeviceTypeTabs.SelectedIndex = cardWriterTabIndex;
			}
			else if (device is PrinterDevice)
			{
				mDeviceTypeTabs.SelectedIndex = printerTabIndex;
			}
			else if (device is PaperTapeDevice)
			{
				mDeviceTypeTabs.SelectedIndex = paperTapeTabIndex;
			}
		}

		public Devices Devices
		{
			get
			{
				return mDevices;
			}
			set
			{
				if (value == null)
				{
					return;
				}

				if (mDevices != null)
				{
					throw new InvalidOperationException("value may only be set once");
				}

				mDevices = value;

				foreach (MixDevice device in mDevices)
				{
					if (device is DiskDevice)
					{
						mDiskSelectorComboBox.Items.Add(device);
					}
					else if (device is TapeDevice)
					{
						mTapeSelectorComboBox.Items.Add(device);
					}
					else if (device is CardReaderDevice)
					{
						var fileDevice = (FileBasedDevice)device;
						mCardReaderEditor.SetDevice(fileDevice);
						mCardReaderPathBox.Text = fileDevice.FilePath;
					}
					else if (device is CardWriterDevice)
					{
						var fileDevice = (FileBasedDevice)device;
						mCardWriterEditor.SetDevice(fileDevice);
						mCardWriterPathBox.Text = fileDevice.FilePath;
					}
					else if (device is PrinterDevice)
					{
						var fileDevice = (FileBasedDevice)device;
						mPrinterEditor.SetDevice(fileDevice);
						mPrinterPathBox.Text = fileDevice.FilePath;
					}
					else if (device is PaperTapeDevice)
					{
						var fileDevice = (FileBasedDevice)device;
						mPaperTapeEditor.SetDevice(fileDevice);
						mPaperTapePathBox.Text = fileDevice.FilePath;
					}
				}

				mLastSelectedDisk = 0;

				if (mDiskSelectorComboBox.Items.Count > 0)
				{
					mDiskSelectorComboBox.SelectedIndex = 0;
				}

				updateDiskControls();

				mLastSelectedTape = 0;

				if (mTapeSelectorComboBox.Items.Count > 0)
				{
					mTapeSelectorComboBox.SelectedIndex = 0;
				}

				updateTapeControls();
			}
		}

		public void UpdateSettings()
		{
			mTapeEditor.UpdateSettings();
			mDiskEditor.UpdateSettings();
			mCardReaderEditor.UpdateSettings();
			mCardWriterEditor.UpdateSettings();
			mPrinterEditor.UpdateSettings();
			mPaperTapeEditor.UpdateSettings();

			updateTapeControls();
			updateDiskControls();
			mCardReaderPathBox.Text = mCardReaderEditor.Device.FilePath;
			mCardWriterPathBox.Text = mCardWriterEditor.Device.FilePath;
			mPrinterPathBox.Text = mPrinterEditor.Device.FilePath;
			mPaperTapePathBox.Text = mPaperTapeEditor.Device.FilePath;
		}

        bool updateDiskControls()
        {
            bool success = true;

            if (mLastSelectedDisk != mDiskSelectorComboBox.SelectedIndex)
            {
                success = mDiskEditor.SaveCurrentSector();
            }

            if (success)
            {
                var selectedDisk = (DiskDevice)mDiskSelectorComboBox.SelectedItem;
                success = mDiskEditor.SetDevice(selectedDisk);

                if (success)
                {
                    mDiskPathBox.Text = selectedDisk.FilePath;
                }
            }

            return success;
        }

        public void UpdateLayout()
		{
			mTapeEditor.UpdateLayout();
			mDiskEditor.UpdateLayout();
			mCardReaderEditor.UpdateLayout();
			mCardWriterEditor.UpdateLayout();
			mPrinterEditor.UpdateLayout();
			mPaperTapeEditor.UpdateLayout();
		}

        bool updateTapeControls()
        {
            bool success = true;

            if (mLastSelectedTape != mTapeSelectorComboBox.SelectedIndex)
            {
                success = mTapeEditor.SaveCurrentSector();
            }

            if (success)
            {
                var selectedTape = (TapeDevice)mTapeSelectorComboBox.SelectedItem;
                success = mTapeEditor.SetDevice(selectedTape);

                if (success)
                {
                    mTapePathBox.Text = selectedTape.FilePath;
                }
            }

            return success;
        }

        void DeviceEditorForm_ResizeBegin(object sender, EventArgs e)
        {
            mDiskEditor.ResizeInProgress = true;
            mTapeEditor.ResizeInProgress = true;
            mCardReaderEditor.ResizeInProgress = true;
            mCardWriterEditor.ResizeInProgress = true;
            mPrinterEditor.ResizeInProgress = true;
            mPaperTapeEditor.ResizeInProgress = true;
        }

        void DeviceEditorForm_ResizeEnd(object sender, EventArgs e)
        {
            mDiskEditor.ResizeInProgress = false;
            mTapeEditor.ResizeInProgress = false;
            mCardReaderEditor.ResizeInProgress = false;
            mCardWriterEditor.ResizeInProgress = false;
            mPrinterEditor.ResizeInProgress = false;
            mPaperTapeEditor.ResizeInProgress = false;
        }

        void DeviceEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            Hide();
        }

        void mDeviceTypeTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mDeviceTypeTabs.SelectedIndex == mLastSelectedDeviceTab)
            {
                return;
            }

            if (saveCurrentTabRecord())
            {
                mLastSelectedDeviceTab = mDeviceTypeTabs.SelectedIndex;
            }
            else
            {
                mDeviceTypeTabs.SelectedIndex = mLastSelectedDeviceTab;
            }
        }

        bool saveCurrentTabRecord()
        {
            bool success = true;

            switch (mLastSelectedDeviceTab)
            {
                case tapesTabIndex:
                    success = mTapeEditor.SaveCurrentSector();
                    break;

                case disksTabIndex:
                    success = mDiskEditor.SaveCurrentSector();
                    break;

                case cardReaderTabIndex:
                    success = mCardReaderEditor.Save();
                    break;

                case cardWriterTabIndex:
                    success = mCardReaderEditor.Save();
                    break;

                case printerTabIndex:
                    success = mCardReaderEditor.Save();
                    break;

                case paperTapeTabIndex:
                    success = mCardReaderEditor.Save();
                    break;
            }

            return success;
        }

        void deleteBinaryDeviceFile(string deviceName, BinaryFileDeviceEditor editor)
        {
            if (MessageBox.Show(this, "Are you sure you want to delete the file for device " + deviceName + "?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                editor.DeleteDeviceFile();
            }
        }

        void deleteTextDeviceFile(string deviceName, TextFileDeviceEditor editor)
        {
            if (MessageBox.Show(this, "Are you sure you want to delete the device file for the " + deviceName + "?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                editor.DeleteDeviceFile();
            }
        }

        void mCardReaderLoadButton_Click(object sender, EventArgs e)
        {
            if (mCardReaderLoadFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                mCardReaderEditor.LoadDeviceFile(mCardReaderLoadFileDialog.FileName);
            }
        }
    }
}