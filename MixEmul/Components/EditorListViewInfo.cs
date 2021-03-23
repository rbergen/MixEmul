using MixLib.Type;
using System;

namespace MixGui.Components
{
	public class EditorListViewInfo : IEquatable<EditorListViewInfo>
	{
		public int FirstVisibleIndex { get; set; }
		public int? SelectedIndex { get; set; }
		public FieldTypes? FocusedField { get; set; }

		public bool Equals(EditorListViewInfo other)
		{
			return other != null && other.FirstVisibleIndex == FirstVisibleIndex;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EditorListViewInfo);
		}

		public override int GetHashCode()
		{
			return FirstVisibleIndex.GetHashCode();
		}
	}
}
