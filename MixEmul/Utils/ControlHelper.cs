using System;
using System.Windows.Forms;

namespace MixGui.Utils
{
	public static class ControlHelper
	{
        const int WM_SETREDRAW = 0x000B;

        public static void SuspendDrawing(this Control control)
		{
			Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
					IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msgSuspendUpdate);
		}

		public static void ResumeDrawing(this Control control)
		{
			// Create a C "true" boolean as an IntPtr
			var wparam = new IntPtr(1);
			Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
					IntPtr.Zero);

			NativeWindow window = NativeWindow.FromHandle(control.Handle);
			window.DefWndProc(ref msgResumeUpdate);

			control.Invalidate();
		}

		public static bool ControlHasFocus(this ContainerControl container, Control control)
		{
			Control activeControl = null;

			while (container != null)
			{
				activeControl = container.ActiveControl;
				if (activeControl == control)
				{
					return true;
				}

				container = activeControl as ContainerControl;
			}

			return false;
		}

		public static bool FocusWithIndex(this TextBoxBase textBox, int? index)
		{
			if (textBox.Focus())
			{
				if (index.HasValue)
				{
					textBox.Select(index.Value > textBox.TextLength ? textBox.TextLength : index.Value, 0);
				}

				return true;
			}

			return false;
		}
	}
}
