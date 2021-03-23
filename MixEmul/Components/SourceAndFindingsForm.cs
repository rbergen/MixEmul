using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Utils;
using MixLib.Instruction;
using MixLib.Misc;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class SourceAndFindingsForm : Form
	{
		Button mCloseButton;
		AssemblyFindingListView mFindingListView;
		Panel mListPanel;
		Button mLoadButton;
		SourceCodeControl mSourceControl;
		Splitter mSplitter;
		StatusStrip mStatusStrip;
		Button mExportButton;
		SaveFileDialog mSaveExportFileDialog;
		ToolStripStatusLabel mToolStripStatusLabel;
		InstructionInstanceBase[] mInstances;

		public SourceAndFindingsForm()
		{
			InitializeComponent();
			mFindingListView.SelectionChanged += MFindingListView_SelectionChanged;
		}

		public void UpdateLayout()
		{
			mSourceControl.UpdateLayout();
		}

		void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceAndFindingsForm));
			this.mStatusStrip = new System.Windows.Forms.StatusStrip();
			this.mToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.mListPanel = new System.Windows.Forms.Panel();
			this.mCloseButton = new System.Windows.Forms.Button();
			this.mExportButton = new System.Windows.Forms.Button();
			this.mLoadButton = new System.Windows.Forms.Button();
			this.mFindingListView = new MixGui.Components.AssemblyFindingListView();
			this.mSplitter = new System.Windows.Forms.Splitter();
			this.mSaveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.mSourceControl = new MixGui.Components.SourceCodeControl();
			this.mStatusStrip.SuspendLayout();
			this.mListPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mStatusStrip
			// 
			this.mStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mToolStripStatusLabel});
			this.mStatusStrip.Location = new System.Drawing.Point(0, 383);
			this.mStatusStrip.Name = "mStatusStrip";
			this.mStatusStrip.Size = new System.Drawing.Size(568, 22);
			this.mStatusStrip.TabIndex = 3;
			// 
			// mToolStripStatusLabel
			// 
			this.mToolStripStatusLabel.Name = "mToolStripStatusLabel";
			this.mToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// mListPanel
			// 
			this.mListPanel.Controls.Add(this.mCloseButton);
			this.mListPanel.Controls.Add(this.mExportButton);
			this.mListPanel.Controls.Add(this.mLoadButton);
			this.mListPanel.Controls.Add(this.mFindingListView);
			this.mListPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.mListPanel.Location = new System.Drawing.Point(0, 303);
			this.mListPanel.Name = "mListPanel";
			this.mListPanel.Size = new System.Drawing.Size(568, 80);
			this.mListPanel.TabIndex = 2;
			// 
			// mCloseButton
			// 
			this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mCloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mCloseButton.Location = new System.Drawing.Point(498, 56);
			this.mCloseButton.Name = "mCloseButton";
			this.mCloseButton.Size = new System.Drawing.Size(62, 23);
			this.mCloseButton.TabIndex = 3;
			this.mCloseButton.Text = "&Close";
			// 
			// mExportButton
			// 
			this.mExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mExportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mExportButton.Location = new System.Drawing.Point(498, 28);
			this.mExportButton.Name = "mExportButton";
			this.mExportButton.Size = new System.Drawing.Size(62, 23);
			this.mExportButton.TabIndex = 2;
			this.mExportButton.Text = "&Export...";
			// 
			// mLoadButton
			// 
			this.mLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mLoadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mLoadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mLoadButton.Location = new System.Drawing.Point(498, 0);
			this.mLoadButton.Name = "mLoadButton";
			this.mLoadButton.Size = new System.Drawing.Size(62, 23);
			this.mLoadButton.TabIndex = 1;
			this.mLoadButton.Text = "&Load";
			// 
			// mFindingListView
			// 
			this.mFindingListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mFindingListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mFindingListView.Location = new System.Drawing.Point(0, 0);
			this.mFindingListView.Name = "mFindingListView";
			this.mFindingListView.SeverityImageList = null;
			this.mFindingListView.Size = new System.Drawing.Size(492, 80);
			this.mFindingListView.TabIndex = 0;
			// 
			// mSplitter
			// 
			this.mSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.mSplitter.Location = new System.Drawing.Point(0, 295);
			this.mSplitter.MinExtra = 100;
			this.mSplitter.MinSize = 80;
			this.mSplitter.Name = "mSplitter";
			this.mSplitter.Size = new System.Drawing.Size(568, 8);
			this.mSplitter.TabIndex = 1;
			this.mSplitter.TabStop = false;
			// 
			// mSaveExportFileDialog
			// 
			this.mSaveExportFileDialog.DefaultExt = "mixdeck";
			this.mSaveExportFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			this.mSaveExportFileDialog.Title = "Specify export file name";
			// 
			// mSourceControl
			// 
			this.mSourceControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mSourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mSourceControl.Location = new System.Drawing.Point(0, 0);
			this.mSourceControl.MarkedFinding = null;
			this.mSourceControl.Name = "mSourceControl";
			this.mSourceControl.Size = new System.Drawing.Size(568, 295);
			this.mSourceControl.TabIndex = 0;
			// 
			// SourceAndFindingsForm
			// 
			this.ClientSize = new System.Drawing.Size(568, 405);
			this.Controls.Add(this.mSourceControl);
			this.Controls.Add(this.mSplitter);
			this.Controls.Add(this.mListPanel);
			this.Controls.Add(this.mStatusStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 224);
			this.Name = "SourceAndFindingsForm";
			this.ShowInTaskbar = false;
			this.Text = "Assembly result";
			this.mStatusStrip.ResumeLayout(false);
			this.mStatusStrip.PerformLayout();
			this.mListPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		void MFindingListView_SelectionChanged(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs args)
		{
			mSourceControl.MarkedFinding = args.SelectedFinding;
		}

		public void SetInstructionsAndFindings(PreInstruction[] instructions, InstructionInstanceBase[] instances, AssemblyFindingCollection findings)
		{
			mSourceControl.Instructions = instructions;
			mSourceControl.Findings = findings;
			mFindingListView.Findings = findings;
			SetStatusBarText(findings);
			mInstances = instances;
			mLoadButton.Enabled = !findings.ContainsErrors;
			mExportButton.Enabled = mLoadButton.Enabled;
		}

		void SetStatusBarText(AssemblyFindingCollection findings)
		{
			var severityNames = Enum.GetNames(typeof(Severity));
			int[] severityCounts = new int[severityNames.Length];

			foreach (AssemblyFinding finding in findings)
			{
				severityCounts[(int)finding.Severity]++;
			}

			string message = "0";
			int currentSeverityIndex = 0;

			for (int i = 0; i < severityCounts.Length; i++)
			{
				if (severityCounts[i] <= 0)
				{
					continue;
				}

				string severityText = severityCounts[i] + " " + severityNames[i].ToLower();

				message = currentSeverityIndex switch
				{
					0 => severityText,
					1 => severityText + " and " + message,
					_ => severityText + ", " + message,
				};

				currentSeverityIndex++;
			}

			message = "Showing " + message + " message";

			if (findings.Count != 1)
			{
				message += 's';
			}

			mToolStripStatusLabel.Text = message;
		}

		public ImageList SeverityImageList
		{
			get => mFindingListView.SeverityImageList;
			set => mFindingListView.SeverityImageList = value;
		}

		void MExportButton_Click(object sender, EventArgs e)
		{
			if (mSaveExportFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					CardDeckExporter.ExportInstructions(mSaveExportFileDialog.FileName, mInstances);
					MessageBox.Show(this, "Program successfully exported.", "Export successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Error while exporting program: " + ex.Message, "Error while exporting", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}
	}
}
