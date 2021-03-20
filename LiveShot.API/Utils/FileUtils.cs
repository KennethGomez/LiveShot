using System;
using System.Drawing;
using System.IO;
using System.Linq;
using LiveShot.API.Canvas;
using LiveShot.API.Image;
using LiveShot.API.Properties;
using Microsoft.Win32;

namespace LiveShot.API.Utils
{
    public static class FileUtils
    {
        public static bool SaveImage(Selection selection, Bitmap source, Bitmap canvasBitmap)
        {
            var formats = ImageSaveFormats.Supported;

            SaveFileDialog dialog = new()
            {
                Filter = string.Join('|', formats.Select(f => f.Filter)),
                FileName = string.Format(Resources.CaptureScreen_SaveImage_FileName, DateTime.Now),
                RestoreDirectory = true,
                Title = Resources.CaptureScreen_SaveImage_Title
            };

            if (dialog.ShowDialog() == false) return false;

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return false;

            try
            {
                var fs = (FileStream) dialog.OpenFile();

                var selectedFormat = formats[dialog.FilterIndex - 1];

                ImageUtils.GetBitmap(selection, source, canvasBitmap).Save(fs, selectedFormat.Format);

                fs.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}