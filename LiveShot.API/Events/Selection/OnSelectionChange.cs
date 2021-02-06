namespace LiveShot.API.Events.Selection
{
    public class OnSelectionChange : Event
    {
    }

    public struct OnSelectionChangeArgs
    {
        public double NewLeft { get; init; }
        public double NewTop { get; init; }
        public double NewWidth { get; init; }
        public double NewHeight { get; init; }

        public static OnSelectionChangeArgs From(Canvas.Selection selection)
        {
            return new()
            {
                NewLeft = selection.Left,
                NewTop = selection.Top,
                NewWidth = selection.Width,
                NewHeight = selection.Height
            };
        }
    }
}