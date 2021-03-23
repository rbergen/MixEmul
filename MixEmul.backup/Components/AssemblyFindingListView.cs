using MixAssembler.Finding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class AssemblyFindingListView : UserControl
	{
		ListView mFindingsListView = new ListView();
		ColumnHeader mLineNumberColumn = new ColumnHeader();
		ColumnHeader mMessageColumn = new ColumnHeader();
		ColumnHeader mSeverityColumn = new ColumnHeader();

		public event SelectionChangedHandler SelectionChanged;

		public AssemblyFindingListView()
		{
			SuspendLayout();

			mSeverityColumn.Text = "Severity";
			mSeverityColumn.Width = 70;

			mLineNumberColumn.Text = "Line";
			mLineNumberColumn.Width = 40;

			mMessageColumn.Text = "Message";
			mMessageColumn.Width = 400;

			mFindingsListView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			mFindingsListView.Columns.AddRange(new ColumnHeader[] { mSeverityColumn, mLineNumberColumn, mMessageColumn });
			mFindingsListView.FullRowSelect = true;
			mFindingsListView.GridLines = true;
			mFindingsListView.Location = new Point(0, 0);
			mFindingsListView.MultiSelect = false;
			mFindingsListView.Name = "mFindingsListView";
			mFindingsListView.Size = Size;
			mFindingsListView.TabIndex = 0;
			mFindingsListView.View = View.Details;
			mFindingsListView.SelectedIndexChanged += SelectedIndexChanged;

			Controls.Add(mFindingsListView);
			Name = "AssemblyFindingListView";
			ResumeLayout(false);
		}

		protected void OnSelectionChanged(SelectionChangedEventArgs args) => SelectionChanged?.Invoke(this, args);

		void SelectedIndexChanged(object sender, EventArgs e) => OnSelectionChanged(new SelectionChangedEventArgs(SelectedFinding));

		void AddFinding(AssemblyFinding finding)
		{
			var item = new ListViewItem(new string[] { finding.Severity.ToString(), (finding.LineNumber == int.MinValue) ? "" : ((finding.LineNumber + 1)).ToString(), finding.Message }, (int)finding.Severity)
			{
				Tag = finding
			};
			mFindingsListView.Items.Add(item);
		}

		public AssemblyFindingCollection Findings
		{
			set
			{
				mFindingsListView.Items.Clear();

				if (value != null)
				{
					var list = new List<AssemblyFinding>(value);
					list.Sort(new FindingComparer());

					foreach (AssemblyFinding finding in list)
					{
						AddFinding(finding);
					}

					if (mFindingsListView.Items.Count > 0)
					{
						mFindingsListView.Items[0].Selected = true;
					}
				}
			}
		}

		public AssemblyFinding SelectedFinding
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = mFindingsListView.SelectedItems;
				if (selectedItems.Count == 0) return null;

				return (AssemblyFinding)selectedItems[0].Tag;
			}
		}

		public ImageList SeverityImageList
		{
			get
			{
				return mFindingsListView.SmallImageList;
			}
			set
			{
				mFindingsListView.SmallImageList = value;
			}
		}

		class FindingComparer : IComparer<AssemblyFinding>
		{
			public int Compare(AssemblyFinding x, AssemblyFinding y)
			{
				int comparison = y.Severity - x.Severity;
				if (comparison != 0) return comparison;

				comparison = x.LineNumber - y.LineNumber;
				if (comparison != 0) return comparison;

				comparison = x.LineSection - y.LineSection;
				if (comparison != 0) return comparison;

				return string.Compare(x.Message, y.Message, StringComparison.Ordinal);
			}
		}

		public class SelectionChangedEventArgs : EventArgs
		{
			readonly AssemblyFinding mSelectedFinding;

			public SelectionChangedEventArgs(AssemblyFinding finding)
			{
				mSelectedFinding = finding;
			}

			public AssemblyFinding SelectedFinding => mSelectedFinding;
		}

		public delegate void SelectionChangedHandler(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs e);
	}
}
