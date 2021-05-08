using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixLib.Type;

namespace MixGui.Components
{
	public class RegistersEditor : UserControl
	{
		private readonly Button compareButton;
		private readonly Label compareLabel;
		private readonly List<WordValueEditor> editors;
		private readonly CheckBox overflowBox;
		private readonly Label overflowLabel;
		private readonly WordValueEditor rI1Editor;
		private readonly Label rI1Label;
		private readonly WordValueEditor rI2Editor;
		private readonly Label rI2Label;
		private readonly WordValueEditor rI3Editor;
		private readonly Label rI3Label;
		private readonly WordValueEditor rI4Editor;
		private readonly Label rI4Label;
		private readonly WordValueEditor r5Editor;
		private readonly Label rI5Label;
		private readonly WordValueEditor rI6Editor;
		private readonly Label rI6Label;
		private readonly WordValueEditor rAEditor;
		private readonly Label rALabel;
		private bool readOnly;
		private MixLib.Registers registers;
		private readonly WordValueEditor rJEditor;
		private readonly Label rJLabel;
		private readonly WordValueEditor rXEditor;
		private readonly Label rXLabel;

		public RegistersEditor(MixLib.Registers registers = null)
		{
			this.registers = registers ?? new MixLib.Registers();
			this.readOnly = false;

			this.rALabel = new Label();
			this.rXLabel = new Label();
			this.rI1Label = new Label();
			this.rI2Label = new Label();
			this.rI3Label = new Label();
			this.rI4Label = new Label();
			this.rI5Label = new Label();
			this.rI6Label = new Label();
			this.rJLabel = new Label();
			this.compareLabel = new Label();
			this.overflowLabel = new Label();

			this.editors = new List<WordValueEditor>
			{
					(this.rAEditor = new WordValueEditor(this.registers.RA)),
					(this.rXEditor = new WordValueEditor(this.registers.RX)),
					(this.rI1Editor = new WordValueEditor(this.registers.RI1)),
					(this.rI2Editor = new WordValueEditor(this.registers.RI2)),
					(this.rI3Editor = new WordValueEditor(this.registers.RI3)),
					(this.rI4Editor = new WordValueEditor(this.registers.RI4)),
					(this.r5Editor = new WordValueEditor(this.registers.RI5)),
					(this.rI6Editor = new WordValueEditor(this.registers.RI6)),
					(this.rJEditor = new WordValueEditor(this.registers.RJ, false))
			};

			this.compareButton = new Button();
			this.overflowBox = new CheckBox();

			SuspendLayout();

			this.rALabel.Location = new Point(0, 0);
			this.rALabel.Name = "mrALabel";
			this.rALabel.Size = new Size(24, 21);
			this.rALabel.TabIndex = 0;
			this.rALabel.Text = "rA:";
			this.rALabel.TextAlign = ContentAlignment.MiddleLeft;

			this.rXLabel.Location = new Point(0, 24);
			this.rXLabel.Name = "mrXLabel";
			this.rXLabel.Size = new Size(24, 21);
			this.rXLabel.TabIndex = 2;
			this.rXLabel.Text = "rX:";
			this.rXLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.rI1Label.Location = new Point(0, 48);
			this.rI1Label.Name = "mr1Label";
			this.rI1Label.Size = new Size(24, 21);
			this.rI1Label.TabIndex = 4;
			this.rI1Label.Text = "rI1:";
			this.rI1Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rI2Label.Location = new Point(0, 72);
			this.rI2Label.Name = "mr2Label";
			this.rI2Label.Size = new Size(24, 21);
			this.rI2Label.TabIndex = 6;
			this.rI2Label.Text = "rI2:";
			this.rI2Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rI3Label.Location = new Point(0, 96);
			this.rI3Label.Name = "mr3Label";
			this.rI3Label.Size = new Size(24, 21);
			this.rI3Label.TabIndex = 8;
			this.rI3Label.Text = "rI3:";
			this.rI3Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rI4Label.Location = new Point(0, 120);
			this.rI4Label.Name = "mr4Label";
			this.rI4Label.Size = new Size(24, 21);
			this.rI4Label.TabIndex = 10;
			this.rI4Label.Text = "rI4:";
			this.rI4Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rI5Label.Location = new Point(0, 144);
			this.rI5Label.Name = "mr5Label";
			this.rI5Label.Size = new Size(24, 21);
			this.rI5Label.TabIndex = 12;
			this.rI5Label.Text = "rI5:";
			this.rI5Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rI6Label.Location = new Point(0, 168);
			this.rI6Label.Name = "mr6Label";
			this.rI6Label.Size = new Size(24, 21);
			this.rI6Label.TabIndex = 14;
			this.rI6Label.Text = "rI6:";
			this.rI6Label.TextAlign = ContentAlignment.MiddleLeft;

			this.rJLabel.Location = new Point(0, 192);
			this.rJLabel.Name = "mrJLabel";
			this.rJLabel.Size = new Size(24, 21);
			this.rJLabel.TabIndex = 16;
			this.rJLabel.Text = "rJ:";
			this.rJLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.rAEditor.Location = new Point(24, 0);
			this.rAEditor.Name = "mrAEditor";
			this.rAEditor.TabIndex = 1;
			this.rAEditor.TextBoxWidth = 80;
			this.rAEditor.NavigationKeyDown += This_KeyDown;

			this.rXEditor.Location = new Point(24, 24);
			this.rXEditor.Name = "mrXEditor";
			this.rXEditor.TabIndex = 3;
			this.rXEditor.TextBoxWidth = 80;
			this.rXEditor.NavigationKeyDown += This_KeyDown;

			this.rI1Editor.Location = new Point(24, 48);
			this.rI1Editor.Name = "mr1Editor";
			this.rI1Editor.TabIndex = 5;
			this.rI1Editor.TextBoxWidth = 40;
			this.rI1Editor.NavigationKeyDown += This_KeyDown;

			this.rI2Editor.Location = new Point(24, 72);
			this.rI2Editor.Name = "mr2Editor";
			this.rI2Editor.TabIndex = 7;
			this.rI2Editor.TextBoxWidth = 40;
			this.rI2Editor.NavigationKeyDown += This_KeyDown;

			this.rI3Editor.Location = new Point(24, 96);
			this.rI3Editor.Name = "mr3Editor";
			this.rI3Editor.TabIndex = 9;
			this.rI3Editor.TextBoxWidth = 40;
			this.rI3Editor.NavigationKeyDown += This_KeyDown;

			this.rI4Editor.Location = new Point(24, 120);
			this.rI4Editor.Name = "mr4Editor";
			this.rI4Editor.TabIndex = 11;
			this.rI4Editor.TextBoxWidth = 40;
			this.rI4Editor.NavigationKeyDown += This_KeyDown;

			this.r5Editor.Location = new Point(24, 144);
			this.r5Editor.Name = "mr5Editor";
			this.r5Editor.TabIndex = 13;
			this.r5Editor.TextBoxWidth = 40;
			this.r5Editor.NavigationKeyDown += This_KeyDown;

			this.rI6Editor.Location = new Point(24, 168);
			this.rI6Editor.Name = "mr6Editor";
			this.rI6Editor.TabIndex = 15;
			this.rI6Editor.TextBoxWidth = 40;
			this.rI6Editor.NavigationKeyDown += This_KeyDown;

			this.rJEditor.Location = new Point(41, 192);
			this.rJEditor.Name = "mrJEditor";
			this.rJEditor.ReadOnly = false;
			this.rJEditor.TabIndex = 17;
			this.rJEditor.TextBoxWidth = 40;
			this.rJEditor.NavigationKeyDown += This_KeyDown;

			this.overflowLabel.Name = "mOverflowLabel";
			this.overflowLabel.Size = new Size(58, 21);
			this.overflowLabel.TabIndex = 18;
			this.overflowLabel.Text = "Overflow:";
			this.overflowLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.overflowBox.Name = "mOverflowBox";
			this.overflowBox.Size = new Size(16, 21);
			this.overflowBox.Location = new Point((this.rAEditor.Left + this.rAEditor.Width) - this.overflowBox.Width, this.rI6Editor.Top);
			this.overflowBox.TabIndex = 19;
			this.overflowBox.FlatStyle = FlatStyle.Flat;
			this.overflowBox.CheckedChanged += OverflowBox_CheckedChanged;

			this.compareLabel.Name = "mCompareLabel";
			this.compareLabel.Size = new Size(58, 21);
			this.compareLabel.TabIndex = 20;
			this.compareLabel.Text = "Compare:";
			this.compareLabel.TextAlign = ContentAlignment.MiddleLeft;

			this.compareButton.FlatStyle = FlatStyle.Flat;
			this.compareButton.Name = "mCompareButton";
			this.compareButton.Size = new Size(18, 21);
			this.compareButton.Location = new Point((this.rAEditor.Left + this.rAEditor.Width) - this.compareButton.Width, this.rJEditor.Top);
			this.compareButton.TabIndex = 21;
			this.compareButton.Text = "" + this.registers.CompareIndicator.ToChar();
			this.compareButton.Click += CompareButton_Click;

			this.overflowLabel.Location = new Point(Math.Min(this.compareButton.Left, this.overflowBox.Left) - Math.Max(this.compareLabel.Width, this.overflowLabel.Width), this.overflowBox.Top);

			this.compareLabel.Location = new Point(this.overflowLabel.Left, this.compareButton.Top);

			Controls.Add(this.compareButton);
			Controls.Add(this.compareLabel);
			Controls.Add(this.overflowBox);
			Controls.Add(this.overflowLabel);
			Controls.Add(this.rJEditor);
			Controls.Add(this.rI6Editor);
			Controls.Add(this.r5Editor);
			Controls.Add(this.rI4Editor);
			Controls.Add(this.rI3Editor);
			Controls.Add(this.rI2Editor);
			Controls.Add(this.rI1Editor);
			Controls.Add(this.rXEditor);
			Controls.Add(this.rAEditor);
			Controls.Add(this.rJLabel);
			Controls.Add(this.rI6Label);
			Controls.Add(this.rI5Label);
			Controls.Add(this.rI4Label);
			Controls.Add(this.rI3Label);
			Controls.Add(this.rI2Label);
			Controls.Add(this.rI1Label);
			Controls.Add(this.rXLabel);
			Controls.Add(this.rALabel);
			Name = "RegistersEditor";
			Size = new Size(this.rAEditor.Left + this.rAEditor.Width, this.rJEditor.Left + this.rJEditor.Height);

			ResumeLayout(false);
		}

