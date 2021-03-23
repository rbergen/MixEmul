using MixGui.Events;
using MixLib.Type;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class FullWordEditor : UserControl, IWordEditor, INavigableControl
	{
		readonly Label mEqualsLabel;
		IFullWord mFullWord;
		bool mReadOnly;
		readonly MixByteCollectionCharTextBox mWordCharTextBox;
		readonly WordValueEditor mWordValueEditor;

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
			InitializeComponent();
		}

		public Control EditorControl => this;

		public FieldTypes? FocusedField => mWordCharTextBox.Focused ? FieldTypes.Chars : mWordValueEditor.FocusedField;

		public int? CaretIndex => FocusedField == FieldTypes.Chars ? mWordCharTextBox.CaretIndex : mWordValueEditor.CaretIndex;

		public bool Focus(FieldTypes? field, int? index)
		{
			return field == FieldTypes.Chars ? mWordCharTextBox.Focus(field, index) : mWordValueEditor.Focus(field, index);
		}

		protected virtual void OnValueChanged(WordEditorValueChangedEventArgs args)
		{
			ValueChanged?.Invoke(this, args);
		}

		void InitializeComponent()
		{
			SuspendLayout();

			mWordValueEditor.Location = new Point(0, 0);
			mWordValueEditor.Name = "mWordValueEditor";
			mWordValueEditor.TabIndex = 2;
			mWordValueEditor.ValueChanged += MWordValueEditor_ValueChanged;
			mWordValueEditor.NavigationKeyDown += This_KeyDown;

			mEqualsLabel.Location = new Point(mWordValueEditor.Right, mWordValueEditor.Top + 2);
			mEqualsLabel.Name = "mFirstEqualsLabel";
			mEqualsLabel.Size = new Size(10, mWordValueEditor.Height - 2);
			mEqualsLabel.TabIndex = 3;
			mEqualsLabel.Text = "=";

			mWordCharTextBox.Location = new Point(mEqualsLabel.Right, mWordValueEditor.Top);
			mWordCharTextBox.Name = "mWordCharTextBox";
			mWordCharTextBox.Size = new Size(45, mWordValueEditor.Height);
			mWordCharTextBox.TabIndex = 4;
			mWordCharTextBox.ValueChanged += MWordCharTextBox_ValueChanged;
			mWordCharTextBox.NavigationKeyDown += This_KeyDown;

			Controls.Add(mWordValueEditor);
			Controls.Add(mEqualsLabel);
			Controls.Add(mWordCharTextBox);
			Name = "FullWordEditor";
			Size = new Size(mWordCharTextBox.Right, mWordCharTextBox.Height);
			KeyDown += This_KeyDown;
			ResumeLayout(false);
		}

		void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
			{
				return;
			}

			FieldTypes editorField = FieldTypes.Chars;
			int? index = mWordCharTextBox.SelectionStart + mWordCharTextBox.SelectionLength;

			if (e is FieldKeyEventArgs args)
			{
				editorField = args.Field;
				index = args.Index;
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

		void MWordCharTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			mWordValueEditor.Update();
			OnValueChanged(new WordEditorValueChangedEventArgs((Word)args.OldValue, (Word)args.NewValue));
		}

		void MWordValueEditor_ValueChanged(IWordEditor sender, WordEditorValueChangedEventArgs args)
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
			get => mFullWord;
			set
			{
				mFullWord = value ?? throw new ArgumentNullException(nameof(value), "FullWord may not be set to null");
				mWordValueEditor.WordValue = mFullWord;
				mWordCharTextBox.MixByteCollectionValue = mFullWord;
			}
		}

		public bool ReadOnly
		{
			get => mReadOnly;
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
			get => FullWord;
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
