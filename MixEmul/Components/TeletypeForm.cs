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
		private Container components = null;
		private Button mClearButton;
		private CheckBox mEchoInputCheckBox;
		private TextBox mInputTextBox;
		private CheckBox mOnTopCheckBox;
		private TextBox mOutputTextBox;
		private Label mPromptLabel;
		private Button mSendButton;
		private StatusBar mStatusBar;
		private StatusBarPanel mStatusBarPanel;
		private MixLib.Device.TeletypeDevice mTeletypeDevice;

		public TeletypeForm(MixLib.Device.TeletypeDevice teleType)
		{
			InitializeComponent();
			UpdateLayout();
			mInputTextBox.Focus();

			TeletypeDevice = teleType;
		}

		private void activated(object sender, EventArgs e)
		{
			mOutputTextBox.Select(mOutputTextBox.TextLength, 0);
			mOutputTextBox.Focus();
			mOutputTextBox.ScrollToCaret();
			mInputTextBox.Focus();
		}

		private delegate void addOutputTextDelegate(string textToAdd);

		public void AddOutputText(string textToAdd)
		{
			if (mOutputTextBox.InvokeRequired)
			{
				mOutputTextBox.Invoke(new addOutputTextDelegate(AddOutputText), textToAdd);
				return;
			}

			if (mOutputTextBox.TextLength > 0)
			{
				textToAdd = Environment.NewLine + textToAdd;
			}

			int startIndex = 0;

			while (mOutputTextBox.TextLength + textToAdd.Length - startIndex > mOutputTextBox.MaxLength)
			{
				startIndex = mOutputTextBox.Text.IndexOf(Environment.NewLine, startIndex) + Environment.NewLine.Length;
			}

			if (startIndex > 0)
			{
				mOutputTextBox.Text = mOutputTextBox.Text.Substring(startIndex) + textToAdd;
			}
			else
			{
				mOutputTextBox.AppendText(textToAdd);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void formClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			base.Visible = false;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeletypeForm));
			this.mOutputTextBox = new System.Windows.Forms.TextBox();
			this.mPromptLabel = new System.Windows.Forms.Label();
			this.mInputTextBox = new System.Windows.Forms.TextBox();
			this.mStatusBar = new System.Windows.Forms.StatusBar();
			this.mStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.mClearButton = new System.Windows.Forms.Button();
			this.mSendButton = new System.Windows.Forms.Button();
			this.mOnTopCheckBox = new System.Windows.Forms.CheckBox();
			this.mEchoInputCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.mStatusBarPanel)).BeginInit();
			this.SuspendLayout();
			// 
			// mOutputTextBox
			// 
			this.mOutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mOutputTextBox.Location = new System.Drawing.Point(0, 0);
			this.mOutputTextBox.MaxLength = 1048576;
			this.mOutputTextBox.Multiline = true;
			this.mOutputTextBox.Name = "mOutputTextBox";
			this.mOutputTextBox.ReadOnly = true;
			this.mOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.mOutputTextBox.Size = new System.Drawing.Size(528, 264);
			this.mOutputTextBox.TabIndex = 0;
			// 
			// mPromptLabel
			// 
			this.mPromptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mPromptLabel.Location = new System.Drawing.Point(0, 266);
			this.mPromptLabel.Name = "mPromptLabel";
			this.mPromptLabel.Size = new System.Drawing.Size(8, 21);
			this.mPromptLabel.TabIndex = 2;
			this.mPromptLabel.Text = ">";
			this.mPromptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mInputTextBox
			// 
			this.mInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mInputTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mInputTextBox.Location = new System.Drawing.Point(16, 265);
			this.mInputTextBox.MaxLength = 70;
			this.mInputTextBox.Name = "mInputTextBox";
			this.mInputTextBox.Size = new System.Drawing.Size(512, 20);
			this.mInputTextBox.TabIndex = 3;
			this.mInputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.keyPress);
			// 
			// mStatusBar
			// 
			this.mStatusBar.Location = new System.Drawing.Point(0, 287);
			this.mStatusBar.Name = "mStatusBar";
			this.mStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.mStatusBarPanel});
			this.mStatusBar.ShowPanels = true;
			this.mStatusBar.Size = new System.Drawing.Size(616, 22);
			this.mStatusBar.TabIndex = 5;
			// 
			// mStatusBarPanel
			// 
			this.mStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.mStatusBarPanel.Name = "mStatusBarPanel";
			this.mStatusBarPanel.Text = "Idle";
			this.mStatusBarPanel.Width = 600;
			// 
			// mClearButton
			// 
			this.mClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mClearButton.Location = new System.Drawing.Point(536, 232);
			this.mClearButton.Name = "mClearButton";
			this.mClearButton.Size = new System.Drawing.Size(75, 23);
			this.mClearButton.TabIndex = 1;
			this.mClearButton.Text = "&Clear";
			this.mClearButton.Click += new System.EventHandler(this.mClearButton_Click);
			// 
			// mSendButton
			// 
			this.mSendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mSendButton.Location = new System.Drawing.Point(536, 264);
			this.mSendButton.Name = "mSendButton";
			this.mSendButton.Size = new System.Drawing.Size(75, 23);
			this.mSendButton.TabIndex = 4;
			this.mSendButton.Text = "&Send";
			this.mSendButton.Click += new System.EventHandler(this.mSendButton_Click);
			// 
			// mOnTopCheckBox
			// 
			this.mOnTopCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mOnTopCheckBox.Location = new System.Drawing.Point(536, 0);
			this.mOnTopCheckBox.Name = "mOnTopCheckBox";
			this.mOnTopCheckBox.Size = new System.Drawing.Size(72, 24);
			this.mOnTopCheckBox.TabIndex = 6;
			this.mOnTopCheckBox.Text = "&On top";
			this.mOnTopCheckBox.CheckedChanged += new System.EventHandler(this.mOnTopCheckBox_CheckedChanged);
			// 
			// mEchoInputCheckBox
			// 
			this.mEchoInputCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mEchoInputCheckBox.Location = new System.Drawing.Point(536, 24);
			this.mEchoInputCheckBox.Name = "mEchoInputCheckBox";
			this.mEchoInputCheckBox.Size = new System.Drawing.Size(88, 24);
			this.mEchoInputCheckBox.TabIndex = 7;
			this.mEchoInputCheckBox.Text = "&Echo input";
			// 
			// TeletypeForm
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 309);
			this.Controls.Add(this.mEchoInputCheckBox);
			this.Controls.Add(this.mOnTopCheckBox);
			this.Controls.Add(this.mSendButton);
			this.Controls.Add(this.mClearButton);
			this.Controls.Add(this.mStatusBar);
			this.Controls.Add(this.mPromptLabel);
			this.Controls.Add(this.mInputTextBox);
			this.Controls.Add(this.mOutputTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TeletypeForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Teletype";
			this.TopMost = true;
			this.Activated += new System.EventHandler(this.activated);
			this.VisibleChanged += new System.EventHandler(this.visibleChanged);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formClosing);
			((System.ComponentModel.ISupportInitialize)(this.mStatusBarPanel)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void keyPress(object sender, KeyPressEventArgs args)
		{
			char keyChar = args.KeyChar;
			if (keyChar == (char)Keys.Enter)
			{
				sendInput();
				args.Handled = true;
			}
			else
			{
				args.Handled = !char.IsControl(keyChar) && (MixByte.MixChars.IndexOf(char.ToUpper(keyChar)) < 0);
			}
		}

		private void mClearButton_Click(object sender, EventArgs e)
		{
			ClearOutput();
		}

		public void ClearOutput()
		{
			if (mOutputTextBox.InvokeRequired)
			{
				mOutputTextBox.Invoke(new MethodInvoker(ClearOutput));
				return;
			}

			mOutputTextBox.Text = "";
		}

		private void mOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			TopMost = mOnTopCheckBox.Checked;
		}

		private void mSendButton_Click(object sender, EventArgs e)
		{
			sendInput();
		}

		private void outputAdded(object sender, EventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(outputAdded));
			}
			else
			{
				string outputLine;
				string textToAdd = "";

				while ((outputLine = mTeletypeDevice.GetOutputLine()) != null)
				{
					if (textToAdd.Length != 0)
					{
						textToAdd = textToAdd + Environment.NewLine;
					}
					textToAdd = textToAdd + outputLine;
				}

				AddOutputText(textToAdd);
				mStatusBarPanel.Text = "Received output from Mix";

				if (base.Visible)
				{
					mOutputTextBox.Select(mOutputTextBox.TextLength, 0);
					mOutputTextBox.Focus();
					mOutputTextBox.ScrollToCaret();
					mInputTextBox.Focus();
				}
			}
		}

		private void sendInput()
		{
			if (EchoInput)
			{
				AddOutputText("> " + mInputTextBox.Text);
			}

			mTeletypeDevice.AddInputLine(mInputTextBox.Text);
			mInputTextBox.Text = "";
			mStatusBarPanel.Text = "Sent input to Mix";
		}

		public void UpdateLayout()
		{
			Font font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			Color color = GuiSettings.GetColor(GuiSettings.TeletypeInputText);

			mPromptLabel.Font = font;
			mPromptLabel.ForeColor = color;

			mInputTextBox.Font = font;
			mInputTextBox.ForeColor = color;
			mInputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeInputBackground);

			mOutputTextBox.Font = font;
			mOutputTextBox.ForeColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputText);
			mOutputTextBox.BackColor = GuiSettings.GetColor(GuiSettings.TeletypeOutputBackground);
		}

		private void visibleChanged(object sender, EventArgs e)
		{
			if (!base.Visible)
			{
				mStatusBarPanel.Text = "Idle";
			}
		}

		public bool EchoInput
		{
			get
			{
				return mEchoInputCheckBox.Checked;
			}
			set
			{
				mEchoInputCheckBox.Checked = value;
			}
		}

		public string StatusText
		{
			set
			{
				mStatusBarPanel.Text = value;
			}
		}

		public MixLib.Device.TeletypeDevice TeletypeDevice
		{
			get
			{
				return mTeletypeDevice;
			}
			set
			{
				if (mTeletypeDevice == null)
				{
					mTeletypeDevice = value;

					if (mTeletypeDevice != null)
					{
						mTeletypeDevice.OutputAdded += new EventHandler(outputAdded);
					}
				}
			}
		}

		public new bool TopMost
		{
			get
			{
				return base.TopMost;
			}
			set
			{
				mOnTopCheckBox.Checked = value;
				base.TopMost = value;
			}
		}
	}
}
