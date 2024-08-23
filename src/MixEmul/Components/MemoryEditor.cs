using System;
using System.Collections;
using System.Drawing;
using System.Linq;
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
		private readonly SortedList breakpoints;
		private Label firstAddressLabel;
		private Panel firstAddressPanel;
		private LongValueTextBox firstAddressTextBox;
		private int markedAddress;
		private IMemory memory;
		private readonly Label noMemoryLabel;
		private bool readOnly;
		private Label setClipboardLabel;
		private ToolTip toolTip;
		private WordEditorList wordEditorList;
		private IndexedAddressCalculatorCallback indexedAddressCalculatorCallback;
		private SymbolCollection symbols;
		private MixCharClipboardButtonControl mixCharButtons;
		private Button exportButton;
		private Button upButton;
		private Button downButton;
		private LinkedItemsSelectorControl<EditorListViewInfo> addressHistorySelector;
		private SaveFileDialog saveExportFileDialog;
		private int? prevDataIndex;
		private int? nextDataIndex;
		private readonly long[] profilingMaxCounts;

		public event AddressSelectedHandler AddressSelected;

		public MemoryEditor(IMemory memory = null)
		{
			this.readOnly = false;
			this.markedAddress = -1;

			this.profilingMaxCounts = new long[Enum.GetValues(typeof(GuiSettings.ProfilingInfoType)).Length];

			this.breakpoints = SortedList.Synchronized([]);

			this.noMemoryLabel = new Label
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
			=> this.breakpoints.Values;

		private long GetMaxProfilingCount(GuiSettings.ProfilingInfoType infoType)
			=> this.profilingMaxCounts[(int)infoType];

		public bool IsBreakpointSet(int address)
			=> this.breakpoints.Contains(address);

		public void MakeAddressVisible(int address)
			=> MakeAddressVisible(address, true);

		public bool IsAddressVisible(int address)
			=> this.wordEditorList.IsIndexVisible(address);

		public void ClearHistory()
			=> this.addressHistorySelector.Clear();

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
			get => this.toolTip;
			set
			{
				this.toolTip = value;

				if (this.wordEditorList != null)
				{
					foreach (MemoryWordEditor editor in this.wordEditorList.Cast<MemoryWordEditor>())
						editor.ToolTip = this.toolTip;
				}
			}
		}

		private IWordEditor CreateWordEditor(int address)
		{
			var editor = new MemoryWordEditor(address >= this.memory.MinWordIndex ? this.memory[address] : new MemoryFullWord(address))
			{
				GetMaxProfilingCount = GetMaxProfilingCount,
				MemoryMinIndex = this.memory.MinWordIndex,
				MemoryMaxIndex = this.memory.MaxWordIndex,
				IndexedAddressCalculatorCallback = this.indexedAddressCalculatorCallback,
				BreakPointChecked = this.breakpoints.Contains(address),
				Marked = this.markedAddress == address,
				ToolTip = this.toolTip,
				Symbols = this.symbols
			};

			editor.BreakpointCheckedChanged += BreakPointCheckedChanged;
			editor.AddressDoubleClick += AddressDoubleClick;
			editor.AddressSelected += This_AddressSelected;

			return editor;
		}

		private void LoadWordEditor(IWordEditor editor, int address)
		{
			var memoryEditor = (MemoryWordEditor)editor;

			if (address == memoryEditor.MemoryWord.Index && address >= this.memory.MinWordIndex && memoryEditor.MemoryWord.Equals(this.memory[address]))
				memoryEditor.Update();

			else
				memoryEditor.MemoryWord = address >= this.memory.MinWordIndex ? this.memory[address] : new MemoryFullWord(address);

			memoryEditor.BreakPointChecked = this.breakpoints.Contains(address);
			memoryEditor.Marked = this.markedAddress == address;
		}

		private void BreakPointCheckedChanged(object sender, EventArgs e)
		{
			if (this.wordEditorList.IsReloading)
				return;

			var editor = (MemoryWordEditor)sender;
			int memoryAddress = editor.MemoryWord.Index;

			if (editor.BreakPointChecked)
				this.breakpoints.Add(memoryAddress, memoryAddress);

			else
				this.breakpoints.Remove(memoryAddress);
		}

		public void ClearBreakpoints()
		{
			this.breakpoints.Clear();
			foreach (MemoryWordEditor editor in this.wordEditorList.Cast<MemoryWordEditor>())
				editor.BreakPointChecked = false;
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			Controls.Clear();

			if (this.memory == null)
			{
				Controls.Add(this.noMemoryLabel);
				this.noMemoryLabel.Size = Size;

				ResumeLayout(false);
				return;
			}

			this.firstAddressPanel = new Panel();
			this.firstAddressLabel = new Label();
			this.firstAddressTextBox = new LongValueTextBox(this.memory.MinWordIndex, this.memory.MaxWordIndex);
			this.setClipboardLabel = new Label();
			this.addressHistorySelector = new LinkedItemsSelectorControl<EditorListViewInfo>();
			this.addressHistorySelector.SetCurrentItemCallback(GetCurrentListViewInfo);
			this.mixCharButtons = new MixCharClipboardButtonControl();
			this.exportButton = new Button();
			this.upButton = new Button();
			this.downButton = new Button();

			this.firstAddressPanel.SuspendLayout();
			this.firstAddressLabel.Location = new Point(0, 0);
			this.firstAddressLabel.Name = "mFirstAddressLabel";
			this.firstAddressLabel.Size = new Size(120, 20);
			this.firstAddressLabel.TabIndex = 0;
			this.firstAddressLabel.Text = "First visible address:";
			this.firstAddressLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.firstAddressTextBox.Location = new Point(this.firstAddressLabel.Width, 0);
			this.firstAddressTextBox.Name = "mFirstAddressTextBox";
			this.firstAddressTextBox.Size = new Size(35, 20);
			this.firstAddressTextBox.TabIndex = 1;
			this.firstAddressTextBox.ClearZero = false;
			this.firstAddressTextBox.ValueChanged += FirstAddressTextBox_ValueChanged;

			this.addressHistorySelector.Location = new Point(this.firstAddressTextBox.Right + 4, 0);
			this.addressHistorySelector.Name = "mAddressHistorySelector";
			this.addressHistorySelector.TabIndex = 2;
			this.addressHistorySelector.ItemSelected += MAddressHistorySelector_ItemSelected;

			this.setClipboardLabel.Location = new Point(this.addressHistorySelector.Right + 16, 0);
			this.setClipboardLabel.Name = "mSetClipboardLabel";
			this.setClipboardLabel.Size = new Size(95, this.firstAddressLabel.Height);
			this.setClipboardLabel.TabIndex = 3;
			this.setClipboardLabel.Text = "Set clipboard to:";
			this.setClipboardLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.mixCharButtons.Location = new Point(this.setClipboardLabel.Right, 0);
			this.mixCharButtons.Name = "mMixCharButtons";
			this.mixCharButtons.TabIndex = 4;

			this.exportButton.Location = new Point(this.mixCharButtons.Right + 16, 0);
			this.exportButton.Name = "mExportButton";
			this.exportButton.Size = new Size(62, 21);
			this.exportButton.TabIndex = 5;
			this.exportButton.Text = "&Export...";
			this.exportButton.FlatStyle = FlatStyle.Flat;
			this.exportButton.Click += ExportButton_Click;

			this.downButton.Visible = this.memory is Memory;
			this.downButton.Enabled = false;
			this.downButton.FlatStyle = FlatStyle.Flat;
			this.downButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.downButton.Image = Resources.Symbols_Down_16xLG;
			this.downButton.Name = "mDownButton";
			this.downButton.Size = new Size(21, 21);
			this.downButton.Location = new Point(Width - this.downButton.Width, 0);
			this.downButton.TabIndex = 7;
			this.downButton.Click += DownButton_Click;

			this.upButton.Visible = this.memory is Memory;
			this.upButton.Enabled = false;
			this.upButton.FlatStyle = FlatStyle.Flat;
			this.upButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.upButton.Image = Resources.Symbols_Up_16xLG___kopie;
			this.upButton.Name = "mUpButton";
			this.upButton.Size = new Size(21, 21);
			this.upButton.Location = new Point(this.downButton.Left - this.upButton.Width, 0);
			this.upButton.TabIndex = 6;
			this.upButton.Click += UpButton_Click;

			this.firstAddressPanel.Controls.Add(this.firstAddressLabel);
			this.firstAddressPanel.Controls.Add(this.firstAddressTextBox);
			this.firstAddressPanel.Controls.Add(this.addressHistorySelector);
			this.firstAddressPanel.Controls.Add(this.setClipboardLabel);
			this.firstAddressPanel.Controls.Add(this.mixCharButtons);
			this.firstAddressPanel.Controls.Add(this.exportButton);
			this.firstAddressPanel.Controls.Add(this.upButton);
			this.firstAddressPanel.Controls.Add(this.downButton);
			this.firstAddressPanel.Dock = DockStyle.Top;
			this.firstAddressPanel.Name = "mFirstAddressPanel";
			this.firstAddressPanel.TabIndex = 0;
			this.firstAddressPanel.Size = new Size(Width, this.firstAddressTextBox.Height + 2);

			this.wordEditorList = new WordEditorList(this.memory.MinWordIndex, this.memory.MaxWordIndex, CreateWordEditor, LoadWordEditor)
			{
				Dock = DockStyle.Fill,
				ReadOnly = this.readOnly,
				BorderStyle = BorderStyle.FixedSingle
			};
			this.wordEditorList.FirstVisibleIndexChanged += MWordEditorList_FirstVisibleIndexChanged;

			Controls.Add(this.wordEditorList);
			Controls.Add(this.firstAddressPanel);
			Name = "MemoryEditor";
			SizeChanged += MemoryEditor_SizeChanged;

			this.firstAddressPanel.ResumeLayout(false);
			ResumeLayout(false);

			SetUpDownButtonStates(NavigationDirection.None);
		}

		private void SetUpButtonState(Memory memory)
		{
			this.prevDataIndex = null;

			if (this.wordEditorList.FirstVisibleIndex != this.memory.MinWordIndex)
			{
				var firstIndex = Math.Max(this.memory.MinWordIndex, this.wordEditorList.FirstVisibleIndex - this.wordEditorList.VisibleEditorCount);

				for (int index = firstIndex; index < this.wordEditorList.FirstVisibleIndex; index++)
				{
					if (memory.HasContents(index))
					{
						this.prevDataIndex = index;
						break;
					}
				}

				if (this.prevDataIndex == null)
				{
					var lastPreviousDataIndex = memory.LastAddressWithContentsBefore(firstIndex);
					if (lastPreviousDataIndex != null)
					{
						this.prevDataIndex = lastPreviousDataIndex.Value;

						for (int index = lastPreviousDataIndex.Value - 1; index >= Math.Max(this.memory.MinWordIndex, lastPreviousDataIndex.Value - this.wordEditorList.VisibleEditorCount + 1); index--)
						{
							if (memory.HasContents(index))
								this.prevDataIndex = index;
						}
					}
				}
			}

			this.upButton.Enabled = this.prevDataIndex.HasValue;
		}

		private void SetDownButtonState(Memory memory)
		{
			this.nextDataIndex = null;

			if (this.wordEditorList.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount <= this.memory.MaxWordIndex)
			{
				var lastIndex = Math.Min(this.memory.MaxWordIndex, this.wordEditorList.FirstVisibleIndex + (2 * this.wordEditorList.VisibleEditorCount) - 1);

				for (int index = this.wordEditorList.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount; index <= lastIndex; index++)
				{
					if (memory.HasContents(index))
					{
						this.nextDataIndex = index;
						break;
					}
				}

				this.nextDataIndex ??= memory.FirstAddressWithContentsAfter(lastIndex);
			}

			this.downButton.Enabled = this.nextDataIndex.HasValue;
		}

		private void SetUpDownButtonStates(NavigationDirection direction)
		{
			if (this.memory is not Memory memory || this.wordEditorList == null)
				return;

			if (direction != NavigationDirection.Up || (this.prevDataIndex.HasValue && this.prevDataIndex.Value >= this.wordEditorList.FirstVisibleIndex))
				SetUpButtonState(memory);

			if (direction != NavigationDirection.Down || (this.nextDataIndex.HasValue && this.nextDataIndex.Value <= this.wordEditorList.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount))
				SetDownButtonState(memory);
		}

		public EditorListViewInfo GetCurrentListViewInfo()
		{
			var viewInfo = new EditorListViewInfo { FirstVisibleIndex = this.wordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = this.wordEditorList.ActiveEditorIndex;
			if (selectedEditorIndex >= 0)
			{
				viewInfo.SelectedIndex = viewInfo.FirstVisibleIndex + selectedEditorIndex;
				viewInfo.FocusedField = this.wordEditorList[selectedEditorIndex].FocusedField;
			}

			return viewInfo;
		}

		private void MAddressHistorySelector_ItemSelected(object sender, ItemSelectedEventArgs<EditorListViewInfo> e)
		{
			EditorListViewInfo viewInfo = e.SelectedItem;

			if (!viewInfo.SelectedIndex.HasValue)
			{
				this.wordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
				return;
			}

			int selectedIndex = viewInfo.SelectedIndex.Value;

			if (selectedIndex < viewInfo.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount)
			{
				this.wordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
				this.wordEditorList[viewInfo.SelectedIndex.Value - viewInfo.FirstVisibleIndex].Focus(viewInfo.FocusedField, null);
			}
			else
			{
				this.wordEditorList.MakeIndexVisible(selectedIndex);
			}
		}

		private void MWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			if (args.FirstVisibleIndex < this.firstAddressTextBox.LongValue)
				SetUpDownButtonStates(NavigationDirection.Up);

			else if (args.FirstVisibleIndex > this.firstAddressTextBox.LongValue)
				SetUpDownButtonStates(NavigationDirection.Down);

			this.firstAddressTextBox.LongValue = args.FirstVisibleIndex;
		}

		public void MakeAddressVisible(int address, bool trackChange)
		{
			if (!trackChange)
			{
				this.wordEditorList.MakeIndexVisible(address);
				return;
			}

			var oldViewInfo = GetCurrentListViewInfo();

			this.wordEditorList.MakeIndexVisible(address);

			var newViewInfo = new EditorListViewInfo { FirstVisibleIndex = this.wordEditorList.FirstVisibleIndex, SelectedIndex = address };
			int selectedEditorIndex = this.wordEditorList.ActiveEditorIndex;

			if (selectedEditorIndex >= 0)
				newViewInfo.FocusedField = this.wordEditorList[selectedEditorIndex].FocusedField;

			this.addressHistorySelector.AddItem(oldViewInfo, newViewInfo);
		}

		private void FirstAddressTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			var oldViewInfo = new EditorListViewInfo { FirstVisibleIndex = this.wordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = this.wordEditorList.ActiveEditorIndex;

			if (selectedEditorIndex >= 0)
			{
				oldViewInfo.SelectedIndex = oldViewInfo.FirstVisibleIndex + selectedEditorIndex;
				oldViewInfo.FocusedField = this.wordEditorList[selectedEditorIndex].FocusedField;
			}

			this.wordEditorList.FirstVisibleIndex = (int)args.NewValue;

			this.addressHistorySelector.AddItem(oldViewInfo, new EditorListViewInfo { FirstVisibleIndex = this.wordEditorList.FirstVisibleIndex });
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => this.indexedAddressCalculatorCallback;
			set
			{
				this.indexedAddressCalculatorCallback = value;

				if (this.wordEditorList != null)
				{
					foreach (MemoryWordEditor wordEditor in this.wordEditorList.Cast<MemoryWordEditor>())
						wordEditor.IndexedAddressCalculatorCallback = this.indexedAddressCalculatorCallback;
				}
			}
		}

		public bool ResizeInProgress
		{
			get => this.wordEditorList != null && this.wordEditorList.ResizeInProgress;

			set
			{
				if (this.wordEditorList != null)
					this.wordEditorList.ResizeInProgress = value;
			}
		}

		public new void Update()
		{
			this.profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = this.memory.MaxProfilingExecutionCount;
			this.profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = this.memory.MaxProfilingTickCount;
			this.wordEditorList.Update();
			SetUpDownButtonStates(NavigationDirection.None);

			base.Update();
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			this.firstAddressTextBox.UpdateLayout();

			this.mixCharButtons.UpdateLayout();

			this.wordEditorList.UpdateLayout();

			ResumeLayout();
		}

		public int FirstVisibleAddress
		{
			get => this.wordEditorList != null ? this.wordEditorList.FirstVisibleIndex : 0;
			set
			{
				if (this.wordEditorList != null)
					this.wordEditorList.FirstVisibleIndex = value;
			}
		}

		public void FindMatch(SearchParameters options)
		{
			int activeEditorIndex = this.wordEditorList.ActiveEditorIndex;
			IWordEditor activeEditor = activeEditorIndex >= 0 ? this.wordEditorList[activeEditorIndex] : null;

			if (activeEditor != null)
			{
				options.SearchFromWordIndex = ((IMemoryFullWord)activeEditor.WordValue).Index;
				options.SearchFromField = activeEditor.FocusedField ?? FieldTypes.None;
				options.SearchFromFieldIndex = options.SearchFromField != FieldTypes.None ? activeEditor.CaretIndex ?? 0 : 0;
			}
			else
			{
				if (this.wordEditorList.FirstVisibleIndex > options.SearchFromWordIndex || this.wordEditorList.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount <= options.SearchFromWordIndex)
					options.SearchFromWordIndex = this.wordEditorList.FirstVisibleIndex;

				options.SearchFromField = FieldTypes.None;
				options.SearchFromFieldIndex = 0;
			}

			var result = this.memory.FindMatch(options);

			if (result == null)
			{
				MessageBox.Show(this, $"Search text \"{options.SearchText}\" could not be found.", "Text not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			MakeAddressVisible(result.WordIndex);
			activeEditor = this.wordEditorList[result.WordIndex - this.wordEditorList.FirstVisibleIndex];
			activeEditor.Focus(result.Field, result.FieldIndex);
			activeEditor.Select(result.FieldIndex, options.SearchText.Length);
		}

		public int MarkedAddress
		{
			get => this.markedAddress;
			set
			{
				if (this.markedAddress == value)
					return;

				SetAddressMarkIfVisible(false);

				this.markedAddress = value;

				SetAddressMarkIfVisible(true);
			}
		}

		private void SetAddressMarkIfVisible(bool mark)
		{
			int firstVisibleIndex = this.wordEditorList.FirstVisibleIndex;

			if (this.markedAddress >= firstVisibleIndex && this.markedAddress < firstVisibleIndex + this.wordEditorList.EditorCount)
				((MemoryWordEditor)this.wordEditorList[this.markedAddress - firstVisibleIndex]).Marked = mark;
		}

		public IMemory Memory
		{
			get => this.memory;
			set
			{
				if (this.memory != null || value == null)
					return;

				this.memory = value;
				InitializeComponent();
				this.profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = this.memory.MaxProfilingExecutionCount;
				this.profilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = this.memory.MaxProfilingTickCount;
			}
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (this.readOnly == value)
					return;

				this.readOnly = value;
				if (this.wordEditorList != null)
					this.wordEditorList.ReadOnly = this.readOnly;
			}
		}

		public SymbolCollection Symbols
		{
			get => this.symbols;
			set
			{
				this.symbols = value;

				if (this.wordEditorList != null)
				{
					foreach (MemoryWordEditor editor in this.wordEditorList.Cast<MemoryWordEditor>())
						editor.Symbols = this.symbols;
				}
			}
		}

		private void UpButton_Click(object sender, EventArgs args)
		{
			if (this.prevDataIndex.HasValue)
				FirstVisibleAddress = this.prevDataIndex.Value;
		}

		private void DownButton_Click(object sender, EventArgs args)
		{
			if (this.nextDataIndex.HasValue)
				FirstVisibleAddress = this.nextDataIndex.Value;
		}

		private void ExportButton_Click(object sender, EventArgs args)
		{
			var exportDialog = new MemoryExportDialog
			{
				MinMemoryIndex = this.memory.MinWordIndex,
				MaxMemoryIndex = this.memory.MaxWordIndex,
				FromAddress = this.wordEditorList.FirstVisibleIndex,
				ToAddress = this.wordEditorList.FirstVisibleIndex + this.wordEditorList.VisibleEditorCount - 1,
				ProgramCounter = this.markedAddress
			};

			if (exportDialog.ShowDialog(this) != DialogResult.OK)
				return;

			this.saveExportFileDialog ??= new SaveFileDialog
			{
				DefaultExt = "mixdeck",
				Filter = "MixEmul card deck files|*.mixdeck|All files|*.*",
				Title = "Specify export file name"
			};

			if (this.saveExportFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			IFullWord[] memoryWords = new IFullWord[exportDialog.ToAddress - exportDialog.FromAddress + 1];

			int fromAddressOffset = exportDialog.FromAddress;
			for (int index = 0; index < memoryWords.Length; index++)
				memoryWords[index] = this.memory[fromAddressOffset + index];

			try
			{
				CardDeckExporter.ExportFullWords(this.saveExportFileDialog.FileName, memoryWords, fromAddressOffset, exportDialog.ProgramCounter);
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
