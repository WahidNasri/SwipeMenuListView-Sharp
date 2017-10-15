using Android.App;
using Android.Widget;
using Android.OS;
using Wahid.SwipemenuListview;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Wahid
{
    [Activity(Label = "Swipe Menu ListView", MainLauncher = true)]
    public class MainActivity : Activity, ISwipeMenuCreator, IOnMenuItemClickListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            SwipeMenuListView listView = FindViewById<SwipeMenuListView>(Resource.Id.listView);
        }
        public void Create(SwipeMenu menu)
        {
            SwipeMenuItem callItem = new SwipeMenuItem(this)
            {
                Width = 200,
                Background = new ColorDrawable(Color.AliceBlue),
                IconRes = Resource.Drawable.phone_co
            };
            menu.AddMenuItem(callItem);

            SwipeMenuItem copyItem = new SwipeMenuItem(this)
            {
                Background = new ColorDrawable(Color.Azure),
                Width = (200),
                IconRes = Resource.Drawable.copy
            };
            menu.AddMenuItem(copyItem);
        }

        public bool OnMenuItemClick(int position, SwipeMenu menu, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}

