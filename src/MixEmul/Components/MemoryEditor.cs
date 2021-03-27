using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Properties;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Misc;
using MixLib.Type;

namespace MixGui.Components
{
	public class MemoryEditor : UserControl, IBreakpointManager
	{
		private readonly SortedList _breakpoints;
		private Label _firstAddressLabel;
		private Panel _firstAddressPanel;
		private LongValueTextBox _firstAddressTextBox;
		private int _markedAddress;
		private IMemory _memory;
		private readonly Label _noMemoryLabel;
		private bool _readOnly;
		private Label _setClipboardLabel;
		private ToolTip _toolTip;
		private WordEditorList _wordEditorList;
		private IndexedAddressCalculatorCallback _indexedAddressCalculatorCallback;
		private SymbolCollection _symbols;
		private MixCharClipboardButtonControl _mixCharButtons;
		private Button _exportButton;
		private Button _upButton;
		private Button _downButton;
		private LinkedItemsSelectorControl<EditorListViewInfo> _addressHistorySelector;
		private SaveFileDialog _saveExportFileDialog;
		private int? _prevDataIndex;
		private int? _nextDataIndex;
		private readonly long[] _profilingMaxCounts;

		public event AddressSelectedHandler AddressSelected;

		public MemoryEditor(IMemory memory = null)
		{
			_readOnly = false;
			_markedAddress = -1;

			_profilingMaxCounts = new long[Enum.GetValues(typeof(GuiSettings.ProfilingInfoType)).Length];

			_breakpoints = SortedList.Synchronized(new SortedList());

			_noMemoryLabel = new Label
			{
				Location = new Point(0, 0),
				Name = "mNoMemoryLabel",
				Size = new Size(120, 16),
				TabIndex = 0,
				Text = "No memory connected",
				TextAlign = ContentAlignment.MiddleCenter
			};

			Memory = memory;
		}

		public ICollection Breakpoints
			=> _breakpoints.Values;

		private long GetMaxProfilingCount(GuiSettings.ProfilingInfoType infoType)
			=> _profilingMaxCounts[(int)infoType];

		public bool IsBreakpointSet(int address)
			=> _breakpoints.Contains(address);

		public void MakeAddressVisible(int address)
			=> MakeAddressVisible(address, true);

		public bool IsAddressVisible(int address)
			=> _wordEditorList.IsIndexVisible(address);

		public void ClearHistory()
			=> _addressHistorySelector.Clear();

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args)
			=> AddressSelected?.Invoke(this, args);

		private void This_AddressSelected(object sender, AddressSelectedEventArgs args)
			=> MakeAddressVisible(args.SelectedAddress);

		private void AddressDoubleClick(object sender, EventArgs e)
			=> OnAddressSelected(new AddressSelectedEventArgs(((MemoryWordEditor)sender).MemoryWord.Index));

		private void MemoryEditor_SizeChanged(object sender, EventArgs e)
			=> SetUpDownButtonStates(NavigationDirection.None);

		public ToolTip ToolTip
		{
			get => _toolTip;
			set
			{
				_toolTip = value;

				if (_wordEditorList != null)
				{
					foreach (MemoryWordEditor editor in _wordEditorList)
						editor.ToolTip = _toolTip;
				}
			}
		}

		private IWordEditor CreateWordEditor(int address)
		{
			var editor = new MemoryWordEditor(address >= _memory.MinWordIndex ? _memory[address] : new MemoryFullWord(address))
			{
				GetMaxProfilingCount = GetMaxProfilingCount,
				MemoryMinIndex = _memory.MinWordIndex,
				MemoryMaxIndex = _memory.MaxWordIndex,
				IndexedAddressCalculatorCallback = _indexedAddressCalculatorCallback,
				BreakPointChecked = _breakpoints.Contains(address),
				Marked = _markedAddress == address,
				ToolTip = _toolTip,
				Symbols = _symbols
			};

			editor.BreakpointCheckedChanged += BreakPointCheckedChanged;
			editor.AddressDoubleClick += AddressDoubleClick;
			editor.AddressSelected += This_AddressSelected;

			return editor;
		}

		private void LoadWordEditor(IWordEditor editor, int address)
		{
			var memoryEditor = (MemoryWordEditor)editor;

			if (address == memoryEditor.MemoryWord.Index && address >= _memory.MinWordIndex && memoryEditor.MemoryWord.Equals(_memory[address]))
				memoryEditor.Update();

			else
				memoryEditor.MemoryWord = address >= _memory.MinWordIndex ? _memory[address] : new MemoryFullWord(address);

			memoryEditor.BreakPointChecked = _breakpoints.Contains(address);
			memoryEditor.Marked = _markedAddress == address;
		}

