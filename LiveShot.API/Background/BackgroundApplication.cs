using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LiveShot.API.Background.ContextOptions;
using LiveShot.API.Events.Capture;
using LiveShot.API.Properties;
using Microsoft.Extensions.Configuration;

namespace LiveShot.API.Background
{
    public class BackgroundApplication : IBackgroundApplication
    {
        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        private static IntPtr _hHook;
        private static HookProc _handler = null!;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private readonly IEnumerable<IContextOption> _contextOptions;
        private readonly IConfiguration _configuration;
        private readonly IEventPipeline _eventPipeline;

        private readonly IContextOption _captureScreenShotOption;

        public BackgroundApplication(
            IEnumerable<IContextOption> contextOptions,
            IConfiguration configuration,
            IEventPipeline eventPipeline
        )
        {
            _contextOptions = contextOptions;
            _configuration = configuration;
            _eventPipeline = eventPipeline;

            _captureScreenShotOption = _contextOptions.First(o => o is CaptureScreenShot);
            _handler = OnKeyBoardMessage;
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
                _handler,
                Marshal.GetHINSTANCE(GetType().Module),
                0
            );
        }

        private int OnKeyBoardMessage(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int keyCode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    int generalKeyCode = int.Parse(_configuration.GetSection("Shortcuts")?["General"] ?? "0");

                    if (generalKeyCode == keyCode)
                    {
                        _eventPipeline.Dispatch<CaptureScreenShotEvent>(null);

                        return 1;
                    }
                }
            }

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }
    }
}