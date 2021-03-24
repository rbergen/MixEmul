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
				_fromAddressUpDown.Minimum = value;
				_toAddressUpDown.Minimum = value;
				_programCounterUpDown.Minimum = value;
			}
		}

		public int MaxMemoryIndex
		{
			set
			{
				_fromAddressUpDown.Maximum = value;
				_toAddressUpDown.Maximum = value;
				_programCounterUpDown.Maximum = value;
			}
		}

		public int FromAddress
		{
			get => (int)_fromAddressUpDown.Value;
			set => _fromAddressUpDown.Value = value;
		}

		public int ToAddress
		{
			get => (int)_toAddressUpDown.Value;
			set => _toAddressUpDown.Value = value;
		}

		public int ProgramCounter
		{
			get => (int)_programCounterUpDown.Value;
			set => _programCounterUpDown.Value = value;
		}

		private void FromAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_toAddressUpDown.Value < _fromAddressUpDown.Value)
				_toAddressUpDown.Value = _fromAddressUpDown.Value;
		}

		private void ToAddressUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_fromAddressUpDown.Value > _toAddressUpDown.Value)
				_fromAddressUpDown.Value = _toAddressUpDown.Value;
		}
	}
}
