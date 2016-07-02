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
        void mExitMenuItem_Click(object sender, EventArgs e) => Close();

        static void application_ThreadException(object sender, ThreadExceptionEventArgs e) => handleException(e.Exception);

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
            handleException((Exception)e.ExceptionObject);

        void mRunItem_Click(object sender, EventArgs e) => switchRunningState();

        void mShowPCButton_Click(object sender, EventArgs e) => mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter);

        void mTeletypeItem_Click(object sender, EventArgs e) => switchTeletypeVisibility();

        void mFloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args) =>
            implementFloatingPointPCChange(args.SelectedAddress);

        void mDevicesControl_DeviceDoubleClick(object sender, DeviceEventArgs e)
        {
            if (e.Device is TeletypeDevice)
            {
                if (!mTeletype.Visible) mTeletype.Show();
                mTeletype.BringToFront();
                return;
            }

            mDeviceEditor.ShowDevice(e.Device);
            if (!mDeviceEditor.Visible) mDeviceEditor.Show();
            mDeviceEditor.BringToFront();
        }

        void mModeCycleButton_ValueChanged(object sender, EventArgs e)
        {
            int pc = mMix.ProgramCounter;
            mMix.Mode = (ModuleBase.RunMode)mModeCycleButton.Value;
            setPCBoxBounds();

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

        void mDeviceEditor_VisibleChanged(object sender, EventArgs e)
        {
            mDeviceEditorButton.Text = mDeviceEditor.Visible ? "Hide E&ditor" : "Show E&ditor";
            mDeviceEditorToolStripMenuItem.Text = mDeviceEditor.Visible ? "Hide &Device Editor" : "Show &Device Editor";
        }

        void mAboutMenuItem_Click(object sender, EventArgs e)
        {
            if (mAboutForm == null)
            {
                mAboutForm = new AboutForm();
            }

            bool teletypeWasOnTop = disableTeletypeOnTop();

            mAboutForm.ShowDialog(this);

            mTeletype.TopMost |= teletypeWasOnTop;
        }
        void mDetachItem_Click(object sender, EventArgs e)
        {
            mMix.RunDetached = !mMix.RunDetached;
            mDetachToolStripButton.Text = mMix.RunDetached ? "Attac&h" : "Detac&h";
            mDetachToolStripMenuItem.Text = mMix.RunDetached ? "&Attach" : "&Detach";
        }

        void listViews_addressSelected(object sender, AddressSelectedEventArgs args)
        {
            if (sender == mLogListView)
            {
                mMemoryTabControl.SelectedIndex = mLogListView.SelectedModule == mMix.FloatingPointModule.ModuleName ? floatingPointMemoryEditorIndex : mainMemoryEditorIndex;
            }
            activeMemoryEditor.MakeAddressVisible(args.SelectedAddress);
        }

        void mMemoryEditor_addressSelected(object sender, AddressSelectedEventArgs args)
        {
            if (!mReadOnly) implementPCChange(args.SelectedAddress);
        }

        void mMix_InputRequired(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(mMix_InputRequired));
            }
            else
            {
                mMix.RequestStop();
                mTeletype.StatusText = "Teletype input required";

                if (!mTeletype.Visible)
                {
                    switchTeletypeVisibility();
                }

                mTeletype.Activate();
            }
        }

        void mMix_LogLineAdded(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(mMix_LogLineAdded));
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

        void mMix_StepPerformed(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(mMix_StepPerformed));
            }
            else
            {
                if (!mRunning) mMix.RequestStop(false);

                DateTime updateStartTime = DateTime.Now;
                Update();

                if (mMix.IsRunning)
                {
                    mMix.ContinueRun(DateTime.Now - updateStartTime);
                }
                else
                {
                    mRunning = false;
                    setSteppingState(steppingState.Idle);
                }
            }
        }

        void mOpenProgramMenuItem_Click(object sender, EventArgs e)
        {
            bool teletypeWasOnTop = disableTeletypeOnTop();

            if (mOpenProgramFileDialog == null)
            {
                mOpenProgramFileDialog = new OpenFileDialog();
                mOpenProgramFileDialog.AddExtension = false;
                mOpenProgramFileDialog.Filter = "MIXAL program (*.mixal)|*.mixal|All files (*.*)|*.*";
                mOpenProgramFileDialog.Title = "Select program to open";
            }

            if (mOpenProgramFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                mLogListView.AddLogLine(new LogLine("Assembler", Severity.Info, "Parsing source", "File: " + mOpenProgramFileDialog.FileName));
                try
                {
                    loadProgram(mOpenProgramFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Unable to open program file: " + ex.Message, "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }

            mTeletype.TopMost |= teletypeWasOnTop;
        }

        void mPreferencesMenuItem_Click(object sender, EventArgs e)
        {
            if (mPreferencesForm == null)
            {
                mPreferencesForm = new PreferencesForm(mConfiguration, mMix.Devices, mDefaultDirectory);
            }

            bool teletypeWasOnTop = disableTeletypeOnTop();

            if (mPreferencesForm.ShowDialog(this) == DialogResult.OK)
            {
                GuiSettings.ColorProfilingCounts = mConfiguration.ColorProfilingCounts;
                UpdateLayout();
                updateDeviceConfig();
                mDeviceEditor.UpdateSettings();
                CardDeckExporter.LoaderCards = mConfiguration.LoaderCards;
            }


            mTeletype.TopMost |= teletypeWasOnTop;
        }

        void mResetItem_Click(object sender, EventArgs e)
        {
            mMemoryTabControl.SelectedIndex = 0;

            mMix.Reset();
            mSymbolListView.Reset();
            mMainMemoryEditor.ClearBreakpoints();
            mMainMemoryEditor.Symbols = null;
            loadControlProgram();

            mMix.FloatingPointModule.FullMemory.Reset();
            applyFloatingPointSettings();
            if (mFloatingPointMemoryEditor != null)
            {
                mFloatingPointMemoryEditor.ClearBreakpoints();
                mFloatingPointMemoryEditor.Symbols = mMix.FloatingPointModule.Symbols;
            }

            Update();

            mMainMemoryEditor.ClearHistory();
        }

        void mResetTicksButton_Click(object sender, EventArgs e)
        {
            mMix.ResetTickCounter();
            mTicksToolStripTextBox.Text = mMix.TickCounter.ToString();
        }

        void mStepItem_Click(object sender, EventArgs e)
        {
            setSteppingState(steppingState.Stepping);
            mMix.StartStep();
        }

        void mTeletype_VisibleChanged(object sender, EventArgs e)
        {
            mTeletypeToolStripButton.Text = mTeletype.Visible ? "Hide &Teletype" : "Show &Teletype";
            mTeletypeToolStripMenuItem.Text = mTeletype.Visible ? "&Hide Teletype" : "&Show Teletype";
        }

        void mTickItem_Click(object sender, EventArgs e)
        {
            mMix.Tick();
            Update();
        }

        void this_FormClosing(object sender, FormClosingEventArgs e)
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

        void this_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                mConfiguration.MainWindowLocation = Location;
            }
        }

        void this_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                mConfiguration.MainWindowSize = Size;
            }
        }

        void this_ResizeBegin(object sender, EventArgs e)
        {
            foreach (MemoryEditor editor in mMemoryEditors)
            {
                if (editor != null)
                {
                    editor.ResizeInProgress = true;
                }
            }
        }

        void MixForm_ResizeEnd(object sender, EventArgs e)
        {
            foreach (MemoryEditor editor in mMemoryEditors)
            {
                if (editor != null)
                {
                    editor.ResizeInProgress = false;
                }
            }
        }

        void mClearBreakpointsItem_Click(object sender, EventArgs e)
        {
            foreach (MemoryEditor editor in mMemoryEditors) editor?.ClearBreakpoints();
        }

        void mDeviceEditorItem_Click(object sender, EventArgs e)
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

        void mGoItem_Click(object sender, EventArgs e)
        {
            if (mSteppingState == steppingState.Idle)
            {
                mMix.PrepareLoader();
                switchRunningState();
            }
        }

        void mMemoryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
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
                    mFloatingPointMemoryEditor.IndexedAddressCalculatorCallback = calculateIndexedAddress;
                    mFloatingPointMemoryEditor.Location = mMainMemoryEditor.Location;
                    mFloatingPointMemoryEditor.Size = mFloatingPointMemoryTab.Size;
                    mFloatingPointMemoryEditor.Name = "mFloatingPointMemoryEditor";
                    mFloatingPointMemoryEditor.ReadOnly = mReadOnly;
                    mFloatingPointMemoryEditor.ResizeInProgress = mMainMemoryEditor.ResizeInProgress;
                    mFloatingPointMemoryEditor.Symbols = mMix.FloatingPointModule.Symbols;
                    mFloatingPointMemoryEditor.TabIndex = 0;
                    mFloatingPointMemoryEditor.AddressSelected += mFloatingPointMemoryEditor_AddressSelected;

                    mFloatingPointMemoryTab.ResumeLayout(true);

                    mMemoryEditors[floatingPointMemoryEditorIndex] = mFloatingPointMemoryEditor;
                    mMix.FloatingPointModule.BreakpointManager = mFloatingPointMemoryEditor;
                }

            }
        }

        void mMemoryTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            MemoryEditor editor = activeMemoryEditor;
            mSymbolListView.Symbols = editor.Symbols;
            mSymbolListView.MemoryMinIndex = editor.Memory.MinWordIndex;
            mSymbolListView.MemoryMaxIndex = editor.Memory.MaxWordIndex;
        }

        void mFindMenuItem_Click(object sender, EventArgs e)
        {
            if (mSearchDialog == null)
            {
                mSearchDialog = new SearchDialog();
            }

            if (mSearchDialog.ShowDialog(this) == DialogResult.OK)
            {
                mSearchOptions = mSearchDialog.SearchParameters;

                findNext();
            }
        }

        void mFindNextMenuItem_Click(object sender, EventArgs e)
        {
            if (mSearchOptions == null)
            {
                mFindMenuItem_Click(sender, e);
                return;
            }

            findNext();
        }

        void mProfilingEnabledMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ExecutionSettings.ProfilingEnabled = mProfilingEnabledMenuItem.Checked;
            mConfiguration.ProfilingEnabled = ExecutionSettings.ProfilingEnabled;

            mProfilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
            mProfilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

            foreach (MemoryEditor editor in mMemoryEditors) editor?.UpdateLayout();
        }

        void mProfilingShowTickCountsMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            GuiSettings.ShowProfilingInfo = mProfilingShowTickCountsMenuItem.Checked ? GuiSettings.ProfilingInfoType.Tick : GuiSettings.ProfilingInfoType.Execution;
            mConfiguration.ShowProfilingInfo = GuiSettings.ShowProfilingInfo;

            foreach (MemoryEditor editor in mMemoryEditors) editor?.UpdateLayout();
        }

        void mProfilingResetCountsMenuItem_Click(object sender, EventArgs e)
        {
            mMix.ResetProfilingCounts();
            foreach (MemoryEditor editor in mMemoryEditors) editor?.Update();
        }
    }
}
