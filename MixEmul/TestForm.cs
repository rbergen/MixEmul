namespace MixGui
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    public class TestForm : Form
    {
        private Container components = null;
        private Label label1;
        private Button mDeltaButton;
        private Button mPiButton;
        private Button mSummaButton;
        private TextBox textBox1;
      private Label label2;
      private VScrollBar vScrollBar1;
        private CheckBox checkBox1;
        private TextBox textBox2;

        public TestForm()
        {
            InitializeComponent();
            mDeltaButton.Text = "Δ";
            mSummaButton.Text = "Σ";
            mPiButton.Text = "Π";
            StringBuilder builder = new StringBuilder("001: ");
            for (int i = 1; i < 0x7530; i++)
            {
                if ((i != 1) && ((i % 100) == 1))
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(i.ToString("D3"));
                    builder.Append(": ");
                }
                builder.Append((char) i);
            }
            textBox1.Text = builder.ToString();
        }

        private void clipboardButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(((Button) sender).Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mDeltaButton = new System.Windows.Forms.Button();
            this.mSummaButton = new System.Windows.Forms.Button();
            this.mPiButton = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(704, 212);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "textBox1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 408);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Set clipboard to:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mDeltaButton
            // 
            this.mDeltaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mDeltaButton.Location = new System.Drawing.Point(88, 408);
            this.mDeltaButton.Name = "mDeltaButton";
            this.mDeltaButton.Size = new System.Drawing.Size(18, 21);
            this.mDeltaButton.TabIndex = 2;
            this.mDeltaButton.Text = "d";
            this.mDeltaButton.Click += new System.EventHandler(this.clipboardButton_Click);
            // 
            // mSummaButton
            // 
            this.mSummaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSummaButton.Location = new System.Drawing.Point(105, 408);
            this.mSummaButton.Name = "mSummaButton";
            this.mSummaButton.Size = new System.Drawing.Size(18, 21);
            this.mSummaButton.TabIndex = 3;
            this.mSummaButton.Text = "s";
            this.mSummaButton.Click += new System.EventHandler(this.clipboardButton_Click);
            // 
            // mPiButton
            // 
            this.mPiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPiButton.Location = new System.Drawing.Point(122, 408);
            this.mPiButton.Name = "mPiButton";
            this.mPiButton.Size = new System.Drawing.Size(18, 21);
            this.mPiButton.TabIndex = 4;
            this.mPiButton.Text = "p";
            this.mPiButton.Click += new System.EventHandler(this.clipboardButton_Click);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(320, 408);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 13);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "textBox2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(9, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "l";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.LargeChange = 1;
            this.vScrollBar1.Location = new System.Drawing.Point(522, 277);
            this.vScrollBar1.Maximum = 0;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(16, 80);
            this.vScrollBar1.TabIndex = 7;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(576, 307);
            this.checkBox1.MaximumSize = new System.Drawing.Size(20, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(20, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            this.ClientSize = new System.Drawing.Size(704, 429);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.mPiButton);
            this.Controls.Add(this.mSummaButton);
            this.Controls.Add(this.mDeltaButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
      {

      }
    }
}
