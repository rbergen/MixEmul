using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class TeletypeForm : Form
	{
		private Button clearButton;
		private CheckBox echoInputCheckBox;
		private TextBox inputTextBox;
		private CheckBox onTopCheckBox;
		private TextBox outputTextBox;
		private Label promptLabel;
		private Button sendButton;
		private StatusStrip statusStrip;
		private ToolStripStatusLabel toolStripStatusLabel;
		private MixLib.Device.TeletypeDevice teletypeDevice;

		public TeletypeForm(MixLib.Device.TeletypeDevice teleType)
		{
			InitializeComponent();
			UpdateLayout();
			this.inputTextBox.Focus();

			TeletypeDevice = teleType;
		}

		private void ClearButton_Click(object sender, EventArgs e) 
			=> ClearOutput();

		private void SendButton_Click(object sender, EventArgs e) 
			=> SendInput();

		private void This_Activated(object sender, EventArgs e)
		{
			this.outputTextBox.Select(this.outputTextBox.TextLength, 0);
			this.outputTextBox.Focus();
			this.outputTextBox.ScrollToCaret();
			this.inputTextBox.Focus();
		}

		private delegate void addOutputTextDelegate(string textToAdd);

		public void AddOutputText(string textToAdd)
		{
			if (this.outputTextBox.InvokeRequired)
			{
				this.outputTextBox.Invoke(new addOutputTextDelegate(AddOutputText), textToAdd);
				return;
			}

			if (this.outputTextBox.TextLength > 0)
				textToAdd = Environment.NewLine + textToAdd;

			int startIndex = 0;

			while (this.outputTextBox.TextLength + textToAdd.Length - startIndex > this.outputTextBox.MaxLength)
				startIndex = this.outputTextBox.Text.IndexOf(Environment.NewLine, startIndex, StringComparison.Ordinal) + Environment.NewLine.Length;

			if (startIndex > 0)
				this.outputTextBox.Text = this.outputTextBox.Text[startIndex..] + textToAdd;

			else
				this.outputTextBox.AppendText(textToAdd);
		}

		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

		private void InitializeComponent()
		{
			var resources = new ComponentResourceManager(typeof(TeletypeForm));
			this.outputTextBox = new TextBox();
			this.promptLabel = new Label();
			this.inputTextBox = new TextBox();
			this.statusStrip = new StatusStrip();
			this.toolStripStatusLabel = new ToolStripStatusLabel();
			this.clearButton = new Button();
			this.sendButton = new Button();
			this.onTopCheckBox = new CheckBox();
			this.echoInputCheckBox = new CheckBox();
			SuspendLayout();
			// 
			// mOutputTextBox
			// 
			this.outputTextBox.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
															| AnchorStyles.Left)
															| AnchorStyles.Right);
			this.outputTextBox.Location = new Point(0, 0);
			this.outputTextBox.MaxLength = 1048576;
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "mOutputTextBox";
			this.outputTextBox.ReadOnly = true;
			this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.outputTextBox.Size = new Size(528, 264);
			this.outputTextBox.TabIndex = 0;
			this.outputTextBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mPromptLabel
			// 
			this.promptLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.promptLabel.Location = new Point(0, 266);
			this.promptLabel.Name = "mPromptLabel";
			this.promptLabel.Size = new Size(8, 21);
			this.promptLabel.TabIndex = 2;
			this.promptLabel.Text = ">";
			this.promptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mInputTextBox
			// 
			this.inputTextBox.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
															| AnchorStyles.Right);
			this.inputTextBox.CharacterCasing = CharacterCasing.Upper;
			this.inputTextBox.Location = new Point(16, 265);
			this.inputTextBox.MaxLength = 70;
			this.inputTextBox.Name = "mInputTextBox";
			this.inputTextBox.Size = new Size(512, 20);
			this.inputTextBox.TabIndex = 3;
			this.inputTextBox.KeyPress += This_KeyPress;
			this.inputTextBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mStatusBar
			// 
			this.statusStrip.Location = new Point(0, 287);
			this.statusStrip.Name = "mStatusBar";
			this.statusStrip.Items.AddRange(new ToolStripItem[] {
						this.toolStripStatusLabel});
			this.statusStrip.Size = new Size(616, 22);
			this.statusStrip.TabIndex = 5;
			// 
			// mStatusBarPanel
			// 
			this.toolStripStatusLabel.AutoSize = true;
			this.toolStripStatusLabel.Name = "mStatusBarPanel";
			this.toolStripStatusLabel.Text = "Idle";
			this.toolStripStatusLabel.Width = 600;
			this.toolStripStatusLabel.BorderStyle = Border3DStyle.Flat;
			// 
			// mClearButton
			// 
			this.clearButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.clearButton.Location = new Point(536, 232);
			this.clearButton.Name = "mClearButton";
			this.clearButton.Size = new Size(75, 23);
			this.clearButton.TabIndex = 1;
			this.clearButton.Text = "&Clear";
			this.clearButton.FlatStyle = FlatStyle.Flat;
			this.clearButton.Click += ClearButton_Click;
			// 
			// mSendButton
			// 
			this.sendButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.sendButton.Location = new Point(536, 264);
			this.sendButton.Name = "mSendButton";
			this.sendButton.Size = new Size(75, 23);
			this.sendButton.TabIndex = 4;
			this.sendButton.Text = "&Send";
			this.sendButton.FlatStyle = FlatStyle.Flat;
			this.sendButton.Click += SendButton_Click;
			// 
			// mOnTopCheckBox
			// 
			this.onTopCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.onTopCheckBox.Location = new Point(536, 0);
			this.onTopCheckBox.Name = "mOnTopCheckBox";
			this.onTopCheckBox.Size = new Size(72, 24);
			this.onTopCheckBox.TabIndex = 6;
			this.onTopCheckBox.Text = "&On top";
			this.onTopCheckBox.FlatStyle = FlatStyle.Flat;
			this.onTopCheckBox.CheckedChanged += MOnTopCheckBox_CheckedChanged;
			// 
			// mEchoInputCheckBox
			// 
			this.echoInputCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.echoInputCheckBox.Location = new Point(536, 24);
			this.echoInputCheckBox.Name = "mEchoInputCheckBox";
			this.echoInputCheckBox.Size = new Size(88, 24);
			this.echoInputCheckBox.TabIndex = 7;
			this.echoInputCheckBox.Text = "&Echo input";
			this.echoInputCheckBox.FlatStyle = FlatStyle.Flat;
			// 
			// TeletypeForm
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new Size(616, 309);
			Controls.Add(this.echoInputCheckBox);
			Controls.Add(this.onTopCheckBox);
			Controls.Add(this.sendButton);
			Controls.Add(this.clearButton);
			Controls.Add(this.statusStrip);
			Controls.Add(this.promptLabel);
			Controls.Add(this.inputTextBox);
			Controls.Add(this.outputTextBox);
			Icon = ((Icon)(resources.GetObject("$this.Icon")));
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "TeletypeForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Teletype";
			TopMost = true;
			Activated += This_Activated;
			VisibleChanged += This_VisibleChanged;
			FormClosing += This_FormClosing;
			ResumeLayout(false);
			PerformLayout();

		}

		private void This_KeyPress(object sender, KeyPressEventArgs args)
		{
			char keyChar = args.KeyChar;
			if (keyChar == (char)Keys.Enter)
			{
				SendInput();
				args.Handled = true;
			}
			else
				args.Handled = !char.IsControl(keyChar) && (MixByte.MixChars.IndexOf(char.ToUpper(keyChar)) < 0);
		}

		public void ClearOutput()
		{
			if (this.outputTextBox.InvokeRequired)
			{
				this.outputTextBox.Invoke(new MethodInvoker(ClearOutput));
				return;
			}

			this.outputTextBox.Text = string.Empty;
		}

		private void MOnTopCheckBox_CheckedChanged(object sender, EventArgs e) => TopMost = this.onTopCheckBox.Checked;

		private void OutputAdded(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(OutputAdded));
				return;
			}

			string outputLine;
			string textToAdd = string.Empty;

			while ((outputLine = this.teletypeDevice.GetOutputLine()) != null)
			{
				if (textToAdd.Length != 0)
				{
					textToAdd += Environment.NewLine;
				}
				textToAdd += outputLine;
			}

			AddOutputText(textToAdd);
			this.toolStripStatusLabel.Text = "Received output from Mix";

			if (Visible)
			{
				this.outputTextBox.Select(this.outputTextBox.TextLength, 0);
				this.outputTextBox.Focus();
				this.outputTextBox.ScrollToCaret();
				this.inputTextBox.Focus();
			}
		}

		private void SendInput()
		{
			if (EchoInput)
				AddOutputText("> " + this.inputTextBox.Text);

			this.teletypeDevice.AddInputLine(this.inputTextBox.Text);
			this.inputTextBox.Text = string.Empty;
			this.toolStripStatusLabel.Text = "Sent input to Mix";
		}

		public void UpdateLayout()
		{
			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			var color = GuiSettings.GetColor(GuiSettings.TeletypeInputText);

			this.promptLabel.Font = font;
			this.promptLabel.ForeColor = color;

			this.inputTextBox.Font = font;
			this.inputTextBox.ForeColor = color;
			this.inputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeInputBackground);

			this.outputTextBox.Font = font;
			this.outputTextBox.ForeColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputText);
			this.outputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputBackground);
		}

		private void This_VisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				this.toolStripStatusLabel.Text = "Idle";
		}

		public bool EchoInput
		{
			get => this.echoInputCheckBox.Checked;
			set => this.echoInputCheckBox.Checked = value;
		}

		public string StatusText
		{
			set => this.toolStripStatusLabel.Text = value;
		}

		public MixLib.Device.TeletypeDevice TeletypeDevice
		{
			get => this.teletypeDevice;
			set
			{
				if (this.teletypeDevice == null)
				{
					this.teletypeDevice = value;

					if (this.teletypeDevice != null)
						this.teletypeDevice.OutputAdded += OutputAdded;
				}
			}
		}

		public new bool TopMost
		{
			get => base.TopMost;
			set
			{
				this.onTopCheckBox.Checked = value;
				base.TopMost = value;
			}
		}
	}
}
