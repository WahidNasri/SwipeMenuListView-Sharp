using Android.Text;
using Android.Widget;
using System.Collections.Generic;
using static Android.Views.View;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenuView : LinearLayout, IOnClickListener
    {

        private SwipeMenuListView mListView;
        public SwipeMenuLayout SwipeMenuLayout { internal get; set; }
        private SwipeMenu mMenu;
        public IOnSwipeItemClickListener SwipeItemClickListener { get; set; }
        private int position;

        public int GetPosition()
        {
            return position;
        }

        public void SetPosition(int position)
        {
            this.position = position;
        }

        public SwipeMenuView(SwipeMenu menu, SwipeMenuListView listView) : base(menu.Context)
        {
            mListView = listView;
            mMenu = menu;
            List<SwipeMenuItem> items = menu.GetMenuItems();
            int id = 0;
            foreach (SwipeMenuItem item in items)
            {
                AddItem(item, id++);
            }
        }

        private void AddItem(SwipeMenuItem item, int id)
        {
            LayoutParams paramss = new LayoutParams(item.Width, LayoutParams.MatchParent);
            LayoutParams paramsP = new LayoutParams(item.Width -2, LayoutParams.MatchParent);
            LinearLayout boss = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = paramss
            };
            LinearLayout parent = new LinearLayout(Context)
            {
                Id = id,
                Orientation = Orientation.Vertical,
                LayoutParameters = paramsP,
                Background = item.Background
            };
            parent.SetGravity(Android.Views.GravityFlags.Center);
            parent.SetOnClickListener(this);
            AddView(boss);

            if (item.Icon != null)
            {
                parent.AddView(CreateIcon(item));
            }
            if (!TextUtils.IsEmpty(item.Title))
            {
                parent.AddView(CreateTitle(item));
            }
            View view = new View(Context)
            {
                LayoutParameters = new LayoutParams(2, LayoutParams.MatchParent),
                Background = new ColorDrawable(Color.LightGray)
            };
            boss.AddView(view);
            boss.AddView(parent);
           
        }

        private ImageView CreateIcon(SwipeMenuItem item)
        {
            ImageView iv = new ImageView(Context);
            iv.SetImageDrawable(item.Icon);
            return iv;
        }

        private TextView CreateTitle(SwipeMenuItem item)
        {
            return new TextView(Context)
            {
                Text = (item.Title),
                Gravity = Android.Views.GravityFlags.Center,
                TextSize = item.TitleSize,
                //tv.SetTextColor(item.TitleColor);
            };
        }       
        public void OnClick(View v)
        {
            if (SwipeItemClickListener != null && SwipeMenuLayout.IsOpen())
            {
                SwipeItemClickListener.OnItemClick(this, mMenu, v.Id);
            }
        }

        public interface IOnSwipeItemClickListener
        {
            void OnItemClick(SwipeMenuView view, SwipeMenu menu, int index);
        }
    }
}