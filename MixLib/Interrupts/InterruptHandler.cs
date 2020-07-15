using MixLib.Modules;
using System;

namespace MixLib.Interrupts
{
	public static class InterruptHandler
	{
		const int registerStorageAddressBase = -9;
		const int forcedInterruptAddress = -12;
		const int timerInterruptAddress = -11;
		const int deviceInterruptAddressBase = -20;

		public static void HandleInterrupt(Mix mix, Interrupt interrupt)
		{
			if (mix.Mode == ModuleBase.RunMode.Control)
			{
				if (interrupt.Type != Interrupt.Types.Forced)
				{
					throw new InvalidOperationException("Only Forced interrupts are allowed in Control mode");
				}

				mix.ProgramCounter = mix.Registers.LoadFromMemory(mix.FullMemory, registerStorageAddressBase);
				mix.Mode = ModuleBase.RunMode.Normal;

				mix.SignalInterruptExecuted();
			}
			else
			{
				mix.Mode = ModuleBase.RunMode.Control;
				mix.Registers.SaveToMemory(mix.FullMemory, registerStorageAddressBase, interrupt.Type == Interrupt.Types.Forced ? mix.ProgramCounter + 1 : mix.ProgramCounter);

				switch (interrupt.Type)
				{
					case Interrupt.Types.Forced:
						mix.ProgramCounter = forcedInterruptAddress;

						break;

					case Interrupt.Types.Timer:
						mix.ProgramCounter = timerInterruptAddress;

						break;

					case Interrupt.Types.Device:
						mix.ProgramCounter = deviceInterruptAddressBase - interrupt.DeviceID;

						break;
				}
			}
		}
	}
}
