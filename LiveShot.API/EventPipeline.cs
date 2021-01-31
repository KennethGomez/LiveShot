using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveShot.Objects.Events;

namespace LiveShot.Objects
{
    public class EventPipeline
    {
        private static readonly Dictionary<Type, Collection<Action<Event>>> Actions = new();

        public static void Subscribe<T>(Action<Event> action)
        {
            var key = typeof(T);

            if (!Actions.ContainsKey(key))
            {
                Actions[key] = new Collection<Action<Event>>();
            }
            
            Actions[key].Add(action);
        }

        public static void Dispatch<T>(object e) where T : Event, new()
        {
            if (!Actions.TryGetValue(typeof(T), out var actions)) 
                return;

            foreach (var action in actions)
            {
                action(new T().With(e));
            }
        }
    }
}