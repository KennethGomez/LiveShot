namespace LiveShot.Objects.Events
{
    public class Event
    {
        private readonly object _args;

        public Event(object args)
        {
            _args = args;
        }

        public T GetArgs<T>() => (T) _args;
    }
}