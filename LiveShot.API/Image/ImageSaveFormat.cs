using System.Drawing.Imaging;

namespace LiveShot.API.Image
{
    public struct ImageSaveFormat
    {
        public string Filter { get; init; }
        public ImageFormat Format { get; init; }
    }

    public static class ImageSaveFormats
    {
        public static ImageSaveFormat[] Supported =>
            new[]
            {
                new ImageSaveFormat
                {
                    Filter = "PNG|*.png",
                    Format = ImageFormat.Png
                },
                new()
                {
                    Filter = "JPEG|*.jpeg",
                    Format = ImageFormat.Jpeg
                },
                new()
                {
                    Filter = "BMP|*.bmp",
                    Format = ImageFormat.Bmp
                },
                new()
                {
                    Filter = "GIF|*.gif",
                    Format = ImageFormat.Bmp
                }
            };
    }
}