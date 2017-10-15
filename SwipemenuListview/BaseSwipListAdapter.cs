
using Android.Widget;
/**
* Created by Abner on 15/11/20.
* Email nimengbo@gmail.com
* github https://github.com/nimengbo
*/
namespace Wahid.SwipemenuListview
{
    public abstract class BaseSwipListAdapter : BaseAdapter
    {
        public bool getSwipEnableByPosition(int position)
        {
            return true;
        }
    }
}