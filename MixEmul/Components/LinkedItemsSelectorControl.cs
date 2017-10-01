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

        const int maxItemCount = 1000;

        LinkedItem<T> mFirstItem;
        LinkedItem<T> mCurrentItem;
        int mItemCount;
        Button mNavigateBackwards;
        Button mNavigateForwards;
        CurrentItemCallback mGetCurrentItem;

        public LinkedItemsSelectorControl()
		{
			InitializeComponent();
		}

        protected void OnItemSelected(ItemSelectedEventArgs<T> e) => ItemSelected?.Invoke(this, e);

        void InitializeComponent()
        {
            mNavigateForwards = new Button();
            mNavigateBackwards = new Button();
            SuspendLayout();
            // 
            // mNavigateBackwards
            // 
            mNavigateBackwards.Enabled = false;
            mNavigateBackwards.FlatStyle = FlatStyle.Flat;
            mNavigateBackwards.Image = Resources.NavigateBackwards_6270;
            mNavigateBackwards.Location = new Point(0, 0);
            mNavigateBackwards.Name = "mNavigateBackwards";
            mNavigateBackwards.Size = new Size(21, 21);
            mNavigateBackwards.TabIndex = 0;
            mNavigateBackwards.Click += MNavigateBackwards_Click;
            // 
            // mNavigateForwards
            // 
            mNavigateForwards.Enabled = false;
            mNavigateForwards.FlatStyle = FlatStyle.Flat;
            mNavigateForwards.Image = Resources.NavigateForward_6271;
            mNavigateForwards.Location = new Point(mNavigateBackwards.Right - 1, 0);
            mNavigateForwards.Name = "mNavigateForwards";
            mNavigateForwards.Size = mNavigateBackwards.Size;
            mNavigateForwards.TabIndex = 1;
            mNavigateForwards.Click += MNavigateForwards_Click;
            // 
            // LinkedItemsSelectorControl
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mNavigateForwards);
            Controls.Add(mNavigateBackwards);
            Name = "LinkedItemsSelectorControl";
            Size = new Size(mNavigateForwards.Right, mNavigateForwards.Height);
            ResumeLayout(false);

        }

        public void SetCurrentItemCallback(CurrentItemCallback getCurrentItem)
		{
			mGetCurrentItem = getCurrentItem;
		}

		public void AddItem(T currentItem, T newItem)
		{
			if (currentItem.Equals(newItem)) return;

			AddItem(currentItem);
			AddItem(newItem);

			PruneToMaxItemCount();

			SetEnabledState();
		}

        void PruneToMaxItemCount()
        {
            while (mItemCount > maxItemCount)
            {
                mFirstItem.Next.Previous = null;
                mFirstItem = mFirstItem.Next;
                mItemCount--;
            }
        }

        void RemoveItem(LinkedItem<T> item)
        {
            if (item.Next != null)
            {
                item.Next.Previous = item.Previous;
            }
            if (item.Previous != null)
            {
                item.Previous.Next = item.Next;
                item.Previous = null;
            }

            item.Next = null;

            mItemCount--;
        }

        void AddItem(T item)
        {
            if (item == null) return;

            if (mFirstItem == null)
            {
                mFirstItem = new LinkedItem<T>(item);
                mCurrentItem = mFirstItem;
                mItemCount++;
            }
            else
            {
                if (mCurrentItem.Item.Equals(item)) return;

                if (mCurrentItem.Next != null)
                {
                    int purgeItemCount = 0;
                    LinkedItem<T> oldNext = mCurrentItem.Next;
                    while (oldNext != null)
                    {
                        purgeItemCount++;
                        oldNext = oldNext.Next;
                    }

                    mCurrentItem.Next.Previous = null;
                    mItemCount -= purgeItemCount;
                }

                mCurrentItem.Next = new LinkedItem<T>(item);
                mItemCount++;
                mCurrentItem.Next.Previous = mCurrentItem;
                mCurrentItem = mCurrentItem.Next;
            }
        }

        void SetEnabledState()
        {
            mNavigateBackwards.Enabled = mCurrentItem != null && mCurrentItem.Previous != null;
            mNavigateForwards.Enabled = mCurrentItem != null && mCurrentItem.Next != null;
        }

        void MNavigateBackwards_Click(object sender, EventArgs e)
        {
            if (mGetCurrentItem != null)
            {
                var item = mGetCurrentItem();

                if (item != null && !mCurrentItem.Item.Equals(item) && !mCurrentItem.Previous.Item.Equals(item))
                {
                    mCurrentItem = InsertItem(mCurrentItem, item);
                }
            }

            mCurrentItem = mCurrentItem.Previous;

            OnItemSelected(new ItemSelectedEventArgs<T>(mCurrentItem.Item));

            SetEnabledState();
        }

        LinkedItem<T> InsertItem(LinkedItem<T> insertBefore, T item)
        {
            if (insertBefore == null || item == null) return null;

            var insertee = new LinkedItem<T>(item)
            {
                Previous = insertBefore.Previous
            };
            if (insertee.Previous != null)
            {
                insertee.Previous.Next = insertee;
            }

            insertee.Next = insertBefore;
            insertBefore.Previous = insertee;

            mItemCount++;

            if (mFirstItem == insertBefore)
            {
                mFirstItem = insertee;
            }

            return insertee;
        }

        void MNavigateForwards_Click(object sender, EventArgs e)
        {
            if (mGetCurrentItem != null)
            {
                var item = mGetCurrentItem();

                if (item != null && !mCurrentItem.Item.Equals(item) && !mCurrentItem.Next.Item.Equals(item))
                {
                    mCurrentItem = InsertItem(mCurrentItem.Next, item);
                }
            }

            mCurrentItem = mCurrentItem.Next;

            OnItemSelected(new ItemSelectedEventArgs<T>(mCurrentItem.Item));

            SetEnabledState();
        }

        public void Clear()
		{
			mFirstItem = mCurrentItem = null;
			mItemCount = 0;

			SetEnabledState();
		}
	}

	public class ItemSelectedEventArgs<T> : EventArgs
	{
		public ItemSelectedEventArgs(T selectedItem)
		{
			SelectedItem = selectedItem;
		}

		public T SelectedItem
		{
			get;
			private set;
		}
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

		public LinkedItem(LinkedItem<T> previous, T item)
			: this(previous, item, null)
		{ }

		public LinkedItem(T item, LinkedItem<T> next)
			: this(null, item, next)
		{ }

		public LinkedItem(T item)
			: this(null, item, null)
		{ }
	}
}
