using System.Text;
using System.Windows.Media;

namespace LiveShot.API.Utils
{
    public static class ColorUtils
    {
        public static Brush GetBrushFromChannels(byte r, byte g, byte b, byte a)
        {
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static string ToHex(this Color color)
        {
            StringBuilder sb = new();
            
            sb.AppendFormat("#{0:X2}", color.R);
            sb.AppendFormat("{0:X2}", color.G);
            sb.AppendFormat("{0:X2}", color.B);

            return sb.ToString();
        }
    }
}