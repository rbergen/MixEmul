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
		private readonly Label addressLabel;
		private readonly Panel addressPanel;
		private readonly Panel instructionPanel;
		private readonly CheckBox breakPointBox;
		private readonly Label colonLabel;
		private readonly Label colonLabel2;
		private readonly Label colonLabel3;
		private readonly InstructionInstanceTextBox instructionTextBox;
		private readonly FullWordEditor fullWordEditor;
		private bool marked;
		private IMemoryFullWord memoryWord;
		private bool readOnly;
		private readonly Label equalsLabel;
		private readonly Label profileLabel;
		private readonly Label sourceLineLabel;
		private GetMaxProfilingCountCallback getMaxProfilingCount;
		private ToolTip toolTip;

		public event EventHandler AddressDoubleClick;
		public event EventHandler BreakpointCheckedChanged;
		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public delegate long GetMaxProfilingCountCallback(GuiSettings.ProfilingInfoType infoType);

		public MemoryWordEditor(IMemoryFullWord memoryWord = null)
		{
			this.readOnly = false;
			this.marked = false;

			if (memoryWord == null)
				memoryWord = new MemoryFullWord(int.MinValue);

			this.instructionTextBox = new InstructionInstanceTextBox(memoryWord);
			this.fullWordEditor = new FullWordEditor(memoryWord);
			this.addressPanel = new Panel();
			this.instructionPanel = new Panel();
			this.breakPointBox = new CheckBox();
			this.addressLabel = new Label();
			this.colonLabel = new Label();
			this.equalsLabel = new Label();
			this.colonLabel = new Label();
			this.colonLabel3 = new Label();
			this.sourceLineLabel = new Label();
			this.colonLabel2 = new Label();
			this.profileLabel = new Label();
			InitializeComponent();

			MemoryWord = memoryWord;

			UpdateShowSourceInlineLayout(true);
			UpdateProfilingLayout(true);
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> this.instructionTextBox.Focused ? FieldTypes.Instruction : this.fullWordEditor.FocusedField;

		public int? CaretIndex
			=> FocusedField == FieldTypes.Instruction ? this.instructionTextBox.CaretIndex : this.fullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> field == FieldTypes.Instruction ? this.instructionTextBox.FocusWithIndex(index) : this.fullWordEditor.Focus(field, index);

		private static Color GetBlendedColor(double fraction)
			=> fraction < .5
			? Interpolate(Color.Green, Color.Yellow, fraction * 2)
			: Interpolate(Color.Yellow, Color.Red, (fraction * 2) - 1);

		private static int RoundForArgb(double d)
			=> d < 0 ? 0 : (d > 255 ? 255 : (int)d);

		private static double Interpolate(double d1, double d2, double fraction)
			=> (d1 * (1 - fraction)) + (d2 * fraction);

		private void OnAddressSelected(AddressSelectedEventArgs args)
			=> AddressSelected?.Invoke(this, args);

		protected virtual void OnAddressDoubleClick(EventArgs e)
			=> AddressDoubleClick?.Invoke(this, e);

		protected virtual void OnBreakpointCheckedChanged(EventArgs e)
			=> BreakpointCheckedChanged?.Invoke(this, e);

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
			=> ValueChanged?.Invoke(this, args);

		private void InstructionTextBox_MouseWheel(object sender, MouseEventArgs e)
			=> OnMouseWheel(e);

		private void InstructionTextBox_AddressSelected(object sender, AddressSelectedEventArgs args)
			=> OnAddressSelected(args);

		private void AddressLabel_DoubleClick(object sender, EventArgs e)
			=> OnAddressDoubleClick(e);

		private void BreakPointBox_CheckedChanged(object sender, EventArgs e)
			=> OnBreakpointCheckedChanged(e);

		public GetMaxProfilingCountCallback GetMaxProfilingCount
		{
			get => this.getMaxProfilingCount;
			set
			{
				if (this.getMaxProfilingCount != value)
				{
					this.getMaxProfilingCount = value;
					UpdateProfilingCount();
				}
			}
		}

		private void InitializeComponent()
		{
			this.addressPanel.SuspendLayout();
			SuspendLayout();

			this.breakPointBox.Location = new Point(2, 2);
			this.breakPointBox.Name = "mBreakPointBox";
			this.breakPointBox.Size = new Size(16, 17);
			this.breakPointBox.TabIndex = 0;
			this.breakPointBox.FlatStyle = FlatStyle.Flat;
			this.breakPointBox.CheckedChanged += BreakPointBox_CheckedChanged;

			this.addressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.addressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.addressLabel.Location = new Point(this.breakPointBox.Right, this.breakPointBox.Top - 1);
			this.addressLabel.Name = "mAddressLabel";
			this.addressLabel.Size = new Size(45, this.breakPointBox.Height);
			this.addressLabel.TabIndex = 1;
			this.addressLabel.Text = "0000";
			this.addressLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.addressLabel.DoubleClick += AddressLabel_DoubleClick;

			this.addressPanel.Controls.Add(this.breakPointBox);
			this.addressPanel.Controls.Add(this.addressLabel);
			this.addressPanel.Size = new Size(this.addressLabel.Right, 21);
			this.addressPanel.Location = new Point(0, 1);
			this.addressLabel.TabIndex = 0;

			this.colonLabel.Location = new Point(this.addressPanel.Right, this.addressPanel.Top + 3);
			this.colonLabel.Name = "mFirstEqualsLabel";
			this.colonLabel.Size = new Size(10, this.addressLabel.Height - 3);
			this.colonLabel.TabIndex = 1;
			this.colonLabel.Text = ":";

			this.fullWordEditor.Location = new Point(this.colonLabel.Right, this.addressPanel.Top);
			this.fullWordEditor.Name = "mFullWordEditor";
			this.fullWordEditor.TabIndex = 2;
			this.fullWordEditor.ValueChanged += FullWordEditor_ValueChanged;
			this.fullWordEditor.NavigationKeyDown += This_KeyDown;

			this.equalsLabel.Location = new Point(this.fullWordEditor.Right, this.fullWordEditor.Top + 2);
			this.equalsLabel.Name = "mEqualsLabel";
			this.equalsLabel.Size = new Size(10, 19);
			this.equalsLabel.TabIndex = 3;
			this.equalsLabel.Text = "=";

			this.instructionTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
			this.instructionTextBox.Dock = DockStyle.Fill;
			this.instructionTextBox.BorderStyle = BorderStyle.None;
			this.instructionTextBox.Location = new Point(0, 0);
			this.instructionTextBox.Multiline = false;
			this.instructionTextBox.Name = "mInstructionTextBox";
			this.instructionTextBox.Size = new Size(39, 21);
			this.instructionTextBox.TabIndex = 0;
			this.instructionTextBox.KeyDown += This_KeyDown;
			this.instructionTextBox.ValueChanged += InstructionTextBox_ValueChanged;
			this.instructionTextBox.AddressSelected += InstructionTextBox_AddressSelected;
			this.instructionTextBox.MouseWheel += InstructionTextBox_MouseWheel;

			this.instructionPanel.Controls.Add(this.instructionTextBox);
			this.instructionPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.instructionPanel.Location = new Point(this.equalsLabel.Right, this.fullWordEditor.Top);
			this.instructionPanel.Size = new Size(this.instructionTextBox.Width, 21);
			this.instructionPanel.TabIndex = 4;
			this.instructionPanel.BorderStyle = BorderStyle.FixedSingle;

			this.colonLabel3.Location = new Point(this.instructionPanel.Right, this.addressPanel.Top + 3);
			this.colonLabel3.Anchor = AnchorStyles.Top;
			this.colonLabel3.Name = "mColonLabel3";
			this.colonLabel3.Size = new Size(10, this.addressLabel.Height - 3);
			this.colonLabel3.TabIndex = 5;
			this.colonLabel3.Text = "<";
			this.colonLabel3.TextAlign = ContentAlignment.MiddleCenter;

			this.sourceLineLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			this.sourceLineLabel.Location = new Point(this.colonLabel3.Right, this.instructionPanel.Top);
			this.sourceLineLabel.Size = new Size(79, this.instructionPanel.Height);
			this.sourceLineLabel.Name = "mSourceLineLabel";
			this.sourceLineLabel.TabIndex = 6;
			this.sourceLineLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.sourceLineLabel.Text = string.Empty;
			this.sourceLineLabel.TextAlign = ContentAlignment.MiddleLeft;
			this.sourceLineLabel.BorderStyle = BorderStyle.FixedSingle;
			this.sourceLineLabel.AutoEllipsis = true;

			this.colonLabel2.Location = new Point(this.sourceLineLabel.Right, this.addressPanel.Top + 3);
			this.colonLabel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.colonLabel2.Name = "mColonLabel2";
			this.colonLabel2.Size = new Size(10, this.addressLabel.Height - 3);
			this.colonLabel2.TabIndex = 7;
			this.colonLabel2.Text = "x";
			this.colonLabel2.TextAlign = ContentAlignment.MiddleCenter;

			this.profileLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			this.profileLabel.Location = new Point(this.colonLabel2.Right, this.instructionPanel.Top);
			this.profileLabel.Size = new Size(64, this.instructionPanel.Height);
			this.profileLabel.Name = "mProfileLabel";
			this.profileLabel.TabIndex = 8;
			this.profileLabel.Text = "0";
			this.profileLabel.TextAlign = ContentAlignment.MiddleRight;
			this.profileLabel.BorderStyle = BorderStyle.FixedSingle;

			Controls.Add(this.addressPanel);
			Controls.Add(this.colonLabel);
			Controls.Add(this.fullWordEditor);
			Controls.Add(this.equalsLabel);
			Controls.Add(this.instructionPanel);
			Controls.Add(this.colonLabel3);
			Controls.Add(this.sourceLineLabel);
			Controls.Add(this.colonLabel2);
			Controls.Add(this.profileLabel);
			Name = "MemoryWordEditor";
			Size = new Size(this.profileLabel.Right + 2, this.instructionPanel.Height + 3);
			KeyDown += This_KeyDown;
			Layout += This_Layout;

			this.addressPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void This_Layout(object sender, LayoutEventArgs e)
		{

			int sourceWidth = Width - (this.addressPanel.Width + this.colonLabel.Width + this.fullWordEditor.Width + this.equalsLabel.Width + 2);
			if (ExecutionSettings.ProfilingEnabled)
				sourceWidth -= this.colonLabel2.Width + this.profileLabel.Width;

			if (!GuiSettings.ShowSourceInline)
			{
				this.instructionPanel.Width = sourceWidth;
				return;
			}

			this.instructionPanel.Width = (sourceWidth - this.colonLabel3.Width) / 3;
			this.colonLabel3.Left = this.instructionPanel.Right;
			this.sourceLineLabel.Left = this.colonLabel3.Right;
			this.sourceLineLabel.Width = sourceWidth - (this.colonLabel3.Width + this.instructionPanel.Width);
			SetSourceLineLabelToolTip();
		}

		private void SetSourceLineLabelToolTip()
			=> this.toolTip?.SetToolTip(this.sourceLineLabel, this.sourceLineLabel.PreferredWidth > this.sourceLineLabel.Width && !string.IsNullOrEmpty(this.memoryWord.SourceLine) ? this.memoryWord.SourceLine : null);

		private static Color Interpolate(Color color1, Color color2, double fraction)
		{
			var r = RoundForArgb(Interpolate(color1.R, color2.R, fraction));
			var g = RoundForArgb(Interpolate(color1.G, color2.G, fraction));
			var b = RoundForArgb(Interpolate(color1.B, color2.B, fraction));
			return Color.FromArgb(r, g, b);
		}

		private void UpdateProfilingCount()
		{
			long count = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick ? this.memoryWord.ProfilingTickCount : this.memoryWord.ProfilingExecutionCount;
			long max;
			this.profileLabel.Text = count.ToString();

			if (count == 0 || !GuiSettings.ColorProfilingCounts || GetMaxProfilingCount == null || (max = GetMaxProfilingCount(GuiSettings.ShowProfilingInfo)) == 0)
			{
				this.profileLabel.BackColor = this.colonLabel2.BackColor;
				this.profileLabel.ForeColor = this.colonLabel2.ForeColor;
			}
			else
			{
				var backColor = GetBlendedColor((double)count / max);
				this.profileLabel.BackColor = backColor;
				this.profileLabel.ForeColor = 255 - (int)(backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) < 105 ? Color.Black : Color.White;
			}
		}

		private void UpdateShowSourceInlineLayout(bool force = false)
		{
			if (!force && this.sourceLineLabel.Visible == GuiSettings.ShowSourceInline)
				return;

			if (!GuiSettings.ShowSourceInline)
			{
				this.colonLabel3.Visible = false;
				this.sourceLineLabel.Visible = false;
				this.instructionTextBox.ShowSourceLineToolTip = true;
			}
			else
			{
				this.colonLabel3.Visible = true;
				this.sourceLineLabel.Visible = true;
				this.instructionTextBox.ShowSourceLineToolTip = false;
			}
		}


		private void UpdateProfilingLayout(bool force = false)
		{
			if (ExecutionSettings.ProfilingEnabled)
				UpdateProfilingCount();

			if (!force && this.profileLabel.Enabled == ExecutionSettings.ProfilingEnabled)
				return;

			if (!ExecutionSettings.ProfilingEnabled)
			{
				this.colonLabel2.Visible = false;
				this.profileLabel.Visible = false;
				this.profileLabel.Enabled = false;
			}
			else
			{
				this.colonLabel2.Visible = true;
				this.profileLabel.Visible = true;
				this.profileLabel.Enabled = true;
			}
		}

		public int MemoryMinIndex
		{
			get => this.instructionTextBox.MemoryMinIndex;
			set => this.instructionTextBox.MemoryMinIndex = value;
		}

		public int MemoryMaxIndex
		{
			get => this.instructionTextBox.MemoryMaxIndex;
			set => this.instructionTextBox.MemoryMaxIndex = value;
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes editorField = FieldTypes.Instruction;
			int? index = this.instructionTextBox.SelectionStart + this.instructionTextBox.SelectionLength;

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
					if (sender == this.fullWordEditor)
					{
						this.instructionTextBox.Focus();
					}
					else if (sender == this.instructionTextBox && this.instructionTextBox.SelectionStart + this.instructionTextBox.SelectionLength == this.instructionTextBox.TextLength && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
						e.Handled = true;
					}

					break;

				case Keys.Left:
					if (sender == this.fullWordEditor)
					{
						NavigationKeyDown?.Invoke(this, e);
					}
					else if (sender == this.instructionTextBox && this.instructionTextBox.SelectionStart + this.instructionTextBox.SelectionLength == 0)
					{
						this.fullWordEditor.Focus(FieldTypes.Chars, null);
						e.Handled = true;
					}

					break;
			}
		}

		private void InstructionTextBox_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			this.fullWordEditor.Update();
			OnValueChanged(args);
		}

		private void FullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			this.instructionTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			this.fullWordEditor.Update();
			this.instructionTextBox.Update();

			if (this.profileLabel.Enabled)
				UpdateProfilingCount();

			base.Update();
		}

		public void UpdateLayout()
		{
			this.addressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.addressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			this.addressPanel.BackColor = this.marked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;

			this.fullWordEditor.UpdateLayout();
			this.instructionTextBox.UpdateLayout();

			UpdateShowSourceInlineLayout();
			UpdateProfilingLayout();
		}

		public bool BreakPointChecked
		{
			get => this.breakPointBox.Checked;
			set => this.breakPointBox.Checked = value;
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => this.instructionTextBox.IndexedAddressCalculatorCallback;
			set => this.instructionTextBox.IndexedAddressCalculatorCallback = value;
		}

		public bool Marked
		{
			get => this.marked;
			set
			{
				if (this.marked != value)
				{
					this.marked = value;
					this.addressPanel.BackColor = this.marked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;
				}
			}
		}

		public IMemoryFullWord MemoryWord
		{
			get => this.memoryWord;
			set
			{
				this.memoryWord = value ?? throw new ArgumentNullException(nameof(value), "MemoryWord may not be set to null");
				var text = this.memoryWord.Index.ToString("D4");

				if (this.memoryWord.Index == 0)
					text = " " + text;

				else if (this.memoryWord.Index > 0)
					text = "+" + text;

				this.addressLabel.Text = text;
				this.instructionTextBox.MemoryAddress = this.memoryWord.Index;
				this.fullWordEditor.WordValue = this.memoryWord;
				this.instructionTextBox.InstructionWord = this.memoryWord;
				this.sourceLineLabel.Text = this.memoryWord.SourceLine ?? string.Empty;

				SetSourceLineLabelToolTip();

				if (this.profileLabel.Enabled)
					UpdateProfilingCount();
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
				this.fullWordEditor.ReadOnly = this.readOnly;
				this.instructionTextBox.ReadOnly = this.readOnly;
			}
		}

		public SymbolCollection Symbols
		{
			get => this.instructionTextBox.Symbols;
			set => this.instructionTextBox.Symbols = value;
		}

		public ToolTip ToolTip
		{
			set
			{
				this.toolTip = value;
				this.instructionTextBox.ToolTip = this.toolTip;
			}
		}

		public IWord WordValue
		{
			get => MemoryWord;
			set
			{
				if (value is not IMemoryFullWord)
					throw new ArgumentException("Value must be an IMemoryFullWord");

				MemoryWord = (IMemoryFullWord)value;
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Instruction)
				this.instructionTextBox.Select(start, length);

			else
				this.fullWordEditor.Select(start, length);
		}
	}
}
