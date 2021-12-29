using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LiveShot.API.Background.ContextOptions;
using LiveShot.API.Properties;

namespace LiveShot.API.Background
{
    public class BackgroundApplication : IBackgroundApplication
    {
        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        private static IntPtr _hHook;
        
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private readonly IEnumerable<IContextOption> _contextOptions;
        private readonly IContextOption _captureScreenShotOption;

        public BackgroundApplication(IEnumerable<IContextOption> contextOptions)
        {
            _contextOptions = contextOptions;
            _captureScreenShotOption = _contextOptions.First(o => o is CaptureScreenShot);
        }

        public void Init()
        {
            CaptureKeyboardKeys();

            var notifyIcon = new NotifyIcon();

            notifyIcon.Icon = new System.Drawing.Icon("bg-icon.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = Resources.NotifyIcon_Title;

            var menuStripOptions = new ContextMenuStrip();

            notifyIcon.Click += _captureScreenShotOption.OnClick;

            foreach (var option in _contextOptions)
            {
                menuStripOptions.Items.Add(option.Title, null, option.OnClick);
            }

            notifyIcon.ContextMenuStrip = menuStripOptions;
        }

        private void CaptureKeyboardKeys()
        {
            _hHook = SetWindowsHookEx(
                WH_KEYBOARD_LL,
                OnKeyBoardMessage,
                Marshal.GetHINSTANCE(typeof(BackgroundApplication).Module),
                0
            );
        }

        private int OnKeyBoardMessage(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int keycode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    Debug.WriteLine(keycode);
                }
            }

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }
    }
}