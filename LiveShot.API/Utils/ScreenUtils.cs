using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LiveShot.API.Utils
{
    public static class ScreenUtils
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        private enum DeviceCap
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LogPixelsX = 88,

            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LogPixelsY = 90
        }

        public static (float X, float Y) GetScalingFactor()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);            
            var desktop = g.GetHdc();

            int dpiX = GetDeviceCaps(desktop, (int)DeviceCap.LogPixelsX);
            int dpiY = GetDeviceCaps(desktop, (int)DeviceCap.LogPixelsY);

            return (dpiX / 96f, dpiY / 96f);
        }
    }
}