using MixLib.Type;
using System.Windows.Forms;

namespace MixGui.Components
{
	public interface IEditor
	{
		FieldTypes? FocusedField { get; }
		int? CaretIndex { get; }
		Control EditorControl { get; }
		bool ReadOnly { get; set; }

		bool Focus(FieldTypes? field, int? index);
		void Update();
		void UpdateLayout();
		void Select(int start, int length);
	}
}
