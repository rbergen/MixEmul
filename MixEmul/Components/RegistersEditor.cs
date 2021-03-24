using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixLib.Type;

namespace MixGui.Components
{
	public class RegistersEditor : UserControl
	{
		private readonly Button _compareButton;
		private readonly Label _compareLabel;
		private readonly List<WordValueEditor> _editors;
		private readonly CheckBox _overflowBox;
		private readonly Label _overflowLabel;
		private readonly WordValueEditor _rI1Editor;
		private readonly Label _rI1Label;
		private readonly WordValueEditor _rI2Editor;
		private readonly Label _rI2Label;
		private readonly WordValueEditor _rI3Editor;
		private readonly Label _rI3Label;
		private readonly WordValueEditor _rI4Editor;
		private readonly Label _rI4Label;
		private readonly WordValueEditor _r5Editor;
		private readonly Label _rI5Label;
		private readonly WordValueEditor _rI6Editor;
		private readonly Label _rI6Label;
		private readonly WordValueEditor _rAEditor;
		private readonly Label _rALabel;
		private bool _readOnly;
		private MixLib.Registers _registers;
		private readonly WordValueEditor _rJEditor;
		private readonly Label _rJLabel;
		private readonly WordValueEditor _rXEditor;
		private readonly Label _rXLabel;