		private void BreakPointCheckedChanged(object sender, EventArgs e)
		{
			if (_wordEditorList.IsReloading)
				return;

			var editor = (MemoryWordEditor)sender;
			int memoryAddress = editor.MemoryWord.Index;

			if (editor.BreakPointChecked)
				_breakpoints.Add(memoryAddress, memoryAddress);

			else
				_breakpoints.Remove(memoryAddress);
		}

		public void ClearBreakpoints()
		{
			_breakpoints.Clear();
			foreach (MemoryWordEditor editor in _wordEditorList)
				editor.BreakPointChecked = false;
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			Controls.Clear();

			if (_memory == null)
			{
				Controls.Add(_noMemoryLabel);
				_noMemoryLabel.Size = Size;

				ResumeLayout(false);
				return;
			}

			_firstAddressPanel = new Panel();
			_firstAddressLabel = new Label();
			_firstAddressTextBox = new LongValueTextBox(_memory.MinWordIndex, _memory.MaxWordIndex);
			_setClipboardLabel = new Label();
			_addressHistorySelector = new LinkedItemsSelectorControl<EditorListViewInfo>();
			_addressHistorySelector.SetCurrentItemCallback(GetCurrentListViewInfo);
			_mixCharButtons = new MixCharClipboardButtonControl();
			_exportButton = new Button();
			_upButton = new Button();
			_downButton = new Button();

			_firstAddressPanel.SuspendLayout();
			_firstAddressLabel.Location = new Point(0, 0);
			_firstAddressLabel.Name = "mFirstAddressLabel";
			_firstAddressLabel.Size = new Size(120, 20);
			_firstAddressLabel.TabIndex = 0;
			_firstAddressLabel.Text = "First visible address:";
			_firstAddressLabel.TextAlign = ContentAlignment.MiddleLeft;

			_firstAddressTextBox.Location = new Point(_firstAddressLabel.Width, 0);
			_firstAddressTextBox.Name = "mFirstAddressTextBox";
			_firstAddressTextBox.Size = new Size(35, 20);
			_firstAddressTextBox.TabIndex = 1;
			_firstAddressTextBox.ClearZero = false;
			_firstAddressTextBox.ValueChanged += FirstAddressTextBox_ValueChanged;

			_addressHistorySelector.Location = new Point(_firstAddressTextBox.Right + 4, 0);
			_addressHistorySelector.Name = "mAddressHistorySelector";
			_addressHistorySelector.TabIndex = 2;
			_addressHistorySelector.ItemSelected += MAddressHistorySelector_ItemSelected;

			_setClipboardLabel.Location = new Point(_addressHistorySelector.Right + 16, 0);
			_setClipboardLabel.Name = "mSetClipboardLabel";
			_setClipboardLabel.Size = new Size(95, _firstAddressLabel.Height);
			_setClipboardLabel.TabIndex = 3;
			_setClipboardLabel.Text = "Set clipboard to:";
			_setClipboardLabel.TextAlign = ContentAlignment.MiddleLeft;

			_mixCharButtons.Location = new Point(_setClipboardLabel.Right, 0);
			_mixCharButtons.Name = "mMixCharButtons";
			_mixCharButtons.TabIndex = 4;

			_exportButton.Location = new Point(_mixCharButtons.Right + 16, 0);
			_exportButton.Name = "mExportButton";
			_exportButton.Size = new Size(62, 21);
			_exportButton.TabIndex = 5;
			_exportButton.Text = "&Export...";
			_exportButton.FlatStyle = FlatStyle.Flat;
			_exportButton.Click += ExportButton_Click;

			_downButton.Visible = _memory is Memory;
			_downButton.Enabled = false;
			_downButton.FlatStyle = FlatStyle.Flat;
			_downButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_downButton.Image = Resources.Symbols_Down_16xLG;
			_downButton.Name = "mDownButton";
			_downButton.Size = new Size(21, 21);
			_downButton.Location = new Point(Width - _downButton.Width, 0);
			_downButton.TabIndex = 7;
			_downButton.Click += DownButton_Click;

			_upButton.Visible = _memory is Memory;
			_upButton.Enabled = false;
			_upButton.FlatStyle = FlatStyle.Flat;
			_upButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_upButton.Image = Resources.Symbols_Up_16xLG___kopie;
			_upButton.Name = "mUpButton";
			_upButton.Size = new Size(21, 21);
			_upButton.Location = new Point(_downButton.Left - _upButton.Width, 0);
			_upButton.TabIndex = 6;
			_upButton.Click += UpButton_Click;

			_firstAddressPanel.Controls.Add(_firstAddressLabel);
			_firstAddressPanel.Controls.Add(_firstAddressTextBox);
			_firstAddressPanel.Controls.Add(_addressHistorySelector);
			_firstAddressPanel.Controls.Add(_setClipboardLabel);
			_firstAddressPanel.Controls.Add(_mixCharButtons);
			_firstAddressPanel.Controls.Add(_exportButton);
			_firstAddressPanel.Controls.Add(_upButton);
			_firstAddressPanel.Controls.Add(_downButton);
			_firstAddressPanel.Dock = DockStyle.Top;
			_firstAddressPanel.Name = "mFirstAddressPanel";
			_firstAddressPanel.TabIndex = 0;
			_firstAddressPanel.Size = new Size(Width, _firstAddressTextBox.Height + 2);

			_wordEditorList = new WordEditorList(_memory.MinWordIndex, _memory.MaxWordIndex, CreateWordEditor, LoadWordEditor)
			{
				Dock = DockStyle.Fill,
				ReadOnly = _readOnly,
				BorderStyle = BorderStyle.FixedSingle
			};
			_wordEditorList.FirstVisibleIndexChanged += MWordEditorList_FirstVisibleIndexChanged;

			Controls.Add(_wordEditorList);
			Controls.Add(_firstAddressPanel);
			Name = "MemoryEditor";
			SizeChanged += MemoryEditor_SizeChanged;

			_firstAddressPanel.ResumeLayout(false);
			ResumeLayout(false);

			SetUpDownButtonStates(NavigationDirection.None);
		}

