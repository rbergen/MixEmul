using MixGui.Events;
using MixGui.Settings;
using MixLib.Type;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class DeviceWordEditor : UserControl, IWordEditor, INavigableControl
	{
		readonly Label mWordIndexLabel;
		readonly FullWordEditor mFullWordEditor;
		int mWordIndex;
		IFullWord mDeviceWord;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public DeviceWordEditor()
			: this(null)
		{
		}

		public DeviceWordEditor(IFullWord deviceWord)
		{
			mDeviceWord = deviceWord;
			if (mDeviceWord == null)
			{
				mDeviceWord = new FullWord();
			}

			mFullWordEditor = new FullWordEditor(mDeviceWord);
			mWordIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl => this;

		public FieldTypes? FocusedField => mFullWordEditor.FocusedField;

		public int? CaretIndex => mFullWordEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
		{
			return mFullWordEditor.Focus(field, index);
		}

		public void Select(int start, int length)
		{
			mFullWordEditor.Select(start, length);
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			ValueChanged?.Invoke(this, args);
		}

		void MFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			OnValueChanged(args);
		}

		void InitializeComponent()
		{
			SuspendLayout();

			mWordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mWordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mWordIndexLabel.Location = new Point(0, 4);
			mWordIndexLabel.Name = "mWordIndexLabel";
			mWordIndexLabel.Size = new Size(30, 13);
			mWordIndexLabel.TabIndex = 0;
			mWordIndexLabel.Text = "00:";

			mFullWordEditor.Location = new Point(mWordIndexLabel.Right, 2);
			mFullWordEditor.Name = "mFullWordEditor";
			mFullWordEditor.TabIndex = 1;
			mFullWordEditor.ValueChanged += MFullWordEditor_ValueChanged;
			mFullWordEditor.NavigationKeyDown += This_KeyDown;

			Controls.Add(mWordIndexLabel);
			Controls.Add(mFullWordEditor);
			Name = "DeviceWordEditor";
			Size = new Size(mFullWordEditor.Right + 2, mFullWordEditor.Height + 3);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, e);
					break;

				case Keys.Right:
					break;
			}
		}

		public new void Update()
		{
			mFullWordEditor.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			mWordIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mWordIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mFullWordEditor.UpdateLayout();
		}

		public int WordIndex
		{
			get => mWordIndex;
			set
			{
				mWordIndex = value;
				mWordIndexLabel.Text = value.ToString("D2") + ":";
			}
		}

		public IFullWord DeviceWord
		{
			get => mDeviceWord;
			set
			{
				mDeviceWord = value ?? throw new ArgumentNullException(nameof(value), "DeviceWord may not be set to null");
				mFullWordEditor.WordValue = mDeviceWord;
			}
		}

		public bool ReadOnly
		{
			get => mFullWordEditor.ReadOnly;
			set => mFullWordEditor.ReadOnly = value;
		}

		public IWord WordValue
		{
			get => DeviceWord;
			set
			{
				if (!(value is IFullWord))
				{
					throw new ArgumentException("Value must be an IFullWord");
				}

				DeviceWord = (IFullWord)value;
			}
		}
	}
}
