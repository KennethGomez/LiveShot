using System;

namespace LiveShot.API.Events
{
    public class Event
    {
        private object? _args;

        public Event With(object? args)
        {
            _args = args;

            return this;
        }

        public T GetArgs<T>()
        {
            return (T) (_args ?? throw new InvalidOperationException());
        }
    }
}