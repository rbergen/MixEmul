using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MixGui.Components
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip |
									   ToolStripItemDesignerAvailability.StatusStrip)]
	public partial class ToolStripCycleButton : ToolStripControlHost
	{
        Step mCurrentStep;
        List<Step> mSteps;

        public event EventHandler ValueChanged;

		public ToolStripCycleButton(IContainer container) : this()
		{
			container.Add(this);
		}

		public ToolStripCycleButton()
			: base(new Button { Text = "", FlatStyle = FlatStyle.Flat, Height = 21, Padding = new Padding(0), TextAlign = ContentAlignment.TopCenter })
		{
			InitializeComponent();
			mSteps = new List<Step>();
            AutoSize = false;
            Size = Control.Size;
			Control.SizeChanged += Control_SizeChanged;
			Control.Click += Control_Click;
			Control.Paint += ToolStripCycleButton_Paint;
			Text = "";
		}

        protected void OnValueChanged(EventArgs e) => ValueChanged?.Invoke(this, e);

        void ToolStripCycleButton_Paint(object sender, PaintEventArgs e)
		{
			if (string.IsNullOrEmpty(Text))
			{
				var stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;

				e.Graphics.DrawString(mCurrentStep == null ? "<No steps>" : mCurrentStep.Text, Control.Font, new SolidBrush(Control.ForeColor), 1, 1);
			}
		}

		void Control_Click(object sender, EventArgs e)
		{
			if (mCurrentStep == null || mCurrentStep.NextStep == null) return;

			setCurrentStep(mCurrentStep.NextStep);
		}

		void Control_SizeChanged(object sender, EventArgs e)
		{
            Size = Control.Size;
		}

		public Step AddStep(object value)
		{
			var step = new Step(value);

			AddStep(step);

			return step;
		}

		public Step AddStep(object value, string text)
		{
			var step = new Step(value, text);

			AddStep(step);

			return step;
		}

		public Step AddStep(object value, string text, Step nextStep)
		{
			var step = new Step(value, text, nextStep);

			AddStep(step);

			return step;
		}

		public Step AddStep(object value, Step nextStep)
		{
			var step = new Step(value, nextStep);

			AddStep(step);

			return step;
		}

		public void AddStep(Step step)
		{
			if (step == null || step.Value == null)
			{
				throw new ArgumentNullException(nameof(step), "step and its Value may not be null");
			}

			if (mSteps.Any(s => s.Value is IComparable ? ((IComparable)s.Value).CompareTo(step.Value) == 0 : s.Value.Equals(step.Value)))
			{
				throw new ArgumentException(string.Format("another step with Value {0} already exists", step.Value));
			}

			mSteps.Add(step);

			if (mCurrentStep == null) setCurrentStep(step);
		}

		public object Value
		{
			get
			{
				return mCurrentStep == null ? null : mCurrentStep.Value;
			}
			set
			{
				if (value == null) return;

				Step step = mSteps.FirstOrDefault(s => s.Value is IComparable ? ((IComparable)s.Value).CompareTo(value) == 0 : s.Value.Equals(value));

				if (step == null)
				{
					throw new ArgumentException(string.Format("no step with Value {0} exists", value));
				}

				setCurrentStep(step);
			}
		}

        void setCurrentStep(Step step)
        {
            if (mCurrentStep == step) return;

            mCurrentStep = step;
            Invalidate();
            OnValueChanged(new EventArgs());
        }

        public class Step
		{
            public object Value { get; private set; }
            public string Text { get; private set; }
            public Step NextStep { get; set; }

            public Step(object value, string text) : this(value, text, null) { }

			public Step(object value) : this(value, value.ToString(), null) { }

			public Step(object value, Step nextStep) : this(value, value.ToString(), nextStep) { }

			public Step(object value, string text, Step nextStep)
			{
				Value = value;
				Text = text;
				NextStep = nextStep;
			}
		}
	}
}
