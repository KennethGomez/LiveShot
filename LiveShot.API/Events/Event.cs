namespace LiveShot.Objects.Events
{
    public class Event
    {
        private readonly object _args;

        protected Event(object args)
        {
            _args = args;
        }

        public object Args => _args;
    }
}