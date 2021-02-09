using System.Windows.Media;

namespace LiveShot.Utils
{
    public static class ColorUtils
    {
        public static Brush GetBrushFromChannels(byte r, byte g, byte b, byte a)
        {
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
    }
}