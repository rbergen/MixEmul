using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixGui.Utils;
using MixLib.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class MemoryWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private Label mAddressLabel;
		private Panel mAddressPanel;
		private Panel mInstructionPanel;
		private CheckBox mBreakPointBox;
		private Label mColonLabel;
		private Label mColonLabel2;
		private InstructionInstanceTextBox mInstructionTextBox;
		private FullWordEditor mFullWordEditor;
		private bool mMarked;
		private IMemoryFullWord mMemoryWord;
		private bool mReadOnly;
		private Label mEqualsLabel;
		private Label mProfileLabel;
		private GetMaxProfilingCountCallback mGetMaxProfilingCount;

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
			initializeComponent();

			MemoryWord = memoryWord;

			updateProfilingLayout();
		}

		public GetMaxProfilingCountCallback GetMaxProfilingCount
		{
			get
			{
				return mGetMaxProfilingCount;
			}
			set
			{
				if (mGetMaxProfilingCount != value)
				{
					mGetMaxProfilingCount = value;
					updateProfilingCount();
				}
			}
		}

		public bool Focus(FieldTypes? field, int? index)
		{
			return field == FieldTypes.Instruction ? mInstructionTextBox.FocusWithIndex(index) : mFullWordEditor.Focus(field, index);
		}

		private void initializeComponent()
		{
			mAddressPanel.SuspendLayout();
			base.SuspendLayout();

			mBreakPointBox.Location = new Point(2, 2);
			mBreakPointBox.Name = "mBreakPointBox";
			mBreakPointBox.Size = new Size(16, 17);
			mBreakPointBox.TabIndex = 0;
			mBreakPointBox.CheckedChanged += mBreakPointBox_CheckedChanged;

			mAddressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mAddressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mAddressLabel.Location = new Point(mBreakPointBox.Right, mBreakPointBox.Top - 1);
			mAddressLabel.Name = "mAddressLabel";
			mAddressLabel.Size = new Size(45, mBreakPointBox.Height);
			mAddressLabel.TabIndex = 1;
			mAddressLabel.Text = "0000";
			mAddressLabel.TextAlign = ContentAlignment.MiddleCenter;
			mAddressLabel.DoubleClick += mAddressLabel_DoubleClick;

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
			mFullWordEditor.ValueChanged += mFullWordEditor_ValueChanged;
			mFullWordEditor.NavigationKeyDown += keyDown;

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
			mInstructionTextBox.KeyDown += keyDown;
			mInstructionTextBox.ValueChanged += mInstructionTextBox_ValueChanged;
			mInstructionTextBox.AddressSelected += mInstructionTextBox_AddressSelected;
			mInstructionTextBox.MouseWheel += mInstructionTextBox_MouseWheel;

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

			base.Controls.Add(mAddressPanel);
			base.Controls.Add(mColonLabel);
			base.Controls.Add(mFullWordEditor);
			base.Controls.Add(mEqualsLabel);
			base.Controls.Add(mInstructionPanel);
			base.Controls.Add(mColonLabel2);
			base.Controls.Add(mProfileLabel);
			base.Name = "MemoryWordEditor";
			base.Size = new Size(mProfileLabel.Right + 2, mInstructionPanel.Height + 3);
			base.KeyDown += keyDown;

			mAddressPanel.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private Color getBlendedColor(double fraction)
		{
			return fraction < .5 ? interpolate(Color.Green, Color.Yellow, fraction * 2) : interpolate(Color.Yellow, Color.Red, fraction * 2 - 1);
		}

		private Color interpolate(Color color1, Color color2, double fraction)
		{
			int r = roundForArgb(interpolate(color1.R, color2.R, fraction));
			int g = roundForArgb(interpolate(color1.G, color2.G, fraction));
			int b = roundForArgb(interpolate(color1.B, color2.B, fraction));
			return Color.FromArgb(r, g, b);
		}

		private int roundForArgb(double d)
		{
			return d < 0 ? 0 : (d > 255 ? 255 : (int)d);
		}

		private double interpolate(double d1, double d2, double fraction)
		{
			return d1 * (1 - fraction) + d2 * fraction;
		}

		private void updateProfilingCount()
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
				Color backColor = getBlendedColor((double)count / max);
				mProfileLabel.BackColor = backColor;
				mProfileLabel.ForeColor = 255 - (int)(backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) < 105 ? Color.Black : Color.White;
			}
		}

		void updateProfilingLayout()
		{
			if (ExecutionSettings.ProfilingEnabled)
			{
				updateProfilingCount();
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

		void mInstructionTextBox_MouseWheel(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		public int MemoryMinIndex
		{
			get
			{
				return mInstructionTextBox.MemoryMinIndex;
			}
			set
			{
				mInstructionTextBox.MemoryMinIndex = value;
			}
		}

		public int MemoryMaxIndex
		{
			get
			{
				return mInstructionTextBox.MemoryMaxIndex;
			}
			set
			{
				mInstructionTextBox.MemoryMaxIndex = value;
			}
		}

		void mInstructionTextBox_AddressSelected(object sender, AddressSelectedEventArgs args)
		{
			OnAddressSelected(args);
		}

		private void OnAddressSelected(AddressSelectedEventArgs args)
		{
			if (AddressSelected != null)
			{
				AddressSelected(this, args);
			}
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			FieldTypes editorField = FieldTypes.Instruction;
			int? index = mInstructionTextBox.SelectionStart + mInstructionTextBox.SelectionLength;

			if (e is FieldKeyEventArgs)
			{
				editorField = ((FieldKeyEventArgs)e).Field;
				index = ((FieldKeyEventArgs)e).Index;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					if (NavigationKeyDown != null)
					{
						NavigationKeyDown(this, new FieldKeyEventArgs(e.KeyData, editorField, index));
					}

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
						if (NavigationKeyDown != null)
						{
							NavigationKeyDown(this, e);
						}
					}
					else if (sender == mInstructionTextBox && mInstructionTextBox.SelectionStart + mInstructionTextBox.SelectionLength == 0)
					{
						mFullWordEditor.Focus(FieldTypes.Chars, null);
						e.Handled = true;
					}

					break;
			}
		}

		private void mAddressLabel_DoubleClick(object sender, EventArgs e)
		{
			OnAddressDoubleClick(e);
		}

		private void mBreakPointBox_CheckedChanged(object sender, EventArgs e)
		{
			OnBreakpointCheckedChanged(e);
		}

		private void mInstructionTextBox_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			mFullWordEditor.Update();
			OnValueChanged(args);
		}

		private void mFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			mInstructionTextBox.Update();
			OnValueChanged(args);
		}

		protected virtual void OnAddressDoubleClick(EventArgs e)
		{
			if (AddressDoubleClick != null)
			{
				AddressDoubleClick(this, e);
			}
		}

		protected virtual void OnBreakpointCheckedChanged(EventArgs e)
		{
			if (BreakpointCheckedChanged != null)
			{
				BreakpointCheckedChanged(this, e);
			}
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
			}
		}

		public new void Update()
		{
			mFullWordEditor.Update();
			mInstructionTextBox.Update();
			if (mProfileLabel.Enabled)
			{
				updateProfilingCount();
			}
			base.Update();
		}

		public void UpdateLayout()
		{
			mAddressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mAddressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			if (mMarked)
			{
				mAddressPanel.BackColor = GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground);
			}

			mFullWordEditor.UpdateLayout();
			mInstructionTextBox.UpdateLayout();

			updateProfilingLayout();
		}

		public bool BreakPointChecked
		{
			get
			{
				return mBreakPointBox.Checked;
			}
			set
			{
				mBreakPointBox.Checked = value;
			}
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get
			{
				return mInstructionTextBox.IndexedAddressCalculatorCallback;
			}
			set
			{
				mInstructionTextBox.IndexedAddressCalculatorCallback = value;
			}
		}

		public bool Marked
		{
			get
			{
				return mMarked;
			}
			set
			{
				if (mMarked != value)
				{
					mMarked = value;
					mAddressPanel.BackColor = mMarked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : SystemColors.Control;
				}
			}
		}

		public IMemoryFullWord MemoryWord
		{
			get
			{
				return mMemoryWord;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "MemoryWord may not be set to null");
				}

				mMemoryWord = value;
				string text = mMemoryWord.Index.ToString("D4");
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
					updateProfilingCount();
				}
			}
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
					mFullWordEditor.ReadOnly = mReadOnly;
					mInstructionTextBox.ReadOnly = mReadOnly;
				}
			}
		}

		public SymbolCollection Symbols
		{
			get
			{
				return mInstructionTextBox.Symbols;
			}
			set
			{
				mInstructionTextBox.Symbols = value;
			}
		}

		public ToolTip ToolTip
		{
			set
			{
				mInstructionTextBox.ToolTip = value;
			}
		}

		public IWord WordValue
		{
			get
			{
				return MemoryWord;
			}
			set
			{
				if (!(value is IMemoryFullWord))
				{
					throw new ArgumentException("Value must be an IMemoryFullWord");
				}

				MemoryWord = (IMemoryFullWord)value;
			}
		}

		public Control EditorControl
		{
			get
			{
				return this;
			}
		}

		public FieldTypes? FocusedField
		{
			get
			{
				return mInstructionTextBox.Focused ? FieldTypes.Instruction : mFullWordEditor.FocusedField;
			}
		}

		public int? CaretIndex
		{
			get
			{
				return FocusedField == FieldTypes.Instruction ? mInstructionTextBox.CaretIndex : mFullWordEditor.CaretIndex;
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
