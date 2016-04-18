using System;
using MixLib.Type;

namespace MixGui.Components
{
	public class EditorListViewInfo : IEquatable<EditorListViewInfo>
	{
		public int FirstVisibleIndex { get; set; }
		public int? SelectedIndex { get; set; }
		public FieldTypes? FocusedField { get; set; }

		public bool Equals(EditorListViewInfo other) => other.FirstVisibleIndex == FirstVisibleIndex;
	}
}
