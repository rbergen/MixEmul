using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MixAssembler;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Components;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Device;
using MixLib.Device.Settings;
using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Modules;
using MixLib.Modules.Settings;
using MixLib.Settings;
using MixLib.Type;

namespace MixGui
{
    public partial class MixForm : Form
	{
        const int mainMemoryEditorIndex = 0;
        const int floatingPointMemoryEditorIndex = 1;

        IContainer components;
        AboutForm mAboutForm;
        Configuration mConfiguration;
        string mDefaultDirectory = Environment.CurrentDirectory;
        readonly Mix mMix;
        OpenFileDialog mOpenProgramFileDialog;
        PreferencesForm mPreferencesForm;
        bool mReadOnly;
        bool mRunning;
        bool mUpdating;
        ImageList mSeverityImageList;
        SourceAndFindingsForm mSourceAndFindingsForm;
        steppingState mSteppingState;
        TeletypeForm mTeletype;
        MenuStrip mMainMenuStrip;
        ToolStripMenuItem mFileToolStripMenuItem;
        ToolStripMenuItem mOpenProgramToolStripMenuItem;
        ToolStripMenuItem mExitToolStripMenuItem;
        ToolStripMenuItem mToolsToolStripMenuItem;
        ToolStripMenuItem mPreferencesToolStripMenuItem;
        ToolStripMenuItem mHelpToolStripMenuItem;
        ToolStripMenuItem mAboutToolStripMenuItem;
        ToolStripMenuItem mActionsToolStripMenuItem;
        ToolStripMenuItem mTickToolStripMenuItem;
        ToolStripMenuItem mStepToolStripMenuItem;
        ToolStripMenuItem mRunToolStripMenuItem;
        ToolStripMenuItem mGoToolStripMenuItem;
        ToolStripMenuItem mDetachToolStripMenuItem;
        ToolStripMenuItem mResetToolStripMenuItem;
        ToolStripMenuItem mViewToolStripMenuItem;
        ToolStripMenuItem mTeletypeToolStripMenuItem;
        ToolStripContainer mToolStripContainer;
        StatusStrip mStatusStrip;
        ToolStripStatusLabel mToolStripStatusLabel;
        LongValueTextBox mPCBox;
        ToolStrip mControlToolStrip;
        ToolStripLabel mPCToolStripLabel;
        ToolStripButton mShowPCToolStripButton;
        ToolStripLabel mTicksToolStripLabel;
        ToolStripTextBox mTicksToolStripTextBox;
        ToolStripButton mResetTicksToolStripButton;
        ToolStripButton mTickToolStripButton;
        ToolStripButton mStepToolStripButton;
        ToolStripButton mRunToolStripButton;
        ToolStripButton mGoToolStripButton;
        ToolStripButton mDetachToolStripButton;
        ToolStripButton mResetToolStripButton;
        ToolStripButton mTeletypeToolStripButton;
        SplitContainer mSplitContainer;
        GroupBox mMemoryGroup;
        MemoryEditor mMainMemoryEditor;
        MemoryEditor mFloatingPointMemoryEditor;
        readonly MemoryEditor[] mMemoryEditors;
        GroupBox mRegistersGroup;
        RegistersEditor mRegistersEditor;
        GroupBox mDevicesGroup;
        DevicesControl mDevicesControl;
        GroupBox mSymbolGroup;
        SymbolListView mSymbolListView;
        GroupBox mLogListGroup;
        LogListView mLogListView;
        ToolStripButton mClearBreakpointsToolStripButton;
        ToolStripMenuItem mClearBreakpointsToolStripMenuItem;
        Button mDeviceEditorButton;
        ToolStripMenuItem mDeviceEditorToolStripMenuItem;
        ToolStripCycleButton mModeCycleButton;
        TabControl mMemoryTabControl;
        TabPage mMainMemoryTab;
        TabPage mFloatingPointMemoryTab;
        ToolStripStatusLabel mModeToolStripStatusLabel;
        ToolStripStatusLabel mStatusToolStripStatusLabel;
        ToolTip mToolTip;
        ToolStripSeparator toolStripSeparator8;
        ToolStripMenuItem mFindMenuItem;
        ToolStripMenuItem mFindNextMenuItem;
        DeviceEditorForm mDeviceEditor;
        SearchDialog mSearchDialog;
        ToolStripSeparator toolStripSeparator9;
        ToolStripMenuItem mProfilingEnabledMenuItem;
        ToolStripMenuItem mProfilingShowTickCountsMenuItem;
        ToolStripMenuItem mProfilingResetCountsMenuItem;
        SearchParameters mSearchOptions;

        public MixForm()
		{
			mConfiguration = Configuration.Load(mDefaultDirectory);
			ExecutionSettings.ProfilingEnabled = mConfiguration.ProfilingEnabled;
			GuiSettings.Colors = mConfiguration.Colors;
			GuiSettings.ShowProfilingInfo = mConfiguration.ShowProfilingInfo;
			GuiSettings.ColorProfilingCounts = mConfiguration.ColorProfilingCounts;
			DeviceSettings.DefaultDeviceFilesDirectory = mDefaultDirectory;
			if (mConfiguration.FloatingPointMemoryWordCount != null)
			{
				ModuleSettings.FloatingPointMemoryWordCount = mConfiguration.FloatingPointMemoryWordCount.Value;
			}

			InitializeComponent();

			mMemoryEditors = new MemoryEditor[2];
			mMemoryEditors[0] = mMainMemoryEditor;
			mMemoryEditors[1] = null;

			mMix = new Mix();
			mMix.StepPerformed += mMix_StepPerformed;
			mMix.LogLineAdded += mMix_LogLineAdded;

			mPCBox = new LongValueTextBox();
			mPCBox.BackColor = Color.White;
			mPCBox.BorderStyle = BorderStyle.FixedSingle;
			mPCBox.ForeColor = Color.Black;
			mPCBox.LongValue = mMix.ProgramCounter;
			mPCBox.ClearZero = false;
			setPCBoxBounds();
			mPCBox.Name = "mPCBox";
			mPCBox.Size = new Size(40, 25);
			mPCBox.TabIndex = 1;
			mPCBox.Text = "0";
			mPCBox.ValueChanged += pcChanged;
			mControlToolStrip.Items.Insert(1, new LongValueToolStripTextBox(mPCBox));

			mRegistersEditor.Registers = mMix.Registers;

			mMainMemoryEditor.ToolTip = mToolTip;
			mMainMemoryEditor.Memory = mMix.FullMemory;
			mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
			mMainMemoryEditor.IndexedAddressCalculatorCallback = calculateIndexedAddress;

			mProfilingEnabledMenuItem.Checked = ExecutionSettings.ProfilingEnabled;
			mProfilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			mProfilingShowTickCountsMenuItem.Checked = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick;
			mProfilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			mSymbolListView.MemoryMinIndex = mMix.FullMemory.MinWordIndex;
			mSymbolListView.MemoryMaxIndex = mMix.FullMemory.MaxWordIndex;

			var layoutStructure = new DevicesControl.LayoutStructure(DevicesControl.Orientations.Horizontal, 8, 4);
			layoutStructure[8] = DevicesControl.Breaks.Sequence;
			layoutStructure[16] = DevicesControl.Breaks.Section;
			layoutStructure[18] = DevicesControl.Breaks.Sequence;

			mDevicesControl.ToolTip = mToolTip;
			mDevicesControl.Structure = layoutStructure;
			mDevicesControl.Devices = mMix.Devices;
			mDevicesControl.DeviceDoubleClick += mDevicesControl_DeviceDoubleClick;

			mTeletype = new TeletypeForm(mMix.Devices.Teletype);
			mTeletype.VisibleChanged += mTeletype_VisibleChanged;

			if (mConfiguration.TeletypeWindowLocation != Point.Empty)
			{
				mTeletype.Location = mConfiguration.TeletypeWindowLocation;
			}

			if (mConfiguration.TeletypeWindowSize != Size.Empty)
			{
				mTeletype.Size = mConfiguration.TeletypeWindowSize;
			}

			mTeletype.TopMost = mConfiguration.TeletypeOnTop;
			mTeletype.EchoInput = mConfiguration.TeletypeEchoInput;

			mTeletype.AddOutputText("*** MIX TELETYPE INITIALIZED ***");

			if (mConfiguration.TeletypeVisible)
			{
				mTeletype.Show();
			}

			CardDeckExporter.LoaderCards = mConfiguration.LoaderCards;
			updateDeviceConfig();

			mDeviceEditor = new DeviceEditorForm(mMix.Devices);
			mDeviceEditor.VisibleChanged += mDeviceEditor_VisibleChanged;

			if (mConfiguration.DeviceEditorWindowLocation != Point.Empty)
			{
				mDeviceEditor.Location = mConfiguration.DeviceEditorWindowLocation;
			}

			if (mConfiguration.DeviceEditorWindowSize != Size.Empty)
			{
				mDeviceEditor.Size = mConfiguration.DeviceEditorWindowSize;
			}

			if (mConfiguration.DeviceEditorVisible)
			{
				mDeviceEditor.Show();
			}

			mMix.Devices.Teletype.InputRequired += mMix_InputRequired;
			mMix.BreakpointManager = mMainMemoryEditor;

			loadControlProgram();
			applyFloatingPointSettings();

			mOpenProgramFileDialog = null;
			mSourceAndFindingsForm = null;
			mReadOnly = false;
			mRunning = false;
			mSteppingState = steppingState.Idle;

			var symbols = new SymbolCollection();
			mSymbolListView.Symbols = symbols;
			mMainMemoryEditor.Symbols = symbols;

			ToolStripCycleButton.Step controlStep = mModeCycleButton.AddStep(ModuleBase.RunMode.Control);
			ToolStripCycleButton.Step normalStep = mModeCycleButton.AddStep(ModuleBase.RunMode.Normal, controlStep);
			mModeCycleButton.AddStep(ModuleBase.RunMode.Module, normalStep);
			controlStep.NextStep = normalStep;

			mModeCycleButton.Value = mMix.Mode;
			mModeCycleButton.ValueChanged += mModeCycleButton_ValueChanged;

			Update();

			if (mConfiguration.MainWindowLocation != Point.Empty)
			{
				Location = mConfiguration.MainWindowLocation;
			}

			if (mConfiguration.MainWindowSize != Size.Empty)
			{
				Size = mConfiguration.MainWindowSize;
			}

			WindowState = mConfiguration.MainWindowState;
			SizeChanged += this_SizeChanged;
			LocationChanged += this_LocationChanged;
		}

