using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Utils;
using MixLib.Type;

namespace MixGui.Components
{
	public class EditorList<T> : UserControl, IEnumerable<T> where T : IEditor
	{
		private readonly object editorsSyncRoot;
		private VScrollBar indexScrollBar;
		private int firstVisibleIndex;
		private bool readOnly;
		private readonly List<T> editors;
		private bool resizeInProgress;
		private bool sizeAdaptationPending;
		private readonly int minIndex;
		private int maxIndex;
		private CreateEditorCallback createEditor;
		private LoadEditorCallback loadEditor;

		public bool IsReloading { get; private set; }

		public delegate T CreateEditorCallback(int index);
		public delegate void LoadEditorCallback(T editor, int index);
		public delegate void FirstVisibleIndexChangedHandler(EditorList<T> sender, FirstVisibleIndexChangedEventArgs e);

		public event FirstVisibleIndexChangedHandler FirstVisibleIndexChanged;

		public EditorList(int minIndex = 0, int maxIndex = -1, CreateEditorCallback createEditor = null, LoadEditorCallback loadEditor = null)
		{
			this.minIndex = minIndex;
			this.maxIndex = maxIndex;
			this.createEditor = createEditor;
			this.loadEditor = loadEditor;
			this.firstVisibleIndex = 0;

			IsReloading = false;
			this.readOnly = false;
			this.resizeInProgress = false;
			this.sizeAdaptationPending = false;

			this.editorsSyncRoot = new object();
			this.editors = new List<T>();

			InitializeComponent();
		}

		public int EditorCount
			=> this.editors.Count;

		public int MaxEditorCount
			=> this.maxIndex - this.minIndex + 1;

		public T this[int index]
			=> this.editors[index];

		public bool IsIndexVisible(int index)
			=> index >= FirstVisibleIndex && index < (FirstVisibleIndex + VisibleEditorCount);

		public IEnumerator GetEnumerator()
			=> this.editors.GetEnumerator();

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
			=> this.editors.GetEnumerator();

		protected void OnFirstVisibleIndexChanged(FirstVisibleIndexChangedEventArgs args)
			=> FirstVisibleIndexChanged?.Invoke(this, args);

		public int ActiveEditorIndex
		{
			get
			{
				ContainerControl container = this;
				Control control;

				var editorControls = new List<Control>(this.editors.Count);

				foreach (T editor in this.editors)
					editorControls.Add(editor.EditorControl);

				int index = -1;

				while (container != null)
				{
					control = container.ActiveControl;

					index = editorControls.IndexOf(control);
					if (index >= 0)
						break;

					container = control as ContainerControl;
				}

				return index;
			}
		}

		private void AdaptToSize()
		{
			if (MaxEditorCount <= 0 || this.editors.Count == 0)
				return;

			int visibleEditorCount = VisibleEditorCount;
			int editorsToAddCount = (visibleEditorCount - this.editors.Count) + 1;
			FirstVisibleIndex = this.firstVisibleIndex;

			lock (this.editorsSyncRoot)
			{
				IsReloading = true;

				for (int i = 0; i < this.editors.Count; i++)
					this.editors[i].EditorControl.Visible = FirstVisibleIndex + i <= MaxIndex;

				if (editorsToAddCount > 0)
				{
					for (int i = 0; i < editorsToAddCount; i++)
						AddEditor(FirstVisibleIndex + this.editors.Count);
				}

				IsReloading = false;
			}


			if (visibleEditorCount == MaxEditorCount)
			{
				this.indexScrollBar.Enabled = false;
			}
			else
			{
				this.indexScrollBar.Enabled = true;
				this.indexScrollBar.Value = this.firstVisibleIndex;
				this.indexScrollBar.LargeChange = visibleEditorCount;
				this.indexScrollBar.Refresh();
			}

			this.sizeAdaptationPending = false;
		}

