using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LiveShot.API.Background.ContextOptions;
using LiveShot.API.Properties;

namespace LiveShot.API.Background
{
    public class BackgroundApplication : IBackgroundApplication
    {
        private readonly IEnumerable<IContextOption> _contextOptions;
        private readonly IContextOption _captureScreenShotOption;

        public BackgroundApplication(IEnumerable<IContextOption> contextOptions)
        {
            _contextOptions = contextOptions;
            _captureScreenShotOption = _contextOptions.First(o => o is CaptureScreenShot);
        }

        public void Init()
        {
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
    }
}