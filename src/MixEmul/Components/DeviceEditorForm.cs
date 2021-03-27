using System;
using System.Windows.Forms;
using MixLib;
using MixLib.Device;
using MixLib.Device.Step;

namespace MixGui.Components
{
	public partial class DeviceEditorForm : Form
	{
		private const int TapesTabIndex = 0;
		private const int DisksTabIndex = 1;
		private const int CardReaderTabIndex = 2;
		private const int CardWriterTabIndex = 3;
		private const int PrinterTabIndex = 4;
		private const int PaperTapeTabIndex = 5;

		private Devices _devices;
		private int _lastSelectedDisk;
		private int _lastSelectedTape;
		private int _lastSelectedDeviceTab;

		public DeviceEditorForm(Devices devices = null)
		{
			InitializeComponent();

			_diskEditor.Initialize(DiskDevice.SectorCount, DiskDevice.WordsPerSector, DiskDevice.OpenStream, DiskDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			_tapeEditor.Initialize(TapeDevice.CalculateRecordCount, TapeDevice.WordsPerRecord, FileBasedDevice.OpenStream, TapeDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			_cardReaderEditor.Initialize(CardReaderDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			_cardWriterEditor.Initialize(CardWriterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			_printerEditor.Initialize(PrinterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			_paperTapeEditor.Initialize(PaperTapeDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);

			Devices = devices;

			_lastSelectedDeviceTab = 0;
		}

		private void CloseButton_Click(object sender, EventArgs e)
			=> Hide();

		private void TapeDeleteButton_Click(object sender, EventArgs e)
			=> DeleteBinaryDeviceFile(_tapeSelectorComboBox.SelectedItem.ToString(), _tapeEditor);

		private void DiskDeleteButton_Click(object sender, EventArgs e)
			=> DeleteBinaryDeviceFile(_diskSelectorComboBox.SelectedItem.ToString(), _diskEditor);

		private void CardReaderDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("card reader", _cardReaderEditor);

		private void CardPunchDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("card punch", _cardWriterEditor);

		private void PrinterDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("printer", _printerEditor);

		private void PaperTapeDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("paper tape", _paperTapeEditor);

		private void DiskSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_lastSelectedDisk == _diskSelectorComboBox.SelectedIndex)
				return;

			if (UpdateDiskControls())
				_lastSelectedDisk = _diskSelectorComboBox.SelectedIndex;

			else
				_diskSelectorComboBox.SelectedIndex = _lastSelectedDisk;
		}

		private void TapeSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_lastSelectedTape == _tapeSelectorComboBox.SelectedIndex)
				return;

			if (UpdateTapeControls())
				_lastSelectedTape = _tapeSelectorComboBox.SelectedIndex;

			else
				_tapeSelectorComboBox.SelectedIndex = _lastSelectedTape;
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
					_deviceTypeTabs.SelectedIndex = DisksTabIndex;
					_diskSelectorComboBox.SelectedItem = device;
					break;

				case TapeDevice:
					_deviceTypeTabs.SelectedIndex = TapesTabIndex;
					_tapeSelectorComboBox.SelectedItem = device;
					break;

				case CardReaderDevice:
					_deviceTypeTabs.SelectedIndex = CardReaderTabIndex;
					break;

				case CardWriterDevice:
					_deviceTypeTabs.SelectedIndex = CardWriterTabIndex;
					break;

				case PrinterDevice:
					_deviceTypeTabs.SelectedIndex = PrinterTabIndex;
					break;

				case PaperTapeDevice:
					_deviceTypeTabs.SelectedIndex = PaperTapeTabIndex;
					break;
			}
		}

		public Devices Devices
		{
			get => _devices;
			set
			{
				if (value == null)
					return;

				if (_devices != null)
					throw new InvalidOperationException("value may only be set once");

				_devices = value;
				FileBasedDevice fileDevice;

				foreach (MixDevice device in _devices)
				{
					switch (device)
					{
						case DiskDevice:
							_diskSelectorComboBox.Items.Add(device);
							break;

						case TapeDevice:
							_tapeSelectorComboBox.Items.Add(device);
							break;

						case CardReaderDevice:
							fileDevice = (FileBasedDevice)device;
							_cardReaderEditor.SetDevice(fileDevice);
							_cardReaderPathBox.Text = fileDevice.FilePath;
							break;

						case CardWriterDevice:
							fileDevice = (FileBasedDevice)device;
							_cardWriterEditor.SetDevice(fileDevice);
							_cardWriterPathBox.Text = fileDevice.FilePath;
							break;

						case PrinterDevice:
							fileDevice = (FileBasedDevice)device;
							_printerEditor.SetDevice(fileDevice);
							_printerPathBox.Text = fileDevice.FilePath;
							break;

						case PaperTapeDevice:
							fileDevice = (FileBasedDevice)device;
							_paperTapeEditor.SetDevice(fileDevice);
							_paperTapePathBox.Text = fileDevice.FilePath;
							break;
					}
				}

				_lastSelectedDisk = 0;

				if (_diskSelectorComboBox.Items.Count > 0)
					_diskSelectorComboBox.SelectedIndex = 0;

				UpdateDiskControls();

				_lastSelectedTape = 0;

				if (_tapeSelectorComboBox.Items.Count > 0)
					_tapeSelectorComboBox.SelectedIndex = 0;

				UpdateTapeControls();
			}
		}

