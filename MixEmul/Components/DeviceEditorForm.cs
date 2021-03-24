using MixLib;
using MixLib.Device;
using MixLib.Device.Step;
using System;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class DeviceEditorForm : Form
	{
		private const int tapesTabIndex = 0;
		private const int disksTabIndex = 1;
		private const int cardReaderTabIndex = 2;
		private const int cardWriterTabIndex = 3;
		private const int printerTabIndex = 4;
		private const int paperTapeTabIndex = 5;

		private Devices mDevices;
		private int mLastSelectedDisk;
		private int mLastSelectedTape;
		private int mLastSelectedDeviceTab;

		public DeviceEditorForm(Devices devices = null)
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

		private void CloseButton_Click(object sender, EventArgs e) 
			=> Hide();

		private void TapeDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteBinaryDeviceFile(mTapeSelectorComboBox.SelectedItem.ToString(), mTapeEditor);

		private void DiskDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteBinaryDeviceFile(mDiskSelectorComboBox.SelectedItem.ToString(), mDiskEditor);

		private void CardReaderDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteTextDeviceFile("card reader", mCardReaderEditor);

		private void CardPunchDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteTextDeviceFile("card punch", mCardWriterEditor);

		private void PrinterDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteTextDeviceFile("printer", mPrinterEditor);

		private void PaperTapeDeleteButton_Click(object sender, EventArgs e) 
			=> DeleteTextDeviceFile("paper tape", mPaperTapeEditor);

		private void DiskSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mLastSelectedDisk == mDiskSelectorComboBox.SelectedIndex)
				return;

			if (UpdateDiskControls())
				mLastSelectedDisk = mDiskSelectorComboBox.SelectedIndex;

			else
				mDiskSelectorComboBox.SelectedIndex = mLastSelectedDisk;
		}

		private void TapeSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mLastSelectedTape == mTapeSelectorComboBox.SelectedIndex)
				return;

			if (UpdateTapeControls())
				mLastSelectedTape = mTapeSelectorComboBox.SelectedIndex;

			else
				mTapeSelectorComboBox.SelectedIndex = mLastSelectedTape;
		}

		public new void Hide()
		{
			if (SaveCurrentTabRecord())
				base.Hide();
		}

		public void ShowDevice(MixDevice device)
		{
			switch (device)
			{
				case DiskDevice:
					mDeviceTypeTabs.SelectedIndex = disksTabIndex;
					mDiskSelectorComboBox.SelectedItem = device;
					break;

				case TapeDevice:
					mDeviceTypeTabs.SelectedIndex = tapesTabIndex;
					mTapeSelectorComboBox.SelectedItem = device;
					break;

				case CardReaderDevice:
					mDeviceTypeTabs.SelectedIndex = cardReaderTabIndex;
					break;

				case CardWriterDevice:
					mDeviceTypeTabs.SelectedIndex = cardWriterTabIndex;
					break;

				case PrinterDevice:
					mDeviceTypeTabs.SelectedIndex = printerTabIndex;
					break;

				case PaperTapeDevice:
					mDeviceTypeTabs.SelectedIndex = paperTapeTabIndex;
					break;
			}
		}

		public Devices Devices
		{
			get => mDevices;
			set
			{
				if (value == null)
					return;

				if (mDevices != null)
					throw new InvalidOperationException("value may only be set once");

				mDevices = value;
				FileBasedDevice fileDevice;

				foreach (MixDevice device in mDevices)
				{
					switch (device)
					{
						case DiskDevice:
							mDiskSelectorComboBox.Items.Add(device);
							break;

						case TapeDevice:
							mTapeSelectorComboBox.Items.Add(device);
							break;

						case CardReaderDevice:
							fileDevice = (FileBasedDevice)device;
							mCardReaderEditor.SetDevice(fileDevice);
							mCardReaderPathBox.Text = fileDevice.FilePath;
							break;

						case CardWriterDevice:
							fileDevice = (FileBasedDevice)device;
							mCardWriterEditor.SetDevice(fileDevice);
							mCardWriterPathBox.Text = fileDevice.FilePath;
							break;

						case PrinterDevice:
							fileDevice = (FileBasedDevice)device;
							mPrinterEditor.SetDevice(fileDevice);
							mPrinterPathBox.Text = fileDevice.FilePath;
							break;

						case PaperTapeDevice:
							fileDevice = (FileBasedDevice)device;
							mPaperTapeEditor.SetDevice(fileDevice);
							mPaperTapePathBox.Text = fileDevice.FilePath;
							break;
					}
				}

				mLastSelectedDisk = 0;

				if (mDiskSelectorComboBox.Items.Count > 0)
					mDiskSelectorComboBox.SelectedIndex = 0;

				UpdateDiskControls();

				mLastSelectedTape = 0;

				if (mTapeSelectorComboBox.Items.Count > 0)
					mTapeSelectorComboBox.SelectedIndex = 0;

				UpdateTapeControls();
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

			UpdateTapeControls();
			UpdateDiskControls();
			mCardReaderPathBox.Text = mCardReaderEditor.Device.FilePath;
			mCardWriterPathBox.Text = mCardWriterEditor.Device.FilePath;
			mPrinterPathBox.Text = mPrinterEditor.Device.FilePath;
			mPaperTapePathBox.Text = mPaperTapeEditor.Device.FilePath;
		}

		private bool UpdateDiskControls()
		{
			bool success = true;

			if (mLastSelectedDisk != mDiskSelectorComboBox.SelectedIndex)
				success = mDiskEditor.SaveCurrentSector();

			if (success)
			{
				var selectedDisk = (DiskDevice)mDiskSelectorComboBox.SelectedItem;
				success = mDiskEditor.SetDevice(selectedDisk);

				if (success)
					mDiskPathBox.Text = selectedDisk.FilePath;
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

		private bool UpdateTapeControls()
		{
			bool success = true;

			if (mLastSelectedTape != mTapeSelectorComboBox.SelectedIndex)
				success = mTapeEditor.SaveCurrentSector();

			if (success)
			{
				var selectedTape = (TapeDevice)mTapeSelectorComboBox.SelectedItem;
				success = mTapeEditor.SetDevice(selectedTape);

				if (success)
					mTapePathBox.Text = selectedTape.FilePath;
			}

			return success;
		}

		private void DeviceEditorForm_ResizeBegin(object sender, EventArgs e)
		{
			mDiskEditor.ResizeInProgress = true;
			mTapeEditor.ResizeInProgress = true;
			mCardReaderEditor.ResizeInProgress = true;
			mCardWriterEditor.ResizeInProgress = true;
			mPrinterEditor.ResizeInProgress = true;
			mPaperTapeEditor.ResizeInProgress = true;
		}

		private void DeviceEditorForm_ResizeEnd(object sender, EventArgs e)
		{
			mDiskEditor.ResizeInProgress = false;
			mTapeEditor.ResizeInProgress = false;
			mCardReaderEditor.ResizeInProgress = false;
			mCardWriterEditor.ResizeInProgress = false;
			mPrinterEditor.ResizeInProgress = false;
			mPaperTapeEditor.ResizeInProgress = false;
		}

		private void DeviceEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;

			Hide();
		}

		private void DeviceTypeTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mDeviceTypeTabs.SelectedIndex == mLastSelectedDeviceTab)
				return;

			if (SaveCurrentTabRecord())
				mLastSelectedDeviceTab = mDeviceTypeTabs.SelectedIndex;

			else
				mDeviceTypeTabs.SelectedIndex = mLastSelectedDeviceTab;
		}

		private bool SaveCurrentTabRecord()
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

		private void DeleteBinaryDeviceFile(string deviceName, BinaryFileDeviceEditor editor)
		{
			if (MessageBox.Show(this, "Are you sure you want to delete the file for device " + deviceName + "?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				editor.DeleteDeviceFile();
		}

		private void DeleteTextDeviceFile(string deviceName, TextFileDeviceEditor editor)
		{
			if (MessageBox.Show(this, "Are you sure you want to delete the device file for the " + deviceName + "?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				editor.DeleteDeviceFile();
		}

		private void CardReaderLoadButton_Click(object sender, EventArgs e)
		{
			if (mCardReaderLoadFileDialog.ShowDialog(this) == DialogResult.OK)
				mCardReaderEditor.LoadDeviceFile(mCardReaderLoadFileDialog.FileName);
		}
	}
}