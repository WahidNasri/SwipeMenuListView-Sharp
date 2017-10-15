using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenuListView : ListView
    {
        public SwipeDirection SwipeDeirection { get; set; } = SwipeDirection.Left;//swipe from right to left by default
        public IOnSwipeListener SwipeListener { internal get; set; }
        public ISwipeMenuCreator MenuCreator { get; set; }
        public IOnMenuItemClickListener MenuItemClickListener { get; set; }
        public IOnMenuStateChangeListener MenuStateChangeListener { internal get; set; }
        public IInterpolator CloseInterpolator { get; set; }
        public IInterpolator OpenInterpolator { get; set; }

        private int MAX_Y = 5;
        private int MAX_X = 3;
        private float mDownX;
        private float mDownY;
        private TouchState mTouchState;
        private int mTouchPosition;
        private SwipeMenuLayout MenuLayout;
       

        public SwipeMenuListView(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
        }
        public SwipeMenuListView(Context context) : base(context)
        {
            Init();
        }

        public SwipeMenuListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init();
        }

        public SwipeMenuListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }
        private void Init()
        {
            MAX_X = Dp2px(MAX_X);
            MAX_Y = Dp2px(MAX_Y);
            mTouchState = TouchState.None;
        }
        public void SetMenuItems(Func<SwipeMenu, List<SwipeMenuItem>> func)
        {
            MenuCreator = new MenuCreatorGenerator(func);
        }
        public override void SetAdapter(IListAdapter adapter)
        {
            base.SetAdapter(new MyAdapter(this, adapter));
        }
        private SwipeMenuAdapter _adapter;
        public override IListAdapter Adapter { get => _adapter?.WrappedAdapter; set { base.Adapter = new MyAdapter(this, value); _adapter = new MyAdapter(this, value); } }
        public static bool InRangeOfView(View view, MotionEvent ev)
        {
            int[] location = new int[2];
            view.GetLocationOnScreen(location);
            int x = location[0];
            int y = location[1];
            if (ev.RawX < x || ev.RawX > (x + view.Width) || ev.RawY < y || ev.RawY > (y + view.Height))
            {
                return false;
            }
            return true;
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            var action = ev.Action;
            switch (action)
            {
                case MotionEventActions.Down:
                    mDownX = ev.GetX();
                    mDownY = ev.GetY();
                    bool handled = base.OnInterceptTouchEvent(ev);
                    mTouchState = TouchState.None;
                    mTouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY());
                    View view = GetChildAt(mTouchPosition - FirstVisiblePosition);
                    
                    if (view is SwipeMenuLayout) {
                        if (MenuLayout != null && MenuLayout.IsOpen() && !InRangeOfView(MenuLayout.MenuView, ev))
                        {
                            return true;
                        }
                        MenuLayout = (SwipeMenuLayout)view;
                        MenuLayout.SetSwipeDirection(SwipeDeirection);
                    }
                    if (MenuLayout != null && MenuLayout.IsOpen() && view != MenuLayout)
                    {
                        handled = true;
                    }

                    if (MenuLayout != null)
                    {
                        MenuLayout.OnSwipe(ev);
                    }
                    return handled;
                case MotionEventActions.Move:
                    float dy = Math.Abs((ev.GetY() - mDownY));
                    float dx = Math.Abs((ev.GetX() - mDownX));
                    if (Math.Abs(dy) > MAX_Y || Math.Abs(dx) > MAX_X)
                    {
                        if (mTouchState == TouchState.None)
                        {
                            if (Math.Abs(dy) > MAX_Y)
                            {
                                mTouchState = TouchState.Y;
                            }
                            else if (dx > MAX_X)
                            {
                                mTouchState = TouchState.X;
                                if (SwipeListener != null)
                                {
                                    SwipeListener.OnSwipeStart(mTouchPosition);
                                }
                            }
                        }
                        return true;
                    }
                    break;
            }
            return base.OnInterceptTouchEvent(ev);
        }
        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (ev.Action !=  MotionEventActions.Down && MenuLayout == null)
                return base.OnTouchEvent(ev);
            var action = ev.Action;
            switch (action)
            {
                case MotionEventActions.Down:
                    int oldPos = mTouchPosition;
                    mDownX = ev.GetX();
                    mDownY = ev.GetY();
                    mTouchState = TouchState.None;

                    mTouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY());

                    if (mTouchPosition == oldPos && MenuLayout != null
                            && MenuLayout.IsOpen())
                    {
                        mTouchState = TouchState.X;
                        MenuLayout.OnSwipe(ev);
                        return true;
                    }

                    View view = GetChildAt(mTouchPosition - FirstVisiblePosition);

                    if (MenuLayout != null && MenuLayout.IsOpen())
                    {
                        MenuLayout.SmoothCloseMenu();
                        MenuLayout = null;
                        // return super.onTouchEvent(ev);
                        // try to cancel the touch event
                        MotionEvent cancelEvent = MotionEvent.Obtain(ev);
                        cancelEvent.Action = MotionEventActions.Cancel;
                        OnTouchEvent(cancelEvent);
                        if (MenuStateChangeListener != null)
                        {
                            MenuStateChangeListener.OnMenuClose(oldPos);
                        }
                        return true;
                    }
                    if (view is SwipeMenuLayout) {
                        MenuLayout = (SwipeMenuLayout)view;
                        MenuLayout.SetSwipeDirection(SwipeDeirection);
                    }
                    if (MenuLayout != null)
                    {
                        MenuLayout.OnSwipe(ev);
                    }
                    break;
                case MotionEventActions.Move:
                    mTouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY()) - HeaderViewsCount;
                    if (!MenuLayout.SwipEnable || mTouchPosition != MenuLayout.GetPosition())
                    {
                        break;
                    }
                    float dy = Math.Abs((ev.GetY() - mDownY));
                    float dx = Math.Abs((ev.GetX() - mDownX));
                    if (mTouchState == TouchState.X)
                    {
                        if (MenuLayout != null)
                        {
                            MenuLayout.OnSwipe(ev);
                        }
                        Selector.SetState(new int[] { 0 });
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                        return true;
                    }
                    else if (mTouchState == TouchState.None)
                    {
                        if (Math.Abs(dy) > MAX_Y)
                        {
                            mTouchState = TouchState.Y;
                        }
                        else if (dx > MAX_X)
                        {
                            mTouchState = TouchState.X;
                            if (SwipeListener != null)
                            {
                                SwipeListener.OnSwipeStart(mTouchPosition);
                            }
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    if (mTouchState == TouchState.X)
                    {
                        if (MenuLayout != null)
                        {
                            bool isBeforeOpen = MenuLayout.IsOpen();
                            MenuLayout.OnSwipe(ev);
                            bool isAfterOpen = MenuLayout.IsOpen();
                            if (isBeforeOpen != isAfterOpen && MenuStateChangeListener != null)
                            {
                                if (isAfterOpen)
                                {
                                    MenuStateChangeListener.OnMenuOpen(mTouchPosition);
                                }
                                else
                                {
                                    MenuStateChangeListener.OnMenuClose(mTouchPosition);
                                }
                            }
                            if (!isAfterOpen)
                            {
                                mTouchPosition = -1;
                                MenuLayout = null;
                            }
                        }
                        if (SwipeListener != null)
                        {
                            SwipeListener.OnSwipeEnd(mTouchPosition);
                        }
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                        return true;
                    }
                    break;
            }
            try
            {
                return base.OnTouchEvent(ev);
            }
            catch (Exception) { return false; }
        }
        public void SmoothOpenMenu(int position)
        {
            if (position >= FirstVisiblePosition
                    && position <= LastVisiblePosition)
            {
                View view = GetChildAt(position - FirstVisiblePosition);
                if (view is SwipeMenuLayout) {
                    mTouchPosition = position;
                    if (MenuLayout != null && MenuLayout.IsOpen())
                    {
                        MenuLayout.SmoothCloseMenu();
                    }
                    MenuLayout = (SwipeMenuLayout)view;
                    MenuLayout.SetSwipeDirection(SwipeDeirection);
                    MenuLayout.SmoothOpenMenu();
                }
            }
        }
        public void SmoothCloseMenu()
        {
            if (MenuLayout != null && MenuLayout.IsOpen())
            {
                MenuLayout.SmoothCloseMenu();
            }
        }

        private int Dp2px(int dp)
        {
            return (int)TypedValue.ApplyDimension( ComplexUnitType.Dip, dp,
                    Context.Resources.DisplayMetrics);
        }

        private class MyAdapter : SwipeMenuAdapter
        {
            private SwipeMenuListView listview;
            public MyAdapter(SwipeMenuListView swipeMenuListView, IListAdapter adapter) : base(swipeMenuListView.Context, adapter)
            {
                this.listview = swipeMenuListView;
            }
            public override void CreateMenu(SwipeMenu menu)
            {
                if (listview.MenuCreator != null)
                {
                    listview.MenuCreator.Create(menu);
                }
            }
            public override void OnItemClick(SwipeMenuView view, SwipeMenu menu, int index)
            {
                bool flag = false;
                if (listview.MenuItemClickListener != null)
                {
                    flag = listview.MenuItemClickListener.OnMenuItemClick(view.GetPosition(), menu, index);
                }
                if (listview.MenuLayout != null && !flag)
                {
                    listview.MenuLayout.SmoothCloseMenu();
                }
            }
        }
        private class MenuCreatorGenerator : ISwipeMenuCreator
        {
            private Func<SwipeMenu, List<SwipeMenuItem>> itemsFunc;
            public MenuCreatorGenerator(Func<SwipeMenu, List<SwipeMenuItem>> itemsFunc)
            {
                this.itemsFunc = itemsFunc;
            }
            public void Create(SwipeMenu menu)
            {
                menu.AddMenuItems(itemsFunc.Invoke(menu));
            }
        }
    }

    public interface IOnMenuItemClickListener
    {
        bool OnMenuItemClick(int position, SwipeMenu menu, int index);
    }

    public interface IOnSwipeListener
    {
        void OnSwipeStart(int position);

        void OnSwipeEnd(int position);
    }

    public interface IOnMenuStateChangeListener
    {
        void OnMenuOpen(int position);

        void OnMenuClose(int position);
    }}