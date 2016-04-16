using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public interface IWordEditor : IEditor
	{
		event WordEditorValueChangedEventHandler ValueChanged;

		IWord WordValue { get; set; }
	}
}
