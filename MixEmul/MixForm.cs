using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MixAssembler;
using MixAssembler.Finding;
using MixAssembler.Instruction;
using MixGui.Components;
using MixGui.Events;
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
	public class MixForm : Form
	{
		private const int mainMemoryEditorIndex = 0;
		private const int floatingPointMemoryEditorIndex = 1;

		private IContainer components;
		private AboutForm mAboutForm;
		private Configuration mConfiguration;
		private string mDefaultDirectory = Environment.CurrentDirectory;
		private Mix mMix;
		private OpenFileDialog mOpenProgramFileDialog;
		private PreferencesForm mPreferencesForm;
		private bool mReadOnly;
		private bool mRunning;
		private bool mUpdating;
		private ImageList mSeverityImageList;
		private SourceAndFindingsForm mSourceAndFindingsForm;
		private steppingState mSteppingState;
		private TeletypeForm mTeletype;
		private MenuStrip mMainMenuStrip;
		private ToolStripMenuItem mFileToolStripMenuItem;
		private ToolStripMenuItem mOpenProgramToolStripMenuItem;
		private ToolStripMenuItem mExitToolStripMenuItem;
		private ToolStripMenuItem mToolsToolStripMenuItem;
		private ToolStripMenuItem mPreferencesToolStripMenuItem;
		private ToolStripMenuItem mHelpToolStripMenuItem;
		private ToolStripMenuItem mAboutToolStripMenuItem;
		private ToolStripMenuItem mActionsToolStripMenuItem;
		private ToolStripMenuItem mTickToolStripMenuItem;
		private ToolStripMenuItem mStepToolStripMenuItem;
		private ToolStripMenuItem mRunToolStripMenuItem;
		private ToolStripMenuItem mGoToolStripMenuItem;
		private ToolStripMenuItem mDetachToolStripMenuItem;
		private ToolStripMenuItem mResetToolStripMenuItem;
		private ToolStripMenuItem mViewToolStripMenuItem;
		private ToolStripMenuItem mTeletypeToolStripMenuItem;
		private ToolStripContainer mToolStripContainer;
		private StatusStrip mStatusStrip;
		private ToolStripStatusLabel mToolStripStatusLabel;
		private LongValueTextBox mPCBox;
		private ToolStrip mControlToolStrip;
		private ToolStripLabel mPCToolStripLabel;
		private ToolStripButton mShowPCToolStripButton;
		private ToolStripLabel mTicksToolStripLabel;
		private ToolStripTextBox mTicksToolStripTextBox;
		private ToolStripButton mResetTicksToolStripButton;
		private ToolStripButton mTickToolStripButton;
		private ToolStripButton mStepToolStripButton;
		private ToolStripButton mRunToolStripButton;
		private ToolStripButton mGoToolStripButton;
		private ToolStripButton mDetachToolStripButton;
		private ToolStripButton mResetToolStripButton;
		private ToolStripButton mTeletypeToolStripButton;
		private SplitContainer mSplitContainer;
		private GroupBox mMemoryGroup;
		private MemoryEditor mMainMemoryEditor;
		private MemoryEditor mFloatingPointMemoryEditor;
		private MemoryEditor[] mMemoryEditors;
		private GroupBox mRegistersGroup;
		private RegistersEditor mRegistersEditor;
		private GroupBox mDevicesGroup;
		private DevicesControl mDevicesControl;
		private GroupBox mSymbolGroup;
		private SymbolListView mSymbolListView;
		private GroupBox mLogListGroup;
		private LogListView mLogListView;
		private ToolStripButton mClearBreakpointsToolStripButton;
		private ToolStripMenuItem mClearBreakpointsToolStripMenuItem;
		private Button mDeviceEditorButton;
		private ToolStripMenuItem mDeviceEditorToolStripMenuItem;
		private ToolStripCycleButton mModeCycleButton;
		private TabControl mMemoryTabControl;
		private TabPage mMainMemoryTab;
		private TabPage mFloatingPointMemoryTab;
		private ToolStripStatusLabel mModeToolStripStatusLabel;
		private ToolStripStatusLabel mStatusToolStripStatusLabel;
		private ToolTip mToolTip;
		private ToolStripSeparator toolStripSeparator8;
		private ToolStripMenuItem mFindMenuItem;
		private ToolStripMenuItem mFindNextMenuItem;
		private DeviceEditorForm mDeviceEditor;
		private SearchDialog mSearchDialog;
		private ToolStripSeparator toolStripSeparator9;
		private ToolStripMenuItem mProfilingEnabledMenuItem;
		private ToolStripMenuItem mProfilingShowTickCountsMenuItem;
		private ToolStripMenuItem mProfilingResetCountsMenuItem;
		private SearchParameters mSearchOptions;

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

			DevicesControl.LayoutStructure layoutStructure = new DevicesControl.LayoutStructure(DevicesControl.Orientations.Horizontal, 8, 4);
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

			SymbolCollection symbols = new SymbolCollection();
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
				base.Location = mConfiguration.MainWindowLocation;
			}

			if (mConfiguration.MainWindowSize != Size.Empty)
			{
				base.Size = mConfiguration.MainWindowSize;
			}

			base.WindowState = mConfiguration.MainWindowState;
			base.SizeChanged += this_SizeChanged;
			base.LocationChanged += this_LocationChanged;
		}

        private MemoryEditor activeMemoryEditor => mMemoryEditors[mMemoryTabControl.SelectedIndex];

        private void findNext() => activeMemoryEditor.FindMatch(mSearchOptions);

        private int calculateIndexedAddress(int address, int index) => 
            InstructionHelpers.GetValidIndexedAddress(mMix, address, index, false);

        private void mExitMenuItem_Click(object sender, EventArgs e) => Close();

        private static void application_ThreadException(object sender, ThreadExceptionEventArgs e) => handleException(e.Exception);

        private static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) => 
            handleException((Exception)e.ExceptionObject);

        private void mRunItem_Click(object sender, EventArgs e) => switchRunningState();

        private void mShowPCButton_Click(object sender, EventArgs e) => mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter);

        private void mTeletypeItem_Click(object sender, EventArgs e) => switchTeletypeVisibility();

        void mFloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args) => 
            implementFloatingPointPCChange(args.SelectedAddress);

        void mDevicesControl_DeviceDoubleClick(object sender, DeviceEventArgs e)
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

		void mModeCycleButton_ValueChanged(object sender, EventArgs e)
		{
			int pc = mMix.ProgramCounter;
			mMix.Mode = (ModuleBase.RunMode)mModeCycleButton.Value;
			setPCBoxBounds();

			if (pc != mMix.ProgramCounter)
			{
				if (mMainMemoryEditor.IsAddressVisible(pc))
				{
					mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter, mMix.Status == Mix.RunStatus.Idle);
				}

				mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
				mPCBox.LongValue = mMix.ProgramCounter;
			}
		}

		private void setPCBoxBounds()
		{
			mPCBox.MinValue = mMix.Memory.MinWordIndex;
			mPCBox.MaxValue = mMix.Memory.MaxWordIndex;
		}

		private void applyFloatingPointSettings()
		{
			mMix.SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);
			if (ModuleSettings.FloatingPointEnabled)
			{
				loadModuleProgram(mMix.FloatingPointModule, ModuleSettings.FloatingPointProgramFile, "floating point");
			}
		}

		private void loadControlProgram()
		{
			string controlProgramFileName = ModuleSettings.ControlProgramFile;

			if (string.IsNullOrEmpty(controlProgramFileName) || !File.Exists(controlProgramFileName))
			{
				return;
			}

			loadModuleProgram(mMix, controlProgramFileName, "control");
			mMix.FullMemory.ClearSourceLines();
		}

		private SymbolCollection loadModuleProgram(ModuleBase module, string fileName, string programName)
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

		void mDeviceEditor_VisibleChanged(object sender, EventArgs e)
		{
			mDeviceEditorButton.Text = mDeviceEditor.Visible ? "Hide E&ditor" : "Show E&ditor";
			mDeviceEditorToolStripMenuItem.Text = mDeviceEditor.Visible ? "Hide &Device Editor" : "Show &Device Editor";
		}

		private bool disableTeletypeOnTop()
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

		private static void handleException(Exception exception)
		{
			Exception handlingException = null;
			string path = Path.Combine(Environment.CurrentDirectory, "exception.log");

			try
			{
				StreamWriter writer = new StreamWriter(path, true);
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

		private void implementPCChange(int address)
		{
			if (mMainMemoryEditor.IsAddressVisible(mMix.ProgramCounter))
			{
				mMainMemoryEditor.MakeAddressVisible(address, mMix.Status == Mix.RunStatus.Idle);
			}

			mMix.ProgramCounter = address;
			mMainMemoryEditor.MarkedAddress = mMix.ProgramCounter;
			mPCBox.LongValue = mMix.ProgramCounter;
		}

		private void InitializeComponent()
		{
            components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator fileMenuToolStripSeparator;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MixForm));
            Registers registers1 = new Registers();
            mSeverityImageList = new System.Windows.Forms.ImageList(components);
            mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mOpenProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mTeletypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mDeviceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            mFindMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mFindNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mTickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mGoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mDetachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mClearBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            mProfilingEnabledMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mProfilingShowTickCountsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            mStatusStrip = new System.Windows.Forms.StatusStrip();
            mModeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            mModeCycleButton = new MixGui.Components.ToolStripCycleButton(components);
            mStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            mToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            mSplitContainer = new System.Windows.Forms.SplitContainer();
            mMemoryGroup = new GroupBox();
            mMemoryTabControl = new System.Windows.Forms.TabControl();
            mMainMemoryTab = new System.Windows.Forms.TabPage();
            mMainMemoryEditor = new MixGui.Components.MemoryEditor();
            mFloatingPointMemoryTab = new System.Windows.Forms.TabPage();
            mRegistersGroup = new GroupBox();
            mRegistersEditor = new MixGui.Components.RegistersEditor();
            mDevicesGroup = new GroupBox();
            mDeviceEditorButton = new Button();
            mDevicesControl = new MixGui.Components.DevicesControl();
            mSymbolGroup = new GroupBox();
            mSymbolListView = new MixGui.Components.SymbolListView();
            mLogListGroup = new GroupBox();
            mLogListView = new MixGui.Components.LogListView();
            mControlToolStrip = new System.Windows.Forms.ToolStrip();
            mPCToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            mShowPCToolStripButton = new System.Windows.Forms.ToolStripButton();
            mTicksToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            mTicksToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            mResetTicksToolStripButton = new System.Windows.Forms.ToolStripButton();
            mTickToolStripButton = new System.Windows.Forms.ToolStripButton();
            mStepToolStripButton = new System.Windows.Forms.ToolStripButton();
            mRunToolStripButton = new System.Windows.Forms.ToolStripButton();
            mGoToolStripButton = new System.Windows.Forms.ToolStripButton();
            mDetachToolStripButton = new System.Windows.Forms.ToolStripButton();
            mClearBreakpointsToolStripButton = new System.Windows.Forms.ToolStripButton();
            mResetToolStripButton = new System.Windows.Forms.ToolStripButton();
            mTeletypeToolStripButton = new System.Windows.Forms.ToolStripButton();
            mToolTip = new System.Windows.Forms.ToolTip(components);
            mProfilingResetCountsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			fileMenuToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
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
            mSeverityImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mSeverityImageList.ImageStream")));
            mSeverityImageList.TransparentColor = Color.Transparent;
            mSeverityImageList.Images.SetKeyName(0, "");
            mSeverityImageList.Images.SetKeyName(1, "");
            mSeverityImageList.Images.SetKeyName(2, "");
            mSeverityImageList.Images.SetKeyName(3, "");
            // 
            // mMainMenuStrip
            // 
            mMainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mOpenProgramToolStripMenuItem,
            fileMenuToolStripSeparator,
            mExitToolStripMenuItem});
            mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            mFileToolStripMenuItem.Size = new Size(37, 20);
            mFileToolStripMenuItem.Text = "&File";
            // 
            // mOpenProgramToolStripMenuItem
            // 
            mOpenProgramToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mOpenProgramToolStripMenuItem.Image")));
            mOpenProgramToolStripMenuItem.Name = "mOpenProgramToolStripMenuItem";
            mOpenProgramToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O);
            mOpenProgramToolStripMenuItem.Size = new Size(204, 22);
            mOpenProgramToolStripMenuItem.Text = "&Open program...";
            mOpenProgramToolStripMenuItem.Click += new EventHandler(mOpenProgramMenuItem_Click);
            // 
            // mExitToolStripMenuItem
            // 
            mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
            mExitToolStripMenuItem.Size = new Size(204, 22);
            mExitToolStripMenuItem.Text = "E&xit";
            mExitToolStripMenuItem.Click += new EventHandler(mExitMenuItem_Click);
            // 
            // mViewToolStripMenuItem
            // 
            mViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mTeletypeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mTeletypeToolStripMenuItem.Image")));
            mTeletypeToolStripMenuItem.Name = "mTeletypeToolStripMenuItem";
            mTeletypeToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T);
            mTeletypeToolStripMenuItem.Size = new Size(215, 22);
            mTeletypeToolStripMenuItem.Text = "&Show Teletype";
            mTeletypeToolStripMenuItem.Click += new EventHandler(mTeletypeItem_Click);
            // 
            // mDeviceEditorToolStripMenuItem
            // 
            mDeviceEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mDeviceEditorToolStripMenuItem.Image")));
            mDeviceEditorToolStripMenuItem.Name = "mDeviceEditorToolStripMenuItem";
            mDeviceEditorToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E);
            mDeviceEditorToolStripMenuItem.Size = new Size(215, 22);
            mDeviceEditorToolStripMenuItem.Text = "Show &Device Editor";
            mDeviceEditorToolStripMenuItem.Click += new EventHandler(mDeviceEditorItem_Click);
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(212, 6);
            // 
            // mFindMenuItem
            // 
            mFindMenuItem.Image = global::MixGui.Properties.Resources.Find_5650;
            mFindMenuItem.Name = "mFindMenuItem";
            mFindMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F);
            mFindMenuItem.Size = new Size(215, 22);
            mFindMenuItem.Text = "Find in memory...";
            mFindMenuItem.Click += new EventHandler(mFindMenuItem_Click);
            // 
            // mFindNextMenuItem
            // 
            mFindNextMenuItem.Name = "mFindNextMenuItem";
            mFindNextMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            mFindNextMenuItem.Size = new Size(215, 22);
            mFindNextMenuItem.Text = "Find next";
            mFindNextMenuItem.Click += new EventHandler(mFindNextMenuItem_Click);
            // 
            // mActionsToolStripMenuItem
            // 
            mActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mTickToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mTickToolStripMenuItem.Image")));
            mTickToolStripMenuItem.Name = "mTickToolStripMenuItem";
            mTickToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            mTickToolStripMenuItem.Size = new Size(207, 22);
            mTickToolStripMenuItem.Text = "T&ick";
            mTickToolStripMenuItem.Click += new EventHandler(mTickItem_Click);
            // 
            // mStepToolStripMenuItem
            // 
            mStepToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mStepToolStripMenuItem.Image")));
            mStepToolStripMenuItem.Name = "mStepToolStripMenuItem";
            mStepToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            mStepToolStripMenuItem.Size = new Size(207, 22);
            mStepToolStripMenuItem.Text = "&Step";
            mStepToolStripMenuItem.Click += new EventHandler(mStepItem_Click);
            // 
            // mRunToolStripMenuItem
            // 
            mRunToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mRunToolStripMenuItem.Image")));
            mRunToolStripMenuItem.Name = "mRunToolStripMenuItem";
            mRunToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            mRunToolStripMenuItem.Size = new Size(207, 22);
            mRunToolStripMenuItem.Text = "R&un";
            mRunToolStripMenuItem.Click += new EventHandler(mRunItem_Click);
            // 
            // mGoToolStripMenuItem
            // 
            mGoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mGoToolStripMenuItem.Image")));
            mGoToolStripMenuItem.Name = "mGoToolStripMenuItem";
            mGoToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5);
            mGoToolStripMenuItem.Size = new Size(207, 22);
            mGoToolStripMenuItem.Text = "&Go";
            mGoToolStripMenuItem.Click += new EventHandler(mGoItem_Click);
            // 
            // mDetachToolStripMenuItem
            // 
            mDetachToolStripMenuItem.Name = "mDetachToolStripMenuItem";
            mDetachToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D);
            mDetachToolStripMenuItem.Size = new Size(207, 22);
            mDetachToolStripMenuItem.Text = "&Detach";
            mDetachToolStripMenuItem.Click += new EventHandler(mDetachItem_Click);
            // 
            // mClearBreakpointsToolStripMenuItem
            // 
            mClearBreakpointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mClearBreakpointsToolStripMenuItem.Image")));
            mClearBreakpointsToolStripMenuItem.Name = "mClearBreakpointsToolStripMenuItem";
            mClearBreakpointsToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B);
            mClearBreakpointsToolStripMenuItem.Size = new Size(207, 22);
            mClearBreakpointsToolStripMenuItem.Text = "Clear &Breakpoints";
            mClearBreakpointsToolStripMenuItem.Click += new EventHandler(mClearBreakpointsItem_Click);
            // 
            // mResetToolStripMenuItem
            // 
            mResetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mResetToolStripMenuItem.Image")));
            mResetToolStripMenuItem.Name = "mResetToolStripMenuItem";
            mResetToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R);
            mResetToolStripMenuItem.Size = new Size(207, 22);
            mResetToolStripMenuItem.Text = "&Reset";
            mResetToolStripMenuItem.Click += new EventHandler(mResetItem_Click);
            // 
            // mToolsToolStripMenuItem
            // 
            mToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mPreferencesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mPreferencesToolStripMenuItem.Image")));
            mPreferencesToolStripMenuItem.Name = "mPreferencesToolStripMenuItem";
            mPreferencesToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P);
            mPreferencesToolStripMenuItem.Size = new Size(185, 22);
            mPreferencesToolStripMenuItem.Text = "&Preferences...";
            mPreferencesToolStripMenuItem.Click += new EventHandler(mPreferencesMenuItem_Click);
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
            mProfilingEnabledMenuItem.CheckedChanged += new EventHandler(mProfilingEnabledMenuItem_CheckedChanged);
            // 
            // mProfilingShowTickCountsMenuItem
            // 
            mProfilingShowTickCountsMenuItem.CheckOnClick = true;
            mProfilingShowTickCountsMenuItem.Name = "mProfilingShowTickCountsMenuItem";
            mProfilingShowTickCountsMenuItem.Size = new Size(185, 22);
            mProfilingShowTickCountsMenuItem.Text = "&Show tick counts";
            mProfilingShowTickCountsMenuItem.CheckedChanged += new EventHandler(mProfilingShowTickCountsMenuItem_CheckedChanged);
            // 
            // mHelpToolStripMenuItem
            // 
            mHelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mAboutToolStripMenuItem});
            mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
            mHelpToolStripMenuItem.Size = new Size(44, 20);
            mHelpToolStripMenuItem.Text = "&Help";
            // 
            // mAboutToolStripMenuItem
            // 
            mAboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mAboutToolStripMenuItem.Image")));
            mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
            mAboutToolStripMenuItem.Size = new Size(116, 22);
            mAboutToolStripMenuItem.Text = "&About...";
            mAboutToolStripMenuItem.Click += new EventHandler(mAboutMenuItem_Click);
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
            mToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
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
            mStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            mStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mSplitContainer.BackColor = System.Drawing.SystemColors.Control;
            mSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            mSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            mSplitContainer.Location = new Point(0, 0);
            mSplitContainer.Name = "mSplitContainer";
            mSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
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
            mMemoryTabControl.SelectedIndexChanged += new EventHandler(mMemoryTabControl_SelectedIndexChanged);
            mMemoryTabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(mMemoryTabControl_Selecting);
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
            mMainMemoryEditor.AddressSelected += new MixGui.Events.AddressSelectedHandler(mMemoryEditor_addressSelected);
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
            mDeviceEditorButton.Click += new EventHandler(mDeviceEditorItem_Click);
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
            mSymbolListView.AddressSelected += new MixGui.Events.AddressSelectedHandler(listViews_addressSelected);
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
            mLogListView.AddressSelected += new MixGui.Events.AddressSelectedHandler(listViews_addressSelected);
            // 
            // mControlToolStrip
            // 
            mControlToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            mControlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            mControlToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            mShowPCToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            mShowPCToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mShowPCToolStripButton.Image")));
            mShowPCToolStripButton.ImageTransparentColor = Color.Magenta;
            mShowPCToolStripButton.Name = "mShowPCToolStripButton";
            mShowPCToolStripButton.Size = new Size(40, 22);
            mShowPCToolStripButton.Text = "Sh&ow";
            mShowPCToolStripButton.ToolTipText = "Show the program counter";
            mShowPCToolStripButton.Click += new EventHandler(mShowPCButton_Click);
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
            mResetTicksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            mResetTicksToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mResetTicksToolStripButton.Image")));
            mResetTicksToolStripButton.ImageTransparentColor = Color.Magenta;
            mResetTicksToolStripButton.Name = "mResetTicksToolStripButton";
            mResetTicksToolStripButton.Size = new Size(39, 22);
            mResetTicksToolStripButton.Text = "&Reset";
            mResetTicksToolStripButton.ToolTipText = "Reset the tick counter";
            mResetTicksToolStripButton.Click += new EventHandler(mResetTicksButton_Click);
            // 
            // mTickToolStripButton
            // 
            mTickToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mTickToolStripButton.Image")));
            mTickToolStripButton.ImageTransparentColor = Color.Magenta;
            mTickToolStripButton.Name = "mTickToolStripButton";
            mTickToolStripButton.Size = new Size(49, 22);
            mTickToolStripButton.Text = "T&ick";
            mTickToolStripButton.ToolTipText = "Execute one tick";
            mTickToolStripButton.Click += new EventHandler(mTickItem_Click);
            // 
            // mStepToolStripButton
            // 
            mStepToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mStepToolStripButton.Image")));
            mStepToolStripButton.ImageTransparentColor = Color.Magenta;
            mStepToolStripButton.Name = "mStepToolStripButton";
            mStepToolStripButton.Size = new Size(50, 22);
            mStepToolStripButton.Text = "Ste&p";
            mStepToolStripButton.ToolTipText = "Execute one step";
            mStepToolStripButton.Click += new EventHandler(mStepItem_Click);
            // 
            // mRunToolStripButton
            // 
            mRunToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mRunToolStripButton.Image")));
            mRunToolStripButton.ImageTransparentColor = Color.Magenta;
            mRunToolStripButton.Name = "mRunToolStripButton";
            mRunToolStripButton.Size = new Size(48, 22);
            mRunToolStripButton.Text = "R&un";
            mRunToolStripButton.ToolTipText = "Start or stop a full run";
            mRunToolStripButton.Click += new EventHandler(mRunItem_Click);
            // 
            // mGoToolStripButton
            // 
            mGoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mGoToolStripButton.Image")));
            mGoToolStripButton.ImageTransparentColor = Color.Magenta;
            mGoToolStripButton.Name = "mGoToolStripButton";
            mGoToolStripButton.Size = new Size(42, 22);
            mGoToolStripButton.Text = "&Go";
            mGoToolStripButton.ToolTipText = "Start the MIX loader";
            mGoToolStripButton.Click += new EventHandler(mGoItem_Click);
            // 
            // mDetachToolStripButton
            // 
            mDetachToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            mDetachToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mDetachToolStripButton.Image")));
            mDetachToolStripButton.ImageTransparentColor = Color.Magenta;
            mDetachToolStripButton.Name = "mDetachToolStripButton";
            mDetachToolStripButton.Size = new Size(48, 22);
            mDetachToolStripButton.Text = "Detac&h";
            mDetachToolStripButton.ToolTipText = "Switch between running MIX in the foreground or background";
            mDetachToolStripButton.Click += new EventHandler(mDetachItem_Click);
            // 
            // mClearBreakpointsToolStripButton
            // 
            mClearBreakpointsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mClearBreakpointsToolStripButton.Image")));
            mClearBreakpointsToolStripButton.ImageTransparentColor = Color.Magenta;
            mClearBreakpointsToolStripButton.Name = "mClearBreakpointsToolStripButton";
            mClearBreakpointsToolStripButton.Size = new Size(119, 22);
            mClearBreakpointsToolStripButton.Text = "Clear &Breakpoints";
            mClearBreakpointsToolStripButton.ToolTipText = "Remove all set breakpoints";
            mClearBreakpointsToolStripButton.Click += new EventHandler(mClearBreakpointsItem_Click);
            // 
            // mResetToolStripButton
            // 
            mResetToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mResetToolStripButton.Image")));
            mResetToolStripButton.ImageTransparentColor = Color.Magenta;
            mResetToolStripButton.Name = "mResetToolStripButton";
            mResetToolStripButton.Size = new Size(55, 22);
            mResetToolStripButton.Text = "R&eset";
            mResetToolStripButton.ToolTipText = "Reset MIX to initial state";
            mResetToolStripButton.Click += new EventHandler(mResetItem_Click);
            // 
            // mTeletypeToolStripButton
            // 
            mTeletypeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mTeletypeToolStripButton.Image")));
            mTeletypeToolStripButton.ImageTransparentColor = Color.Magenta;
            mTeletypeToolStripButton.Name = "mTeletypeToolStripButton";
            mTeletypeToolStripButton.Size = new Size(104, 22);
            mTeletypeToolStripButton.Text = "Show &Teletype";
            mTeletypeToolStripButton.ToolTipText = "Show or hide the teletype";
            mTeletypeToolStripButton.Click += new EventHandler(mTeletypeItem_Click);
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
            mProfilingResetCountsMenuItem.Click += new EventHandler(mProfilingResetCountsMenuItem_Click);
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
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(this_FormClosing);
            ResizeBegin += new EventHandler(this_ResizeBegin);
            ResizeEnd += new EventHandler(MixForm_ResizeEnd);
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

		private void loadProgram(string filePath)
		{
			if (filePath == null)
			{
				return;
			}

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

				if (mSourceAndFindingsForm.ShowDialog(this) != DialogResult.OK)
				{
					return;
				}
			}

			if (instances != null)
			{
				mSymbolListView.Symbols = symbols;
				mMainMemoryEditor.Symbols = symbols;
				mMix.LoadInstructionInstances(instances, symbols);
				Update();
			}
		}

		private void mAboutMenuItem_Click(object sender, EventArgs e)
		{
			if (mAboutForm == null)
			{
				mAboutForm = new AboutForm();
			}

			bool teletypeWasOnTop = disableTeletypeOnTop();

			mAboutForm.ShowDialog(this);

			if (teletypeWasOnTop)
			{
				mTeletype.TopMost = true;
			}
		}

		[STAThread]
		private static void Main()
		{
			Application.ThreadException += new ThreadExceptionEventHandler(MixForm.application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MixForm.currentDomain_UnhandledException);
			MixForm mixForm = new MixForm();
			Application.Run(mixForm);
		}

		private void mDetachItem_Click(object sender, EventArgs e)
		{
			mMix.RunDetached = !mMix.RunDetached;
			mDetachToolStripButton.Text = mMix.RunDetached ? "Attac&h" : "Detac&h";
			mDetachToolStripMenuItem.Text = mMix.RunDetached ? "&Attach" : "&Detach";
		}

		private void listViews_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (sender == mLogListView)
			{
				mMemoryTabControl.SelectedIndex = mLogListView.SelectedModule == mMix.FloatingPointModule.ModuleName ? floatingPointMemoryEditorIndex : mainMemoryEditorIndex;
			}
			activeMemoryEditor.MakeAddressVisible(args.SelectedAddress);
		}

		private void mMemoryEditor_addressSelected(object sender, AddressSelectedEventArgs args)
		{
			if (!mReadOnly)
			{
				implementPCChange(args.SelectedAddress);
			}
		}

		private void mMix_InputRequired(object sender, EventArgs e)
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

		private void mMix_LogLineAdded(object sender, EventArgs e)
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

		private void mMix_StepPerformed(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(mMix_StepPerformed));
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
					setSteppingState(steppingState.Idle);
				}
			}
		}

		private void mOpenProgramMenuItem_Click(object sender, EventArgs e)
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

			if (teletypeWasOnTop)
			{
				mTeletype.TopMost = true;
			}
		}

		private void mPreferencesMenuItem_Click(object sender, EventArgs e)
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


			if (teletypeWasOnTop)
			{
				mTeletype.TopMost = true;
			}
		}

		private void mResetItem_Click(object sender, EventArgs e)
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

		private void mResetTicksButton_Click(object sender, EventArgs e)
		{
			mMix.ResetTickCounter();
			mTicksToolStripTextBox.Text = mMix.TickCounter.ToString();
		}

		private void switchRunningState()
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

		private void mStepItem_Click(object sender, EventArgs e)
		{
			setSteppingState(steppingState.Stepping);
			mMix.StartStep();
		}

		private void mTeletype_VisibleChanged(object sender, EventArgs e)
		{
			mTeletypeToolStripButton.Text = mTeletype.Visible ? "Hide &Teletype" : "Show &Teletype";
			mTeletypeToolStripMenuItem.Text = mTeletype.Visible ? "&Hide Teletype" : "&Show Teletype";
		}

        private void mTickItem_Click(object sender, EventArgs e)
		{
			mMix.Tick();
			Update();
		}

		private void pcChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			if (!mUpdating)
			{
				implementPCChange((int)args.NewValue);
			}
		}

		private void setSteppingState(steppingState state)
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

		private void switchTeletypeVisibility()
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

		private void this_FormClosing(object sender, FormClosingEventArgs e)
		{
			mConfiguration.MainWindowState = base.WindowState;
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

		private void this_LocationChanged(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Normal)
			{
				mConfiguration.MainWindowLocation = base.Location;
			}
		}

		private void this_SizeChanged(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Normal)
			{
				mConfiguration.MainWindowSize = base.Size;
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
				mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter, mMix.Status == Mix.RunStatus.Idle);
			}

			mMix.NeutralizeStatus();

			mModeCycleButton.Value = mMix.Mode;

			mRegistersEditor.Update();
			mMainMemoryEditor.Update();
			if (mFloatingPointMemoryEditor != null)
			{
				mFloatingPointMemoryEditor.Update();
			}
			mDevicesControl.Update();
			base.Update();
			mUpdating = false;
		}

		private void updateDeviceConfig()
		{
			DeviceSettings.DeviceFilesDirectory = mConfiguration.DeviceFilesDirectory;
			DeviceSettings.TickCounts = mConfiguration.TickCounts;
			DeviceSettings.DeviceReloadInterval = mConfiguration.DeviceReloadInterval;
			mMix.Devices.UpdateSettings();

			foreach (MixDevice device in mMix.Devices)
			{
				if (!(device is FileBasedDevice))
				{
					continue;
				}

				FileBasedDevice fileBasedDevice = (FileBasedDevice)device;

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
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.UpdateLayout();
				}
			}
			mDevicesControl.UpdateLayout();

			if (mSourceAndFindingsForm != null)
			{
				mSourceAndFindingsForm.UpdateLayout();
			}

			if (mPreferencesForm != null)
			{
				mPreferencesForm.UpdateLayout();
			}

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

		private enum steppingState
		{
			Idle,
			Stepping
		}

		private void this_ResizeBegin(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.ResizeInProgress = true;
				}
			}
		}

		private void MixForm_ResizeEnd(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.ResizeInProgress = false;
				}
			}
		}

		private void mClearBreakpointsItem_Click(object sender, EventArgs e)
		{
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.ClearBreakpoints();
				}
			}
		}

		private void mDeviceEditorItem_Click(object sender, EventArgs e)
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

		private void mGoItem_Click(object sender, EventArgs e)
		{
			if (mSteppingState == steppingState.Idle)
			{
				mMix.PrepareLoader();
				switchRunningState();
			}
		}

		private void mMemoryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
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

		private void implementFloatingPointPCChange(int address)
		{
			if (mFloatingPointMemoryEditor != null && mFloatingPointMemoryEditor.IsAddressVisible(mMix.FloatingPointModule.ProgramCounter))
			{
				mFloatingPointMemoryEditor.MakeAddressVisible(address, mMix.Status == Mix.RunStatus.Idle);
			}

			mMix.FloatingPointModule.ProgramCounter = address;

			if (mFloatingPointMemoryEditor != null)
			{
				mFloatingPointMemoryEditor.MarkedAddress = mMix.FloatingPointModule.ProgramCounter;
			}
		}

		private void mMemoryTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			MemoryEditor editor = activeMemoryEditor;
			mSymbolListView.Symbols = editor.Symbols;
			mSymbolListView.MemoryMinIndex = editor.Memory.MinWordIndex;
			mSymbolListView.MemoryMaxIndex = editor.Memory.MaxWordIndex;
		}

		private void mFindMenuItem_Click(object sender, EventArgs e)
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

		private void mFindNextMenuItem_Click(object sender, EventArgs e)
		{
			if (mSearchOptions == null)
			{
				mFindMenuItem_Click(sender, e);
				return;
			}

			findNext();
		}

		private void mProfilingEnabledMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ExecutionSettings.ProfilingEnabled = mProfilingEnabledMenuItem.Checked;
			mConfiguration.ProfilingEnabled = ExecutionSettings.ProfilingEnabled;

			mProfilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			mProfilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.UpdateLayout();
				}
			}
		}

		private void mProfilingShowTickCountsMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			GuiSettings.ShowProfilingInfo = mProfilingShowTickCountsMenuItem.Checked ? GuiSettings.ProfilingInfoType.Tick : GuiSettings.ProfilingInfoType.Execution;
			mConfiguration.ShowProfilingInfo = GuiSettings.ShowProfilingInfo;

			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.UpdateLayout();
				}
			}
		}

		private void mProfilingResetCountsMenuItem_Click(object sender, EventArgs e)
		{
			mMix.ResetProfilingCounts();
			foreach (MemoryEditor editor in mMemoryEditors)
			{
				if (editor != null)
				{
					editor.Update();
				}
			}
		}
	}
}
