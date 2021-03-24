﻿using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;

namespace MixGui.Components
{
	public class MixCharClipboardButtonControl : UserControl
	{
		private readonly Button _deltaButton;
		private readonly Button _sigmaButton;
		private readonly Button _piButton;

		public MixCharClipboardButtonControl()
		{
			_deltaButton = new Button();
			_sigmaButton = new Button();
			_piButton = new Button();

			SuspendLayout();

			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_deltaButton.FlatStyle = FlatStyle.Flat;
			_deltaButton.Font = font;
			_deltaButton.Location = new Point(0, 0);
			_deltaButton.Name = "mDeltaButton";
			_deltaButton.Size = new Size(21, 21);
			_deltaButton.TabIndex = 0;
			_deltaButton.Text = "Δ";
			_deltaButton.Click += ClipboardButton_Click;

			_sigmaButton.FlatStyle = FlatStyle.Flat;
			_sigmaButton.Font = font;
			_sigmaButton.Location = new Point(_deltaButton.Right - 1, 0);
			_sigmaButton.Name = "mSigmaButton";
			_sigmaButton.Size = _deltaButton.Size;
			_sigmaButton.TabIndex = 1;
			_sigmaButton.Text = "Σ";
			_sigmaButton.Click += ClipboardButton_Click;

			_piButton.FlatStyle = FlatStyle.Flat;
			_piButton.Font = font;
			_piButton.Location = new Point(_sigmaButton.Right - 1, 0);
			_piButton.Name = "mPiButton";
			_piButton.Size = _deltaButton.Size;
			_piButton.TabIndex = 2;
			_piButton.Text = "Π";
			_piButton.Click += ClipboardButton_Click;

			Controls.Add(_deltaButton);
			Controls.Add(_sigmaButton);
			Controls.Add(_piButton);
			Size = new Size(_piButton.Right, _piButton.Height);
			Name = "MemoryEditor";
			ResumeLayout(false);
		}

		private void ClipboardButton_Click(object sender, EventArgs e)
			=> Clipboard.SetDataObject(((Button)sender).Text);

		public void UpdateLayout()
		{
			var font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			_deltaButton.Font = font;
			_sigmaButton.Font = font;
			_piButton.Font = font;
		}
	}
}
