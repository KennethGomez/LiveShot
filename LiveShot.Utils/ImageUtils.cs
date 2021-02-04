using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using LiveShot.API;
using LiveShot.API.Canvas;

namespace LiveShot.Utils
{
    public static class ImageUtils
    {
        public static bool CopyImage(Selection selection, Bitmap source)
        {
            if (selection.HasInvalidSize) return false;
            
            var bitmap = GetBitmap(selection, source);
            var bitmapSource = GetBitmapSource(bitmap);

            Clipboard.SetImage(bitmapSource);

            return true;
        }

        public static Bitmap GetBitmap(Selection selection, Bitmap source)
        {
            var bitmap = new Bitmap((int) selection.Width, (int) selection.Height);

            using var graphics = Graphics.FromImage(bitmap);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle((int) selection.Left, (int) selection.Top, bitmap.Width, bitmap.Height),
                GraphicsUnit.Pixel
            );

            return bitmap;
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