		private void SetUpButtonState(Memory memory)
		{
			_prevDataIndex = null;

			if (_wordEditorList.FirstVisibleIndex != _memory.MinWordIndex)
			{
				var firstIndex = Math.Max(_memory.MinWordIndex, _wordEditorList.FirstVisibleIndex - _wordEditorList.VisibleEditorCount);

				for (int index = firstIndex; index < _wordEditorList.FirstVisibleIndex; index++)
				{
					if (memory.HasContents(index))
					{
						_prevDataIndex = index;
						break;
					}
				}

				if (_prevDataIndex == null)
				{
					var lastPreviousDataIndex = memory.LastAddressWithContentsBefore(firstIndex);
					if (lastPreviousDataIndex != null)
					{
						_prevDataIndex = lastPreviousDataIndex.Value;

						for (int index = lastPreviousDataIndex.Value - 1; index >= Math.Max(_memory.MinWordIndex, lastPreviousDataIndex.Value - _wordEditorList.VisibleEditorCount + 1); index--)
						{
							if (memory.HasContents(index))
								_prevDataIndex = index;
						}
					}
				}
			}

			_upButton.Enabled = _prevDataIndex.HasValue;
		}

		private void SetDownButtonState(Memory memory)
		{
			_nextDataIndex = null;

			if (_wordEditorList.FirstVisibleIndex + _wordEditorList.VisibleEditorCount <= _memory.MaxWordIndex)
			{
				var lastIndex = Math.Min(_memory.MaxWordIndex, _wordEditorList.FirstVisibleIndex + (2 * _wordEditorList.VisibleEditorCount) - 1);

				for (int index = _wordEditorList.FirstVisibleIndex + _wordEditorList.VisibleEditorCount; index <= lastIndex; index++)
				{
					if (memory.HasContents(index))
					{
						_nextDataIndex = index;
						break;
					}
				}

				if (_nextDataIndex == null)
					_nextDataIndex = memory.FirstAddressWithContentsAfter(lastIndex);
			}

			_downButton.Enabled = _nextDataIndex.HasValue;
		}

		private void SetUpDownButtonStates(NavigationDirection direction)
		{
			if (_memory is not Memory memory || _wordEditorList == null)
				return;

			if (direction != NavigationDirection.Up || (_prevDataIndex.HasValue && _prevDataIndex.Value >= _wordEditorList.FirstVisibleIndex))
				SetUpButtonState(memory);

			if (direction != NavigationDirection.Down || (_nextDataIndex.HasValue && _nextDataIndex.Value <= _wordEditorList.FirstVisibleIndex + _wordEditorList.VisibleEditorCount))
				SetDownButtonState(memory);
		}

