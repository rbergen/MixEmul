using System;

namespace MixLib.Interrupts
{
	public class Interrupt
	{
        public Types Type { get; private set; }
        public int DeviceID { get; private set; }

        public Interrupt(Types type)
		{
			if (type == Types.Device)
			{
				throw new ArgumentException("Interrupt of type Device must be constructed with device ID");
			}

			Type = type;
			DeviceID = -1;
		}

		public Interrupt(int deviceId)
		{
			Type = Types.Device;
			DeviceID = deviceId;
		}

		public enum Types
		{
			Device,
			Forced,
			Timer
		}
	}
}