        MemoryEditor activeMemoryEditor => mMemoryEditors[mMemoryTabControl.SelectedIndex];

        void findNext() => activeMemoryEditor.FindMatch(mSearchOptions);

        int calculateIndexedAddress(int address, int index) =>
            InstructionHelpers.GetValidIndexedAddress(mMix, address, index, false);


        void setPCBoxBounds()
        {
            mPCBox.MinValue = mMix.Memory.MinWordIndex;
            mPCBox.MaxValue = mMix.Memory.MaxWordIndex;
        }

        void applyFloatingPointSettings()
        {
            mMix.SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);
            if (ModuleSettings.FloatingPointEnabled)
            {
                loadModuleProgram(mMix.FloatingPointModule, ModuleSettings.FloatingPointProgramFile, "floating point");
            }
        }

        void loadControlProgram()
        {
            string controlProgramFileName = ModuleSettings.ControlProgramFile;

            if (string.IsNullOrEmpty(controlProgramFileName) || !File.Exists(controlProgramFileName)) return;

            loadModuleProgram(mMix, controlProgramFileName, "control");
            mMix.FullMemory.ClearSourceLines();
        }

        SymbolCollection loadModuleProgram(ModuleBase module, string fileName, string programName)
        {
            try
            {
                PreInstruction[] instructions;
                AssemblyFindingCollection findings;
                SymbolCollection symbols;

                InstructionInstanceBase[] instances = Assembler.Assemble(File.ReadAllLines(fileName), out instructions, out symbols, out findings);
                if (instances != null)
                {
                    if (!module.LoadInstructionInstances(instances, symbols))
                    {
                        mLogListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", string.Format("Failed to load {0} program", programName)));
                    }
                }
                else
                {
                    if (findings != null)
                    {
                        mLogListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Parse failed", string.Format("Error(s) occured while assembling {0} program", programName)));
                        foreach (AssemblyFinding finding in findings)
                        {
                            mLogListView.AddLogLine(new LogLine("Assembler", finding.Severity, "Parse error", finding.LineNumber == int.MinValue ? finding.Message : string.Format("Line {0}: {1}", finding.LineNumber, finding.Message)));
                        }
                    }
                    mLogListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", string.Format("Failed to assemble {0} program", programName)));

                    return symbols;

                }
            }
            catch (Exception ex)
            {
                mLogListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", string.Format("Failed to read {0} program: {1}", programName, ex.Message)));
            }

            return null;
        }

        bool disableTeletypeOnTop()
        {
            if (mTeletype != null && mTeletype.Visible && mTeletype.TopMost)
            {
                mTeletype.TopMost = false;
                return true;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                components?.Dispose();
                mMix?.Dispose();
            }

			base.Dispose(disposing);
		}

        static void handleException(Exception exception)
        {
            Exception handlingException = null;
            string path = Path.Combine(Environment.CurrentDirectory, "exception.log");

            try
            {
                var writer = new StreamWriter(path, true);
                writer.WriteLine("Unhandled exception " + exception.GetType().FullName + ": " + exception.Message);
                writer.WriteLine("  Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    writer.WriteLine("  Inner exception: " + exception.InnerException);
                }

                if (exception.Data.Count > 0)
                {
                    writer.WriteLine("  Exception data:");

                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        writer.WriteLine(string.Concat(new object[] { "  * ", entry.Key, " = ", entry.Value }));
                    }
                }

                writer.WriteLine();
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                handlingException = ex;
            }

            string text = string.Concat(new object[] { "I'm sorry, MixEmul seems to have encountered a fatal problem.\nAn unhandled error of type ", exception.GetType().Name, " occured, with message: ", exception.Message, '\n' });

            if (handlingException == null)
            {
                text = text + "A detailed error log has been written to the following location: " + path + ".\nPlease attach this file when reporting the error.";
            }
            else
            {
                string str3 = text;
                text = str3 + "Additionally, an error of type " + handlingException.GetType().Name + " occured while logging the error, with message: " + handlingException.Message + "\nPlease include this information when reporting this error.";
            }

            MessageBox.Show(text, "Unhandled program error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            Application.Exit();
        }

        void implementPCChange(int address)
        {
            if (mMainMemoryEditor.IsAddressVisible(mMix.ProgramCounter))
            {
                mMainMemoryEditor.MakeAddressVisible(address, mMix.Status == ModuleBase.RunStatus.Idle);
            }

            mMix.ProgramCounter = address;
            mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
            mPCBox.LongValue = mMix.ProgramCounter;
        }

