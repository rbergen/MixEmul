namespace MixGui.Components
{
	using MixGui.Events;
	using MixGui.Settings;
	using MixGui.Utils;
	using MixLib.Type;
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class DeviceMixByteCollectionEditor : UserControl, IMixByteCollectionEditor, INavigableControl
	{
		readonly Label mMixByteCollectionIndexLabel;
		readonly MixByteCollectionCharTextBox mCharTextBox;
		int mMixByteCollectionIndex;
		IMixByteCollection mDeviceMixByteCollection;
		int mIndexCharCount = 2;

		public event KeyEventHandler NavigationKeyDown;
		public event MixByteCollectionEditorValueChangedEventHandler ValueChanged;

		public DeviceMixByteCollectionEditor()
			: this(null)
		{
		}

		public DeviceMixByteCollectionEditor(IMixByteCollection mixByteCollection)
		{
			mDeviceMixByteCollection = mixByteCollection;
			if (mDeviceMixByteCollection == null)
			{
				mDeviceMixByteCollection = new MixByteCollection(FullWord.ByteCount);
			}

			mCharTextBox = new MixByteCollectionCharTextBox(mDeviceMixByteCollection);
			mMixByteCollectionIndexLabel = new Label();
			InitializeComponent();
		}

		public Control EditorControl => this;

		public FieldTypes? FocusedField => mCharTextBox.Focused ? FieldTypes.Chars : null;

		public int? CaretIndex => mCharTextBox.Focused ? mCharTextBox.SelectionStart + mCharTextBox.SelectionLength : null;

		public void Select(int start, int length)
		{
			mCharTextBox.Select(start, length);
		}

		public bool Focus(FieldTypes? field, int? index)
		{
			return mCharTextBox.FocusWithIndex(index);
		}

		protected virtual void OnValueChanged(MixByteCollectionEditorValueChangedEventArgs args)
		{
			ValueChanged?.Invoke(this, args);
		}

		void MMixByteCollectionEditor_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			OnValueChanged(args);
		}

		void InitializeComponent()
		{
			SuspendLayout();

			mMixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mMixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mMixByteCollectionIndexLabel.Location = new Point(0, 0);
			mMixByteCollectionIndexLabel.Name = "mMixByteCollectionIndexLabel";
			mMixByteCollectionIndexLabel.Size = new Size(30, 13);
			mMixByteCollectionIndexLabel.AutoSize = true;
			mMixByteCollectionIndexLabel.TabIndex = 0;
			mMixByteCollectionIndexLabel.Text = "00:";

			mCharTextBox.Location = new Point(mMixByteCollectionIndexLabel.Right, 0);
			mCharTextBox.Name = "mCharTextBox";
			mCharTextBox.TabIndex = 1;
			mCharTextBox.ValueChanged += MMixByteCollectionEditor_ValueChanged;
			mCharTextBox.NavigationKeyDown += This_KeyDown;
			mCharTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			mCharTextBox.BorderStyle = BorderStyle.None;
			mCharTextBox.Height = 13;

			Controls.Add(mMixByteCollectionIndexLabel);
			Controls.Add(mCharTextBox);
			Name = "DeviceMixByteCollectionEditor";
			Size = new Size(mCharTextBox.Right, mCharTextBox.Height);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
					NavigationKeyDown?.Invoke(this, e);
					break;
			}
		}

		public new void Update()
		{
			mCharTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			mMixByteCollectionIndexLabel.Font = GuiSettings.GetFont(GuiSettings.FixedWidth);
			mMixByteCollectionIndexLabel.ForeColor = GuiSettings.GetColor(GuiSettings.AddressText);
			mCharTextBox.UpdateLayout();
		}

		public int MixByteCollectionIndex
		{
			get => mMixByteCollectionIndex;
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				mMixByteCollectionIndex = value;
				SetIndexLabelText();
			}
		}

		void SetIndexLabelText()
		{
			mMixByteCollectionIndexLabel.Text = mMixByteCollectionIndex.ToString("D" + mIndexCharCount) + ":";
		}

		public int IndexCharCount
		{
			get => mIndexCharCount;
			set
			{
				if (mIndexCharCount == value)
				{
					return;
				}

				mIndexCharCount = value;
				SetIndexLabelText();

				int oldCharBoxLeft = mCharTextBox.Left;
				mCharTextBox.Left = mMixByteCollectionIndexLabel.Right + 3;
				mCharTextBox.Width += oldCharBoxLeft - mCharTextBox.Left;
			}
		}

		public IMixByteCollection DeviceMixByteCollection
		{
			get => mDeviceMixByteCollection;
			set
			{
				mDeviceMixByteCollection = value ?? throw new ArgumentNullException(nameof(value), "DeviceMixByteCollection may not be set to null");
				mCharTextBox.MixByteCollectionValue = mDeviceMixByteCollection;
				mCharTextBox.Select(0, 0);
			}
		}

		public bool ReadOnly
		{
			get => mCharTextBox.ReadOnly;
			set => mCharTextBox.ReadOnly = value;
		}

		public IMixByteCollection MixByteCollectionValue
		{
			get => DeviceMixByteCollection;
			set => DeviceMixByteCollection = value;
		}
	}
}
