using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LiveShot.API.Utils
{
    public static class CursorUtils
    {
        public static Cursor GetCursorFromElement(FrameworkElement element, Point point)
        {
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var rect = new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height);

            RenderTargetBitmap targetBitmap = new(
                (int) element.DesiredSize.Width, (int) element.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32
            );

            element.Arrange(rect);
            targetBitmap.Render(element);

            element.Arrange(rect);

            var cursorStream = GetCursorStream(point, targetBitmap);

            return new Cursor(cursorStream);
        }

        private static MemoryStream GetCursorStream(Point point, BitmapSource targetBitmap)
        {
            var pngStream = new MemoryStream();

            PngBitmapEncoder png = new();

            png.Frames.Add(BitmapFrame.Create(targetBitmap));
            png.Save(pngStream);

            var cursorStream = new MemoryStream();

            cursorStream.Write(new byte[] {0x00, 0x00}, 0, 2);
            cursorStream.Write(new byte[] {0x02, 0x00}, 0, 2);
            cursorStream.Write(new byte[] {0x01, 0x00}, 0, 2);
            cursorStream.Write(new[] {(byte) targetBitmap.Width}, 0, 1);
            cursorStream.Write(new[] {(byte) targetBitmap.Height}, 0, 1);
            cursorStream.Write(new byte[] {0x00}, 0, 1);
            cursorStream.Write(new byte[] {0x00}, 0, 1);
            cursorStream.Write(new byte[] {(byte) (point.X * targetBitmap.Width), 0x00}, 0, 2);
            cursorStream.Write(new byte[] {(byte) (point.Y * targetBitmap.Height), 0x00}, 0, 2);
            cursorStream.Write(new[]
            {
                (byte) (pngStream.Length & 0x000000FF),
                (byte) ((pngStream.Length & 0x0000FF00) >> 0x08),
                (byte) ((pngStream.Length & 0x00FF0000) >> 0x10),
                (byte) ((pngStream.Length & 0xFF000000) >> 0x18)
            }, 0, 4);

            cursorStream.Write(new byte[] {0x16, 0x00, 0x00, 0x00}, 0, 4);

            pngStream.Seek(0, SeekOrigin.Begin);
            pngStream.CopyTo(cursorStream);

            cursorStream.Seek(0, SeekOrigin.Begin);
            return cursorStream;
        }
    }
}