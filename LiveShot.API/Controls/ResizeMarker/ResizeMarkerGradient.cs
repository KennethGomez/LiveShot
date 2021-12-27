using System.Windows;
using System.Windows.Media;

namespace LiveShot.API.Controls.ResizeMarker
{
    public class ResizeMarkerGradient
    {
        public static LinearGradientBrush Striped = new()
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection
            {
                new(Colors.Black, 0.0),
                new(Colors.Black, 0.5),
                new(Colors.White, 0.5),
                new(Colors.White, 1)
            },
            RelativeTransform = new ScaleTransform
            {
                ScaleX = 0.3,
                ScaleY = 0.3
            },
            SpreadMethod = GradientSpreadMethod.Repeat
        };
    }
}