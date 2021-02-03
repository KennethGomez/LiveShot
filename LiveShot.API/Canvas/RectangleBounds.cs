using System.Windows.Shapes;

namespace LiveShot.API.Canvas
{
    public static class RectangleBounds
    {
        public static RectangleBound[] GetBounds(Selection selection, double screenWidth, double screenHeight)
        {
            (double left, double top, double width, double height) = selection.Transform;

            return new[]
            {
                new RectangleBound
                {
                    Rectangle = OpacityRectangle.From(left, top + height),
                    Left = 0,
                    Top = 0
                },
                new RectangleBound
                {
                    Rectangle = OpacityRectangle.From(left + width, screenHeight - (top + height)),
                    Left = 0,
                    Top = top + height
                },
                new RectangleBound
                {
                    Rectangle = OpacityRectangle.From(screenWidth - (left + width), screenHeight - top),
                    Left = left + width,
                    Top = top
                },
                new RectangleBound
                {
                    Rectangle = OpacityRectangle.From(screenWidth - left, top),
                    Left = left,
                    Top = 0
                }
            };
        }
    }

    public struct RectangleBound
    {
        public Rectangle Rectangle { get; init; }
        public double Left { get; init; }
        public double Top { get; init; }

        public void Deconstruct(out Rectangle rectangle, out double left, out double top)
        {
            rectangle = Rectangle;
            left = Left < 0 ? 0 : Left;
            top = Top < 0 ? 0 : Top;
        }
    }
}