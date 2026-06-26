namespace EKStudio
{
    using System;
    using System.Collections.Generic;
    
    public enum GameEvent
    {
        OnStart,
        OnSetSlotBaggage,
        OnBaggageReceiving,
        OnJoker,
        OnWin,
        OnLose,
    
        //SOUND EVENTS
        OnPlaySound,
        OnPlaySoundVolume
    }
    public static class EventManager
    {
        private static Dictionary<GameEvent, Action> eventTable =
            new Dictionary<GameEvent, Action>();
    
        public static void AddHandler(GameEvent gameEvent, Action action)
        {
            if (!eventTable.ContainsKey(gameEvent))
                eventTable[gameEvent] = action;
            else eventTable[gameEvent] += action;
        }
    
        public static void RemoveHandler(GameEvent gameEvent, Action action)
        {
            if (eventTable[gameEvent] != null)
                eventTable[gameEvent] -= action;
    
            if (eventTable[gameEvent] == null)
                eventTable.Remove(gameEvent);
        }
    
        public static void Broadcast(GameEvent gameEvent)
        {
            if (eventTable[gameEvent] != null)
                eventTable[gameEvent]();
        }
    
        private static Dictionary<GameEvent, Action<object>> eventTableFloat
            = new Dictionary<GameEvent, Action<object>>();
    
        public static void AddHandler(GameEvent gameEvent, Action<object> action)
        {
            if (!eventTableFloat.ContainsKey(gameEvent)) eventTableFloat[gameEvent]
                     = action;
            else eventTableFloat[gameEvent] += action;
        }
    
        public static void RemoveHandler(GameEvent gameEvent, Action<object> action)
        {
            if (eventTableFloat[gameEvent] != null)
                eventTableFloat[gameEvent] -= action;
    
            if (eventTableFloat[gameEvent] == null)
                eventTableFloat.Remove(gameEvent);
        }
    
        private static Dictionary<GameEvent, Action<object, object>> eventTableDouble
            = new Dictionary<GameEvent, Action<object, object>>();
    
        public static void AddHandler(GameEvent gameEvent, Action<object, object> action)
        {
            if (!eventTableDouble.ContainsKey(gameEvent)) eventTableDouble[gameEvent]
                     = action;
            else eventTableDouble[gameEvent] += action;
        }
    
        public static void RemoveHandler(GameEvent gameEvent, Action<object, object> action)
        {
            if (eventTableDouble[gameEvent] != null)
                eventTableDouble[gameEvent] -= action;
    
            if (eventTableDouble[gameEvent] == null)
                eventTableDouble.Remove(gameEvent);
        }
    
        public static void Broadcast(GameEvent gameEvent, object value1, object value2)
        {
            if (eventTableDouble[gameEvent] != null)
                eventTableDouble[gameEvent](value1, value2);
        }
    
        public static void Broadcast(GameEvent gameEvent, object value)
        {
            if (eventTableFloat[gameEvent] != null)
                eventTableFloat[gameEvent](value);
        }
    
        private static Dictionary<GameEvent, Action<object, object, object>> eventTableTriple
            = new Dictionary<GameEvent, Action<object, object, object>>();
    
        public static void AddHandler(GameEvent gameEvent, Action<object, object, object> action)
        {
            if (!eventTableTriple.ContainsKey(gameEvent)) eventTableTriple[gameEvent]
                     = action;
            else eventTableTriple[gameEvent] += action;
        }
    
        public static void RemoveHandler(GameEvent gameEvent, Action<object, object, object> action)
        {
            if (eventTableTriple[gameEvent] != null)
                eventTableTriple[gameEvent] -= action;
    
            if (eventTableTriple[gameEvent] == null)
                eventTableTriple.Remove(gameEvent);
        }
    
        public static void Broadcast(GameEvent gameEvent, object value1, object value2, object value3)
        {
            if (eventTableTriple[gameEvent] != null)
                eventTableTriple[gameEvent](value1, value2, value3);
        }
    
    
    
        private static Dictionary<GameEvent, Action<object, object, object, object>> eventTableQuadra
            = new Dictionary<GameEvent, Action<object, object, object, object>>();
    
        public static void AddHandler(GameEvent gameEvent, Action<object, object, object, object> action)
        {
            if (!eventTableQuadra.ContainsKey(gameEvent)) eventTableQuadra[gameEvent]
                     = action;
            else eventTableQuadra[gameEvent] += action;
        }
    
        public static void RemoveHandler(GameEvent gameEvent, Action<object, object, object, object> action)
        {
            if (eventTableQuadra[gameEvent] != null)
                eventTableQuadra[gameEvent] -= action;
    
            if (eventTableQuadra[gameEvent] == null)
                eventTableQuadra.Remove(gameEvent);
        }
    
        public static void Broadcast(GameEvent gameEvent, object value1, object value2, object value3, object value4)
        {
            if (eventTableQuadra[gameEvent] != null)
                eventTableQuadra[gameEvent](value1, value2, value3, value4);
        }
    }
    
}
