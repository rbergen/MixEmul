using System.Windows.Forms;
using MixLib.Type;

namespace MixGui.Components
{
	public class FieldKeyEventArgs : IndexKeyEventArgs
	{
		private FieldTypes mField;

		public FieldKeyEventArgs(Keys keyData, FieldTypes field)
			: this(keyData, field, null) { }

		public FieldKeyEventArgs(Keys keyData, FieldTypes field, int? index)
			: base(keyData, index)
		{
			mField = field;
		}

		public FieldTypes Field
		{
			get
			{
				return mField;
			}
		}
	}
}
