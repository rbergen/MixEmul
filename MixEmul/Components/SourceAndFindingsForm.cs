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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceAndFindingsForm));
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._listPanel = new System.Windows.Forms.Panel();
			this._closeButton = new System.Windows.Forms.Button();
			this._exportButton = new System.Windows.Forms.Button();
			this._loadButton = new System.Windows.Forms.Button();
			this._findingListView = new MixGui.Components.AssemblyFindingListView();
			this._splitter = new System.Windows.Forms.Splitter();
			this._saveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this._sourceControl = new MixGui.Components.SourceCodeControl();
			this._statusStrip.SuspendLayout();
			this._listPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripStatusLabel});
			this._statusStrip.Location = new System.Drawing.Point(0, 383);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(568, 22);
			this._statusStrip.TabIndex = 3;
			// 
			// _toolStripStatusLabel
			// 
			this._toolStripStatusLabel.Name = "_toolStripStatusLabel";
			this._toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// _listPanel
			// 
			this._listPanel.Controls.Add(this._closeButton);
			this._listPanel.Controls.Add(this._exportButton);
			this._listPanel.Controls.Add(this._loadButton);
			this._listPanel.Controls.Add(this._findingListView);
			this._listPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._listPanel.Location = new System.Drawing.Point(0, 303);
			this._listPanel.Name = "_listPanel";
			this._listPanel.Size = new System.Drawing.Size(568, 80);
			this._listPanel.TabIndex = 2;
			// 
			// _closeButton
			// 
			this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._closeButton.Location = new System.Drawing.Point(498, 56);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(62, 23);
			this._closeButton.TabIndex = 3;
			this._closeButton.Text = "&Close";
			// 
			// _exportButton
			// 
			this._exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._exportButton.Location = new System.Drawing.Point(498, 28);
			this._exportButton.Name = "_exportButton";
			this._exportButton.Size = new System.Drawing.Size(62, 23);
			this._exportButton.TabIndex = 2;
			this._exportButton.Text = "&Export...";
			// 
			// _loadButton
			// 
			this._loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._loadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._loadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._loadButton.Location = new System.Drawing.Point(498, 0);
			this._loadButton.Name = "_loadButton";
			this._loadButton.Size = new System.Drawing.Size(62, 23);
			this._loadButton.TabIndex = 1;
			this._loadButton.Text = "&Load";
			// 
			// _findingListView
			// 
			this._findingListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._findingListView.Location = new System.Drawing.Point(0, 0);
			this._findingListView.Name = "_findingListView";
			this._findingListView.SeverityImageList = null;
			this._findingListView.Size = new System.Drawing.Size(492, 80);
			this._findingListView.TabIndex = 0;
			// 
			// _splitter
			// 
			this._splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._splitter.Location = new System.Drawing.Point(0, 295);
			this._splitter.MinExtra = 100;
			this._splitter.MinSize = 80;
			this._splitter.Name = "_splitter";
			this._splitter.Size = new System.Drawing.Size(568, 8);
			this._splitter.TabIndex = 1;
			this._splitter.TabStop = false;
			// 
			// _saveExportFileDialog
			// 
			this._saveExportFileDialog.DefaultExt = "mixdeck";
			this._saveExportFileDialog.Filter = "MixEmul card deck files|*.mixdeck|All files|*.*";
			this._saveExportFileDialog.Title = "Specify export file name";
			// 
			// _sourceControl
			// 
			this._sourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._sourceControl.Location = new System.Drawing.Point(0, 0);
			this._sourceControl.MarkedFinding = null;
			this._sourceControl.Name = "_sourceControl";
			this._sourceControl.Size = new System.Drawing.Size(568, 295);
			this._sourceControl.TabIndex = 0;
			// 
			// SourceAndFindingsForm
			// 
			this.ClientSize = new System.Drawing.Size(568, 405);
			this.Controls.Add(this._sourceControl);
			this.Controls.Add(this._splitter);
			this.Controls.Add(this._listPanel);
			this.Controls.Add(this._statusStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 224);
			this.Name = "SourceAndFindingsForm";
			this.ShowInTaskbar = false;
			this.Text = "Assembly result";
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this._listPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

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
