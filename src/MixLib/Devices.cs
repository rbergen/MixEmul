using System.Collections;
using MixLib.Device;
using MixLib.Events;

namespace MixLib
{
	public class Devices : IEnumerable
	{
		public const int DeviceCount = 21;
		public const byte CardReaderUnitCode = 16;

		private readonly MixDevice[] _devices = new MixDevice[DeviceCount];

		public TeletypeDevice Teletype { get; private set; }

		public event DeviceReportingEventHandler DeviceReportingEvent;

		public Devices()
		{
			int index = 0;

			while (index < 8)
			{
				_devices[index] = new TapeDevice(index);
				index++;
			}

			while (index < 16)
			{
				_devices[index] = new DiskDevice(index);
				index++;
			}

			_devices[CardReaderUnitCode] = new CardReaderDevice(CardReaderUnitCode);
			_devices[17] = new CardWriterDevice(17);
			_devices[18] = new PrinterDevice(18);
			Teletype = new TeletypeDevice(19);
			_devices[19] = Teletype;
			_devices[20] = new PaperTapeDevice(20);

			foreach (MixDevice device in _devices)
				device.ReportingEvent += Device_Reporting;
		}

		public int Count 
			=> _devices.Length;

		public MixDevice this[int index] 
			=> _devices[index];

		public IEnumerator GetEnumerator() 
			=> _devices.GetEnumerator();

		protected virtual void OnDeviceReportingEvent(DeviceReportingEventArgs args) 
			=> DeviceReportingEvent?.Invoke(this, args);

		private void Device_Reporting(object sender, ReportingEventArgs args) 
			=> OnDeviceReportingEvent(new DeviceReportingEventArgs((MixDevice)sender, args.Severity, args.Message));

		public void Reset()
		{
			foreach (MixDevice device in _devices)
				device.Reset();
		}

		public void Tick()
		{
			foreach (MixDevice device in _devices)
				device.Tick();
		}

		public void UpdateSettings()
		{
			foreach (MixDevice device in _devices)
				device.UpdateSettings();
		}

		public bool IsAnyBusy
		{
			get
			{
				foreach (MixDevice device in _devices)
				{
					if (device.Busy)
						return true;
				}

				return false;
			}
		}
	}
}
