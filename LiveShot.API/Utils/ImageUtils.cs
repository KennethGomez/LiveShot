using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveShot.API.Canvas;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace LiveShot.API.Utils
{
    public static class ImageUtils
    {
        public static bool CopyImage(Selection selection, Bitmap source, Bitmap canvasBitmap)
        {
            if (selection.Invalid) return false;

            var bitmap = GetBitmap(selection, source, canvasBitmap);
            var bitmapSource = GetBitmapSource(bitmap);

            Clipboard.SetImage(bitmapSource);

            return true;
        }

        public static Bitmap GetBitmap(Selection selection, Bitmap source, Bitmap canvasBitmap)
        {
            double realWidth = selection.Width;
            double realHeight = selection.Height;

            if (selection.Left + realWidth > source.Width)
            {
                realWidth = source.Width - selection.Left;
            }

            if (selection.Top + realHeight > source.Height)
            {
                realHeight = source.Height - selection.Top;
            }

            var bitmap = new Bitmap((int)realWidth, (int)realHeight);

            using var graphics = Graphics.FromImage(bitmap);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle((int)selection.Left, (int)selection.Top, bitmap.Width, bitmap.Height),
                GraphicsUnit.Pixel
            );

            graphics.DrawImage(
                canvasBitmap,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle((int)selection.Left, (int)selection.Top, bitmap.Width, bitmap.Height),
                GraphicsUnit.Pixel
            );

            return bitmap;
        }

        public static Bitmap GetBitmapFromCanvas(System.Windows.Controls.Canvas canvas)
        {
            var bitmapSource = new RenderTargetBitmap(
                (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32
            );
            bitmapSource.Render(canvas);

            return GetBitmapFromSource(bitmapSource);
        }

        public static Bitmap CaptureScreen(int width, int height, int left, int top)
        {
            (float fX, float fY) = ScreenUtils.GetScalingFactor();

            var bmp = new Bitmap((int)(width * fX), (int)(height * fY));

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
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
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

        public static System.Windows.Controls.Image GetMagnifiedImage(
            Bitmap source, Point point, int size, int scale
        )
        {
            var bitmap = GetMagnifiedBitmap(point, size, source);

            var image = new System.Windows.Controls.Image
            {
                Width = size * scale,
                Height = size * scale,
                Source = bitmap,
                SnapsToDevicePixels = true
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            return image;
        }

        public static BitmapSource GetMagnifiedBitmap(Point point, int size, Bitmap source)
        {
            var bitmap = new Bitmap(size, size);

            using var graphics = Graphics.FromImage(bitmap);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, size, size),
                new Rectangle((int)(point.X - size / 2), (int)(point.Y - size / 2), size, size),
                GraphicsUnit.Pixel
            );

            return GetBitmapSource(bitmap);
        }

        public static byte[] GetBytes(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat
            );

            int length = bitmapData.Stride * bitmapData.Height;

            byte[] bytes = new byte[length];

            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);

            bitmap.UnlockBits(bitmapData);

            return bytes;
        }
    }
}