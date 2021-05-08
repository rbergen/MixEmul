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

		private LinkedItem<T> firstItem;
		private LinkedItem<T> currentItem;
		private int itemCount;
		private Button navigateBackwards;
		private Button navigateForwards;
		private CurrentItemCallback getCurrentItem;

		public LinkedItemsSelectorControl()
			=> InitializeComponent();

		protected void OnItemSelected(ItemSelectedEventArgs<T> e)
			=> ItemSelected?.Invoke(this, e);

		private void InitializeComponent()
		{
			this.navigateForwards = new Button();
			this.navigateBackwards = new Button();

			SuspendLayout();
			// 
			// mNavigateBackwards
			// 
			this.navigateBackwards.Enabled = false;
			this.navigateBackwards.FlatStyle = FlatStyle.Flat;
			this.navigateBackwards.Image = Resources.NavigateBackwards_6270;
			this.navigateBackwards.Location = new Point(0, 0);
			this.navigateBackwards.Name = "mNavigateBackwards";
			this.navigateBackwards.Size = new Size(21, 21);
			this.navigateBackwards.TabIndex = 0;
			this.navigateBackwards.Click += MNavigateBackwards_Click;
			// 
			// mNavigateForwards
			// 
			this.navigateForwards.Enabled = false;
			this.navigateForwards.FlatStyle = FlatStyle.Flat;
			this.navigateForwards.Image = Resources.NavigateForward_6271;
			this.navigateForwards.Location = new Point(this.navigateBackwards.Right - 1, 0);
			this.navigateForwards.Name = "mNavigateForwards";
			this.navigateForwards.Size = this.navigateBackwards.Size;
			this.navigateForwards.TabIndex = 1;
			this.navigateForwards.Click += MNavigateForwards_Click;
			// 
			// LinkedItemsSelectorControl
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(this.navigateForwards);
			Controls.Add(this.navigateBackwards);
			Name = "LinkedItemsSelectorControl";
			Size = new Size(this.navigateForwards.Right, this.navigateForwards.Height);

			ResumeLayout(false);
		}

		public void SetCurrentItemCallback(CurrentItemCallback getCurrentItem)
			=> this.getCurrentItem = getCurrentItem;

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
			while (this.itemCount > MaxItemCount)
			{
				this.firstItem.Next.Previous = null;
				this.firstItem = this.firstItem.Next;
				this.itemCount--;
			}
		}

		private void AddItem(T item)
		{
			if (item == null)
				return;

			if (this.firstItem == null)
			{
				this.firstItem = new LinkedItem<T>(item);
				this.currentItem = this.firstItem;
				this.itemCount++;

				return;
			}

			if (this.currentItem.Item.Equals(item))
				return;

			if (this.currentItem.Next != null)
			{
				int purgeItemCount = 0;
				LinkedItem<T> oldNext = this.currentItem.Next;
				while (oldNext != null)
				{
					purgeItemCount++;
					oldNext = oldNext.Next;
				}

				this.currentItem.Next.Previous = null;
				this.itemCount -= purgeItemCount;
			}

			this.currentItem.Next = new LinkedItem<T>(item);
			this.itemCount++;
			this.currentItem.Next.Previous = this.currentItem;
			this.currentItem = this.currentItem.Next;
		}

		private void SetEnabledState()
		{
			this.navigateBackwards.Enabled = this.currentItem != null && this.currentItem.Previous != null;
			this.navigateForwards.Enabled = this.currentItem != null && this.currentItem.Next != null;
		}

		private void MNavigateBackwards_Click(object sender, EventArgs e)
		{
			var item = this.getCurrentItem?.Invoke();

			if (item != null && !this.currentItem.Item.Equals(item) && !this.currentItem.Previous.Item.Equals(item))
				this.currentItem = InsertItem(this.currentItem, item);

			this.currentItem = this.currentItem.Previous;

			OnItemSelected(new ItemSelectedEventArgs<T>(this.currentItem.Item));

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

			this.itemCount++;

			if (this.firstItem == insertBefore)
				this.firstItem = insertee;

			return insertee;
		}

		private void MNavigateForwards_Click(object sender, EventArgs e)
		{
			var item = this.getCurrentItem?.Invoke();

			if (item != null && !this.currentItem.Item.Equals(item) && !this.currentItem.Next.Item.Equals(item))
				this.currentItem = InsertItem(this.currentItem.Next, item);

			this.currentItem = this.currentItem.Next;

			OnItemSelected(new ItemSelectedEventArgs<T>(this.currentItem.Item));

			SetEnabledState();
		}

		public void Clear()
		{
			this.firstItem = this.currentItem = null;
			this.itemCount = 0;

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
