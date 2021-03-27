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
		private Button _clearButton;
		private CheckBox _echoInputCheckBox;
		private TextBox _inputTextBox;
		private CheckBox _onTopCheckBox;
		private TextBox _outputTextBox;
		private Label _promptLabel;
		private Button _sendButton;
		private StatusStrip _statusStrip;
		private ToolStripStatusLabel _toolStripStatusLabel;
		private MixLib.Device.TeletypeDevice _teletypeDevice;

		public TeletypeForm(MixLib.Device.TeletypeDevice teleType)
		{
			InitializeComponent();
			UpdateLayout();
			_inputTextBox.Focus();

			TeletypeDevice = teleType;
		}

		private void ClearButton_Click(object sender, EventArgs e) 
			=> ClearOutput();

		private void SendButton_Click(object sender, EventArgs e) 
			=> SendInput();

		private void This_Activated(object sender, EventArgs e)
		{
			_outputTextBox.Select(_outputTextBox.TextLength, 0);
			_outputTextBox.Focus();
			_outputTextBox.ScrollToCaret();
			_inputTextBox.Focus();
		}

		private delegate void addOutputTextDelegate(string textToAdd);

		public void AddOutputText(string textToAdd)
		{
			if (_outputTextBox.InvokeRequired)
			{
				_outputTextBox.Invoke(new addOutputTextDelegate(AddOutputText), textToAdd);
				return;
			}

			if (_outputTextBox.TextLength > 0)
				textToAdd = Environment.NewLine + textToAdd;

			int startIndex = 0;

			while (_outputTextBox.TextLength + textToAdd.Length - startIndex > _outputTextBox.MaxLength)
				startIndex = _outputTextBox.Text.IndexOf(Environment.NewLine, startIndex, StringComparison.Ordinal) + Environment.NewLine.Length;

			if (startIndex > 0)
				_outputTextBox.Text = _outputTextBox.Text[startIndex..] + textToAdd;

			else
				_outputTextBox.AppendText(textToAdd);
		}

		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

		private void InitializeComponent()
		{
			var resources = new ComponentResourceManager(typeof(TeletypeForm));
			_outputTextBox = new TextBox();
			_promptLabel = new Label();
			_inputTextBox = new TextBox();
			_statusStrip = new StatusStrip();
			_toolStripStatusLabel = new ToolStripStatusLabel();
			_clearButton = new Button();
			_sendButton = new Button();
			_onTopCheckBox = new CheckBox();
			_echoInputCheckBox = new CheckBox();
			SuspendLayout();
			// 
			// mOutputTextBox
			// 
			_outputTextBox.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
															| AnchorStyles.Left)
															| AnchorStyles.Right);
			_outputTextBox.Location = new Point(0, 0);
			_outputTextBox.MaxLength = 1048576;
			_outputTextBox.Multiline = true;
			_outputTextBox.Name = "mOutputTextBox";
			_outputTextBox.ReadOnly = true;
			_outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			_outputTextBox.Size = new Size(528, 264);
			_outputTextBox.TabIndex = 0;
			_outputTextBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mPromptLabel
			// 
			_promptLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			_promptLabel.Location = new Point(0, 266);
			_promptLabel.Name = "mPromptLabel";
			_promptLabel.Size = new Size(8, 21);
			_promptLabel.TabIndex = 2;
			_promptLabel.Text = ">";
			_promptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mInputTextBox
			// 
			_inputTextBox.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
															| AnchorStyles.Right);
			_inputTextBox.CharacterCasing = CharacterCasing.Upper;
			_inputTextBox.Location = new Point(16, 265);
			_inputTextBox.MaxLength = 70;
			_inputTextBox.Name = "mInputTextBox";
			_inputTextBox.Size = new Size(512, 20);
			_inputTextBox.TabIndex = 3;
			_inputTextBox.KeyPress += This_KeyPress;
			_inputTextBox.BorderStyle = BorderStyle.FixedSingle;
			// 
			// mStatusBar
			// 
			_statusStrip.Location = new Point(0, 287);
			_statusStrip.Name = "mStatusBar";
			_statusStrip.Items.AddRange(new ToolStripItem[] {
						_toolStripStatusLabel});
			_statusStrip.Size = new Size(616, 22);
			_statusStrip.TabIndex = 5;
			// 
			// mStatusBarPanel
			// 
			_toolStripStatusLabel.AutoSize = true;
			_toolStripStatusLabel.Name = "mStatusBarPanel";
			_toolStripStatusLabel.Text = "Idle";
			_toolStripStatusLabel.Width = 600;
			_toolStripStatusLabel.BorderStyle = Border3DStyle.Flat;
			// 
			// mClearButton
			// 
			_clearButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			_clearButton.Location = new Point(536, 232);
			_clearButton.Name = "mClearButton";
			_clearButton.Size = new Size(75, 23);
			_clearButton.TabIndex = 1;
			_clearButton.Text = "&Clear";
			_clearButton.FlatStyle = FlatStyle.Flat;
			_clearButton.Click += ClearButton_Click;
			// 
			// mSendButton
			// 
			_sendButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			_sendButton.Location = new Point(536, 264);
			_sendButton.Name = "mSendButton";
			_sendButton.Size = new Size(75, 23);
			_sendButton.TabIndex = 4;
			_sendButton.Text = "&Send";
			_sendButton.FlatStyle = FlatStyle.Flat;
			_sendButton.Click += SendButton_Click;
			// 
			// mOnTopCheckBox
			// 
			_onTopCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_onTopCheckBox.Location = new Point(536, 0);
			_onTopCheckBox.Name = "mOnTopCheckBox";
			_onTopCheckBox.Size = new Size(72, 24);
			_onTopCheckBox.TabIndex = 6;
			_onTopCheckBox.Text = "&On top";
			_onTopCheckBox.FlatStyle = FlatStyle.Flat;
			_onTopCheckBox.CheckedChanged += MOnTopCheckBox_CheckedChanged;
			// 
			// mEchoInputCheckBox
			// 
			_echoInputCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			_echoInputCheckBox.Location = new Point(536, 24);
			_echoInputCheckBox.Name = "mEchoInputCheckBox";
			_echoInputCheckBox.Size = new Size(88, 24);
			_echoInputCheckBox.TabIndex = 7;
			_echoInputCheckBox.Text = "&Echo input";
			_echoInputCheckBox.FlatStyle = FlatStyle.Flat;
			// 
			// TeletypeForm
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new Size(616, 309);
			Controls.Add(_echoInputCheckBox);
			Controls.Add(_onTopCheckBox);
			Controls.Add(_sendButton);
			Controls.Add(_clearButton);
			Controls.Add(_statusStrip);
			Controls.Add(_promptLabel);
			Controls.Add(_inputTextBox);
			Controls.Add(_outputTextBox);
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
			if (_outputTextBox.InvokeRequired)
			{
				_outputTextBox.Invoke(new MethodInvoker(ClearOutput));
				return;
			}

			_outputTextBox.Text = "";
		}

		private void MOnTopCheckBox_CheckedChanged(object sender, EventArgs e) => TopMost = _onTopCheckBox.Checked;

		private void OutputAdded(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(OutputAdded));
				return;
			}

			string outputLine;
			string textToAdd = "";

			while ((outputLine = _teletypeDevice.GetOutputLine()) != null)
			{
				if (textToAdd.Length != 0)
				{
					textToAdd += Environment.NewLine;
				}
				textToAdd += outputLine;
			}

			AddOutputText(textToAdd);
			_toolStripStatusLabel.Text = "Received output from Mix";

			if (Visible)
			{
				_outputTextBox.Select(_outputTextBox.TextLength, 0);
				_outputTextBox.Focus();
				_outputTextBox.ScrollToCaret();
				_inputTextBox.Focus();
			}
		}

		private void SendInput()
		{
			if (EchoInput)
				AddOutputText("> " + _inputTextBox.Text);

			_teletypeDevice.AddInputLine(_inputTextBox.Text);
			_inputTextBox.Text = "";
			_toolStripStatusLabel.Text = "Sent input to Mix";
		}

		public void UpdateLayout()
		{
			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			var color = GuiSettings.GetColor(GuiSettings.TeletypeInputText);

			_promptLabel.Font = font;
			_promptLabel.ForeColor = color;

			_inputTextBox.Font = font;
			_inputTextBox.ForeColor = color;
			_inputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeInputBackground);

			_outputTextBox.Font = font;
			_outputTextBox.ForeColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputText);
			_outputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputBackground);
		}

		private void This_VisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				_toolStripStatusLabel.Text = "Idle";
		}

		public bool EchoInput
		{
			get => _echoInputCheckBox.Checked;
			set => _echoInputCheckBox.Checked = value;
		}

		public string StatusText
		{
			set => _toolStripStatusLabel.Text = value;
		}

		public MixLib.Device.TeletypeDevice TeletypeDevice
		{
			get => _teletypeDevice;
			set
			{
				if (_teletypeDevice == null)
				{
					_teletypeDevice = value;

					if (_teletypeDevice != null)
						_teletypeDevice.OutputAdded += OutputAdded;
				}
			}
		}

		public new bool TopMost
		{
			get => base.TopMost;
			set
			{
				_onTopCheckBox.Checked = value;
				base.TopMost = value;
			}
		}
	}
}
