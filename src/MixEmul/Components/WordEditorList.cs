namespace MixGui.Components
{
	public class WordEditorList : EditorList<IWordEditor>
	{
		public WordEditorList() : this(0, -1, null, null) { }

		public WordEditorList(int minIndex, int maxIndex, CreateEditorCallback createEditor, LoadEditorCallback loadEditor)
			: base(minIndex, maxIndex, createEditor, loadEditor) { }
	}
}