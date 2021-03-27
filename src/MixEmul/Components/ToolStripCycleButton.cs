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
		private Step _currentStep;
		private readonly List<Step> _steps;

		public event EventHandler ValueChanged;

		public ToolStripCycleButton(IContainer container) : this() 
			=> container.Add(this);

		public ToolStripCycleButton()
			: base(new Button { Text = "", FlatStyle = FlatStyle.Flat, Height = 21, Padding = new Padding(0), TextAlign = ContentAlignment.TopCenter })
		{
			InitializeComponent();
			_steps = new List<Step>();
			AutoSize = false;
			Size = Control.Size;
			Control.SizeChanged += Control_SizeChanged;
			Control.Click += Control_Click;
			Control.Paint += ToolStripCycleButton_Paint;
			Text = "";
		}

		protected void OnValueChanged(EventArgs e) 
			=> ValueChanged?.Invoke(this, e);

		private void ToolStripCycleButton_Paint(object sender, PaintEventArgs e)
		{
			if (string.IsNullOrEmpty(Text))
				e.Graphics.DrawString(_currentStep == null ? "<No steps>" : _currentStep.Text, Control.Font, new SolidBrush(Control.ForeColor), 1, 1);
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (_currentStep == null || _currentStep.NextStep == null)
				return;

			SetCurrentStep(_currentStep.NextStep);
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

			if (_steps.Any(s => s.Value is IComparable comparable ? comparable.CompareTo(step.Value) == 0 : s.Value.Equals(step.Value)))
				throw new ArgumentException($"another step with Value {step.Value} already exists");

			_steps.Add(step);

			if (_currentStep == null)
				SetCurrentStep(step);
		}

		public object Value
		{
			get => _currentStep?.Value;
			set
			{
				if (value == null)
					return;

				var step = _steps.FirstOrDefault(s => s.Value is IComparable comparable ? comparable.CompareTo(value) == 0 : s.Value.Equals(value));

				if (step == null)
					throw new ArgumentException($"no step with Value {value} exists");

				SetCurrentStep(step);
			}
		}

		private void SetCurrentStep(Step step)
		{
			if (_currentStep == step)
				return;

			_currentStep = step;
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
