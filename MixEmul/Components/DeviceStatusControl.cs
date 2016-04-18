using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Device;

namespace MixGui.Components
{
	public class DeviceStatusControl : UserControl
	{
		private ContextMenu mContextMenu;
		private MenuItem mContextSeparator;
		private MixDevice mDevice;
		private Label mIndexLabel;
		private MenuItem mInputMenuItem;
		private MenuItem mOutputMenuItem;
		private MenuItem mResetMenuItem;
		private Label mStatusLabel;

		public ToolTip ToolTip { get; set; }

		public DeviceStatusControl()
			: this(null)
		{
		}

		public DeviceStatusControl(MixDevice device)
		{
			mDevice = device;

			mIndexLabel = new Label();
			mStatusLabel = new Label();
			mContextMenu = new ContextMenu();
			mInputMenuItem = new MenuItem();
			mOutputMenuItem = new MenuItem();
			mContextSeparator = new MenuItem();
			mResetMenuItem = new MenuItem();

			base.SuspendLayout();

			mIndexLabel.Location = new Point(0, 0);
			mIndexLabel.Name = "mIndexLabel";
			mIndexLabel.Size = new Size(20, 16);
			mIndexLabel.TabIndex = 0;
			mIndexLabel.Text = "00:";
			mIndexLabel.TextAlign = ContentAlignment.MiddleRight;
			mIndexLabel.DoubleClick += control_DoubleClick;

			mStatusLabel.BorderStyle = BorderStyle.FixedSingle;
			mStatusLabel.Location = new Point(20, 0);
			mStatusLabel.Name = "mStatusLabel";
			mStatusLabel.Size = new Size(32, 16);
			mStatusLabel.TabIndex = 1;
			mStatusLabel.Text = "";
			mStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
			mStatusLabel.DoubleClick += control_DoubleClick;

			mInputMenuItem.Index = 0;
			mInputMenuItem.Text = "Input device";

			mOutputMenuItem.Index = 1;
			mOutputMenuItem.Text = "Output device";

			mContextSeparator.Index = 2;
			mContextSeparator.Text = "-";

			mResetMenuItem.DefaultItem = true;
			mResetMenuItem.Index = 3;
			mResetMenuItem.Text = "Reset";
			mResetMenuItem.Click += new EventHandler(mResetMenuItem_Click);

			mContextMenu.MenuItems.AddRange(new MenuItem[] { mInputMenuItem, mOutputMenuItem, mContextSeparator, mResetMenuItem });
			ContextMenu = mContextMenu;

			base.Controls.Add(mIndexLabel);
			base.Controls.Add(mStatusLabel);
			base.Name = "DeviceStatusControl";
			base.Size = new Size(mIndexLabel.Width + mStatusLabel.Width, 16);
			base.ResumeLayout(false);

			UpdateLayout();
		}

        void control_DoubleClick(object sender, EventArgs e) => OnDoubleClick(e);

        private void mResetMenuItem_Click(object sender, EventArgs e)
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
				base.SuspendLayout();

				mIndexLabel.Text = "";
				mStatusLabel.BackColor = Color.Gray;
				mStatusLabel.Text = "";
				if (ToolTip != null)
				{
					ToolTip.SetToolTip(mStatusLabel, "No device connected");
				}

				mInputMenuItem.Checked = false;
				mOutputMenuItem.Checked = false;

				base.ResumeLayout();
			}
			else
			{
				base.SuspendLayout();

				mIndexLabel.Text = Device.Id + ":";
				mStatusLabel.BackColor = GuiSettings.GetColor(Device.Busy ? GuiSettings.DeviceBusy : GuiSettings.DeviceIdle);
				mStatusLabel.Text = Device.ShortName;
				if (ToolTip != null)
				{
					ToolTip.SetToolTip(mStatusLabel, Device.StatusDescription);
				}
				mInputMenuItem.Checked = Device.SupportsInput;
				mOutputMenuItem.Checked = Device.SupportsOutput;

				base.ResumeLayout();
			}
		}

		public void UpdateLayout()
		{
			mStatusLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			Update();
		}

		public MixDevice Device
		{
			get
			{
				return mDevice;
			}
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
