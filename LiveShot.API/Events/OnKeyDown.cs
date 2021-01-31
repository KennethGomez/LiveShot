using System.Windows.Input;

namespace LiveShot.Objects.Events
{
    public class OnKeyDown : Event
    {
        public OnKeyDown(KeyEventArgs args) : base(args)
        {
        }
    }
}