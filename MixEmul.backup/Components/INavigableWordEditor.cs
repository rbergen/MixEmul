using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MixGui.Components
{
  interface INavigableControl
  {
    event KeyEventHandler NavigationKeyDown;
  }
}
