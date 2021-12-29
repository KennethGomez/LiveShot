using System;
using System.Windows.Forms;
using LiveShot.API.Events.Capture;
using LiveShot.API.Properties;

namespace LiveShot.API.Background.ContextOptions
{
    public class CaptureScreenShot : IContextOption
    {
        private readonly IEventPipeline _eventPipeline;

        public CaptureScreenShot(IEventPipeline eventPipeline)
        {
            _eventPipeline = eventPipeline;
        }

        public string Title => Resources.ContextMenu_CaptureScreenShot_Title;

        public void OnClick(object? sender, EventArgs e)
        {
            // If is MouseEventArgs it's being clicked the icon
            if (e is MouseEventArgs mouseEvent)
            {
                if ((mouseEvent.Button & MouseButtons.Left) != 0)
                {
                    _eventPipeline.Dispatch<CaptureScreenShotEvent>(null);
                }
            }
            else
            {
                _eventPipeline.Dispatch<CaptureScreenShotEvent>(null);
            }
        }
    }
}