
namespace MixGui.Components
{
	public class MixByteCollectionEditorList(int maxWordCount = 0, EditorList<IMixByteCollectionEditor>.CreateEditorCallback createEditor = null, EditorList<IMixByteCollectionEditor>.LoadEditorCallback loadEditor = null) 
		: EditorList<IMixByteCollectionEditor>(0, maxWordCount - 1, createEditor, loadEditor)
	{
  }
}
