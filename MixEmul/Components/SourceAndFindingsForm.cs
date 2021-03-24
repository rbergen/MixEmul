using System;
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
		private Button _closeButton;
		private AssemblyFindingListView _findingListView;
		private Panel _listPanel;
		private Button _loadButton;
		private SourceCodeControl _sourceControl;
		private Splitter _splitter;
		private StatusStrip _statusStrip;
		private Button _exportButton;
		private SaveFileDialog _saveExportFileDialog;
		private ToolStripStatusLabel _toolStripStatusLabel;
		private InstructionInstanceBase[] _instances;

		public SourceAndFindingsForm()
		{
			InitializeComponent();
			_findingListView.SelectionChanged += FindingListView_SelectionChanged;
		}

		public void UpdateLayout() 
			=> _sourceControl.UpdateLayout();

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new(typeof(SourceAndFindingsForm));
			_statusStrip = new System.Windows.Forms.StatusStrip();
			_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			_listPanel = new System.Windows.Forms.Panel();
			_closeButton = new System.Windows.Forms.Button();
			_exportButton = new System.Windows.Forms.Button();
			_loadButton = new System.Windows.Forms.Button();
			_findingListView = new MixGui.Components.AssemblyFindingListView();
			_splitter = new System.Windows.Forms.Splitter();
			_saveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
			_sourceControl = new MixGui.Components.SourceCodeControl();
			_statusStrip.SuspendLayout();
			_listPanel.SuspendLayout();
			SuspendLayout();
			// 
			// mStatusStrip
			// 
			_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
						_toolStripStatusLabel});
			_statusStrip.Location = new System.Drawing.Point(0, 383);
			_statusStrip.Name = "mStatusStrip";
			_statusStrip.Size = new System.Drawing.Size(568, 22);
			_statusStrip.TabIndex = 3;
			// 
			// mToolStripStatusLabel
			// 
			_toolStripStatusLabel.Name = "mToolStripStatusLabel";
			_toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// mListPanel
			// 
			_listPanel.Controls.Add(_closeButton);
			_listPanel.Controls.Add(_exportButton);
			_listPanel.Controls.Add(_loadButton);
			_listPanel.Controls.Add(_findingListView);
			_listPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			_listPanel.Location = new System.Drawing.Point(0, 303);
			_listPanel.Name = "mListPanel";
			_listPanel.Size = new System.Drawing.Size(568, 80);
			_listPanel.TabIndex = 2;
			// 
			// mCloseButton
			// 
			_closeButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_closeButton.Location = new System.Drawing.Point(498, 56);
			_closeButton.Name = "mCloseButton";
			_closeButton.Size = new System.Drawing.Size(62, 23);
			_closeButton.TabIndex = 3;
			_closeButton.Text = "&Close";
			// 
			// mExportButton
			// 
			_exportButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_exportButton.Location = new System.Drawing.Point(498, 28);
			_exportButton.Name = "mExportButton";
			_exportButton.Size = new System.Drawing.Size(62, 23);
			_exportButton.TabIndex = 2;
			_exportButton.Text = "&Export...";
			// 
			// mLoadButton
			// 
			_loadButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_loadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_loadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_loadButton.Location = new System.Drawing.Point(498, 0);
			_loadButton.Name = "mLoadButton";
			_loadButton.Size = new System.Drawing.Size(62, 23);
			_loadButton.TabIndex = 1;
			_loadButton.Text = "&Load";
			// 
			// mFindingListView
			// 
			_findingListView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right);
			_findingListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_findingListView.Location = new System.Drawing.Point(0, 0);
			_findingListView.Name = "mFindingListView";
			_findingListView.SeverityImageList = null;
			_findingListView.Size = new System.Drawing.Size(492, 80);
			_findingListView.TabIndex = 0;
			// 
			// mSplitter
			// 
			_splitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			_splitter.Location = new System.Drawing.Point(0, 295);
			_splitter.MinExtra = 100;
			_splitter.MinSize = 80;
			_splitter.Name = "mSplitter";
			_splitter.Size = new System.Drawing.Size(568, 8);
			_splitter.TabIndex = 1;
			_splitter.TabStop = false;
			// 
			// mSaveExportFileDialog
			// 
			_saveExportFileDialog.DefaultExt = "mixdeck";
			_saveExportFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			_saveExportFileDialog.Title = "Specify export file name";
			// 
			// mSourceControl
			// 
			_sourceControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_sourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
			_sourceControl.Location = new System.Drawing.Point(0, 0);
			_sourceControl.MarkedFinding = null;
			_sourceControl.Name = "mSourceControl";
			_sourceControl.Size = new System.Drawing.Size(568, 295);
			_sourceControl.TabIndex = 0;
			// 
			// SourceAndFindingsForm
			// 
			ClientSize = new System.Drawing.Size(568, 405);
			Controls.Add(_sourceControl);
			Controls.Add(_splitter);
			Controls.Add(_listPanel);
			Controls.Add(_statusStrip);
			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(400, 224);
			Name = "SourceAndFindingsForm";
			ShowInTaskbar = false;
			Text = "Assembly result";
			_statusStrip.ResumeLayout(false);
			_statusStrip.PerformLayout();
			_listPanel.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();

		}

		private void FindingListView_SelectionChanged(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs args) 
			=> _sourceControl.MarkedFinding = args.SelectedFinding;

		public void SetInstructionsAndFindings(PreInstruction[] instructions, InstructionInstanceBase[] instances, AssemblyFindingCollection findings)
		{
			_sourceControl.Instructions = instructions;
			_sourceControl.Findings = findings;
			_findingListView.Findings = findings;
			SetStatusBarText(findings);
			_instances = instances;
			_loadButton.Enabled = !findings.ContainsErrors;
			_exportButton.Enabled = _loadButton.Enabled;
		}

		private void SetStatusBarText(AssemblyFindingCollection findings)
		{
			var severityNames = Enum.GetNames(typeof(Severity));
			int[] severityCounts = new int[severityNames.Length];

			foreach (AssemblyFinding finding in findings)
				severityCounts[(int)finding.Severity]++;


			string message = "0";
			int currentSeverityIndex = 0;

			for (int i = 0; i < severityCounts.Length; i++)
			{
				if (severityCounts[i] <= 0)
					continue;

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
				message += 's';

			_toolStripStatusLabel.Text = message;
		}

		public ImageList SeverityImageList
		{
			get => _findingListView.SeverityImageList;
			set => _findingListView.SeverityImageList = value;
		}

		private void ExportButton_Click(object sender, EventArgs e)
		{
			if (_saveExportFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					CardDeckExporter.ExportInstructions(_saveExportFileDialog.FileName, _instances);
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
