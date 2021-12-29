using System;
using System.Diagnostics;
using LiveShot.API.Properties;

namespace LiveShot.API.Background.ContextOptions
{
    public class Configuration : IContextOption
    {
        public string Title => Resources.ContextMenu_Configuration_Title;

        public void OnClick(object? sender, EventArgs e)
        {
            Debug.WriteLine("Configuration context menu option clicked");
        }
    }
}