using System.Windows.Forms;

namespace MixGui.Components
{
	internal interface INavigableControl
	{
		event KeyEventHandler NavigationKeyDown;
	}
}
