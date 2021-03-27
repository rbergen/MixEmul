
namespace MixGui.Components
{
	public class MixByteCollectionEditorList : EditorList<IMixByteCollectionEditor>
	{
		public MixByteCollectionEditorList(int maxWordCount = 0, CreateEditorCallback createEditor = null, LoadEditorCallback loadEditor = null)
				: base(0, maxWordCount - 1, createEditor, loadEditor) { }
	}
}
