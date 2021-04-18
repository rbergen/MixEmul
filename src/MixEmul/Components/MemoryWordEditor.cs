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
		private readonly Label _addressLabel;
		private readonly Panel _addressPanel;
		private readonly Panel _instructionPanel;
		private readonly CheckBox _breakPointBox;
		private readonly Label _colonLabel;
		private readonly Label _colonLabel2;
		private readonly Label _colonLabel3;
		private readonly InstructionInstanceTextBox _instructionTextBox;
		private readonly FullWordEditor _fullWordEditor;
		private bool _marked;
		private IMemoryFullWord _memoryWord;
		private bool _readOnly;
		private readonly Label _equalsLabel;
		private readonly Label _profileLabel;
		private readonly Label _sourceLineLabel;
		private GetMaxProfilingCountCallback _getMaxProfilingCount;
		private ToolTip _toolTip;

		public event EventHandler AddressDoubleClick;
		public event EventHandler BreakpointCheckedChanged;
		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;
		public event AddressSelectedHandler AddressSelected;

		public delegate long GetMaxProfilingCountCallback(GuiSettings.ProfilingInfoType infoType);

		public MemoryWordEditor(IMemoryFullWord memoryWord = null)
		{
			_readOnly = false;
			_marked = false;

			if (memoryWord == null)
				memoryWord = new MemoryFullWord(int.MinValue);

			_instructionTextBox = new InstructionInstanceTextBox(memoryWord);
			_fullWordEditor = new FullWordEditor(memoryWord);
			_addressPanel = new Panel();
			_instructionPanel = new Panel();
			_breakPointBox = new CheckBox();
			_addressLabel = new Label();
			_colonLabel = new Label();
			_equalsLabel = new Label();
			_colonLabel = new Label();
			_colonLabel3 = new Label();
			_sourceLineLabel = new Label();
			_colonLabel2 = new Label();
			_profileLabel = new Label();
			InitializeComponent();

			MemoryWord = memoryWord;

			UpdateShowSourceInlineLayout(true);
			UpdateProfilingLayout(true);
		}

		public Control EditorControl
			=> this;

		public FieldTypes? FocusedField
			=> _instructionTextBox.Focused ? FieldTypes.Instruction : _fullWordEditor.FocusedField;

		public int? CaretIndex
			=> FocusedField == FieldTypes.Instruction ? _instructionTextBox.CaretIndex : _fullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
			=> field == FieldTypes.Instruction ? _instructionTextBox.FocusWithIndex(index) : _fullWordEditor.Focus(field, index);

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
			get => _getMaxProfilingCount;
			set
			{
				if (_getMaxProfilingCount != value)
				{
					_getMaxProfilingCount = value;
					UpdateProfilingCount();
				}
			}
		}

		private void InitializeComponent()
		{
			_addressPanel.SuspendLayout();
			SuspendLayout();

			_breakPointBox.Location = new Point(2, 2);
			_breakPointBox.Name = "mBreakPointBox";
			_breakPointBox.Size = new Size(16, 17);
			_breakPointBox.TabIndex = 0;
			_breakPointBox.FlatStyle = FlatStyle.Flat;
			_breakPointBox.CheckedChanged += BreakPointBox_CheckedChanged;

			_addressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_addressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_addressLabel.Location = new Point(_breakPointBox.Right, _breakPointBox.Top - 1);
			_addressLabel.Name = "mAddressLabel";
			_addressLabel.Size = new Size(45, _breakPointBox.Height);
			_addressLabel.TabIndex = 1;
			_addressLabel.Text = "0000";
			_addressLabel.TextAlign = ContentAlignment.MiddleCenter;
			_addressLabel.DoubleClick += AddressLabel_DoubleClick;

			_addressPanel.Controls.Add(_breakPointBox);
			_addressPanel.Controls.Add(_addressLabel);
			_addressPanel.Size = new Size(_addressLabel.Right, 21);
			_addressPanel.Location = new Point(0, 1);
			_addressLabel.TabIndex = 0;

			_colonLabel.Location = new Point(_addressPanel.Right, _addressPanel.Top + 3);
			_colonLabel.Name = "mFirstEqualsLabel";
			_colonLabel.Size = new Size(10, _addressLabel.Height - 3);
			_colonLabel.TabIndex = 1;
			_colonLabel.Text = ":";

			_fullWordEditor.Location = new Point(_colonLabel.Right, _addressPanel.Top);
			_fullWordEditor.Name = "mFullWordEditor";
			_fullWordEditor.TabIndex = 2;
			_fullWordEditor.ValueChanged += FullWordEditor_ValueChanged;
			_fullWordEditor.NavigationKeyDown += This_KeyDown;

			_equalsLabel.Location = new Point(_fullWordEditor.Right, _fullWordEditor.Top + 2);
			_equalsLabel.Name = "mEqualsLabel";
			_equalsLabel.Size = new Size(10, 19);
			_equalsLabel.TabIndex = 3;
			_equalsLabel.Text = "=";

			_instructionTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
			_instructionTextBox.Dock = DockStyle.Fill;
			_instructionTextBox.BorderStyle = BorderStyle.None;
			_instructionTextBox.Location = new Point(0, 0);
			_instructionTextBox.Multiline = false;
			_instructionTextBox.Name = "mInstructionTextBox";
			_instructionTextBox.Size = new Size(39, 21);
			_instructionTextBox.TabIndex = 0;
			_instructionTextBox.KeyDown += This_KeyDown;
			_instructionTextBox.ValueChanged += InstructionTextBox_ValueChanged;
			_instructionTextBox.AddressSelected += InstructionTextBox_AddressSelected;
			_instructionTextBox.MouseWheel += InstructionTextBox_MouseWheel;

			_instructionPanel.Controls.Add(_instructionTextBox);
			_instructionPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			_instructionPanel.Location = new Point(_equalsLabel.Right, _fullWordEditor.Top);
			_instructionPanel.Size = new Size(_instructionTextBox.Width, 21);
			_instructionPanel.TabIndex = 4;
			_instructionPanel.BorderStyle = BorderStyle.FixedSingle;

			_colonLabel3.Location = new Point(_instructionPanel.Right, _addressPanel.Top + 3);
			_colonLabel3.Anchor = AnchorStyles.Top;
			_colonLabel3.Name = "mColonLabel3";
			_colonLabel3.Size = new Size(10, _addressLabel.Height - 3);
			_colonLabel3.TabIndex = 5;
			_colonLabel3.Text = "<";
			_colonLabel3.TextAlign = ContentAlignment.MiddleCenter;

			_sourceLineLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			_sourceLineLabel.Location = new Point(_colonLabel3.Right, _instructionPanel.Top);
			_sourceLineLabel.Size = new Size(79, _instructionPanel.Height);
			_sourceLineLabel.Name = "mSourceLineLabel";
			_sourceLineLabel.TabIndex = 6;
			_sourceLineLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_sourceLineLabel.Text = string.Empty;
			_sourceLineLabel.TextAlign = ContentAlignment.MiddleLeft;
			_sourceLineLabel.BorderStyle = BorderStyle.FixedSingle;
			_sourceLineLabel.AutoEllipsis = true;

			_colonLabel2.Location = new Point(_sourceLineLabel.Right, _addressPanel.Top + 3);
			_colonLabel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_colonLabel2.Name = "mColonLabel2";
			_colonLabel2.Size = new Size(10, _addressLabel.Height - 3);
			_colonLabel2.TabIndex = 7;
			_colonLabel2.Text = "x";
			_colonLabel2.TextAlign = ContentAlignment.MiddleCenter;

			_profileLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			_profileLabel.Location = new Point(_colonLabel2.Right, _instructionPanel.Top);
			_profileLabel.Size = new Size(64, _instructionPanel.Height);
			_profileLabel.Name = "mProfileLabel";
			_profileLabel.TabIndex = 8;
			_profileLabel.Text = "0";
			_profileLabel.TextAlign = ContentAlignment.MiddleRight;
			_profileLabel.BorderStyle = BorderStyle.FixedSingle;

			Controls.Add(_addressPanel);
			Controls.Add(_colonLabel);
			Controls.Add(_fullWordEditor);
			Controls.Add(_equalsLabel);
			Controls.Add(_instructionPanel);
			Controls.Add(_colonLabel3);
			Controls.Add(_sourceLineLabel);
			Controls.Add(_colonLabel2);
			Controls.Add(_profileLabel);
			Name = "MemoryWordEditor";
			Size = new Size(_profileLabel.Right + 2, _instructionPanel.Height + 3);
			KeyDown += This_KeyDown;
			Layout += This_Layout;

			_addressPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void This_Layout(object sender, LayoutEventArgs e)
		{

			int sourceWidth = Width - (_addressPanel.Width + _colonLabel.Width + _fullWordEditor.Width + _equalsLabel.Width + 2);
			if (ExecutionSettings.ProfilingEnabled)
				sourceWidth -= _colonLabel2.Width + _profileLabel.Width;

			if (!GuiSettings.ShowSourceInline)
			{
				_instructionPanel.Width = sourceWidth;
				return;
			}

			_instructionPanel.Width = (sourceWidth - _colonLabel3.Width) / 3;
			_colonLabel3.Left = _instructionPanel.Right;
			_sourceLineLabel.Left = _colonLabel3.Right;
			_sourceLineLabel.Width = sourceWidth - (_colonLabel3.Width + _instructionPanel.Width);
			SetSourceLineLabelToolTip();
		}

		private void SetSourceLineLabelToolTip()
			=> _toolTip?.SetToolTip(_sourceLineLabel, _sourceLineLabel.PreferredWidth > _sourceLineLabel.Width && !string.IsNullOrEmpty(_memoryWord.SourceLine) ? _memoryWord.SourceLine : null);

		private static Color Interpolate(Color color1, Color color2, double fraction)
		{
			var r = RoundForArgb(Interpolate(color1.R, color2.R, fraction));
			var g = RoundForArgb(Interpolate(color1.G, color2.G, fraction));
			var b = RoundForArgb(Interpolate(color1.B, color2.B, fraction));
			return Color.FromArgb(r, g, b);
		}

		private void UpdateProfilingCount()
		{
			long count = GuiSettings.ShowProfilingInfo == GuiSettings.ProfilingInfoType.Tick ? _memoryWord.ProfilingTickCount : _memoryWord.ProfilingExecutionCount;
			long max;
			_profileLabel.Text = count.ToString();

			if (count == 0 || !GuiSettings.ColorProfilingCounts || GetMaxProfilingCount == null || (max = GetMaxProfilingCount(GuiSettings.ShowProfilingInfo)) == 0)
			{
				_profileLabel.BackColor = _colonLabel2.BackColor;
				_profileLabel.ForeColor = _colonLabel2.ForeColor;
			}
			else
			{
				var backColor = GetBlendedColor((double)count / max);
				_profileLabel.BackColor = backColor;
				_profileLabel.ForeColor = 255 - (int)(backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) < 105 ? Color.Black : Color.White;
			}
		}

		private void UpdateShowSourceInlineLayout(bool force = false)
		{
			if (!force && _sourceLineLabel.Visible == GuiSettings.ShowSourceInline)
				return;

			if (!GuiSettings.ShowSourceInline)
			{
				_colonLabel3.Visible = false;
				_sourceLineLabel.Visible = false;
				_instructionTextBox.ShowSourceLineToolTip = true;
			}
			else
			{
				_colonLabel3.Visible = true;
				_sourceLineLabel.Visible = true;
				_instructionTextBox.ShowSourceLineToolTip = false;
			}
		}


		private void UpdateProfilingLayout(bool force = false)
		{
			if (ExecutionSettings.ProfilingEnabled)
				UpdateProfilingCount();

			if (!force && _profileLabel.Enabled == ExecutionSettings.ProfilingEnabled)
				return;

			if (!ExecutionSettings.ProfilingEnabled)
			{
				_colonLabel2.Visible = false;
				_profileLabel.Visible = false;
				_profileLabel.Enabled = false;
			}
			else
			{
				_colonLabel2.Visible = true;
				_profileLabel.Visible = true;
				_profileLabel.Enabled = true;
			}
		}

		public int MemoryMinIndex
		{
			get => _instructionTextBox.MemoryMinIndex;
			set => _instructionTextBox.MemoryMinIndex = value;
		}

		public int MemoryMaxIndex
		{
			get => _instructionTextBox.MemoryMaxIndex;
			set => _instructionTextBox.MemoryMaxIndex = value;
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes editorField = FieldTypes.Instruction;
			int? index = _instructionTextBox.SelectionStart + _instructionTextBox.SelectionLength;

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
					if (sender == _fullWordEditor)
					{
						_instructionTextBox.Focus();
					}
					else if (sender == _instructionTextBox && _instructionTextBox.SelectionStart + _instructionTextBox.SelectionLength == _instructionTextBox.TextLength && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
						e.Handled = true;
					}

					break;

				case Keys.Left:
					if (sender == _fullWordEditor)
					{
						NavigationKeyDown?.Invoke(this, e);
					}
					else if (sender == _instructionTextBox && _instructionTextBox.SelectionStart + _instructionTextBox.SelectionLength == 0)
					{
						_fullWordEditor.Focus(FieldTypes.Chars, null);
						e.Handled = true;
					}

					break;
			}
		}

		private void InstructionTextBox_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			_fullWordEditor.Update();
			OnValueChanged(args);
		}

		private void FullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			_instructionTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			_fullWordEditor.Update();
			_instructionTextBox.Update();

			if (_profileLabel.Enabled)
				UpdateProfilingCount();

			base.Update();
		}

		public void UpdateLayout()
		{
			_addressLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_addressLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			_addressPanel.BackColor = _marked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;

			_fullWordEditor.UpdateLayout();
			_instructionTextBox.UpdateLayout();

			UpdateShowSourceInlineLayout();
			UpdateProfilingLayout();
		}

		public bool BreakPointChecked
		{
			get => _breakPointBox.Checked;
			set => _breakPointBox.Checked = value;
		}

		public IndexedAddressCalculatorCallback IndexedAddressCalculatorCallback
		{
			get => _instructionTextBox.IndexedAddressCalculatorCallback;
			set => _instructionTextBox.IndexedAddressCalculatorCallback = value;
		}

		public bool Marked
		{
			get => _marked;
			set
			{
				if (_marked != value)
				{
					_marked = value;
					_addressPanel.BackColor = _marked ? GuiSettings.GetColor(GuiSettings.ProgramCounterAddressBackground) : Color.Transparent;
				}
			}
		}

		public IMemoryFullWord MemoryWord
		{
			get => _memoryWord;
			set
			{
				_memoryWord = value ?? throw new ArgumentNullException(nameof(value), "MemoryWord may not be set to null");
				var text = _memoryWord.Index.ToString("D4");

				if (_memoryWord.Index == 0)
					text = " " + text;

				else if (_memoryWord.Index > 0)
					text = "+" + text;

				_addressLabel.Text = text;
				_instructionTextBox.MemoryAddress = _memoryWord.Index;
				_fullWordEditor.WordValue = _memoryWord;
				_instructionTextBox.InstructionWord = _memoryWord;
				_sourceLineLabel.Text = _memoryWord.SourceLine ?? string.Empty;

				SetSourceLineLabelToolTip();

				if (_profileLabel.Enabled)
					UpdateProfilingCount();
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
				_fullWordEditor.ReadOnly = _readOnly;
				_instructionTextBox.ReadOnly = _readOnly;
			}
		}

		public SymbolCollection Symbols
		{
			get => _instructionTextBox.Symbols;
			set => _instructionTextBox.Symbols = value;
		}

		public ToolTip ToolTip
		{
			set
			{
				_toolTip = value;
				_instructionTextBox.ToolTip = _toolTip;
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
				_instructionTextBox.Select(start, length);

			else
				_fullWordEditor.Select(start, length);
		}
	}
}
