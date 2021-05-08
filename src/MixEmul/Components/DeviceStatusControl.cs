using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Device;

namespace MixGui.Components
{
	public class DeviceStatusControl : UserControl
	{
		private readonly ContextMenuStrip contextMenuStrip;
		private readonly ToolStripSeparator contextSeparator;
		private MixDevice device;
		private readonly Label indexLabel;
		private readonly ToolStripMenuItem inputMenuItem;
		private readonly ToolStripMenuItem outputMenuItem;
		private readonly ToolStripMenuItem resetMenuItem;
		private readonly Label statusLabel;

		public ToolTip ToolTip { get; set; }

		public DeviceStatusControl() : this(null) { }

		public DeviceStatusControl(MixDevice device)
		{
			this.device = device;

			this.indexLabel = new Label();
			this.statusLabel = new Label();
			this.contextMenuStrip = new ContextMenuStrip();
			this.inputMenuItem = new ToolStripMenuItem();
			this.outputMenuItem = new ToolStripMenuItem();
			this.contextSeparator = new ToolStripSeparator();
			this.resetMenuItem = new ToolStripMenuItem();

			SuspendLayout();

			this.indexLabel.Location = new Point(0, 0);
			this.indexLabel.Name = "mIndexLabel";
			this.indexLabel.Size = new Size(20, 16);
			this.indexLabel.TabIndex = 0;
			this.indexLabel.Text = "00:";
			this.indexLabel.TextAlign = ContentAlignment.MiddleRight;
			this.indexLabel.DoubleClick += Control_DoubleClick;

			this.statusLabel.BorderStyle = BorderStyle.FixedSingle;
			this.statusLabel.Location = new Point(20, 0);
			this.statusLabel.Name = "mStatusLabel";
			this.statusLabel.Size = new Size(32, 16);
			this.statusLabel.TabIndex = 1;
			this.statusLabel.Text = "";
			this.statusLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.statusLabel.DoubleClick += Control_DoubleClick;

			this.inputMenuItem.Text = "Input device";

			this.outputMenuItem.Text = "Output device";

			this.resetMenuItem.Text = "Reset";
			this.resetMenuItem.Click += ResetMenuItem_Click;

			this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { this.inputMenuItem, this.outputMenuItem, this.contextSeparator, this.resetMenuItem });
			ContextMenuStrip = this.contextMenuStrip;

			Controls.Add(this.indexLabel);
			Controls.Add(this.statusLabel);
			Name = "DeviceStatusControl";
			Size = new Size(this.indexLabel.Width + this.statusLabel.Width, 16);
			ResumeLayout(false);

			UpdateLayout();
		}

		private void Control_DoubleClick(object sender, EventArgs e)
			=> OnDoubleClick(e);

		private void ResetMenuItem_Click(object sender, EventArgs e)
		{
			if (this.device != null)
			{
				this.device.Reset();
				Update();
			}
		}

		public new void Update()
		{
			if (Device == null)
			{
				SuspendLayout();

				this.indexLabel.Text = "";
				this.statusLabel.BackColor = Color.Gray;
				this.statusLabel.Text = "";
				ToolTip?.SetToolTip(this.statusLabel, "No device connected");

				this.inputMenuItem.Checked = false;
				this.outputMenuItem.Checked = false;

				ResumeLayout();
			}
			else
			{
				SuspendLayout();

				this.indexLabel.Text = Device.Id + ":";
				this.statusLabel.BackColor = GuiSettings.GetColor(Device.Busy ? GuiSettings.DeviceBusy : GuiSettings.DeviceIdle);
				this.statusLabel.Text = Device.ShortName;
				ToolTip?.SetToolTip(this.statusLabel, Device.StatusDescription);
				this.inputMenuItem.Checked = Device.SupportsInput;
				this.outputMenuItem.Checked = Device.SupportsOutput;

				ResumeLayout();
			}
		}

		public void UpdateLayout()
		{
			this.statusLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			Update();
		}

		public MixDevice Device
		{
			get => this.device;
			set
			{
				if (this.device == null)
				{
					this.device = value;
					Update();
				}
			}
		}
	}
}
