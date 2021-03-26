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

		private IContainer _components;
		private AboutForm _aboutForm;
		private readonly Configuration _configuration;
		private readonly string _defaultDirectory = Environment.CurrentDirectory;
		private readonly Mix _mix;
		private OpenFileDialog _openProgramFileDialog;
		private PreferencesForm _preferencesForm;
		private bool _readOnly;
		private bool _running;
		private bool _updating;
		private ImageList _severityImageList;
		private SourceAndFindingsForm _sourceAndFindingsForm;
		private SteppingState _steppingState;
		private readonly TeletypeForm _teletype;
		private MenuStrip _mainMenuStrip;
		private ToolStripMenuItem _fileToolStripMenuItem;
		private ToolStripMenuItem _openProgramToolStripMenuItem;
		private ToolStripMenuItem _exitToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem _preferencesToolStripMenuItem;
		private ToolStripMenuItem _helpToolStripMenuItem;
		private ToolStripMenuItem _aboutToolStripMenuItem;
		private ToolStripMenuItem _actionsToolStripMenuItem;
		private ToolStripMenuItem _tickToolStripMenuItem;
		private ToolStripMenuItem _stepToolStripMenuItem;
		private ToolStripMenuItem _runToolStripMenuItem;
		private ToolStripMenuItem _goToolStripMenuItem;
		private ToolStripMenuItem _detachToolStripMenuItem;
		private ToolStripMenuItem _resetToolStripMenuItem;
		private ToolStripMenuItem _viewToolStripMenuItem;
		private ToolStripMenuItem _teletypeToolStripMenuItem;
		private ToolStripContainer _toolStripContainer;
		private StatusStrip _statusStrip;
		private ToolStripStatusLabel _toolStripStatusLabel;
		private readonly LongValueTextBox _pcBox;
		private ToolStrip _controlToolStrip;
		private ToolStripLabel _pcToolStripLabel;
		private ToolStripButton _showPCToolStripButton;
		private ToolStripLabel _ticksToolStripLabel;
		private ToolStripTextBox _ticksToolStripTextBox;
		private ToolStripButton _resetTicksToolStripButton;
		private ToolStripButton _tickToolStripButton;
		private ToolStripButton _stepToolStripButton;
		private ToolStripButton _runToolStripButton;
		private ToolStripButton _goToolStripButton;
		private ToolStripButton _detachToolStripButton;
		private ToolStripButton _resetToolStripButton;
		private ToolStripButton _teletypeToolStripButton;
		private SplitContainer _splitContainer;
		private GroupBox _memoryGroup;
		private MemoryEditor _mainMemoryEditor;
		private MemoryEditor _floatingPointMemoryEditor;
		private readonly MemoryEditor[] _memoryEditors;
		private GroupBox _registersGroup;
		private RegistersEditor _registersEditor;
		private GroupBox _devicesGroup;
		private DevicesControl _devicesControl;
		private GroupBox _symbolGroup;
		private SymbolListView _symbolListView;
		private GroupBox _logListGroup;
		private LogListView _logListView;
		private ToolStripButton _clearBreakpointsToolStripButton;
		private ToolStripMenuItem _clearBreakpointsToolStripMenuItem;
		private Button _deviceEditorButton;
		private ToolStripMenuItem _deviceEditorToolStripMenuItem;
		private ToolStripCycleButton _modeCycleButton;
		private TabControl _memoryTabControl;
		private TabPage _mainMemoryTab;
		private TabPage _floatingPointMemoryTab;
		private ToolStripStatusLabel _modeToolStripStatusLabel;
		private ToolStripStatusLabel _statusToolStripStatusLabel;
		private ToolTip _toolTip;
		private ToolStripSeparator _toolStripSeparator8;
		private ToolStripMenuItem _findMenuItem;
		private ToolStripMenuItem _findNextMenuItem;
		private readonly DeviceEditorForm _deviceEditor;
		private SearchDialog _searchDialog;
		private ToolStripSeparator _toolStripSeparator9;
		private ToolStripMenuItem _profilingEnabledMenuItem;
		private ToolStripMenuItem _profilingShowTickCountsMenuItem;
		private ToolStripMenuItem _profilingResetCountsMenuItem;
		private SearchParameters _searchOptions;

		public MixForm()
		{
			_configuration = Configuration.Load(_defaultDirectory);
			ExecutionSettings.ProfilingEnabled = _configuration.ProfilingEnabled;
			GuiSettings.Colors = _configuration.Colors;
			GuiSettings.ShowProfilingInfo = _configuration.ShowProfilingInfo;
			GuiSettings.ColorProfilingCounts = _configuration.ColorProfilingCounts;
			DeviceSettings.DefaultDeviceFilesDirectory = _defaultDirectory;

			if (_configuration.FloatingPointMemoryWordCount != null)
				ModuleSettings.FloatingPointMemoryWordCount = _configuration.FloatingPointMemoryWordCount.Value;

			InitializeComponent();

			_memoryEditors = new MemoryEditor[2];
			_memoryEditors[0] = _mainMemoryEditor;
			_memoryEditors[1] = null;

			_mix = new Mix();
			_mix.StepPerformed += Mix_StepPerformed;
			_mix.LogLineAdded += Mix_LogLineAdded;

			_pcBox = new LongValueTextBox
			{
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = Color.Black,
				LongValue = _mix.ProgramCounter,
				ClearZero = false
			};

			SetPCBoxBounds();
			_pcBox.Name = "mPCBox";
			_pcBox.Size = new Size(40, 25);
			_pcBox.TabIndex = 1;
			_pcBox.Text = "0";
			_pcBox.ValueChanged += PcChanged;
			_controlToolStrip.Items.Insert(1, new LongValueToolStripTextBox(_pcBox));

			_registersEditor.Registers = _mix.Registers;

			_mainMemoryEditor.ToolTip = _toolTip;
			_mainMemoryEditor.Memory = _mix.FullMemory;
			_mainMemoryEditor.MarkedAddress = _mix.ProgramCounter;
			_mainMemoryEditor.IndexedAddressCalculatorCallback = CalculateIndexedAddress;

			_profilingEnabledMenuItem.Checked = ExecutionSettings.ProfilingEnabled;
			_profilingShowTickCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;
			_profilingShowTickCountsMenuItem.Checked = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick;
			_profilingResetCountsMenuItem.Enabled = ExecutionSettings.ProfilingEnabled;

			_symbolListView.MemoryMinIndex = _mix.FullMemory.MinWordIndex;
			_symbolListView.MemoryMaxIndex = _mix.FullMemory.MaxWordIndex;

			var layoutStructure = new DevicesControl.LayoutStructure(DevicesControl.Orientations.Horizontal, 8, 4);
			layoutStructure[8] = DevicesControl.Breaks.Sequence;
			layoutStructure[16] = DevicesControl.Breaks.Section;
			layoutStructure[18] = DevicesControl.Breaks.Sequence;

			_devicesControl.ToolTip = _toolTip;
			_devicesControl.Structure = layoutStructure;
			_devicesControl.Devices = _mix.Devices;
			_devicesControl.DeviceDoubleClick += DevicesControl_DeviceDoubleClick;

			_teletype = new TeletypeForm(_mix.Devices.Teletype);
			_teletype.VisibleChanged += Teletype_VisibleChanged;

			if (_configuration.TeletypeWindowLocation != Point.Empty)
			{
				_teletype.StartPosition = FormStartPosition.Manual;
				_teletype.Location = _configuration.TeletypeWindowLocation;
			}

			if (_configuration.TeletypeWindowSize != Size.Empty)
				_teletype.Size = _configuration.TeletypeWindowSize;

			_teletype.TopMost = _configuration.TeletypeOnTop;
			_teletype.EchoInput = _configuration.TeletypeEchoInput;

			_teletype.AddOutputText("*** MIX TELETYPE INITIALIZED ***");

			if (_configuration.TeletypeVisible)
				_teletype.Show();

			CardDeckExporter.LoaderCards = _configuration.LoaderCards;
			UpdateDeviceConfig();

			_deviceEditor = new DeviceEditorForm(_mix.Devices);
			_deviceEditor.VisibleChanged += DeviceEditor_VisibleChanged;

			if (_configuration.DeviceEditorWindowLocation != Point.Empty)
			{
				_deviceEditor.StartPosition = FormStartPosition.Manual;
				_deviceEditor.Location = _configuration.DeviceEditorWindowLocation;
			}

			if (_configuration.DeviceEditorWindowSize != Size.Empty)
				_deviceEditor.Size = _configuration.DeviceEditorWindowSize;

			if (_configuration.DeviceEditorVisible)
				_deviceEditor.Show();

			_mix.Devices.Teletype.InputRequired += Mix_InputRequired;
			_mix.BreakpointManager = _mainMemoryEditor;

			LoadControlProgram();
			ApplyFloatingPointSettings();

			_openProgramFileDialog = null;
			_sourceAndFindingsForm = null;
			_readOnly = false;
			_running = false;
			_steppingState = SteppingState.Idle;

			var symbols = new SymbolCollection();
			_symbolListView.Symbols = symbols;
			_mainMemoryEditor.Symbols = symbols;

			var controlStep = _modeCycleButton.AddStep(ModuleBase.RunMode.Control);
			var normalStep = _modeCycleButton.AddStep(ModuleBase.RunMode.Normal, controlStep);
			_modeCycleButton.AddStep(ModuleBase.RunMode.Module, normalStep);
			controlStep.NextStep = normalStep;

			_modeCycleButton.Value = _mix.Mode;
			_modeCycleButton.ValueChanged += ModeCycleButton_ValueChanged;

			Update();

			if (_configuration.MainWindowLocation != Point.Empty)
			{
				StartPosition = FormStartPosition.Manual;
				Location = _configuration.MainWindowLocation;
			}
			else
				StartPosition = FormStartPosition.CenterScreen;

			if (_configuration.MainWindowSize != Size.Empty)
				Size = _configuration.MainWindowSize;

			WindowState = _configuration.MainWindowState;
			SizeChanged += This_SizeChanged;
			LocationChanged += This_LocationChanged;
		}

		private MemoryEditor ActiveMemoryEditor => _memoryEditors[_memoryTabControl.SelectedIndex];

		private void FindNext() => ActiveMemoryEditor.FindMatch(_searchOptions);

		private int CalculateIndexedAddress(int address, int index) => InstructionHelpers.GetValidIndexedAddress(_mix, address, index, false);

		private void SetPCBoxBounds()
		{
			_pcBox.MinValue = _mix.Memory.MinWordIndex;
			_pcBox.MaxValue = _mix.Memory.MaxWordIndex;
		}

		private void ApplyFloatingPointSettings()
		{
			_mix.SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);

			if (ModuleSettings.FloatingPointEnabled)
				LoadModuleProgram(_mix.FloatingPointModule, ModuleSettings.FloatingPointProgramFile, "floating point");
		}

		private void LoadControlProgram()
		{
			string controlProgramFileName = ModuleSettings.ControlProgramFile;

			if (string.IsNullOrEmpty(controlProgramFileName) || !File.Exists(controlProgramFileName))
				return;

			LoadModuleProgram(_mix, controlProgramFileName, "control");
			_mix.FullMemory.ClearSourceLines();
		}

		private SymbolCollection LoadModuleProgram(ModuleBase module, string fileName, string programName)
		{
			try
			{
				var instances = Assembler.Assemble(File.ReadAllLines(fileName), out PreInstruction[] instructions, out SymbolCollection symbols, out AssemblyFindingCollection findings);
				if (instances != null)
				{
					if (!module.LoadInstructionInstances(instances, symbols))
						_logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to load {programName} program"));
				}
				else
				{
					if (findings != null)
					{
						_logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Parse failed", $"Error(s) occured while assembling {programName} program"));
						foreach (AssemblyFinding finding in findings)
							_logListView.AddLogLine(new LogLine("Assembler", finding.Severity, "Parse error", finding.LineNumber == int.MinValue ? finding.Message : $"Line {finding.LineNumber}: {finding.Message}"));
					}
					_logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to assemble {programName} program"));

					return symbols;

				}
			}
			catch (Exception ex)
			{
				_logListView.AddLogLine(new LogLine("Assembler", Severity.Error, "Load failed", $"Failed to read {programName} program: {ex.Message}"));
			}

			return null;
		}

		private bool DisableTeletypeOnTop()
		{
			if (_teletype != null && _teletype.Visible && _teletype.TopMost)
			{
				_teletype.TopMost = false;
				return true;
			}

			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_components?.Dispose();
				_mix?.Dispose();
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
			if (_mainMemoryEditor.IsAddressVisible(_mix.ProgramCounter))
				_mainMemoryEditor.MakeAddressVisible(address, _mix.Status == ModuleBase.RunStatus.Idle);

			_mix.ProgramCounter = address;
			_mainMemoryEditor.MarkedAddress = _mix.ProgramCounter;
			_pcBox.LongValue = _mix.ProgramCounter;
		}

		private void InitializeComponent()
		{
			_components = new Container();
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
			_severityImageList = new ImageList(_components);
			_mainMenuStrip = new MenuStrip();
			_fileToolStripMenuItem = new ToolStripMenuItem();
			_openProgramToolStripMenuItem = new ToolStripMenuItem();
			_exitToolStripMenuItem = new ToolStripMenuItem();
			_viewToolStripMenuItem = new ToolStripMenuItem();
			_teletypeToolStripMenuItem = new ToolStripMenuItem();
			_deviceEditorToolStripMenuItem = new ToolStripMenuItem();
			_toolStripSeparator8 = new ToolStripSeparator();
			_findMenuItem = new ToolStripMenuItem();
			_findNextMenuItem = new ToolStripMenuItem();
			_actionsToolStripMenuItem = new ToolStripMenuItem();
			_tickToolStripMenuItem = new ToolStripMenuItem();
			_stepToolStripMenuItem = new ToolStripMenuItem();
			_runToolStripMenuItem = new ToolStripMenuItem();
			_goToolStripMenuItem = new ToolStripMenuItem();
			_detachToolStripMenuItem = new ToolStripMenuItem();
			_clearBreakpointsToolStripMenuItem = new ToolStripMenuItem();
			_resetToolStripMenuItem = new ToolStripMenuItem();
			toolsToolStripMenuItem = new ToolStripMenuItem();
			_preferencesToolStripMenuItem = new ToolStripMenuItem();
			_toolStripSeparator9 = new ToolStripSeparator();
			_profilingEnabledMenuItem = new ToolStripMenuItem();
			_profilingShowTickCountsMenuItem = new ToolStripMenuItem();
			_helpToolStripMenuItem = new ToolStripMenuItem();
			_aboutToolStripMenuItem = new ToolStripMenuItem();
			_toolStripContainer = new ToolStripContainer();
			_statusStrip = new StatusStrip();
			_modeToolStripStatusLabel = new ToolStripStatusLabel();
			_modeCycleButton = new ToolStripCycleButton(_components);
			_statusToolStripStatusLabel = new ToolStripStatusLabel();
			_toolStripStatusLabel = new ToolStripStatusLabel();
			_splitContainer = new SplitContainer();
			_memoryGroup = new GroupBox();
			_memoryTabControl = new TabControl();
			_mainMemoryTab = new TabPage();
			_mainMemoryEditor = new MemoryEditor();
			_floatingPointMemoryTab = new TabPage();
			_registersGroup = new GroupBox();
			_registersEditor = new RegistersEditor();
			_devicesGroup = new GroupBox();
			_deviceEditorButton = new Button();
			_devicesControl = new DevicesControl();
			_symbolGroup = new GroupBox();
			_symbolListView = new SymbolListView();
			_logListGroup = new GroupBox();
			_logListView = new LogListView();
			_controlToolStrip = new ToolStrip();
			_pcToolStripLabel = new ToolStripLabel();
			_showPCToolStripButton = new ToolStripButton();
			_ticksToolStripLabel = new ToolStripLabel();
			_ticksToolStripTextBox = new ToolStripTextBox();
			_resetTicksToolStripButton = new ToolStripButton();
			_tickToolStripButton = new ToolStripButton();
			_stepToolStripButton = new ToolStripButton();
			_runToolStripButton = new ToolStripButton();
			_goToolStripButton = new ToolStripButton();
			_detachToolStripButton = new ToolStripButton();
			_clearBreakpointsToolStripButton = new ToolStripButton();
			_resetToolStripButton = new ToolStripButton();
			_teletypeToolStripButton = new ToolStripButton();
			_toolTip = new ToolTip(_components);
			_profilingResetCountsMenuItem = new ToolStripMenuItem();
			fileMenuToolStripSeparator = new ToolStripSeparator();
			toolStripSeparator1 = new ToolStripSeparator();
			toolStripSeparator2 = new ToolStripSeparator();
			toolStripSeparator3 = new ToolStripSeparator();
			toolStripSeparator4 = new ToolStripSeparator();
			toolStripSeparator5 = new ToolStripSeparator();
			toolStripSeparator6 = new ToolStripSeparator();
			toolStripSeparator7 = new ToolStripSeparator();
			_mainMenuStrip.SuspendLayout();
			_toolStripContainer.BottomToolStripPanel.SuspendLayout();
			_toolStripContainer.ContentPanel.SuspendLayout();
			_toolStripContainer.TopToolStripPanel.SuspendLayout();
			_toolStripContainer.SuspendLayout();
			_statusStrip.SuspendLayout();
			_splitContainer.Panel1.SuspendLayout();
			_splitContainer.Panel2.SuspendLayout();
			_splitContainer.SuspendLayout();
			_memoryGroup.SuspendLayout();
			_memoryTabControl.SuspendLayout();
			_mainMemoryTab.SuspendLayout();
			_registersGroup.SuspendLayout();
			_devicesGroup.SuspendLayout();
			_symbolGroup.SuspendLayout();
			_logListGroup.SuspendLayout();
			_controlToolStrip.SuspendLayout();
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
			_severityImageList.ImageStream = ((ImageListStreamer)(resources.GetObject("mSeverityImageList.ImageStream")));
			_severityImageList.TransparentColor = Color.Transparent;
			_severityImageList.Images.SetKeyName(0, "");
			_severityImageList.Images.SetKeyName(1, "");
			_severityImageList.Images.SetKeyName(2, "");
			_severityImageList.Images.SetKeyName(3, "");
			// 
			// mMainMenuStrip
			// 
			_mainMenuStrip.Dock = DockStyle.None;
			_mainMenuStrip.Items.AddRange(new ToolStripItem[] {
						_fileToolStripMenuItem,
						_viewToolStripMenuItem,
						_actionsToolStripMenuItem,
						toolsToolStripMenuItem,
						_helpToolStripMenuItem});
			_mainMenuStrip.Location = new Point(0, 0);
			_mainMenuStrip.Name = "mMainMenuStrip";
			_mainMenuStrip.Size = new Size(804, 24);
			_mainMenuStrip.TabIndex = 0;
			_mainMenuStrip.Text = "Main Menu";
			// 
			// mFileToolStripMenuItem
			// 
			_fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						_openProgramToolStripMenuItem,
						fileMenuToolStripSeparator,
						_exitToolStripMenuItem});
			_fileToolStripMenuItem.Name = "mFileToolStripMenuItem";
			_fileToolStripMenuItem.Size = new Size(37, 20);
			_fileToolStripMenuItem.Text = "&File";
			// 
			// mOpenProgramToolStripMenuItem
			// 
			_openProgramToolStripMenuItem.Image = ((Image)(resources.GetObject("mOpenProgramToolStripMenuItem.Image")));
			_openProgramToolStripMenuItem.Name = "mOpenProgramToolStripMenuItem";
			_openProgramToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.O);
			_openProgramToolStripMenuItem.Size = new Size(204, 22);
			_openProgramToolStripMenuItem.Text = "&Open program...";
			_openProgramToolStripMenuItem.Click += OpenProgramMenuItem_Click;
			// 
			// mExitToolStripMenuItem
			// 
			_exitToolStripMenuItem.Name = "mExitToolStripMenuItem";
			_exitToolStripMenuItem.Size = new Size(204, 22);
			_exitToolStripMenuItem.Text = "E&xit";
			_exitToolStripMenuItem.Click += ExitMenuItem_Click;
			// 
			// mViewToolStripMenuItem
			// 
			_viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						_teletypeToolStripMenuItem,
						_deviceEditorToolStripMenuItem,
						_toolStripSeparator8,
						_findMenuItem,
						_findNextMenuItem});
			_viewToolStripMenuItem.Name = "mViewToolStripMenuItem";
			_viewToolStripMenuItem.Size = new Size(44, 20);
			_viewToolStripMenuItem.Text = "&View";
			// 
			// mTeletypeToolStripMenuItem
			// 
			_teletypeToolStripMenuItem.Image = ((Image)(resources.GetObject("mTeletypeToolStripMenuItem.Image")));
			_teletypeToolStripMenuItem.Name = "mTeletypeToolStripMenuItem";
			_teletypeToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.T);
			_teletypeToolStripMenuItem.Size = new Size(215, 22);
			_teletypeToolStripMenuItem.Text = "&Show Teletype";
			_teletypeToolStripMenuItem.Click += TeletypeItem_Click;
			// 
			// mDeviceEditorToolStripMenuItem
			// 
			_deviceEditorToolStripMenuItem.Image = ((Image)(resources.GetObject("mDeviceEditorToolStripMenuItem.Image")));
			_deviceEditorToolStripMenuItem.Name = "mDeviceEditorToolStripMenuItem";
			_deviceEditorToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.E);
			_deviceEditorToolStripMenuItem.Size = new Size(215, 22);
			_deviceEditorToolStripMenuItem.Text = "Show &Device Editor";
			_deviceEditorToolStripMenuItem.Click += DeviceEditorItem_Click;
			// 
			// toolStripSeparator8
			// 
			_toolStripSeparator8.Name = "toolStripSeparator8";
			_toolStripSeparator8.Size = new Size(212, 6);
			// 
			// mFindMenuItem
			// 
			_findMenuItem.Image = Properties.Resources.Find_5650;
			_findMenuItem.Name = "mFindMenuItem";
			_findMenuItem.ShortcutKeys = (Keys.Control | Keys.F);
			_findMenuItem.Size = new Size(215, 22);
			_findMenuItem.Text = "Find in memory...";
			_findMenuItem.Click += FindMenuItem_Click;
			// 
			// mFindNextMenuItem
			// 
			_findNextMenuItem.Name = "mFindNextMenuItem";
			_findNextMenuItem.ShortcutKeys = Keys.F3;
			_findNextMenuItem.Size = new Size(215, 22);
			_findNextMenuItem.Text = "Find next";
			_findNextMenuItem.Click += FindNextMenuItem_Click;
			// 
			// mActionsToolStripMenuItem
			// 
			_actionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						_tickToolStripMenuItem,
						_stepToolStripMenuItem,
						_runToolStripMenuItem,
						_goToolStripMenuItem,
						toolStripSeparator1,
						_detachToolStripMenuItem,
						toolStripSeparator2,
						_clearBreakpointsToolStripMenuItem,
						_resetToolStripMenuItem});
			_actionsToolStripMenuItem.Name = "mActionsToolStripMenuItem";
			_actionsToolStripMenuItem.Size = new Size(59, 20);
			_actionsToolStripMenuItem.Text = "&Actions";
			// 
			// mTickToolStripMenuItem
			// 
			_tickToolStripMenuItem.Image = ((Image)(resources.GetObject("mTickToolStripMenuItem.Image")));
			_tickToolStripMenuItem.Name = "mTickToolStripMenuItem";
			_tickToolStripMenuItem.ShortcutKeys = Keys.F11;
			_tickToolStripMenuItem.Size = new Size(207, 22);
			_tickToolStripMenuItem.Text = "T&ick";
			_tickToolStripMenuItem.Click += TickItem_Click;
			// 
			// mStepToolStripMenuItem
			// 
			_stepToolStripMenuItem.Image = ((Image)(resources.GetObject("mStepToolStripMenuItem.Image")));
			_stepToolStripMenuItem.Name = "mStepToolStripMenuItem";
			_stepToolStripMenuItem.ShortcutKeys = Keys.F10;
			_stepToolStripMenuItem.Size = new Size(207, 22);
			_stepToolStripMenuItem.Text = "&Step";
			_stepToolStripMenuItem.Click += StepItem_Click;
			// 
			// mRunToolStripMenuItem
			// 
			_runToolStripMenuItem.Image = ((Image)(resources.GetObject("mRunToolStripMenuItem.Image")));
			_runToolStripMenuItem.Name = "mRunToolStripMenuItem";
			_runToolStripMenuItem.ShortcutKeys = Keys.F5;
			_runToolStripMenuItem.Size = new Size(207, 22);
			_runToolStripMenuItem.Text = "R&un";
			_runToolStripMenuItem.Click += RunItem_Click;
			// 
			// mGoToolStripMenuItem
			// 
			_goToolStripMenuItem.Image = ((Image)(resources.GetObject("mGoToolStripMenuItem.Image")));
			_goToolStripMenuItem.Name = "mGoToolStripMenuItem";
			_goToolStripMenuItem.ShortcutKeys = (Keys.Shift | Keys.F5);
			_goToolStripMenuItem.Size = new Size(207, 22);
			_goToolStripMenuItem.Text = "&Go";
			_goToolStripMenuItem.Click += GoItem_Click;
			// 
			// mDetachToolStripMenuItem
			// 
			_detachToolStripMenuItem.Name = "mDetachToolStripMenuItem";
			_detachToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.D);
			_detachToolStripMenuItem.Size = new Size(207, 22);
			_detachToolStripMenuItem.Text = "&Detach";
			_detachToolStripMenuItem.Click += DetachItem_Click;
			// 
			// mClearBreakpointsToolStripMenuItem
			// 
			_clearBreakpointsToolStripMenuItem.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripMenuItem.Image")));
			_clearBreakpointsToolStripMenuItem.Name = "mClearBreakpointsToolStripMenuItem";
			_clearBreakpointsToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.B);
			_clearBreakpointsToolStripMenuItem.Size = new Size(207, 22);
			_clearBreakpointsToolStripMenuItem.Text = "Clear &Breakpoints";
			_clearBreakpointsToolStripMenuItem.Click += ClearBreakpointsItem_Click;
			// 
			// mResetToolStripMenuItem
			// 
			_resetToolStripMenuItem.Image = ((Image)(resources.GetObject("mResetToolStripMenuItem.Image")));
			_resetToolStripMenuItem.Name = "mResetToolStripMenuItem";
			_resetToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.R);
			_resetToolStripMenuItem.Size = new Size(207, 22);
			_resetToolStripMenuItem.Text = "&Reset";
			_resetToolStripMenuItem.Click += ResetItem_Click;
			// 
			// mToolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						_preferencesToolStripMenuItem,
						_toolStripSeparator9,
						_profilingEnabledMenuItem,
						_profilingShowTickCountsMenuItem,
						_profilingResetCountsMenuItem});
			toolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(48, 20);
			toolsToolStripMenuItem.Text = "&Tools";
			// 
			// mPreferencesToolStripMenuItem
			// 
			_preferencesToolStripMenuItem.Image = ((Image)(resources.GetObject("mPreferencesToolStripMenuItem.Image")));
			_preferencesToolStripMenuItem.Name = "mPreferencesToolStripMenuItem";
			_preferencesToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.P);
			_preferencesToolStripMenuItem.Size = new Size(185, 22);
			_preferencesToolStripMenuItem.Text = "&Preferences...";
			_preferencesToolStripMenuItem.Click += PreferencesMenuItem_Click;
			// 
			// toolStripSeparator9
			// 
			_toolStripSeparator9.Name = "toolStripSeparator9";
			_toolStripSeparator9.Size = new Size(182, 6);
			// 
			// mProfilingEnabledMenuItem
			// 
			_profilingEnabledMenuItem.CheckOnClick = true;
			_profilingEnabledMenuItem.Name = "mProfilingEnabledMenuItem";
			_profilingEnabledMenuItem.Size = new Size(185, 22);
			_profilingEnabledMenuItem.Text = "&Enable profiling";
			_profilingEnabledMenuItem.CheckedChanged += ProfilingEnabledMenuItem_CheckedChanged;
			// 
			// mProfilingShowTickCountsMenuItem
			// 
			_profilingShowTickCountsMenuItem.CheckOnClick = true;
			_profilingShowTickCountsMenuItem.Name = "mProfilingShowTickCountsMenuItem";
			_profilingShowTickCountsMenuItem.Size = new Size(185, 22);
			_profilingShowTickCountsMenuItem.Text = "&Show tick counts";
			_profilingShowTickCountsMenuItem.CheckedChanged += ProfilingShowTickCountsMenuItem_CheckedChanged;
			// 
			// mHelpToolStripMenuItem
			// 
			_helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
						_aboutToolStripMenuItem});
			_helpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
			_helpToolStripMenuItem.Size = new Size(44, 20);
			_helpToolStripMenuItem.Text = "&Help";
			// 
			// mAboutToolStripMenuItem
			// 
			_aboutToolStripMenuItem.Image = ((Image)(resources.GetObject("mAboutToolStripMenuItem.Image")));
			_aboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
			_aboutToolStripMenuItem.Size = new Size(116, 22);
			_aboutToolStripMenuItem.Text = "&About...";
			_aboutToolStripMenuItem.Click += AboutMenuItem_Click;
			// 
			// mToolStripContainer
			// 
			// 
			// mToolStripContainer.BottomToolStripPanel
			// 
			_toolStripContainer.BottomToolStripPanel.Controls.Add(_statusStrip);
			// 
			// mToolStripContainer.ContentPanel
			// 
			_toolStripContainer.ContentPanel.Controls.Add(_splitContainer);
			_toolStripContainer.ContentPanel.Size = new Size(804, 491);
			_toolStripContainer.Dock = DockStyle.Fill;
			_toolStripContainer.LeftToolStripPanelVisible = false;
			_toolStripContainer.Location = new Point(0, 0);
			_toolStripContainer.Name = "mToolStripContainer";
			_toolStripContainer.RightToolStripPanelVisible = false;
			_toolStripContainer.Size = new Size(804, 562);
			_toolStripContainer.TabIndex = 0;
			_toolStripContainer.Text = "ToolStripContainer";
			// 
			// mToolStripContainer.TopToolStripPanel
			// 
			_toolStripContainer.TopToolStripPanel.Controls.Add(_mainMenuStrip);
			_toolStripContainer.TopToolStripPanel.Controls.Add(_controlToolStrip);
			// 
			// mStatusStrip
			// 
			_statusStrip.Dock = DockStyle.None;
			_statusStrip.Items.AddRange(new ToolStripItem[] {
						_modeToolStripStatusLabel,
						_modeCycleButton,
						_statusToolStripStatusLabel,
						_toolStripStatusLabel});
			_statusStrip.Location = new Point(0, 0);
			_statusStrip.Name = "mStatusStrip";
			_statusStrip.Size = new Size(804, 22);
			_statusStrip.TabIndex = 8;
			// 
			// mModeToolStripStatusLabel
			// 
			_modeToolStripStatusLabel.Name = "mModeToolStripStatusLabel";
			_modeToolStripStatusLabel.Size = new Size(41, 17);
			_modeToolStripStatusLabel.Text = "Mode:";
			// 
			// mModeCycleButton
			// 
			_modeCycleButton.AutoSize = false;
			_modeCycleButton.Name = "mModeCycleButton";
			_modeCycleButton.Size = new Size(60, 20);
			_modeCycleButton.Value = null;
			// 
			// mStatusToolStripStatusLabel
			// 
			_statusToolStripStatusLabel.Name = "mStatusToolStripStatusLabel";
			_statusToolStripStatusLabel.Size = new Size(48, 17);
			_statusToolStripStatusLabel.Text = "| Status:";
			// 
			// mToolStripStatusLabel
			// 
			_toolStripStatusLabel.Name = "mToolStripStatusLabel";
			_toolStripStatusLabel.Size = new Size(10, 17);
			_toolStripStatusLabel.Text = " ";
			// 
			// mSplitContainer
			// 
			_splitContainer.BackColor = SystemColors.Control;
			_splitContainer.Dock = DockStyle.Fill;
			_splitContainer.FixedPanel = FixedPanel.Panel2;
			_splitContainer.Location = new Point(0, 0);
			_splitContainer.Name = "mSplitContainer";
			_splitContainer.Orientation = Orientation.Horizontal;
			// 
			// mSplitContainer.Panel1
			// 
			_splitContainer.Panel1.Controls.Add(_memoryGroup);
			_splitContainer.Panel1.Controls.Add(_registersGroup);
			_splitContainer.Panel1.Controls.Add(_devicesGroup);
			_splitContainer.Panel1.Controls.Add(_symbolGroup);
			_splitContainer.Panel1MinSize = 380;
			// 
			// mSplitContainer.Panel2
			// 
			_splitContainer.Panel2.Controls.Add(_logListGroup);
			_splitContainer.Panel2MinSize = 96;
			_splitContainer.Size = new Size(804, 491);
			_splitContainer.SplitterDistance = 385;
			_splitContainer.TabIndex = 6;
			// 
			// mMemoryGroup
			// 
			_memoryGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_memoryGroup.Controls.Add(_memoryTabControl);
			_memoryGroup.Location = new Point(236, -2);
			_memoryGroup.Name = "mMemoryGroup";
			_memoryGroup.Size = new Size(568, 328);
			_memoryGroup.TabIndex = 4;
			_memoryGroup.TabStop = false;
			_memoryGroup.Text = "Memory";
			// 
			// mMemoryTabControl
			// 
			_memoryTabControl.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_memoryTabControl.Controls.Add(_mainMemoryTab);
			_memoryTabControl.Controls.Add(_floatingPointMemoryTab);
			_memoryTabControl.Location = new Point(4, 16);
			_memoryTabControl.Name = "mMemoryTabControl";
			_memoryTabControl.SelectedIndex = 0;
			_memoryTabControl.Size = new Size(560, 309);
			_memoryTabControl.TabIndex = 0;
			_memoryTabControl.SelectedIndexChanged += MemoryTabControl_SelectedIndexChanged;
			_memoryTabControl.Selecting += MemoryTabControl_Selecting;
			// 
			// mMainMemoryTab
			// 
			_mainMemoryTab.Controls.Add(_mainMemoryEditor);
			_mainMemoryTab.Location = new Point(4, 22);
			_mainMemoryTab.Name = "mMainMemoryTab";
			_mainMemoryTab.Padding = new Padding(3);
			_mainMemoryTab.Size = new Size(552, 283);
			_mainMemoryTab.TabIndex = 0;
			_mainMemoryTab.Text = "Main";
			_mainMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mMainMemoryEditor
			// 
			_mainMemoryEditor.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_mainMemoryEditor.FirstVisibleAddress = 0;
			_mainMemoryEditor.IndexedAddressCalculatorCallback = null;
			_mainMemoryEditor.Location = new Point(0, 0);
			_mainMemoryEditor.MarkedAddress = -1;
			_mainMemoryEditor.Memory = null;
			_mainMemoryEditor.Name = "mMainMemoryEditor";
			_mainMemoryEditor.ReadOnly = false;
			_mainMemoryEditor.ResizeInProgress = false;
			_mainMemoryEditor.Size = new Size(552, 283);
			_mainMemoryEditor.Symbols = null;
			_mainMemoryEditor.TabIndex = 0;
			_mainMemoryEditor.ToolTip = null;
			_mainMemoryEditor.AddressSelected += MemoryEditor_addressSelected;
			// 
			// mFloatingPointMemoryTab
			// 
			_floatingPointMemoryTab.Location = new Point(4, 22);
			_floatingPointMemoryTab.Name = "mFloatingPointMemoryTab";
			_floatingPointMemoryTab.Padding = new Padding(3);
			_floatingPointMemoryTab.Size = new Size(552, 283);
			_floatingPointMemoryTab.TabIndex = 1;
			_floatingPointMemoryTab.Text = "Floating Point";
			_floatingPointMemoryTab.UseVisualStyleBackColor = true;
			// 
			// mRegistersGroup
			// 
			_registersGroup.Controls.Add(_registersEditor);
			_registersGroup.Location = new Point(0, -2);
			_registersGroup.Name = "mRegistersGroup";
			_registersGroup.Size = new Size(232, 236);
			_registersGroup.TabIndex = 2;
			_registersGroup.TabStop = false;
			_registersGroup.Text = "Registers";
			// 
			// mRegistersEditor
			// 
			_registersEditor.Location = new Point(8, 16);
			_registersEditor.Name = "mRegistersEditor";
			_registersEditor.ReadOnly = false;
			registers1.CompareIndicator = MixLib.Registers.CompValues.Equal;
			registers1.OverflowIndicator = false;
			_registersEditor.Registers = registers1;
			_registersEditor.Size = new Size(218, 214);
			_registersEditor.TabIndex = 0;
			// 
			// mDevicesGroup
			// 
			_devicesGroup.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
									| AnchorStyles.Right);
			_devicesGroup.Controls.Add(_deviceEditorButton);
			_devicesGroup.Controls.Add(_devicesControl);
			_devicesGroup.Location = new Point(0, 326);
			_devicesGroup.Name = "mDevicesGroup";
			_devicesGroup.Size = new Size(804, 56);
			_devicesGroup.TabIndex = 5;
			_devicesGroup.TabStop = false;
			_devicesGroup.Text = "Devices";
			// 
			// mDeviceEditorButton
			// 
			_deviceEditorButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_deviceEditorButton.Location = new Point(716, 16);
			_deviceEditorButton.Margin = new Padding(0);
			_deviceEditorButton.Name = "mDeviceEditorButton";
			_deviceEditorButton.Size = new Size(80, 23);
			_deviceEditorButton.TabIndex = 1;
			_deviceEditorButton.Text = "Show E&ditor";
			_deviceEditorButton.UseVisualStyleBackColor = true;
			_deviceEditorButton.FlatStyle = FlatStyle.Flat;
			_deviceEditorButton.Click += DeviceEditorItem_Click;
			// 
			// mDevicesControl
			// 
			_devicesControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
									| AnchorStyles.Right);
			_devicesControl.Devices = null;
			_devicesControl.Location = new Point(4, 16);
			_devicesControl.Name = "mDevicesControl";
			_devicesControl.Size = new Size(706, 36);
			_devicesControl.Structure = null;
			_devicesControl.TabIndex = 0;
			_devicesControl.ToolTip = null;
			// 
			// mSymbolGroup
			// 
			_symbolGroup.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left);
			_symbolGroup.Controls.Add(_symbolListView);
			_symbolGroup.Location = new Point(0, 234);
			_symbolGroup.Name = "mSymbolGroup";
			_symbolGroup.Size = new Size(232, 92);
			_symbolGroup.TabIndex = 3;
			_symbolGroup.TabStop = false;
			_symbolGroup.Text = "Symbols";
			// 
			// mSymbolListView
			// 
			_symbolListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_symbolListView.Location = new Point(8, 16);
			_symbolListView.MemoryMaxIndex = 0;
			_symbolListView.MemoryMinIndex = 0;
			_symbolListView.Name = "mSymbolListView";
			_symbolListView.Size = new Size(218, 70);
			_symbolListView.Symbols = null;
			_symbolListView.TabIndex = 0;
			_symbolListView.AddressSelected += ListViews_addressSelected;
			// 
			// mLogListGroup
			// 
			_logListGroup.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_logListGroup.Controls.Add(_logListView);
			_logListGroup.Location = new Point(0, 3);
			_logListGroup.Name = "mLogListGroup";
			_logListGroup.Size = new Size(804, 95);
			_logListGroup.TabIndex = 7;
			_logListGroup.TabStop = false;
			_logListGroup.Text = "Messages";
			// 
			// mLogListView
			// 
			_logListView.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
									| AnchorStyles.Left)
									| AnchorStyles.Right);
			_logListView.Location = new Point(8, 16);
			_logListView.Name = "mLogListView";
			_logListView.SeverityImageList = _severityImageList;
			_logListView.Size = new Size(788, 73);
			_logListView.TabIndex = 0;
			_logListView.AddressSelected += ListViews_addressSelected;
			// 
			// mControlToolStrip
			// 
			_controlToolStrip.Dock = DockStyle.None;
			_controlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_controlToolStrip.Items.AddRange(new ToolStripItem[] 
			{
				_pcToolStripLabel,
				_showPCToolStripButton,
				toolStripSeparator3,
				_ticksToolStripLabel,
				_ticksToolStripTextBox,
				_resetTicksToolStripButton,
				toolStripSeparator4,
				_tickToolStripButton,
				_stepToolStripButton,
				_runToolStripButton,
				_goToolStripButton,
				toolStripSeparator5,
				_detachToolStripButton,
				toolStripSeparator6,
				_clearBreakpointsToolStripButton,
				_resetToolStripButton,
				toolStripSeparator7,
				_teletypeToolStripButton
			});

			_controlToolStrip.Location = new Point(3, 24);
			_controlToolStrip.Name = "mControlToolStrip";
			_controlToolStrip.Size = new Size(761, 25);
			_controlToolStrip.TabIndex = 1;
			// 
			// mPCToolStripLabel
			// 
			_pcToolStripLabel.Name = "mPCToolStripLabel";
			_pcToolStripLabel.Size = new Size(28, 22);
			_pcToolStripLabel.Text = "PC: ";
			// 
			// mShowPCToolStripButton
			// 
			_showPCToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			_showPCToolStripButton.Image = ((Image)(resources.GetObject("mShowPCToolStripButton.Image")));
			_showPCToolStripButton.ImageTransparentColor = Color.Magenta;
			_showPCToolStripButton.Name = "mShowPCToolStripButton";
			_showPCToolStripButton.Size = new Size(40, 22);
			_showPCToolStripButton.Text = "Sh&ow";
			_showPCToolStripButton.ToolTipText = "Show the program counter";
			_showPCToolStripButton.Click += ShowPCButton_Click;
			// 
			// mTicksToolStripLabel
			// 
			_ticksToolStripLabel.Name = "mTicksToolStripLabel";
			_ticksToolStripLabel.Size = new Size(40, 22);
			_ticksToolStripLabel.Text = "Ticks: ";
			// 
			// mTicksToolStripTextBox
			// 
			_ticksToolStripTextBox.BorderStyle = BorderStyle.FixedSingle;
			_ticksToolStripTextBox.Name = "mTicksToolStripTextBox";
			_ticksToolStripTextBox.ReadOnly = true;
			_ticksToolStripTextBox.Size = new Size(64, 25);
			_ticksToolStripTextBox.Text = "0";
			// 
			// mResetTicksToolStripButton
			// 
			_resetTicksToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			_resetTicksToolStripButton.Image = ((Image)(resources.GetObject("mResetTicksToolStripButton.Image")));
			_resetTicksToolStripButton.ImageTransparentColor = Color.Magenta;
			_resetTicksToolStripButton.Name = "mResetTicksToolStripButton";
			_resetTicksToolStripButton.Size = new Size(39, 22);
			_resetTicksToolStripButton.Text = "&Reset";
			_resetTicksToolStripButton.ToolTipText = "Reset the tick counter";
			_resetTicksToolStripButton.Click += ResetTicksButton_Click;
			// 
			// mTickToolStripButton
			// 
			_tickToolStripButton.Image = ((Image)(resources.GetObject("mTickToolStripButton.Image")));
			_tickToolStripButton.ImageTransparentColor = Color.Magenta;
			_tickToolStripButton.Name = "mTickToolStripButton";
			_tickToolStripButton.Size = new Size(49, 22);
			_tickToolStripButton.Text = "T&ick";
			_tickToolStripButton.ToolTipText = "Execute one tick";
			_tickToolStripButton.Click += TickItem_Click;
			// 
			// mStepToolStripButton
			// 
			_stepToolStripButton.Image = ((Image)(resources.GetObject("mStepToolStripButton.Image")));
			_stepToolStripButton.ImageTransparentColor = Color.Magenta;
			_stepToolStripButton.Name = "mStepToolStripButton";
			_stepToolStripButton.Size = new Size(50, 22);
			_stepToolStripButton.Text = "Ste&p";
			_stepToolStripButton.ToolTipText = "Execute one step";
			_stepToolStripButton.Click += StepItem_Click;
			// 
			// mRunToolStripButton
			// 
			_runToolStripButton.Image = ((Image)(resources.GetObject("mRunToolStripButton.Image")));
			_runToolStripButton.ImageTransparentColor = Color.Magenta;
			_runToolStripButton.Name = "mRunToolStripButton";
			_runToolStripButton.Size = new Size(48, 22);
			_runToolStripButton.Text = "R&un";
			_runToolStripButton.ToolTipText = "Start or stop a full run";
			_runToolStripButton.Click += RunItem_Click;
			// 
			// mGoToolStripButton
			// 
			_goToolStripButton.Image = ((Image)(resources.GetObject("mGoToolStripButton.Image")));
			_goToolStripButton.ImageTransparentColor = Color.Magenta;
			_goToolStripButton.Name = "mGoToolStripButton";
			_goToolStripButton.Size = new Size(42, 22);
			_goToolStripButton.Text = "&Go";
			_goToolStripButton.ToolTipText = "Start the MIX loader";
			_goToolStripButton.Click += GoItem_Click;
			// 
			// mDetachToolStripButton
			// 
			_detachToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			_detachToolStripButton.Image = ((Image)(resources.GetObject("mDetachToolStripButton.Image")));
			_detachToolStripButton.ImageTransparentColor = Color.Magenta;
			_detachToolStripButton.Name = "mDetachToolStripButton";
			_detachToolStripButton.Size = new Size(48, 22);
			_detachToolStripButton.Text = "Detac&h";
			_detachToolStripButton.ToolTipText = "Switch between running MIX in the foreground or background";
			_detachToolStripButton.Click += DetachItem_Click;
			// 
			// mClearBreakpointsToolStripButton
			// 
			_clearBreakpointsToolStripButton.Image = ((Image)(resources.GetObject("mClearBreakpointsToolStripButton.Image")));
			_clearBreakpointsToolStripButton.ImageTransparentColor = Color.Magenta;
			_clearBreakpointsToolStripButton.Name = "mClearBreakpointsToolStripButton";
			_clearBreakpointsToolStripButton.Size = new Size(119, 22);
			_clearBreakpointsToolStripButton.Text = "Clear &Breakpoints";
			_clearBreakpointsToolStripButton.ToolTipText = "Remove all set breakpoints";
			_clearBreakpointsToolStripButton.Click += ClearBreakpointsItem_Click;
			// 
			// mResetToolStripButton
			// 
			_resetToolStripButton.Image = ((Image)(resources.GetObject("mResetToolStripButton.Image")));
			_resetToolStripButton.ImageTransparentColor = Color.Magenta;
			_resetToolStripButton.Name = "mResetToolStripButton";
			_resetToolStripButton.Size = new Size(55, 22);
			_resetToolStripButton.Text = "R&eset";
			_resetToolStripButton.ToolTipText = "Reset MIX to initial state";
			_resetToolStripButton.Click += ResetItem_Click;
			// 
			// mTeletypeToolStripButton
			// 
			_teletypeToolStripButton.Image = ((Image)(resources.GetObject("mTeletypeToolStripButton.Image")));
			_teletypeToolStripButton.ImageTransparentColor = Color.Magenta;
			_teletypeToolStripButton.Name = "mTeletypeToolStripButton";
			_teletypeToolStripButton.Size = new Size(104, 22);
			_teletypeToolStripButton.Text = "Show &Teletype";
			_teletypeToolStripButton.ToolTipText = "Show or hide the teletype";
			_teletypeToolStripButton.Click += TeletypeItem_Click;
			// 
			// mToolTip
			// 
			_toolTip.AutoPopDelay = 10000;
			_toolTip.InitialDelay = 100;
			_toolTip.ReshowDelay = 100;
			_toolTip.ShowAlways = true;
			// 
			// mProfilingResetCountsMenuItem
			// 
			_profilingResetCountsMenuItem.Name = "mProfilingResetCountsMenuItem";
			_profilingResetCountsMenuItem.Size = new Size(185, 22);
			_profilingResetCountsMenuItem.Text = "&Reset counts";
			_profilingResetCountsMenuItem.Click += ProfilingResetCountsMenuItem_Click;
			// 
			// MixForm
			// 
			ClientSize = new Size(804, 562);
			Controls.Add(_toolStripContainer);
			Icon = ((Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			MainMenuStrip = _mainMenuStrip;
			MinimumSize = new Size(820, 600);
			Name = "MixForm";
			StartPosition = FormStartPosition.Manual;
			Text = "MixEmul";
			FormClosing += This_FormClosing;
			ResizeBegin += This_ResizeBegin;
			ResizeEnd += This_ResizeEnd;
			_mainMenuStrip.ResumeLayout(false);
			_mainMenuStrip.PerformLayout();
			_toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			_toolStripContainer.BottomToolStripPanel.PerformLayout();
			_toolStripContainer.ContentPanel.ResumeLayout(false);
			_toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			_toolStripContainer.TopToolStripPanel.PerformLayout();
			_toolStripContainer.ResumeLayout(false);
			_toolStripContainer.PerformLayout();
			_statusStrip.ResumeLayout(false);
			_statusStrip.PerformLayout();
			_splitContainer.Panel1.ResumeLayout(false);
			_splitContainer.Panel2.ResumeLayout(false);
			_splitContainer.ResumeLayout(false);
			_memoryGroup.ResumeLayout(false);
			_memoryTabControl.ResumeLayout(false);
			_mainMemoryTab.ResumeLayout(false);
			_registersGroup.ResumeLayout(false);
			_devicesGroup.ResumeLayout(false);
			_symbolGroup.ResumeLayout(false);
			_logListGroup.ResumeLayout(false);
			_controlToolStrip.ResumeLayout(false);
			_controlToolStrip.PerformLayout();
			ResumeLayout(false);

		}

		private void LoadProgram(string filePath)
		{
			if (filePath == null)
				return;

			var instances = Assembler.Assemble(File.ReadAllLines(filePath), out PreInstruction[] instructions, out SymbolCollection symbols, out AssemblyFindingCollection findings);
			if (instructions != null && findings != null)
			{
				if (_sourceAndFindingsForm == null)
				{
					_sourceAndFindingsForm = new SourceAndFindingsForm
					{
						SeverityImageList = _severityImageList
					};
				}

				_sourceAndFindingsForm.SetInstructionsAndFindings(instructions, instances, findings);

				if (_sourceAndFindingsForm.ShowDialog(this) != DialogResult.OK)
					return;
			}

			if (instances != null)
			{
				_symbolListView.Symbols = symbols;
				_mainMemoryEditor.Symbols = symbols;
				_mix.LoadInstructionInstances(instances, symbols);
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
			if (_steppingState == SteppingState.Idle)
			{
				SetSteppingState(SteppingState.Stepping);
				_running = true;
				_mix.StartRun();
			}
			else
			{
				_running = false;
				_mix.RequestStop();
			}
		}

		private void PcChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			if (!_updating)
				ImplementPCChange((int)args.NewValue);
		}

		private void SetSteppingState(SteppingState state)
		{
			if (_steppingState != state)
			{
				_steppingState = state;

				if (_steppingState == SteppingState.Idle)
				{
					ReadOnly = false;
					_runToolStripButton.Text = "R&un";
					_runToolStripMenuItem.Text = "R&un";
				}
				else
				{
					ReadOnly = true;
					_runToolStripButton.Text = "St&op";
					_runToolStripMenuItem.Text = "St&op";
				}
			}
		}

		private void SwitchTeletypeVisibility()
		{
			if (_teletype.Visible)
				_teletype.Hide();

			else
				_teletype.Show();
		}

		public new void Update()
		{
			_updating = true;
			var pcVisible = _mainMemoryEditor.IsAddressVisible((int)_pcBox.LongValue);
			SetPCBoxBounds();
			_pcBox.LongValue = _mix.ProgramCounter;
			_mainMemoryEditor.MarkedAddress = _mix.ProgramCounter;

			if (_floatingPointMemoryEditor != null)
			{
				var floatPcVisible = _floatingPointMemoryEditor.IsAddressVisible(_floatingPointMemoryEditor.MarkedAddress);
				_floatingPointMemoryEditor.MarkedAddress = _mix.FloatingPointModule.ProgramCounter;

				if (floatPcVisible)
					_floatingPointMemoryEditor.MakeAddressVisible(_floatingPointMemoryEditor.MarkedAddress);
			}

			_ticksToolStripTextBox.Text = _mix.TickCounter.ToString();

			switch (_mix.Status)
			{
				case ModuleBase.RunStatus.InvalidInstruction:
					_toolStripStatusLabel.Text = "Encountered invalid instruction";
					pcVisible = true;
					break;

				case ModuleBase.RunStatus.RuntimeError:
					_toolStripStatusLabel.Text = "Runtime error occured";
					pcVisible = true;
					break;

				case ModuleBase.RunStatus.BreakpointReached:
					_toolStripStatusLabel.Text = "Breakpoint reached";
					break;

				default:
					_toolStripStatusLabel.Text = _mix.Status.ToString();
					break;
			}

			if (pcVisible)
				_mainMemoryEditor.MakeAddressVisible(_mix.ProgramCounter, _mix.Status == ModuleBase.RunStatus.Idle);

			_mix.NeutralizeStatus();

			_modeCycleButton.Value = _mix.Mode;

			_registersEditor.Update();
			_mainMemoryEditor.Update();
			_floatingPointMemoryEditor?.Update();
			_devicesControl.Update();
			base.Update();
			_updating = false;
		}

		private void UpdateDeviceConfig()
		{
			DeviceSettings.DeviceFilesDirectory = _configuration.DeviceFilesDirectory;
			DeviceSettings.TickCounts = _configuration.TickCounts;
			DeviceSettings.DeviceReloadInterval = _configuration.DeviceReloadInterval;
			_mix.Devices.UpdateSettings();

			foreach (var device in _mix.Devices.OfType<FileBasedDevice>())
			{
				device.FilePath = _configuration.DeviceFilePaths.ContainsKey(device.Id)
					? _configuration.DeviceFilePaths[device.Id]
					: null;
			}
		}

		public void UpdateLayout()
		{
			GuiSettings.Colors = _configuration.Colors;
			_pcBox.UpdateLayout();
			_teletype.UpdateLayout();
			_registersEditor.UpdateLayout();

			foreach (MemoryEditor editor in _memoryEditors)
				editor?.UpdateLayout();

			_devicesControl.UpdateLayout();
			_sourceAndFindingsForm?.UpdateLayout();
			_preferencesForm?.UpdateLayout();
			_deviceEditor.UpdateLayout();
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly == value)
					return;

				_readOnly = value;
				_pcBox.ReadOnly = _readOnly;
				_resetTicksToolStripButton.Enabled = !_readOnly;
				_stepToolStripButton.Enabled = !_readOnly;
				_stepToolStripMenuItem.Enabled = !ReadOnly;
				_tickToolStripButton.Enabled = !_readOnly;
				_tickToolStripMenuItem.Enabled = !_readOnly;
				_goToolStripButton.Enabled = !_readOnly;
				_goToolStripMenuItem.Enabled = !_readOnly;
				_resetToolStripButton.Enabled = !_readOnly;
				_resetToolStripMenuItem.Enabled = !_readOnly;
				_registersEditor.ReadOnly = _readOnly;

				foreach (MemoryEditor editor in _memoryEditors.Where(editor => editor != null))
					editor.ReadOnly = _readOnly;

				_mainMemoryEditor.ReadOnly = _readOnly;
				_openProgramToolStripMenuItem.Enabled = !_readOnly;
				_preferencesToolStripMenuItem.Enabled = !_readOnly;
			}
		}

		private enum SteppingState
		{
			Idle,
			Stepping
		}

		private void ImplementFloatingPointPCChange(int address)
		{
			if (_floatingPointMemoryEditor != null && _floatingPointMemoryEditor.IsAddressVisible(_mix.FloatingPointModule.ProgramCounter))
				_floatingPointMemoryEditor.MakeAddressVisible(address, _mix.Status == ModuleBase.RunStatus.Idle);

			_mix.FloatingPointModule.ProgramCounter = address;

			if (_floatingPointMemoryEditor != null)
				_floatingPointMemoryEditor.MarkedAddress = _mix.FloatingPointModule.ProgramCounter;
		}

	}
}
