using Android.Content;
using System.Collections.Generic;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenu
    {

        public Context Context { get; internal set; }
        private List<SwipeMenuItem> mItems;
        private int mViewType;

        public SwipeMenu(Context context)
        {
            Context = context;
            mItems = new List<SwipeMenuItem>();
        }

        public void AddMenuItem(SwipeMenuItem item)
        {
            mItems.Add(item);
        }
        public void AddMenuItems(List<SwipeMenuItem> items)
        {
            mItems.AddRange(items);
        }

        public void RemoveMenuItem(SwipeMenuItem item)
        {
            mItems.Remove(item);
        }

        public List<SwipeMenuItem> GetMenuItems()
        {
            return mItems;
        }

        public SwipeMenuItem GetMenuItem(int index)
        {
            return mItems[index];
        }

        public int GetViewType()
        {
            return mViewType;
        }

        public void SetViewType(int viewType)
        {
            this.mViewType = viewType;
        }

    }
}