		private void AddEditor(int index)
		{
			Control lastEditorControl = this.editors[^1].EditorControl;
			bool indexInItemRange = index <= this.maxIndex;

			SuspendLayout();

			var newEditor = this.createEditor(indexInItemRange ? index : int.MinValue);
			Control newEditorControl = newEditor.EditorControl;
			newEditorControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			newEditorControl.Location = new Point(0, lastEditorControl.Bottom);
			newEditorControl.Size = lastEditorControl.Size;
			newEditorControl.TabIndex = this.editors.Count + 1;
			newEditorControl.Visible = indexInItemRange;
			newEditorControl.MouseWheel += This_MouseWheel;
			newEditor.ReadOnly = this.readOnly;

			if (newEditor is INavigableControl control)
				control.NavigationKeyDown += This_KeyDown;

			this.editors.Add(newEditor);

			Controls.Add(newEditor.EditorControl);

			ResumeLayout(false);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			Controls.Clear();

			this.indexScrollBar = new VScrollBar
			{
				Location = new Point(Width - 16, 0),
				Size = new Size(16, Height),
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
				Minimum = this.minIndex,
				Maximum = this.maxIndex,
				Name = "mAddressScrollBar",
				LargeChange = 1,
				TabIndex = 0,
				Enabled = !this.readOnly
			};
			this.indexScrollBar.Scroll += MIndexScrollBar_Scroll;

			Control editorControl = this.createEditor != null ? CreateFirstEditor() : null;

			Controls.Add(this.indexScrollBar);

			if (editorControl != null)
				Controls.Add(editorControl);

			Name = "WordEditorList";

			BorderStyle = BorderStyle.FixedSingle;

			ResumeLayout(false);

			AdaptToSize();

			SizeChanged += This_SizeChanged;
			KeyDown += This_KeyDown;
			MouseWheel += This_MouseWheel;
		}

		private void This_MouseWheel(object sender, MouseEventArgs e)
			=> FirstVisibleIndex -= (int)(e.Delta / 120.0F * SystemInformation.MouseWheelScrollLines);

		private Control CreateFirstEditor()
		{
			var editor = this.createEditor(0);
			Control editorControl = editor.EditorControl;
			editorControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			editorControl.Location = new Point(0, 0);
			editorControl.Width = this.indexScrollBar.Left;
			editorControl.TabIndex = 1;
			editorControl.MouseWheel += This_MouseWheel;
			editor.ReadOnly = this.readOnly;

			if (editor is INavigableControl control)
				control.NavigationKeyDown += This_KeyDown;

			this.editors.Add(editor);

			return editorControl;
		}

		private void This_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			FieldTypes? editorField = null;
			int? index = null;

			if (e is IndexKeyEventArgs ike)
				index = ike.Index;

			if (e is FieldKeyEventArgs fke)
				editorField = fke.Field;

			T editor;

			switch (e.KeyCode)
			{
				case Keys.Prior:
					FirstVisibleIndex -= VisibleEditorCount;
					break;

				case Keys.Next:
					FirstVisibleIndex += VisibleEditorCount;
					return;

				case Keys.Up:
					if (sender is not INavigableControl)
					{
						FirstVisibleIndex--;
						return;
					}

					editor = (T)sender;

					if (!editor.Equals(this.editors[0]))
					{
						this.editors[this.editors.IndexOf(editor) - 1].Focus(editorField, index);
						return;
					}

					this.indexScrollBar.Focus();
					FirstVisibleIndex--;
					editor.Focus(editorField, index);

					return;

				case Keys.Right:
					break;

				case Keys.Down:
					if (sender is not INavigableControl)
					{
						FirstVisibleIndex++;
						return;
					}

					editor = (T)sender;

					if (this.editors.IndexOf(editor) < VisibleEditorCount - 1)
					{
						this.editors[this.editors.IndexOf(editor) + 1].Focus(editorField, index);
						return;
					}

					this.indexScrollBar.Focus();
					FirstVisibleIndex++;
					this.editors[VisibleEditorCount - 1].Focus(editorField, index);

					return;
			}
		}

		private void MIndexScrollBar_Scroll(object sender, ScrollEventArgs e)
			=> FirstVisibleIndex = this.indexScrollBar.Value;

		public void MakeIndexVisible(int index)
		{
			if (!IsIndexVisible(index))
				FirstVisibleIndex = index;
		}

		public LoadEditorCallback LoadEditor
		{
			get => this.loadEditor;
			set
			{
				if (value == null)
					return;

				if (this.loadEditor != null)
					throw new InvalidOperationException("value may only be set once");

				this.loadEditor = value;
			}
		}

		public CreateEditorCallback CreateEditor
		{
			get => this.createEditor;
			set
			{
				if (value == null)
					return;

				if (this.createEditor != null)
					throw new InvalidOperationException("value may only be set once");

				this.createEditor = value;

				Controls.Add(CreateFirstEditor());
				AdaptToSize();
			}
		}

