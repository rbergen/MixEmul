using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Settings;
using MixLib.Device;

namespace MixGui.Components
{
	public class DeviceStatusControl : UserControl
	{
		private readonly ContextMenuStrip _contextMenuStrip;
		private readonly ToolStripSeparator _contextSeparator;
		private MixDevice _device;
		private readonly Label _indexLabel;
		private readonly ToolStripMenuItem _inputMenuItem;
		private readonly ToolStripMenuItem _outputMenuItem;
		private readonly ToolStripMenuItem _resetMenuItem;
		private readonly Label _statusLabel;

		public ToolTip ToolTip { get; set; }

		public DeviceStatusControl() : this(null) { }

		public DeviceStatusControl(MixDevice device)
		{
			_device = device;

			_indexLabel = new Label();
			_statusLabel = new Label();
			_contextMenuStrip = new ContextMenuStrip();
			_inputMenuItem = new ToolStripMenuItem();
			_outputMenuItem = new ToolStripMenuItem();
			_contextSeparator = new ToolStripSeparator();
			_resetMenuItem = new ToolStripMenuItem();

			SuspendLayout();

			_indexLabel.Location = new Point(0, 0);
			_indexLabel.Name = "mIndexLabel";
			_indexLabel.Size = new Size(20, 16);
			_indexLabel.TabIndex = 0;
			_indexLabel.Text = "00:";
			_indexLabel.TextAlign = ContentAlignment.MiddleRight;
			_indexLabel.DoubleClick += Control_DoubleClick;

			_statusLabel.BorderStyle = BorderStyle.FixedSingle;
			_statusLabel.Location = new Point(20, 0);
			_statusLabel.Name = "mStatusLabel";
			_statusLabel.Size = new Size(32, 16);
			_statusLabel.TabIndex = 1;
			_statusLabel.Text = "";
			_statusLabel.TextAlign = ContentAlignment.MiddleCenter;
			_statusLabel.DoubleClick += Control_DoubleClick;

			_inputMenuItem.Text = "Input device";

			_outputMenuItem.Text = "Output device";

			_resetMenuItem.Text = "Reset";
			_resetMenuItem.Click += ResetMenuItem_Click;

			_contextMenuStrip.Items.AddRange(new ToolStripItem[] { _inputMenuItem, _outputMenuItem, _contextSeparator, _resetMenuItem });
			ContextMenuStrip = _contextMenuStrip;

			Controls.Add(_indexLabel);
			Controls.Add(_statusLabel);
			Name = "DeviceStatusControl";
			Size = new Size(_indexLabel.Width + _statusLabel.Width, 16);
			ResumeLayout(false);

			UpdateLayout();
		}

		private void Control_DoubleClick(object sender, EventArgs e)
			=> OnDoubleClick(e);

		private void ResetMenuItem_Click(object sender, EventArgs e)
		{
			if (_device != null)
			{
				_device.Reset();
				Update();
			}
		}

		public new void Update()
		{
			if (Device == null)
			{
				SuspendLayout();

				_indexLabel.Text = "";
				_statusLabel.BackColor = Color.Gray;
				_statusLabel.Text = "";
				ToolTip?.SetToolTip(_statusLabel, "No device connected");

				_inputMenuItem.Checked = false;
				_outputMenuItem.Checked = false;

				ResumeLayout();
			}
			else
			{
				SuspendLayout();

				_indexLabel.Text = Device.Id + ":";
				_statusLabel.BackColor = GuiSettings.GetColor(Device.Busy ? GuiSettings.DeviceBusy : GuiSettings.DeviceIdle);
				_statusLabel.Text = Device.ShortName;
				ToolTip?.SetToolTip(_statusLabel, Device.StatusDescription);
				_inputMenuItem.Checked = Device.SupportsInput;
				_outputMenuItem.Checked = Device.SupportsOutput;

				ResumeLayout();
			}
		}

		public void UpdateLayout()
		{
			_statusLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			Update();
		}

		public MixDevice Device
		{
			get => _device;
			set
			{
				if (_device == null)
				{
					_device = value;
					Update();
				}
			}
		}
	}
}
