using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Settings;
using MixLib.Type;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class MemoryWordEditor : UserControl, IWordEditor, INavigableControl
	{
		readonly Label mAddressLabel;
		readonly Panel mAddressPanel;
		readonly Panel mInstructionPanel;
		readonly CheckBox mBreakPointBox;
		readonly Label mColonLabel;
		readonly Label mColonLabel2;
		readonly InstructionInstanceTextBox mInstructionTextBox;
		readonly FullWordEditor mFullWordEditor;
		bool mMarked;
		IMemoryFullWord mMemoryWord;
		bool mReadOnly;
		readonly Label mEqualsLabel;
		readonly Label mProfileLabel;
		GetMaxProfilingCountCallback mGetMaxProfilingCount;

		public event EventHandler AddressDoubleClick;
		public event EventHandler BreakpointCheckedChanged;
		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public delegate long GetMaxProfilingCountCallback(GuiSettings.ProfilingInfoType infoType);

		public MemoryWordEditor()
			: this(null)
		{
		}

		public MemoryWordEditor(IMemoryFullWord memoryWord)
		{
			mReadOnly = false;
			mMarked = false;
			if (memoryWord == null)
			{
				memoryWord = new MemoryFullWord(int.MinValue);
			}

			mInstructionTextBox = new InstructionInstanceTextBox(mMemoryWord);
			mFullWordEditor = new FullWordEditor(mMemoryWord);
			mAddressPanel = new Panel();
			mInstructionPanel = new Panel();
			mBreakPointBox = new CheckBox();
			mAddressLabel = new Label();
			mColonLabel = new Label();
			mEqualsLabel = new Label();
			mColonLabel = new Label();
			mColonLabel2 = new Label();
			mProfileLabel = new Label();
			InitializeComponent();

			MemoryWord = memoryWord;

			UpdateProfilingLayout();
		}

		public Control EditorControl => this;

		public FieldTypes? FocusedField => mInstructionTextBox.Focused ? FieldTypes.Instruction : mFullWordEditor.FocusedField;

		public int? CaretIndex => FocusedField == FieldTypes.Instruction ? mInstructionTextBox.CaretIndex : mFullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
		{
			return field == FieldTypes.Instruction ? mInstructionTextBox.FocusWithIndex(index) : mFullWordEditor.Focus(field, index);
		}

		static Color GetBlendedColor(double fraction)
		{
			return fraction < .5 ? Interpolate(Color.Green, Color.Yellow, fraction * 2) : Interpolate(Color.Yellow, Color.Red, fraction * 2 - 1);
		}

		private static int RoundForArgb(double d)
		{
			return d < 0 ? 0 : (d > 255 ? 255 : (int)d);
		}

		static double Interpolate(double d1, double d2, double fraction)
		{
			return d1 * (1 - fraction) + d2 * fraction;
		}

		void OnAddressSelected(AddressSelectedEventArgs args)
		{
			AddressSelected?.Invoke(this, args);
		}

		protected virtual void OnAddressDoubleClick(EventArgs e)
		{
			AddressDoubleClick?.Invoke(this, e);
		}

		protected virtual void OnBreakpointCheckedChanged(EventArgs e)
		{
			BreakpointCheckedChanged?.Invoke(this, e);
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			ValueChanged?.Invoke(this, args);
		}

		void MInstructionTextBox_MouseWheel(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		void MInstructionTextBox_AddressSelected(object sender, AddressSelectedEventArgs args)
		{
			OnAddressSelected(args);
		}

		void MAddressLabel_DoubleClick(object sender, EventArgs e)
		{
			OnAddressDoubleClick(e);
		}

		void MBreakPointBox_CheckedChanged(object sender, EventArgs e)
		{
			OnBreakpointCheckedChanged(e);
		}

		public GetMaxProfilingCountCallback GetMaxProfilingCount
		{
			get => mGetMaxProfilingCount;
			set
			{
				if (mGetMaxProfilingCount != value)
				{
					mGetMaxProfilingCount = value;
					UpdateProfilingCount();
				}
			}
		}

		void InitializeComponent()
		{
			mAddressPanel.SuspendLayout();
			SuspendLayout();

			mBreakPointBox.Location = new Point(2, 2);
			mBreakPointBox.Name = "mBreakPointBox";
			mBreakPointBox.Size = new Size(16, 17);
			mBreakPointBox.TabIndex = 0;
			mBreakPointBox.FlatStyle = FlatStyle.Flat;
			mBreakPointBox.CheckedChanged += MBreakPointBox_CheckedChanged;

			mAddressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mAddressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mAddressLabel.Location = new Point(mBreakPointBox.Right, mBreakPointBox.Top - 1);
			mAddressLabel.Name = "mAddressLabel";
			mAddressLabel.Size = new Size(45, mBreakPointBox.Height);
			mAddressLabel.TabIndex = 1;
			mAddressLabel.Text = "0000";
			mAddressLabel.TextAlign = ContentAlignment.MiddleCenter;
			mAddressLabel.DoubleClick += MAddressLabel_DoubleClick;

			mAddressPanel.Controls.Add(mBreakPointBox);
			mAddressPanel.Controls.Add(mAddressLabel);
			mAddressPanel.Size = new Size(mAddressLabel.Right, 21);
			mAddressPanel.Location = new Point(0, 1);
			mAddressLabel.TabIndex = 0;

			mColonLabel.Location = new Point(mAddressPanel.Right, mAddressPanel.Top + 3);
			mColonLabel.Name = "mFirstEqualsLabel";
			mColonLabel.Size = new Size(10, mAddressLabel.Height - 3);
			mColonLabel.TabIndex = 1;
			mColonLabel.Text = ":";

			mFullWordEditor.Location = new Point(mColonLabel.Right, mAddressPanel.Top);
			mFullWordEditor.Name = "mFullWordEditor";
			mFullWordEditor.TabIndex = 2;
			mFullWordEditor.ValueChanged += MFullWordEditor_ValueChanged;
			mFullWordEditor.NavigationKeyDown += This_KeyDown;

			mEqualsLabel.Location = new Point(mFullWordEditor.Right, mFullWordEditor.Top + 2);
			mEqualsLabel.Name = "mEqualsLabel";
			mEqualsLabel.Size = new Size(10, 19);
			mEqualsLabel.TabIndex = 3;
			mEqualsLabel.Text = "=";

			mInstructionTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
			mInstructionTextBox.Dock = DockStyle.Fill;
			mInstructionTextBox.BorderStyle = BorderStyle.None;
			mInstructionTextBox.Location = new Point(0, 0);
			mInstructionTextBox.Multiline = false;
			mInstructionTextBox.Name = "mInstructionTextBox";
			mInstructionTextBox.Size = new Size(128, 21);
			mInstructionTextBox.TabIndex = 0;
			mInstructionTextBox.KeyDown += This_KeyDown;
			mInstructionTextBox.ValueChanged += MInstructionTextBox_ValueChanged;
			mInstructionTextBox.AddressSelected += MInstructionTextBox_AddressSelected;
			mInstructionTextBox.MouseWheel += MInstructionTextBox_MouseWheel;

			mInstructionPanel.Controls.Add(mInstructionTextBox);
			mInstructionPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			mInstructionPanel.Location = new Point(mEqualsLabel.Right, mFullWordEditor.Top);
			mInstructionPanel.Size = new Size(mInstructionTextBox.Width, 21);
			mInstructionPanel.TabIndex = 4;
			mInstructionPanel.BorderStyle = BorderStyle.FixedSingle;

			mColonLabel2.Location = new Point(mInstructionPanel.Right, mAddressPanel.Top + 3);
			mColonLabel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			mColonLabel2.Name = "mColonLabel2";
			mColonLabel2.Size = new Size(10, mAddressLabel.Height - 3);
			mColonLabel2.TabIndex = 5;
			mColonLabel2.Text = "x";
			mColonLabel2.TextAlign = ContentAlignment.MiddleCenter;

			mProfileLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			mProfileLabel.Location = new Point(mColonLabel2.Right, mInstructionPanel.Top);
			mProfileLabel.Size = new Size(64, mInstructionPanel.Height);
			mProfileLabel.Name = "mProfileLabel";
			mProfileLabel.TabIndex = 6;
			mProfileLabel.Text = "0";
			mProfileLabel.TextAlign = ContentAlignment.MiddleRight;
			mProfileLabel.BorderStyle = BorderStyle.FixedSingle;

			Controls.Add(mAddressPanel);
			Controls.Add(mColonLabel);
			Controls.Add(mFullWordEditor);
			Controls.Add(mEqualsLabel);
			Controls.Add(mInstructionPanel);
			Controls.Add(mColonLabel2);
			Controls.Add(mProfileLabel);
			Name = "MemoryWordEditor";
			Size = new Size(mProfileLabel.Right + 2, mInstructionPanel.Height + 3);
			KeyDown += This_KeyDown;

			mAddressPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		static Color Interpolate(Color color1, Color color2, double fraction)
		{
			var r = RoundForArgb(Interpolate(color1.R, color2.R, fraction));
			var g = RoundForArgb(Interpolate(color1.G, color2.G, fraction));
			var b = RoundForArgb(Interpolate(color1.B, color2.B, fraction));
			return Color.FromArgb(r, g, b);
		}

		void UpdateProfilingCount()
		{
			long count = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick ? mMemoryWord.ProfilingTickCount : mMemoryWord.ProfilingExecutionCount;
			long max;
			mProfileLabel.Text = count.ToString();
			if (count == 0 || !GuiSettings.ColorProfilingCounts || GetMaxProfilingCount == null || (max = GetMaxProfilingCount(GuiSettings.ShowProfilingInfo)) == 0)
			{
				mProfileLabel.BackColor = mColonLabel2.BackColor;
				mProfileLabel.ForeColor = mColonLabel2.ForeColor;
			}
			else
			{
				var backColor = GetBlendedColor((double)count / max);
				mProfileLabel.BackColor = backColor;
				mProfileLabel.ForeColor = 255 - (int)(backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) < 105 ? Color.Black : Color.White;
			}
		}

		void UpdateProfilingLayout()
		{
			if (ExecutionSettings.ProfilingEnabled)
			{
				UpdateProfilingCount();
			}

			if (mProfileLabel.Enabled == ExecutionSettings.ProfilingEnabled)
			{
				return;
			}

			if (!ExecutionSettings.ProfilingEnabled)
			{
				mInstructionPanel.Width += mColonLabel2.Width + mProfileLabel.Width;
				mColonLabel2.Visible = false;
				mProfileLabel.Visible = false;
				mProfileLabel.Enabled = false;
			}
			else
			{
				mColonLabel2.Visible = true;
				mProfileLabel.Visible = true;
				mProfileLabel.Enabled = true;
				mInstructionPanel.Width -= mColonLabel2.Width + mProfileLabel.Width;
			}
		}

		public int MemoryMinIndex
		{
			get => mInstructionTextBox.MemoryMinIndex;
			set => mInstructionTextBox.MemoryMinIndex = value;
		}

		public int MemoryMaxIndex
		{
			get => mInstructionTextBox.MemoryMaxIndex;
			set => mInstructionTextBox.MemoryMaxIndex = value;
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			FieldTypes editorField = FieldTypes.Instruction;
			int? index = mInstructionTextBox.SelectionStart + mInstructionTextBox.SelectionLength;

			if (e is FieldKeyEventArgs args)
			{
				editorField = args.Field;
				index = args.Index;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, new FieldKeyEventArgs(e.KeyData, editorField, index));

					e.Handled = true;
					break;

				case Keys.Right:
					if (sender == mFullWordEditor)
					{
						mInstructionTextBox.Focus();
					}
					else if (sender == mInstructionTextBox && mInstructionTextBox.SelectionStart + mInstructionTextBox.SelectionLength == mInstructionTextBox.TextLength && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
						e.Handled = true;
					}

					break;

				case Keys.Left:
					if (sender == mFullWordEditor)
					{
						NavigationKeyDown?.Invoke(this, e);
					}
					else if (sender == mInstructionTextBox && mInstructionTextBox.SelectionStart + mInstructionTextBox.SelectionLength == 0)
					{
						mFullWordEditor.Focus(FieldTypes.Chars, null);
						e.Handled = true;
					}

					break;
			}
		}

		void MInstructionTextBox_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			mFullWordEditor.Update();
			OnValueChanged(args);
		}

		void MFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			mInstructionTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			mFullWordEditor.Update();
			mInstructionTextBox.Update();
			if (mProfileLabel.Enabled)
			{
				UpdateProfilingCount();
			}

			base.Update();
		}

		public void UpdateLayout()
		{
			mAddressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mAddressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mAddressPanel.BackColor = mMarked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;

			mFullWordEditor.UpdateLayout();
			mInstructionTextBox.UpdateLayout();

			UpdateProfilingLayout();
		}

		public bool BreakPointChecked
		{
			get => mBreakPointBox.Checked;
			set => mBreakPointBox.Checked = value;
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => mInstructionTextBox.IndexedAddressCalculatorCallback;
			set => mInstructionTextBox.IndexedAddressCalculatorCallback = value;
		}

		public bool Marked
		{
			get => mMarked;
			set
			{
				if (mMarked != value)
				{
					mMarked = value;
					mAddressPanel.BackColor = mMarked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;
				}
			}
		}

		public IMemoryFullWord MemoryWord
		{
			get => mMemoryWord;
			set
			{
				mMemoryWord = value ?? throw new ArgumentNullException(nameof(value), "MemoryWord may not be set to null");
				var text = mMemoryWord.Index.ToString("D4");
				if (mMemoryWord.Index == 0)
				{
					text = " " + text;
				}
				else if (mMemoryWord.Index > 0)
				{
					text = "+" + text;
				}
				mAddressLabel.Text = text;
				mInstructionTextBox.MemoryAddress = mMemoryWord.Index;
				mFullWordEditor.WordValue = mMemoryWord;
				mInstructionTextBox.InstructionWord = mMemoryWord;
				if (mProfileLabel.Enabled)
				{
					UpdateProfilingCount();
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
					mFullWordEditor.ReadOnly = mReadOnly;
					mInstructionTextBox.ReadOnly = mReadOnly;
				}
			}
		}

		public SymbolCollection Symbols
		{
			get => mInstructionTextBox.Symbols;
			set => mInstructionTextBox.Symbols = value;
		}

		public ToolTip ToolTip
		{
			set => mInstructionTextBox.ToolTip = value;
		}

		public IWord WordValue
		{
			get => MemoryWord;
			set
			{
				if (!(value is IMemoryFullWord))
				{
					throw new ArgumentException("Value must be an IMemoryFullWord");
				}

				MemoryWord = (IMemoryFullWord)value;
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Instruction)
			{
				mInstructionTextBox.Select(start, length);
			}
			else
			{
				mFullWordEditor.Select(start, length);
			}
		}
	}
}
