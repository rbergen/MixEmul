using MixLib.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class RegistersEditor : UserControl
	{
		Button mCompareButton;
		Label mCompareLabel;
		List<WordValueEditor> mEditors;
		CheckBox mOverflowBox;
		Label mOverflowLabel;
		readonly WordValueEditor mrI1Editor;
		Label mrI1Label;
		readonly WordValueEditor mrI2Editor;
		Label mrI2Label;
		readonly WordValueEditor mrI3Editor;
		Label mrI3Label;
		readonly WordValueEditor mrI4Editor;
		Label mrI4Label;
		readonly WordValueEditor mr5Editor;
		Label mrI5Label;
		readonly WordValueEditor mrI6Editor;
		Label mrI6Label;
		readonly WordValueEditor mrAEditor;
		Label mrALabel;
		bool mReadOnly;
		MixLib.Registers mRegisters;
		readonly WordValueEditor mrJEditor;
		Label mrJLabel;
		readonly WordValueEditor mrXEditor;
		Label mrXLabel;

		public RegistersEditor()
	: this(null)
		{
		}

		public RegistersEditor(MixLib.Registers registers)
		{
			if (registers == null)
			{
				registers = new MixLib.Registers();
			}
			mRegisters = registers;
			mReadOnly = false;

			mrALabel = new Label();
			mrXLabel = new Label();
			mrI1Label = new Label();
			mrI2Label = new Label();
			mrI3Label = new Label();
			mrI4Label = new Label();
			mrI5Label = new Label();
			mrI6Label = new Label();
			mrJLabel = new Label();
			mCompareLabel = new Label();
			mOverflowLabel = new Label();

			mEditors = new List<WordValueEditor>
						{
								(mrAEditor = new WordValueEditor(mRegisters.RA)),
								(mrXEditor = new WordValueEditor(mRegisters.RX)),
								(mrI1Editor = new WordValueEditor(mRegisters.RI1)),
								(mrI2Editor = new WordValueEditor(mRegisters.RI2)),
								(mrI3Editor = new WordValueEditor(mRegisters.RI3)),
								(mrI4Editor = new WordValueEditor(mRegisters.RI4)),
								(mr5Editor = new WordValueEditor(mRegisters.RI5)),
								(mrI6Editor = new WordValueEditor(mRegisters.RI6)),
								(mrJEditor = new WordValueEditor(mRegisters.RJ, false))
						};

			mCompareButton = new Button();
			mOverflowBox = new CheckBox();

			SuspendLayout();

			mrALabel.Location = new Point(0, 0);
			mrALabel.Name = "mrALabel";
			mrALabel.Size = new Size(24, 21);
			mrALabel.TabIndex = 0;
			mrALabel.Text = "rA:";
			mrALabel.TextAlign = ContentAlignment.MiddleLeft;

			mrXLabel.Location = new Point(0, 24);
			mrXLabel.Name = "mrXLabel";
			mrXLabel.Size = new Size(24, 21);
			mrXLabel.TabIndex = 2;
			mrXLabel.Text = "rX:";
			mrXLabel.TextAlign = ContentAlignment.MiddleLeft;

			mrI1Label.Location = new Point(0, 48);
			mrI1Label.Name = "mr1Label";
			mrI1Label.Size = new Size(24, 21);
			mrI1Label.TabIndex = 4;
			mrI1Label.Text = "rI1:";
			mrI1Label.TextAlign = ContentAlignment.MiddleLeft;

			mrI2Label.Location = new Point(0, 72);
			mrI2Label.Name = "mr2Label";
			mrI2Label.Size = new Size(24, 21);
			mrI2Label.TabIndex = 6;
			mrI2Label.Text = "rI2:";
			mrI2Label.TextAlign = ContentAlignment.MiddleLeft;

			mrI3Label.Location = new Point(0, 96);
			mrI3Label.Name = "mr3Label";
			mrI3Label.Size = new Size(24, 21);
			mrI3Label.TabIndex = 8;
			mrI3Label.Text = "rI3:";
			mrI3Label.TextAlign = ContentAlignment.MiddleLeft;

			mrI4Label.Location = new Point(0, 120);
			mrI4Label.Name = "mr4Label";
			mrI4Label.Size = new Size(24, 21);
			mrI4Label.TabIndex = 10;
			mrI4Label.Text = "rI4:";
			mrI4Label.TextAlign = ContentAlignment.MiddleLeft;

			mrI5Label.Location = new Point(0, 144);
			mrI5Label.Name = "mr5Label";
			mrI5Label.Size = new Size(24, 21);
			mrI5Label.TabIndex = 12;
			mrI5Label.Text = "rI5:";
			mrI5Label.TextAlign = ContentAlignment.MiddleLeft;

			mrI6Label.Location = new Point(0, 168);
			mrI6Label.Name = "mr6Label";
			mrI6Label.Size = new Size(24, 21);
			mrI6Label.TabIndex = 14;
			mrI6Label.Text = "rI6:";
			mrI6Label.TextAlign = ContentAlignment.MiddleLeft;

			mrJLabel.Location = new Point(0, 192);
			mrJLabel.Name = "mrJLabel";
			mrJLabel.Size = new Size(24, 21);
			mrJLabel.TabIndex = 16;
			mrJLabel.Text = "rJ:";
			mrJLabel.TextAlign = ContentAlignment.MiddleLeft;

			mrAEditor.Location = new Point(24, 0);
			mrAEditor.Name = "mrAEditor";
			mrAEditor.TabIndex = 1;
			mrAEditor.TextBoxWidth = 80;
			mrAEditor.NavigationKeyDown += This_KeyDown;

			mrXEditor.Location = new Point(24, 24);
			mrXEditor.Name = "mrXEditor";
			mrXEditor.TabIndex = 3;
			mrXEditor.TextBoxWidth = 80;
			mrXEditor.NavigationKeyDown += This_KeyDown;

			mrI1Editor.Location = new Point(24, 48);
			mrI1Editor.Name = "mr1Editor";
			mrI1Editor.TabIndex = 5;
			mrI1Editor.TextBoxWidth = 40;
			mrI1Editor.NavigationKeyDown += This_KeyDown;

			mrI2Editor.Location = new Point(24, 72);
			mrI2Editor.Name = "mr2Editor";
			mrI2Editor.TabIndex = 7;
			mrI2Editor.TextBoxWidth = 40;
			mrI2Editor.NavigationKeyDown += This_KeyDown;

			mrI3Editor.Location = new Point(24, 96);
			mrI3Editor.Name = "mr3Editor";
			mrI3Editor.TabIndex = 9;
			mrI3Editor.TextBoxWidth = 40;
			mrI3Editor.NavigationKeyDown += This_KeyDown;

			mrI4Editor.Location = new Point(24, 120);
			mrI4Editor.Name = "mr4Editor";
			mrI4Editor.TabIndex = 11;
			mrI4Editor.TextBoxWidth = 40;
			mrI4Editor.NavigationKeyDown += This_KeyDown;

			mr5Editor.Location = new Point(24, 144);
			mr5Editor.Name = "mr5Editor";
			mr5Editor.TabIndex = 13;
			mr5Editor.TextBoxWidth = 40;
			mr5Editor.NavigationKeyDown += This_KeyDown;

			mrI6Editor.Location = new Point(24, 168);
			mrI6Editor.Name = "mr6Editor";
			mrI6Editor.TabIndex = 15;
			mrI6Editor.TextBoxWidth = 40;
			mrI6Editor.NavigationKeyDown += This_KeyDown;

			mrJEditor.Location = new Point(41, 192);
			mrJEditor.Name = "mrJEditor";
			mrJEditor.ReadOnly = false;
			mrJEditor.TabIndex = 17;
			mrJEditor.TextBoxWidth = 40;
			mrJEditor.NavigationKeyDown += This_KeyDown;

			mOverflowLabel.Name = "mOverflowLabel";
			mOverflowLabel.Size = new Size(56, 21);
			mOverflowLabel.TabIndex = 18;
			mOverflowLabel.Text = "Overflow:";
			mOverflowLabel.TextAlign = ContentAlignment.MiddleLeft;

			mOverflowBox.Name = "mOverflowBox";
			mOverflowBox.Size = new Size(16, 21);
			mOverflowBox.Location = new Point((mrAEditor.Left + mrAEditor.Width) - mOverflowBox.Width, mrI6Editor.Top);
			mOverflowBox.TabIndex = 19;
			mOverflowBox.FlatStyle = FlatStyle.Flat;
			mOverflowBox.CheckedChanged += MOverflowBox_CheckedChanged;

			mCompareLabel.Name = "mCompareLabel";
			mCompareLabel.Size = new Size(56, 21);
			mCompareLabel.TabIndex = 20;
			mCompareLabel.Text = "Compare:";
			mCompareLabel.TextAlign = ContentAlignment.MiddleLeft;

			mCompareButton.FlatStyle = FlatStyle.Flat;
			mCompareButton.Name = "mCompareButton";
			mCompareButton.Size = new Size(18, 21);
			mCompareButton.Location = new Point((mrAEditor.Left + mrAEditor.Width) - mCompareButton.Width, mrJEditor.Top);
			mCompareButton.TabIndex = 21;
			mCompareButton.Text = "" + mRegisters.CompareIndicator.ToChar();
			mCompareButton.Click += MCompareButton_Click;

			mOverflowLabel.Location = new Point(Math.Min(mCompareButton.Left, mOverflowBox.Left) - Math.Max(mCompareLabel.Width, mOverflowLabel.Width), mOverflowBox.Top);

			mCompareLabel.Location = new Point(mOverflowLabel.Left, mCompareButton.Top);

			Controls.Add(mCompareButton);
			Controls.Add(mCompareLabel);
			Controls.Add(mOverflowBox);
			Controls.Add(mOverflowLabel);
			Controls.Add(mrJEditor);
			Controls.Add(mrI6Editor);
			Controls.Add(mr5Editor);
			Controls.Add(mrI4Editor);
			Controls.Add(mrI3Editor);
			Controls.Add(mrI2Editor);
			Controls.Add(mrI1Editor);
			Controls.Add(mrXEditor);
			Controls.Add(mrAEditor);
			Controls.Add(mrJLabel);
			Controls.Add(mrI6Label);
			Controls.Add(mrI5Label);
			Controls.Add(mrI4Label);
			Controls.Add(mrI3Label);
			Controls.Add(mrI2Label);
			Controls.Add(mrI1Label);
			Controls.Add(mrXLabel);
			Controls.Add(mrALabel);
			Name = "RegistersEditor";
			Size = new Size(mrAEditor.Left + mrAEditor.Width, mrJEditor.Left + mrJEditor.Height);

			ResumeLayout(false);
		}

		void MOverflowBox_CheckedChanged(object sender, EventArgs e)
		{
			mRegisters.OverflowIndicator = mOverflowBox.Checked;
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			FieldTypes? editorField = null;
			int? index = null;

			if (e is FieldKeyEventArgs)
			{
				editorField = ((FieldKeyEventArgs)e).Field;
				index = ((FieldKeyEventArgs)e).Index;
			}

			var editorIndex = mEditors.IndexOf((WordValueEditor)sender);

			if (e.KeyCode == Keys.Down && editorIndex < mEditors.Count - 1)
			{
				mEditors[editorIndex + 1].Focus(editorField, index);
			}
			else if (e.KeyCode == Keys.Up && editorIndex > 0)
			{
				mEditors[editorIndex - 1].Focus(editorField, index);
			}
		}

		void MCompareButton_Click(object sender, EventArgs e)
		{
			var compValue = mRegisters.CompareIndicator.Next();
			mRegisters.CompareIndicator = compValue;
			mCompareButton.Text = "" + compValue.ToChar();
		}

		public new void Update()
		{
			foreach (WordValueEditor editor in mEditors) editor.Update();

			mCompareButton.Text = "" + mRegisters.CompareIndicator.ToChar();
			mOverflowBox.Checked = mRegisters.OverflowIndicator;

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (WordValueEditor editor in mEditors) editor.UpdateLayout();
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

					foreach (WordValueEditor editor in mEditors)
					{
						editor.ReadOnly = mReadOnly;
					}

					mCompareButton.Enabled = !mReadOnly;
					mOverflowBox.Enabled = !mReadOnly;
				}
			}
		}

		public MixLib.Registers Registers
		{
			get
			{
				return mRegisters;
			}
			set
			{
				if (value != null && mRegisters != value)
				{
					mRegisters = value;
					mrAEditor.WordValue = mRegisters.RA;
					mrXEditor.WordValue = mRegisters.RX;
					mrI1Editor.WordValue = mRegisters.RI1;
					mrI2Editor.WordValue = mRegisters.RI2;
					mrI3Editor.WordValue = mRegisters.RI3;
					mrI4Editor.WordValue = mRegisters.RI4;
					mr5Editor.WordValue = mRegisters.RI5;
					mrI6Editor.WordValue = mRegisters.RI6;
					mrJEditor.WordValue = mRegisters.RJ;
				}
			}
		}
	}
}
