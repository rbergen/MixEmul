using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixAssembler.Finding;

namespace MixGui.Components
{
	public class AssemblyFindingListView : UserControl
	{
		private readonly ListView mFindingsListView = new();
		private readonly ColumnHeader mLineNumberColumn = new();
		private readonly ColumnHeader mMessageColumn = new();
		private readonly ColumnHeader mSeverityColumn = new();

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
			mFindingsListView.Columns.AddRange([mSeverityColumn, mLineNumberColumn, mMessageColumn]);
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

		protected void OnSelectionChanged(SelectionChangedEventArgs args)
			=> SelectionChanged?.Invoke(this, args);

		private void SelectedIndexChanged(object sender, EventArgs e)
			=> OnSelectionChanged(new SelectionChangedEventArgs(SelectedFinding));

		private void AddFinding(AssemblyFinding finding)
		{
			var item = new ListViewItem(new string[] { finding.Severity.ToString(), (finding.LineNumber == int.MinValue) ? string.Empty : (finding.LineNumber + 1).ToString(), finding.Message }, (int)finding.Severity)
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

				if (value == null)
					return;

				var list = new List<AssemblyFinding>(value);
				list.Sort(new FindingComparer());

				foreach (AssemblyFinding finding in list)
					AddFinding(finding);

				if (mFindingsListView.Items.Count > 0)
					mFindingsListView.Items[0].Selected = true;
			}
		}

		public AssemblyFinding SelectedFinding
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = mFindingsListView.SelectedItems;

				return selectedItems.Count == 0 ? null : (AssemblyFinding)selectedItems[0].Tag;
			}
		}

		public ImageList SeverityImageList
		{
			get => mFindingsListView.SmallImageList;
			set => mFindingsListView.SmallImageList = value;
		}

		private class FindingComparer : IComparer<AssemblyFinding>
		{
			public int Compare(AssemblyFinding x, AssemblyFinding y)
			{
				int comparison = y.Severity - x.Severity;

				if (comparison != 0)
					return comparison;

				comparison = x.LineNumber - y.LineNumber;

				if (comparison != 0)
					return comparison;

				comparison = x.LineSection - y.LineSection;

				return comparison != 0 ? comparison : string.Compare(x.Message, y.Message, StringComparison.Ordinal);
			}
		}

		public class SelectionChangedEventArgs(AssemblyFinding finding) : EventArgs
		{
			private readonly AssemblyFinding mSelectedFinding = finding;

			public AssemblyFinding SelectedFinding => mSelectedFinding;
		}

		public delegate void SelectionChangedHandler(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs e);
	}
}
