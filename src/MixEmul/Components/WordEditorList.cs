namespace MixGui.Components
{
	public class WordEditorList(int minIndex, int maxIndex, EditorList<IWordEditor>.CreateEditorCallback createEditor, EditorList<IWordEditor>.LoadEditorCallback loadEditor) 
		: EditorList<IWordEditor>(minIndex, maxIndex, createEditor, loadEditor)
	{
		public WordEditorList() : this(0, -1, null, null) { }
	}
}
