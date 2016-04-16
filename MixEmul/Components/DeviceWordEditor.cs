using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixGui.Settings;
using MixLib.Type;

namespace MixGui.Components
{
	public class DeviceWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private Label mWordIndexLabel;
		private FullWordEditor mFullWordEditor;
		private int mWordIndex;
		private IFullWord mDeviceWord;

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
			initializeComponent();
		}

		public bool Focus(FieldTypes? field, int? index)
		{
			return mFullWordEditor.Focus(field, index);
		}

		private void initializeComponent()
		{
			base.SuspendLayout();

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
			mFullWordEditor.ValueChanged += new WordEditorValueChangedEventHandler(mFullWordEditor_ValueChanged);
			mFullWordEditor.NavigationKeyDown += new KeyEventHandler(keyDown);

			base.Controls.Add(mWordIndexLabel);
			base.Controls.Add(mFullWordEditor);
			base.Name = "DeviceWordEditor";
			base.Size = new Size(mFullWordEditor.Right + 2, mFullWordEditor.Height + 3);
			base.KeyDown += new KeyEventHandler(keyDown);
			base.ResumeLayout(false);
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					if (NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}
					break;

				case Keys.Right:
					break;
			}
		}

		private void mFullWordEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			OnValueChanged(args);
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, args);
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
			get
			{
				return mWordIndex;
			}
			set
			{
				mWordIndex = value;
				mWordIndexLabel.Text = value.ToString("D2") + ":";
			}
		}

		public IFullWord DeviceWord
		{
			get
			{
				return mDeviceWord;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "DeviceWord may not be set to null");
				}

				mDeviceWord = value;
				mFullWordEditor.WordValue = mDeviceWord;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return mFullWordEditor.ReadOnly;
			}
			set
			{
				mFullWordEditor.ReadOnly = value;
			}
		}

		public IWord WordValue
		{
			get
			{
				return DeviceWord;
			}
			set
			{
				if (!(value is IFullWord))
				{
					throw new ArgumentException("Value must be an IFullWord");
				}

				DeviceWord = (IFullWord)value;
			}
		}

		public Control EditorControl
		{
			get
			{
				return this;
			}
		}

		public FieldTypes? FocusedField
		{
			get
			{
				return mFullWordEditor.FocusedField;
			}
		}

		public int? CaretIndex
		{
			get
			{
				return mFullWordEditor.CaretIndex;
			}
		}

		public void Select(int start, int length)
		{
			mFullWordEditor.Select(start, length);
		}
	}
}