		public RegistersEditor(MixLib.Registers registers = null)
		{
			_registers = registers ?? new MixLib.Registers();
			_readOnly = false;

			_rALabel = new Label();
			_rXLabel = new Label();
			_rI1Label = new Label();
			_rI2Label = new Label();
			_rI3Label = new Label();
			_rI4Label = new Label();
			_rI5Label = new Label();
			_rI6Label = new Label();
			_rJLabel = new Label();
			_compareLabel = new Label();
			_overflowLabel = new Label();

			_editors = new List<WordValueEditor>
			{
					(_rAEditor = new WordValueEditor(_registers.RA)),
					(_rXEditor = new WordValueEditor(_registers.RX)),
					(_rI1Editor = new WordValueEditor(_registers.RI1)),
					(_rI2Editor = new WordValueEditor(_registers.RI2)),
					(_rI3Editor = new WordValueEditor(_registers.RI3)),
					(_rI4Editor = new WordValueEditor(_registers.RI4)),
					(_r5Editor = new WordValueEditor(_registers.RI5)),
					(_rI6Editor = new WordValueEditor(_registers.RI6)),
					(_rJEditor = new WordValueEditor(_registers.RJ, false))
			};

			_compareButton = new Button();
			_overflowBox = new CheckBox();

			SuspendLayout();

			_rALabel.Location = new Point(0, 0);
			_rALabel.Name = "mrALabel";
			_rALabel.Size = new Size(24, 21);
			_rALabel.TabIndex = 0;
			_rALabel.Text = "rA:";
			_rALabel.TextAlign = ContentAlignment.MiddleLeft;

			_rXLabel.Location = new Point(0, 24);
			_rXLabel.Name = "mrXLabel";
			_rXLabel.Size = new Size(24, 21);
			_rXLabel.TabIndex = 2;
			_rXLabel.Text = "rX:";
			_rXLabel.TextAlign = ContentAlignment.MiddleLeft;

			_rI1Label.Location = new Point(0, 48);
			_rI1Label.Name = "mr1Label";
			_rI1Label.Size = new Size(24, 21);
			_rI1Label.TabIndex = 4;
			_rI1Label.Text = "rI1:";
			_rI1Label.TextAlign = ContentAlignment.MiddleLeft;

			_rI2Label.Location = new Point(0, 72);
			_rI2Label.Name = "mr2Label";
			_rI2Label.Size = new Size(24, 21);
			_rI2Label.TabIndex = 6;
			_rI2Label.Text = "rI2:";
			_rI2Label.TextAlign = ContentAlignment.MiddleLeft;

			_rI3Label.Location = new Point(0, 96);
			_rI3Label.Name = "mr3Label";
			_rI3Label.Size = new Size(24, 21);
			_rI3Label.TabIndex = 8;
			_rI3Label.Text = "rI3:";
			_rI3Label.TextAlign = ContentAlignment.MiddleLeft;

			_rI4Label.Location = new Point(0, 120);
			_rI4Label.Name = "mr4Label";
			_rI4Label.Size = new Size(24, 21);
			_rI4Label.TabIndex = 10;
			_rI4Label.Text = "rI4:";
			_rI4Label.TextAlign = ContentAlignment.MiddleLeft;

			_rI5Label.Location = new Point(0, 144);
			_rI5Label.Name = "mr5Label";
			_rI5Label.Size = new Size(24, 21);
			_rI5Label.TabIndex = 12;
			_rI5Label.Text = "rI5:";
			_rI5Label.TextAlign = ContentAlignment.MiddleLeft;

			_rI6Label.Location = new Point(0, 168);
			_rI6Label.Name = "mr6Label";
			_rI6Label.Size = new Size(24, 21);
			_rI6Label.TabIndex = 14;
			_rI6Label.Text = "rI6:";
			_rI6Label.TextAlign = ContentAlignment.MiddleLeft;

			_rJLabel.Location = new Point(0, 192);
			_rJLabel.Name = "mrJLabel";
			_rJLabel.Size = new Size(24, 21);
			_rJLabel.TabIndex = 16;
			_rJLabel.Text = "rJ:";
			_rJLabel.TextAlign = ContentAlignment.MiddleLeft;

			_rAEditor.Location = new Point(24, 0);
			_rAEditor.Name = "mrAEditor";
			_rAEditor.TabIndex = 1;
			_rAEditor.TextBoxWidth = 80;
			_rAEditor.NavigationKeyDown += This_KeyDown;

			_rXEditor.Location = new Point(24, 24);
			_rXEditor.Name = "mrXEditor";
			_rXEditor.TabIndex = 3;
			_rXEditor.TextBoxWidth = 80;
			_rXEditor.NavigationKeyDown += This_KeyDown;

			_rI1Editor.Location = new Point(24, 48);
			_rI1Editor.Name = "mr1Editor";
			_rI1Editor.TabIndex = 5;
			_rI1Editor.TextBoxWidth = 40;
			_rI1Editor.NavigationKeyDown += This_KeyDown;

			_rI2Editor.Location = new Point(24, 72);
			_rI2Editor.Name = "mr2Editor";
			_rI2Editor.TabIndex = 7;
			_rI2Editor.TextBoxWidth = 40;
			_rI2Editor.NavigationKeyDown += This_KeyDown;

			_rI3Editor.Location = new Point(24, 96);
			_rI3Editor.Name = "mr3Editor";
			_rI3Editor.TabIndex = 9;
			_rI3Editor.TextBoxWidth = 40;
			_rI3Editor.NavigationKeyDown += This_KeyDown;

			_rI4Editor.Location = new Point(24, 120);
			_rI4Editor.Name = "mr4Editor";
			_rI4Editor.TabIndex = 11;
			_rI4Editor.TextBoxWidth = 40;
			_rI4Editor.NavigationKeyDown += This_KeyDown;

			_r5Editor.Location = new Point(24, 144);
			_r5Editor.Name = "mr5Editor";
			_r5Editor.TabIndex = 13;
			_r5Editor.TextBoxWidth = 40;
			_r5Editor.NavigationKeyDown += This_KeyDown;

			_rI6Editor.Location = new Point(24, 168);
			_rI6Editor.Name = "mr6Editor";
			_rI6Editor.TabIndex = 15;
			_rI6Editor.TextBoxWidth = 40;
			_rI6Editor.NavigationKeyDown += This_KeyDown;

			_rJEditor.Location = new Point(41, 192);
			_rJEditor.Name = "mrJEditor";
			_rJEditor.ReadOnly = false;
			_rJEditor.TabIndex = 17;
			_rJEditor.TextBoxWidth = 40;
			_rJEditor.NavigationKeyDown += This_KeyDown;

			_overflowLabel.Name = "mOverflowLabel";
			_overflowLabel.Size = new Size(58, 21);
			_overflowLabel.TabIndex = 18;
			_overflowLabel.Text = "Overflow:";
			_overflowLabel.TextAlign = ContentAlignment.MiddleLeft;

			_overflowBox.Name = "mOverflowBox";
			_overflowBox.Size = new Size(16, 21);
			_overflowBox.Location = new Point((_rAEditor.Left + _rAEditor.Width) - _overflowBox.Width, _rI6Editor.Top);
			_overflowBox.TabIndex = 19;
			_overflowBox.FlatStyle = FlatStyle.Flat;
			_overflowBox.CheckedChanged += OverflowBox_CheckedChanged;

			_compareLabel.Name = "mCompareLabel";
			_compareLabel.Size = new Size(58, 21);
			_compareLabel.TabIndex = 20;
			_compareLabel.Text = "Compare:";
			_compareLabel.TextAlign = ContentAlignment.MiddleLeft;

			_compareButton.FlatStyle = FlatStyle.Flat;
			_compareButton.Name = "mCompareButton";
			_compareButton.Size = new Size(18, 21);
			_compareButton.Location = new Point((_rAEditor.Left + _rAEditor.Width) - _compareButton.Width, _rJEditor.Top);
			_compareButton.TabIndex = 21;
			_compareButton.Text = "" + _registers.CompareIndicator.ToChar();
			_compareButton.Click += CompareButton_Click;

			_overflowLabel.Location = new Point(Math.Min(_compareButton.Left, _overflowBox.Left) - Math.Max(_compareLabel.Width, _overflowLabel.Width), _overflowBox.Top);

			_compareLabel.Location = new Point(_overflowLabel.Left, _compareButton.Top);

			Controls.Add(_compareButton);
			Controls.Add(_compareLabel);
			Controls.Add(_overflowBox);
			Controls.Add(_overflowLabel);
			Controls.Add(_rJEditor);
			Controls.Add(_rI6Editor);
			Controls.Add(_r5Editor);
			Controls.Add(_rI4Editor);
			Controls.Add(_rI3Editor);
			Controls.Add(_rI2Editor);
			Controls.Add(_rI1Editor);
			Controls.Add(_rXEditor);
			Controls.Add(_rAEditor);
			Controls.Add(_rJLabel);
			Controls.Add(_rI6Label);
			Controls.Add(_rI5Label);
			Controls.Add(_rI4Label);
			Controls.Add(_rI3Label);
			Controls.Add(_rI2Label);
			Controls.Add(_rI1Label);
			Controls.Add(_rXLabel);
			Controls.Add(_rALabel);
			Name = "RegistersEditor";
			Size = new Size(_rAEditor.Left + _rAEditor.Width, _rJEditor.Left + _rJEditor.Height);

			ResumeLayout(false);
		}

