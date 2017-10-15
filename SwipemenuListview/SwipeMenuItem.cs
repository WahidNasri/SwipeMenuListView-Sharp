using Android.Content;
using Android.Graphics.Drawables;

namespace Wahid.SwipemenuListview
{
    public class SwipeMenuItem
    {

        public int Id { get; set; }
        public Context Context { get; internal set; }
        public string Title { get; set; }
        public Drawable Icon { get; set; }
        public Drawable Background { get; set; }
        public int TitleColor { get; set; }
        public int TitleSize { get; set; }
        public int Width { get; set; }

        public int IconRes
        {
            set
            {
                Icon = Context.Resources.GetDrawable(value);
            }
        }
        public int TitleRes
        {
            set
            {
                Title = Context.GetString(value);
            }
        }
        public int BackgroundRes
        {
            set
            {
                Background = Context.Resources.GetDrawable(value);
            }
        }

        public SwipeMenuItem(Context context)
        {
            Context = context;
        }
               
    }
}