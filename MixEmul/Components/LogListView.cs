using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Misc;

namespace MixGui.Components
{
	public class LogListView : UserControl
	{
        const int moduleFieldIndex = 1;
        const int addressFieldIndex = 2;
        const int noAddress = -1;

        ColumnHeader mSeverityColumn = new ColumnHeader();
        ColumnHeader mModuleColumn = new ColumnHeader();
        ColumnHeader mAddressColumn = new ColumnHeader();
        ColumnHeader mTitleColumn = new ColumnHeader();
        ColumnHeader mMessageColumn = new ColumnHeader();

        Button mClearButton = new Button();
        readonly ListView mListView = new ListView();

        public event AddressSelectedHandler AddressSelected;

		public LogListView()
		{
			SuspendLayout();

			mSeverityColumn.Text = "Severity";
			mSeverityColumn.Width = 70;

			mModuleColumn.Text = "Module";
			mSeverityColumn.Width = 70;

			mAddressColumn.Text = "Address";
			mAddressColumn.Width = 50;

			mTitleColumn.Text = "Title";
			mTitleColumn.Width = 100;

			mMessageColumn.Text = "Message";
			mMessageColumn.Width = 400;

			mListView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			mListView.Columns.AddRange(new ColumnHeader[] { mSeverityColumn, mModuleColumn, mAddressColumn, mTitleColumn, mMessageColumn });
			mListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			mListView.FullRowSelect = true;
			mListView.GridLines = true;
			mListView.Location = new Point(0, 0);
			mListView.MultiSelect = false;
			mListView.Name = "mListView";
			mListView.TabIndex = 0;
			mListView.View = View.Details;
			mListView.BorderStyle = BorderStyle.FixedSingle;
			mListView.DoubleClick += This_DoubleClick;
			mListView.KeyPress += MListView_KeyPress;

			mClearButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			mClearButton.Name = "mClearButton";
			mClearButton.TabIndex = 1;
			mClearButton.Text = "&Clear";
			mClearButton.Location = new Point(Width - mClearButton.Width, 0);
			mClearButton.Click += MClearButton_Click;

			mListView.Size = new Size(mClearButton.Width - 8, Size.Height);

			Controls.Add(mListView);
			Controls.Add(mClearButton);
			Name = "mLogListView";
			ResumeLayout(false);
		}

        public void AddLogLine(LogLine line) => 
            mListView.Items.Insert(0, new ListViewItem(new string[] { line.Severity.ToString(), line.ModuleName ?? "", (line.Address == -1) ? "" : line.Address.ToString("D4"), line.Title ?? "", line.Message ?? "" }, (int)line.Severity));

        protected virtual void OnAddressSelected(AddressSelectedEventArgs args) => AddressSelected?.Invoke(this, args);

        void This_DoubleClick(object sender, EventArgs args) => HandleAddressSelected();

        void MClearButton_Click(object sender, EventArgs e) => mListView.Items.Clear();

        void MListView_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				HandleAddressSelected();
			}
		}

        void HandleAddressSelected()
        {
            int selectedAddress = SelectedAddress;
            if (selectedAddress != noAddress) OnAddressSelected(new AddressSelectedEventArgs(selectedAddress));
        }

        public string SelectedModule
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = mListView.SelectedItems;

				if (selectedItems.Count == 0) return null;

				string module = selectedItems[0].SubItems[moduleFieldIndex].Text;

				return string.IsNullOrEmpty(module) ? null : module;
			}
		}

		public int SelectedAddress
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = mListView.SelectedItems;

				if (selectedItems.Count == 0 || selectedItems[0].SubItems[addressFieldIndex].Text == "") return noAddress;

				int address = noAddress;
				try
				{
					address = int.Parse(selectedItems[0].SubItems[addressFieldIndex].Text);
				}
				catch (FormatException)
				{
				}

				return address;
			}
		}

		public ImageList SeverityImageList
		{
			get
			{
				return mListView.SmallImageList;
			}
			set
			{
				mListView.SmallImageList = value;
			}
		}
	}
}
