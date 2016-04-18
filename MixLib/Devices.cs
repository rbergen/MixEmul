using System.Collections;
using MixLib.Device;
using MixLib.Events;

namespace MixLib
{
    public class Devices : IEnumerable
    {
        public const int DeviceCount = 21;
        public const byte CardReaderUnitCode = 16;

        private MixDevice[] mDevices = new MixDevice[DeviceCount];

        public TeletypeDevice Teletype { get; private set; }

        public event DeviceReportingEventHandler DeviceReportingEvent;

        public Devices()
        {
            int index = 0;

            while (index < 8)
            {
                mDevices[index] = new TapeDevice(index);
                index++;
            }

            while (index < 16)
            {
                mDevices[index] = new DiskDevice(index);
                index++;
            }

            mDevices[CardReaderUnitCode] = new CardReaderDevice(CardReaderUnitCode);
            mDevices[17] = new CardWriterDevice(17);
            mDevices[18] = new PrinterDevice(18);
            Teletype = new TeletypeDevice(19);
            mDevices[19] = Teletype;
            mDevices[20] = new PaperTapeDevice(20);

            foreach (MixDevice device in mDevices)
            {
                device.ReportingEvent += new ReportingEventHandler(device_Reporting);
            }
        }

        private void device_Reporting(object sender, ReportingEventArgs args)
        {
            OnDeviceReportingEvent(new DeviceReportingEventArgs((MixDevice)sender, args.Severity, args.Message));
        }

        public IEnumerator GetEnumerator()
        {
            return mDevices.GetEnumerator();
        }

        protected virtual void OnDeviceReportingEvent(DeviceReportingEventArgs args)
        {
            DeviceReportingEvent?.Invoke(this, args);
        }

        public void Reset()
        {
            foreach (MixDevice device in mDevices)
            {
                device.Reset();
            }
        }

        public void Tick()
        {
            foreach (MixDevice device in mDevices)
            {
                device.Tick();
            }
        }

        public void UpdateSettings()
        {
            foreach (MixDevice device in mDevices)
            {
                device.UpdateSettings();
            }
        }

        public int Count
        {
            get
            {
                return mDevices.Length;
            }
        }

        public bool IsAnyBusy
        {
            get
            {
                foreach (MixDevice device in mDevices)
                {
                    if (device.Busy)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public MixDevice this[int index]
        {
            get
            {
                return mDevices[index];
            }
        }
    }
}
