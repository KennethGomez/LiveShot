using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.API.Canvas
{
    public static class OpacityRectangle
    {
        public static Rectangle From(double width, double height) => new()
        {
            Width = width < 0 ? 0 : width,
            Height = height < 0 ? 0 : height,
            Fill = Brushes.Black,
            Opacity = .5
        };
    }
}