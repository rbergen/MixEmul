using System;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class MemoryExportDialog : Form
	{
		public MemoryExportDialog()
			=> InitializeComponent();

		public int MinMemoryIndex
		{
			set
			{
				this.fromAddressUpDown.Minimum = value;
				this.toAddressUpDown.Minimum = value;
				this.programCounterUpDown.Minimum = value;
			}
		}

		public int MaxMemoryIndex
		{
			set
			{
				this.fromAddressUpDown.Maximum = value;
				this.toAddressUpDown.Maximum = value;
				this.programCounterUpDown.Maximum = value;
			}
		}

		public int FromAddress
		{
			get => (int)this.fromAddressUpDown.Value;
			set => this.fromAddressUpDown.Value = value;
		}

		public int ToAddress
		{
			get => (int)this.toAddressUpDown.Value;
			set => this.toAddressUpDown.Value = value;
		}

		public int ProgramCounter
		{
			get => (int)this.programCounterUpDown.Value;
			set => this.programCounterUpDown.Value = value;
		}

		private void FromAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (this.toAddressUpDown.Value < this.fromAddressUpDown.Value)
				this.toAddressUpDown.Value = this.fromAddressUpDown.Value;
		}

		private void ToAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (this.fromAddressUpDown.Value > this.toAddressUpDown.Value)
				this.fromAddressUpDown.Value = this.toAddressUpDown.Value;
		}
	}
}
