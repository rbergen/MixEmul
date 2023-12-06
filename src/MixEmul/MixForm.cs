using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private const int MainMemoryEditorIndex = 0;
		private const int FloatingPointMemoryEditorIndex = 1;

		private IContainer components;
		private AboutForm aboutForm;
		private readonly Configuration configuration;
		private readonly string defaultDirectory = Environment.CurrentDirectory;
		private readonly Mix mix;
		private OpenFileDialog openProgramFileDialog;
		private PreferencesForm preferencesForm;
		private bool readOnly;
		private bool running;
		private bool updating;
		private ImageList severityImageList;
		private SourceAndFindingsForm sourceAndFindingsForm;
		private SteppingState steppingState;
		private readonly TeletypeForm teletype;
		private MenuStrip mainMenuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem openProgramToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem preferencesToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem actionsToolStripMenuItem;
		private ToolStripMenuItem tickToolStripMenuItem;
		private ToolStripMenuItem stepToolStripMenuItem;
		private ToolStripMenuItem runToolStripMenuItem;
		private ToolStripMenuItem goToolStripMenuItem;
		private ToolStripMenuItem detachToolStripMenuItem;
		private ToolStripMenuItem resetToolStripMenuItem;
		private ToolStripMenuItem viewToolStripMenuItem;
		private ToolStripMenuItem teletypeToolStripMenuItem;
		private ToolStripContainer toolStripContainer;
		private StatusStrip statusStrip;
		private ToolStripStatusLabel toolStripStatusLabel;
		private readonly LongValueTextBox pcBox;
		private ToolStrip controlToolStrip;
		private ToolStripLabel pcToolStripLabel;
		private ToolStripButton showPCToolStripButton;
		private ToolStripLabel ticksToolStripLabel;
		private ToolStripTextBox ticksToolStripTextBox;
		private ToolStripButton resetTicksToolStripButton;
		private ToolStripButton tickToolStripButton;
		private ToolStripButton stepToolStripButton;
		private ToolStripButton runToolStripButton;
		private ToolStripButton goToolStripButton;
		private ToolStripButton detachToolStripButton;
		private ToolStripButton resetToolStripButton;
		private ToolStripButton teletypeToolStripButton;
		private SplitContainer splitContainer;
		private GroupBox memoryGroup;
		private MemoryEditor mainMemoryEditor;
		private MemoryEditor floatingPointMemoryEditor;
		private readonly MemoryEditor[] memoryEditors;
		private GroupBox registersGroup;
		private RegistersEditor registersEditor;
		private GroupBox devicesGroup;
		private DevicesControl devicesControl;
		private GroupBox symbolGroup;
		private SymbolListView symbolListView;
		private GroupBox logListGroup;
		private LogListView logListView;
		private ToolStripButton clearBreakpointsToolStripButton;
		private ToolStripMenuItem clearBreakpointsToolStripMenuItem;
		private Button deviceEditorButton;
		private ToolStripMenuItem deviceEditorToolStripMenuItem;
		private ToolStripCycleButton modeCycleButton;
		private TabControl memoryTabControl;
		private TabPage mainMemoryTab;
		private TabPage floatingPointMemoryTab;
		private ToolStripStatusLabel modeToolStripStatusLabel;
		private ToolStripStatusLabel statusToolStripStatusLabel;
		private ToolTip toolTip;
		private ToolStripMenuItem findMenuItem;
		private ToolStripMenuItem findNextMenuItem;
		private readonly DeviceEditorForm deviceEditor;
		private SearchDialog searchDialog;
		private ToolStripMenuItem profilingEnabledMenuItem;
		private ToolStripMenuItem profilingShowTickCountsMenuItem;
		private ToolStripMenuItem profilingResetCountsMenuItem;
		private SearchParameters searchOptions;
		private ToolStripMenuItem showSourceInlineMenuItem;

		public MixForm()
		{
			this.configuration = Configuration.Load(this.defaultDirectory);
			ExecutionSettings.ProfilingEnabled = this.configuration.ProfilingEnabled;
			GuiSettings.Colors = this.configuration.Colors;
			GuiSettings.ShowProfilingInfo = this.configuration.ShowProfilingInfo;
			GuiSettings.ColorProfilingCounts = this.configuration.ColorProfilingCounts;
			GuiSettings.ShowSourceInline = this.configuration.ShowSourceInline;
			DeviceSettings.DefaultDeviceFilesDirectory = this.defaultDirectory;

			if (this.configuration.FloatingPointMemoryWordCount != null)
				ModuleSettings.FloatingPointMemoryWordCount = this.configuration.FloatingPointMemoryWordCount.Value;

			InitializeComponent();

			this.memoryEditors = new MemoryEditor[2];
			this.memoryEditors[0] = this.mainMemoryEditor;
			this.memoryEditors[1] = null;

			this.mix = new Mix();
			this.mix.StepPerformed += Mix_StepPerformed;
			this.mix.LogLineAdded += Mix_LogLineAdded;

			this.pcBox = new LongValueTextBox
			{
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = Color.Black,
				LongValue = this.mix.ProgramCounter,
				ClearZero = false
			};

			SetPCBoxBounds();
			this.pcBox.Name = "mPCBox";
			this.pcBox.Size = new Size(40, 25);
			this.pcBox.TabIndex = 1;
			this.pcBox.Text = "0";
			this.pcBox.ValueChanged += PcChanged;
			this.controlToolStrip.Items.Insert(1, new LongValueToolStripTextBox(this.pcBox));

			this.registersEditor.Registers = this.mix.Registers;

			this.mainMemoryEditor.ToolTip = this.toolTip;
			this.mainMemoryEditor.Memory = this.mix.FullMemory;
			this.mainMemoryEditor.MarkedAddress = this.mix.ProgramCounter;
			this.mainMemoryEditor.IndexedAddressCalculatorCallback = CalculateIndexedAddress;

			this.profilingEnabledMenuItem.Checked = ExecutionSettings.ProfilingEnabled;
			this.profilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			this.profilingShowTickCountsMenuItem.Checked = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick;
			this.profilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			this.showSourceInlineMenuItem.Checked = GuiSettings.ShowSourceInline;

			this.symbolListView.MemoryMinIndex = this.mix.FullMemory.MinWordIndex;
			this.symbolListView.MemoryMaxIndex = this.mix.FullMemory.MaxWordIndex;

			var layoutStructure = new DevicesControl.LayoutStructure(DevicesControl.Orientations.Horizontal, 8, 4);
			layoutStructure[8] = DevicesControl.Breaks.Sequence;
			layoutStructure[16] = DevicesControl.Breaks.Section;
			layoutStructure[18] = DevicesControl.Breaks.Sequence;

			this.devicesControl.ToolTip = this.toolTip;
			this.devicesControl.Structure = layoutStructure;
			this.devicesControl.Devices = this.mix.Devices;
			this.devicesControl.DeviceDoubleClick += DevicesControl_DeviceDoubleClick;

			this.teletype = new TeletypeForm(this.mix.Devices.Teletype);
			this.teletype.VisibleChanged += Teletype_VisibleChanged;

			if (this.configuration.TeletypeWindowLocation != Point.Empty)
			{
				this.teletype.StartPosition = FormStartPosition.Manual;
				this.teletype.Location = this.configuration.TeletypeWindowLocation;
			}

			if (this.configuration.TeletypeWindowSize != Size.Empty)
				this.teletype.Size = this.configuration.TeletypeWindowSize;

			this.teletype.TopMost = this.configuration.TeletypeOnTop;
			this.teletype.EchoInput = this.configuration.TeletypeEchoInput;

			this.teletype.AddOutputText("*** MIX TELETYPE INITIALIZED ***");

			if (this.configuration.TeletypeVisible)
				this.teletype.Show();

			CardDeckExporter.LoaderCards = this.configuration.LoaderCards;
			UpdateDeviceConfig();

			this.deviceEditor = new DeviceEditorForm(this.mix.Devices);
			this.deviceEditor.VisibleChanged += DeviceEditor_VisibleChanged;

			if (this.configuration.DeviceEditorWindowLocation != Point.Empty)
			{
				this.deviceEditor.StartPosition = FormStartPosition.Manual;
				this.deviceEditor.Location = this.configuration.DeviceEditorWindowLocation;
			}

			if (this.configuration.DeviceEditorWindowSize != Size.Empty)
				this.deviceEditor.Size = this.configuration.DeviceEditorWindowSize;

			if (this.configuration.DeviceEditorVisible)
				this.deviceEditor.Show();

			this.mix.Devices.Teletype.InputRequired += Mix_InputRequired;
			this.mix.BreakpointManager = this.mainMemoryEditor;

			LoadControlProgram();
			ApplyFloatingPointSettings();

			this.openProgramFileDialog = null;
			this.sourceAndFindingsForm = null;
			this.readOnly = false;
			this.running = false;
			this.steppingState = SteppingState.Idle;

			var symbols = new SymbolCollection();
			this.symbolListView.Symbols = symbols;
			this.mainMemoryEditor.Symbols = symbols;

			var controlStep = this.modeCycleButton.AddStep(ModuleBase.RunMode.Control);
			var normalStep = this.modeCycleButton.AddStep(ModuleBase.RunMode.Normal, controlStep);
			this.modeCycleButton.AddStep(ModuleBase.RunMode.Module, normalStep);
			controlStep.NextStep = normalStep;

			this.modeCycleButton.Value = this.mix.Mode;
			this.modeCycleButton.ValueChanged += ModeCycleButton_ValueChanged;

			Update();

			if (this.configuration.MainWindowLocation != Point.Empty)
			{
				StartPosition = FormStartPosition.Manual;
				Location = this.configuration.MainWindowLocation;
			}
			else
				StartPosition = FormStartPosition.CenterScreen;

			if (this.configuration.MainWindowSize != Size.Empty)
				Size = this.configuration.MainWindowSize;

			WindowState = this.configuration.MainWindowState;
			SizeChanged += This_SizeChanged;
			LocationChanged += This_LocationChanged;
		}

		private MemoryEditor ActiveMemoryEditor => this.memoryEditors[this.memoryTabControl.SelectedIndex];

		private void FindNext() => ActiveMemoryEditor.FindMatch(this.searchOptions);

		private int CalculateIndexedAddress(int address, int index) => InstructionHelpers.GetValidIndexedAddress(this.mix, address, index, false);

		private void SetPCBoxBounds()
		{
			this.pcBox.MinValue = this.mix.Memory.MinWordIndex;
			this.pcBox.MaxValue = this.mix.Memory.MaxWordIndex;
		}

		private void ApplyFloatingPointSettings()
		{
			this.mix.SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);

			if (ModuleSettings.FloatingPointEnabled)
				LoadModuleProgram(this.mix.FloatingPointModule, ModuleSettings.FloatingPointProgramFile, "floating point");
		}

		private void LoadControlProgram()
		{
			string controlProgramFileName = ModuleSettings.ControlProgramFile;

			if (string.IsNullOrEmpty(controlProgramFileName) || !File.Exists(controlProgramFileName))
				return;

			LoadModuleProgram(this.mix, controlProgramFileName, "control");
			this.mix.FullMemory.ClearSourceLines();
		}

		private SymbolCollection LoadModuleProgram(ModuleBase module, string fileName, string programName)
		{
			try
			{
				var instances = Assembler.Assemble(File.ReadAllLines(fileName), out PreInstruction[] instructions, out SymbolCollection symbols, out AssemblyFindingCollection findings);
				if (instances != null)
				{
					if (!module.LoadInstructionInstances(instances, symbols))
						this.logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to load {programName} program"));
				}
				else
				{
					if (findings != null)
					{
						this.logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Parse failed", $"Error(s) occured while assembling {programName} program"));
						foreach (AssemblyFinding finding in findings)
							this.logListView.AddLogLine(new LogLine("Assembler", finding.Severity, "Parse error", finding.LineNumber == int.MinValue ? finding.Message : $"Line {finding.LineNumber}: {finding.Message}"));
					}
					this.logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to assemble {programName} program"));

					return symbols;

				}
			}
			catch (Exception ex)
			{
				this.logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to read {programName} program: {ex.Message}"));
			}

			return null;
		}

		private bool DisableTeletypeOnTop()
		{
			if (this.teletype != null && this.teletype.Visible && this.teletype.TopMost)
			{
				this.teletype.TopMost = false;
				return true;
			}

			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.components?.Dispose();
				this.mix?.Dispose();
			}

			base.Dispose(disposing);
		}

		private static void HandleException(Exception exception)
		{
			Exception handlingException = null;
			var path = Path.Combine(Environment.CurrentDirectory, "exception.log");

			try
			{
				var writer = new StreamWriter(path, true);
				writer.WriteLine("Unhandled exception " + exception.GetType().FullName + ": " + exception.Message);
				writer.WriteLine("  Stack trace: " + exception.StackTrace);

				if (exception.InnerException != null)
					writer.WriteLine("  Inner exception: " + exception.InnerException);

				if (exception.Data.Count > 0)
				{
					writer.WriteLine("  Exception data:");

					foreach (DictionaryEntry entry in exception.Data)
						writer.WriteLine(string.Concat(new object[] { "  * ", entry.Key, " = ", entry.Value }));
				}

				writer.WriteLine();
				writer.Close();
			}
			catch (Exception ex)
			{
				handlingException = ex;
			}

			var text = string.Concat(new object[] { "I'm sorry, MixEmul seems to have encountered a fatal problem.\nAn unhandled error of type ", exception.GetType().Name, " occured, with message: ", exception.Message, '\n' });

			if (handlingException == null)
				text = text + "A detailed error log has been written to the following location: " + path + ".\nPlease attach this file when reporting the error.";

			else
			{
				string str3 = text;
				text = str3 + "Additionally, an error of type " + handlingException.GetType().Name + " occured while logging the error, with message: " + handlingException.Message + "\nPlease include this information when reporting this error.";
			}

			MessageBox.Show(text, "Unhandled program error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}

		private void ImplementPCChange(int address)
		{
			if (this.mainMemoryEditor.IsAddressVisible(this.mix.ProgramCounter))
				this.mainMemoryEditor.MakeAddressVisible(address, this.mix.Status == ModuleBase.RunStatus.Idle);

			this.mix.ProgramCounter = address;
			this.mainMemoryEditor.MarkedAddress = this.mix.ProgramCounter;
			this.pcBox.LongValue = this.mix.ProgramCounter;
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ToolStripSeparator fileMenuToolStripSeparator;
			ToolStripSeparator toolStripSeparator1;
			ToolStripSeparator toolStripSeparator2;
			ToolStripSeparator toolStripSeparator3;
			ToolStripSeparator toolStripSeparator4;
			ToolStripSeparator toolStripSeparator5;
			ToolStripSeparator toolStripSeparator6;
			ToolStripSeparator toolStripSeparator7;
			ToolStripSeparator toolStripSeparator8;
			ToolStripSeparator toolStripSeparator9;
			ToolStripSeparator toolStripSeparator10;
			var resources = new ComponentResourceManager(typeof(MixForm));
			var registers1 = new Registers();
			this.severityImageList = new ImageList(this.components);
			this.mainMenuStrip = new MenuStrip();
			this.fileToolStripMenuItem = new ToolStripMenuItem();
			this.openProgramToolStripMenuItem = new ToolStripMenuItem();
			this.exitToolStripMenuItem = new ToolStripMenuItem();
			this.viewToolStripMenuItem = new ToolStripMenuItem();
			this.teletypeToolStripMenuItem = new ToolStripMenuItem();
			this.deviceEditorToolStripMenuItem = new ToolStripMenuItem();
			this.showSourceInlineMenuItem = new ToolStripMenuItem();
			this.findMenuItem = new ToolStripMenuItem();
			this.findNextMenuItem = new ToolStripMenuItem();
			this.actionsToolStripMenuItem = new ToolStripMenuItem();
			this.tickToolStripMenuItem = new ToolStripMenuItem();
			this.stepToolStripMenuItem = new ToolStripMenuItem();
			this.runToolStripMenuItem = new ToolStripMenuItem();
			this.goToolStripMenuItem = new ToolStripMenuItem();
			this.detachToolStripMenuItem = new ToolStripMenuItem();
			this.clearBreakpointsToolStripMenuItem = new ToolStripMenuItem();
			this.resetToolStripMenuItem = new ToolStripMenuItem();
			toolsToolStripMenuItem = new ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new ToolStripMenuItem();
			this.profilingEnabledMenuItem = new ToolStripMenuItem();
			this.profilingShowTickCountsMenuItem = new ToolStripMenuItem();
			this.helpToolStripMenuItem = new ToolStripMenuItem();
			this.aboutToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripContainer = new ToolStripContainer();
			this.statusStrip = new StatusStrip();
			this.modeToolStripStatusLabel = new ToolStripStatusLabel();
			this.modeCycleButton = new ToolStripCycleButton(this.components);
			this.statusToolStripStatusLabel = new ToolStripStatusLabel();
			this.toolStripStatusLabel = new ToolStripStatusLabel();
			this.splitContainer = new SplitContainer();
			this.memoryGroup = new GroupBox();
			this.memoryTabControl = new TabControl();
			this.mainMemoryTab = new TabPage();
			this.mainMemoryEditor = new MemoryEditor();
			this.floatingPointMemoryTab = new TabPage();
			this.registersGroup = new GroupBox();
			this.registersEditor = new RegistersEditor();
			this.devicesGroup = new GroupBox();
			this.deviceEditorButton = new Button();
			this.devicesControl = new DevicesControl();
			this.symbolGroup = new GroupBox();
			this.symbolListView = new SymbolListView();
			this.logListGroup = new GroupBox();
			this.logListView = new LogListView();
			this.controlToolStrip = new ToolStrip();
			this.pcToolStripLabel = new ToolStripLabel();
			this.showPCToolStripButton = new ToolStripButton();
			this.ticksToolStripLabel = new ToolStripLabel();
			this.ticksToolStripTextBox = new ToolStripTextBox();
			this.resetTicksToolStripButton = new ToolStripButton();
			this.tickToolStripButton = new ToolStripButton();
			this.stepToolStripButton = new ToolStripButton();
			this.runToolStripButton = new ToolStripButton();
			this.goToolStripButton = new ToolStripButton();
			this.detachToolStripButton = new ToolStripButton();
			this.clearBreakpointsToolStripButton = new ToolStripButton();
			this.resetToolStripButton = new ToolStripButton();
			this.teletypeToolStripButton = new ToolStripButton();
			this.toolTip = new ToolTip(this.components);
			this.profilingResetCountsMenuItem = new ToolStripMenuItem();
			fileMenuToolStripSeparator = new ToolStripSeparator();
			toolStripSeparator1 = new ToolStripSeparator();
			toolStripSeparator2 = new ToolStripSeparator();
			toolStripSeparator3 = new ToolStripSeparator();
			toolStripSeparator4 = new ToolStripSeparator();
			toolStripSeparator5 = new ToolStripSeparator();
			toolStripSeparator6 = new ToolStripSeparator();
			toolStripSeparator7 = new ToolStripSeparator();
			toolStripSeparator8 = new ToolStripSeparator();
			toolStripSeparator9 = new ToolStripSeparator();
			toolStripSeparator10 = new ToolStripSeparator();
			this.mainMenuStrip.SuspendLayout();
			this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.memoryGroup.SuspendLayout();
			this.memoryTabControl.SuspendLayout();
			this.mainMemoryTab.SuspendLayout();
			this.registersGroup.SuspendLayout();
			this.devicesGroup.SuspendLayout();
			this.symbolGroup.SuspendLayout();
			this.logListGroup.SuspendLayout();
			this.controlToolStrip.SuspendLayout();
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
			// toolStripSeparator8
			// 
			toolStripSeparator8.Name = "toolStripSeparator8";
			toolStripSeparator8.Size = new Size(6, 25);
			// 
			// toolStripSeparator9
			// 
			toolStripSeparator9.Name = "toolStripSeparator9";
			toolStripSeparator9.Size = new Size(6, 25);
			// 
			// toolStripSeparator10
			// 
			toolStripSeparator10.Name = "toolStripSeparator10";
			toolStripSeparator10.Size = new Size(6, 25);
			// 
			// mSeverityImageList
			// 
			this.severityImageList.ImageStream = ((ImageListStreamer)(resources.GetObject("mSeverityImageList.ImageStream")));
			this.severityImageList.TransparentColor = Color.Transparent;
			this.severityImageList.Images.SetKeyName(0, string.Empty);
			this.severityImageList.Images.SetKeyName(1, string.Empty);
			this.severityImageList.Images.SetKeyName(2, string.Empty);
			this.severityImageList.Images.SetKeyName(3, string.Empty);
			// 
			// mMainMenuStrip
			// 
			this.mainMenuStrip.Dock = DockStyle.None;
			this.mainMenuStrip.Items.AddRange(new ToolStripItem[] {
						this.fileToolStripMenuItem,
						this.viewToolStripMenuItem,
						this.actionsToolStripMenuItem,
						toolsToolStripMenuItem,
						this.helpToolStripMenuItem});
			this.mainMenuStrip.Location = new Point(0, 0);
			this.mainMenuStrip.Name = "mMainMenuStrip";
			this.mainMenuStrip.Size = new Size(804, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "Main Menu";
			// 
			// mFileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						this.openProgramToolStripMenuItem,
						fileMenuToolStripSeparator,
						this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "mFileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// mOpenProgramToolStripMenuItem
			// 
			this.openProgramToolStripMenuItem.Image = ((Image)(resources.GetObject("mOpenProgramToolStripMenuItem.Image")));
			this.openProgramToolStripMenuItem.Name = "mOpenProgramToolStripMenuItem";
			this.openProgramToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.O);
			this.openProgramToolStripMenuItem.Size = new Size(204, 22);
			this.openProgramToolStripMenuItem.Text = "&Open program...";
			this.openProgramToolStripMenuItem.Click += OpenProgramMenuItem_Click;
			// 
			// mExitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "mExitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new Size(204, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += ExitMenuItem_Click;
			// 
			// mViewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						this.teletypeToolStripMenuItem,
						this.deviceEditorToolStripMenuItem,
						toolStripSeparator10,
						this.showSourceInlineMenuItem,
						toolStripSeparator8,
						this.findMenuItem,
						this.findNextMenuItem});
			this.viewToolStripMenuItem.Name = "mViewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// mTeletypeToolStripMenuItem
			// 
			this.teletypeToolStripMenuItem.Image = ((Image)(resources.GetObject("mTeletypeToolStripMenuItem.Image")));
			this.teletypeToolStripMenuItem.Name = "mTeletypeToolStripMenuItem";
			this.teletypeToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.T);
			this.teletypeToolStripMenuItem.Size = new Size(215, 22);
			this.teletypeToolStripMenuItem.Text = "&Show Teletype";
			this.teletypeToolStripMenuItem.Click += TeletypeItem_Click;
			// 
			// mDeviceEditorToolStripMenuItem
			// 
			this.deviceEditorToolStripMenuItem.Image = ((Image)(resources.GetObject("mDeviceEditorToolStripMenuItem.Image")));
			this.deviceEditorToolStripMenuItem.Name = "mDeviceEditorToolStripMenuItem";
			this.deviceEditorToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.E);
			this.deviceEditorToolStripMenuItem.Size = new Size(215, 22);
			this.deviceEditorToolStripMenuItem.Text = "Show &Device Editor";
			this.deviceEditorToolStripMenuItem.Click += DeviceEditorItem_Click;
			// 
			// this.showSourceInlineMenuItem
			// 
			this.showSourceInlineMenuItem.CheckOnClick = true;
			this.showSourceInlineMenuItem.Name = "_showSourceInlineMenuItem";
			this.showSourceInlineMenuItem.ShortcutKeys = (Keys.Control | Keys.I);
			this.showSourceInlineMenuItem.Size = new Size(215, 22);
			this.showSourceInlineMenuItem.Text = "Show source inline";
			this.showSourceInlineMenuItem.CheckedChanged += ShowSourceInlineMenuItem_CheckedChanged;
			// 
			// mFindMenuItem
			// 
			this.findMenuItem.Image = Properties.Resources.Find_5650;
			this.findMenuItem.Name = "mFindMenuItem";
			this.findMenuItem.ShortcutKeys = (Keys.Control | Keys.F);
			this.findMenuItem.Size = new Size(215, 22);
			this.findMenuItem.Text = "Find in memory...";
			this.findMenuItem.Click += FindMenuItem_Click;
			// 
			// mFindNextMenuItem
			// 
			this.findNextMenuItem.Name = "mFindNextMenuItem";
			this.findNextMenuItem.ShortcutKeys = Keys.F3;
			this.findNextMenuItem.Size = new Size(215, 22);
			this.findNextMenuItem.Text = "Find next";
			this.findNextMenuItem.Click += FindNextMenuItem_Click;
			// 
			// mActionsToolStripMenuItem
			// 
			this.actionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						this.tickToolStripMenuItem,
						this.stepToolStripMenuItem,
						this.runToolStripMenuItem,
						this.goToolStripMenuItem,
						toolStripSeparator1,
						this.detachToolStripMenuItem,
						toolStripSeparator2,
						this.clearBreakpointsToolStripMenuItem,
						this.resetToolStripMenuItem});
			this.actionsToolStripMenuItem.Name = "mActionsToolStripMenuItem";
			this.actionsToolStripMenuItem.Size = new Size(59, 20);
			this.actionsToolStripMenuItem.Text = "&Actions";
			// 
			// mTickToolStripMenuItem
			// 
			this.tickToolStripMenuItem.Image = ((Image)(resources.GetObject("mTickToolStripMenuItem.Image")));
			this.tickToolStripMenuItem.Name = "mTickToolStripMenuItem";
			this.tickToolStripMenuItem.ShortcutKeys = Keys.F11;
			this.tickToolStripMenuItem.Size = new Size(207, 22);
			this.tickToolStripMenuItem.Text = "T&ick";
			this.tickToolStripMenuItem.Click += TickItem_Click;
			// 
			// mStepToolStripMenuItem
			// 
			this.stepToolStripMenuItem.Image = ((Image)(resources.GetObject("mStepToolStripMenuItem.Image")));
			this.stepToolStripMenuItem.Name = "mStepToolStripMenuItem";
			this.stepToolStripMenuItem.ShortcutKeys = Keys.F10;
			this.stepToolStripMenuItem.Size = new Size(207, 22);
			this.stepToolStripMenuItem.Text = "&Step";
			this.stepToolStripMenuItem.Click += StepItem_Click;
			// 
			// mRunToolStripMenuItem
			// 
			this.runToolStripMenuItem.Image = ((Image)(resources.GetObject("mRunToolStripMenuItem.Image")));
			this.runToolStripMenuItem.Name = "mRunToolStripMenuItem";
			this.runToolStripMenuItem.ShortcutKeys = Keys.F5;
			this.runToolStripMenuItem.Size = new Size(207, 22);
			this.runToolStripMenuItem.Text = "R&un";
			this.runToolStripMenuItem.Click += RunItem_Click;
			// 
			// mGoToolStripMenuItem
			// 
			this.goToolStripMenuItem.Image = ((Image)(resources.GetObject("mGoToolStripMenuItem.Image")));
			this.goToolStripMenuItem.Name = "mGoToolStripMenuItem";
			this.goToolStripMenuItem.ShortcutKeys = (Keys.Shift | Keys.F5);
			this.goToolStripMenuItem.Size = new Size(207, 22);
			this.goToolStripMenuItem.Text = "&Go";
			this.goToolStripMenuItem.Click += GoItem_Click;
			// 
			// mDetachToolStripMenuItem
			// 
			this.detachToolStripMenuItem.Name = "mDetachToolStripMenuItem";
			this.detachToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.D);
			this.detachToolStripMenuItem.Size = new Size(207, 22);
			this.detachToolStripMenuItem.Text = "&Detach";
			this.detachToolStripMenuItem.Click += DetachItem_Click;
			// 
			// mClearBreakpointsToolStripMenuItem
			// 
			this.clearBreakpointsToolStripMenuItem.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripMenuItem.Image")));
			this.clearBreakpointsToolStripMenuItem.Name = "mClearBreakpointsToolStripMenuItem";
			this.clearBreakpointsToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.B);
			this.clearBreakpointsToolStripMenuItem.Size = new Size(207, 22);
			this.clearBreakpointsToolStripMenuItem.Text = "Clear &Breakpoints";
			this.clearBreakpointsToolStripMenuItem.Click += ClearBreakpointsItem_Click;
			// 
			// mResetToolStripMenuItem
			// 
			this.resetToolStripMenuItem.Image = ((Image)(resources.GetObject("mResetToolStripMenuItem.Image")));
			this.resetToolStripMenuItem.Name = "mResetToolStripMenuItem";
			this.resetToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.R);
			this.resetToolStripMenuItem.Size = new Size(207, 22);
			this.resetToolStripMenuItem.Text = "&Reset";
			this.resetToolStripMenuItem.Click += ResetItem_Click;
			// 
			// mToolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						this.preferencesToolStripMenuItem,
						toolStripSeparator9,
						this.profilingEnabledMenuItem,
						this.profilingShowTickCountsMenuItem,
						this.profilingResetCountsMenuItem});
			toolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(48, 20);
			toolsToolStripMenuItem.Text = "&Tools";
			// 
			// mPreferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Image = ((Image)(resources.GetObject("mPreferencesToolStripMenuItem.Image")));
			this.preferencesToolStripMenuItem.Name = "mPreferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.P);
			this.preferencesToolStripMenuItem.Size = new Size(185, 22);
			this.preferencesToolStripMenuItem.Text = "&Preferences...";
			this.preferencesToolStripMenuItem.Click += PreferencesMenuItem_Click;
			// 
			// mProfilingEnabledMenuItem
			// 
			this.profilingEnabledMenuItem.CheckOnClick = true;
			this.profilingEnabledMenuItem.Name = "mProfilingEnabledMenuItem";
			this.profilingEnabledMenuItem.Size = new Size(185, 22);
			this.profilingEnabledMenuItem.Text = "&Enable profiling";
			this.profilingEnabledMenuItem.CheckedChanged += ProfilingEnabledMenuItem_CheckedChanged;
			// 
			// mProfilingShowTickCountsMenuItem
			// 
			this.profilingShowTickCountsMenuItem.CheckOnClick = true;
			this.profilingShowTickCountsMenuItem.Name = "mProfilingShowTickCountsMenuItem";
			this.profilingShowTickCountsMenuItem.Size = new Size(185, 22);
			this.profilingShowTickCountsMenuItem.Text = "&Show tick counts";
			this.profilingShowTickCountsMenuItem.CheckedChanged += ProfilingShowTickCountsMenuItem_CheckedChanged;
			// 
			// mHelpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// mAboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((Image)(resources.GetObject("mAboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new Size(116, 22);
			this.aboutToolStripMenuItem.Text = "&About...";
			this.aboutToolStripMenuItem.Click += AboutMenuItem_Click;
			// 
			// mToolStripContainer
			// 
			// 
			// mToolStripContainer.BottomToolStripPanel
			// 
			this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
			// 
			// mToolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Controls.Add(this.splitContainer);
			this.toolStripContainer.ContentPanel.Size = new Size(804, 491);
			this.toolStripContainer.Dock = DockStyle.Fill;
			this.toolStripContainer.LeftToolStripPanelVisible = false;
			this.toolStripContainer.Location = new Point(0, 0);
			this.toolStripContainer.Name = "mToolStripContainer";
			this.toolStripContainer.RightToolStripPanelVisible = false;
			this.toolStripContainer.Size = new Size(804, 562);
			this.toolStripContainer.TabIndex = 0;
			this.toolStripContainer.Text = "ToolStripContainer";
			// 
			// mToolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainMenuStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.controlToolStrip);
			// 
			// mStatusStrip
			// 
			this.statusStrip.Dock = DockStyle.None;
			this.statusStrip.Items.AddRange(new ToolStripItem[] {
						this.modeToolStripStatusLabel,
						this.modeCycleButton,
						this.statusToolStripStatusLabel,
						this.toolStripStatusLabel});
			this.statusStrip.Location = new Point(0, 0);
			this.statusStrip.Name = "mStatusStrip";
			this.statusStrip.Size = new Size(804, 22);
			this.statusStrip.TabIndex = 8;
			// 
			// mModeToolStripStatusLabel
			// 
			this.modeToolStripStatusLabel.Name = "mModeToolStripStatusLabel";
			this.modeToolStripStatusLabel.Size = new Size(41, 17);
			this.modeToolStripStatusLabel.Text = "Mode:";
			// 
			// mModeCycleButton
			// 
			this.modeCycleButton.AutoSize = false;
			this.modeCycleButton.Name = "mModeCycleButton";
			this.modeCycleButton.Size = new Size(60, 20);
			this.modeCycleButton.Value = null;
			// 
			// mStatusToolStripStatusLabel
			// 
			this.statusToolStripStatusLabel.Name = "mStatusToolStripStatusLabel";
			this.statusToolStripStatusLabel.Size = new Size(48, 17);
			this.statusToolStripStatusLabel.Text = "| Status:";
			// 
			// mToolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "mToolStripStatusLabel";
			this.toolStripStatusLabel.Size = new Size(10, 17);
			this.toolStripStatusLabel.Text = " ";
			// 
			// mSplitContainer
			// 
			this.splitContainer.BackColor = SystemColors.Control;
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.FixedPanel = FixedPanel.Panel2;
			this.splitContainer.Location = new Point(0, 0);
			this.splitContainer.Name = "mSplitContainer";
			this.splitContainer.Orientation = Orientation.Horizontal;
			// 
			// mSplitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.memoryGroup);
			this.splitContainer.Panel1.Controls.Add(this.registersGroup);
			this.splitContainer.Panel1.Controls.Add(this.devicesGroup);
			this.splitContainer.Panel1.Controls.Add(this.symbolGroup);
			this.splitContainer.Panel1MinSize = 380;
			// 
			// mSplitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.logListGroup);
			this.splitContainer.Panel2MinSize = 96;
			this.splitContainer.Size = new Size(804, 491);
			this.splitContainer.SplitterDistance = 385;
			this.splitContainer.TabIndex = 6;
			// 
			// mMemoryGroup
			// 
			this.memoryGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.memoryGroup.Controls.Add(this.memoryTabControl);
			this.memoryGroup.Location = new Point(236, -2);
			this.memoryGroup.Name = "mMemoryGroup";
			this.memoryGroup.Size = new Size(568, 328);
			this.memoryGroup.TabIndex = 4;
			this.memoryGroup.TabStop = false;
			this.memoryGroup.Text = "Memory";
			// 
			// mMemoryTabControl
			// 
			this.memoryTabControl.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.memoryTabControl.Controls.Add(this.mainMemoryTab);
			this.memoryTabControl.Controls.Add(this.floatingPointMemoryTab);
			this.memoryTabControl.Location = new Point(4, 16);
			this.memoryTabControl.Name = "mMemoryTabControl";
			this.memoryTabControl.SelectedIndex = 0;
			this.memoryTabControl.Size = new Size(560, 309);
			this.memoryTabControl.TabIndex = 0;
			this.memoryTabControl.SelectedIndexChanged += MemoryTabControl_SelectedIndexChanged;
			this.memoryTabControl.Selecting += MemoryTabControl_Selecting;
			// 
			// mMainMemoryTab
			// 
			this.mainMemoryTab.Controls.Add(this.mainMemoryEditor);
			this.mainMemoryTab.Location = new Point(4, 22);
			this.mainMemoryTab.Name = "mMainMemoryTab";
			this.mainMemoryTab.Padding = new Padding(3);
			this.mainMemoryTab.Size = new Size(552, 283);
			this.mainMemoryTab.TabIndex = 0;
			this.mainMemoryTab.Text = "Main";
			this.mainMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mMainMemoryEditor
			// 
			this.mainMemoryEditor.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.mainMemoryEditor.FirstVisibleAddress = 0;
			this.mainMemoryEditor.IndexedAddressCalculatorCallback = null;
			this.mainMemoryEditor.Location = new Point(0, 0);
			this.mainMemoryEditor.MarkedAddress = -1;
			this.mainMemoryEditor.Memory = null;
			this.mainMemoryEditor.Name = "mMainMemoryEditor";
			this.mainMemoryEditor.ReadOnly = false;
			this.mainMemoryEditor.ResizeInProgress = false;
			this.mainMemoryEditor.Size = new Size(552, 283);
			this.mainMemoryEditor.Symbols = null;
			this.mainMemoryEditor.TabIndex = 0;
			this.mainMemoryEditor.ToolTip = null;
			this.mainMemoryEditor.AddressSelected += MemoryEditor_addressSelected;
			// 
			// mFloatingPointMemoryTab
			// 
			this.floatingPointMemoryTab.Location = new Point(4, 22);
			this.floatingPointMemoryTab.Name = "mFloatingPointMemoryTab";
			this.floatingPointMemoryTab.Padding = new Padding(3);
			this.floatingPointMemoryTab.Size = new Size(552, 283);
			this.floatingPointMemoryTab.TabIndex = 1;
			this.floatingPointMemoryTab.Text = "Floating Point";
			this.floatingPointMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mRegistersGroup
			// 
			this.registersGroup.Controls.Add(this.registersEditor);
			this.registersGroup.Location = new Point(0, -2);
			this.registersGroup.Name = "mRegistersGroup";
			this.registersGroup.Size = new Size(232, 236);
			this.registersGroup.TabIndex = 2;
			this.registersGroup.TabStop = false;
			this.registersGroup.Text = "Registers";
			// 
			// mRegistersEditor
			// 
			this.registersEditor.Location = new Point(8, 16);
			this.registersEditor.Name = "mRegistersEditor";
			this.registersEditor.ReadOnly = false;
			registers1.CompareIndicator = MixLib.Registers.CompValues.Equal;
			registers1.OverflowIndicator = false;
			this.registersEditor.Registers = registers1;
			this.registersEditor.Size = new Size(218, 214);
			this.registersEditor.TabIndex = 0;
			// 
			// mDevicesGroup
			// 
			this.devicesGroup.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.devicesGroup.Controls.Add(this.deviceEditorButton);
			this.devicesGroup.Controls.Add(this.devicesControl);
			this.devicesGroup.Location = new Point(0, 326);
			this.devicesGroup.Name = "mDevicesGroup";
			this.devicesGroup.Size = new Size(804, 56);
			this.devicesGroup.TabIndex = 5;
			this.devicesGroup.TabStop = false;
			this.devicesGroup.Text = "Devices";
			// 
			// mDeviceEditorButton
			// 
			this.deviceEditorButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.deviceEditorButton.Location = new Point(716, 16);
			this.deviceEditorButton.Margin = new Padding(0);
			this.deviceEditorButton.Name = "mDeviceEditorButton";
			this.deviceEditorButton.Size = new Size(80, 23);
			this.deviceEditorButton.TabIndex = 1;
			this.deviceEditorButton.Text = "Show E&ditor";
			this.deviceEditorButton.UseVisualStyleBackColor = true;
			this.deviceEditorButton.FlatStyle = FlatStyle.Flat;
			this.deviceEditorButton.Click += DeviceEditorItem_Click;
			// 
			// mDevicesControl
			// 
			this.devicesControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			this.devicesControl.Devices = null;
			this.devicesControl.Location = new Point(4, 16);
			this.devicesControl.Name = "mDevicesControl";
			this.devicesControl.Size = new Size(706, 36);
			this.devicesControl.Structure = null;
			this.devicesControl.TabIndex = 0;
			this.devicesControl.ToolTip = null;
			// 
			// mSymbolGroup
			// 
			this.symbolGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left);
			this.symbolGroup.Controls.Add(this.symbolListView);
			this.symbolGroup.Location = new Point(0, 234);
			this.symbolGroup.Name = "mSymbolGroup";
			this.symbolGroup.Size = new Size(232, 92);
			this.symbolGroup.TabIndex = 3;
			this.symbolGroup.TabStop = false;
			this.symbolGroup.Text = "Symbols";
			// 
			// mSymbolListView
			// 
			this.symbolListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.symbolListView.Location = new Point(8, 16);
			this.symbolListView.MemoryMaxIndex = 0;
			this.symbolListView.MemoryMinIndex = 0;
			this.symbolListView.Name = "mSymbolListView";
			this.symbolListView.Size = new Size(218, 70);
			this.symbolListView.Symbols = null;
			this.symbolListView.TabIndex = 0;
			this.symbolListView.AddressSelected += ListViews_addressSelected;
			// 
			// mLogListGroup
			// 
			this.logListGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.logListGroup.Controls.Add(this.logListView);
			this.logListGroup.Location = new Point(0, 3);
			this.logListGroup.Name = "mLogListGroup";
			this.logListGroup.Size = new Size(804, 95);
			this.logListGroup.TabIndex = 7;
			this.logListGroup.TabStop = false;
			this.logListGroup.Text = "Messages";
			// 
			// mLogListView
			// 
			this.logListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			this.logListView.Location = new Point(8, 16);
			this.logListView.Name = "mLogListView";
			this.logListView.SeverityImageList = this.severityImageList;
			this.logListView.Size = new Size(788, 73);
			this.logListView.TabIndex = 0;
			this.logListView.AddressSelected += ListViews_addressSelected;
			// 
			// mControlToolStrip
			// 
			this.controlToolStrip.Dock = DockStyle.None;
			this.controlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.controlToolStrip.Items.AddRange(new ToolStripItem[] 
			{
				this.pcToolStripLabel,
				this.showPCToolStripButton,
				toolStripSeparator3,
				this.ticksToolStripLabel,
				this.ticksToolStripTextBox,
				this.resetTicksToolStripButton,
				toolStripSeparator4,
				this.tickToolStripButton,
				this.stepToolStripButton,
				this.runToolStripButton,
				this.goToolStripButton,
				toolStripSeparator5,
				this.detachToolStripButton,
				toolStripSeparator6,
				this.clearBreakpointsToolStripButton,
				this.resetToolStripButton,
				toolStripSeparator7,
				this.teletypeToolStripButton
			});

			this.controlToolStrip.Location = new Point(3, 24);
			this.controlToolStrip.Name = "mControlToolStrip";
			this.controlToolStrip.Size = new Size(761, 25);
			this.controlToolStrip.TabIndex = 1;
			// 
			// mPCToolStripLabel
			// 
			this.pcToolStripLabel.Name = "mPCToolStripLabel";
			this.pcToolStripLabel.Size = new Size(28, 22);
			this.pcToolStripLabel.Text = "PC: ";
			// 
			// mShowPCToolStripButton
			// 
			this.showPCToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.showPCToolStripButton.Image = ((Image)(resources.GetObject("mShowPCToolStripButton.Image")));
			this.showPCToolStripButton.ImageTransparentColor = Color.Magenta;
			this.showPCToolStripButton.Name = "mShowPCToolStripButton";
			this.showPCToolStripButton.Size = new Size(40, 22);
			this.showPCToolStripButton.Text = "Sh&ow";
			this.showPCToolStripButton.ToolTipText = "Show the program counter";
			this.showPCToolStripButton.Click += ShowPCButton_Click;
			// 
			// mTicksToolStripLabel
			// 
			this.ticksToolStripLabel.Name = "mTicksToolStripLabel";
			this.ticksToolStripLabel.Size = new Size(40, 22);
			this.ticksToolStripLabel.Text = "Ticks: ";
			// 
			// mTicksToolStripTextBox
			// 
			this.ticksToolStripTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.ticksToolStripTextBox.Name = "mTicksToolStripTextBox";
			this.ticksToolStripTextBox.ReadOnly = true;
			this.ticksToolStripTextBox.Size = new Size(64, 25);
			this.ticksToolStripTextBox.Text = "0";
			// 
			// mResetTicksToolStripButton
			// 
			this.resetTicksToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.resetTicksToolStripButton.Image = ((Image)(resources.GetObject("mResetTicksToolStripButton.Image")));
			this.resetTicksToolStripButton.ImageTransparentColor = Color.Magenta;
			this.resetTicksToolStripButton.Name = "mResetTicksToolStripButton";
			this.resetTicksToolStripButton.Size = new Size(39, 22);
			this.resetTicksToolStripButton.Text = "&Reset";
			this.resetTicksToolStripButton.ToolTipText = "Reset the tick counter";
			this.resetTicksToolStripButton.Click += ResetTicksButton_Click;
			// 
			// mTickToolStripButton
			// 
			this.tickToolStripButton.Image = ((Image)(resources.GetObject("mTickToolStripButton.Image")));
			this.tickToolStripButton.ImageTransparentColor = Color.Magenta;
			this.tickToolStripButton.Name = "mTickToolStripButton";
			this.tickToolStripButton.Size = new Size(49, 22);
			this.tickToolStripButton.Text = "T&ick";
			this.tickToolStripButton.ToolTipText = "Execute one tick";
			this.tickToolStripButton.Click += TickItem_Click;
			// 
			// mStepToolStripButton
			// 
			this.stepToolStripButton.Image = ((Image)(resources.GetObject("mStepToolStripButton.Image")));
			this.stepToolStripButton.ImageTransparentColor = Color.Magenta;
			this.stepToolStripButton.Name = "mStepToolStripButton";
			this.stepToolStripButton.Size = new Size(50, 22);
			this.stepToolStripButton.Text = "Ste&p";
			this.stepToolStripButton.ToolTipText = "Execute one step";
			this.stepToolStripButton.Click += StepItem_Click;
			// 
			// mRunToolStripButton
			// 
			this.runToolStripButton.Image = ((Image)(resources.GetObject("mRunToolStripButton.Image")));
			this.runToolStripButton.ImageTransparentColor = Color.Magenta;
			this.runToolStripButton.Name = "mRunToolStripButton";
			this.runToolStripButton.Size = new Size(48, 22);
			this.runToolStripButton.Text = "R&un";
			this.runToolStripButton.ToolTipText = "Start or stop a full run";
			this.runToolStripButton.Click += RunItem_Click;
			// 
			// mGoToolStripButton
			// 
			this.goToolStripButton.Image = ((Image)(resources.GetObject("mGoToolStripButton.Image")));
			this.goToolStripButton.ImageTransparentColor = Color.Magenta;
			this.goToolStripButton.Name = "mGoToolStripButton";
			this.goToolStripButton.Size = new Size(42, 22);
			this.goToolStripButton.Text = "&Go";
			this.goToolStripButton.ToolTipText = "Start the MIX loader";
			this.goToolStripButton.Click += GoItem_Click;
			// 
			// mDetachToolStripButton
			// 
			this.detachToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.detachToolStripButton.Image = ((Image)(resources.GetObject("mDetachToolStripButton.Image")));
			this.detachToolStripButton.ImageTransparentColor = Color.Magenta;
			this.detachToolStripButton.Name = "mDetachToolStripButton";
			this.detachToolStripButton.Size = new Size(48, 22);
			this.detachToolStripButton.Text = "Detac&h";
			this.detachToolStripButton.ToolTipText = "Switch between running MIX in the foreground or background";
			this.detachToolStripButton.Click += DetachItem_Click;
			// 
			// mClearBreakpointsToolStripButton
			// 
			this.clearBreakpointsToolStripButton.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripButton.Image")));
			this.clearBreakpointsToolStripButton.ImageTransparentColor = Color.Magenta;
			this.clearBreakpointsToolStripButton.Name = "mClearBreakpointsToolStripButton";
			this.clearBreakpointsToolStripButton.Size = new Size(119, 22);
			this.clearBreakpointsToolStripButton.Text = "Clear &Breakpoints";
			this.clearBreakpointsToolStripButton.ToolTipText = "Remove all set breakpoints";
			this.clearBreakpointsToolStripButton.Click += ClearBreakpointsItem_Click;
			// 
			// mResetToolStripButton
			// 
			this.resetToolStripButton.Image = ((Image)(resources.GetObject("mResetToolStripButton.Image")));
			this.resetToolStripButton.ImageTransparentColor = Color.Magenta;
			this.resetToolStripButton.Name = "mResetToolStripButton";
			this.resetToolStripButton.Size = new Size(55, 22);
			this.resetToolStripButton.Text = "R&eset";
			this.resetToolStripButton.ToolTipText = "Reset MIX to initial state";
			this.resetToolStripButton.Click += ResetItem_Click;
			// 
			// mTeletypeToolStripButton
			// 
			this.teletypeToolStripButton.Image = ((Image)(resources.GetObject("mTeletypeToolStripButton.Image")));
			this.teletypeToolStripButton.ImageTransparentColor = Color.Magenta;
			this.teletypeToolStripButton.Name = "mTeletypeToolStripButton";
			this.teletypeToolStripButton.Size = new Size(104, 22);
			this.teletypeToolStripButton.Text = "Show &Teletype";
			this.teletypeToolStripButton.ToolTipText = "Show or hide the teletype";
			this.teletypeToolStripButton.Click += TeletypeItem_Click;
			// 
			// mToolTip
			// 
			this.toolTip.AutoPopDelay = 10000;
			this.toolTip.InitialDelay = 100;
			this.toolTip.ReshowDelay = 100;
			this.toolTip.ShowAlways = true;
			// 
			// mProfilingResetCountsMenuItem
			// 
			this.profilingResetCountsMenuItem.Name = "mProfilingResetCountsMenuItem";
			this.profilingResetCountsMenuItem.Size = new Size(185, 22);
			this.profilingResetCountsMenuItem.Text = "&Reset counts";
			this.profilingResetCountsMenuItem.Click += ProfilingResetCountsMenuItem_Click;
			// 
			// MixForm
			// 
			ClientSize = new Size(804, 562);
			Controls.Add(this.toolStripContainer);
			Icon = ((Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			MainMenuStrip = this.mainMenuStrip;
			MinimumSize = new Size(820, 600);
			Name = "MixForm";
			StartPosition = FormStartPosition.Manual;
			Text = "MixEmul";
			FormClosing += This_FormClosing;
			ResizeBegin += This_ResizeBegin;
			ResizeEnd += This_ResizeEnd;
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.memoryGroup.ResumeLayout(false);
			this.memoryTabControl.ResumeLayout(false);
			this.mainMemoryTab.ResumeLayout(false);
			this.registersGroup.ResumeLayout(false);
			this.devicesGroup.ResumeLayout(false);
			this.symbolGroup.ResumeLayout(false);
			this.logListGroup.ResumeLayout(false);
			this.controlToolStrip.ResumeLayout(false);
			this.controlToolStrip.PerformLayout();
			ResumeLayout(false);

		}

		private void LoadProgram(string filePath)
		{
			if (filePath == null)
				return;

			var instances = Assembler.Assemble(File.ReadAllLines(filePath), out PreInstruction[] instructions, out SymbolCollection symbols, out AssemblyFindingCollection findings);
			if (instructions != null && findings != null)
			{
				if (this.sourceAndFindingsForm == null)
				{
					this.sourceAndFindingsForm = new SourceAndFindingsForm
					{
						SeverityImageList = this.severityImageList
					};
				}

				this.sourceAndFindingsForm.SetInstructionsAndFindings(instructions, instances, findings);

				if (this.sourceAndFindingsForm.ShowDialog(this) != DialogResult.OK)
					return;
			}

			if (instances != null)
			{
				this.symbolListView.Symbols = symbols;
				this.mainMemoryEditor.Symbols = symbols;
				this.mix.LoadInstructionInstances(instances, symbols);
				Update();
			}
		}

		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.ThreadException += MixForm.Application_ThreadException;
			AppDomain.CurrentDomain.UnhandledException += MixForm.CurrentDomain_UnhandledException;
			var mixForm = new MixForm();
			Application.Run(mixForm);
		}

		private void SwitchRunningState()
		{
			if (this.steppingState == SteppingState.Idle)
			{
				SetSteppingState(SteppingState.Stepping);
				this.running = true;
				this.mix.StartRun();
				UpdateStatusIndicator();
			}
			else
			{
				this.running = false;
				this.mix.RequestStop();
			}
		}

		private void PcChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			if (!this.updating)
				ImplementPCChange((int)args.NewValue);
		}

		private void SetSteppingState(SteppingState state)
		{
			if (this.steppingState != state)
			{
				this.steppingState = state;

				if (this.steppingState == SteppingState.Idle)
				{
					ReadOnly = false;
					this.runToolStripButton.Text = "R&un";
					this.runToolStripMenuItem.Text = "R&un";
				}
				else
				{
					ReadOnly = true;
					this.runToolStripButton.Text = "St&op";
					this.runToolStripMenuItem.Text = "St&op";
				}
			}
		}

		private void SwitchTeletypeVisibility()
		{
			if (this.teletype.Visible)
				this.teletype.Hide();

			else
				this.teletype.Show();
		}

		private void UpdateStatusIndicator()
			=> this.toolStripStatusLabel.Text = this.mix.Status switch
			{
				ModuleBase.RunStatus.InvalidInstruction => "Encountered invalid instruction",
				ModuleBase.RunStatus.RuntimeError => "Runtime error occured",
				ModuleBase.RunStatus.BreakpointReached => "Breakpoint reached",
				_ => this.mix.Status.ToString()
			};

		public new void Update()
		{
			this.updating = true;
			var pcVisible = this.mainMemoryEditor.IsAddressVisible((int)this.pcBox.LongValue);
			SetPCBoxBounds();
			this.pcBox.LongValue = this.mix.ProgramCounter;
			this.mainMemoryEditor.MarkedAddress = this.mix.ProgramCounter;

			if (this.floatingPointMemoryEditor != null)
			{
				var floatPcVisible = this.floatingPointMemoryEditor.IsAddressVisible(this.floatingPointMemoryEditor.MarkedAddress);
				this.floatingPointMemoryEditor.MarkedAddress = this.mix.FloatingPointModule.ProgramCounter;

				if (floatPcVisible)
					this.floatingPointMemoryEditor.MakeAddressVisible(this.floatingPointMemoryEditor.MarkedAddress);
			}

			this.ticksToolStripTextBox.Text = this.mix.TickCounter.ToString();

			UpdateStatusIndicator();

			if (this.mix.Status == ModuleBase.RunStatus.InvalidInstruction || this.mix.Status == ModuleBase.RunStatus.RuntimeError)
					pcVisible = true;

			if (pcVisible)
				this.mainMemoryEditor.MakeAddressVisible(this.mix.ProgramCounter, this.mix.Status == ModuleBase.RunStatus.Idle);

			this.mix.NeutralizeStatus();

			this.modeCycleButton.Value = this.mix.Mode;

			this.registersEditor.Update();
			this.mainMemoryEditor.Update();
			this.floatingPointMemoryEditor?.Update();
			this.devicesControl.Update();
			base.Update();
			this.updating = false;
		}

		private void UpdateDeviceConfig()
		{
			DeviceSettings.DeviceFilesDirectory = this.configuration.DeviceFilesDirectory;
			DeviceSettings.TickCounts = this.configuration.TickCounts;
			DeviceSettings.DeviceReloadInterval = this.configuration.DeviceReloadInterval;
			this.mix.Devices.UpdateSettings();

			foreach (var device in this.mix.Devices.OfType<FileBasedDevice>())
			{
				device.FilePath = this.configuration.DeviceFilePaths.ContainsKey(device.Id)
					? this.configuration.DeviceFilePaths[device.Id]
					: null;
			}
		}

		public void UpdateLayout()
		{
			GuiSettings.Colors = this.configuration.Colors;
			this.pcBox.UpdateLayout();
			this.teletype.UpdateLayout();
			this.registersEditor.UpdateLayout();

			foreach (MemoryEditor editor in this.memoryEditors)
				editor?.UpdateLayout();

			this.devicesControl.UpdateLayout();
			this.sourceAndFindingsForm?.UpdateLayout();
			this.preferencesForm?.UpdateLayout();
			this.deviceEditor.UpdateLayout();
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (this.readOnly == value)
					return;

				this.readOnly = value;
				this.pcBox.ReadOnly = this.readOnly;
				this.resetTicksToolStripButton.Enabled = !this.readOnly;
				this.stepToolStripButton.Enabled = !this.readOnly;
				this.stepToolStripMenuItem.Enabled = !ReadOnly;
				this.tickToolStripButton.Enabled = !this.readOnly;
				this.tickToolStripMenuItem.Enabled = !this.readOnly;
				this.goToolStripButton.Enabled = !this.readOnly;
				this.goToolStripMenuItem.Enabled = !this.readOnly;
				this.resetToolStripButton.Enabled = !this.readOnly;
				this.resetToolStripMenuItem.Enabled = !this.readOnly;
				this.registersEditor.ReadOnly = this.readOnly;

				foreach (MemoryEditor editor in this.memoryEditors.Where(editor => editor != null))
					editor.ReadOnly = this.readOnly;

				this.mainMemoryEditor.ReadOnly = this.readOnly;
				this.openProgramToolStripMenuItem.Enabled = !this.readOnly;
				this.preferencesToolStripMenuItem.Enabled = !this.readOnly;
			}
		}

		private enum SteppingState
		{
			Idle,
			Stepping
		}

		private void ImplementFloatingPointPCChange(int address)
		{
			if (this.floatingPointMemoryEditor != null && this.floatingPointMemoryEditor.IsAddressVisible(this.mix.FloatingPointModule.ProgramCounter))
				this.floatingPointMemoryEditor.MakeAddressVisible(address, this.mix.Status == ModuleBase.RunStatus.Idle);

			this.mix.FloatingPointModule.ProgramCounter = address;

			if (this.floatingPointMemoryEditor != null)
				this.floatingPointMemoryEditor.MarkedAddress = this.mix.FloatingPointModule.ProgramCounter;
		}

	}
}
