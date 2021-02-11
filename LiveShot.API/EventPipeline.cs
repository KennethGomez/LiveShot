using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveShot.API.Events;

namespace LiveShot.API
{
    public class EventPipeline : IEventPipeline
    {
        private readonly Dictionary<Type, Collection<Action<Event>>> _actions = new();

        public void Subscribe<T>(Action<Event> action)
        {
            var key = typeof(T);

            if (!_actions.ContainsKey(key)) _actions[key] = new Collection<Action<Event>>();

            _actions[key].Add(action);
        }

        public void Dispatch<T>(object? e) where T : Event, new()
        {
            if (!_actions.TryGetValue(typeof(T), out var actions))
                return;

            foreach (var action in actions) action(new T().With(e));
        }
    }
}