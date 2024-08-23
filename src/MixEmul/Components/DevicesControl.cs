using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Device;

namespace MixGui.Components
{
	public class DevicesControl : UserControl
	{
		private DeviceStatusControl[] deviceControls;
		private MixLib.Devices devices;
		private int lastSectionSize;
		private int maxSequenceSize;
		private readonly Label noDevicesLabel;
		private int sectionSize;
		private int sequenceSize;
		private LayoutStructure structure;
		private ToolTip toolTip;

		public event EventHandler<DeviceEventArgs> DeviceDoubleClick;

		public DevicesControl(MixLib.Devices devices = null, LayoutStructure layout = null)
		{
			this.structure = layout;

			this.noDevicesLabel = new Label
			{
				Location = new Point(0, 0),
				Name = "mNoDevicesLabel",
				Size = new Size(120, 16),
				TabIndex = 0,
				Text = "No devices connected",
				TextAlign = ContentAlignment.MiddleCenter
			};

			Name = "DevicesControl";
			Devices = devices;
		}

		private void AddDeviceControl(int index)
		{
			var orientation = GetOrientation();

			Breaks breaks = index == 0
				? Breaks.Sequence
				: this.structure != null
					? this.structure[index]
					: Breaks.None;

			var horizontalSpacing = GetHorizontalSpacing();
			var verticalSpacing = GetVerticalSpacing();

			if (index == 0)
			{
				this.sequenceSize = 0;
				this.sectionSize = 0;
				this.lastSectionSize = 0;
				this.maxSequenceSize = 0;
			}

			var controlLocation = new Point();
			DeviceStatusControl control = this.deviceControls[index];

			switch (breaks)
			{
				case Breaks.Sequence:
					if (orientation == Orientations.Horizontal)
					{
						controlLocation.X = this.lastSectionSize;
						controlLocation.Y = this.sequenceSize;
						this.sequenceSize += control.Height + verticalSpacing;
						this.sectionSize = Math.Max(this.sectionSize, this.lastSectionSize + control.Width + horizontalSpacing);
					}
					else
					{
						controlLocation.X = this.sequenceSize;
						controlLocation.Y = this.lastSectionSize;
						this.sequenceSize += control.Width + horizontalSpacing;
						this.sectionSize = Math.Max(this.sectionSize, this.lastSectionSize + control.Height + verticalSpacing);
					}
					break;

				case Breaks.Section:
					if (orientation == Orientations.Horizontal)
					{
						this.lastSectionSize = this.sectionSize;
						controlLocation.X = this.lastSectionSize;
						controlLocation.Y = 0;
						this.sequenceSize = control.Height + verticalSpacing;
						this.sectionSize = this.lastSectionSize + control.Width + horizontalSpacing;
					}
					else
					{
						this.lastSectionSize = this.sectionSize;
						controlLocation.X = 0;
						controlLocation.Y = this.lastSectionSize;
						this.sequenceSize = control.Width + horizontalSpacing;
						this.sectionSize = this.lastSectionSize + control.Height + verticalSpacing;
					}
					break;

				default:
					if (orientation == Orientations.Horizontal)
					{
						DeviceStatusControl previousControl = this.deviceControls[index - 1];
						controlLocation.X = previousControl.Left + previousControl.Width + horizontalSpacing;
						controlLocation.Y = previousControl.Top;
						this.sequenceSize = Math.Max(this.sequenceSize, controlLocation.Y + control.Height + verticalSpacing);
						this.sectionSize = Math.Max(this.sectionSize, controlLocation.X + control.Width + horizontalSpacing);
					}
					else
					{
						DeviceStatusControl previousControl = this.deviceControls[index - 1];
						controlLocation.X = previousControl.Left;
						controlLocation.Y = previousControl.Top + previousControl.Height + horizontalSpacing;
						this.sequenceSize = Math.Max(this.sequenceSize, controlLocation.X + control.Width + horizontalSpacing);
						this.sectionSize = Math.Max(this.sectionSize, controlLocation.Y + control.Height + verticalSpacing);
					}

					break;
			}

			this.maxSequenceSize = Math.Max(this.maxSequenceSize, this.sequenceSize);

			control.Location = controlLocation;
			Controls.Add(control);
		}

		private void AddDeviceControls()
		{
			SuspendLayout();
			Controls.Clear();

			if (Devices == null)
			{
				Controls.Add(this.noDevicesLabel);
			}
			else
			{
				for (int i = 0; i < this.deviceControls.Length; i++)
					AddDeviceControl(i);
			}

			ResumeLayout(false);
		}

		private int GetHorizontalSpacing()
			=> this.structure == null ? 0 : this.structure.HorizontalSpacing;

		private Orientations GetOrientation()
			=> this.structure == null ? Orientations.Horizontal : this.structure.Orientation;

		private int GetVerticalSpacing()
			=> this.structure == null ? 0 : this.structure.VerticalSpacing;

		public new void Update()
		{
			foreach (DeviceStatusControl control in this.deviceControls)
				control.Update();

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (DeviceStatusControl control in this.deviceControls)
				control.UpdateLayout();
		}

		public MixLib.Devices Devices
		{
			get => this.devices;
			set
			{
				if (this.devices != null)
					return;

				this.devices = value;
				if (this.devices != null)
				{
					this.deviceControls = new DeviceStatusControl[this.devices.Count];
					DeviceStatusControl control;
					foreach (MixDevice device in this.devices)
					{
						control = new DeviceStatusControl(device)
						{
							ToolTip = this.toolTip
						};

						control.DoubleClick += DevicesControl_DoubleClick;

						this.deviceControls[device.Id] = control;
					}
				}

				AddDeviceControls();
			}
		}

		private void DevicesControl_DoubleClick(object sender, EventArgs e)
			=> DeviceDoubleClick?.Invoke(this, new DeviceEventArgs(((DeviceStatusControl)sender).Device));

		public LayoutStructure Structure
		{
			get => this.structure;
			set
			{
				if (value != null)
				{
					this.structure = value;
					AddDeviceControls();
				}
			}
		}

		public enum Breaks
		{
			None = 0,
			Sequence,
			Section
		}

		public ToolTip ToolTip
		{
			get => this.toolTip;
			set
			{
				this.toolTip = value;

				if (this.deviceControls != null)
				{
					foreach (DeviceStatusControl control in this.deviceControls)
						control.ToolTip = value;
				}
			}
		}

		public class LayoutStructure(DevicesControl.Orientations orientation, int horizontalSpacing, int verticalSpacing)
	{
			private readonly SortedList<int, Breaks> mLayoutList = [];

			public Orientations Orientation { get; set; } = orientation;
			public int HorizontalSpacing { get; set; } = horizontalSpacing;
			public int VerticalSpacing { get; set; } = verticalSpacing;

			public LayoutStructure(Orientations orientation) : this(orientation, 0, 0) { }

			public Breaks this[int location]
			{
				get => !mLayoutList.TryGetValue(location, out var value) ? Breaks.None : value;
				set
				{
					if (mLayoutList.ContainsKey(location))
						mLayoutList.Remove(location);

					if (value != Breaks.None)
						mLayoutList.Add(location, value);
				}
			}
		}

		public enum Orientations
		{
			Horizontal,
			Vertical
		}
	}
}
