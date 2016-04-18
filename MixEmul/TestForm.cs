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
            textBox1 = new TextBox();
            label1 = new Label();
            mDeltaButton = new Button();
            mSummaButton = new Button();
            mPiButton = new Button();
            textBox2 = new TextBox();
            label2 = new Label();
            vScrollBar1 = new System.Windows.Forms.VScrollBar();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
                      | AnchorStyles.Left)
                      | AnchorStyles.Right);
            textBox1.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(0, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox1.Size = new Size(704, 212);
            textBox1.TabIndex = 0;
            textBox1.Text = "textBox1";
            // 
            // label1
            // 
            label1.Location = new Point(0, 408);
            label1.Name = "label1";
            label1.Size = new Size(88, 21);
            label1.TabIndex = 1;
            label1.Text = "Set clipboard to:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // mDeltaButton
            // 
            mDeltaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mDeltaButton.Location = new Point(88, 408);
            mDeltaButton.Name = "mDeltaButton";
            mDeltaButton.Size = new Size(18, 21);
            mDeltaButton.TabIndex = 2;
            mDeltaButton.Text = "d";
            mDeltaButton.Click += new EventHandler(clipboardButton_Click);
            // 
            // mSummaButton
            // 
            mSummaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mSummaButton.Location = new Point(105, 408);
            mSummaButton.Name = "mSummaButton";
            mSummaButton.Size = new Size(18, 21);
            mSummaButton.TabIndex = 3;
            mSummaButton.Text = "s";
            mSummaButton.Click += new EventHandler(clipboardButton_Click);
            // 
            // mPiButton
            // 
            mPiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mPiButton.Location = new Point(122, 408);
            mPiButton.Name = "mPiButton";
            mPiButton.Size = new Size(18, 21);
            mPiButton.TabIndex = 4;
            mPiButton.Text = "p";
            mPiButton.Click += new EventHandler(clipboardButton_Click);
            // 
            // textBox2
            // 
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Location = new Point(320, 408);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 13);
            textBox2.TabIndex = 5;
            textBox2.Text = "textBox2";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(267, 260);
            label2.Name = "label2";
            label2.Size = new Size(9, 13);
            label2.TabIndex = 6;
            label2.Text = "l";
            // 
            // vScrollBar1
            // 
            vScrollBar1.LargeChange = 1;
            vScrollBar1.Location = new Point(522, 277);
            vScrollBar1.Maximum = 0;
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(16, 80);
            vScrollBar1.TabIndex = 7;
            vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBar1_Scroll);
            // 
            // TestForm
            // 
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new Size(704, 429);
            Controls.Add(vScrollBar1);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(mPiButton);
            Controls.Add(mSummaButton);
            Controls.Add(mDeltaButton);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Name = "TestForm";
            Text = "TestForm";
            ResumeLayout(false);
            PerformLayout();

        }

      private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
      {

      }
    }
}
