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
		private Button closeButton;
		private AssemblyFindingListView findingListView;
		private Panel listPanel;
		private Button loadButton;
		private SourceCodeControl sourceControl;
		private Splitter splitter;
		private StatusStrip statusStrip;
		private Button exportButton;
		private SaveFileDialog saveExportFileDialog;
		private ToolStripStatusLabel toolStripStatusLabel;
		private InstructionInstanceBase[] instances;

		public SourceAndFindingsForm()
		{
			InitializeComponent();
			this.findingListView.SelectionChanged += FindingListView_SelectionChanged;
		}

		public void UpdateLayout() 
			=> this.sourceControl.UpdateLayout();

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceAndFindingsForm));
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.listPanel = new System.Windows.Forms.Panel();
			this.closeButton = new System.Windows.Forms.Button();
			this.exportButton = new System.Windows.Forms.Button();
			this.loadButton = new System.Windows.Forms.Button();
			this.findingListView = new MixGui.Components.AssemblyFindingListView();
			this.splitter = new System.Windows.Forms.Splitter();
			this.saveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.sourceControl = new MixGui.Components.SourceCodeControl();
			this.statusStrip.SuspendLayout();
			this.listPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// this.statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 383);
			this.statusStrip.Name = "_statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(568, 22);
			this.statusStrip.TabIndex = 3;
			// 
			// this.toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "_toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// this.listPanel
			// 
			this.listPanel.Controls.Add(this.closeButton);
			this.listPanel.Controls.Add(this.exportButton);
			this.listPanel.Controls.Add(this.loadButton);
			this.listPanel.Controls.Add(this.findingListView);
			this.listPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listPanel.Location = new System.Drawing.Point(0, 303);
			this.listPanel.Name = "_listPanel";
			this.listPanel.Size = new System.Drawing.Size(568, 80);
			this.listPanel.TabIndex = 2;
			// 
			// this.closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.closeButton.Location = new System.Drawing.Point(498, 56);
			this.closeButton.Name = "_closeButton";
			this.closeButton.Size = new System.Drawing.Size(62, 23);
			this.closeButton.TabIndex = 3;
			this.closeButton.Text = "&Close";
			// 
			// this.exportButton
			// 
			this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.exportButton.Location = new System.Drawing.Point(498, 28);
			this.exportButton.Name = "_exportButton";
			this.exportButton.Size = new System.Drawing.Size(62, 23);
			this.exportButton.TabIndex = 2;
			this.exportButton.Text = "&Export...";
			// 
			// this.loadButton
			// 
			this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.loadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.loadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.loadButton.Location = new System.Drawing.Point(498, 0);
			this.loadButton.Name = "_loadButton";
			this.loadButton.Size = new System.Drawing.Size(62, 23);
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			// 
			// this.findingListView
			// 
			this.findingListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.findingListView.Location = new System.Drawing.Point(0, 0);
			this.findingListView.Name = "_findingListView";
			this.findingListView.SeverityImageList = null;
			this.findingListView.Size = new System.Drawing.Size(492, 80);
			this.findingListView.TabIndex = 0;
			// 
			// this.splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter.Location = new System.Drawing.Point(0, 295);
			this.splitter.MinExtra = 100;
			this.splitter.MinSize = 80;
			this.splitter.Name = "_splitter";
			this.splitter.Size = new System.Drawing.Size(568, 8);
			this.splitter.TabIndex = 1;
			this.splitter.TabStop = false;
			// 
			// this.saveExportFileDialog
			// 
			this.saveExportFileDialog.DefaultExt = "mixdeck";
			this.saveExportFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			this.saveExportFileDialog.Title = "Specify export file name";
			// 
			// this.sourceControl
			// 
			this.sourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sourceControl.Location = new System.Drawing.Point(0, 0);
			this.sourceControl.MarkedFinding = null;
			this.sourceControl.Name = "_sourceControl";
			this.sourceControl.Size = new System.Drawing.Size(568, 295);
			this.sourceControl.TabIndex = 0;
			// 
			// SourceAndFindingsForm
			// 
			this.ClientSize = new System.Drawing.Size(568, 405);
			this.Controls.Add(this.sourceControl);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.listPanel);
			this.Controls.Add(this.statusStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 224);
			this.Name = "SourceAndFindingsForm";
			this.ShowInTaskbar = false;
			this.Text = "Assembly result";
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.listPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void FindingListView_SelectionChanged(AssemblyFindingListView sender, AssemblyFindingListView.SelectionChangedEventArgs args) 
			=> this.sourceControl.MarkedFinding = args.SelectedFinding;

		public void SetInstructionsAndFindings(PreInstruction[] instructions, InstructionInstanceBase[] instances, AssemblyFindingCollection findings)
		{
			this.sourceControl.Instructions = instructions;
			this.sourceControl.Findings = findings;
			this.findingListView.Findings = findings;
			SetStatusBarText(findings);
			this.instances = instances;
			this.loadButton.Enabled = !findings.ContainsErrors;
			this.exportButton.Enabled = this.loadButton.Enabled;
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

			this.toolStripStatusLabel.Text = message;
		}

		public ImageList SeverityImageList
		{
			get => this.findingListView.SeverityImageList;
			set => this.findingListView.SeverityImageList = value;
		}

		private void ExportButton_Click(object sender, EventArgs e)
		{
			if (this.saveExportFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					CardDeckExporter.ExportInstructions(this.saveExportFileDialog.FileName, this.instances);
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
