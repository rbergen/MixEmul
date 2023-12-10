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
		private Step currentStep;
		private readonly List<Step> steps;

		public event EventHandler ValueChanged;

		public ToolStripCycleButton(IContainer container) : this() 
			=> container.Add(this);

		public ToolStripCycleButton()
			: base(new Button { Text = string.Empty, FlatStyle = FlatStyle.Flat, Height = 21, Padding = new Padding(0), TextAlign = ContentAlignment.TopCenter })
		{
			InitializeComponent();
			this.steps = [];
			AutoSize = false;
			Size = Control.Size;
			Control.SizeChanged += Control_SizeChanged;
			Control.Click += Control_Click;
			Control.Paint += ToolStripCycleButton_Paint;
			Text = string.Empty;
		}

		protected void OnValueChanged(EventArgs e) 
			=> ValueChanged?.Invoke(this, e);

		private void ToolStripCycleButton_Paint(object sender, PaintEventArgs e)
		{
			if (string.IsNullOrEmpty(Text))
				e.Graphics.DrawString(this.currentStep == null ? "<No steps>" : this.currentStep.Text, Control.Font, new SolidBrush(Control.ForeColor), 1, 1);
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (this.currentStep == null || this.currentStep.NextStep == null)
				return;

			SetCurrentStep(this.currentStep.NextStep);
		}

		private void Control_SizeChanged(object sender, EventArgs e) 
			=> Size = Control.Size;

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
				throw new ArgumentNullException(nameof(step), "step and its Value may not be null");

			if (this.steps.Any(s => s.Value is IComparable comparable ? comparable.CompareTo(step.Value) == 0 : s.Value.Equals(step.Value)))
				throw new ArgumentException($"another step with Value {step.Value} already exists");

			this.steps.Add(step);

			if (this.currentStep == null)
				SetCurrentStep(step);
		}

		public object Value
		{
			get => this.currentStep?.Value;
			set
			{
				if (value == null)
					return;

				var step = this.steps.FirstOrDefault(s => s.Value is IComparable comparable ? comparable.CompareTo(value) == 0 : s.Value.Equals(value)) 
					?? throw new ArgumentException($"no step with Value {value} exists");
		    
				SetCurrentStep(step);
			}
		}

		private void SetCurrentStep(Step step)
		{
			if (this.currentStep == step)
				return;

			this.currentStep = step;
			Invalidate();
			OnValueChanged(new EventArgs());
		}

		public class Step(object value, string text, Step nextStep)
	{
	  public object Value { get; private set; } = value;
	  public string Text { get; private set; } = text;
	  public Step NextStep { get; set; } = nextStep;

	  public Step(object value, string text) : this(value, text, null) { }

		public Step(object value) : this(value, value.ToString(), null) { }

		public Step(object value, Step nextStep) : this(value, value.ToString(), nextStep) { }
	}
  }
}
