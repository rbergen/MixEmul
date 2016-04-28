using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class LongValueToolStripTextBox : ToolStripControlHost
	{
		public LongValueToolStripTextBox()
			: this(true, long.MinValue, long.MaxValue, 0L)
		{
		}

		public LongValueToolStripTextBox(IContainer container)
			: this()
		{
			container.Add(this);
		}

		public LongValueToolStripTextBox(bool supportsSign)
			: this(supportsSign, long.MinValue, long.MaxValue, 0L)
		{
		}

		public LongValueToolStripTextBox(bool supportsSign, long minValue, long maxValue)
			: this(supportsSign, minValue, maxValue, 0L)
		{
		}

		public LongValueToolStripTextBox(bool supportsSign, long minValue, long maxValue, long value)
			: base(new LongValueTextBox(supportsSign, minValue, maxValue, value))
		{
			InitializeComponent();
            AutoSize = false;
            Size = Control.Size;
			Control.SizeChanged += Control_SizeChanged;
		}

		public LongValueToolStripTextBox(LongValueTextBox innerBox) : base(innerBox)
		{
			InitializeComponent();
            AutoSize = false;
            Size = innerBox.Size;
			Control.SizeChanged += Control_SizeChanged;
		}

		public LongValueTextBox LongValueTextBox => (LongValueTextBox)Control;

        public bool SupportSign => ((LongValueTextBox)Control).SupportSign;

        void Control_SizeChanged(object sender, EventArgs e)
        {
            Size = Control.Size;
        }

        public long LongValue
		{
			get
			{
				return ((LongValueTextBox)Control).LongValue;
			}
			set
			{
				((LongValueTextBox)Control).LongValue = value;
			}
		}

		public long MaxValue
		{
			get
			{
				return ((LongValueTextBox)Control).MaxValue;
			}
			set
			{
				((LongValueTextBox)Control).MaxValue = value;
			}
		}

		public long MinValue
		{
			get
			{
				return ((LongValueTextBox)Control).MinValue;
			}
			set
			{
				((LongValueTextBox)Control).MinValue = value;
			}
		}

		public bool ClearZero
		{
			get
			{
				return ((LongValueTextBox)Control).ClearZero;
			}
			set
			{
				((LongValueTextBox)Control).ClearZero = value;
			}
		}

		public override string Text
		{
			get
			{
				return ((LongValueTextBox)Control).Text;
			}
			set
			{
				((LongValueTextBox)Control).Text = value;
			}
		}

	}
}
