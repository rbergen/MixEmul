namespace MixGui.Events
{
	using System;

	public class AddressSelectedEventArgs(int selectedAddress) : EventArgs
	{
		public int SelectedAddress => selectedAddress;
  }
}
