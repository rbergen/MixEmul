using MixLib.Type;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class FieldKeyEventArgs : IndexKeyEventArgs
	{
		public FieldTypes Field { get; private set; }

		public FieldKeyEventArgs(Keys keyData, FieldTypes field) : this(keyData, field, null) { }

		public FieldKeyEventArgs(Keys keyData, FieldTypes field, int? index) : base(keyData, index) 
			=> Field = field;
	}
}
