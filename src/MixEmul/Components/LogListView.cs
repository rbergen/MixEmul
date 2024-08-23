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

		private readonly ColumnHeader severityColumn = new();
		private readonly ColumnHeader moduleColumn = new();
		private readonly ColumnHeader addressColumn = new();
		private readonly ColumnHeader titleColumn = new();
		private readonly ColumnHeader messageColumn = new();
		private readonly Button clearButton = new();
		private readonly ListView listView = new();

		public event AddressSelectedHandler AddressSelected;

		public LogListView()
		{
			SuspendLayout();

			this.severityColumn.Text = "Severity";
			this.severityColumn.Width = 70;

			this.moduleColumn.Text = "Module";
			this.severityColumn.Width = 70;

			this.addressColumn.Text = "Address";
			this.addressColumn.Width = 50;

			this.titleColumn.Text = "Title";
			this.titleColumn.Width = 100;

			this.messageColumn.Text = "Message";
			this.messageColumn.Width = 400;

			this.listView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			this.listView.Columns.AddRange([this.severityColumn, this.moduleColumn, this.addressColumn, this.titleColumn, this.messageColumn]);
			this.listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Location = new Point(0, 0);
			this.listView.MultiSelect = false;
			this.listView.Name = "mListView";
			this.listView.TabIndex = 0;
			this.listView.View = View.Details;
			this.listView.BorderStyle = BorderStyle.FixedSingle;
			this.listView.DoubleClick += This_DoubleClick;
			this.listView.KeyPress += ListView_KeyPress;

			this.clearButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			this.clearButton.Name = "mClearButton";
			this.clearButton.TabIndex = 1;
			this.clearButton.Text = "&Clear";
			this.clearButton.Location = new Point(Width - this.clearButton.Width, 0);
			this.clearButton.FlatStyle = FlatStyle.Flat;
			this.clearButton.Click += ClearButton_Click;

			this.listView.Size = new Size(this.clearButton.Width - 8, Size.Height);

			Controls.Add(this.listView);
			Controls.Add(this.clearButton);
			Name = "mLogListView";

			ResumeLayout(false);
		}

		public void AddLogLine(LogLine line)
			=> this.listView.Items.Insert(0, new ListViewItem(new string[] 
			{ 
				line.Severity.ToString(), 
				line.ModuleName ?? string.Empty, 
				(line.Address == -1) ? string.Empty : line.Address.ToString("D4"), 
				line.Title ?? string.Empty, 
				line.Message ?? string.Empty 
			}, (int)line.Severity));

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args)
			=> AddressSelected?.Invoke(this, args);

		private void This_DoubleClick(object sender, EventArgs args)
			=> HandleAddressSelected();

		private void ClearButton_Click(object sender, EventArgs e)
			=> this.listView.Items.Clear();

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
				ListView.SelectedListViewItemCollection selectedItems = this.listView.SelectedItems;

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
				ListView.SelectedListViewItemCollection selectedItems = this.listView.SelectedItems;

				if (selectedItems.Count == 0 || selectedItems[0].SubItems[AddressFieldIndex].Text == string.Empty)
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
			get => this.listView.SmallImageList;
			set => this.listView.SmallImageList = value;
		}
	}
}