		public void UpdateSettings()
		{
			_tapeEditor.UpdateSettings();
			_diskEditor.UpdateSettings();
			_cardReaderEditor.UpdateSettings();
			_cardWriterEditor.UpdateSettings();
			_printerEditor.UpdateSettings();
			_paperTapeEditor.UpdateSettings();

			UpdateTapeControls();
			UpdateDiskControls();
			_cardReaderPathBox.Text = _cardReaderEditor.Device.FilePath;
			_cardWriterPathBox.Text = _cardWriterEditor.Device.FilePath;
			_printerPathBox.Text = _printerEditor.Device.FilePath;
			_paperTapePathBox.Text = _paperTapeEditor.Device.FilePath;
		}

		private bool UpdateDiskControls()
		{
			bool success = true;

			if (_lastSelectedDisk != _diskSelectorComboBox.SelectedIndex)
				success = _diskEditor.SaveCurrentSector();

			if (success)
			{
				var selectedDisk = (DiskDevice)_diskSelectorComboBox.SelectedItem;
				success = _diskEditor.SetDevice(selectedDisk);

				if (success)
					_diskPathBox.Text = selectedDisk.FilePath;
			}

			return success;
		}

		public void UpdateLayout()
		{
			_tapeEditor.UpdateLayout();
			_diskEditor.UpdateLayout();
			_cardReaderEditor.UpdateLayout();
			_cardWriterEditor.UpdateLayout();
			_printerEditor.UpdateLayout();
			_paperTapeEditor.UpdateLayout();
		}

		private bool UpdateTapeControls()
		{
			bool success = true;

			if (_lastSelectedTape != _tapeSelectorComboBox.SelectedIndex)
				success = _tapeEditor.SaveCurrentSector();

			if (success)
			{
				var selectedTape = (TapeDevice)_tapeSelectorComboBox.SelectedItem;
				success = _tapeEditor.SetDevice(selectedTape);

				if (success)
					_tapePathBox.Text = selectedTape.FilePath;
			}

			return success;
		}

		private void DeviceEditorForm_ResizeBegin(object sender, EventArgs e)
		{
			_diskEditor.ResizeInProgress = true;
			_tapeEditor.ResizeInProgress = true;
			_cardReaderEditor.ResizeInProgress = true;
			_cardWriterEditor.ResizeInProgress = true;
			_printerEditor.ResizeInProgress = true;
			_paperTapeEditor.ResizeInProgress = true;
		}

		private void DeviceEditorForm_ResizeEnd(object sender, EventArgs e)
		{
			_diskEditor.ResizeInProgress = false;
			_tapeEditor.ResizeInProgress = false;
			_cardReaderEditor.ResizeInProgress = false;
			_cardWriterEditor.ResizeInProgress = false;
			_printerEditor.ResizeInProgress = false;
			_paperTapeEditor.ResizeInProgress = false;
		}

		private void DeviceEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;

			Hide();
		}

		private void DeviceTypeTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_deviceTypeTabs.SelectedIndex == _lastSelectedDeviceTab)
				return;

			if (SaveCurrentTabRecord())
				_lastSelectedDeviceTab = _deviceTypeTabs.SelectedIndex;

			else
				_deviceTypeTabs.SelectedIndex = _lastSelectedDeviceTab;
		}

		private bool SaveCurrentTabRecord()
		{
			bool success = true;

			switch (_lastSelectedDeviceTab)
			{
				case TapesTabIndex:
					success = _tapeEditor.SaveCurrentSector();
					break;

				case DisksTabIndex:
					success = _diskEditor.SaveCurrentSector();
					break;

				case CardReaderTabIndex:
					success = _cardReaderEditor.Save();
					break;

				case CardWriterTabIndex:
					success = _cardReaderEditor.Save();
					break;

				case PrinterTabIndex:
					success = _cardReaderEditor.Save();
					break;

				case PaperTapeTabIndex:
					success = _cardReaderEditor.Save();
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
			if (_cardReaderLoadFileDialog.ShowDialog(this) == DialogResult.OK)
				_cardReaderEditor.LoadDeviceFile(_cardReaderLoadFileDialog.FileName);
		}
	}
}
