using System.Windows.Forms;
using MixLib.Type;

namespace MixGui.Components
{
	public class FieldKeyEventArgs(Keys keyData, FieldTypes field, int? index) : IndexKeyEventArgs(keyData, index)
	{
		public FieldTypes Field => @field;

		public FieldKeyEventArgs(Keys keyData, FieldTypes field) : this(keyData, field, null) { }
  }
}
