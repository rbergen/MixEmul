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
			this.AutoSize = false;
			this.Size = Control.Size;
			Control.SizeChanged += new EventHandler(Control_SizeChanged);
		}

		void Control_SizeChanged(object sender, EventArgs e)
		{
			this.Size = Control.Size;
		}

		public LongValueToolStripTextBox(LongValueTextBox innerBox)
			: base(innerBox)
		{
			InitializeComponent();
			this.AutoSize = false;
			this.Size = innerBox.Size;
			Control.SizeChanged += new EventHandler(Control_SizeChanged);
		}

		public LongValueTextBox LongValueTextBox
		{
			get
			{
				return (LongValueTextBox)Control;
			}
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

		public bool SupportSign
		{
			get
			{
				return ((LongValueTextBox)Control).SupportSign;
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
