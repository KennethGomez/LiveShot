using System.Windows;

namespace LiveShot.API.Utils
{
    public static class PointUtils
    {
        public static (double, double) GetCoords(Point start, Point end)
        {
            double xDiff = start.X - end.X;
            double yDiff = start.Y - end.Y;

            bool growingX = xDiff > 0;
            bool growingY = yDiff > 0;

            double rectLeft = growingX ? end.X : start.X;
            double rectTop = growingY ? end.Y : start.Y;

            return (rectLeft, rectTop);
        }
    }
}