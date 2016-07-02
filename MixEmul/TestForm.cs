namespace MixGui
{
    using System;
    using System.Text;
    using System.Windows.Forms;

    public class TestForm : Form
    {
        Label label1;
        Button mDeltaButton;
        Button mPiButton;
        Button mSummaButton;
        TextBox textBox1;
        Label label2;
        VScrollBar vScrollBar1;
        CheckBox checkBox1;
        TextBox textBox2;

        public TestForm()
        {
            InitializeComponent();
            mDeltaButton.Text = "Δ";
            mSummaButton.Text = "Σ";
            mPiButton.Text = "Π";
            var builder = new StringBuilder("001: ");
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

        void clipboardButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(((Button)sender).Text);
        }

        void InitializeComponent()
        {
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            mDeltaButton = new System.Windows.Forms.Button();
            mSummaButton = new System.Windows.Forms.Button();
            mPiButton = new System.Windows.Forms.Button();
            textBox2 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            vScrollBar1 = new System.Windows.Forms.VScrollBar();
            checkBox1 = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox1.Location = new System.Drawing.Point(0, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox1.Size = new System.Drawing.Size(704, 212);
            textBox1.TabIndex = 0;
            textBox1.Text = "textBox1";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(0, 408);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 21);
            label1.TabIndex = 1;
            label1.Text = "Set clipboard to:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mDeltaButton
            // 
            mDeltaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mDeltaButton.Location = new System.Drawing.Point(88, 408);
            mDeltaButton.Name = "mDeltaButton";
            mDeltaButton.Size = new System.Drawing.Size(18, 21);
            mDeltaButton.TabIndex = 2;
            mDeltaButton.Text = "d";
            mDeltaButton.Click += clipboardButton_Click;
            // 
            // mSummaButton
            // 
            mSummaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mSummaButton.Location = new System.Drawing.Point(105, 408);
            mSummaButton.Name = "mSummaButton";
            mSummaButton.Size = new System.Drawing.Size(18, 21);
            mSummaButton.TabIndex = 3;
            mSummaButton.Text = "s";
            mSummaButton.Click += clipboardButton_Click;
            // 
            // mPiButton
            // 
            mPiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            mPiButton.Location = new System.Drawing.Point(122, 408);
            mPiButton.Name = "mPiButton";
            mPiButton.Size = new System.Drawing.Size(18, 21);
            mPiButton.TabIndex = 4;
            mPiButton.Text = "p";
            mPiButton.Click += clipboardButton_Click;
            // 
            // textBox2
            // 
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox2.Location = new System.Drawing.Point(320, 408);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(100, 13);
            textBox2.TabIndex = 5;
            textBox2.Text = "textBox2";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(267, 260);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(9, 13);
            label2.TabIndex = 6;
            label2.Text = "l";
            // 
            // vScrollBar1
            // 
            vScrollBar1.LargeChange = 1;
            vScrollBar1.Location = new System.Drawing.Point(522, 277);
            vScrollBar1.Maximum = 0;
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new System.Drawing.Size(16, 80);
            vScrollBar1.TabIndex = 7;
            vScrollBar1.Scroll += vScrollBar1_Scroll;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(576, 307);
            checkBox1.MaximumSize = new System.Drawing.Size(20, 20);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(20, 17);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            ClientSize = new System.Drawing.Size(704, 429);
            Controls.Add(checkBox1);
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

        void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
