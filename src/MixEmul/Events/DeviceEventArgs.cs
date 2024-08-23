using System;
using MixLib.Device;

namespace MixGui.Events
{
	public class DeviceEventArgs(MixDevice device) : EventArgs
	{
		public MixDevice Device => device;
  }
}
