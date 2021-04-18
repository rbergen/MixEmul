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
			=> _mainMemoryEditor.MakeAddressVisible(_mix.ProgramCounter);

		private void TeletypeItem_Click(object sender, EventArgs e) 
			=> SwitchTeletypeVisibility();

		private void FloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args) 
			=> ImplementFloatingPointPCChange(args.SelectedAddress);

		private void DevicesControl_DeviceDoubleClick(object sender, DeviceEventArgs e)
		{
			if (e.Device is TeletypeDevice)
			{
				if (!_teletype.Visible)
					_teletype.Show();

				_teletype.BringToFront();
				return;
			}

			_deviceEditor.ShowDevice(e.Device);
			if (!_deviceEditor.Visible)
				_deviceEditor.Show();

			_deviceEditor.BringToFront();
		}

		private void ModeCycleButton_ValueChanged(object sender, EventArgs e)
		{
			int pc = _mix.ProgramCounter;
			_mix.Mode = (ModuleBase.RunMode)_modeCycleButton.Value;
			SetPCBoxBounds();

			if (pc != _mix.ProgramCounter)
			{
				if (_mainMemoryEditor.IsAddressVisible(pc))
					_mainMemoryEditor.MakeAddressVisible(_mix.ProgramCounter, _mix.Status == ModuleBase.RunStatus.Idle);

				_mainMemoryEditor.MarkedAddress = _mix.ProgramCounter;
				_pcBox.LongValue = _mix.ProgramCounter;
			}
		}

		private void DeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			_deviceEditorButton.Text = _deviceEditor.Visible ? "Hide E&ditor" : "Show E&ditor";
			_deviceEditorToolStripMenuItem.Text = _deviceEditor.Visible ? "Hide &Device Editor" : "Show &Device Editor";
		}

		private void AboutMenuItem_Click(object sender, EventArgs e)
		{
			if (_aboutForm == null)
				_aboutForm = new AboutForm();

			var teletypeWasOnTop = DisableTeletypeOnTop();

			_aboutForm.ShowDialog(this);

			_teletype.TopMost |= teletypeWasOnTop;
		}

		private void DetachItem_Click(object sender, EventArgs e)
		{
			_mix.RunDetached = !_mix.RunDetached;
			_detachToolStripButton.Text = _mix.RunDetached ? "Attac&h" : "Detac&h";
			_detachToolStripMenuItem.Text = _mix.RunDetached ? "&Attach" : "&Detach";
		}

		private void ListViews_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (sender == _logListView)
				_memoryTabControl.SelectedIndex = _logListView.SelectedModule == _mix.FloatingPointModule.ModuleName ? FloatingPointMemoryEditorIndex : MainMemoryEditorIndex;

			ActiveMemoryEditor.MakeAddressVisible(args.SelectedAddress);
		}

		private void MemoryEditor_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (!_readOnly)
				ImplementPCChange(args.SelectedAddress);
		}

		private void Mix_InputRequired(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_InputRequired));
				return;
			}

			_mix.RequestStop();
			_teletype.StatusText = "Teletype input required";

			if (!_teletype.Visible)
				SwitchTeletypeVisibility();

			_teletype.Activate();
		}

		private void Mix_LogLineAdded(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_LogLineAdded));
				return;
			}

			LogLine line;
			while ((line = _mix.GetLogLine()) != null)
				_logListView.AddLogLine(line);
		}

		private void Mix_StepPerformed(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_StepPerformed));
				return;
			}

			if (!_running)
				_mix.RequestStop(false);

			DateTime updateStartTime = DateTime.Now;
			Update();

			if (_mix.IsRunning)
				_mix.ContinueRun(DateTime.Now - updateStartTime);

			else
			{
				_running = false;
				SetSteppingState(SteppingState.Idle);
			}
		}

		private void OpenProgramMenuItem_Click(object sender, EventArgs e)
		{
			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (_openProgramFileDialog == null)
			{
				_openProgramFileDialog = new OpenFileDialog
				{
					AddExtension = false,
					Filter = "MIXAL program (*.mixal)|*.mixal|All files (*.*)|*.*",
					Title = "Select program to open"
				};
			}

			if (_openProgramFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				_logListView.AddLogLine(new LogLine("Assembler", Severity.Info, "Parsing source", "File: " + _openProgramFileDialog.FileName));
				try
				{
					LoadProgram(_openProgramFileDialog.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to open program file: " + ex.Message, "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}

			_teletype.TopMost |= teletypeWasOnTop;
		}

		private void PreferencesMenuItem_Click(object sender, EventArgs e)
		{
			if (_preferencesForm == null)
				_preferencesForm = new PreferencesForm(_configuration, _mix.Devices, _defaultDirectory);

			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (_preferencesForm.ShowDialog(this) == DialogResult.OK)
			{
				GuiSettings.ColorProfilingCounts = _configuration.ColorProfilingCounts;
				UpdateLayout();
				UpdateDeviceConfig();
				_deviceEditor.UpdateSettings();
				CardDeckExporter.LoaderCards = _configuration.LoaderCards;
			}

			_teletype.TopMost |= teletypeWasOnTop;
		}

		private void ResetItem_Click(object sender, EventArgs e)
		{
			_memoryTabControl.SelectedIndex = 0;

			_mix.Reset();
			_symbolListView.Reset();
			_mainMemoryEditor.ClearBreakpoints();
			_mainMemoryEditor.Symbols = null;
			LoadControlProgram();

			_mix.FloatingPointModule.FullMemory.Reset();
			ApplyFloatingPointSettings();

			if (_floatingPointMemoryEditor != null)
			{
				_floatingPointMemoryEditor.ClearBreakpoints();
				_floatingPointMemoryEditor.Symbols = _mix.FloatingPointModule.Symbols;
			}

			Update();

			_mainMemoryEditor.ClearHistory();
		}

		private void ResetTicksButton_Click(object sender, EventArgs e)
		{
			_mix.ResetTickCounter();
			_ticksToolStripTextBox.Text = _mix.TickCounter.ToString();
		}

		private void StepItem_Click(object sender, EventArgs e)
		{
			SetSteppingState(SteppingState.Stepping);
			_mix.StartStep();
		}

		private void Teletype_VisibleChanged(object sender, EventArgs e)
		{
			_teletypeToolStripButton.Text = _teletype.Visible ? "Hide &Teletype" : "Show &Teletype";
			_teletypeToolStripMenuItem.Text = _teletype.Visible ? "&Hide Teletype" : "&Show Teletype";
		}

		private void TickItem_Click(object sender, EventArgs e)
		{
			_mix.Tick();
			Update();
		}

		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			_configuration.MainWindowState = WindowState;
			_configuration.DeviceEditorWindowLocation = _deviceEditor.Location;
			_configuration.DeviceEditorWindowSize = _deviceEditor.Size;
			_configuration.DeviceEditorVisible = _deviceEditor.Visible;
			_configuration.TeletypeOnTop = _teletype.TopMost;
			_configuration.TeletypeWindowLocation = _teletype.Location;
			_configuration.TeletypeWindowSize = _teletype.Size;
			_configuration.TeletypeVisible = _teletype.Visible;
			_configuration.TeletypeEchoInput = _teletype.EchoInput;

			try
			{
				_configuration.Save(_defaultDirectory);
			}
			catch (Exception) { }
		}

		private void This_LocationChanged(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
				_configuration.MainWindowLocation = Location;
		}

		private void This_SizeChanged(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
				_configuration.MainWindowSize = Size;
		}

		private void This_ResizeBegin(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in _memoryEditors.Where(editor => editor != null))
				editor.ResizeInProgress = true;
		}

		private void This_ResizeEnd(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in _memoryEditors.Where(editor => editor != null))
				editor.ResizeInProgress = false;
		}

		private void ClearBreakpointsItem_Click(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in _memoryEditors)
				editor?.ClearBreakpoints();
		}

		private void DeviceEditorItem_Click(object sender, EventArgs e)
		{
			if (_deviceEditor.Visible)
				_deviceEditor.Hide();

			else
				_deviceEditor.Show();
		}

		private void GoItem_Click(object sender, EventArgs e)
		{
			if (_steppingState == SteppingState.Idle)
			{
				_mix.PrepareLoader();
				SwitchRunningState();
			}
		}

		private void MemoryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPageIndex != FloatingPointMemoryEditorIndex)
				return;

			if (_floatingPointMemoryEditor != null)
				return;

			_floatingPointMemoryEditor = new MemoryEditor(_mix.FloatingPointModule.FullMemory);

			_floatingPointMemoryTab.SuspendLayout();
			_floatingPointMemoryTab.Controls.Add(_floatingPointMemoryEditor);

			_floatingPointMemoryEditor.ToolTip = _toolTip;
			_floatingPointMemoryEditor.Anchor = _mainMemoryEditor.Anchor;
			_floatingPointMemoryEditor.MarkedAddress = _mix.FloatingPointModule.ProgramCounter;
			_floatingPointMemoryEditor.FirstVisibleAddress = _floatingPointMemoryEditor.MarkedAddress;
			_floatingPointMemoryEditor.IndexedAddressCalculatorCallback = CalculateIndexedAddress;
			_floatingPointMemoryEditor.Location = _mainMemoryEditor.Location;
			_floatingPointMemoryEditor.Size = _floatingPointMemoryTab.Size;
			_floatingPointMemoryEditor.Name = "mFloatingPointMemoryEditor";
			_floatingPointMemoryEditor.ReadOnly = _readOnly;
			_floatingPointMemoryEditor.ResizeInProgress = _mainMemoryEditor.ResizeInProgress;
			_floatingPointMemoryEditor.Symbols = _mix.FloatingPointModule.Symbols;
			_floatingPointMemoryEditor.TabIndex = 0;
			_floatingPointMemoryEditor.AddressSelected += FloatingPointMemoryEditor_AddressSelected;

			_floatingPointMemoryTab.ResumeLayout(true);

			_memoryEditors[FloatingPointMemoryEditorIndex] = _floatingPointMemoryEditor;
			_mix.FloatingPointModule.BreakpointManager = _floatingPointMemoryEditor;
		}

		private void MemoryTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			MemoryEditor editor = ActiveMemoryEditor;
			_symbolListView.Symbols = editor.Symbols;
			_symbolListView.MemoryMinIndex = editor.Memory.MinWordIndex;
			_symbolListView.MemoryMaxIndex = editor.Memory.MaxWordIndex;
		}

		private void FindMenuItem_Click(object sender, EventArgs e)
		{
			if (_searchDialog == null)
				_searchDialog = new SearchDialog();

			if (_searchDialog.ShowDialog(this) == DialogResult.OK)
			{
				_searchOptions = _searchDialog.SearchParameters;

				FindNext();
			}
		}

		private void FindNextMenuItem_Click(object sender, EventArgs e)
		{
			if (_searchOptions == null)
			{
				FindMenuItem_Click(sender, e);
				return;
			}

			FindNext();
		}

		private void ProfilingEnabledMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ExecutionSettings.ProfilingEnabled = _profilingEnabledMenuItem.Checked;
			_configuration.ProfilingEnabled = ExecutionSettings.ProfilingEnabled;

			_profilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			_profilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			foreach (MemoryEditor editor in _memoryEditors)
				editor?.UpdateLayout();
		}

		private void ProfilingShowTickCountsMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowProfilingInfo = _profilingShowTickCountsMenuItem.Checked ? GuiSettings.ProfilingInfoType.Tick : GuiSettings.ProfilingInfoType.Execution;
			_configuration.ShowProfilingInfo = GuiSettings.ShowProfilingInfo;

			foreach (MemoryEditor editor in _memoryEditors)
				editor?.UpdateLayout();
		}

		private void ProfilingResetCountsMenuItem_Click(object sender, EventArgs e)
		{
			_mix.ResetProfilingCounts();
			foreach (MemoryEditor editor in _memoryEditors)
				editor?.Update();
		}
		private void ShowSourceInlineMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowSourceInline = _showSourceInlineMenuItem.Checked;
			_configuration.ShowSourceInline = GuiSettings.ShowSourceInline;

			foreach (MemoryEditor editor in _memoryEditors)
				editor?.UpdateLayout();
		}


	}
}
