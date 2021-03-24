using System;
using MixLib.Modules;

namespace MixLib.Interrupts
{
	public static class InterruptHandler
	{
		private const int RegisterStorageAddressBase = -9;
		private const int ForcedInterruptAddress = -12;
		private const int TimerInterruptAddress = -11;
		private const int DeviceInterruptAddressBase = -20;

		public static void HandleInterrupt(Mix mix, Interrupt interrupt)
		{
			if (mix.Mode == ModuleBase.RunMode.Control)
			{
				if (interrupt.Type != Interrupt.Types.Forced)
					throw new InvalidOperationException("Only Forced interrupts are allowed in Control mode");

				mix.ProgramCounter = mix.Registers.LoadFromMemory(mix.FullMemory, RegisterStorageAddressBase);
				mix.Mode = ModuleBase.RunMode.Normal;

				mix.SignalInterruptExecuted();

				return;
			}

			mix.Mode = ModuleBase.RunMode.Control;
			mix.Registers.SaveToMemory(mix.FullMemory, RegisterStorageAddressBase, interrupt.Type == Interrupt.Types.Forced ? mix.ProgramCounter + 1 : mix.ProgramCounter);

			switch (interrupt.Type)
			{
				case Interrupt.Types.Forced:
					mix.ProgramCounter = ForcedInterruptAddress;

					break;

				case Interrupt.Types.Timer:
					mix.ProgramCounter = TimerInterruptAddress;

					break;

				case Interrupt.Types.Device:
					mix.ProgramCounter = DeviceInterruptAddressBase - interrupt.DeviceID;

					break;
			}
		}
	}
}
