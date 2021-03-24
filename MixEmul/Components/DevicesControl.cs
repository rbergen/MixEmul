using MixGui.Events;
using MixLib.Device;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class DevicesControl : UserControl
	{
		private DeviceStatusControl[] mDeviceControls;
		private MixLib.Devices mDevices;
		private int mLastSectionSize;
		private int mMaxSequenceSize;
		private readonly Label mNoDevicesLabel;
		private int mSectionSize;
		private int mSequenceSize;
		private LayoutStructure mStructure;
		private ToolTip mToolTip;

		public event EventHandler<DeviceEventArgs> DeviceDoubleClick;

		public DevicesControl(MixLib.Devices devices = null, LayoutStructure layout = null)
		{
			mStructure = layout;

			mNoDevicesLabel = new Label
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
			Breaks breaks = mStructure != null && index > 0 ? mStructure[index] : Breaks.None;

			var horizontalSpacing = GetHorizontalSpacing();
			var verticalSpacing = GetVerticalSpacing();

			if (index == 0)
			{
				mSequenceSize = 0;
				mSectionSize = 0;
				mLastSectionSize = 0;
				mMaxSequenceSize = 0;
			}

			var controlLocation = new Point();
			DeviceStatusControl control = mDeviceControls[index];

			if (index == 0 || breaks == Breaks.Sequence)
			{
				if (orientation == Orientations.Horizontal)
				{
					controlLocation.X = mLastSectionSize;
					controlLocation.Y = mSequenceSize;
					mSequenceSize += control.Height + verticalSpacing;
					mSectionSize = Math.Max(mSectionSize, (mLastSectionSize + control.Width) + horizontalSpacing);
				}
				else
				{
					controlLocation.X = mSequenceSize;
					controlLocation.Y = mLastSectionSize;
					mSequenceSize += control.Width + horizontalSpacing;
					mSectionSize = Math.Max(mSectionSize, (mLastSectionSize + control.Height) + verticalSpacing);
				}
			}
			else if (breaks == Breaks.Section)
			{
				if (orientation == Orientations.Horizontal)
				{
					mLastSectionSize = mSectionSize;
					controlLocation.X = mLastSectionSize;
					controlLocation.Y = 0;
					mSequenceSize = control.Height + verticalSpacing;
					mSectionSize = (mLastSectionSize + control.Width) + horizontalSpacing;
				}
				else
				{
					mLastSectionSize = mSectionSize;
					controlLocation.X = 0;
					controlLocation.Y = mLastSectionSize;
					mSequenceSize = control.Width + horizontalSpacing;
					mSectionSize = (mLastSectionSize + control.Height) + verticalSpacing;
				}
			}
			else if (orientation == Orientations.Horizontal)
			{
				DeviceStatusControl previousControl = mDeviceControls[index - 1];
				controlLocation.X = (previousControl.Left + previousControl.Width) + horizontalSpacing;
				controlLocation.Y = previousControl.Top;
				mSequenceSize = Math.Max(mSequenceSize, (controlLocation.Y + control.Height) + verticalSpacing);
				mSectionSize = Math.Max(mSectionSize, (controlLocation.X + control.Width) + horizontalSpacing);
			}
			else
			{
				DeviceStatusControl previousControl = mDeviceControls[index - 1];
				controlLocation.X = previousControl.Left;
				controlLocation.Y = (previousControl.Top + previousControl.Height) + horizontalSpacing;
				mSequenceSize = Math.Max(mSequenceSize, (controlLocation.X + control.Width) + horizontalSpacing);
				mSectionSize = Math.Max(mSectionSize, (controlLocation.Y + control.Height) + verticalSpacing);
			}

			mMaxSequenceSize = Math.Max(mMaxSequenceSize, mSequenceSize);

			control.Location = controlLocation;
			Controls.Add(control);
		}

		private void AddDeviceControls()
		{
			SuspendLayout();
			Controls.Clear();

			if (Devices == null)
				Controls.Add(mNoDevicesLabel);

			else
			{
				for (int i = 0; i < mDeviceControls.Length; i++)
					AddDeviceControl(i);
			}

			ResumeLayout(false);
		}

		private int GetHorizontalSpacing() 
			=> mStructure == null ? 0 : mStructure.HorizontalSpacing;

		private Orientations GetOrientation() 
			=> mStructure == null ? Orientations.Horizontal : mStructure.Orientation;

		private int GetVerticalSpacing() 
			=> mStructure == null ? 0 : mStructure.VerticalSpacing;

		public new void Update()
		{
			foreach (DeviceStatusControl control in mDeviceControls)
				control.Update();

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (DeviceStatusControl control in mDeviceControls)
			{
				control.UpdateLayout();
			}
		}

		public MixLib.Devices Devices
		{
			get => mDevices;
			set
			{
				if (mDevices == null)
				{
					mDevices = value;
					if (mDevices != null)
					{
						mDeviceControls = new DeviceStatusControl[mDevices.Count];
						DeviceStatusControl control;
						foreach (MixDevice device in mDevices)
						{
							control = new DeviceStatusControl(device)
							{
								ToolTip = mToolTip
							};

							control.DoubleClick += DevicesControl_DoubleClick;

							mDeviceControls[device.Id] = control;
						}
					}

					AddDeviceControls();
				}
			}
		}

		private void DevicesControl_DoubleClick(object sender, EventArgs e)
		{
			DeviceDoubleClick?.Invoke(this, new DeviceEventArgs(((DeviceStatusControl)sender).Device));
		}

		public LayoutStructure Structure
		{
			get => mStructure;
			set
			{
				if (value != null)
				{
					mStructure = value;
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
			get => mToolTip;
			set
			{
				mToolTip = value;

				if (mDeviceControls != null)
				{
					foreach (DeviceStatusControl control in mDeviceControls)
						control.ToolTip = value;
				}
			}
		}

		public class LayoutStructure
		{
			private readonly SortedList<int, DevicesControl.Breaks> mLayoutList;

			public DevicesControl.Orientations Orientation { get; set; }
			public int HorizontalSpacing { get; set; }
			public int VerticalSpacing { get; set; }

			public LayoutStructure(DevicesControl.Orientations orientation) : this(orientation, 0, 0) {}

			public LayoutStructure(DevicesControl.Orientations orientation, int horizontalSpacing, int verticalSpacing)
			{
				mLayoutList = new SortedList<int, DevicesControl.Breaks>();
				Orientation = orientation;
				HorizontalSpacing = horizontalSpacing;
				VerticalSpacing = verticalSpacing;
			}

			public DevicesControl.Breaks this[int location]
			{
				get => !mLayoutList.ContainsKey(location) ? DevicesControl.Breaks.None : mLayoutList[location];
				set
				{
					if (mLayoutList.ContainsKey(location))
						mLayoutList.Remove(location);

					if (value != DevicesControl.Breaks.None)
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