		private void OverflowBox_CheckedChanged(object sender, EventArgs e) 
			=> _registers.OverflowIndicator = _overflowBox.Checked;

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			FieldTypes? editorField = null;
			int? index = null;

			if (e is FieldKeyEventArgs args)
			{
				editorField = args.Field;
				index = args.Index;
			}

			var editorIndex = _editors.IndexOf((WordValueEditor)sender);

			if (e.KeyCode == Keys.Down && editorIndex < _editors.Count - 1)
				_editors[editorIndex + 1].Focus(editorField, index);

			else if (e.KeyCode == Keys.Up && editorIndex > 0)
				_editors[editorIndex - 1].Focus(editorField, index);
		}

		private void CompareButton_Click(object sender, EventArgs e)
		{
			var compValue = _registers.CompareIndicator.Next();
			_registers.CompareIndicator = compValue;
			_compareButton.Text = "" + compValue.ToChar();
		}

		public new void Update()
		{
			foreach (WordValueEditor editor in _editors)
				editor.Update();

			_compareButton.Text = "" + _registers.CompareIndicator.ToChar();
			_overflowBox.Checked = _registers.OverflowIndicator;

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (WordValueEditor editor in _editors)
				editor.UpdateLayout();
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly != value)
				{
					_readOnly = value;

					foreach (WordValueEditor editor in _editors)
						editor.ReadOnly = _readOnly;

					_compareButton.Enabled = !_readOnly;
					_overflowBox.Enabled = !_readOnly;
				}
			}
		}

		public MixLib.Registers Registers
		{
			get => _registers;
			set
			{
				if (value != null && _registers != value)
				{
					_registers = value;
					_rAEditor.WordValue = _registers.RA;
					_rXEditor.WordValue = _registers.RX;
					_rI1Editor.WordValue = _registers.RI1;
					_rI2Editor.WordValue = _registers.RI2;
					_rI3Editor.WordValue = _registers.RI3;
					_rI4Editor.WordValue = _registers.RI4;
					_r5Editor.WordValue = _registers.RI5;
					_rI6Editor.WordValue = _registers.RI6;
					_rJEditor.WordValue = _registers.RJ;
				}
			}
		}
	}
}
