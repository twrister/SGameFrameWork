using System.Collections;
using System.Collections.Generic;
using System;

namespace SthGame
{
    public class GlobalEventSystem : ClientSystem
    {
        public static GlobalEventSystem Instance { get; private set; }

        private Dictionary<EventId, List<Action<object[]>>> eventHandlers = new Dictionary<EventId, List<Action<object[]>>>();

        public override void Init()
        {
            Instance = this;
        }

        public override void ShutDown()
        {
            eventHandlers.Clear();
        }

        public void Bind(EventId eventId, Action<object[]> func)
        {
            if (!eventHandlers.ContainsKey(eventId))
            {
                eventHandlers.Add(eventId, new List<Action<object[]>>());
            }
            eventHandlers[eventId].Add(func);
        }

        public void UnBind(EventId eventId, Action<object[]> func)
        {
            if (eventHandlers.ContainsKey(eventId))
            {
                if (eventHandlers[eventId].Contains(func))
                {
                    eventHandlers[eventId].Remove(func);
                    if (eventHandlers[eventId].Count == 0)
                    {
                        eventHandlers[eventId].Clear();
                        eventHandlers.Remove(eventId);
                    }
                }
            }
        }

        public void UnBindAll(EventId eventId)
        {
            if (eventHandlers.ContainsKey(eventId))
            {
                eventHandlers[eventId].Clear();
                eventHandlers.Remove(eventId);
            }
        }

        public void Fire(EventId eventId, params object[] inParams)
        {
            if (eventHandlers.ContainsKey(eventId))
            {
                for (int i = 0; i < eventHandlers[eventId].Count; i++)
                {
                    eventHandlers[eventId][i](inParams);
                }
            }
        }
    }
}
