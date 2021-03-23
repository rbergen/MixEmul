using System;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class MemoryExportDialog : Form
	{
		public MemoryExportDialog()
		{
			InitializeComponent();
		}

		public int MinMemoryIndex
		{
			set
			{
				mFromAddressUpDown.Minimum = value;
				mToAddressUpDown.Minimum = value;
				mProgramCounterUpDown.Minimum = value;
			}
		}

		public int MaxMemoryIndex
		{
			set
			{
				mFromAddressUpDown.Maximum = value;
				mToAddressUpDown.Maximum = value;
				mProgramCounterUpDown.Maximum = value;
			}
		}

		public int FromAddress
		{
			get
			{
				return (int)mFromAddressUpDown.Value;
			}
			set
			{
				mFromAddressUpDown.Value = value;
			}
		}

		public int ToAddress
		{
			get
			{
				return (int)mToAddressUpDown.Value;
			}
			set
			{
				mToAddressUpDown.Value = value;
			}
		}

		public int ProgramCounter
		{
			get
			{
				return (int)mProgramCounterUpDown.Value;
			}
			set
			{
				mProgramCounterUpDown.Value = value;
			}
		}

		void MFromAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (mToAddressUpDown.Value < mFromAddressUpDown.Value)
			{
				mToAddressUpDown.Value = mFromAddressUpDown.Value;
			}
		}

		void MToAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (mFromAddressUpDown.Value > mToAddressUpDown.Value)
			{
				mFromAddressUpDown.Value = mToAddressUpDown.Value;
			}
		}
	}
}
