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
			mPCBox.BackColor = System.Drawing.Color.White;
			mPCBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			mPCBox.ForeColor = System.Drawing.Color.Black;
			mPCBox.LongValue = mMix.ProgramCounter;
			mPCBox.ClearZero = false;
			setPCBoxBounds();
			mPCBox.Name = "mPCBox";
			mPCBox.Size = new System.Drawing.Size(40, 25);
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

		private MemoryEditor activeMemoryEditor
		{
			get
			{
				return mMemoryEditors[mMemoryTabControl.SelectedIndex];
			}
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

		private static void application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			handleException(e.Exception);
		}

		private static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			handleException((Exception)e.ExceptionObject);
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
                if (components != null)
                {
                    components.Dispose();
                }

                if (mMix != null)
                {
                    mMix.Dispose();
                }
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator fileMenuToolStripSeparator;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MixForm));
			MixLib.Registers registers1 = new MixLib.Registers();
			this.mSeverityImageList = new System.Windows.Forms.ImageList(this.components);
			this.mMainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mOpenProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mTeletypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mDeviceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.mFindMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mFindNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mTickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mGoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mDetachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mClearBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.mProfilingEnabledMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mProfilingShowTickCountsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mToolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.mStatusStrip = new System.Windows.Forms.StatusStrip();
			this.mModeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.mModeCycleButton = new MixGui.Components.ToolStripCycleButton(this.components);
			this.mStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.mToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.mSplitContainer = new System.Windows.Forms.SplitContainer();
			this.mMemoryGroup = new System.Windows.Forms.GroupBox();
			this.mMemoryTabControl = new System.Windows.Forms.TabControl();
			this.mMainMemoryTab = new System.Windows.Forms.TabPage();
			this.mMainMemoryEditor = new MixGui.Components.MemoryEditor();
			this.mFloatingPointMemoryTab = new System.Windows.Forms.TabPage();
			this.mRegistersGroup = new System.Windows.Forms.GroupBox();
			this.mRegistersEditor = new MixGui.Components.RegistersEditor();
			this.mDevicesGroup = new System.Windows.Forms.GroupBox();
			this.mDeviceEditorButton = new System.Windows.Forms.Button();
			this.mDevicesControl = new MixGui.Components.DevicesControl();
			this.mSymbolGroup = new System.Windows.Forms.GroupBox();
			this.mSymbolListView = new MixGui.Components.SymbolListView();
			this.mLogListGroup = new System.Windows.Forms.GroupBox();
			this.mLogListView = new MixGui.Components.LogListView();
			this.mControlToolStrip = new System.Windows.Forms.ToolStrip();
			this.mPCToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.mShowPCToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mTicksToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.mTicksToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
			this.mResetTicksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mTickToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mStepToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mRunToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mGoToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mDetachToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mClearBreakpointsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mResetToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mTeletypeToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.mProfilingResetCountsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			fileMenuToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.mMainMenuStrip.SuspendLayout();
			this.mToolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.mToolStripContainer.ContentPanel.SuspendLayout();
			this.mToolStripContainer.TopToolStripPanel.SuspendLayout();
			this.mToolStripContainer.SuspendLayout();
			this.mStatusStrip.SuspendLayout();
			this.mSplitContainer.Panel1.SuspendLayout();
			this.mSplitContainer.Panel2.SuspendLayout();
			this.mSplitContainer.SuspendLayout();
			this.mMemoryGroup.SuspendLayout();
			this.mMemoryTabControl.SuspendLayout();
			this.mMainMemoryTab.SuspendLayout();
			this.mRegistersGroup.SuspendLayout();
			this.mDevicesGroup.SuspendLayout();
			this.mSymbolGroup.SuspendLayout();
			this.mLogListGroup.SuspendLayout();
			this.mControlToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// fileMenuToolStripSeparator
			// 
			fileMenuToolStripSeparator.Name = "fileMenuToolStripSeparator";
			fileMenuToolStripSeparator.Size = new System.Drawing.Size(201, 6);
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator6
			// 
			toolStripSeparator6.Name = "toolStripSeparator6";
			toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator7
			// 
			toolStripSeparator7.Name = "toolStripSeparator7";
			toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
			// 
			// mSeverityImageList
			// 
			this.mSeverityImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mSeverityImageList.ImageStream")));
			this.mSeverityImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.mSeverityImageList.Images.SetKeyName(0, "");
			this.mSeverityImageList.Images.SetKeyName(1, "");
			this.mSeverityImageList.Images.SetKeyName(2, "");
			this.mSeverityImageList.Images.SetKeyName(3, "");
			// 
			// mMainMenuStrip
			// 
			this.mMainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mViewToolStripMenuItem,
            this.mActionsToolStripMenuItem,
            this.mToolsToolStripMenuItem,
            this.mHelpToolStripMenuItem});
			this.mMainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mMainMenuStrip.Name = "mMainMenuStrip";
			this.mMainMenuStrip.Size = new System.Drawing.Size(804, 24);
			this.mMainMenuStrip.TabIndex = 0;
			this.mMainMenuStrip.Text = "Main Menu";
			// 
			// mFileToolStripMenuItem
			// 
			this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOpenProgramToolStripMenuItem,
            fileMenuToolStripSeparator,
            this.mExitToolStripMenuItem});
			this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
			this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.mFileToolStripMenuItem.Text = "&File";
			// 
			// mOpenProgramToolStripMenuItem
			// 
			this.mOpenProgramToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mOpenProgramToolStripMenuItem.Image")));
			this.mOpenProgramToolStripMenuItem.Name = "mOpenProgramToolStripMenuItem";
			this.mOpenProgramToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.mOpenProgramToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.mOpenProgramToolStripMenuItem.Text = "&Open program...";
			this.mOpenProgramToolStripMenuItem.Click += new System.EventHandler(this.mOpenProgramMenuItem_Click);
			// 
			// mExitToolStripMenuItem
			// 
			this.mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
			this.mExitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.mExitToolStripMenuItem.Text = "E&xit";
			this.mExitToolStripMenuItem.Click += new System.EventHandler(this.mExitMenuItem_Click);
			// 
			// mViewToolStripMenuItem
			// 
			this.mViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mTeletypeToolStripMenuItem,
            this.mDeviceEditorToolStripMenuItem,
            this.toolStripSeparator8,
            this.mFindMenuItem,
            this.mFindNextMenuItem});
			this.mViewToolStripMenuItem.Name = "mViewToolStripMenuItem";
			this.mViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.mViewToolStripMenuItem.Text = "&View";
			// 
			// mTeletypeToolStripMenuItem
			// 
			this.mTeletypeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mTeletypeToolStripMenuItem.Image")));
			this.mTeletypeToolStripMenuItem.Name = "mTeletypeToolStripMenuItem";
			this.mTeletypeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
			this.mTeletypeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			this.mTeletypeToolStripMenuItem.Text = "&Show Teletype";
			this.mTeletypeToolStripMenuItem.Click += new System.EventHandler(this.mTeletypeItem_Click);
			// 
			// mDeviceEditorToolStripMenuItem
			// 
			this.mDeviceEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mDeviceEditorToolStripMenuItem.Image")));
			this.mDeviceEditorToolStripMenuItem.Name = "mDeviceEditorToolStripMenuItem";
			this.mDeviceEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.mDeviceEditorToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			this.mDeviceEditorToolStripMenuItem.Text = "Show &Device Editor";
			this.mDeviceEditorToolStripMenuItem.Click += new System.EventHandler(this.mDeviceEditorItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(212, 6);
			// 
			// mFindMenuItem
			// 
			this.mFindMenuItem.Image = global::MixGui.Properties.Resources.Find_5650;
			this.mFindMenuItem.Name = "mFindMenuItem";
			this.mFindMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.mFindMenuItem.Size = new System.Drawing.Size(215, 22);
			this.mFindMenuItem.Text = "Find in memory...";
			this.mFindMenuItem.Click += new System.EventHandler(this.mFindMenuItem_Click);
			// 
			// mFindNextMenuItem
			// 
			this.mFindNextMenuItem.Name = "mFindNextMenuItem";
			this.mFindNextMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.mFindNextMenuItem.Size = new System.Drawing.Size(215, 22);
			this.mFindNextMenuItem.Text = "Find next";
			this.mFindNextMenuItem.Click += new System.EventHandler(this.mFindNextMenuItem_Click);
			// 
			// mActionsToolStripMenuItem
			// 
			this.mActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mTickToolStripMenuItem,
            this.mStepToolStripMenuItem,
            this.mRunToolStripMenuItem,
            this.mGoToolStripMenuItem,
            toolStripSeparator1,
            this.mDetachToolStripMenuItem,
            toolStripSeparator2,
            this.mClearBreakpointsToolStripMenuItem,
            this.mResetToolStripMenuItem});
			this.mActionsToolStripMenuItem.Name = "mActionsToolStripMenuItem";
			this.mActionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.mActionsToolStripMenuItem.Text = "&Actions";
			// 
			// mTickToolStripMenuItem
			// 
			this.mTickToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mTickToolStripMenuItem.Image")));
			this.mTickToolStripMenuItem.Name = "mTickToolStripMenuItem";
			this.mTickToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
			this.mTickToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mTickToolStripMenuItem.Text = "T&ick";
			this.mTickToolStripMenuItem.Click += new System.EventHandler(this.mTickItem_Click);
			// 
			// mStepToolStripMenuItem
			// 
			this.mStepToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mStepToolStripMenuItem.Image")));
			this.mStepToolStripMenuItem.Name = "mStepToolStripMenuItem";
			this.mStepToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
			this.mStepToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mStepToolStripMenuItem.Text = "&Step";
			this.mStepToolStripMenuItem.Click += new System.EventHandler(this.mStepItem_Click);
			// 
			// mRunToolStripMenuItem
			// 
			this.mRunToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mRunToolStripMenuItem.Image")));
			this.mRunToolStripMenuItem.Name = "mRunToolStripMenuItem";
			this.mRunToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mRunToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mRunToolStripMenuItem.Text = "R&un";
			this.mRunToolStripMenuItem.Click += new System.EventHandler(this.mRunItem_Click);
			// 
			// mGoToolStripMenuItem
			// 
			this.mGoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mGoToolStripMenuItem.Image")));
			this.mGoToolStripMenuItem.Name = "mGoToolStripMenuItem";
			this.mGoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			this.mGoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mGoToolStripMenuItem.Text = "&Go";
			this.mGoToolStripMenuItem.Click += new System.EventHandler(this.mGoItem_Click);
			// 
			// mDetachToolStripMenuItem
			// 
			this.mDetachToolStripMenuItem.Name = "mDetachToolStripMenuItem";
			this.mDetachToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.mDetachToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mDetachToolStripMenuItem.Text = "&Detach";
			this.mDetachToolStripMenuItem.Click += new System.EventHandler(this.mDetachItem_Click);
			// 
			// mClearBreakpointsToolStripMenuItem
			// 
			this.mClearBreakpointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mClearBreakpointsToolStripMenuItem.Image")));
			this.mClearBreakpointsToolStripMenuItem.Name = "mClearBreakpointsToolStripMenuItem";
			this.mClearBreakpointsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.mClearBreakpointsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mClearBreakpointsToolStripMenuItem.Text = "Clear &Breakpoints";
			this.mClearBreakpointsToolStripMenuItem.Click += new System.EventHandler(this.mClearBreakpointsItem_Click);
			// 
			// mResetToolStripMenuItem
			// 
			this.mResetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mResetToolStripMenuItem.Image")));
			this.mResetToolStripMenuItem.Name = "mResetToolStripMenuItem";
			this.mResetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.mResetToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.mResetToolStripMenuItem.Text = "&Reset";
			this.mResetToolStripMenuItem.Click += new System.EventHandler(this.mResetItem_Click);
			// 
			// mToolsToolStripMenuItem
			// 
			this.mToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mPreferencesToolStripMenuItem,
            this.toolStripSeparator9,
            this.mProfilingEnabledMenuItem,
            this.mProfilingShowTickCountsMenuItem,
            this.mProfilingResetCountsMenuItem});
			this.mToolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
			this.mToolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.mToolsToolStripMenuItem.Text = "&Tools";
			// 
			// mPreferencesToolStripMenuItem
			// 
			this.mPreferencesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mPreferencesToolStripMenuItem.Image")));
			this.mPreferencesToolStripMenuItem.Name = "mPreferencesToolStripMenuItem";
			this.mPreferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.mPreferencesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.mPreferencesToolStripMenuItem.Text = "&Preferences...";
			this.mPreferencesToolStripMenuItem.Click += new System.EventHandler(this.mPreferencesMenuItem_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(182, 6);
			// 
			// mProfilingEnabledMenuItem
			// 
			this.mProfilingEnabledMenuItem.CheckOnClick = true;
			this.mProfilingEnabledMenuItem.Name = "mProfilingEnabledMenuItem";
			this.mProfilingEnabledMenuItem.Size = new System.Drawing.Size(185, 22);
			this.mProfilingEnabledMenuItem.Text = "&Enable profiling";
			this.mProfilingEnabledMenuItem.CheckedChanged += new System.EventHandler(this.mProfilingEnabledMenuItem_CheckedChanged);
			// 
			// mProfilingShowTickCountsMenuItem
			// 
			this.mProfilingShowTickCountsMenuItem.CheckOnClick = true;
			this.mProfilingShowTickCountsMenuItem.Name = "mProfilingShowTickCountsMenuItem";
			this.mProfilingShowTickCountsMenuItem.Size = new System.Drawing.Size(185, 22);
			this.mProfilingShowTickCountsMenuItem.Text = "&Show tick counts";
			this.mProfilingShowTickCountsMenuItem.CheckedChanged += new System.EventHandler(this.mProfilingShowTickCountsMenuItem_CheckedChanged);
			// 
			// mHelpToolStripMenuItem
			// 
			this.mHelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAboutToolStripMenuItem});
			this.mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
			this.mHelpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.mHelpToolStripMenuItem.Text = "&Help";
			// 
			// mAboutToolStripMenuItem
			// 
			this.mAboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mAboutToolStripMenuItem.Image")));
			this.mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
			this.mAboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.mAboutToolStripMenuItem.Text = "&About...";
			this.mAboutToolStripMenuItem.Click += new System.EventHandler(this.mAboutMenuItem_Click);
			// 
			// mToolStripContainer
			// 
			// 
			// mToolStripContainer.BottomToolStripPanel
			// 
			this.mToolStripContainer.BottomToolStripPanel.Controls.Add(this.mStatusStrip);
			// 
			// mToolStripContainer.ContentPanel
			// 
			this.mToolStripContainer.ContentPanel.Controls.Add(this.mSplitContainer);
			this.mToolStripContainer.ContentPanel.Size = new System.Drawing.Size(804, 491);
			this.mToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mToolStripContainer.LeftToolStripPanelVisible = false;
			this.mToolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.mToolStripContainer.Name = "mToolStripContainer";
			this.mToolStripContainer.RightToolStripPanelVisible = false;
			this.mToolStripContainer.Size = new System.Drawing.Size(804, 562);
			this.mToolStripContainer.TabIndex = 0;
			this.mToolStripContainer.Text = "ToolStripContainer";
			// 
			// mToolStripContainer.TopToolStripPanel
			// 
			this.mToolStripContainer.TopToolStripPanel.Controls.Add(this.mMainMenuStrip);
			this.mToolStripContainer.TopToolStripPanel.Controls.Add(this.mControlToolStrip);
			// 
			// mStatusStrip
			// 
			this.mStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.mStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mModeToolStripStatusLabel,
            this.mModeCycleButton,
            this.mStatusToolStripStatusLabel,
            this.mToolStripStatusLabel});
			this.mStatusStrip.Location = new System.Drawing.Point(0, 0);
			this.mStatusStrip.Name = "mStatusStrip";
			this.mStatusStrip.Size = new System.Drawing.Size(804, 22);
			this.mStatusStrip.TabIndex = 8;
			// 
			// mModeToolStripStatusLabel
			// 
			this.mModeToolStripStatusLabel.Name = "mModeToolStripStatusLabel";
			this.mModeToolStripStatusLabel.Size = new System.Drawing.Size(41, 17);
			this.mModeToolStripStatusLabel.Text = "Mode:";
			// 
			// mModeCycleButton
			// 
			this.mModeCycleButton.AutoSize = false;
			this.mModeCycleButton.Name = "mModeCycleButton";
			this.mModeCycleButton.Size = new System.Drawing.Size(60, 20);
			this.mModeCycleButton.Value = null;
			// 
			// mStatusToolStripStatusLabel
			// 
			this.mStatusToolStripStatusLabel.Name = "mStatusToolStripStatusLabel";
			this.mStatusToolStripStatusLabel.Size = new System.Drawing.Size(48, 17);
			this.mStatusToolStripStatusLabel.Text = "| Status:";
			// 
			// mToolStripStatusLabel
			// 
			this.mToolStripStatusLabel.Name = "mToolStripStatusLabel";
			this.mToolStripStatusLabel.Size = new System.Drawing.Size(10, 17);
			this.mToolStripStatusLabel.Text = " ";
			// 
			// mSplitContainer
			// 
			this.mSplitContainer.BackColor = System.Drawing.SystemColors.Control;
			this.mSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.mSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.mSplitContainer.Name = "mSplitContainer";
			this.mSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// mSplitContainer.Panel1
			// 
			this.mSplitContainer.Panel1.Controls.Add(this.mMemoryGroup);
			this.mSplitContainer.Panel1.Controls.Add(this.mRegistersGroup);
			this.mSplitContainer.Panel1.Controls.Add(this.mDevicesGroup);
			this.mSplitContainer.Panel1.Controls.Add(this.mSymbolGroup);
			this.mSplitContainer.Panel1MinSize = 380;
			// 
			// mSplitContainer.Panel2
			// 
			this.mSplitContainer.Panel2.Controls.Add(this.mLogListGroup);
			this.mSplitContainer.Panel2MinSize = 96;
			this.mSplitContainer.Size = new System.Drawing.Size(804, 491);
			this.mSplitContainer.SplitterDistance = 385;
			this.mSplitContainer.TabIndex = 6;
			// 
			// mMemoryGroup
			// 
			this.mMemoryGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mMemoryGroup.Controls.Add(this.mMemoryTabControl);
			this.mMemoryGroup.Location = new System.Drawing.Point(236, -2);
			this.mMemoryGroup.Name = "mMemoryGroup";
			this.mMemoryGroup.Size = new System.Drawing.Size(568, 328);
			this.mMemoryGroup.TabIndex = 4;
			this.mMemoryGroup.TabStop = false;
			this.mMemoryGroup.Text = "Memory";
			// 
			// mMemoryTabControl
			// 
			this.mMemoryTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mMemoryTabControl.Controls.Add(this.mMainMemoryTab);
			this.mMemoryTabControl.Controls.Add(this.mFloatingPointMemoryTab);
			this.mMemoryTabControl.Location = new System.Drawing.Point(4, 16);
			this.mMemoryTabControl.Name = "mMemoryTabControl";
			this.mMemoryTabControl.SelectedIndex = 0;
			this.mMemoryTabControl.Size = new System.Drawing.Size(560, 309);
			this.mMemoryTabControl.TabIndex = 0;
			this.mMemoryTabControl.SelectedIndexChanged += new System.EventHandler(this.mMemoryTabControl_SelectedIndexChanged);
			this.mMemoryTabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.mMemoryTabControl_Selecting);
			// 
			// mMainMemoryTab
			// 
			this.mMainMemoryTab.Controls.Add(this.mMainMemoryEditor);
			this.mMainMemoryTab.Location = new System.Drawing.Point(4, 22);
			this.mMainMemoryTab.Name = "mMainMemoryTab";
			this.mMainMemoryTab.Padding = new System.Windows.Forms.Padding(3);
			this.mMainMemoryTab.Size = new System.Drawing.Size(552, 283);
			this.mMainMemoryTab.TabIndex = 0;
			this.mMainMemoryTab.Text = "Main";
			this.mMainMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mMainMemoryEditor
			// 
			this.mMainMemoryEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mMainMemoryEditor.FirstVisibleAddress = 0;
			this.mMainMemoryEditor.IndexedAddressCalculatorCallback = null;
			this.mMainMemoryEditor.Location = new System.Drawing.Point(0, 0);
			this.mMainMemoryEditor.MarkedAddress = -1;
			this.mMainMemoryEditor.Memory = null;
			this.mMainMemoryEditor.Name = "mMainMemoryEditor";
			this.mMainMemoryEditor.ReadOnly = false;
			this.mMainMemoryEditor.ResizeInProgress = false;
			this.mMainMemoryEditor.Size = new System.Drawing.Size(552, 283);
			this.mMainMemoryEditor.Symbols = null;
			this.mMainMemoryEditor.TabIndex = 0;
			this.mMainMemoryEditor.ToolTip = null;
			this.mMainMemoryEditor.AddressSelected += new MixGui.Events.AddressSelectedHandler(this.mMemoryEditor_addressSelected);
			// 
			// mFloatingPointMemoryTab
			// 
			this.mFloatingPointMemoryTab.Location = new System.Drawing.Point(4, 22);
			this.mFloatingPointMemoryTab.Name = "mFloatingPointMemoryTab";
			this.mFloatingPointMemoryTab.Padding = new System.Windows.Forms.Padding(3);
			this.mFloatingPointMemoryTab.Size = new System.Drawing.Size(552, 283);
			this.mFloatingPointMemoryTab.TabIndex = 1;
			this.mFloatingPointMemoryTab.Text = "Floating Point";
			this.mFloatingPointMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mRegistersGroup
			// 
			this.mRegistersGroup.Controls.Add(this.mRegistersEditor);
			this.mRegistersGroup.Location = new System.Drawing.Point(0, -2);
			this.mRegistersGroup.Name = "mRegistersGroup";
			this.mRegistersGroup.Size = new System.Drawing.Size(232, 236);
			this.mRegistersGroup.TabIndex = 2;
			this.mRegistersGroup.TabStop = false;
			this.mRegistersGroup.Text = "Registers";
			// 
			// mRegistersEditor
			// 
			this.mRegistersEditor.Location = new System.Drawing.Point(8, 16);
			this.mRegistersEditor.Name = "mRegistersEditor";
			this.mRegistersEditor.ReadOnly = false;
			registers1.CompareIndicator = MixLib.Registers.CompValues.Equal;
			registers1.OverflowIndicator = false;
			this.mRegistersEditor.Registers = registers1;
			this.mRegistersEditor.Size = new System.Drawing.Size(218, 214);
			this.mRegistersEditor.TabIndex = 0;
			// 
			// mDevicesGroup
			// 
			this.mDevicesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDevicesGroup.Controls.Add(this.mDeviceEditorButton);
			this.mDevicesGroup.Controls.Add(this.mDevicesControl);
			this.mDevicesGroup.Location = new System.Drawing.Point(0, 326);
			this.mDevicesGroup.Name = "mDevicesGroup";
			this.mDevicesGroup.Size = new System.Drawing.Size(804, 56);
			this.mDevicesGroup.TabIndex = 5;
			this.mDevicesGroup.TabStop = false;
			this.mDevicesGroup.Text = "Devices";
			// 
			// mDeviceEditorButton
			// 
			this.mDeviceEditorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mDeviceEditorButton.Location = new System.Drawing.Point(721, 16);
			this.mDeviceEditorButton.Margin = new System.Windows.Forms.Padding(0);
			this.mDeviceEditorButton.Name = "mDeviceEditorButton";
			this.mDeviceEditorButton.Size = new System.Drawing.Size(75, 23);
			this.mDeviceEditorButton.TabIndex = 1;
			this.mDeviceEditorButton.Text = "Show E&ditor";
			this.mDeviceEditorButton.UseVisualStyleBackColor = true;
			this.mDeviceEditorButton.Click += new System.EventHandler(this.mDeviceEditorItem_Click);
			// 
			// mDevicesControl
			// 
			this.mDevicesControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mDevicesControl.Devices = null;
			this.mDevicesControl.Location = new System.Drawing.Point(4, 16);
			this.mDevicesControl.Name = "mDevicesControl";
			this.mDevicesControl.Size = new System.Drawing.Size(711, 36);
			this.mDevicesControl.Structure = null;
			this.mDevicesControl.TabIndex = 0;
			this.mDevicesControl.ToolTip = null;
			// 
			// mSymbolGroup
			// 
			this.mSymbolGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.mSymbolGroup.Controls.Add(this.mSymbolListView);
			this.mSymbolGroup.Location = new System.Drawing.Point(0, 234);
			this.mSymbolGroup.Name = "mSymbolGroup";
			this.mSymbolGroup.Size = new System.Drawing.Size(232, 92);
			this.mSymbolGroup.TabIndex = 3;
			this.mSymbolGroup.TabStop = false;
			this.mSymbolGroup.Text = "Symbols";
			// 
			// mSymbolListView
			// 
			this.mSymbolListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mSymbolListView.Location = new System.Drawing.Point(8, 16);
			this.mSymbolListView.MemoryMaxIndex = 0;
			this.mSymbolListView.MemoryMinIndex = 0;
			this.mSymbolListView.Name = "mSymbolListView";
			this.mSymbolListView.Size = new System.Drawing.Size(218, 70);
			this.mSymbolListView.Symbols = null;
			this.mSymbolListView.TabIndex = 0;
			this.mSymbolListView.AddressSelected += new MixGui.Events.AddressSelectedHandler(this.listViews_addressSelected);
			// 
			// mLogListGroup
			// 
			this.mLogListGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLogListGroup.Controls.Add(this.mLogListView);
			this.mLogListGroup.Location = new System.Drawing.Point(0, 3);
			this.mLogListGroup.Name = "mLogListGroup";
			this.mLogListGroup.Size = new System.Drawing.Size(804, 95);
			this.mLogListGroup.TabIndex = 7;
			this.mLogListGroup.TabStop = false;
			this.mLogListGroup.Text = "Messages";
			// 
			// mLogListView
			// 
			this.mLogListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLogListView.Location = new System.Drawing.Point(8, 16);
			this.mLogListView.Name = "mLogListView";
			this.mLogListView.SeverityImageList = this.mSeverityImageList;
			this.mLogListView.Size = new System.Drawing.Size(788, 73);
			this.mLogListView.TabIndex = 0;
			this.mLogListView.AddressSelected += new MixGui.Events.AddressSelectedHandler(this.listViews_addressSelected);
			// 
			// mControlToolStrip
			// 
			this.mControlToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.mControlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.mControlToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mPCToolStripLabel,
            this.mShowPCToolStripButton,
            toolStripSeparator3,
            this.mTicksToolStripLabel,
            this.mTicksToolStripTextBox,
            this.mResetTicksToolStripButton,
            toolStripSeparator4,
            this.mTickToolStripButton,
            this.mStepToolStripButton,
            this.mRunToolStripButton,
            this.mGoToolStripButton,
            toolStripSeparator5,
            this.mDetachToolStripButton,
            toolStripSeparator6,
            this.mClearBreakpointsToolStripButton,
            this.mResetToolStripButton,
            toolStripSeparator7,
            this.mTeletypeToolStripButton});
			this.mControlToolStrip.Location = new System.Drawing.Point(3, 24);
			this.mControlToolStrip.Name = "mControlToolStrip";
			this.mControlToolStrip.Size = new System.Drawing.Size(761, 25);
			this.mControlToolStrip.TabIndex = 1;
			// 
			// mPCToolStripLabel
			// 
			this.mPCToolStripLabel.Name = "mPCToolStripLabel";
			this.mPCToolStripLabel.Size = new System.Drawing.Size(28, 22);
			this.mPCToolStripLabel.Text = "PC: ";
			// 
			// mShowPCToolStripButton
			// 
			this.mShowPCToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mShowPCToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mShowPCToolStripButton.Image")));
			this.mShowPCToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mShowPCToolStripButton.Name = "mShowPCToolStripButton";
			this.mShowPCToolStripButton.Size = new System.Drawing.Size(40, 22);
			this.mShowPCToolStripButton.Text = "Sh&ow";
			this.mShowPCToolStripButton.ToolTipText = "Show the program counter";
			this.mShowPCToolStripButton.Click += new System.EventHandler(this.mShowPCButton_Click);
			// 
			// mTicksToolStripLabel
			// 
			this.mTicksToolStripLabel.Name = "mTicksToolStripLabel";
			this.mTicksToolStripLabel.Size = new System.Drawing.Size(40, 22);
			this.mTicksToolStripLabel.Text = "Ticks: ";
			// 
			// mTicksToolStripTextBox
			// 
			this.mTicksToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mTicksToolStripTextBox.Name = "mTicksToolStripTextBox";
			this.mTicksToolStripTextBox.ReadOnly = true;
			this.mTicksToolStripTextBox.Size = new System.Drawing.Size(64, 25);
			this.mTicksToolStripTextBox.Text = "0";
			// 
			// mResetTicksToolStripButton
			// 
			this.mResetTicksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mResetTicksToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mResetTicksToolStripButton.Image")));
			this.mResetTicksToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mResetTicksToolStripButton.Name = "mResetTicksToolStripButton";
			this.mResetTicksToolStripButton.Size = new System.Drawing.Size(39, 22);
			this.mResetTicksToolStripButton.Text = "&Reset";
			this.mResetTicksToolStripButton.ToolTipText = "Reset the tick counter";
			this.mResetTicksToolStripButton.Click += new System.EventHandler(this.mResetTicksButton_Click);
			// 
			// mTickToolStripButton
			// 
			this.mTickToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mTickToolStripButton.Image")));
			this.mTickToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mTickToolStripButton.Name = "mTickToolStripButton";
			this.mTickToolStripButton.Size = new System.Drawing.Size(49, 22);
			this.mTickToolStripButton.Text = "T&ick";
			this.mTickToolStripButton.ToolTipText = "Execute one tick";
			this.mTickToolStripButton.Click += new System.EventHandler(this.mTickItem_Click);
			// 
			// mStepToolStripButton
			// 
			this.mStepToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mStepToolStripButton.Image")));
			this.mStepToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mStepToolStripButton.Name = "mStepToolStripButton";
			this.mStepToolStripButton.Size = new System.Drawing.Size(50, 22);
			this.mStepToolStripButton.Text = "Ste&p";
			this.mStepToolStripButton.ToolTipText = "Execute one step";
			this.mStepToolStripButton.Click += new System.EventHandler(this.mStepItem_Click);
			// 
			// mRunToolStripButton
			// 
			this.mRunToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mRunToolStripButton.Image")));
			this.mRunToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mRunToolStripButton.Name = "mRunToolStripButton";
			this.mRunToolStripButton.Size = new System.Drawing.Size(48, 22);
			this.mRunToolStripButton.Text = "R&un";
			this.mRunToolStripButton.ToolTipText = "Start or stop a full run";
			this.mRunToolStripButton.Click += new System.EventHandler(this.mRunItem_Click);
			// 
			// mGoToolStripButton
			// 
			this.mGoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mGoToolStripButton.Image")));
			this.mGoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mGoToolStripButton.Name = "mGoToolStripButton";
			this.mGoToolStripButton.Size = new System.Drawing.Size(42, 22);
			this.mGoToolStripButton.Text = "&Go";
			this.mGoToolStripButton.ToolTipText = "Start the MIX loader";
			this.mGoToolStripButton.Click += new System.EventHandler(this.mGoItem_Click);
			// 
			// mDetachToolStripButton
			// 
			this.mDetachToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mDetachToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mDetachToolStripButton.Image")));
			this.mDetachToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mDetachToolStripButton.Name = "mDetachToolStripButton";
			this.mDetachToolStripButton.Size = new System.Drawing.Size(48, 22);
			this.mDetachToolStripButton.Text = "Detac&h";
			this.mDetachToolStripButton.ToolTipText = "Switch between running MIX in the foreground or background";
			this.mDetachToolStripButton.Click += new System.EventHandler(this.mDetachItem_Click);
			// 
			// mClearBreakpointsToolStripButton
			// 
			this.mClearBreakpointsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mClearBreakpointsToolStripButton.Image")));
			this.mClearBreakpointsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mClearBreakpointsToolStripButton.Name = "mClearBreakpointsToolStripButton";
			this.mClearBreakpointsToolStripButton.Size = new System.Drawing.Size(119, 22);
			this.mClearBreakpointsToolStripButton.Text = "Clear &Breakpoints";
			this.mClearBreakpointsToolStripButton.ToolTipText = "Remove all set breakpoints";
			this.mClearBreakpointsToolStripButton.Click += new System.EventHandler(this.mClearBreakpointsItem_Click);
			// 
			// mResetToolStripButton
			// 
			this.mResetToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mResetToolStripButton.Image")));
			this.mResetToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mResetToolStripButton.Name = "mResetToolStripButton";
			this.mResetToolStripButton.Size = new System.Drawing.Size(55, 22);
			this.mResetToolStripButton.Text = "R&eset";
			this.mResetToolStripButton.ToolTipText = "Reset MIX to initial state";
			this.mResetToolStripButton.Click += new System.EventHandler(this.mResetItem_Click);
			// 
			// mTeletypeToolStripButton
			// 
			this.mTeletypeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mTeletypeToolStripButton.Image")));
			this.mTeletypeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mTeletypeToolStripButton.Name = "mTeletypeToolStripButton";
			this.mTeletypeToolStripButton.Size = new System.Drawing.Size(104, 22);
			this.mTeletypeToolStripButton.Text = "Show &Teletype";
			this.mTeletypeToolStripButton.ToolTipText = "Show or hide the teletype";
			this.mTeletypeToolStripButton.Click += new System.EventHandler(this.mTeletypeItem_Click);
			// 
			// mToolTip
			// 
			this.mToolTip.AutoPopDelay = 10000;
			this.mToolTip.InitialDelay = 100;
			this.mToolTip.ReshowDelay = 100;
			this.mToolTip.ShowAlways = true;
			// 
			// mProfilingResetCountsMenuItem
			// 
			this.mProfilingResetCountsMenuItem.Name = "mProfilingResetCountsMenuItem";
			this.mProfilingResetCountsMenuItem.Size = new System.Drawing.Size(185, 22);
			this.mProfilingResetCountsMenuItem.Text = "&Reset counts";
			this.mProfilingResetCountsMenuItem.Click += new System.EventHandler(this.mProfilingResetCountsMenuItem_Click);
			// 
			// MixForm
			// 
			this.ClientSize = new System.Drawing.Size(804, 562);
			this.Controls.Add(this.mToolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.mMainMenuStrip;
			this.MinimumSize = new System.Drawing.Size(820, 600);
			this.Name = "MixForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "MixEmul";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.this_FormClosing);
			this.ResizeBegin += new System.EventHandler(this.this_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.MixForm_ResizeEnd);
			this.mMainMenuStrip.ResumeLayout(false);
			this.mMainMenuStrip.PerformLayout();
			this.mToolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.mToolStripContainer.BottomToolStripPanel.PerformLayout();
			this.mToolStripContainer.ContentPanel.ResumeLayout(false);
			this.mToolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.mToolStripContainer.TopToolStripPanel.PerformLayout();
			this.mToolStripContainer.ResumeLayout(false);
			this.mToolStripContainer.PerformLayout();
			this.mStatusStrip.ResumeLayout(false);
			this.mStatusStrip.PerformLayout();
			this.mSplitContainer.Panel1.ResumeLayout(false);
			this.mSplitContainer.Panel2.ResumeLayout(false);
			this.mSplitContainer.ResumeLayout(false);
			this.mMemoryGroup.ResumeLayout(false);
			this.mMemoryTabControl.ResumeLayout(false);
			this.mMainMemoryTab.ResumeLayout(false);
			this.mRegistersGroup.ResumeLayout(false);
			this.mDevicesGroup.ResumeLayout(false);
			this.mSymbolGroup.ResumeLayout(false);
			this.mLogListGroup.ResumeLayout(false);
			this.mControlToolStrip.ResumeLayout(false);
			this.mControlToolStrip.PerformLayout();
			this.ResumeLayout(false);

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

		private int calculateIndexedAddress(int address, int index)
		{
			return InstructionHelpers.GetValidIndexedAddress(mMix, address, index, false);
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

		private void mExitMenuItem_Click(object sender, EventArgs e)
		{
			base.Close();
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
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(mMix_InputRequired));
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
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(mMix_LogLineAdded));
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

		private void mRunItem_Click(object sender, EventArgs e)
		{
			switchRunningState();
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

		private void mShowPCButton_Click(object sender, EventArgs e)
		{
			mMainMemoryEditor.MakeAddressVisible(mMix.ProgramCounter);
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

		private void mTeletypeItem_Click(object sender, EventArgs e)
		{
			switchTeletypeVisibility();
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

		void mFloatingPointMemoryEditor_AddressSelected(object sender, AddressSelectedEventArgs args)
		{
			implementFloatingPointPCChange(args.SelectedAddress);
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

		private void findNext()
		{
			activeMemoryEditor.FindMatch(mSearchOptions);
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
