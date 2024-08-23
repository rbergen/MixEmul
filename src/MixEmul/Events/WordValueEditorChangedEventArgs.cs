using System;
using MixLib.Type;

namespace MixGui.Events
{
	public class WordEditorValueChangedEventArgs(IWord oldValue, IWord newValue) : EventArgs
	{
		public IWord NewValue => newValue;
		public IWord OldValue => oldValue;
  }
}
