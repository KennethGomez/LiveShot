using System;
using LiveShot.API.Events;

namespace LiveShot.API
{
    public interface IEventPipeline
    {
        void Subscribe<T>(Action<Event> action);
        void Dispatch<T>(object? e) where T : Event, new();
    }
}