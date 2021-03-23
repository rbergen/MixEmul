using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class LongValueToolStripTextBox : ToolStripControlHost
	{
		public LongValueToolStripTextBox(IContainer container)
			: this()
		{
			container.Add(this);
		}

		public LongValueToolStripTextBox()
			: this(long.MinValue, long.MaxValue, 0L)
		{
		}

		public LongValueToolStripTextBox(long minValue, long maxValue)
			: this(minValue, maxValue, 0L)
		{
		}

		public LongValueToolStripTextBox(long minValue, long maxValue, long value)
			: base(new LongValueTextBox(minValue, maxValue, value))
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
			get => ((LongValueTextBox)Control).LongValue;
			set => ((LongValueTextBox)Control).LongValue = value;
		}

		public long MaxValue
		{
			get => ((LongValueTextBox)Control).MaxValue;
			set => ((LongValueTextBox)Control).MaxValue = value;
		}

		public long MinValue
		{
			get => ((LongValueTextBox)Control).MinValue;
			set => ((LongValueTextBox)Control).MinValue = value;
		}

		public bool ClearZero
		{
			get => ((LongValueTextBox)Control).ClearZero;
			set => ((LongValueTextBox)Control).ClearZero = value;
		}

		public override string Text
		{
			get => ((LongValueTextBox)Control).Text;
			set => ((LongValueTextBox)Control).Text = value;
		}

	}
}
