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
        Button mClearButton;
        CheckBox mEchoInputCheckBox;
        TextBox mInputTextBox;
        CheckBox mOnTopCheckBox;
        TextBox mOutputTextBox;
        Label mPromptLabel;
        Button mSendButton;
        StatusBar mStatusBar;
        StatusBarPanel mStatusBarPanel;
        MixLib.Device.TeletypeDevice mTeletypeDevice;

        public TeletypeForm(MixLib.Device.TeletypeDevice teleType)
		{
			InitializeComponent();
			UpdateLayout();
			mInputTextBox.Focus();

			TeletypeDevice = teleType;
		}

        void mClearButton_Click(object sender, EventArgs e) => ClearOutput();

        void mSendButton_Click(object sender, EventArgs e) => sendInput();

        void activated(object sender, EventArgs e)
        {
            mOutputTextBox.Select(mOutputTextBox.TextLength, 0);
            mOutputTextBox.Focus();
            mOutputTextBox.ScrollToCaret();
            mInputTextBox.Focus();
        }

        delegate void addOutputTextDelegate(string textToAdd);

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
				startIndex = mOutputTextBox.Text.IndexOf(Environment.NewLine, startIndex, StringComparison.Ordinal) + Environment.NewLine.Length;
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

        void formClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(TeletypeForm));
            mOutputTextBox = new TextBox();
            mPromptLabel = new Label();
            mInputTextBox = new TextBox();
            mStatusBar = new System.Windows.Forms.StatusBar();
            mStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            mClearButton = new Button();
            mSendButton = new Button();
            mOnTopCheckBox = new CheckBox();
            mEchoInputCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)(mStatusBarPanel)).BeginInit();
            SuspendLayout();
            // 
            // mOutputTextBox
            // 
            mOutputTextBox.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                                    | AnchorStyles.Left)
                                    | AnchorStyles.Right);
            mOutputTextBox.Location = new Point(0, 0);
            mOutputTextBox.MaxLength = 1048576;
            mOutputTextBox.Multiline = true;
            mOutputTextBox.Name = "mOutputTextBox";
            mOutputTextBox.ReadOnly = true;
            mOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            mOutputTextBox.Size = new Size(528, 264);
            mOutputTextBox.TabIndex = 0;
            // 
            // mPromptLabel
            // 
            mPromptLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            mPromptLabel.Location = new Point(0, 266);
            mPromptLabel.Name = "mPromptLabel";
            mPromptLabel.Size = new Size(8, 21);
            mPromptLabel.TabIndex = 2;
            mPromptLabel.Text = ">";
            mPromptLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // mInputTextBox
            // 
            mInputTextBox.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left)
                                    | AnchorStyles.Right);
            mInputTextBox.CharacterCasing = CharacterCasing.Upper;
            mInputTextBox.Location = new Point(16, 265);
            mInputTextBox.MaxLength = 70;
            mInputTextBox.Name = "mInputTextBox";
            mInputTextBox.Size = new Size(512, 20);
            mInputTextBox.TabIndex = 3;
            mInputTextBox.KeyPress += keyPress;
            // 
            // mStatusBar
            // 
            mStatusBar.Location = new Point(0, 287);
            mStatusBar.Name = "mStatusBar";
            mStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            mStatusBarPanel});
            mStatusBar.ShowPanels = true;
            mStatusBar.Size = new Size(616, 22);
            mStatusBar.TabIndex = 5;
            // 
            // mStatusBarPanel
            // 
            mStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            mStatusBarPanel.Name = "mStatusBarPanel";
            mStatusBarPanel.Text = "Idle";
            mStatusBarPanel.Width = 600;
            // 
            // mClearButton
            // 
            mClearButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            mClearButton.Location = new Point(536, 232);
            mClearButton.Name = "mClearButton";
            mClearButton.Size = new Size(75, 23);
            mClearButton.TabIndex = 1;
            mClearButton.Text = "&Clear";
            mClearButton.Click += mClearButton_Click;
            // 
            // mSendButton
            // 
            mSendButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            mSendButton.Location = new Point(536, 264);
            mSendButton.Name = "mSendButton";
            mSendButton.Size = new Size(75, 23);
            mSendButton.TabIndex = 4;
            mSendButton.Text = "&Send";
            mSendButton.Click += mSendButton_Click;
            // 
            // mOnTopCheckBox
            // 
            mOnTopCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            mOnTopCheckBox.Location = new Point(536, 0);
            mOnTopCheckBox.Name = "mOnTopCheckBox";
            mOnTopCheckBox.Size = new Size(72, 24);
            mOnTopCheckBox.TabIndex = 6;
            mOnTopCheckBox.Text = "&On top";
            mOnTopCheckBox.CheckedChanged += mOnTopCheckBox_CheckedChanged;
            // 
            // mEchoInputCheckBox
            // 
            mEchoInputCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            mEchoInputCheckBox.Location = new Point(536, 24);
            mEchoInputCheckBox.Name = "mEchoInputCheckBox";
            mEchoInputCheckBox.Size = new Size(88, 24);
            mEchoInputCheckBox.TabIndex = 7;
            mEchoInputCheckBox.Text = "&Echo input";
            // 
            // TeletypeForm
            // 
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new Size(616, 309);
            Controls.Add(mEchoInputCheckBox);
            Controls.Add(mOnTopCheckBox);
            Controls.Add(mSendButton);
            Controls.Add(mClearButton);
            Controls.Add(mStatusBar);
            Controls.Add(mPromptLabel);
            Controls.Add(mInputTextBox);
            Controls.Add(mOutputTextBox);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TeletypeForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Teletype";
            TopMost = true;
            Activated += activated;
            VisibleChanged += visibleChanged;
            FormClosing += formClosing;
            ((System.ComponentModel.ISupportInitialize)(mStatusBarPanel)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        void keyPress(object sender, KeyPressEventArgs args)
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

        public void ClearOutput()
		{
			if (mOutputTextBox.InvokeRequired)
			{
				mOutputTextBox.Invoke(new MethodInvoker(ClearOutput));
				return;
			}

			mOutputTextBox.Text = "";
		}

        void mOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = mOnTopCheckBox.Checked;
        }

        void outputAdded(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(outputAdded));
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

                if (Visible)
                {
                    mOutputTextBox.Select(mOutputTextBox.TextLength, 0);
                    mOutputTextBox.Focus();
                    mOutputTextBox.ScrollToCaret();
                    mInputTextBox.Focus();
                }
            }
        }

        void sendInput()
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

        void visibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
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
						mTeletypeDevice.OutputAdded += outputAdded;
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
