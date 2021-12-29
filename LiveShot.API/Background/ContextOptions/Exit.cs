using System;
using System.Diagnostics;
using LiveShot.API.Events.Application;
using LiveShot.API.Properties;

namespace LiveShot.API.Background.ContextOptions
{
    public class Exit : IContextOption
    {
        private readonly IEventPipeline _eventPipeline;

        public Exit(IEventPipeline eventPipeline)
        {
            _eventPipeline = eventPipeline;
        }

        public string Title => Resources.ContextMenu_Exit_Title;

        public void OnClick(object? sender, EventArgs e)
        {
            _eventPipeline.Dispatch<ShutdownApplicationEvent>(null);
        }
    }
}