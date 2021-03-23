using MixGui.Components;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Device;
using MixLib.Misc;
using MixLib.Modules;
using MixLib.Settings;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MixGui
{
	public partial class MixForm
	{
		void ExitMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			HandleException(e.Exception);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HandleException((Exception)e.ExceptionObject);
		}

		void RunItem_Click(object sender, EventArgs e)
		{
			SwitchRunningState();
		}

		void ShowPCButton_Click(object sender, EventArgs e)
		{
			mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter);
		}

		void TeletypeItem_Click(object sender, EventArgs e)
		{
			SwitchTeletypeVisibility();
		}

		void FloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args)
		{
			ImplementFloatingPointPCChange(args.SelectedAddress);
		}

		void DevicesControl_DeviceDoubleClick(object sender, DeviceEventArgs e)
		{
			if (e.Device is TeletypeDevice)
			{
				if (!mTeletype.Visible)
				{
					mTeletype.Show();
				}

				mTeletype.BringToFront();
				return;
			}

			mDeviceEditor.ShowDevice(e.Device);
			if (!mDeviceEditor.Visible)
			{
				mDeviceEditor.Show();
			}

			mDeviceEditor.BringToFront();
		}

		void ModeCycleButton_ValueChanged(object sender, EventArgs e)
		{
			int pc = mMix.ProgramCounter;
			mMix.Mode = (ModuleBase.RunMode)mModeCycleButton.Value;
			SetPCBoxBounds();

			if (pc != mMix.ProgramCounter)
			{
				if (mMainMemoryEditor.IsAddressVisible(pc))
				{
					mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter, mMix.Status == ModuleBase.RunStatus.Idle);
				}

				mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
				mPCBox.LongValue = mMix.ProgramCounter;
			}
		}

		void DeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			mDeviceEditorButton.Text = mDeviceEditor.Visible ? "Hide E&ditor" : "Show E&ditor";
			mDeviceEditorToolStripMenuItem.Text = mDeviceEditor.Visible ? "Hide &Device Editor" : "Show &Device Editor";
		}

		void AboutMenuItem_Click(object sender, EventArgs e)
		{
			if (mAboutForm == null)
			{
				mAboutForm = new AboutForm();
			}

			var teletypeWasOnTop = DisableTeletypeOnTop();

			mAboutForm.ShowDialog(this);

			mTeletype.TopMost |= teletypeWasOnTop;
		}
		void DetachItem_Click(object sender, EventArgs e)
		{
			mMix.RunDetached = !mMix.RunDetached;
			mDetachToolStripButton.Text = mMix.RunDetached ? "Attac&h" : "Detac&h";
			mDetachToolStripMenuItem.Text = mMix.RunDetached ? "&Attach" : "&Detach";
		}

		void ListViews_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (sender == mLogListView)
			{
				mMemoryTabControl.SelectedIndex = mLogListView.SelectedModule == mMix.FloatingPointModule.ModuleName ? floatingPointMemoryEditorIndex : mainMemoryEditorIndex;
			}
			ActiveMemoryEditor.MakeAddressVisible(args.SelectedAddress);
		}

		void MemoryEditor_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (!mReadOnly)
			{
				ImplementPCChange(args.SelectedAddress);
			}
		}

		void Mix_InputRequired(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_InputRequired));
			}
			else
			{
				mMix.RequestStop();
				mTeletype.StatusText = "Teletype input required";

				if (!mTeletype.Visible)
				{
					SwitchTeletypeVisibility();
				}

				mTeletype.Activate();
			}
		}

		void Mix_LogLineAdded(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_LogLineAdded));
			}
			else
			{
				LogLine line;
				while ((line = mMix.GetLogLine()) != null)
				{
					mLogListView.AddLogLine(line);
				}
			}
		}

		void Mix_StepPerformed(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(Mix_StepPerformed));
			}
			else
			{
				if (!mRunning)
				{
					mMix.RequestStop(false);
				}

				DateTime updateStartTime = DateTime.Now;
				Update();

				if (mMix.IsRunning)
				{
					mMix.ContinueRun(DateTime.Now - updateStartTime);
				}
				else
				{
					mRunning = false;
					SetSteppingState(SteppingState.Idle);
				}
			}
		}

		void OpenProgramMenuItem_Click(object sender, EventArgs e)
		{
			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (mOpenProgramFileDialog == null)
			{
				mOpenProgramFileDialog = new OpenFileDialog
				{
					AddExtension = false,
					Filter = "MIXAL program (*.mixal)|*.mixal|All files (*.*)|*.*",
					Title = "Select program to open"
				};
			}

			if (mOpenProgramFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				mLogListView.AddLogLine(new LogLine("Assembler", Severity.Info, "Parsing source", "File: " + mOpenProgramFileDialog.FileName));
				try
				{
					LoadProgram(mOpenProgramFileDialog.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Unable to open program file: " + ex.Message, "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}

			mTeletype.TopMost |= teletypeWasOnTop;
		}

		void PreferencesMenuItem_Click(object sender, EventArgs e)
		{
			if (mPreferencesForm == null)
			{
				mPreferencesForm = new PreferencesForm(mConfiguration, mMix.Devices, mDefaultDirectory);
			}

			var teletypeWasOnTop = DisableTeletypeOnTop();

			if (mPreferencesForm.ShowDialog(this) == DialogResult.OK)
			{
				GuiSettings.ColorProfilingCounts = mConfiguration.ColorProfilingCounts;
				UpdateLayout();
				UpdateDeviceConfig();
				mDeviceEditor.UpdateSettings();
				CardDeckExporter.LoaderCards = mConfiguration.LoaderCards;
			}


			mTeletype.TopMost |= teletypeWasOnTop;
		}

		void ResetItem_Click(object sender, EventArgs e)
		{
			mMemoryTabControl.SelectedIndex = 0;

			mMix.Reset();
			mSymbolListView.Reset();
			mMainMemoryEditor.ClearBreakpoints();
			mMainMemoryEditor.Symbols = null;
			LoadControlProgram();

			mMix.FloatingPointModule.FullMemory.Reset();
			ApplyFloatingPointSettings();
			if (mFloatingPointMemoryEditor != null)
			{
				mFloatingPointMemoryEditor.ClearBreakpoints();
				mFloatingPointMemoryEditor.Symbols = mMix.FloatingPointModule.Symbols;
			}

			Update();

			mMainMemoryEditor.ClearHistory();
		}

		void ResetTicksButton_Click(object sender, EventArgs e)
		{
			mMix.ResetTickCounter();
			mTicksToolStripTextBox.Text = mMix.TickCounter.ToString();
		}

		void StepItem_Click(object sender, EventArgs e)
		{
			SetSteppingState(SteppingState.Stepping);
			mMix.StartStep();
		}

		void Teletype_VisibleChanged(object sender, EventArgs e)
		{
			mTeletypeToolStripButton.Text = mTeletype.Visible ? "Hide &Teletype" : "Show &Teletype";
			mTeletypeToolStripMenuItem.Text = mTeletype.Visible ? "&Hide Teletype" : "&Show Teletype";
		}

		void TickItem_Click(object sender, EventArgs e)
		{
			mMix.Tick();
			Update();
		}

		void FormClosingHandler(object sender, FormClosingEventArgs e)
		{
			mConfiguration.MainWindowState = WindowState;
			mConfiguration.DeviceEditorWindowLocation = mDeviceEditor.Location;
			mConfiguration.DeviceEditorWindowSize = mDeviceEditor.Size;
			mConfiguration.DeviceEditorVisible = mDeviceEditor.Visible;
			mConfiguration.TeletypeOnTop = mTeletype.TopMost;
			mConfiguration.TeletypeWindowLocation = mTeletype.Location;
			mConfiguration.TeletypeWindowSize = mTeletype.Size;
			mConfiguration.TeletypeVisible = mTeletype.Visible;
			mConfiguration.TeletypeEchoInput = mTeletype.EchoInput;

			try
			{
				mConfiguration.Save(mDefaultDirectory);
			}
			catch (Exception)
			{
			}
		}

		void LocationChangedHandler(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				mConfiguration.MainWindowLocation = Location;
			}
		}

		void SizeChangedHandler(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				mConfiguration.MainWindowSize = Size;
			}
		}

		void ResizeBeginHandler(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.ResizeInProgress = true;
				}
			}
		}

		void ResizeEndHandler(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.ResizeInProgress = false;
				}
			}
		}

		void ClearBreakpointsItem_Click(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				editor?.ClearBreakpoints();
			}
		}

		void DeviceEditorItem_Click(object sender, EventArgs e)
		{
			if (mDeviceEditor.Visible)
			{
				mDeviceEditor.Hide();
			}
			else
			{
				mDeviceEditor.Show();
			}

		}

		void GoItem_Click(object sender, EventArgs e)
		{
			if (mSteppingState == SteppingState.Idle)
			{
				mMix.PrepareLoader();
				SwitchRunningState();
			}
		}

		void MemoryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPageIndex == floatingPointMemoryEditorIndex)
			{
				if (mFloatingPointMemoryEditor == null)
				{
					mFloatingPointMemoryEditor = new MemoryEditor(mMix.FloatingPointModule.FullMemory);

					mFloatingPointMemoryTab.SuspendLayout();
					mFloatingPointMemoryTab.Controls.Add(mFloatingPointMemoryEditor);

					mFloatingPointMemoryEditor.ToolTip = mToolTip;
					mFloatingPointMemoryEditor.Anchor = mMainMemoryEditor.Anchor;
					mFloatingPointMemoryEditor.MarkedAddress = mMix.FloatingPointModule.ProgramCounter;
					mFloatingPointMemoryEditor.FirstVisibleAddress = mFloatingPointMemoryEditor.MarkedAddress;
					mFloatingPointMemoryEditor.IndexedAddressCalculatorCallback = CalculateIndexedAddress;
					mFloatingPointMemoryEditor.Location = mMainMemoryEditor.Location;
					mFloatingPointMemoryEditor.Size = mFloatingPointMemoryTab.Size;
					mFloatingPointMemoryEditor.Name = "mFloatingPointMemoryEditor";
					mFloatingPointMemoryEditor.ReadOnly = mReadOnly;
					mFloatingPointMemoryEditor.ResizeInProgress = mMainMemoryEditor.ResizeInProgress;
					mFloatingPointMemoryEditor.Symbols = mMix.FloatingPointModule.Symbols;
					mFloatingPointMemoryEditor.TabIndex = 0;
					mFloatingPointMemoryEditor.AddressSelected += FloatingPointMemoryEditor_AddressSelected;

					mFloatingPointMemoryTab.ResumeLayout(true);

					mMemoryEditors[floatingPointMemoryEditorIndex] = mFloatingPointMemoryEditor;
					mMix.FloatingPointModule.BreakpointManager = mFloatingPointMemoryEditor;
				}

			}
		}

		void MemoryTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			MemoryEditor editor = ActiveMemoryEditor;
			mSymbolListView.Symbols = editor.Symbols;
			mSymbolListView.MemoryMinIndex = editor.Memory.MinWordIndex;
			mSymbolListView.MemoryMaxIndex = editor.Memory.MaxWordIndex;
		}

		void FindMenuItem_Click(object sender, EventArgs e)
		{
			if (mSearchDialog == null)
			{
				mSearchDialog = new SearchDialog();
			}

			if (mSearchDialog.ShowDialog(this) == DialogResult.OK)
			{
				mSearchOptions = mSearchDialog.SearchParameters;

				FindNext();
			}
		}

		void FindNextMenuItem_Click(object sender, EventArgs e)
		{
			if (mSearchOptions == null)
			{
				FindMenuItem_Click(sender, e);
				return;
			}

			FindNext();
		}

		void ProfilingEnabledMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ExecutionSettings.ProfilingEnabled = mProfilingEnabledMenuItem.Checked;
			mConfiguration.ProfilingEnabled = ExecutionSettings.ProfilingEnabled;

			mProfilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			mProfilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			foreach (MemoryEditor editor in mMemoryEditors)
			{
				editor?.UpdateLayout();
			}
		}

		void ProfilingShowTickCountsMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowProfilingInfo = mProfilingShowTickCountsMenuItem.Checked ? GuiSettings.ProfilingInfoType.Tick : GuiSettings.ProfilingInfoType.Execution;
			mConfiguration.ShowProfilingInfo = GuiSettings.ShowProfilingInfo;

			foreach (MemoryEditor editor in mMemoryEditors)
			{
				editor?.UpdateLayout();
			}
		}

		void ProfilingResetCountsMenuItem_Click(object sender, EventArgs e)
		{
			mMix.ResetProfilingCounts();
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				editor?.Update();
			}
		}
	}
}
