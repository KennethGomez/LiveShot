using System;

namespace LiveShot.Objects.Events
{
    public class Event
    {
        private object? _args;

        public Event With(object args)
        {
            _args = args;
            
            return this;
        }

        public T GetArgs<T>() => (T) (_args ?? throw new InvalidOperationException());
    }
}