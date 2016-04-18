using System;
using System.ComponentModel;
using System.Windows.Forms;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Utils;
using MixLib.Instruction;
using MixLib.Misc;

namespace MixGui.Components
{
	public class SourceAndFindingsForm : Form
	{
		private Container components = null;
		private Button mCloseButton;
		private AssemblyFindingListView mFindingListView;
		private Panel mListPanel;
		private Button mLoadButton;
		private SourceCodeControl mSourceControl;
		private Splitter mSplitter;
		private StatusBar mStatusBar;
		private Button mExportButton;
		private SaveFileDialog mSaveExportFileDialog;
		private StatusBarPanel mStatusBarPanel;
		private InstructionInstanceBase[] mInstances;

		public SourceAndFindingsForm()
		{
			InitializeComponent();
			mFindingListView.SelectionChanged += new AssemblyFindingListView.SelectionChangedHandler(mFindingListView_SelectionChanged);
		}

        public void UpdateLayout() => mSourceControl.UpdateLayout();

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SourceAndFindingsForm));
            mStatusBar = new System.Windows.Forms.StatusBar();
            mStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            mListPanel = new System.Windows.Forms.Panel();
            mCloseButton = new Button();
            mExportButton = new Button();
            mLoadButton = new Button();
            mFindingListView = new MixGui.Components.AssemblyFindingListView();
            mSplitter = new System.Windows.Forms.Splitter();
            mSaveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
            mSourceControl = new MixGui.Components.SourceCodeControl();
			((System.ComponentModel.ISupportInitialize)(mStatusBarPanel)).BeginInit();
            mListPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mStatusBar
            // 
            mStatusBar.Location = new System.Drawing.Point(0, 383);
            mStatusBar.Name = "mStatusBar";
            mStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            mStatusBarPanel});
            mStatusBar.ShowPanels = true;
            mStatusBar.Size = new System.Drawing.Size(568, 22);
            mStatusBar.TabIndex = 3;
            // 
            // mStatusBarPanel
            // 
            mStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            mStatusBarPanel.Name = "mStatusBarPanel";
            mStatusBarPanel.Width = 551;
            // 
            // mListPanel
            // 
            mListPanel.Controls.Add(mCloseButton);
            mListPanel.Controls.Add(mExportButton);
            mListPanel.Controls.Add(mLoadButton);
            mListPanel.Controls.Add(mFindingListView);
            mListPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            mListPanel.Location = new System.Drawing.Point(0, 303);
            mListPanel.Name = "mListPanel";
            mListPanel.Size = new System.Drawing.Size(568, 80);
            mListPanel.TabIndex = 2;
            // 
            // mCloseButton
            // 
            mCloseButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            mCloseButton.DialogResult = DialogResult.Cancel;
            mCloseButton.Location = new System.Drawing.Point(498, 53);
            mCloseButton.Name = "mCloseButton";
            mCloseButton.Size = new System.Drawing.Size(62, 23);
            mCloseButton.TabIndex = 3;
            mCloseButton.Text = "&Close";
            // 
            // mExportButton
            // 
            mExportButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            mExportButton.Location = new System.Drawing.Point(498, 28);
            mExportButton.Name = "mExportButton";
            mExportButton.Size = new System.Drawing.Size(62, 23);
            mExportButton.TabIndex = 2;
            mExportButton.Text = "&Export...";
            mExportButton.Click += new EventHandler(mExportButton_Click);
            // 
            // mLoadButton
            // 
            mLoadButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            mLoadButton.DialogResult = DialogResult.OK;
            mLoadButton.Location = new System.Drawing.Point(498, 3);
            mLoadButton.Name = "mLoadButton";
            mLoadButton.Size = new System.Drawing.Size(62, 23);
            mLoadButton.TabIndex = 1;
            mLoadButton.Text = "&Load";
            // 
            // mFindingListView
            // 
            mFindingListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mFindingListView.Location = new System.Drawing.Point(0, 0);
            mFindingListView.Name = "mFindingListView";
            mFindingListView.SeverityImageList = null;
            mFindingListView.Size = new System.Drawing.Size(492, 80);
            mFindingListView.TabIndex = 0;
            // 
            // mSplitter
            // 
            mSplitter.BorderStyle = BorderStyle.Fixed3D;
            mSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            mSplitter.Location = new System.Drawing.Point(0, 295);
            mSplitter.MinExtra = 100;
            mSplitter.MinSize = 80;
            mSplitter.Name = "mSplitter";
            mSplitter.Size = new System.Drawing.Size(568, 8);
            mSplitter.TabIndex = 1;
            mSplitter.TabStop = false;
            // 
            // mSaveExportFileDialog
            // 
            mSaveExportFileDialog.DefaultExt = "mixdeck";
            mSaveExportFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
            mSaveExportFileDialog.Title = "Specify export file name";
            // 
            // mSourceControl
            // 
            mSourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
            mSourceControl.Location = new System.Drawing.Point(0, 0);
            mSourceControl.MarkedFinding = null;
            mSourceControl.Name = "mSourceControl";
            mSourceControl.Size = new System.Drawing.Size(568, 295);
            mSourceControl.TabIndex = 0;
            // 
            // SourceAndFindingsForm
            // 
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(568, 405);
            Controls.Add(mSourceControl);
            Controls.Add(mSplitter);
            Controls.Add(mListPanel);
            Controls.Add(mStatusBar);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(400, 224);
            Name = "SourceAndFindingsForm";
            ShowInTaskbar = false;
            Text = "Assembly result";
			((System.ComponentModel.ISupportInitialize)(mStatusBarPanel)).EndInit();
            mListPanel.ResumeLayout(false);
            ResumeLayout(false);

		}

		private void mFindingListView_SelectionChanged(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs args)
		{
			mSourceControl.MarkedFinding = args.SelectedFinding;
		}

		public void SetInstructionsAndFindings(PreInstruction[] instructions, InstructionInstanceBase[] instances, AssemblyFindingCollection findings)
		{
			mSourceControl.Instructions = instructions;
			mSourceControl.Findings = findings;
			mFindingListView.Findings = findings;
			setStatusBarText(findings);
			mInstances = instances;
			mLoadButton.Enabled = !findings.ContainsErrors;
			mExportButton.Enabled = mLoadButton.Enabled;
		}

		private void setStatusBarText(AssemblyFindingCollection findings)
		{
			string[] severityNames = Enum.GetNames(typeof(Severity));
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

				string severityText = severityCounts[i].ToString() + " " + severityNames[i].ToLower();

				switch (currentSeverityIndex)
				{
					case 0:
						message = severityText;
						break;

					case 1:
						message = severityText + " and " + message;
						break;

					default:
						message = severityText + ", " + message;
						break;
				}

				currentSeverityIndex++;
			}

			message = "Showing " + message + " message";

			if (findings.Count != 1)
			{
				message = message + 's';
			}

			mStatusBarPanel.Text = message;
		}

		public ImageList SeverityImageList
		{
			get
			{
				return mFindingListView.SeverityImageList;
			}
			set
			{
				mFindingListView.SeverityImageList = value;
			}
		}

		private void mExportButton_Click(object sender, EventArgs e)
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