		public bool ResizeInProgress
		{
			get => this.resizeInProgress;

			set
			{
				if (this.resizeInProgress == value)
					return;

				this.resizeInProgress = value;

				if (this.resizeInProgress)
				{
					for (int i = 1; i < this.editors.Count; i++)
						this.editors[i].EditorControl.SuspendDrawing();
				}
				else
				{
					for (int i = 1; i < this.editors.Count; i++)
						this.editors[i].EditorControl.ResumeDrawing();

					if (this.sizeAdaptationPending)
						AdaptToSize();

					Invalidate(true);
				}
			}
		}

		private void This_SizeChanged(object sender, EventArgs e)
		{
			if (!this.resizeInProgress)
				AdaptToSize();

			else
				this.sizeAdaptationPending = true;
		}

		public new void Update()
		{
			lock (this.editorsSyncRoot)
			{
				for (int i = 0; i < this.editors.Count; i++)
				{
					int index = this.firstVisibleIndex + i;
					this.loadEditor(this.editors[i], index <= this.maxIndex ? index : int.MinValue);
				}
			}

			base.Update();
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			lock (this.editorsSyncRoot)
			{
				foreach (T editor in this.editors)
					editor.UpdateLayout();
			}

			ResumeLayout();
		}

		public int FirstVisibleIndex
		{
			get => this.firstVisibleIndex;
			set
			{
				if (value < this.minIndex)
					value = this.minIndex;

				if (value + VisibleEditorCount > this.maxIndex + 1)
					value = this.maxIndex - VisibleEditorCount + 1;

				if (value == this.firstVisibleIndex)
					return;

				int indexDelta = value - this.firstVisibleIndex;
				int selectedEditorIndex = ActiveEditorIndex;
				FieldTypes? field = selectedEditorIndex >= 0 ? this.editors[selectedEditorIndex].FocusedField : null;
				int? caretIndex = selectedEditorIndex >= 0 ? this.editors[selectedEditorIndex].CaretIndex : null;

				this.firstVisibleIndex = value;

				lock (this.editorsSyncRoot)
				{
					IsReloading = true;

					for (int i = 0; i < this.editors.Count; i++)
					{
						int index = this.firstVisibleIndex + i;
						bool indexInItemRange = index <= this.maxIndex;

						T editor = this.editors[i];
						this.loadEditor?.Invoke(editor, indexInItemRange ? index : int.MinValue);

						editor.EditorControl.Visible = indexInItemRange;
					}

					if (selectedEditorIndex != -1)
					{
						selectedEditorIndex -= indexDelta;
						if (selectedEditorIndex < 0)
							selectedEditorIndex = 0;

						else if (selectedEditorIndex >= VisibleEditorCount)
							selectedEditorIndex = VisibleEditorCount - 1;

						this.editors[selectedEditorIndex].Focus(field, caretIndex);
					}

					IsReloading = false;
				}

				this.indexScrollBar.Value = this.firstVisibleIndex;
				OnFirstVisibleIndexChanged(new FirstVisibleIndexChangedEventArgs(this.firstVisibleIndex));
			}
		}

		public bool ReadOnly
		{
			get => this.readOnly;
			set
			{
				if (this.readOnly == value)
					return;

				this.readOnly = value;
				lock (this.editorsSyncRoot)
				{
					foreach (T editor in this.editors)
						editor.ReadOnly = this.readOnly;
				}
			}
		}

		public int VisibleEditorCount
		{
			get
			{
				if (this.editors.Count == 0)
					return 0;

				int editorAreaHeight = Height;

				if (editorAreaHeight < 0)
					editorAreaHeight = 0;

				return Math.Min(editorAreaHeight / this.editors[0].EditorControl.Height, MaxEditorCount);
			}
		}

		public int MaxIndex
		{
			get => this.maxIndex;
			set
			{
				this.maxIndex = value;

				this.indexScrollBar.Maximum = this.maxIndex;

				AdaptToSize();
			}
		}

		public int MinIndex
		{
			get => this.minIndex;
			set
			{
				this.maxIndex = value;

				this.indexScrollBar.Minimum = this.minIndex;

				AdaptToSize();
			}
		}

		public class FirstVisibleIndexChangedEventArgs : EventArgs
		{
			public int FirstVisibleIndex { get; private set; }

			public FirstVisibleIndexChangedEventArgs(int firstVisibleIndex)
				=> FirstVisibleIndex = firstVisibleIndex;
		}

	}
}
