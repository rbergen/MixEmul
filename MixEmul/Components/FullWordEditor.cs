using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public class FullWordEditor : UserControl, IWordEditor, INavigableControl
	{
		private Label mEqualsLabel;
		private IFullWord mFullWord;
		private bool mReadOnly;
		private MixByteCollectionCharTextBox mWordCharTextBox;
		private WordValueEditor mWordValueEditor;

		public event KeyEventHandler NavigationKeyDown;
		public event WordEditorValueChangedEventHandler ValueChanged;

		public FullWordEditor()
			: this(null)
		{
		}

		public FullWordEditor(IFullWord fullWord)
		{
			mFullWord = fullWord;
			mReadOnly = false;
			if (mFullWord == null)
			{
				mFullWord = new FullWord();
			}

			mWordValueEditor = new WordValueEditor(mFullWord);
			mWordCharTextBox = new MixByteCollectionCharTextBox(mFullWord);
			mEqualsLabel = new Label();
			initializeComponent();
		}

        public Control EditorControl => this;

        public FieldTypes? FocusedField => mWordCharTextBox.Focused ? FieldTypes.Chars : mWordValueEditor.FocusedField;

        public int? CaretIndex => FocusedField == FieldTypes.Chars ? mWordCharTextBox.CaretIndex : mWordValueEditor.CaretIndex;

        public bool Focus(FieldTypes? field, int? index) => field == FieldTypes.Chars ? mWordCharTextBox.Focus(field, index) : mWordValueEditor.Focus(field, index);

        protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args) => ValueChanged?.Invoke(this, args);

        private void initializeComponent()
		{
			base.SuspendLayout();

			mWordValueEditor.Location = new Point(0, 0);
			mWordValueEditor.Name = "mWordValueEditor";
			mWordValueEditor.TabIndex = 2;
			mWordValueEditor.ValueChanged += mWordValueEditor_ValueChanged;
			mWordValueEditor.NavigationKeyDown += keyDown;

			mEqualsLabel.Location = new Point(mWordValueEditor.Right, mWordValueEditor.Top + 2);
			mEqualsLabel.Name = "mFirstEqualsLabel";
			mEqualsLabel.Size = new Size(10, mWordValueEditor.Height - 2);
			mEqualsLabel.TabIndex = 3;
			mEqualsLabel.Text = "=";

			mWordCharTextBox.Location = new Point(mEqualsLabel.Right, mWordValueEditor.Top);
			mWordCharTextBox.Name = "mWordCharTextBox";
			mWordCharTextBox.Size = new Size(45, mWordValueEditor.Height);
			mWordCharTextBox.TabIndex = 4;
			mWordCharTextBox.ValueChanged += mWordCharTextBox_ValueChanged;
			mWordCharTextBox.NavigationKeyDown += keyDown;

			base.Controls.Add(mWordValueEditor);
			base.Controls.Add(mEqualsLabel);
			base.Controls.Add(mWordCharTextBox);
			base.Name = "FullWordEditor";
			base.Size = new Size(mWordCharTextBox.Right, mWordCharTextBox.Height);
			base.KeyDown += keyDown;
			base.ResumeLayout(false);
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			FieldTypes editorField = FieldTypes.Chars;
			int? index = mWordCharTextBox.SelectionStart + mWordCharTextBox.SelectionLength;

			if (e is FieldKeyEventArgs)
			{
				editorField = ((FieldKeyEventArgs)e).Field;
				index = ((FieldKeyEventArgs)e).Index;
			}

			switch (e.KeyCode)
			{
				case Keys.Prior:
				case Keys.Next:
				case Keys.Up:
				case Keys.Down:
                    NavigationKeyDown?.Invoke(this, new FieldKeyEventArgs(e.KeyData, editorField, index));
                    break;

				case Keys.Right:
					if (sender == mWordValueEditor)
					{
						mWordCharTextBox.Focus();
					}
					else if (sender == mWordCharTextBox && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}

					break;

				case Keys.Left:
					if (sender == mWordCharTextBox)
					{
						mWordValueEditor.Focus(FieldTypes.Value, null);
					}
					else if (sender == mWordValueEditor && NavigationKeyDown != null)
					{
						NavigationKeyDown(this, e);
					}

					break;
			}
		}

		private void mWordCharTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			mWordValueEditor.Update();
			OnValueChanged(new WordEditorValueChangedEventArgs((Word)args.OldValue, (Word)args.NewValue));
		}

		private void mWordValueEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
		{
			mWordCharTextBox.Update();
			OnValueChanged(args);
		}

		public new void Update()
		{
			mWordValueEditor.Update();
			mWordCharTextBox.Update();
			base.Update();
		}

		public void UpdateLayout()
		{
			mWordValueEditor.UpdateLayout();
			mWordCharTextBox.UpdateLayout();
		}

		public IFullWord FullWord
		{
			get
			{
				return mFullWord;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "FullWord may not be set to null");
				}

				mFullWord = value;
				mWordValueEditor.WordValue = mFullWord;
				mWordCharTextBox.MixByteCollectionValue = mFullWord;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return mReadOnly;
			}
			set
			{
				if (mReadOnly != value)
				{
					mReadOnly = value;
					mWordValueEditor.ReadOnly = mReadOnly;
					mWordCharTextBox.ReadOnly = mReadOnly;
				}
			}
		}

		public IWord WordValue
		{
			get
			{
				return FullWord;
			}
			set
			{
				if (!(value is IFullWord))
				{
					throw new ArgumentException("Value must be an IFullWord");
				}

				FullWord = (IFullWord)value;
			}
		}

		public void Select(int start, int length)
		{
			if (FocusedField == FieldTypes.Chars)
			{
				mWordCharTextBox.Select(start, length);
			}
			else
			{
				mWordValueEditor.Select(start, length);
			}
		}
	}
}
