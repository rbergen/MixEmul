namespace MixGui.Events
{
	using System;

	public class AddressSelectedEventArgs : EventArgs
	{
		public int SelectedAddress { get; private set; }

		public AddressSelectedEventArgs(int selectedAddress)
		{
			SelectedAddress = selectedAddress;
		}
	}
}
