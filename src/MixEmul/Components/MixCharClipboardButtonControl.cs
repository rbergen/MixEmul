using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;

namespace MixGui.Components
{
	public class MixCharClipboardButtonControl : UserControl
	{
		private readonly Button deltaButton;
		private readonly Button sigmaButton;
		private readonly Button piButton;

		public MixCharClipboardButtonControl()
		{
			this.deltaButton = new Button();
			this.sigmaButton = new Button();
			this.piButton = new Button();

			SuspendLayout();

			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.deltaButton.FlatStyle = FlatStyle.Flat;
			this.deltaButton.Font = font;
			this.deltaButton.Location = new Point(0, 0);
			this.deltaButton.Name = "mDeltaButton";
			this.deltaButton.Size = new Size(21, 21);
			this.deltaButton.TabIndex = 0;
			this.deltaButton.Text = "Δ";
			this.deltaButton.Click += ClipboardButton_Click;

			this.sigmaButton.FlatStyle = FlatStyle.Flat;
			this.sigmaButton.Font = font;
			this.sigmaButton.Location = new Point(this.deltaButton.Right - 1, 0);
			this.sigmaButton.Name = "mSigmaButton";
			this.sigmaButton.Size = this.deltaButton.Size;
			this.sigmaButton.TabIndex = 1;
			this.sigmaButton.Text = "Σ";
			this.sigmaButton.Click += ClipboardButton_Click;

			this.piButton.FlatStyle = FlatStyle.Flat;
			this.piButton.Font = font;
			this.piButton.Location = new Point(this.sigmaButton.Right - 1, 0);
			this.piButton.Name = "mPiButton";
			this.piButton.Size = this.deltaButton.Size;
			this.piButton.TabIndex = 2;
			this.piButton.Text = "Π";
			this.piButton.Click += ClipboardButton_Click;

			Controls.Add(this.deltaButton);
			Controls.Add(this.sigmaButton);
			Controls.Add(this.piButton);
			Size = new Size(this.piButton.Right, this.piButton.Height);
			Name = "MemoryEditor";
			ResumeLayout(false);
		}

		private void ClipboardButton_Click(object sender, EventArgs e)
			=> Clipboard.SetDataObject(((Button)sender).Text);

		public void UpdateLayout()
		{
			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			this.deltaButton.Font = font;
			this.sigmaButton.Font = font;
			this.piButton.Font = font;
		}
	}
}