        void InitializeComponent()
        {
            components = new Container();
            ToolStripSeparator fileMenuToolStripSeparator;
            ToolStripSeparator toolStripSeparator1;
            ToolStripSeparator toolStripSeparator2;
            ToolStripSeparator toolStripSeparator3;
            ToolStripSeparator toolStripSeparator4;
            ToolStripSeparator toolStripSeparator5;
            ToolStripSeparator toolStripSeparator6;
            ToolStripSeparator toolStripSeparator7;
            var resources = new ComponentResourceManager(typeof(MixForm));
            var registers1 = new Registers();
            mSeverityImageList = new ImageList(components);
            mMainMenuStrip = new MenuStrip();
            mFileToolStripMenuItem = new ToolStripMenuItem();
            mOpenProgramToolStripMenuItem = new ToolStripMenuItem();
            mExitToolStripMenuItem = new ToolStripMenuItem();
            mViewToolStripMenuItem = new ToolStripMenuItem();
            mTeletypeToolStripMenuItem = new ToolStripMenuItem();
            mDeviceEditorToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            mFindMenuItem = new ToolStripMenuItem();
            mFindNextMenuItem = new ToolStripMenuItem();
            mActionsToolStripMenuItem = new ToolStripMenuItem();
            mTickToolStripMenuItem = new ToolStripMenuItem();
            mStepToolStripMenuItem = new ToolStripMenuItem();
            mRunToolStripMenuItem = new ToolStripMenuItem();
            mGoToolStripMenuItem = new ToolStripMenuItem();
            mDetachToolStripMenuItem = new ToolStripMenuItem();
            mClearBreakpointsToolStripMenuItem = new ToolStripMenuItem();
            mResetToolStripMenuItem = new ToolStripMenuItem();
            mToolsToolStripMenuItem = new ToolStripMenuItem();
            mPreferencesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            mProfilingEnabledMenuItem = new ToolStripMenuItem();
            mProfilingShowTickCountsMenuItem = new ToolStripMenuItem();
            mHelpToolStripMenuItem = new ToolStripMenuItem();
            mAboutToolStripMenuItem = new ToolStripMenuItem();
            mToolStripContainer = new ToolStripContainer();
            mStatusStrip = new StatusStrip();
            mModeToolStripStatusLabel = new ToolStripStatusLabel();
            mModeCycleButton = new ToolStripCycleButton(components);
            mStatusToolStripStatusLabel = new ToolStripStatusLabel();
            mToolStripStatusLabel = new ToolStripStatusLabel();
            mSplitContainer = new SplitContainer();
            mMemoryGroup = new GroupBox();
            mMemoryTabControl = new TabControl();
            mMainMemoryTab = new TabPage();
            mMainMemoryEditor = new MemoryEditor();
            mFloatingPointMemoryTab = new TabPage();
            mRegistersGroup = new GroupBox();
            mRegistersEditor = new RegistersEditor();
            mDevicesGroup = new GroupBox();
            mDeviceEditorButton = new Button();
            mDevicesControl = new DevicesControl();
            mSymbolGroup = new GroupBox();
            mSymbolListView = new SymbolListView();
            mLogListGroup = new GroupBox();
            mLogListView = new LogListView();
            mControlToolStrip = new ToolStrip();
            mPCToolStripLabel = new ToolStripLabel();
            mShowPCToolStripButton = new ToolStripButton();
            mTicksToolStripLabel = new ToolStripLabel();
            mTicksToolStripTextBox = new ToolStripTextBox();
            mResetTicksToolStripButton = new ToolStripButton();
            mTickToolStripButton = new ToolStripButton();
            mStepToolStripButton = new ToolStripButton();
            mRunToolStripButton = new ToolStripButton();
            mGoToolStripButton = new ToolStripButton();
            mDetachToolStripButton = new ToolStripButton();
            mClearBreakpointsToolStripButton = new ToolStripButton();
            mResetToolStripButton = new ToolStripButton();
            mTeletypeToolStripButton = new ToolStripButton();
            mToolTip = new ToolTip(components);
            mProfilingResetCountsMenuItem = new ToolStripMenuItem();
            fileMenuToolStripSeparator = new ToolStripSeparator();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripSeparator5 = new ToolStripSeparator();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripSeparator7 = new ToolStripSeparator();
            mMainMenuStrip.SuspendLayout();
            mToolStripContainer.BottomToolStripPanel.SuspendLayout();
            mToolStripContainer.ContentPanel.SuspendLayout();
            mToolStripContainer.TopToolStripPanel.SuspendLayout();
            mToolStripContainer.SuspendLayout();
            mStatusStrip.SuspendLayout();
            mSplitContainer.Panel1.SuspendLayout();
            mSplitContainer.Panel2.SuspendLayout();
            mSplitContainer.SuspendLayout();
            mMemoryGroup.SuspendLayout();
            mMemoryTabControl.SuspendLayout();
            mMainMemoryTab.SuspendLayout();
            mRegistersGroup.SuspendLayout();
            mDevicesGroup.SuspendLayout();
            mSymbolGroup.SuspendLayout();
            mLogListGroup.SuspendLayout();
            mControlToolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // fileMenuToolStripSeparator
            // 
            fileMenuToolStripSeparator.Name = "fileMenuToolStripSeparator";
            fileMenuToolStripSeparator.Size = new Size(201, 6);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(204, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(204, 6);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 25);
            // 
            // mSeverityImageList
            // 
            mSeverityImageList.ImageStream = ((ImageListStreamer)(resources.GetObject("mSeverityImageList.ImageStream")));
            mSeverityImageList.TransparentColor = Color.Transparent;
            mSeverityImageList.Images.SetKeyName(0, "");
            mSeverityImageList.Images.SetKeyName(1, "");
            mSeverityImageList.Images.SetKeyName(2, "");
            mSeverityImageList.Images.SetKeyName(3, "");
            // 
            // mMainMenuStrip
            // 
            mMainMenuStrip.Dock = DockStyle.None;
            mMainMenuStrip.Items.AddRange(new ToolStripItem[] {
            mFileToolStripMenuItem,
            mViewToolStripMenuItem,
            mActionsToolStripMenuItem,
            mToolsToolStripMenuItem,
            mHelpToolStripMenuItem});
            mMainMenuStrip.Location = new Point(0, 0);
            mMainMenuStrip.Name = "mMainMenuStrip";
            mMainMenuStrip.Size = new Size(804, 24);
            mMainMenuStrip.TabIndex = 0;
            mMainMenuStrip.Text = "Main Menu";
            // 
            // mFileToolStripMenuItem
            // 
            mFileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            mOpenProgramToolStripMenuItem,
            fileMenuToolStripSeparator,
            mExitToolStripMenuItem});
            mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            mFileToolStripMenuItem.Size = new Size(37, 20);
            mFileToolStripMenuItem.Text = "&File";
            // 
            // mOpenProgramToolStripMenuItem
            // 
            mOpenProgramToolStripMenuItem.Image = ((Image)(resources.GetObject("mOpenProgramToolStripMenuItem.Image")));
            mOpenProgramToolStripMenuItem.Name = "mOpenProgramToolStripMenuItem";
            mOpenProgramToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.O);
            mOpenProgramToolStripMenuItem.Size = new Size(204, 22);
            mOpenProgramToolStripMenuItem.Text = "&Open program...";
            mOpenProgramToolStripMenuItem.Click += mOpenProgramMenuItem_Click;
            // 
            // mExitToolStripMenuItem
            // 
            mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
            mExitToolStripMenuItem.Size = new Size(204, 22);
            mExitToolStripMenuItem.Text = "E&xit";
            mExitToolStripMenuItem.Click += mExitMenuItem_Click;
            // 
            // mViewToolStripMenuItem
            // 
            mViewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            mTeletypeToolStripMenuItem,
            mDeviceEditorToolStripMenuItem,
            toolStripSeparator8,
            mFindMenuItem,
            mFindNextMenuItem});
            mViewToolStripMenuItem.Name = "mViewToolStripMenuItem";
            mViewToolStripMenuItem.Size = new Size(44, 20);
            mViewToolStripMenuItem.Text = "&View";
            // 
            // mTeletypeToolStripMenuItem
            // 
            mTeletypeToolStripMenuItem.Image = ((Image)(resources.GetObject("mTeletypeToolStripMenuItem.Image")));
            mTeletypeToolStripMenuItem.Name = "mTeletypeToolStripMenuItem";
            mTeletypeToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.T);
            mTeletypeToolStripMenuItem.Size = new Size(215, 22);
            mTeletypeToolStripMenuItem.Text = "&Show Teletype";
            mTeletypeToolStripMenuItem.Click += mTeletypeItem_Click;
            // 
            // mDeviceEditorToolStripMenuItem
            // 
            mDeviceEditorToolStripMenuItem.Image = ((Image)(resources.GetObject("mDeviceEditorToolStripMenuItem.Image")));
            mDeviceEditorToolStripMenuItem.Name = "mDeviceEditorToolStripMenuItem";
            mDeviceEditorToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.E);
            mDeviceEditorToolStripMenuItem.Size = new Size(215, 22);
            mDeviceEditorToolStripMenuItem.Text = "Show &Device Editor";
            mDeviceEditorToolStripMenuItem.Click += mDeviceEditorItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(212, 6);
            // 
            // mFindMenuItem
            // 
            mFindMenuItem.Image = Properties.Resources.Find_5650;
            mFindMenuItem.Name = "mFindMenuItem";
            mFindMenuItem.ShortcutKeys = (Keys.Control | Keys.F);
            mFindMenuItem.Size = new Size(215, 22);
            mFindMenuItem.Text = "Find in memory...";
            mFindMenuItem.Click += mFindMenuItem_Click;
            // 
            // mFindNextMenuItem
            // 
            mFindNextMenuItem.Name = "mFindNextMenuItem";
            mFindNextMenuItem.ShortcutKeys = Keys.F3;
            mFindNextMenuItem.Size = new Size(215, 22);
            mFindNextMenuItem.Text = "Find next";
            mFindNextMenuItem.Click += mFindNextMenuItem_Click;
            // 
            // mActionsToolStripMenuItem
            // 
            mActionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            mTickToolStripMenuItem,
            mStepToolStripMenuItem,
            mRunToolStripMenuItem,
            mGoToolStripMenuItem,
            toolStripSeparator1,
            mDetachToolStripMenuItem,
            toolStripSeparator2,
            mClearBreakpointsToolStripMenuItem,
            mResetToolStripMenuItem});
            mActionsToolStripMenuItem.Name = "mActionsToolStripMenuItem";
            mActionsToolStripMenuItem.Size = new Size(59, 20);
            mActionsToolStripMenuItem.Text = "&Actions";
            // 
            // mTickToolStripMenuItem
            // 
            mTickToolStripMenuItem.Image = ((Image)(resources.GetObject("mTickToolStripMenuItem.Image")));
            mTickToolStripMenuItem.Name = "mTickToolStripMenuItem";
            mTickToolStripMenuItem.ShortcutKeys = Keys.F11;
            mTickToolStripMenuItem.Size = new Size(207, 22);
            mTickToolStripMenuItem.Text = "T&ick";
            mTickToolStripMenuItem.Click += mTickItem_Click;
            // 
            // mStepToolStripMenuItem
            // 
            mStepToolStripMenuItem.Image = ((Image)(resources.GetObject("mStepToolStripMenuItem.Image")));
            mStepToolStripMenuItem.Name = "mStepToolStripMenuItem";
            mStepToolStripMenuItem.ShortcutKeys = Keys.F10;
            mStepToolStripMenuItem.Size = new Size(207, 22);
            mStepToolStripMenuItem.Text = "&Step";
            mStepToolStripMenuItem.Click += mStepItem_Click;
            // 
            // mRunToolStripMenuItem
            // 
            mRunToolStripMenuItem.Image = ((Image)(resources.GetObject("mRunToolStripMenuItem.Image")));
            mRunToolStripMenuItem.Name = "mRunToolStripMenuItem";
            mRunToolStripMenuItem.ShortcutKeys = Keys.F5;
            mRunToolStripMenuItem.Size = new Size(207, 22);
            mRunToolStripMenuItem.Text = "R&un";
            mRunToolStripMenuItem.Click += mRunItem_Click;
            // 
            // mGoToolStripMenuItem
            // 
            mGoToolStripMenuItem.Image = ((Image)(resources.GetObject("mGoToolStripMenuItem.Image")));
            mGoToolStripMenuItem.Name = "mGoToolStripMenuItem";
            mGoToolStripMenuItem.ShortcutKeys = (Keys.Shift | Keys.F5);
            mGoToolStripMenuItem.Size = new Size(207, 22);
            mGoToolStripMenuItem.Text = "&Go";
            mGoToolStripMenuItem.Click += mGoItem_Click;
            // 
            // mDetachToolStripMenuItem
            // 
            mDetachToolStripMenuItem.Name = "mDetachToolStripMenuItem";
            mDetachToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.D);
            mDetachToolStripMenuItem.Size = new Size(207, 22);
            mDetachToolStripMenuItem.Text = "&Detach";
            mDetachToolStripMenuItem.Click += mDetachItem_Click;
            // 
            // mClearBreakpointsToolStripMenuItem
            // 
            mClearBreakpointsToolStripMenuItem.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripMenuItem.Image")));
            mClearBreakpointsToolStripMenuItem.Name = "mClearBreakpointsToolStripMenuItem";
            mClearBreakpointsToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.B);
            mClearBreakpointsToolStripMenuItem.Size = new Size(207, 22);
            mClearBreakpointsToolStripMenuItem.Text = "Clear &Breakpoints";
            mClearBreakpointsToolStripMenuItem.Click += mClearBreakpointsItem_Click;
            // 
            // mResetToolStripMenuItem
            // 
            mResetToolStripMenuItem.Image = ((Image)(resources.GetObject("mResetToolStripMenuItem.Image")));
            mResetToolStripMenuItem.Name = "mResetToolStripMenuItem";
            mResetToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.R);
            mResetToolStripMenuItem.Size = new Size(207, 22);
            mResetToolStripMenuItem.Text = "&Reset";
            mResetToolStripMenuItem.Click += mResetItem_Click;
            // 
            // mToolsToolStripMenuItem
            // 
            mToolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            mPreferencesToolStripMenuItem,
            toolStripSeparator9,
            mProfilingEnabledMenuItem,
            mProfilingShowTickCountsMenuItem,
            mProfilingResetCountsMenuItem});
            mToolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
            mToolsToolStripMenuItem.Size = new Size(48, 20);
            mToolsToolStripMenuItem.Text = "&Tools";
            // 
            // mPreferencesToolStripMenuItem
            // 
            mPreferencesToolStripMenuItem.Image = ((Image)(resources.GetObject("mPreferencesToolStripMenuItem.Image")));
            mPreferencesToolStripMenuItem.Name = "mPreferencesToolStripMenuItem";
            mPreferencesToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.P);
            mPreferencesToolStripMenuItem.Size = new Size(185, 22);
            mPreferencesToolStripMenuItem.Text = "&Preferences...";
            mPreferencesToolStripMenuItem.Click += mPreferencesMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(182, 6);
            // 
            // mProfilingEnabledMenuItem
            // 
            mProfilingEnabledMenuItem.CheckOnClick = true;
            mProfilingEnabledMenuItem.Name = "mProfilingEnabledMenuItem";
            mProfilingEnabledMenuItem.Size = new Size(185, 22);
            mProfilingEnabledMenuItem.Text = "&Enable profiling";
            mProfilingEnabledMenuItem.CheckedChanged += mProfilingEnabledMenuItem_CheckedChanged;
            // 
            // mProfilingShowTickCountsMenuItem
            // 
            mProfilingShowTickCountsMenuItem.CheckOnClick = true;
            mProfilingShowTickCountsMenuItem.Name = "mProfilingShowTickCountsMenuItem";
            mProfilingShowTickCountsMenuItem.Size = new Size(185, 22);
            mProfilingShowTickCountsMenuItem.Text = "&Show tick counts";
            mProfilingShowTickCountsMenuItem.CheckedChanged += mProfilingShowTickCountsMenuItem_CheckedChanged;
            // 
            // mHelpToolStripMenuItem
            // 
            mHelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            mAboutToolStripMenuItem});
            mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
            mHelpToolStripMenuItem.Size = new Size(44, 20);
            mHelpToolStripMenuItem.Text = "&Help";
            // 
            // mAboutToolStripMenuItem
            // 
            mAboutToolStripMenuItem.Image = ((Image)(resources.GetObject("mAboutToolStripMenuItem.Image")));
            mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
            mAboutToolStripMenuItem.Size = new Size(116, 22);
            mAboutToolStripMenuItem.Text = "&About...";
            mAboutToolStripMenuItem.Click += mAboutMenuItem_Click;
            // 
            // mToolStripContainer
            // 
            // 
            // mToolStripContainer.BottomToolStripPanel
            // 
            mToolStripContainer.BottomToolStripPanel.Controls.Add(mStatusStrip);
            // 
            // mToolStripContainer.ContentPanel
            // 
            mToolStripContainer.ContentPanel.Controls.Add(mSplitContainer);
            mToolStripContainer.ContentPanel.Size = new Size(804, 491);
            mToolStripContainer.Dock = DockStyle.Fill;
            mToolStripContainer.LeftToolStripPanelVisible = false;
            mToolStripContainer.Location = new Point(0, 0);
            mToolStripContainer.Name = "mToolStripContainer";
            mToolStripContainer.RightToolStripPanelVisible = false;
            mToolStripContainer.Size = new Size(804, 562);
            mToolStripContainer.TabIndex = 0;
            mToolStripContainer.Text = "ToolStripContainer";
            // 
            // mToolStripContainer.TopToolStripPanel
            // 
            mToolStripContainer.TopToolStripPanel.Controls.Add(mMainMenuStrip);
            mToolStripContainer.TopToolStripPanel.Controls.Add(mControlToolStrip);
            // 
            // mStatusStrip
            // 
            mStatusStrip.Dock = DockStyle.None;
            mStatusStrip.Items.AddRange(new ToolStripItem[] {
            mModeToolStripStatusLabel,
            mModeCycleButton,
            mStatusToolStripStatusLabel,
            mToolStripStatusLabel});
            mStatusStrip.Location = new Point(0, 0);
            mStatusStrip.Name = "mStatusStrip";
            mStatusStrip.Size = new Size(804, 22);
            mStatusStrip.TabIndex = 8;
            // 
            // mModeToolStripStatusLabel
            // 
            mModeToolStripStatusLabel.Name = "mModeToolStripStatusLabel";
            mModeToolStripStatusLabel.Size = new Size(41, 17);
            mModeToolStripStatusLabel.Text = "Mode:";
            // 
            // mModeCycleButton
            // 
            mModeCycleButton.AutoSize = false;
            mModeCycleButton.Name = "mModeCycleButton";
            mModeCycleButton.Size = new Size(60, 20);
            mModeCycleButton.Value = null;
            // 
            // mStatusToolStripStatusLabel
            // 
            mStatusToolStripStatusLabel.Name = "mStatusToolStripStatusLabel";
            mStatusToolStripStatusLabel.Size = new Size(48, 17);
            mStatusToolStripStatusLabel.Text = "| Status:";
            // 
            // mToolStripStatusLabel
            // 
            mToolStripStatusLabel.Name = "mToolStripStatusLabel";
            mToolStripStatusLabel.Size = new Size(10, 17);
            mToolStripStatusLabel.Text = " ";
            // 
            // mSplitContainer
            // 
            mSplitContainer.BackColor = SystemColors.Control;
            mSplitContainer.Dock = DockStyle.Fill;
            mSplitContainer.FixedPanel = FixedPanel.Panel2;
            mSplitContainer.Location = new Point(0, 0);
            mSplitContainer.Name = "mSplitContainer";
            mSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // mSplitContainer.Panel1
            // 
            mSplitContainer.Panel1.Controls.Add(mMemoryGroup);
            mSplitContainer.Panel1.Controls.Add(mRegistersGroup);
            mSplitContainer.Panel1.Controls.Add(mDevicesGroup);
            mSplitContainer.Panel1.Controls.Add(mSymbolGroup);
            mSplitContainer.Panel1MinSize = 380;
            // 
            // mSplitContainer.Panel2
            // 
            mSplitContainer.Panel2.Controls.Add(mLogListGroup);
            mSplitContainer.Panel2MinSize = 96;
            mSplitContainer.Size = new Size(804, 491);
            mSplitContainer.SplitterDistance = 385;
            mSplitContainer.TabIndex = 6;
            // 
            // mMemoryGroup
            // 
            mMemoryGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mMemoryGroup.Controls.Add(mMemoryTabControl);
            mMemoryGroup.Location = new Point(236, -2);
            mMemoryGroup.Name = "mMemoryGroup";
            mMemoryGroup.Size = new Size(568, 328);
            mMemoryGroup.TabIndex = 4;
            mMemoryGroup.TabStop = false;
            mMemoryGroup.Text = "Memory";
            // 
            // mMemoryTabControl
            // 
            mMemoryTabControl.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mMemoryTabControl.Controls.Add(mMainMemoryTab);
            mMemoryTabControl.Controls.Add(mFloatingPointMemoryTab);
            mMemoryTabControl.Location = new Point(4, 16);
            mMemoryTabControl.Name = "mMemoryTabControl";
            mMemoryTabControl.SelectedIndex = 0;
            mMemoryTabControl.Size = new Size(560, 309);
            mMemoryTabControl.TabIndex = 0;
            mMemoryTabControl.SelectedIndexChanged += mMemoryTabControl_SelectedIndexChanged;
            mMemoryTabControl.Selecting += mMemoryTabControl_Selecting;
            // 
            // mMainMemoryTab
            // 
            mMainMemoryTab.Controls.Add(mMainMemoryEditor);
            mMainMemoryTab.Location = new Point(4, 22);
            mMainMemoryTab.Name = "mMainMemoryTab";
            mMainMemoryTab.Padding = new Padding(3);
            mMainMemoryTab.Size = new Size(552, 283);
            mMainMemoryTab.TabIndex = 0;
            mMainMemoryTab.Text = "Main";
            mMainMemoryTab.UseVisualStyleBackColor = true;
            // 
            // mMainMemoryEditor
            // 
            mMainMemoryEditor.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mMainMemoryEditor.FirstVisibleAddress = 0;
            mMainMemoryEditor.IndexedAddressCalculatorCallback = null;
            mMainMemoryEditor.Location = new Point(0, 0);
            mMainMemoryEditor.MarkedAddress = -1;
            mMainMemoryEditor.Memory = null;
            mMainMemoryEditor.Name = "mMainMemoryEditor";
            mMainMemoryEditor.ReadOnly = false;
            mMainMemoryEditor.ResizeInProgress = false;
            mMainMemoryEditor.Size = new Size(552, 283);
            mMainMemoryEditor.Symbols = null;
            mMainMemoryEditor.TabIndex = 0;
            mMainMemoryEditor.ToolTip = null;
            mMainMemoryEditor.AddressSelected += mMemoryEditor_addressSelected;
            // 
            // mFloatingPointMemoryTab
            // 
            mFloatingPointMemoryTab.Location = new Point(4, 22);
            mFloatingPointMemoryTab.Name = "mFloatingPointMemoryTab";
            mFloatingPointMemoryTab.Padding = new Padding(3);
            mFloatingPointMemoryTab.Size = new Size(552, 283);
            mFloatingPointMemoryTab.TabIndex = 1;
            mFloatingPointMemoryTab.Text = "Floating Point";
            mFloatingPointMemoryTab.UseVisualStyleBackColor = true;
            // 
            // mRegistersGroup
            // 
            mRegistersGroup.Controls.Add(mRegistersEditor);
            mRegistersGroup.Location = new Point(0, -2);
            mRegistersGroup.Name = "mRegistersGroup";
            mRegistersGroup.Size = new Size(232, 236);
            mRegistersGroup.TabIndex = 2;
            mRegistersGroup.TabStop = false;
            mRegistersGroup.Text = "Registers";
            // 
            // mRegistersEditor
            // 
            mRegistersEditor.Location = new Point(8, 16);
            mRegistersEditor.Name = "mRegistersEditor";
            mRegistersEditor.ReadOnly = false;
            registers1.CompareIndicator = MixLib.Registers.CompValues.Equal;
            registers1.OverflowIndicator = false;
            mRegistersEditor.Registers = registers1;
            mRegistersEditor.Size = new Size(218, 214);
            mRegistersEditor.TabIndex = 0;
            // 
            // mDevicesGroup
            // 
            mDevicesGroup.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mDevicesGroup.Controls.Add(mDeviceEditorButton);
            mDevicesGroup.Controls.Add(mDevicesControl);
            mDevicesGroup.Location = new Point(0, 326);
            mDevicesGroup.Name = "mDevicesGroup";
            mDevicesGroup.Size = new Size(804, 56);
            mDevicesGroup.TabIndex = 5;
            mDevicesGroup.TabStop = false;
            mDevicesGroup.Text = "Devices";
            // 
            // mDeviceEditorButton
            // 
            mDeviceEditorButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            mDeviceEditorButton.Location = new Point(721, 16);
            mDeviceEditorButton.Margin = new Padding(0);
            mDeviceEditorButton.Name = "mDeviceEditorButton";
            mDeviceEditorButton.Size = new Size(75, 23);
            mDeviceEditorButton.TabIndex = 1;
            mDeviceEditorButton.Text = "Show E&ditor";
            mDeviceEditorButton.UseVisualStyleBackColor = true;
            mDeviceEditorButton.Click += mDeviceEditorItem_Click;
            // 
            // mDevicesControl
            // 
            mDevicesControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mDevicesControl.Devices = null;
            mDevicesControl.Location = new Point(4, 16);
            mDevicesControl.Name = "mDevicesControl";
            mDevicesControl.Size = new Size(711, 36);
            mDevicesControl.Structure = null;
            mDevicesControl.TabIndex = 0;
            mDevicesControl.ToolTip = null;
            // 
            // mSymbolGroup
            // 
            mSymbolGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left);
            mSymbolGroup.Controls.Add(mSymbolListView);
            mSymbolGroup.Location = new Point(0, 234);
            mSymbolGroup.Name = "mSymbolGroup";
            mSymbolGroup.Size = new Size(232, 92);
            mSymbolGroup.TabIndex = 3;
            mSymbolGroup.TabStop = false;
            mSymbolGroup.Text = "Symbols";
            // 
            // mSymbolListView
            // 
            mSymbolListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mSymbolListView.Location = new Point(8, 16);
            mSymbolListView.MemoryMaxIndex = 0;
            mSymbolListView.MemoryMinIndex = 0;
            mSymbolListView.Name = "mSymbolListView";
            mSymbolListView.Size = new Size(218, 70);
            mSymbolListView.Symbols = null;
            mSymbolListView.TabIndex = 0;
            mSymbolListView.AddressSelected += listViews_addressSelected;
            // 
            // mLogListGroup
            // 
            mLogListGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mLogListGroup.Controls.Add(mLogListView);
            mLogListGroup.Location = new Point(0, 3);
            mLogListGroup.Name = "mLogListGroup";
            mLogListGroup.Size = new Size(804, 95);
            mLogListGroup.TabIndex = 7;
            mLogListGroup.TabStop = false;
            mLogListGroup.Text = "Messages";
            // 
            // mLogListView
            // 
            mLogListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right);
            mLogListView.Location = new Point(8, 16);
            mLogListView.Name = "mLogListView";
            mLogListView.SeverityImageList = mSeverityImageList;
            mLogListView.Size = new Size(788, 73);
            mLogListView.TabIndex = 0;
            mLogListView.AddressSelected += listViews_addressSelected;
            // 
            // mControlToolStrip
            // 
            mControlToolStrip.Dock = DockStyle.None;
            mControlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            mControlToolStrip.Items.AddRange(new ToolStripItem[] {
            mPCToolStripLabel,
            mShowPCToolStripButton,
            toolStripSeparator3,
            mTicksToolStripLabel,
            mTicksToolStripTextBox,
            mResetTicksToolStripButton,
            toolStripSeparator4,
            mTickToolStripButton,
            mStepToolStripButton,
            mRunToolStripButton,
            mGoToolStripButton,
            toolStripSeparator5,
            mDetachToolStripButton,
            toolStripSeparator6,
            mClearBreakpointsToolStripButton,
            mResetToolStripButton,
            toolStripSeparator7,
            mTeletypeToolStripButton});
            mControlToolStrip.Location = new Point(3, 24);
            mControlToolStrip.Name = "mControlToolStrip";
            mControlToolStrip.Size = new Size(761, 25);
            mControlToolStrip.TabIndex = 1;
            // 
            // mPCToolStripLabel
            // 
            mPCToolStripLabel.Name = "mPCToolStripLabel";
            mPCToolStripLabel.Size = new Size(28, 22);
            mPCToolStripLabel.Text = "PC: ";
            // 
            // mShowPCToolStripButton
            // 
            mShowPCToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mShowPCToolStripButton.Image = ((Image)(resources.GetObject("mShowPCToolStripButton.Image")));
            mShowPCToolStripButton.ImageTransparentColor = Color.Magenta;
            mShowPCToolStripButton.Name = "mShowPCToolStripButton";
            mShowPCToolStripButton.Size = new Size(40, 22);
            mShowPCToolStripButton.Text = "Sh&ow";
            mShowPCToolStripButton.ToolTipText = "Show the program counter";
            mShowPCToolStripButton.Click += mShowPCButton_Click;
            // 
            // mTicksToolStripLabel
            // 
            mTicksToolStripLabel.Name = "mTicksToolStripLabel";
            mTicksToolStripLabel.Size = new Size(40, 22);
            mTicksToolStripLabel.Text = "Ticks: ";
            // 
            // mTicksToolStripTextBox
            // 
            mTicksToolStripTextBox.BorderStyle = BorderStyle.FixedSingle;
            mTicksToolStripTextBox.Name = "mTicksToolStripTextBox";
            mTicksToolStripTextBox.ReadOnly = true;
            mTicksToolStripTextBox.Size = new Size(64, 25);
            mTicksToolStripTextBox.Text = "0";
            // 
            // mResetTicksToolStripButton
            // 
            mResetTicksToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mResetTicksToolStripButton.Image = ((Image)(resources.GetObject("mResetTicksToolStripButton.Image")));
            mResetTicksToolStripButton.ImageTransparentColor = Color.Magenta;
            mResetTicksToolStripButton.Name = "mResetTicksToolStripButton";
            mResetTicksToolStripButton.Size = new Size(39, 22);
            mResetTicksToolStripButton.Text = "&Reset";
            mResetTicksToolStripButton.ToolTipText = "Reset the tick counter";
            mResetTicksToolStripButton.Click += mResetTicksButton_Click;
            // 
            // mTickToolStripButton
            // 
            mTickToolStripButton.Image = ((Image)(resources.GetObject("mTickToolStripButton.Image")));
            mTickToolStripButton.ImageTransparentColor = Color.Magenta;
            mTickToolStripButton.Name = "mTickToolStripButton";
            mTickToolStripButton.Size = new Size(49, 22);
            mTickToolStripButton.Text = "T&ick";
            mTickToolStripButton.ToolTipText = "Execute one tick";
            mTickToolStripButton.Click += mTickItem_Click;
            // 
            // mStepToolStripButton
            // 
            mStepToolStripButton.Image = ((Image)(resources.GetObject("mStepToolStripButton.Image")));
            mStepToolStripButton.ImageTransparentColor = Color.Magenta;
            mStepToolStripButton.Name = "mStepToolStripButton";
            mStepToolStripButton.Size = new Size(50, 22);
            mStepToolStripButton.Text = "Ste&p";
            mStepToolStripButton.ToolTipText = "Execute one step";
            mStepToolStripButton.Click += mStepItem_Click;
            // 
            // mRunToolStripButton
            // 
            mRunToolStripButton.Image = ((Image)(resources.GetObject("mRunToolStripButton.Image")));
            mRunToolStripButton.ImageTransparentColor = Color.Magenta;
            mRunToolStripButton.Name = "mRunToolStripButton";
            mRunToolStripButton.Size = new Size(48, 22);
            mRunToolStripButton.Text = "R&un";
            mRunToolStripButton.ToolTipText = "Start or stop a full run";
            mRunToolStripButton.Click += mRunItem_Click;
            // 
            // mGoToolStripButton
            // 
            mGoToolStripButton.Image = ((Image)(resources.GetObject("mGoToolStripButton.Image")));
            mGoToolStripButton.ImageTransparentColor = Color.Magenta;
            mGoToolStripButton.Name = "mGoToolStripButton";
            mGoToolStripButton.Size = new Size(42, 22);
            mGoToolStripButton.Text = "&Go";
            mGoToolStripButton.ToolTipText = "Start the MIX loader";
            mGoToolStripButton.Click += mGoItem_Click;
            // 
            // mDetachToolStripButton
            // 
            mDetachToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mDetachToolStripButton.Image = ((Image)(resources.GetObject("mDetachToolStripButton.Image")));
            mDetachToolStripButton.ImageTransparentColor = Color.Magenta;
            mDetachToolStripButton.Name = "mDetachToolStripButton";
            mDetachToolStripButton.Size = new Size(48, 22);
            mDetachToolStripButton.Text = "Detac&h";
            mDetachToolStripButton.ToolTipText = "Switch between running MIX in the foreground or background";
            mDetachToolStripButton.Click += mDetachItem_Click;
            // 
            // mClearBreakpointsToolStripButton
            // 
            mClearBreakpointsToolStripButton.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripButton.Image")));
            mClearBreakpointsToolStripButton.ImageTransparentColor = Color.Magenta;
            mClearBreakpointsToolStripButton.Name = "mClearBreakpointsToolStripButton";
            mClearBreakpointsToolStripButton.Size = new Size(119, 22);
            mClearBreakpointsToolStripButton.Text = "Clear &Breakpoints";
            mClearBreakpointsToolStripButton.ToolTipText = "Remove all set breakpoints";
            mClearBreakpointsToolStripButton.Click += mClearBreakpointsItem_Click;
            // 
            // mResetToolStripButton
            // 
            mResetToolStripButton.Image = ((Image)(resources.GetObject("mResetToolStripButton.Image")));
            mResetToolStripButton.ImageTransparentColor = Color.Magenta;
            mResetToolStripButton.Name = "mResetToolStripButton";
            mResetToolStripButton.Size = new Size(55, 22);
            mResetToolStripButton.Text = "R&eset";
            mResetToolStripButton.ToolTipText = "Reset MIX to initial state";
            mResetToolStripButton.Click += mResetItem_Click;
            // 
            // mTeletypeToolStripButton
            // 
            mTeletypeToolStripButton.Image = ((Image)(resources.GetObject("mTeletypeToolStripButton.Image")));
            mTeletypeToolStripButton.ImageTransparentColor = Color.Magenta;
            mTeletypeToolStripButton.Name = "mTeletypeToolStripButton";
            mTeletypeToolStripButton.Size = new Size(104, 22);
            mTeletypeToolStripButton.Text = "Show &Teletype";
            mTeletypeToolStripButton.ToolTipText = "Show or hide the teletype";
            mTeletypeToolStripButton.Click += mTeletypeItem_Click;
            // 
            // mToolTip
            // 
            mToolTip.AutoPopDelay = 10000;
            mToolTip.InitialDelay = 100;
            mToolTip.ReshowDelay = 100;
            mToolTip.ShowAlways = true;
            // 
            // mProfilingResetCountsMenuItem
            // 
            mProfilingResetCountsMenuItem.Name = "mProfilingResetCountsMenuItem";
            mProfilingResetCountsMenuItem.Size = new Size(185, 22);
            mProfilingResetCountsMenuItem.Text = "&Reset counts";
            mProfilingResetCountsMenuItem.Click += mProfilingResetCountsMenuItem_Click;
            // 
            // MixForm
            // 
            ClientSize = new Size(804, 562);
            Controls.Add(mToolStripContainer);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            KeyPreview = true;
            MainMenuStrip = mMainMenuStrip;
            MinimumSize = new Size(820, 600);
            Name = "MixForm";
            StartPosition = FormStartPosition.Manual;
            Text = "MixEmul";
            FormClosing += this_FormClosing;
            ResizeBegin += this_ResizeBegin;
            ResizeEnd += MixForm_ResizeEnd;
            mMainMenuStrip.ResumeLayout(false);
            mMainMenuStrip.PerformLayout();
            mToolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            mToolStripContainer.BottomToolStripPanel.PerformLayout();
            mToolStripContainer.ContentPanel.ResumeLayout(false);
            mToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            mToolStripContainer.TopToolStripPanel.PerformLayout();
            mToolStripContainer.ResumeLayout(false);
            mToolStripContainer.PerformLayout();
            mStatusStrip.ResumeLayout(false);
            mStatusStrip.PerformLayout();
            mSplitContainer.Panel1.ResumeLayout(false);
            mSplitContainer.Panel2.ResumeLayout(false);
            mSplitContainer.ResumeLayout(false);
            mMemoryGroup.ResumeLayout(false);
            mMemoryTabControl.ResumeLayout(false);
            mMainMemoryTab.ResumeLayout(false);
            mRegistersGroup.ResumeLayout(false);
            mDevicesGroup.ResumeLayout(false);
            mSymbolGroup.ResumeLayout(false);
            mLogListGroup.ResumeLayout(false);
            mControlToolStrip.ResumeLayout(false);
            mControlToolStrip.PerformLayout();
            ResumeLayout(false);

        }

        void loadProgram(string filePath)
        {
            if (filePath == null) return;

            PreInstruction[] instructions;
            AssemblyFindingCollection findings;
            SymbolCollection symbols;

            InstructionInstanceBase[] instances = Assembler.Assemble(File.ReadAllLines(filePath), out instructions, out symbols, out findings);
            if (instructions != null && findings != null)
            {
                if (mSourceAndFindingsForm == null)
                {
                    mSourceAndFindingsForm = new SourceAndFindingsForm();
                    mSourceAndFindingsForm.SeverityImageList = mSeverityImageList;
                }

                mSourceAndFindingsForm.SetInstructionsAndFindings(instructions, instances, findings);

                if (mSourceAndFindingsForm.ShowDialog(this) != DialogResult.OK) return;
            }

            if (instances != null)
            {
                mSymbolListView.Symbols = symbols;
                mMainMemoryEditor.Symbols = symbols;
                mMix.LoadInstructionInstances(instances, symbols);
                Update();
            }
        }

        [STAThread]
        static void Main()
        {
            Application.ThreadException += MixForm.application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += MixForm.currentDomain_UnhandledException;
            var mixForm = new MixForm();
            Application.Run(mixForm);
        }


        void switchRunningState()
        {
            if (mSteppingState == steppingState.Idle)
            {
                setSteppingState(steppingState.Stepping);
                mRunning = true;
                mMix.StartRun();
            }
            else
            {
                mRunning = false;
                mMix.RequestStop();
            }
        }

        void pcChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
        {
            if (!mUpdating) implementPCChange((int)args.NewValue);
        }

        void setSteppingState(steppingState state)
        {
            if (mSteppingState != state)
            {
                mSteppingState = state;

                if (mSteppingState == steppingState.Idle)
                {
                    ReadOnly = false;
                    mRunToolStripButton.Text = "R&un";
                    mRunToolStripMenuItem.Text = "R&un";
                }
                else
                {
                    ReadOnly = true;
                    mRunToolStripButton.Text = "St&op";
                    mRunToolStripMenuItem.Text = "St&op";
                }
            }
        }

        void switchTeletypeVisibility()
        {
            if (mTeletype.Visible)
            {
                mTeletype.Hide();
            }
            else
            {
                mTeletype.Show();
            }
        }

        public new void Update()
		{
			mUpdating = true;
			bool pcVisible = mMainMemoryEditor.IsAddressVisible((int)mPCBox.LongValue);
			setPCBoxBounds();
			mPCBox.LongValue = mMix.ProgramCounter;
			mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
			if (mFloatingPointMemoryEditor != null)
			{
				bool floatPcVisible = mFloatingPointMemoryEditor.IsAddressVisible(mFloatingPointMemoryEditor.MarkedAddress);
				mFloatingPointMemoryEditor.MarkedAddress = mMix.FloatingPointModule.ProgramCounter;
				if (floatPcVisible)
				{
					mFloatingPointMemoryEditor.MakeAddressVisible(mFloatingPointMemoryEditor.MarkedAddress);
				}
			}
			mTicksToolStripTextBox.Text = mMix.TickCounter.ToString();

			switch (mMix.Status)
			{
				case ModuleBase.RunStatus.InvalidInstruction:
					mToolStripStatusLabel.Text = "Encountered invalid instruction";
					pcVisible = true;
					break;

				case ModuleBase.RunStatus.RuntimeError:
					mToolStripStatusLabel.Text = "Runtime error occured";
					pcVisible = true;
					break;

				case ModuleBase.RunStatus.BreakpointReached:
					mToolStripStatusLabel.Text = "Breakpoint reached";
					break;

				default:
					mToolStripStatusLabel.Text = mMix.Status.ToString();
					break;
			}

			if (pcVisible)
			{
				mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter, mMix.Status == ModuleBase.RunStatus.Idle);
			}

			mMix.NeutralizeStatus();

			mModeCycleButton.Value = mMix.Mode;

			mRegistersEditor.Update();
			mMainMemoryEditor.Update();
			mFloatingPointMemoryEditor?.Update();
			mDevicesControl.Update();
			base.Update();
			mUpdating = false;
		}

        void updateDeviceConfig()
        {
            DeviceSettings.DeviceFilesDirectory = mConfiguration.DeviceFilesDirectory;
            DeviceSettings.TickCounts = mConfiguration.TickCounts;
            DeviceSettings.DeviceReloadInterval = mConfiguration.DeviceReloadInterval;
            mMix.Devices.UpdateSettings();

            foreach (MixDevice device in mMix.Devices)
            {
                if (!(device is FileBasedDevice)) continue;

                var fileBasedDevice = (FileBasedDevice)device;

                if (mConfiguration.DeviceFilePaths.ContainsKey(device.Id))
                {
                    fileBasedDevice.FilePath = mConfiguration.DeviceFilePaths[device.Id];
                    continue;
                }

                fileBasedDevice.FilePath = null;
            }
        }

        public void UpdateLayout()
		{
			GuiSettings.Colors = mConfiguration.Colors;
			mPCBox.UpdateLayout();
			mTeletype.UpdateLayout();
			mRegistersEditor.UpdateLayout();
			foreach (MemoryEditor editor in mMemoryEditors) editor?.UpdateLayout();
            mDevicesControl.UpdateLayout();
			mSourceAndFindingsForm?.UpdateLayout();
			mPreferencesForm?.UpdateLayout();
			mDeviceEditor.UpdateLayout();
		}

		public bool ReadOnly
		{
			get
			{
				return mReadOnly;
			}
			set
			{
				if (mReadOnly != value)
				{
					mReadOnly = value;
					mPCBox.ReadOnly = mReadOnly;
					mResetTicksToolStripButton.Enabled = !mReadOnly;
					mStepToolStripButton.Enabled = !mReadOnly;
					mStepToolStripMenuItem.Enabled = !ReadOnly;
					mTickToolStripButton.Enabled = !mReadOnly;
					mTickToolStripMenuItem.Enabled = !mReadOnly;
					mGoToolStripButton.Enabled = !mReadOnly;
					mGoToolStripMenuItem.Enabled = !mReadOnly;
					mResetToolStripButton.Enabled = !mReadOnly;
					mResetToolStripMenuItem.Enabled = !mReadOnly;
					mRegistersEditor.ReadOnly = mReadOnly;
					foreach (MemoryEditor editor in mMemoryEditors)
					{
						if (editor != null)
						{
							editor.ReadOnly = mReadOnly;
						}
					}
					mMainMemoryEditor.ReadOnly = mReadOnly;
					mOpenProgramToolStripMenuItem.Enabled = !mReadOnly;
					mPreferencesToolStripMenuItem.Enabled = !mReadOnly;
				}
			}
		}

        enum steppingState
        {
            Idle,
            Stepping
        }

        void implementFloatingPointPCChange(int address)
        {
            if (mFloatingPointMemoryEditor != null && mFloatingPointMemoryEditor.IsAddressVisible(mMix.FloatingPointModule.ProgramCounter))
            {
                mFloatingPointMemoryEditor.MakeAddressVisible(address, mMix.Status == ModuleBase.RunStatus.Idle);
            }

            mMix.FloatingPointModule.ProgramCounter = address;

            if (mFloatingPointMemoryEditor != null)
            {
                mFloatingPointMemoryEditor.MarkedAddress = mMix.FloatingPointModule.ProgramCounter;
            }
        }

    }
}
