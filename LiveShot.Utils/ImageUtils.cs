using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using LiveShot.API;

namespace LiveShot.Utils
{
    public static class ImageUtils
    {
        public static void CopyImage(Selection selection, Bitmap source)
        {
            using var target = new Bitmap((int) selection.Width, (int) selection.Height);
            using var graphics = Graphics.FromImage(target);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, target.Width, target.Height),
                new Rectangle((int) selection.Left, (int) selection.Top, target.Width, target.Height),
                GraphicsUnit.Pixel
            );

            var bitmapSource = GetBitmapSource(target);

            Clipboard.SetImage(bitmapSource);
        }

        public static Bitmap CaptureScreen(int width, int height, int left, int top)
        {
            var bmp = new Bitmap(width, height);

            using var graphics = Graphics.FromImage(bmp);

            graphics.CopyFromScreen(left, top, 0, 0, bmp.Size);

            return bmp;
        }

        public static BitmapSource GetBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height)
            );
        }
    }
}