		public EditorListViewInfo GetCurrentListViewInfo()
		{
			var viewInfo = new EditorListViewInfo { FirstVisibleIndex = _wordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = _wordEditorList.ActiveEditorIndex;
			if (selectedEditorIndex >= 0)
			{
				viewInfo.SelectedIndex = viewInfo.FirstVisibleIndex + selectedEditorIndex;
				viewInfo.FocusedField = _wordEditorList[selectedEditorIndex].FocusedField;
			}

			return viewInfo;
		}

		private void MAddressHistorySelector_ItemSelected(object sender, ItemSelectedEventArgs<EditorListViewInfo> e)
		{
			EditorListViewInfo viewInfo = e.SelectedItem;

			if (!viewInfo.SelectedIndex.HasValue)
			{
				_wordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
				return;
			}

			int selectedIndex = viewInfo.SelectedIndex.Value;

			if (selectedIndex < viewInfo.FirstVisibleIndex + _wordEditorList.VisibleEditorCount)
			{
				_wordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
				_wordEditorList[viewInfo.SelectedIndex.Value - viewInfo.FirstVisibleIndex].Focus(viewInfo.FocusedField, null);
			}
			else
			{
				_wordEditorList.MakeIndexVisible(selectedIndex);
			}
		}

		private void MWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			if (args.FirstVisibleIndex < _firstAddressTextBox.LongValue)
				SetUpDownButtonStates(NavigationDirection.Up);

			else if (args.FirstVisibleIndex > _firstAddressTextBox.LongValue)
				SetUpDownButtonStates(NavigationDirection.Down);

			_firstAddressTextBox.LongValue = args.FirstVisibleIndex;
		}

		public void MakeAddressVisible(int address, bool trackChange)
		{
			if (!trackChange)
			{
				_wordEditorList.MakeIndexVisible(address);
				return;
			}

			var oldViewInfo = GetCurrentListViewInfo();

			_wordEditorList.MakeIndexVisible(address);

			var newViewInfo = new EditorListViewInfo { FirstVisibleIndex = _wordEditorList.FirstVisibleIndex, SelectedIndex = address };
			int selectedEditorIndex = _wordEditorList.ActiveEditorIndex;

			if (selectedEditorIndex >= 0)
				newViewInfo.FocusedField = _wordEditorList[selectedEditorIndex].FocusedField;

			_addressHistorySelector.AddItem(oldViewInfo, newViewInfo);
		}

		private void FirstAddressTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			var oldViewInfo = new EditorListViewInfo { FirstVisibleIndex = _wordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = _wordEditorList.ActiveEditorIndex;

			if (selectedEditorIndex >= 0)
			{
				oldViewInfo.SelectedIndex = oldViewInfo.FirstVisibleIndex + selectedEditorIndex;
				oldViewInfo.FocusedField = _wordEditorList[selectedEditorIndex].FocusedField;
			}

			_wordEditorList.FirstVisibleIndex = (int)args.NewValue;

			_addressHistorySelector.AddItem(oldViewInfo, new EditorListViewInfo { FirstVisibleIndex = _wordEditorList.FirstVisibleIndex });
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => _indexedAddressCalculatorCallback;
			set
			{
				_indexedAddressCalculatorCallback = value;

				if (_wordEditorList != null)
				{
					foreach (MemoryWordEditor wordEditor in _wordEditorList)
						wordEditor.IndexedAddressCalculatorCallback = _indexedAddressCalculatorCallback;
				}
			}
		}

		public bool ResizeInProgress
		{
			get => _wordEditorList != null && _wordEditorList.ResizeInProgress;

			set
			{
				if (_wordEditorList != null)
					_wordEditorList.ResizeInProgress = value;
			}
		}

		public new void Update()
		{
			_profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = _memory.MaxProfilingExecutionCount;
			_profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = _memory.MaxProfilingTickCount;
			_wordEditorList.Update();
			SetUpDownButtonStates(NavigationDirection.None);

			base.Update();
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			_firstAddressTextBox.UpdateLayout();

			_mixCharButtons.UpdateLayout();

			_wordEditorList.UpdateLayout();

			ResumeLayout();
		}

		public int FirstVisibleAddress
		{
			get => _wordEditorList != null ? _wordEditorList.FirstVisibleIndex : 0;
			set
			{
				if (_wordEditorList != null)
					_wordEditorList.FirstVisibleIndex = value;
			}
		}

