using System;
using MixLib.Type;

namespace MixGui.Events
{
	public class MixByteCollectionEditorValueChangedEventArgs : EventArgs
	{
		public IMixByteCollection NewValue { get; private set; }
		public IMixByteCollection OldValue { get; private set; }

		public MixByteCollectionEditorValueChangedEventArgs(IMixByteCollection oldValue, IMixByteCollection newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