		private void OverflowBox_CheckedChanged(object sender, EventArgs e) 
			=> this.registers.OverflowIndicator = this.overflowBox.Checked;

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			FieldTypes? editorField = null;
			int? index = null;

			if (e is FieldKeyEventArgs args)
			{
				editorField = args.Field;
				index = args.Index;
			}

			var editorIndex = this.editors.IndexOf((WordValueEditor)sender);

			if (e.KeyCode == Keys.Down && editorIndex < this.editors.Count - 1)
				this.editors[editorIndex + 1].Focus(editorField, index);

			else if (e.KeyCode == Keys.Up && editorIndex > 0)
				this.editors[editorIndex - 1].Focus(editorField, index);
		}

		private void CompareButton_Click(object sender, EventArgs e)
		{
			var compValue = this.registers.CompareIndicator.Next();
			this.registers.CompareIndicator = compValue;
			this.compareButton.Text = "" + compValue.ToChar();
		}

		public new void Update()
		{
			foreach (WordValueEditor editor in this.editors)
				editor.Update();

			this.compareButton.Text = "" + this.registers.CompareIndicator.ToChar();
			this.overflowBox.Checked = this.registers.OverflowIndicator;

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (WordValueEditor editor in this.editors)
				editor.UpdateLayout();
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (this.readOnly != value)
				{
					this.readOnly = value;

					foreach (WordValueEditor editor in this.editors)
						editor.ReadOnly = this.readOnly;

					this.compareButton.Enabled = !this.readOnly;
					this.overflowBox.Enabled = !this.readOnly;
				}
			}
		}

		public MixLib.Registers Registers
		{
			get => this.registers;
			set
			{
				if (value != null && this.registers != value)
				{
					this.registers = value;
					this.rAEditor.WordValue = this.registers.RA;
					this.rXEditor.WordValue = this.registers.RX;
					this.rI1Editor.WordValue = this.registers.RI1;
					this.rI2Editor.WordValue = this.registers.RI2;
					this.rI3Editor.WordValue = this.registers.RI3;
					this.rI4Editor.WordValue = this.registers.RI4;
					this.r5Editor.WordValue = this.registers.RI5;
					this.rI6Editor.WordValue = this.registers.RI6;
					this.rJEditor.WordValue = this.registers.RJ;
				}
			}
		}
	}
}
