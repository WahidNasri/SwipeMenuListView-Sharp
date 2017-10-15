using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using static Android.Views.GestureDetector;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenuLayout : FrameLayout
    {
        private static readonly int CONTENT_VIEW_ID = 1;
        private static readonly int MENU_VIEW_ID = 2;
        
        private SwipeDirection mSwipeDirection;

        public View ContentView { get; internal set; }
        public SwipeMenuView MenuView { get; internal set; }
        private int mDownX;
        private SwipeMenuLayoutState state = SwipeMenuLayoutState.Close;
        private GestureDetectorCompat mGestureDetector;
        private IOnGestureListener mGestureListener;
        public bool isFling;
        public int MIN_FLING;
        public int MAX_VELOCITYX;
        private ScrollerCompat mOpenScroller;
        private ScrollerCompat mCloseScroller;
        private int mBaseX;
        private int position;
        private IInterpolator mCloseInterpolator;
        private IInterpolator mOpenInterpolator;

        public bool SwipEnable { get; set; } = true;

        public SwipeMenuLayout(View contentView, SwipeMenuView menuView) : this(contentView, menuView, null, null)
        {

        }

        public SwipeMenuLayout(View contentView, SwipeMenuView menuView, IInterpolator closeInterpolator, IInterpolator openInterpolator) : base(contentView.Context)
        {
            mCloseInterpolator = closeInterpolator;
            mOpenInterpolator = openInterpolator;
            ContentView = contentView;
            MenuView = menuView;
            MenuView.SwipeMenuLayout = this;
            Init();
        }

        // private SwipeMenuLayout(Context context, AttributeSet attrs, int
        // defStyle) {
        // super(context, attrs, defStyle);
        // }

        private SwipeMenuLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        private SwipeMenuLayout(Context context) : base(context)
        {
        }
        public int GetPosition()
        {
            return position;
        }

        public void SetPosition(int position)
        {
            this.position = position;
            MenuView.SetPosition(position);
        }

        public void SetSwipeDirection(SwipeDirection swipeDirection)
        {
            mSwipeDirection = swipeDirection;
        }
        private void Init()
        {
            MIN_FLING = Dp2px(15);
            MAX_VELOCITYX = -Dp2px(500);

            LayoutParameters = (new AbsListView.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
            mGestureListener = new GestureListerner(this);
            mGestureDetector = new GestureDetectorCompat(Context, mGestureListener);

            // mScroller = ScrollerCompat.create(getContext(), new
            // BounceInterpolator());
            if (mCloseInterpolator != null)
            {
                mCloseScroller = ScrollerCompat.Create(Context, mCloseInterpolator);
            }
            else
            {
                mCloseScroller = ScrollerCompat.Create(Context);
            }
            if (mOpenInterpolator != null)
            {
                mOpenScroller = ScrollerCompat.Create(Context, mOpenInterpolator);
            }
            else
            {
                mOpenScroller = ScrollerCompat.Create(Context);
            }

            LayoutParams contentParams = new LayoutParams(
                    LayoutParams.MatchParent, LayoutParams.WrapContent);
            ContentView.LayoutParameters = (contentParams);
            if (ContentView.Id < 1)
            {
                ContentView.Id = CONTENT_VIEW_ID;
            }

            MenuView.Id = (MENU_VIEW_ID);
            MenuView.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            AddView(ContentView);
            AddView(MenuView);
        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        public bool OnSwipe(MotionEvent e)
        {
            mGestureDetector.OnTouchEvent(e);
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mDownX = (int)e.GetX();
                    isFling = false;
                    break;
                case MotionEventActions.Move:
                    // Log.i("byz", "downX = " + mDownX + ", moveX = " + event.getX());
                    int dis = (int)(mDownX - e.GetX());
                    if (state == SwipeMenuLayoutState.Open)
                    {
                        dis += MenuView.Width * (int)mSwipeDirection; ;
                    }

                    Swipe(dis);
                    break;
                case MotionEventActions.Up:
                    if ((isFling || Math.Abs(mDownX - e.GetX()) > (MenuView.Width / 2)) && Math.Sign(mDownX - e.GetX()) == (int)mSwipeDirection)
                    {
                        // open
                        SmoothOpenMenu();
                    }
                    else
                    {
                        // close
                        SmoothCloseMenu();
                        return false;
                    }
                    break;
            }
            return true;
        }

        public bool IsOpen()
        {
            return state == SwipeMenuLayoutState.Open;
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            return base.OnTouchEvent(e);
        }
        private void Swipe(int dis)
        {
            if (!SwipEnable)
            {
                return;
            }
            if (Math.Sign(dis) != (int)mSwipeDirection)
            {
                dis = 0;
            }
            else if (Math.Abs(dis) > MenuView.Width)
            {
                dis = MenuView.Width * (int)mSwipeDirection;
            }

            ContentView.Layout(-dis, ContentView.Top,
                    ContentView.Width - dis, MeasuredHeight);

            if (mSwipeDirection == SwipeDirection.Left)
            {

                MenuView.Layout(ContentView.Width - dis, MenuView.Top,
                        ContentView.Width + MenuView.Width - dis,
                        MenuView.Bottom);
            }
            else
            {
                MenuView.Layout(-MenuView.Width - dis, MenuView.Top,
                        -dis, MenuView.Bottom);
            }
        }
        public override void ComputeScroll()
        {
            if (state == SwipeMenuLayoutState.Open)
            {
                if (mOpenScroller.ComputeScrollOffset())
                {
                    Swipe(mOpenScroller.CurrX * (int)mSwipeDirection);
                    PostInvalidate();
                }
            }
            else
            {
                if (mCloseScroller.ComputeScrollOffset())
                {
                    Swipe((mBaseX - mCloseScroller.CurrX) * (int)mSwipeDirection);
                    PostInvalidate();
                }
            }
        }
        public void SmoothCloseMenu()
        {
            state = SwipeMenuLayoutState.Close;
            if (mSwipeDirection == SwipeDirection.Left)
            {
                mBaseX = -ContentView.Left;
                mCloseScroller.StartScroll(0, 0, MenuView.Width, 0, 350);
            }
            else
            {
                mBaseX = MenuView.Right;
                mCloseScroller.StartScroll(0, 0, MenuView.Width, 0, 350);
            }
            PostInvalidate();
        }

        public void SmoothOpenMenu()
        {
            if (!SwipEnable)
            {
                return;
            }
            state = SwipeMenuLayoutState.Open;
            if (mSwipeDirection == SwipeDirection.Left)
            {
                mOpenScroller.StartScroll(-ContentView.Left, 0, MenuView.Width, 0, 350);
            }
            else
            {
                mOpenScroller.StartScroll(ContentView.Left, 0, MenuView.Width, 0, 350);
            }
            PostInvalidate();
        }

        public void CloseMenu()
        {
            if (mCloseScroller.ComputeScrollOffset())
            {
                mCloseScroller.AbortAnimation();
            }
            if (state == SwipeMenuLayoutState.Open)
            {
                state = SwipeMenuLayoutState.Close;
                Swipe(0);
            }
        }

        public void OpenMenu()
        {
            if (!SwipEnable)
            {
                return;
            }
            if (state == SwipeMenuLayoutState.Close)
            {
                state = SwipeMenuLayoutState.Open;
                Swipe(MenuView.Width * (int)mSwipeDirection);
            }
        }
        
        public int Dp2px(int dp)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp,
                    Context.Resources.DisplayMetrics);
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            MenuView.Measure(MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), MeasureSpec.MakeMeasureSpec(MeasuredHeight, MeasureSpecMode.Exactly));
        }
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            ContentView.Layout(0, 0, MeasuredWidth, ContentView.MeasuredHeight);
            if (mSwipeDirection == SwipeDirection.Left)
            {
                MenuView.Layout(MeasuredWidth, 0,
                        MeasuredWidth + MenuView.MeasuredWidth,
                        ContentView.MeasuredHeight);
            }
            else
            {
                MenuView.Layout(-MenuView.MeasuredWidth, 0,
                        0, ContentView.MeasuredHeight);
            }
        }
        public void SetMenuHeight(int measuredHeight)
        {
            LayoutParams param = (LayoutParams)MenuView.LayoutParameters;
            if (param.Height != measuredHeight) {
			param.Height = measuredHeight;
                MenuView.LayoutParameters = (MenuView.LayoutParameters);
            }
        }        
    }
    class GestureListerner : SimpleOnGestureListener
    {
        private SwipeMenuLayout swipeMenuLayout;
        public GestureListerner(SwipeMenuLayout swipeMenuLayout)
        {
            this.swipeMenuLayout = swipeMenuLayout;
        }
        public override bool OnDown(MotionEvent e)
        {
            swipeMenuLayout.isFling = false;
            return false;
        }
        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (Math.Abs(e1.GetX() - e2.GetX()) > swipeMenuLayout.MIN_FLING && velocityX < swipeMenuLayout.MAX_VELOCITYX)
            {
                swipeMenuLayout.isFling = true;
            }
            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }
}