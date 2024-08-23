using System;
using MixLib.Type;

namespace MixGui.Events
{
	public class MixByteCollectionEditorValueChangedEventArgs(IMixByteCollection oldValue, IMixByteCollection newValue) : EventArgs
	{
		public IMixByteCollection NewValue => newValue;
		public IMixByteCollection OldValue => oldValue;
  }
}
