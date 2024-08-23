using System.Windows.Forms;

namespace MixGui.Components
{
	public class IndexKeyEventArgs(Keys keyData, int? index) : KeyEventArgs(keyData)
	{
		public int? Index => index;
  }
}
