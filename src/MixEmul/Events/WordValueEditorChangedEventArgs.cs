using System;
using MixLib.Type;

namespace MixGui.Events
{
	public class WordEditorValueChangedEventArgs : EventArgs
	{
		public IWord NewValue { get; private set; }
		public IWord OldValue { get; private set; }

		public WordEditorValueChangedEventArgs(IWord oldValue, IWord newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
