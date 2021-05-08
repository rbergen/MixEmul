using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MixGui.Components;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Device;
using MixLib.Misc;
using MixLib.Modules;
using MixLib.Settings;

namespace MixGui
{
	public partial class MixForm
	{
		private void ExitMenuItem_Click(object sender, EventArgs e) 
			=> Close();

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) 
			=> HandleException(e.Exception);

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) 
			=> HandleException((Exception)e.ExceptionObject);

		private void RunItem_Click(object sender, EventArgs e) 
			=> SwitchRunningState();

		private void ShowPCButton_Click(object sender, EventArgs e) 
			=> this.mainMemoryEditor.MakeAddressVisible(this.mix.ProgramCounter);

		private void TeletypeItem_Click(object sender, EventArgs e) 
			=> SwitchTeletypeVisibility();

		private void FloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args) 
			=> ImplementFloatingPointPCChange(args.SelectedAddress);

		private void DevicesControl_DeviceDoubleClick(object sender, DeviceEventArgs e)
		{
			if (e.Device is TeletypeDevice)
			{
				if (!this.teletype.Visible)
					this.teletype.Show();

				this.teletype.BringToFront();
				return;
			}

			this.deviceEditor.ShowDevice(e.Device);
			if (!this.deviceEditor.Visible)
				this.deviceEditor.Show();

			this.deviceEditor.BringToFront();
		}

		private void ModeCycleButton_ValueChanged(object sender, EventArgs e)
		{
			int pc = this.mix.ProgramCounter;
			this.mix.Mode = (ModuleBase.RunMode)this.modeCycleButton.Value;
			SetPCBoxBounds();

			if (pc != this.mix.ProgramCounter)
			{
				if (this.mainMemoryEditor.IsAddressVisible(pc))
					this.mainMemoryEditor.MakeAddressVisible(this.mix.ProgramCounter, this.mix.Status == ModuleBase.RunStatus.Idle);

				this.mainMemoryEditor.MarkedAddress = this.mix.ProgramCounter;
				this.pcBox.LongValue = this.mix.ProgramCounter;
			}
		}

		private void DeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			this.deviceEditorButton.Text = this.deviceEditor.Visible ? "Hide E&ditor" : "Show E&ditor";
			this.deviceEditorToolStripMenuItem.Text = this.deviceEditor.Visible ? "Hide &Device Editor" : "Show &Device Editor";
		}

		private void AboutMenuItem_Click(object sender, EventArgs e)
		{
			if (this.aboutForm == null)
				this.aboutForm = new AboutForm();

			var teletypeWasOnTop = DisableTeletypeOnTop();

			this.aboutForm.ShowDialog(this);

			this.teletype.TopMost |= teletypeWasOnTop;
		}

		private void DetachItem_Click(object sender, EventArgs e)
		{
			this.mix.RunDetached = !this.mix.RunDetached;
			this.detachToolStripButton.Text = this.mix.RunDetached ? "Attac&h" : "Detac&h";
			this.detachToolStripMenuItem.Text = this.mix.RunDetached ? "&Attach" : "&Detach";
		}

		private void ListViews_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (sender == this.logListView)
				this.memoryTabControl.SelectedIndex = this.logListView.SelectedModule == this.mix.FloatingPointModule.ModuleName ? FloatingPointMemoryEditorIndex : MainMemoryEditorIndex;

			ActiveMemoryEditor.MakeAddressVisible(args.SelectedAddress);
		}

		private void MemoryEditor_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (!this.readOnly)
				ImplementPCChange(args.SelectedAddress);
		}

		private void Mix_InputRequired(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_InputRequired));
				return;
			}

			this.mix.RequestStop();
			this.teletype.StatusText = "Teletype input required";

			if (!this.teletype.Visible)
				SwitchTeletypeVisibility();

			this.teletype.Activate();
		}

		private void Mix_LogLineAdded(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_LogLineAdded));
				return;
			}

			LogLine line;
			while ((line = this.mix.GetLogLine()) != null)
				this.logListView.AddLogLine(line);
		}

		private void Mix_StepPerformed(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_StepPerformed));
				return;
			}

			if (!this.running)
				this.mix.RequestStop(false);

			DateTime updateStartTime = DateTime.Now;
			Update();

			if (this.mix.IsRunning)
				this.mix.ContinueRun(DateTime.Now - updateStartTime);

			else
			{
				this.running = false;
				SetSteppingState(SteppingState.Idle);
			}
		}

		private void OpenProgramMenuItem_Click(object sender, EventArgs e)
		{
			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (this.openProgramFileDialog == null)
			{
				this.openProgramFileDialog = new OpenFileDialog
				{
					AddExtension = false,
					Filter = "MIXAL program (*.mixal)|*.mixal|All files (*.*)|*.*",
					Title = "Select program to open"
				};
			}

			if (this.openProgramFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.logListView.AddLogLine(new LogLine("Assembler", Severity.Info, "Parsing source", "File: " + this.openProgramFileDialog.FileName));
				try
				{
					LoadProgram(this.openProgramFileDialog.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to open program file: " + ex.Message, "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}

			this.teletype.TopMost |= teletypeWasOnTop;
		}

		private void PreferencesMenuItem_Click(object sender, EventArgs e)
		{
			if (this.preferencesForm == null)
				this.preferencesForm = new PreferencesForm(this.configuration, this.mix.Devices, this.defaultDirectory);

			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (this.preferencesForm.ShowDialog(this) == DialogResult.OK)
			{
				GuiSettings.ColorProfilingCounts = this.configuration.ColorProfilingCounts;
				UpdateLayout();
				UpdateDeviceConfig();
				this.deviceEditor.UpdateSettings();
				CardDeckExporter.LoaderCards = this.configuration.LoaderCards;
			}

			this.teletype.TopMost |= teletypeWasOnTop;
		}

		private void ResetItem_Click(object sender, EventArgs e)
		{
			this.memoryTabControl.SelectedIndex = 0;

			this.mix.Reset();
			this.symbolListView.Reset();
			this.mainMemoryEditor.ClearBreakpoints();
			this.mainMemoryEditor.Symbols = null;
			LoadControlProgram();

			this.mix.FloatingPointModule.FullMemory.Reset();
			ApplyFloatingPointSettings();

			if (this.floatingPointMemoryEditor != null)
			{
				this.floatingPointMemoryEditor.ClearBreakpoints();
				this.floatingPointMemoryEditor.Symbols = this.mix.FloatingPointModule.Symbols;
			}

			Update();

			this.mainMemoryEditor.ClearHistory();
		}

		private void ResetTicksButton_Click(object sender, EventArgs e)
		{
			this.mix.ResetTickCounter();
			this.ticksToolStripTextBox.Text = this.mix.TickCounter.ToString();
		}

		private void StepItem_Click(object sender, EventArgs e)
		{
			SetSteppingState(SteppingState.Stepping);
			this.mix.StartStep();
		}

		private void Teletype_VisibleChanged(object sender, EventArgs e)
		{
			this.teletypeToolStripButton.Text = this.teletype.Visible ? "Hide &Teletype" : "Show &Teletype";
			this.teletypeToolStripMenuItem.Text = this.teletype.Visible ? "&Hide Teletype" : "&Show Teletype";
		}

		private void TickItem_Click(object sender, EventArgs e)
		{
			this.mix.Tick();
			Update();
		}

		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.configuration.MainWindowState = WindowState;
			this.configuration.DeviceEditorWindowLocation = this.deviceEditor.Location;
			this.configuration.DeviceEditorWindowSize = this.deviceEditor.Size;
			this.configuration.DeviceEditorVisible = this.deviceEditor.Visible;
			this.configuration.TeletypeOnTop = this.teletype.TopMost;
			this.configuration.TeletypeWindowLocation = this.teletype.Location;
			this.configuration.TeletypeWindowSize = this.teletype.Size;
			this.configuration.TeletypeVisible = this.teletype.Visible;
			this.configuration.TeletypeEchoInput = this.teletype.EchoInput;

			try
			{
				this.configuration.Save(this.defaultDirectory);
			}
			catch (Exception) { }
		}

		private void This_LocationChanged(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
				this.configuration.MainWindowLocation = Location;
		}

		private void This_SizeChanged(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
				this.configuration.MainWindowSize = Size;
		}

		private void This_ResizeBegin(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in this.memoryEditors.Where(editor => editor != null))
				editor.ResizeInProgress = true;
		}

		private void This_ResizeEnd(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in this.memoryEditors.Where(editor => editor != null))
				editor.ResizeInProgress = false;
		}

		private void ClearBreakpointsItem_Click(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.ClearBreakpoints();
		}

		private void DeviceEditorItem_Click(object sender, EventArgs e)
		{
			if (this.deviceEditor.Visible)
				this.deviceEditor.Hide();

			else
				this.deviceEditor.Show();
		}

		private void GoItem_Click(object sender, EventArgs e)
		{
			if (this.steppingState == SteppingState.Idle)
			{
				this.mix.PrepareLoader();
				SwitchRunningState();
			}
		}

		private void MemoryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPageIndex != FloatingPointMemoryEditorIndex)
				return;

			if (this.floatingPointMemoryEditor != null)
				return;

			this.floatingPointMemoryEditor = new MemoryEditor(this.mix.FloatingPointModule.FullMemory);

			this.floatingPointMemoryTab.SuspendLayout();
			this.floatingPointMemoryTab.Controls.Add(this.floatingPointMemoryEditor);

			this.floatingPointMemoryEditor.ToolTip = this.toolTip;
			this.floatingPointMemoryEditor.Anchor = this.mainMemoryEditor.Anchor;
			this.floatingPointMemoryEditor.MarkedAddress = this.mix.FloatingPointModule.ProgramCounter;
			this.floatingPointMemoryEditor.FirstVisibleAddress = this.floatingPointMemoryEditor.MarkedAddress;
			this.floatingPointMemoryEditor.IndexedAddressCalculatorCallback = CalculateIndexedAddress;
			this.floatingPointMemoryEditor.Location = this.mainMemoryEditor.Location;
			this.floatingPointMemoryEditor.Size = this.floatingPointMemoryTab.Size;
			this.floatingPointMemoryEditor.Name = "mFloatingPointMemoryEditor";
			this.floatingPointMemoryEditor.ReadOnly = this.readOnly;
			this.floatingPointMemoryEditor.ResizeInProgress = this.mainMemoryEditor.ResizeInProgress;
			this.floatingPointMemoryEditor.Symbols = this.mix.FloatingPointModule.Symbols;
			this.floatingPointMemoryEditor.TabIndex = 0;
			this.floatingPointMemoryEditor.AddressSelected += FloatingPointMemoryEditor_AddressSelected;

			this.floatingPointMemoryTab.ResumeLayout(true);

			this.memoryEditors[FloatingPointMemoryEditorIndex] = this.floatingPointMemoryEditor;
			this.mix.FloatingPointModule.BreakpointManager = this.floatingPointMemoryEditor;
		}

		private void MemoryTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			MemoryEditor editor = ActiveMemoryEditor;
			this.symbolListView.Symbols = editor.Symbols;
			this.symbolListView.MemoryMinIndex = editor.Memory.MinWordIndex;
			this.symbolListView.MemoryMaxIndex = editor.Memory.MaxWordIndex;
		}

		private void FindMenuItem_Click(object sender, EventArgs e)
		{
			if (this.searchDialog == null)
				this.searchDialog = new SearchDialog();

			if (this.searchDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.searchOptions = this.searchDialog.SearchParameters;

				FindNext();
			}
		}

		private void FindNextMenuItem_Click(object sender, EventArgs e)
		{
			if (this.searchOptions == null)
			{
				FindMenuItem_Click(sender, e);
				return;
			}

			FindNext();
		}

		private void ProfilingEnabledMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ExecutionSettings.ProfilingEnabled = this.profilingEnabledMenuItem.Checked;
			this.configuration.ProfilingEnabled = ExecutionSettings.ProfilingEnabled;

			this.profilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			this.profilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.UpdateLayout();
		}

		private void ProfilingShowTickCountsMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowProfilingInfo = this.profilingShowTickCountsMenuItem.Checked ? GuiSettings.ProfilingInfoType.Tick : GuiSettings.ProfilingInfoType.Execution;
			this.configuration.ShowProfilingInfo = GuiSettings.ShowProfilingInfo;

			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.UpdateLayout();
		}

		private void ProfilingResetCountsMenuItem_Click(object sender, EventArgs e)
		{
			this.mix.ResetProfilingCounts();
			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.Update();
		}
		private void ShowSourceInlineMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowSourceInline = this.showSourceInlineMenuItem.Checked;
			this.configuration.ShowSourceInline = GuiSettings.ShowSourceInline;

			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.UpdateLayout();
		}


	}
}