		public void FindMatch(SearchParameters options)
		{
			int activeEditorIndex = _wordEditorList.ActiveEditorIndex;
			IWordEditor activeEditor = activeEditorIndex >= 0 ? _wordEditorList[activeEditorIndex] : null;

			if (activeEditor != null)
			{
				options.SearchFromWordIndex = ((IMemoryFullWord)activeEditor.WordValue).Index;
				options.SearchFromField = activeEditor.FocusedField ?? FieldTypes.None;
				options.SearchFromFieldIndex = options.SearchFromField != FieldTypes.None ? activeEditor.CaretIndex ?? 0 : 0;
			}
			else
			{
				if (_wordEditorList.FirstVisibleIndex > options.SearchFromWordIndex || _wordEditorList.FirstVisibleIndex + _wordEditorList.VisibleEditorCount <= options.SearchFromWordIndex)
					options.SearchFromWordIndex = _wordEditorList.FirstVisibleIndex;

				options.SearchFromField = FieldTypes.None;
				options.SearchFromFieldIndex = 0;
			}

			var result = _memory.FindMatch(options);

			if (result == null)
			{
				MessageBox.Show(this, $"Search text \"{options.SearchText}\" could not be found.", "Text not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			MakeAddressVisible(result.WordIndex);
			activeEditor = _wordEditorList[result.WordIndex - _wordEditorList.FirstVisibleIndex];
			activeEditor.Focus(result.Field, result.FieldIndex);
			activeEditor.Select(result.FieldIndex, options.SearchText.Length);
		}

		public int MarkedAddress
		{
			get => _markedAddress;
			set
			{
				if (_markedAddress == value)
					return;

				SetAddressMarkIfVisible(false);

				_markedAddress = value;

				SetAddressMarkIfVisible(true);
			}
		}

		private void SetAddressMarkIfVisible(bool mark)
		{
			int firstVisibleIndex = _wordEditorList.FirstVisibleIndex;

			if (_markedAddress >= firstVisibleIndex && _markedAddress < firstVisibleIndex + _wordEditorList.EditorCount)
				((MemoryWordEditor)_wordEditorList[_markedAddress - firstVisibleIndex]).Marked = mark;
		}

		public IMemory Memory
		{
			get => _memory;
			set
			{
				if (_memory != null || value == null)
					return;

				_memory = value;
				InitializeComponent();
				_profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = _memory.MaxProfilingExecutionCount;
				_profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = _memory.MaxProfilingTickCount;
			}
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly == value)
					return;

				_readOnly = value;
				if (_wordEditorList != null)
					_wordEditorList.ReadOnly = _readOnly;
			}
		}

		public SymbolCollection Symbols
		{
			get => _symbols;
			set
			{
				_symbols = value;

				if (_wordEditorList != null)
				{
					foreach (MemoryWordEditor editor in _wordEditorList)
						editor.Symbols = _symbols;
				}
			}
		}

		private void UpButton_Click(object sender, EventArgs args)
		{
			if (_prevDataIndex.HasValue)
				FirstVisibleAddress = _prevDataIndex.Value;
		}

		private void DownButton_Click(object sender, EventArgs args)
		{
			if (_nextDataIndex.HasValue)
				FirstVisibleAddress = _nextDataIndex.Value;
		}

		private void ExportButton_Click(object sender, EventArgs args)
		{
			var exportDialog = new MemoryExportDialog
			{
				MinMemoryIndex = _memory.MinWordIndex,
				MaxMemoryIndex = _memory.MaxWordIndex,
				FromAddress = _wordEditorList.FirstVisibleIndex,
				ToAddress = _wordEditorList.FirstVisibleIndex + _wordEditorList.VisibleEditorCount - 1,
				ProgramCounter = _markedAddress
			};

			if (exportDialog.ShowDialog(this) != DialogResult.OK)
				return;

			if (_saveExportFileDialog == null)
			{
				_saveExportFileDialog = new SaveFileDialog
				{
					DefaultExt = "mixdeck",
					Filter = "MixEmul card deck files|*.mixdeck|All files|*.*",
					Title = "Specify export file name"
				};
			}

			if (_saveExportFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			IFullWord[] memoryWords = new IFullWord[exportDialog.ToAddress - exportDialog.FromAddress + 1];

			int fromAddressOffset = exportDialog.FromAddress;
			for (int index = 0; index < memoryWords.Length; index++)
				memoryWords[index] = _memory[fromAddressOffset + index];

			try
			{
				CardDeckExporter.ExportFullWords(_saveExportFileDialog.FileName, memoryWords, fromAddressOffset, exportDialog.ProgramCounter);
				MessageBox.Show(this, "Memory successfully exported.", "Export successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error while exporting memory: " + ex.Message, "Error while exporting", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private enum NavigationDirection
		{
			None,
			Up,
			Down
		}
	}
}
