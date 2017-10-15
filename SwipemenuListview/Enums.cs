using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Wahid.SwipemenuListview
{
    public enum SwipeDirection
    {
        Right = -1,
        Left = 1
    }
    public enum TouchState
    {
        None = 0,
        X = 1,
        Y = 2
    }
    public enum SwipeMenuLayoutState
    {
        Close,
        Open
    }
}