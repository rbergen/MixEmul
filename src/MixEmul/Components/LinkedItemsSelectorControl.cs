using System;
using System.Drawing;
using System.Windows.Forms;
using MixGui.Properties;

namespace MixGui.Components
{
	public class LinkedItemsSelectorControl<T> : UserControl where T : class, IEquatable<T>
	{
		public event EventHandler<ItemSelectedEventArgs<T>> ItemSelected;
		public delegate T CurrentItemCallback();

		private const int MaxItemCount = 1000;

		private LinkedItem<T> _firstItem;
		private LinkedItem<T> _currentItem;
		private int _itemCount;
		private Button _navigateBackwards;
		private Button _navigateForwards;
		private CurrentItemCallback _getCurrentItem;

		public LinkedItemsSelectorControl()
			=> InitializeComponent();

		protected void OnItemSelected(ItemSelectedEventArgs<T> e)
			=> ItemSelected?.Invoke(this, e);

		private void InitializeComponent()
		{
			_navigateForwards = new Button();
			_navigateBackwards = new Button();

			SuspendLayout();
			// 
			// mNavigateBackwards
			// 
			_navigateBackwards.Enabled = false;
			_navigateBackwards.FlatStyle = FlatStyle.Flat;
			_navigateBackwards.Image = Resources.NavigateBackwards_6270;
			_navigateBackwards.Location = new Point(0, 0);
			_navigateBackwards.Name = "mNavigateBackwards";
			_navigateBackwards.Size = new Size(21, 21);
			_navigateBackwards.TabIndex = 0;
			_navigateBackwards.Click += MNavigateBackwards_Click;
			// 
			// mNavigateForwards
			// 
			_navigateForwards.Enabled = false;
			_navigateForwards.FlatStyle = FlatStyle.Flat;
			_navigateForwards.Image = Resources.NavigateForward_6271;
			_navigateForwards.Location = new Point(_navigateBackwards.Right - 1, 0);
			_navigateForwards.Name = "mNavigateForwards";
			_navigateForwards.Size = _navigateBackwards.Size;
			_navigateForwards.TabIndex = 1;
			_navigateForwards.Click += MNavigateForwards_Click;
			// 
			// LinkedItemsSelectorControl
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(_navigateForwards);
			Controls.Add(_navigateBackwards);
			Name = "LinkedItemsSelectorControl";
			Size = new Size(_navigateForwards.Right, _navigateForwards.Height);

			ResumeLayout(false);
		}

		public void SetCurrentItemCallback(CurrentItemCallback getCurrentItem)
			=> _getCurrentItem = getCurrentItem;

		public void AddItem(T currentItem, T newItem)
		{
			if (currentItem.Equals(newItem))
				return;

			AddItem(currentItem);
			AddItem(newItem);

			PruneToMaxItemCount();

			SetEnabledState();
		}

		private void PruneToMaxItemCount()
		{
			while (_itemCount > MaxItemCount)
			{
				_firstItem.Next.Previous = null;
				_firstItem = _firstItem.Next;
				_itemCount--;
			}
		}

		private void AddItem(T item)
		{
			if (item == null)
				return;

			if (_firstItem == null)
			{
				_firstItem = new LinkedItem<T>(item);
				_currentItem = _firstItem;
				_itemCount++;

				return;
			}

			if (_currentItem.Item.Equals(item))
				return;

			if (_currentItem.Next != null)
			{
				int purgeItemCount = 0;
				LinkedItem<T> oldNext = _currentItem.Next;
				while (oldNext != null)
				{
					purgeItemCount++;
					oldNext = oldNext.Next;
				}

				_currentItem.Next.Previous = null;
				_itemCount -= purgeItemCount;
			}

			_currentItem.Next = new LinkedItem<T>(item);
			_itemCount++;
			_currentItem.Next.Previous = _currentItem;
			_currentItem = _currentItem.Next;
		}

		private void SetEnabledState()
		{
			_navigateBackwards.Enabled = _currentItem != null && _currentItem.Previous != null;
			_navigateForwards.Enabled = _currentItem != null && _currentItem.Next != null;
		}

		private void MNavigateBackwards_Click(object sender, EventArgs e)
		{
			var item = _getCurrentItem?.Invoke();

			if (item != null && !_currentItem.Item.Equals(item) && !_currentItem.Previous.Item.Equals(item))
				_currentItem = InsertItem(_currentItem, item);

			_currentItem = _currentItem.Previous;

			OnItemSelected(new ItemSelectedEventArgs<T>(_currentItem.Item));

			SetEnabledState();
		}

		private LinkedItem<T> InsertItem(LinkedItem<T> insertBefore, T item)
		{
			if (insertBefore == null || item == null)
				return null;

			var insertee = new LinkedItem<T>(item)
			{
				Previous = insertBefore.Previous
			};

			if (insertee.Previous != null)
				insertee.Previous.Next = insertee;

			insertee.Next = insertBefore;
			insertBefore.Previous = insertee;

			_itemCount++;

			if (_firstItem == insertBefore)
				_firstItem = insertee;

			return insertee;
		}

		private void MNavigateForwards_Click(object sender, EventArgs e)
		{
			var item = _getCurrentItem?.Invoke();

			if (item != null && !_currentItem.Item.Equals(item) && !_currentItem.Next.Item.Equals(item))
				_currentItem = InsertItem(_currentItem.Next, item);

			_currentItem = _currentItem.Next;

			OnItemSelected(new ItemSelectedEventArgs<T>(_currentItem.Item));

			SetEnabledState();
		}

		public void Clear()
		{
			_firstItem = _currentItem = null;
			_itemCount = 0;

			SetEnabledState();
		}
	}

	public class ItemSelectedEventArgs<T> : EventArgs
	{
		public ItemSelectedEventArgs(T selectedItem)
			=> SelectedItem = selectedItem;

		public T SelectedItem { get; private set; }
	}

	public class LinkedItem<T>
	{
		public LinkedItem<T> Previous { get; set; }
		public LinkedItem<T> Next { get; set; }
		public T Item { get; private set; }

		public LinkedItem(LinkedItem<T> previous, T item, LinkedItem<T> next)
		{
			Previous = previous;
			Item = item;
			Next = next;
		}

		public LinkedItem(LinkedItem<T> previous, T item) : this(previous, item, null) { }

		public LinkedItem(T item, LinkedItem<T> next) : this(null, item, next) { }

		public LinkedItem(T item) : this(null, item, null) { }
	}
}
