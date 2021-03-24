using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Misc;

namespace MixGui.Components
{
	public class LogListView : UserControl
	{
		private const int ModuleFieldIndex = 1;
		private const int AddressFieldIndex = 2;
		private const int NoAddress = -1;

		private readonly ColumnHeader _severityColumn = new();
		private readonly ColumnHeader _moduleColumn = new();
		private readonly ColumnHeader _addressColumn = new();
		private readonly ColumnHeader _titleColumn = new();
		private readonly ColumnHeader _messageColumn = new();
		private readonly Button _clearButton = new();
		private readonly ListView _listView = new();

		public event AddressSelectedHandler AddressSelected;

		public LogListView()
		{
			SuspendLayout();

			_severityColumn.Text = "Severity";
			_severityColumn.Width = 70;

			_moduleColumn.Text = "Module";
			_severityColumn.Width = 70;

			_addressColumn.Text = "Address";
			_addressColumn.Width = 50;

			_titleColumn.Text = "Title";
			_titleColumn.Width = 100;

			_messageColumn.Text = "Message";
			_messageColumn.Width = 400;

			_listView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			_listView.Columns.AddRange(new ColumnHeader[] { _severityColumn, _moduleColumn, _addressColumn, _titleColumn, _messageColumn });
			_listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			_listView.FullRowSelect = true;
			_listView.GridLines = true;
			_listView.Location = new Point(0, 0);
			_listView.MultiSelect = false;
			_listView.Name = "mListView";
			_listView.TabIndex = 0;
			_listView.View = View.Details;
			_listView.BorderStyle = BorderStyle.FixedSingle;
			_listView.DoubleClick += This_DoubleClick;
			_listView.KeyPress += ListView_KeyPress;

			_clearButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			_clearButton.Name = "mClearButton";
			_clearButton.TabIndex = 1;
			_clearButton.Text = "&Clear";
			_clearButton.Location = new Point(Width - _clearButton.Width, 0);
			_clearButton.FlatStyle = FlatStyle.Flat;
			_clearButton.Click += ClearButton_Click;

			_listView.Size = new Size(_clearButton.Width - 8, Size.Height);

			Controls.Add(_listView);
			Controls.Add(_clearButton);
			Name = "mLogListView";

			ResumeLayout(false);
		}

		public void AddLogLine(LogLine line)
			=> _listView.Items.Insert(0, new ListViewItem(new string[] { line.Severity.ToString(), line.ModuleName ?? "", (line.Address == -1) ? "" : line.Address.ToString("D4"), line.Title ?? "", line.Message ?? "" }, (int)line.Severity));

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args)
			=> AddressSelected?.Invoke(this, args);

		private void This_DoubleClick(object sender, EventArgs args)
			=> HandleAddressSelected();

		private void ClearButton_Click(object sender, EventArgs e)
			=> _listView.Items.Clear();

		private void ListView_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				HandleAddressSelected();
			}
		}

		private void HandleAddressSelected()
		{
			int selectedAddress = SelectedAddress;

			if (selectedAddress != NoAddress)
				OnAddressSelected(new AddressSelectedEventArgs(selectedAddress));
		}

		public string SelectedModule
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = _listView.SelectedItems;

				if (selectedItems.Count == 0)
					return null;

				string module = selectedItems[0].SubItems[ModuleFieldIndex].Text;

				return string.IsNullOrEmpty(module) ? null : module;
			}
		}

		public int SelectedAddress
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = _listView.SelectedItems;

				if (selectedItems.Count == 0 || selectedItems[0].SubItems[AddressFieldIndex].Text == "")
					return NoAddress;

				int address = NoAddress;
				try
				{
					address = int.Parse(selectedItems[0].SubItems[AddressFieldIndex].Text);
				}
				catch (FormatException) { }

				return address;
			}
		}

		public ImageList SeverityImageList
		{
			get => _listView.SmallImageList;
			set => _listView.SmallImageList = value;
		}
	}
}
