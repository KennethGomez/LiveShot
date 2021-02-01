using System;
using LiveShot.Objects.Events;

namespace LiveShot.Objects
{
    public interface IEventPipeline
    {
        void Subscribe<T>(Action<Event> action);
        void Dispatch<T>(object e) where T : Event, new();
    }
}