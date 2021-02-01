using System;

using RootWindow = System.Windows.Window;

namespace LiveShot.Objects.Events.Window
{
    public class OnClosed : Event
    {
    }

    public record OnClosedArgs
    {
        public EventArgs? Root { get; init; }

        public RootWindow? Window { get; init; }
    }
}