using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class MemoryExportDialog : Form
	{
		public MemoryExportDialog()
			=> InitializeComponent();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MinMemoryIndex
		{
			set
			{
				this.fromAddressUpDown.Minimum = value;
				this.toAddressUpDown.Minimum = value;
				this.programCounterUpDown.Minimum = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MaxMemoryIndex
		{
			set
			{
				this.fromAddressUpDown.Maximum = value;
				this.toAddressUpDown.Maximum = value;
				this.programCounterUpDown.Maximum = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FromAddress
		{
			get => (int)this.fromAddressUpDown.Value;
			set => this.fromAddressUpDown.Value = value;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ToAddress
		{
			get => (int)this.toAddressUpDown.Value;
			set => this.toAddressUpDown.Value = value;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
