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
            if (!Actions.TryGetValue(typeof(T), out var actions))
            {
                actions = new Collection<Action<Event>>();
                
                Actions.Add(typeof(T), actions);
            }
            
            actions.Add(action);
        }

        public static void Dispatch(Event e)
        {
            if (!Actions.TryGetValue(e.GetType(), out var actions)) 
                return;

            foreach (var action in actions)
            {
                action(e);
            }
        }
    }
}