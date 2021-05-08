using System.Collections;
using MixLib.Device;
using MixLib.Events;

namespace MixLib
{
	public class Devices : IEnumerable
	{
		public const int DeviceCount = 21;
		public const byte CardReaderUnitCode = 16;

		private readonly MixDevice[] devices = new MixDevice[DeviceCount];

		public TeletypeDevice Teletype { get; private set; }

		public event DeviceReportingEventHandler DeviceReportingEvent;

		public Devices()
		{
			int index = 0;

			while (index < 8)
			{
				this.devices[index] = new TapeDevice(index);
				index++;
			}

			while (index < 16)
			{
				this.devices[index] = new DiskDevice(index);
				index++;
			}

			this.devices[CardReaderUnitCode] = new CardReaderDevice(CardReaderUnitCode);
			this.devices[17] = new CardWriterDevice(17);
			this.devices[18] = new PrinterDevice(18);
			Teletype = new TeletypeDevice(19);
			this.devices[19] = Teletype;
			this.devices[20] = new PaperTapeDevice(20);

			foreach (MixDevice device in this.devices)
				device.ReportingEvent += Device_Reporting;
		}

		public int Count 
			=> this.devices.Length;

		public MixDevice this[int index] 
			=> this.devices[index];

		public IEnumerator GetEnumerator() 
			=> this.devices.GetEnumerator();

		protected virtual void OnDeviceReportingEvent(DeviceReportingEventArgs args) 
			=> DeviceReportingEvent?.Invoke(this, args);

		private void Device_Reporting(object sender, ReportingEventArgs args) 
			=> OnDeviceReportingEvent(new DeviceReportingEventArgs((MixDevice)sender, args.Severity, args.Message));

		public void Reset()
		{
			foreach (MixDevice device in this.devices)
				device.Reset();
		}

		public void Tick()
		{
			foreach (MixDevice device in this.devices)
				device.Tick();
		}

		public void UpdateSettings()
		{
			foreach (MixDevice device in this.devices)
				device.UpdateSettings();
		}

		public bool IsAnyBusy
		{
			get
			{
				foreach (MixDevice device in this.devices)
				{
					if (device.Busy)
						return true;
				}

				return false;
			}
		}
	}
}
