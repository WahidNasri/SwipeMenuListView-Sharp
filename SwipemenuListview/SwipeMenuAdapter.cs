using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenuAdapter : Java.Lang.Object, IWrapperListAdapter, SwipeMenuView.IOnSwipeItemClickListener
    {

        private IListAdapter mAdapter;
        private Context mContext;
        public IOnMenuItemClickListener MenuItemClickListener { get; set; }

        public IListAdapter WrappedAdapter => mAdapter;

        public int Count => mAdapter.Count;

        public bool HasStableIds => mAdapter.HasStableIds;

        public bool IsEmpty => mAdapter.IsEmpty;

        public int ViewTypeCount => mAdapter.ViewTypeCount;

        public SwipeMenuAdapter(Context context, IListAdapter adapter)
        {
            mAdapter = adapter;
            mContext = context;
        }


        public virtual void CreateMenu(SwipeMenu menu)
        {
        }

        public bool AreAllItemsEnabled()
        {
            return mAdapter.AreAllItemsEnabled();
        }

        public bool IsEnabled(int position)
        {
            return mAdapter.IsEnabled(position);
        }

        public Object GetItem(int position)
        {
            return mAdapter.GetItem(position);

        }

        public long GetItemId(int position)
        {
            return mAdapter.GetItemId(position);
        }

        public int GetItemViewType(int position)
        {
            return mAdapter.GetItemViewType(position);
        }

        public View GetView(int position, View convertView, ViewGroup parent)
        {
            SwipeMenuLayout layout = null;
            if (convertView == null)
            {
                View contentView = mAdapter.GetView(position, convertView, parent);
                SwipeMenu menu = new SwipeMenu(mContext);
                menu.SetViewType(GetItemViewType(position));
                CreateMenu(menu);
                SwipeMenuView menuView = new SwipeMenuView(menu, (SwipeMenuListView)parent)
                {
                    SwipeItemClickListener = this
                };
                SwipeMenuListView listView = (SwipeMenuListView)parent;
                layout = new SwipeMenuLayout(contentView, menuView, listView.CloseInterpolator, listView.OpenInterpolator);
                layout.SetPosition(position);
            }
            else
            {
                layout = (SwipeMenuLayout)convertView;
                layout.CloseMenu();
                layout.SetPosition(position);
                View view = mAdapter.GetView(position, layout.ContentView,
                        parent);
            }
            if (mAdapter is BaseSwipListAdapter)
            {
                bool swipEnable = (((BaseSwipListAdapter)mAdapter).getSwipEnableByPosition(position));
                layout.SwipEnable = swipEnable;
            }
            return layout;
        }

        public void RegisterDataSetObserver(DataSetObserver observer)
        {
            mAdapter.RegisterDataSetObserver(observer);
        }

        public void UnregisterDataSetObserver(DataSetObserver observer)
        {
            mAdapter.UnregisterDataSetObserver(observer);
        }

        public virtual void OnItemClick(SwipeMenuView view, SwipeMenu menu, int index)
        {
            if (MenuItemClickListener != null)
            {
                MenuItemClickListener.OnMenuItemClick(view.GetPosition(), menu,
                        index);
            }
        }
    }
}