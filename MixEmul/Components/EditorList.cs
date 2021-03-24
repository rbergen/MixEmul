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
		private readonly object _editorsSyncRoot;
		private VScrollBar _indexScrollBar;
		private int _firstVisibleIndex;
		private bool _readOnly;
		private readonly List<T> _editors;
		private bool _resizeInProgress;
		private bool _sizeAdaptationPending;
		private readonly int _minIndex;
		private int _maxIndex;
		private CreateEditorCallback _createEditor;
		private LoadEditorCallback _loadEditor;

		public bool IsReloading { get; private set; }

		public delegate T CreateEditorCallback(int index);
		public delegate void LoadEditorCallback(T editor, int index);
		public delegate void FirstVisibleIndexChangedHandler(EditorList<T> sender, FirstVisibleIndexChangedEventArgs e);

		public event FirstVisibleIndexChangedHandler FirstVisibleIndexChanged;

		public EditorList(int minIndex = 0, int maxIndex = -1, CreateEditorCallback createEditor = null, LoadEditorCallback loadEditor = null)
		{
			_minIndex = minIndex;
			_maxIndex = maxIndex;
			_createEditor = createEditor;
			_loadEditor = loadEditor;
			_firstVisibleIndex = 0;

			IsReloading = false;
			_readOnly = false;
			_resizeInProgress = false;
			_sizeAdaptationPending = false;

			_editorsSyncRoot = new object();
			_editors = new List<T>();

			InitializeComponent();
		}

		public int EditorCount
			=> _editors.Count;

		public int MaxEditorCount
			=> _maxIndex - _minIndex + 1;

		public T this[int index]
			=> _editors[index];

		public bool IsIndexVisible(int index)
			=> index >= FirstVisibleIndex && index < (FirstVisibleIndex + VisibleEditorCount);

		public IEnumerator GetEnumerator()
			=> _editors.GetEnumerator();

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
			=> _editors.GetEnumerator();

		protected void OnFirstVisibleIndexChanged(FirstVisibleIndexChangedEventArgs args)
			=> FirstVisibleIndexChanged?.Invoke(this, args);

		public int ActiveEditorIndex
		{
			get
			{
				ContainerControl container = this;
				Control control;

				var editorControls = new List<Control>(_editors.Count);

				foreach (T editor in _editors)
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
			if (MaxEditorCount <= 0 || _editors.Count == 0)
				return;

			int visibleEditorCount = VisibleEditorCount;
			int editorsToAddCount = (visibleEditorCount - _editors.Count) + 1;
			FirstVisibleIndex = _firstVisibleIndex;

			lock (_editorsSyncRoot)
			{
				IsReloading = true;

				for (int i = 0; i < _editors.Count; i++)
					_editors[i].EditorControl.Visible = FirstVisibleIndex + i <= MaxIndex;

				if (editorsToAddCount > 0)
				{
					for (int i = 0; i < editorsToAddCount; i++)
						AddEditor(FirstVisibleIndex + _editors.Count);
				}

				IsReloading = false;
			}


			if (visibleEditorCount == MaxEditorCount)
			{
				_indexScrollBar.Enabled = false;
			}
			else
			{
				_indexScrollBar.Enabled = true;
				_indexScrollBar.Value = _firstVisibleIndex;
				_indexScrollBar.LargeChange = visibleEditorCount;
				_indexScrollBar.Refresh();
			}

			_sizeAdaptationPending = false;
		}

		private void AddEditor(int index)
		{
			Control lastEditorControl = _editors[^1].EditorControl;
			bool indexInItemRange = index <= _maxIndex;

			SuspendLayout();

			var newEditor = _createEditor(indexInItemRange ? index : int.MinValue);
			Control newEditorControl = newEditor.EditorControl;
			newEditorControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			newEditorControl.Location = new Point(0, lastEditorControl.Bottom);
			newEditorControl.Size = lastEditorControl.Size;
			newEditorControl.TabIndex = _editors.Count + 1;
			newEditorControl.Visible = indexInItemRange;
			newEditorControl.MouseWheel += This_MouseWheel;
			newEditor.ReadOnly = _readOnly;

			if (newEditor is INavigableControl control)
				control.NavigationKeyDown += This_KeyDown;

			_editors.Add(newEditor);

			Controls.Add(newEditor.EditorControl);

			ResumeLayout(false);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			Controls.Clear();

			_indexScrollBar = new VScrollBar
			{
				Location = new Point(Width - 16, 0),
				Size = new Size(16, Height),
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
				Minimum = _minIndex,
				Maximum = _maxIndex,
				Name = "mAddressScrollBar",
				LargeChange = 1,
				TabIndex = 0,
				Enabled = !_readOnly
			};
			_indexScrollBar.Scroll += MIndexScrollBar_Scroll;

			Control editorControl = _createEditor != null ? CreateFirstEditor() : null;

			Controls.Add(_indexScrollBar);

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
			var editor = _createEditor(0);
			Control editorControl = editor.EditorControl;
			editorControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			editorControl.Location = new Point(0, 0);
			editorControl.Width = _indexScrollBar.Left;
			editorControl.TabIndex = 1;
			editorControl.MouseWheel += This_MouseWheel;
			editor.ReadOnly = _readOnly;

			if (editor is INavigableControl control)
				control.NavigationKeyDown += This_KeyDown;

			_editors.Add(editor);

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

					if (!editor.Equals(_editors[0]))
					{
						_editors[_editors.IndexOf(editor) - 1].Focus(editorField, index);
						return;
					}

					_indexScrollBar.Focus();
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

					if (_editors.IndexOf(editor) < VisibleEditorCount - 1)
					{
						_editors[_editors.IndexOf(editor) + 1].Focus(editorField, index);
						return;
					}

					_indexScrollBar.Focus();
					FirstVisibleIndex++;
					_editors[VisibleEditorCount - 1].Focus(editorField, index);

					return;
			}
		}

		private void MIndexScrollBar_Scroll(object sender, ScrollEventArgs e)
			=> FirstVisibleIndex = _indexScrollBar.Value;

		public void MakeIndexVisible(int index)
		{
			if (!IsIndexVisible(index))
				FirstVisibleIndex = index;
		}

		public LoadEditorCallback LoadEditor
		{
			get => _loadEditor;
			set
			{
				if (value == null)
					return;

				if (_loadEditor != null)
					throw new InvalidOperationException("value may only be set once");

				_loadEditor = value;
			}
		}

		public CreateEditorCallback CreateEditor
		{
			get => _createEditor;
			set
			{
				if (value == null)
					return;

				if (_createEditor != null)
					throw new InvalidOperationException("value may only be set once");

				_createEditor = value;

				Controls.Add(CreateFirstEditor());
				AdaptToSize();
			}
		}

		public bool ResizeInProgress
		{
			get => _resizeInProgress;

			set
			{
				if (_resizeInProgress == value)
					return;

				_resizeInProgress = value;

				if (_resizeInProgress)
				{
					for (int i = 1; i < _editors.Count; i++)
						_editors[i].EditorControl.SuspendDrawing();
				}
				else
				{
					for (int i = 1; i < _editors.Count; i++)
						_editors[i].EditorControl.ResumeDrawing();

					if (_sizeAdaptationPending)
						AdaptToSize();

					Invalidate(true);
				}
			}
		}

		private void This_SizeChanged(object sender, EventArgs e)
		{
			if (!_resizeInProgress)
				AdaptToSize();

			else
				_sizeAdaptationPending = true;
		}

		public new void Update()
		{
			lock (_editorsSyncRoot)
			{
				for (int i = 0; i < _editors.Count; i++)
				{
					int index = _firstVisibleIndex + i;
					_loadEditor(_editors[i], index <= _maxIndex ? index : int.MinValue);
				}
			}

			base.Update();
		}

		public void UpdateLayout()
		{
			SuspendLayout();

			lock (_editorsSyncRoot)
			{
				foreach (T editor in _editors)
					editor.UpdateLayout();
			}

			ResumeLayout();
		}

		public int FirstVisibleIndex
		{
			get => _firstVisibleIndex;
			set
			{
				if (value < _minIndex)
					value = _minIndex;

				if (value + VisibleEditorCount > _maxIndex + 1)
					value = _maxIndex - VisibleEditorCount + 1;

				if (value == _firstVisibleIndex)
					return;

				int indexDelta = value - _firstVisibleIndex;
				int selectedEditorIndex = ActiveEditorIndex;
				FieldTypes? field = selectedEditorIndex >= 0 ? _editors[selectedEditorIndex].FocusedField : null;
				int? caretIndex = selectedEditorIndex >= 0 ? _editors[selectedEditorIndex].CaretIndex : null;

				_firstVisibleIndex = value;

				lock (_editorsSyncRoot)
				{
					IsReloading = true;

					for (int i = 0; i < _editors.Count; i++)
					{
						int index = _firstVisibleIndex + i;
						bool indexInItemRange = index <= _maxIndex;

						T editor = _editors[i];
						_loadEditor?.Invoke(editor, indexInItemRange ? index : int.MinValue);

						editor.EditorControl.Visible = indexInItemRange;
					}

					if (selectedEditorIndex != -1)
					{
						selectedEditorIndex -= indexDelta;
						if (selectedEditorIndex < 0)
							selectedEditorIndex = 0;

						else if (selectedEditorIndex >= VisibleEditorCount)
							selectedEditorIndex = VisibleEditorCount - 1;

						_editors[selectedEditorIndex].Focus(field, caretIndex);
					}

					IsReloading = false;
				}

				_indexScrollBar.Value = _firstVisibleIndex;
				OnFirstVisibleIndexChanged(new FirstVisibleIndexChangedEventArgs(_firstVisibleIndex));
			}
		}

		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly == value)
					return;

				_readOnly = value;
				lock (_editorsSyncRoot)
				{
					foreach (T editor in _editors)
						editor.ReadOnly = _readOnly;
				}
			}
		}

		public int VisibleEditorCount
		{
			get
			{
				if (_editors.Count == 0)
					return 0;

				int editorAreaHeight = Height;

				if (editorAreaHeight < 0)
					editorAreaHeight = 0;

				return Math.Min(editorAreaHeight / _editors[0].EditorControl.Height, MaxEditorCount);
			}
		}

		public int MaxIndex
		{
			get => _maxIndex;
			set
			{
				_maxIndex = value;

				_indexScrollBar.Maximum = _maxIndex;

				AdaptToSize();
			}
		}

		public int MinIndex
		{
			get => _minIndex;
			set
			{
				_maxIndex = value;

				_indexScrollBar.Minimum = _minIndex;

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
