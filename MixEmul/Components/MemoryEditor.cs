using MixGui.Events;
using MixGui.Properties;
using MixGui.Settings;
using MixGui.Utils;
using MixLib;
using MixLib.Misc;
using MixLib.Type;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class MemoryEditor : UserControl, IBreakpointManager
	{
		readonly SortedList mBreakpoints;
		Label mFirstAddressLabel;
		Panel mFirstAddressPanel;
		LongValueTextBox mFirstAddressTextBox;
		int mMarkedAddress;
		IMemory mMemory;
		readonly Label mNoMemoryLabel;
		bool mReadOnly;
		Label mSetClipboardLabel;
		ToolTip mToolTip;
		WordEditorList mWordEditorList;
		IndexedAddressCalculatorCallback mIndexedAddressCalculatorCallback;
		SymbolCollection mSymbols;
		MixCharClipboardButtonControl mMixCharButtons;
		Button mExportButton;
		Button mUpButton;
		Button mDownButton;
		LinkedItemsSelectorControl<EditorListViewInfo> mAddressHistorySelector;
		SaveFileDialog mSaveExportFileDialog;
		int? mPrevDataIndex;
		int? mNextDataIndex;
		readonly long[] mProfilingMaxCounts;

		public event AddressSelectedHandler AddressSelected;

		public MemoryEditor()
			: this(null)
		{
		}

		public MemoryEditor(IMemory memory)
		{
			mReadOnly = false;
			mMarkedAddress = -1;

			mProfilingMaxCounts = new long[Enum.GetValues(typeof(GuiSettings.ProfilingInfoType)).Length];

			mBreakpoints = SortedList.Synchronized(new SortedList());

			mNoMemoryLabel = new Label
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

		public ICollection Breakpoints => mBreakpoints.Values;

		long GetMaxProfilingCount(GuiSettings.ProfilingInfoType infoType)
		{
			return mProfilingMaxCounts[(int)infoType];
		}

		public bool IsBreakpointSet(int address)
		{
			return mBreakpoints.Contains(address);
		}

		public void MakeAddressVisible(int address)
		{
			MakeAddressVisible(address, true);
		}

		public bool IsAddressVisible(int address)
		{
			return mWordEditorList.IsIndexVisible(address);
		}

		public void ClearHistory()
		{
			mAddressHistorySelector.Clear();
		}

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args)
		{
			AddressSelected?.Invoke(this, args);
		}

		void This_AddressSelected(object sender, AddressSelectedEventArgs args)
		{
			MakeAddressVisible(args.SelectedAddress);
		}

		void AddressDoubleClick(object sender, EventArgs e)
		{
			OnAddressSelected(new AddressSelectedEventArgs(((MemoryWordEditor)sender).MemoryWord.Index));
		}

		void MemoryEditor_SizeChanged(object sender, EventArgs e)
		{
			SetUpDownButtonStates(NavigationDirection.None);
		}

		public ToolTip ToolTip
		{
			get => mToolTip;
			set
			{
				mToolTip = value;

				if (mWordEditorList != null)
				{
					foreach (MemoryWordEditor editor in mWordEditorList)
					{
						editor.ToolTip = mToolTip;
					}
				}
			}
		}

		IWordEditor CreateWordEditor(int address)
		{
			var editor = new MemoryWordEditor(address >= mMemory.MinWordIndex ? mMemory[address] : new MemoryFullWord(address))
			{
				GetMaxProfilingCount = GetMaxProfilingCount,
				MemoryMinIndex = mMemory.MinWordIndex,
				MemoryMaxIndex = mMemory.MaxWordIndex,
				IndexedAddressCalculatorCallback = mIndexedAddressCalculatorCallback,
				BreakPointChecked = mBreakpoints.Contains(address),
				Marked = mMarkedAddress == address,
				ToolTip = mToolTip,
				Symbols = mSymbols
			};
			editor.BreakpointCheckedChanged += BreakPointCheckedChanged;
			editor.AddressDoubleClick += AddressDoubleClick;
			editor.AddressSelected += This_AddressSelected;

			return editor;
		}

		void LoadWordEditor(IWordEditor editor, int address)
		{
			var memoryEditor = (MemoryWordEditor)editor;

			if (address == memoryEditor.MemoryWord.Index && address >= mMemory.MinWordIndex && memoryEditor.MemoryWord.Equals(mMemory[address]))
			{
				memoryEditor.Update();
			}
			else
			{
				memoryEditor.MemoryWord = address >= mMemory.MinWordIndex ? mMemory[address] : new MemoryFullWord(address);
			}

			memoryEditor.BreakPointChecked = mBreakpoints.Contains(address);
			memoryEditor.Marked = mMarkedAddress == address;
		}

		void BreakPointCheckedChanged(object sender, EventArgs e)
		{
			if (!mWordEditorList.IsReloading)
			{
				var editor = (MemoryWordEditor)sender;
				int memoryAddress = editor.MemoryWord.Index;

				if (editor.BreakPointChecked)
				{
					mBreakpoints.Add(memoryAddress, memoryAddress);
				}
				else
				{
					mBreakpoints.Remove(memoryAddress);
				}
			}
		}

		public void ClearBreakpoints()
		{
			mBreakpoints.Clear();
			foreach (MemoryWordEditor editor in mWordEditorList)
			{
				editor.BreakPointChecked = false;
			}
		}

		void InitializeComponent()
		{
			SuspendLayout();
			Controls.Clear();

			if (mMemory == null)
			{
				Controls.Add(mNoMemoryLabel);
				mNoMemoryLabel.Size = Size;

				ResumeLayout(false);
			}
			else
			{
				mFirstAddressPanel = new Panel();
				mFirstAddressLabel = new Label();
				mFirstAddressTextBox = new LongValueTextBox(mMemory.MinWordIndex, mMemory.MaxWordIndex);
				mSetClipboardLabel = new Label();
				mAddressHistorySelector = new LinkedItemsSelectorControl<EditorListViewInfo>();
				mAddressHistorySelector.SetCurrentItemCallback(GetCurrentListViewInfo);
				mMixCharButtons = new MixCharClipboardButtonControl();
				mExportButton = new Button();
				mUpButton = new Button();
				mDownButton = new Button();

				mFirstAddressPanel.SuspendLayout();
				mFirstAddressLabel.Location = new Point(0, 0);
				mFirstAddressLabel.Name = "mFirstAddressLabel";
				mFirstAddressLabel.Size = new Size(120, 20);
				mFirstAddressLabel.TabIndex = 0;
				mFirstAddressLabel.Text = "First visible address:";
				mFirstAddressLabel.TextAlign = ContentAlignment.MiddleLeft;

				mFirstAddressTextBox.Location = new Point(mFirstAddressLabel.Width, 0);
				mFirstAddressTextBox.Name = "mFirstAddressTextBox";
				mFirstAddressTextBox.Size = new Size(35, 20);
				mFirstAddressTextBox.TabIndex = 1;
				mFirstAddressTextBox.ClearZero = false;
				mFirstAddressTextBox.ValueChanged += MFirstAddressTextBox_ValueChanged;

				mAddressHistorySelector.Location = new Point(mFirstAddressTextBox.Right + 4, 0);
				mAddressHistorySelector.Name = "mAddressHistorySelector";
				mAddressHistorySelector.TabIndex = 2;
				mAddressHistorySelector.ItemSelected += MAddressHistorySelector_ItemSelected;

				mSetClipboardLabel.Location = new Point(mAddressHistorySelector.Right + 16, 0);
				mSetClipboardLabel.Name = "mSetClipboardLabel";
				mSetClipboardLabel.Size = new Size(95, mFirstAddressLabel.Height);
				mSetClipboardLabel.TabIndex = 3;
				mSetClipboardLabel.Text = "Set clipboard to:";
				mSetClipboardLabel.TextAlign = ContentAlignment.MiddleLeft;

				mMixCharButtons.Location = new Point(mSetClipboardLabel.Right, 0);
				mMixCharButtons.Name = "mMixCharButtons";
				mMixCharButtons.TabIndex = 4;

				mExportButton.Location = new Point(mMixCharButtons.Right + 16, 0);
				mExportButton.Name = "mExportButton";
				mExportButton.Size = new Size(62, 21);
				mExportButton.TabIndex = 5;
				mExportButton.Text = "&Export...";
				mExportButton.FlatStyle = FlatStyle.Flat;
				mExportButton.Click += MExportButton_Click;

				mDownButton.Visible = mMemory is Memory;
				mDownButton.Enabled = false;
				mDownButton.FlatStyle = FlatStyle.Flat;
				mDownButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				mDownButton.Image = Resources.Symbols_Down_16xLG;
				mDownButton.Name = "mDownButton";
				mDownButton.Size = new Size(21, 21);
				mDownButton.Location = new Point(Width - mDownButton.Width, 0);
				mDownButton.TabIndex = 7;
				mDownButton.Click += MDownButton_Click;

				mUpButton.Visible = mMemory is Memory;
				mUpButton.Enabled = false;
				mUpButton.FlatStyle = FlatStyle.Flat;
				mUpButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				mUpButton.Image = Resources.Symbols_Up_16xLG___kopie;
				mUpButton.Name = "mUpButton";
				mUpButton.Size = new Size(21, 21);
				mUpButton.Location = new Point(mDownButton.Left - mUpButton.Width, 0);
				mUpButton.TabIndex = 6;
				mUpButton.Click += MUpButton_Click;

				mFirstAddressPanel.Controls.Add(mFirstAddressLabel);
				mFirstAddressPanel.Controls.Add(mFirstAddressTextBox);
				mFirstAddressPanel.Controls.Add(mAddressHistorySelector);
				mFirstAddressPanel.Controls.Add(mSetClipboardLabel);
				mFirstAddressPanel.Controls.Add(mMixCharButtons);
				mFirstAddressPanel.Controls.Add(mExportButton);
				mFirstAddressPanel.Controls.Add(mUpButton);
				mFirstAddressPanel.Controls.Add(mDownButton);
				mFirstAddressPanel.Dock = DockStyle.Top;
				mFirstAddressPanel.Name = "mFirstAddressPanel";
				mFirstAddressPanel.TabIndex = 0;
				mFirstAddressPanel.Size = new Size(Width, mFirstAddressTextBox.Height + 2);

				mWordEditorList = new WordEditorList(mMemory.MinWordIndex, mMemory.MaxWordIndex, CreateWordEditor, LoadWordEditor)
				{
					Dock = DockStyle.Fill,
					ReadOnly = mReadOnly,
					BorderStyle = BorderStyle.FixedSingle
				};
				mWordEditorList.FirstVisibleIndexChanged += MWordEditorList_FirstVisibleIndexChanged;

				Controls.Add(mWordEditorList);
				Controls.Add(mFirstAddressPanel);
				Name = "MemoryEditor";
				SizeChanged += MemoryEditor_SizeChanged;

				mFirstAddressPanel.ResumeLayout(false);
				ResumeLayout(false);

				SetUpDownButtonStates(NavigationDirection.None);
			}
		}

		void SetUpButtonState(Memory memory)
		{
			mPrevDataIndex = null;

			if (mWordEditorList.FirstVisibleIndex != mMemory.MinWordIndex)
			{
				var firstIndex = Math.Max(mMemory.MinWordIndex, mWordEditorList.FirstVisibleIndex - mWordEditorList.VisibleEditorCount);

				for (int index = firstIndex; index < mWordEditorList.FirstVisibleIndex; index++)
				{
					if (memory.HasContents(index))
					{
						mPrevDataIndex = index;
						break;
					}
				}

				if (mPrevDataIndex == null)
				{
					var lastPreviousDataIndex = memory.LastAddressWithContentsBefore(firstIndex);
					if (lastPreviousDataIndex != null)
					{
						mPrevDataIndex = lastPreviousDataIndex.Value;

						for (int index = lastPreviousDataIndex.Value - 1; index >= Math.Max(mMemory.MinWordIndex, lastPreviousDataIndex.Value - mWordEditorList.VisibleEditorCount + 1); index--)
						{
							if (memory.HasContents(index))
							{
								mPrevDataIndex = index;
							}
						}
					}
				}
			}

			mUpButton.Enabled = mPrevDataIndex.HasValue;
		}

		void SetDownButtonState(Memory memory)
		{
			mNextDataIndex = null;

			if (mWordEditorList.FirstVisibleIndex + mWordEditorList.VisibleEditorCount <= mMemory.MaxWordIndex)
			{
				var lastIndex = Math.Min(mMemory.MaxWordIndex, mWordEditorList.FirstVisibleIndex + (2 * mWordEditorList.VisibleEditorCount) - 1);

				for (int index = mWordEditorList.FirstVisibleIndex + mWordEditorList.VisibleEditorCount; index <= lastIndex; index++)
				{
					if (memory.HasContents(index))
					{
						mNextDataIndex = index;
						break;
					}
				}

				if (mNextDataIndex == null)
				{
					mNextDataIndex = memory.FirstAddressWithContentsAfter(lastIndex);
				}
			}

			mDownButton.Enabled = mNextDataIndex.HasValue;
		}

		void SetUpDownButtonStates(NavigationDirection direction)
		{
			if (mMemory is not Memory memory || mWordEditorList == null)
			{
				return;
			}

			if (direction != NavigationDirection.Up || (mPrevDataIndex.HasValue && mPrevDataIndex.Value >= mWordEditorList.FirstVisibleIndex))
			{
				SetUpButtonState(memory);
			}

			if (direction != NavigationDirection.Down || (mNextDataIndex.HasValue && mNextDataIndex.Value <= mWordEditorList.FirstVisibleIndex + mWordEditorList.VisibleEditorCount))
			{
				SetDownButtonState(memory);
			}

		}

		public EditorListViewInfo GetCurrentListViewInfo()
		{
			var viewInfo = new EditorListViewInfo { FirstVisibleIndex = mWordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = mWordEditorList.ActiveEditorIndex;
			if (selectedEditorIndex >= 0)
			{
				viewInfo.SelectedIndex = viewInfo.FirstVisibleIndex + selectedEditorIndex;
				viewInfo.FocusedField = mWordEditorList[selectedEditorIndex].FocusedField;
			}

			return viewInfo;
		}

		void MAddressHistorySelector_ItemSelected(object sender, ItemSelectedEventArgs<EditorListViewInfo> e)
		{
			EditorListViewInfo viewInfo = e.SelectedItem;

			if (viewInfo.SelectedIndex.HasValue)
			{
				int selectedIndex = viewInfo.SelectedIndex.Value;

				if (selectedIndex < viewInfo.FirstVisibleIndex + mWordEditorList.VisibleEditorCount)
				{
					mWordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
					mWordEditorList[viewInfo.SelectedIndex.Value - viewInfo.FirstVisibleIndex].Focus(viewInfo.FocusedField, null);
				}
				else
				{
					mWordEditorList.MakeIndexVisible(selectedIndex);
				}
			}
			else
			{
				mWordEditorList.FirstVisibleIndex = viewInfo.FirstVisibleIndex;
			}
		}

		void MWordEditorList_FirstVisibleIndexChanged(EditorList<IWordEditor> sender, WordEditorList.FirstVisibleIndexChangedEventArgs args)
		{
			if (args.FirstVisibleIndex < mFirstAddressTextBox.LongValue)
			{
				SetUpDownButtonStates(NavigationDirection.Up);
			}
			else if (args.FirstVisibleIndex > mFirstAddressTextBox.LongValue)
			{
				SetUpDownButtonStates(NavigationDirection.Down);
			}

			mFirstAddressTextBox.LongValue = args.FirstVisibleIndex;
		}

		public void MakeAddressVisible(int address, bool trackChange)
		{
			if (trackChange)
			{
				var oldViewInfo = GetCurrentListViewInfo();

				mWordEditorList.MakeIndexVisible(address);

				var newViewInfo = new EditorListViewInfo { FirstVisibleIndex = mWordEditorList.FirstVisibleIndex, SelectedIndex = address };
				int selectedEditorIndex = mWordEditorList.ActiveEditorIndex;
				if (selectedEditorIndex >= 0)
				{
					newViewInfo.FocusedField = mWordEditorList[selectedEditorIndex].FocusedField;
				}

				mAddressHistorySelector.AddItem(oldViewInfo, newViewInfo);
			}
			else
			{
				mWordEditorList.MakeIndexVisible(address);
			}
		}

		void MFirstAddressTextBox_ValueChanged(LongValueTextBox source, LongValueTextBox.ValueChangedEventArgs args)
		{
			var oldViewInfo = new EditorListViewInfo { FirstVisibleIndex = mWordEditorList.FirstVisibleIndex };
			int selectedEditorIndex = mWordEditorList.ActiveEditorIndex;
			if (selectedEditorIndex >= 0)
			{
				oldViewInfo.SelectedIndex = oldViewInfo.FirstVisibleIndex + selectedEditorIndex;
				oldViewInfo.FocusedField = mWordEditorList[selectedEditorIndex].FocusedField;
			}

			mWordEditorList.FirstVisibleIndex = (int)args.NewValue;

			mAddressHistorySelector.AddItem(oldViewInfo, new EditorListViewInfo { FirstVisibleIndex = mWordEditorList.FirstVisibleIndex });
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => mIndexedAddressCalculatorCallback;
			set
			{
				mIndexedAddressCalculatorCallback = value;

				if (mWordEditorList != null)
				{
					foreach (MemoryWordEditor wordEditor in mWordEditorList)
					{
						wordEditor.IndexedAddressCalculatorCallback = mIndexedAddressCalculatorCallback;
					}
				}
			}
		}

		public bool ResizeInProgress
		{
			get => mWordEditorList != null && mWordEditorList.ResizeInProgress;

			set
			{
				if (mWordEditorList != null)
				{
					mWordEditorList.ResizeInProgress = value;
				}
			}
		}

		public new void Update()
		{
			mProfilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = mMemory.MaxProfilingExecutionCount;
			mProfilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = mMemory.MaxProfilingTickCount;
			mWordEditorList.Update();
			SetUpDownButtonStates(NavigationDirection.None);

			base.Update();
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			mFirstAddressTextBox.UpdateLayout();

			mMixCharButtons.UpdateLayout();

			mWordEditorList.UpdateLayout();

			ResumeLayout();
		}

		public int FirstVisibleAddress
		{
			get => mWordEditorList != null ? mWordEditorList.FirstVisibleIndex : 0;
			set
			{
				if (mWordEditorList != null)
				{
					mWordEditorList.FirstVisibleIndex = value;
				}
			}
		}

		public void FindMatch(SearchParameters options)
		{
			int activeEditorIndex = mWordEditorList.ActiveEditorIndex;
			IWordEditor activeEditor = activeEditorIndex >= 0 ? mWordEditorList[activeEditorIndex] : null;

			if (activeEditor != null)
			{
				options.SearchFromWordIndex = ((IMemoryFullWord)activeEditor.WordValue).Index;
				options.SearchFromField = activeEditor.FocusedField ?? FieldTypes.None;
				options.SearchFromFieldIndex = options.SearchFromField != FieldTypes.None ? activeEditor.CaretIndex ?? 0 : 0;
			}
			else
			{
				if (mWordEditorList.FirstVisibleIndex > options.SearchFromWordIndex || mWordEditorList.FirstVisibleIndex + mWordEditorList.VisibleEditorCount <= options.SearchFromWordIndex)
				{
					options.SearchFromWordIndex = mWordEditorList.FirstVisibleIndex;
				}
				options.SearchFromField = FieldTypes.None;
				options.SearchFromFieldIndex = 0;
			}

			var result = mMemory.FindMatch(options);

			if (result != null)
			{
				MakeAddressVisible(result.WordIndex);
				activeEditor = mWordEditorList[result.WordIndex - mWordEditorList.FirstVisibleIndex];
				activeEditor.Focus(result.Field, result.FieldIndex);
				activeEditor.Select(result.FieldIndex, options.SearchText.Length);
			}
			else
			{
				MessageBox.Show(this, string.Format("Search text \"{0}\" could not be found.", options.SearchText), "Text not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		public int MarkedAddress
		{
			get => mMarkedAddress;
			set
			{
				if (mMarkedAddress != value)
				{
					SetAddressMarkIfVisible(false);

					mMarkedAddress = value;

					SetAddressMarkIfVisible(true);
				}
			}
		}

		void SetAddressMarkIfVisible(bool mark)
		{
			int firstVisibleIndex = mWordEditorList.FirstVisibleIndex;

			if (mMarkedAddress >= firstVisibleIndex && mMarkedAddress < firstVisibleIndex + mWordEditorList.EditorCount)
			{
				((MemoryWordEditor)mWordEditorList[mMarkedAddress - firstVisibleIndex]).Marked = mark;
			}
		}

		public IMemory Memory
		{
			get => mMemory;
			set
			{
				if (mMemory == null && value != null)
				{
					mMemory = value;
					InitializeComponent();
					mProfilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Execution] = mMemory.MaxProfilingExecutionCount;
					mProfilingMaxCounts[(int)GuiSettings.ProfilingInfoType.Tick] = mMemory.MaxProfilingTickCount;
				}
			}
		}

		public bool ReadOnly
		{
			get => mReadOnly;
			set
			{
				if (mReadOnly != value)
				{
					mReadOnly = value;
					if (mWordEditorList != null)
					{
						mWordEditorList.ReadOnly = mReadOnly;
					}
				}
			}
		}

		public SymbolCollection Symbols
		{
			get => mSymbols;
			set
			{
				mSymbols = value;

				if (mWordEditorList != null)
				{
					foreach (MemoryWordEditor editor in mWordEditorList)
					{
						editor.Symbols = mSymbols;
					}
				}
			}
		}

		void MUpButton_Click(object sender, EventArgs args)
		{
			if (mPrevDataIndex.HasValue)
			{
				FirstVisibleAddress = mPrevDataIndex.Value;
			}
		}

		void MDownButton_Click(object sender, EventArgs args)
		{
			if (mNextDataIndex.HasValue)
			{
				FirstVisibleAddress = mNextDataIndex.Value;
			}
		}

		void MExportButton_Click(object sender, EventArgs args)
		{
			var exportDialog = new MemoryExportDialog
			{
				MinMemoryIndex = mMemory.MinWordIndex,
				MaxMemoryIndex = mMemory.MaxWordIndex,
				FromAddress = mWordEditorList.FirstVisibleIndex,
				ToAddress = mWordEditorList.FirstVisibleIndex + mWordEditorList.VisibleEditorCount - 1,
				ProgramCounter = mMarkedAddress
			};

			if (exportDialog.ShowDialog(this) != DialogResult.OK)
			{
				return;
			}

			if (mSaveExportFileDialog == null)
			{
				mSaveExportFileDialog = new SaveFileDialog
				{
					DefaultExt = "mixdeck",
					Filter = "MixEmul card deck files|*.mixdeck|All files|*.*",
					Title = "Specify export file name"
				};
			}

			if (mSaveExportFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				IFullWord[] memoryWords = new IFullWord[exportDialog.ToAddress - exportDialog.FromAddress + 1];

				int fromAddressOffset = exportDialog.FromAddress;
				for (int index = 0; index < memoryWords.Length; index++)
				{
					memoryWords[index] = mMemory[fromAddressOffset + index];
				}

				try
				{
					CardDeckExporter.ExportFullWords(mSaveExportFileDialog.FileName, memoryWords, fromAddressOffset, exportDialog.ProgramCounter);
					MessageBox.Show(this, "Memory successfully exported.", "Export successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Error while exporting memory: " + ex.Message, "Error while exporting", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		enum NavigationDirection
		{
			None,
			Up,
			Down
		}
	}
}
