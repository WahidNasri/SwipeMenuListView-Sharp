# SwipeMenuListView-Sharp
Xamarin.Android Swipe menu Listview.

I used this project, modified a little to make the code more C# style and recommendations:
https://github.com/baoyongzhang/SwipeMenuListView

1. Add view in AXML Layout file:
<Wahid.SwipemenuListview.SwipeMenuListView
      android:id="@+id/listView"
      android:layout_width="match_parent"
      android:layout_height="match_parent" />
			
2. Set the Adapter as usual
listview.Adapter = new MyCustomAdapter(contacts);

3. Set Menu Creator
*Option 1: implement ISwipeMenuCreator, in Create(SwipeMenu menu) method, add swipe menu items
 public void Create(SwipeMenu menu)
        {
            SwipeMenuItem callItem = new SwipeMenuItem(Activity)
            {
                Width = 200,
                Background = new ColorDrawable(Resources.GetColor(Resource.Color.bg_swipe_menu)),
                IconRes = menu.GetViewType() == 0 ? Resource.Drawable.phone_co : Resource.Drawable.email_co
            };
            menu.AddMenuItem(callItem);

            SwipeMenuItem copyItem = new SwipeMenuItem(Activity)
            {
                Background = new ColorDrawable(Resources.GetColor(Resource.Color.bg_swipe_menu)),
                Width = (200),
                IconRes = Resource.Drawable.copy
            };
            menu.AddMenuItem(copyItem);

           
        }

*Option 2: use Lumbda Expression
listview.SetMenuItems((menu) =>
            {
                return new List<SwipeMenuItem>()
                {
                    new SwipeMenuItem(Activity)
                            {
                                Width = 200,
                                Background = new ColorDrawable(Resources.GetColor(Resource.Color.bg_swipe_menu)),
                                IconRes = menu.GetViewType() == 0 ? Resource.Drawable.phone_co : Resource.Drawable.email_co
                            },
                    new SwipeMenuItem(Activity)
                            {
                                Background = new ColorDrawable(Resources.GetColor(Resource.Color.bg_swipe_menu)),
                                Width = 200,
                                IconRes = Resource.Drawable.copy
                            }
                };
            });
