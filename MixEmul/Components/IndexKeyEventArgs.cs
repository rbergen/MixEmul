using System.Windows.Forms;

namespace MixGui.Components
{
	public class IndexKeyEventArgs : KeyEventArgs
	{
		public int? Index { get; private set; }

		public IndexKeyEventArgs(Keys keyData, int? index)
			: base(keyData)
		{
			Index = index;
		}
	}
}
