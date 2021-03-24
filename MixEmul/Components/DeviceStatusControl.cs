using MixGui.Settings;
using MixLib.Device;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class DeviceStatusControl : UserControl
	{
		private readonly ContextMenuStrip mContextMenuStrip;
		private readonly ToolStripSeparator mContextSeparator;
		private MixDevice mDevice;
		private readonly Label mIndexLabel;
		private readonly ToolStripMenuItem mInputMenuItem;
		private readonly ToolStripMenuItem mOutputMenuItem;
		private readonly ToolStripMenuItem mResetMenuItem;
		private readonly Label mStatusLabel;

		public ToolTip ToolTip { get; set; }

		public DeviceStatusControl() : this(null) {}

		public DeviceStatusControl(MixDevice device)
		{
			mDevice = device;

			mIndexLabel = new Label();
			mStatusLabel = new Label();
			mContextMenuStrip = new ContextMenuStrip();
			mInputMenuItem = new ToolStripMenuItem();
			mOutputMenuItem = new ToolStripMenuItem();
			mContextSeparator = new ToolStripSeparator();
			mResetMenuItem = new ToolStripMenuItem();

			SuspendLayout();

			mIndexLabel.Location = new Point(0, 0);
			mIndexLabel.Name = "mIndexLabel";
			mIndexLabel.Size = new Size(20, 16);
			mIndexLabel.TabIndex = 0;
			mIndexLabel.Text = "00:";
			mIndexLabel.TextAlign = ContentAlignment.MiddleRight;
			mIndexLabel.DoubleClick += Control_DoubleClick;

			mStatusLabel.BorderStyle = BorderStyle.FixedSingle;
			mStatusLabel.Location = new Point(20, 0);
			mStatusLabel.Name = "mStatusLabel";
			mStatusLabel.Size = new Size(32, 16);
			mStatusLabel.TabIndex = 1;
			mStatusLabel.Text = "";
			mStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
			mStatusLabel.DoubleClick += Control_DoubleClick;

			mInputMenuItem.Text = "Input device";

			mOutputMenuItem.Text = "Output device";

			mResetMenuItem.Text = "Reset";
			mResetMenuItem.Click += ResetMenuItem_Click;

			mContextMenuStrip.Items.AddRange(new ToolStripItem[] { mInputMenuItem, mOutputMenuItem, mContextSeparator, mResetMenuItem });
			ContextMenuStrip = mContextMenuStrip;

			Controls.Add(mIndexLabel);
			Controls.Add(mStatusLabel);
			Name = "DeviceStatusControl";
			Size = new Size(mIndexLabel.Width + mStatusLabel.Width, 16);
			ResumeLayout(false);

			UpdateLayout();
		}

		private void Control_DoubleClick(object sender, EventArgs e) 
			=> OnDoubleClick(e);

		private void ResetMenuItem_Click(object sender, EventArgs e)
		{
			if (mDevice != null)
			{
				mDevice.Reset();
				Update();
			}
		}

		public new void Update()
		{
			if (Device == null)
			{
				SuspendLayout();

				mIndexLabel.Text = "";
				mStatusLabel.BackColor = Color.Gray;
				mStatusLabel.Text = "";
				ToolTip?.SetToolTip(mStatusLabel, "No device connected");

				mInputMenuItem.Checked = false;
				mOutputMenuItem.Checked = false;

				ResumeLayout();
			}
			else
			{
				SuspendLayout();

				mIndexLabel.Text = Device.Id + ":";
				mStatusLabel.BackColor = GuiSettings.GetColor(Device.Busy ? GuiSettings.DeviceBusy : GuiSettings.DeviceIdle);
				mStatusLabel.Text = Device.ShortName;
				ToolTip?.SetToolTip(mStatusLabel, Device.StatusDescription);
				mInputMenuItem.Checked = Device.SupportsInput;
				mOutputMenuItem.Checked = Device.SupportsOutput;

				ResumeLayout();
			}
		}

		public void UpdateLayout()
		{
			mStatusLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			Update();
		}

		public MixDevice Device
		{
			get => mDevice;
			set
			{
				if (mDevice == null)
				{
					mDevice = value;
					Update();
				}
			}
		}
	}
}
