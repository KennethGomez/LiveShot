using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveShot.API.Canvas;
using PixelFormat = System.Windows.Media.PixelFormat;
using Point = System.Drawing.Point;

namespace LiveShot.Utils
{
    public static class ImageUtils
    {
        public static bool CopyImage(Selection selection, Bitmap source, Canvas canvas)
        {
            if (selection.HasInvalidSize) return false;

            var bitmap = GetBitmap(selection, source, canvas);
            var bitmapSource = GetBitmapSource(bitmap);

            Clipboard.SetImage(bitmapSource);

            return true;
        }

        public static Bitmap GetBitmap(Selection selection, Bitmap source, Canvas canvas)
        {
            var bitmap = new Bitmap((int) selection.Width, (int) selection.Height);

            var canvasBitmap = new RenderTargetBitmap(
                (int) canvas.ActualWidth, (int) canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32
            );
            canvasBitmap.Render(canvas);

            using var graphics = Graphics.FromImage(bitmap);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle((int) selection.Left, (int) selection.Top, bitmap.Width, bitmap.Height),
                GraphicsUnit.Pixel
            );
            
            graphics.DrawImage(
                GetBitmapFromSource(canvasBitmap),
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

        public static Bitmap GetBitmapFromSource(BitmapSource source)
        {
            var bitmap = new Bitmap(source.PixelWidth, source.PixelHeight);

            var data = bitmap.LockBits(
                new Rectangle(Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat
            );

            source.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,
                data.Height * data.Stride,
                data.Stride
            );
            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}