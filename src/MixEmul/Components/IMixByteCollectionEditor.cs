using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public interface IMixByteCollectionEditor : IEditor
	{
		event MixByteCollectionEditorValueChangedEventHandler ValueChanged;

		IMixByteCollection MixByteCollectionValue { get; set; }
	}
}
