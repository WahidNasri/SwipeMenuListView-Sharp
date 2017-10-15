# SwipeMenuListView-Sharp
Xamarin.Android Swipe menu Listview.

I used this project, modified a little to make the code more C# style and recommendations:
https://github.com/baoyongzhang/SwipeMenuListView

Result:
![SwipemenuListView](https://raw.githubusercontent.com/baoyongzhang/SwipeMenuListView/master/demo.gif)
# 1. Add view in AXML Layout file:
```
<Wahid.SwipemenuListview.SwipeMenuListView
      android:id="@+id/listView"
      android:layout_width="match_parent"
      android:layout_height="match_parent" />
```			
# 2. Set the Adapter as usual
```
listview.Adapter = new MyCustomAdapter(contacts);
```
# 3. Set Menu Creator
## Option 1: implement ISwipeMenuCreator, in Create(SwipeMenu menu) method, add swipe menu items
```
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
```
```
listview.MenuCreator = new MenuCreatorImplementation();
```
## Option 2: use Lumbda Expression
```
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
```
# 3. Set MenuItemClickListener
Implement 'IOnMenuItemClickListener' interface, on the OnMenuItemClick(int position, SwipeMenu menu, int index) method we put our logic.
Example:
```
	public bool OnMenuItemClick(int position, SwipeMenu menu, int index)
        {
            //var contact = ((listview.Adapter as SwipeMenuAdapter).WrappedAdapter as ContactUsSwipeAdapter).Items[position];
            var contact = contacts[position];
            switch (index)
            {
                case 0:
                    var tp = menu.GetViewType();
                    if(menu.GetViewType() == 0)
                    {
                    }
                    else
                    {                     
                    }
                    break;
                case 1: // copy
                 break;
            }
            return false;
        }
```
```	
listview.MenuItemClickListener = new MenuItemClickListenerImplementation();
```
