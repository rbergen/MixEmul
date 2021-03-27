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
		private DeviceStatusControl[] _deviceControls;
		private MixLib.Devices _devices;
		private int _lastSectionSize;
		private int _maxSequenceSize;
		private readonly Label _noDevicesLabel;
		private int _sectionSize;
		private int _sequenceSize;
		private LayoutStructure _structure;
		private ToolTip _toolTip;

		public event EventHandler<DeviceEventArgs> DeviceDoubleClick;

		public DevicesControl(MixLib.Devices devices = null, LayoutStructure layout = null)
		{
			_structure = layout;

			_noDevicesLabel = new Label
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
				: _structure != null
					? _structure[index]
					: Breaks.None;

			var horizontalSpacing = GetHorizontalSpacing();
			var verticalSpacing = GetVerticalSpacing();

			if (index == 0)
			{
				_sequenceSize = 0;
				_sectionSize = 0;
				_lastSectionSize = 0;
				_maxSequenceSize = 0;
			}

			var controlLocation = new Point();
			DeviceStatusControl control = _deviceControls[index];

			switch (breaks)
			{
				case Breaks.Sequence:
					if (orientation == Orientations.Horizontal)
					{
						controlLocation.X = _lastSectionSize;
						controlLocation.Y = _sequenceSize;
						_sequenceSize += control.Height + verticalSpacing;
						_sectionSize = Math.Max(_sectionSize, _lastSectionSize + control.Width + horizontalSpacing);
					}
					else
					{
						controlLocation.X = _sequenceSize;
						controlLocation.Y = _lastSectionSize;
						_sequenceSize += control.Width + horizontalSpacing;
						_sectionSize = Math.Max(_sectionSize, _lastSectionSize + control.Height + verticalSpacing);
					}
					break;

				case Breaks.Section:
					if (orientation == Orientations.Horizontal)
					{
						_lastSectionSize = _sectionSize;
						controlLocation.X = _lastSectionSize;
						controlLocation.Y = 0;
						_sequenceSize = control.Height + verticalSpacing;
						_sectionSize = _lastSectionSize + control.Width + horizontalSpacing;
					}
					else
					{
						_lastSectionSize = _sectionSize;
						controlLocation.X = 0;
						controlLocation.Y = _lastSectionSize;
						_sequenceSize = control.Width + horizontalSpacing;
						_sectionSize = _lastSectionSize + control.Height + verticalSpacing;
					}
					break;

				default:
					if (orientation == Orientations.Horizontal)
					{
						DeviceStatusControl previousControl = _deviceControls[index - 1];
						controlLocation.X = previousControl.Left + previousControl.Width + horizontalSpacing;
						controlLocation.Y = previousControl.Top;
						_sequenceSize = Math.Max(_sequenceSize, controlLocation.Y + control.Height + verticalSpacing);
						_sectionSize = Math.Max(_sectionSize, controlLocation.X + control.Width + horizontalSpacing);
					}
					else
					{
						DeviceStatusControl previousControl = _deviceControls[index - 1];
						controlLocation.X = previousControl.Left;
						controlLocation.Y = previousControl.Top + previousControl.Height + horizontalSpacing;
						_sequenceSize = Math.Max(_sequenceSize, controlLocation.X + control.Width + horizontalSpacing);
						_sectionSize = Math.Max(_sectionSize, controlLocation.Y + control.Height + verticalSpacing);
					}

					break;
			}

			_maxSequenceSize = Math.Max(_maxSequenceSize, _sequenceSize);

			control.Location = controlLocation;
			Controls.Add(control);
		}

		private void AddDeviceControls()
		{
			SuspendLayout();
			Controls.Clear();

			if (Devices == null)
			{
				Controls.Add(_noDevicesLabel);
			}
			else
			{
				for (int i = 0; i < _deviceControls.Length; i++)
					AddDeviceControl(i);
			}

			ResumeLayout(false);
		}

		private int GetHorizontalSpacing()
			=> _structure == null ? 0 : _structure.HorizontalSpacing;

		private Orientations GetOrientation()
			=> _structure == null ? Orientations.Horizontal : _structure.Orientation;

		private int GetVerticalSpacing()
			=> _structure == null ? 0 : _structure.VerticalSpacing;

		public new void Update()
		{
			foreach (DeviceStatusControl control in _deviceControls)
				control.Update();

			base.Update();
		}

		public void UpdateLayout()
		{
			foreach (DeviceStatusControl control in _deviceControls)
				control.UpdateLayout();
		}

		public MixLib.Devices Devices
		{
			get => _devices;
			set
			{
				if (_devices != null)
					return;

				_devices = value;
				if (_devices != null)
				{
					_deviceControls = new DeviceStatusControl[_devices.Count];
					DeviceStatusControl control;
					foreach (MixDevice device in _devices)
					{
						control = new DeviceStatusControl(device)
						{
							ToolTip = _toolTip
						};

						control.DoubleClick += DevicesControl_DoubleClick;

						_deviceControls[device.Id] = control;
					}
				}

				AddDeviceControls();
			}
		}

		private void DevicesControl_DoubleClick(object sender, EventArgs e)
			=> DeviceDoubleClick?.Invoke(this, new DeviceEventArgs(((DeviceStatusControl)sender).Device));

		public LayoutStructure Structure
		{
			get => _structure;
			set
			{
				if (value != null)
				{
					_structure = value;
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
			get => _toolTip;
			set
			{
				_toolTip = value;

				if (_deviceControls != null)
				{
					foreach (DeviceStatusControl control in _deviceControls)
						control.ToolTip = value;
				}
			}
		}

		public class LayoutStructure
		{
			private readonly SortedList<int, Breaks> mLayoutList;

			public Orientations Orientation { get; set; }
			public int HorizontalSpacing { get; set; }
			public int VerticalSpacing { get; set; }

			public LayoutStructure(Orientations orientation) : this(orientation, 0, 0) { }

			public LayoutStructure(Orientations orientation, int horizontalSpacing, int verticalSpacing)
			{
				mLayoutList = new SortedList<int, Breaks>();
				Orientation = orientation;
				HorizontalSpacing = horizontalSpacing;
				VerticalSpacing = verticalSpacing;
			}

			public Breaks this[int location]
			{
				get => !mLayoutList.ContainsKey(location) ? Breaks.None : mLayoutList[location];
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
