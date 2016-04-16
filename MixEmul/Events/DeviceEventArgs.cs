using System;
using MixLib.Device;

namespace MixGui.Events
{
	public class DeviceEventArgs : EventArgs
	{
        public MixDevice Device { get; private set; }

        public DeviceEventArgs(MixDevice device)
		{
			Device = device;
		}
	}
}
