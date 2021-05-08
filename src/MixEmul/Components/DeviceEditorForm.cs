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

		private Devices devices;
		private int lastSelectedDisk;
		private int lastSelectedTape;
		private int lastSelectedDeviceTab;

		public DeviceEditorForm(Devices devices = null)
		{
			InitializeComponent();

			this.diskEditor.Initialize(DiskDevice.SectorCount, DiskDevice.WordsPerSector, DiskDevice.OpenStream, DiskDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			this.tapeEditor.Initialize(TapeDevice.CalculateRecordCount, TapeDevice.WordsPerRecord, FileBasedDevice.OpenStream, TapeDevice.CalculateBytePosition, BinaryReadStep.ReadWords, BinaryWriteStep.WriteWords);
			this.cardReaderEditor.Initialize(CardReaderDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			this.cardWriterEditor.Initialize(CardWriterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			this.printerEditor.Initialize(PrinterDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);
			this.paperTapeEditor.Initialize(PaperTapeDevice.BytesPerRecord, FileBasedDevice.OpenStream, TextReadStep.ReadBytes, TextWriteStep.WriteBytes);

			Devices = devices;

			this.lastSelectedDeviceTab = 0;
		}

		private void CloseButton_Click(object sender, EventArgs e)
			=> Hide();

		private void TapeDeleteButton_Click(object sender, EventArgs e)
			=> DeleteBinaryDeviceFile(this.tapeSelectorComboBox.SelectedItem.ToString(), this.tapeEditor);

		private void DiskDeleteButton_Click(object sender, EventArgs e)
			=> DeleteBinaryDeviceFile(this.diskSelectorComboBox.SelectedItem.ToString(), this.diskEditor);

		private void CardReaderDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("card reader", this.cardReaderEditor);

		private void CardPunchDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("card punch", this.cardWriterEditor);

		private void PrinterDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("printer", this.printerEditor);

		private void PaperTapeDeleteButton_Click(object sender, EventArgs e)
			=> DeleteTextDeviceFile("paper tape", this.paperTapeEditor);

		private void DiskSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.lastSelectedDisk == this.diskSelectorComboBox.SelectedIndex)
				return;

			if (UpdateDiskControls())
				this.lastSelectedDisk = this.diskSelectorComboBox.SelectedIndex;

			else
				this.diskSelectorComboBox.SelectedIndex = this.lastSelectedDisk;
		}

		private void TapeSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.lastSelectedTape == this.tapeSelectorComboBox.SelectedIndex)
				return;

			if (UpdateTapeControls())
				this.lastSelectedTape = this.tapeSelectorComboBox.SelectedIndex;

			else
				this.tapeSelectorComboBox.SelectedIndex = this.lastSelectedTape;
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
					this.deviceTypeTabs.SelectedIndex = DisksTabIndex;
					this.diskSelectorComboBox.SelectedItem = device;
					break;

				case TapeDevice:
					this.deviceTypeTabs.SelectedIndex = TapesTabIndex;
					this.tapeSelectorComboBox.SelectedItem = device;
					break;

				case CardReaderDevice:
					this.deviceTypeTabs.SelectedIndex = CardReaderTabIndex;
					break;

				case CardWriterDevice:
					this.deviceTypeTabs.SelectedIndex = CardWriterTabIndex;
					break;

				case PrinterDevice:
					this.deviceTypeTabs.SelectedIndex = PrinterTabIndex;
					break;

				case PaperTapeDevice:
					this.deviceTypeTabs.SelectedIndex = PaperTapeTabIndex;
					break;
			}
		}

		public Devices Devices
		{
			get => this.devices;
			set
			{
				if (value == null)
					return;

				if (this.devices != null)
					throw new InvalidOperationException("value may only be set once");

				this.devices = value;
				FileBasedDevice fileDevice;

				foreach (MixDevice device in this.devices)
				{
					switch (device)
					{
						case DiskDevice:
							this.diskSelectorComboBox.Items.Add(device);
							break;

						case TapeDevice:
							this.tapeSelectorComboBox.Items.Add(device);
							break;

						case CardReaderDevice:
							fileDevice = (FileBasedDevice)device;
							this.cardReaderEditor.SetDevice(fileDevice);
							this.cardReaderPathBox.Text = fileDevice.FilePath;
							break;

						case CardWriterDevice:
							fileDevice = (FileBasedDevice)device;
							this.cardWriterEditor.SetDevice(fileDevice);
							this.cardWriterPathBox.Text = fileDevice.FilePath;
							break;

						case PrinterDevice:
							fileDevice = (FileBasedDevice)device;
							this.printerEditor.SetDevice(fileDevice);
							this.printerPathBox.Text = fileDevice.FilePath;
							break;

						case PaperTapeDevice:
							fileDevice = (FileBasedDevice)device;
							this.paperTapeEditor.SetDevice(fileDevice);
							this.paperTapePathBox.Text = fileDevice.FilePath;
							break;
					}
				}

				this.lastSelectedDisk = 0;

				if (this.diskSelectorComboBox.Items.Count > 0)
					this.diskSelectorComboBox.SelectedIndex = 0;

				UpdateDiskControls();

				this.lastSelectedTape = 0;

				if (this.tapeSelectorComboBox.Items.Count > 0)
					this.tapeSelectorComboBox.SelectedIndex = 0;

				UpdateTapeControls();
			}
		}

		public void UpdateSettings()
		{
			this.tapeEditor.UpdateSettings();
			this.diskEditor.UpdateSettings();
			this.cardReaderEditor.UpdateSettings();
			this.cardWriterEditor.UpdateSettings();
			this.printerEditor.UpdateSettings();
			this.paperTapeEditor.UpdateSettings();

			UpdateTapeControls();
			UpdateDiskControls();
			this.cardReaderPathBox.Text = this.cardReaderEditor.Device.FilePath;
			this.cardWriterPathBox.Text = this.cardWriterEditor.Device.FilePath;
			this.printerPathBox.Text = this.printerEditor.Device.FilePath;
			this.paperTapePathBox.Text = this.paperTapeEditor.Device.FilePath;
		}

		private bool UpdateDiskControls()
		{
			bool success = true;

			if (this.lastSelectedDisk != this.diskSelectorComboBox.SelectedIndex)
				success = this.diskEditor.SaveCurrentSector();

			if (success)
			{
				var selectedDisk = (DiskDevice)this.diskSelectorComboBox.SelectedItem;
				success = this.diskEditor.SetDevice(selectedDisk);

				if (success)
					this.diskPathBox.Text = selectedDisk.FilePath;
			}

			return success;
		}

		public void UpdateLayout()
		{
			this.tapeEditor.UpdateLayout();
			this.diskEditor.UpdateLayout();
			this.cardReaderEditor.UpdateLayout();
			this.cardWriterEditor.UpdateLayout();
			this.printerEditor.UpdateLayout();
			this.paperTapeEditor.UpdateLayout();
		}

		private bool UpdateTapeControls()
		{
			bool success = true;

			if (this.lastSelectedTape != this.tapeSelectorComboBox.SelectedIndex)
				success = this.tapeEditor.SaveCurrentSector();

			if (success)
			{
				var selectedTape = (TapeDevice)this.tapeSelectorComboBox.SelectedItem;
				success = this.tapeEditor.SetDevice(selectedTape);

				if (success)
					this.tapePathBox.Text = selectedTape.FilePath;
			}

			return success;
		}

		private void DeviceEditorForm_ResizeBegin(object sender, EventArgs e)
		{
			this.diskEditor.ResizeInProgress = true;
			this.tapeEditor.ResizeInProgress = true;
			this.cardReaderEditor.ResizeInProgress = true;
			this.cardWriterEditor.ResizeInProgress = true;
			this.printerEditor.ResizeInProgress = true;
			this.paperTapeEditor.ResizeInProgress = true;
		}

		private void DeviceEditorForm_ResizeEnd(object sender, EventArgs e)
		{
			this.diskEditor.ResizeInProgress = false;
			this.tapeEditor.ResizeInProgress = false;
			this.cardReaderEditor.ResizeInProgress = false;
			this.cardWriterEditor.ResizeInProgress = false;
			this.printerEditor.ResizeInProgress = false;
			this.paperTapeEditor.ResizeInProgress = false;
		}

		private void DeviceEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;

			Hide();
		}

		private void DeviceTypeTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.deviceTypeTabs.SelectedIndex == this.lastSelectedDeviceTab)
				return;

			if (SaveCurrentTabRecord())
				this.lastSelectedDeviceTab = this.deviceTypeTabs.SelectedIndex;

			else
				this.deviceTypeTabs.SelectedIndex = this.lastSelectedDeviceTab;
		}

		private bool SaveCurrentTabRecord()
		{
			bool success = true;

			switch (this.lastSelectedDeviceTab)
			{
				case TapesTabIndex:
					success = this.tapeEditor.SaveCurrentSector();
					break;

				case DisksTabIndex:
					success = this.diskEditor.SaveCurrentSector();
					break;

				case CardReaderTabIndex:
					success = this.cardReaderEditor.Save();
					break;

				case CardWriterTabIndex:
					success = this.cardReaderEditor.Save();
					break;

				case PrinterTabIndex:
					success = this.cardReaderEditor.Save();
					break;

				case PaperTapeTabIndex:
					success = this.cardReaderEditor.Save();
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
			if (this.cardReaderLoadFileDialog.ShowDialog(this) == DialogResult.OK)
				this.cardReaderEditor.LoadDeviceFile(this.cardReaderLoadFileDialog.FileName);
		}
	}
}
