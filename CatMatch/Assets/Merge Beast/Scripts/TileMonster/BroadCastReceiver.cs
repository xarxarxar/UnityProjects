using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tiledom
{
    using BroadcastAction = System.Action<object>;
    using BroadcastAction2 = System.Action;
    using Broadcasts = List<Broadcast>;

    public class Broadcast
    {
        public BroadcastAction action;
        public BroadcastAction2 action2;
        public object target;
    }

    public class BroadCastReceiver
    {        

        public static Dictionary<string, Broadcasts> broadcasts = new Dictionary<string, Broadcasts>();

        public static void Broadcast(string key, object data = null)
        {            
            lock (broadcasts)
            {
                if (broadcasts.ContainsKey(key))
                {
                    Broadcasts _broadcasts = broadcasts[key];
                    Broadcasts removers = new Broadcasts();
                    foreach (Broadcast broadcast in _broadcasts)
                    {
                        if (broadcast.target == null || broadcast.target.Equals(null) || broadcast.target is null)
                        {
                            removers.Add(broadcast);
                            continue;
                        }
                        broadcast.action?.Invoke(data);
                        broadcast.action2?.Invoke();
                    }
                    for (int i = 0; i < removers.Count; i++)
                    {
                        _broadcasts.Remove(removers[i]);
                    }
                }
            }
        }

        public static void Register(string key, BroadcastAction action, object target)
        {
            lock (broadcasts)
            {
                if (!broadcasts.ContainsKey(key))
                {
                    broadcasts.Add(key, new Broadcasts());
                }
                broadcasts[key].Add(new Broadcast()
                {
                    action = action,
                    target = target
                });
            }
        }

        public static void Register(string key, BroadcastAction2 action, object target)
        {
            lock (broadcasts)
            {
                if (!broadcasts.ContainsKey(key))
                {
                    broadcasts.Add(key, new Broadcasts());
                }
                broadcasts[key].Add(new Broadcast()
                {
                    action2 = action,
                    target = target
                });
            }
        }

        public static void UnRegister(string key)
        {
            lock (broadcasts)
            {
                if (broadcasts.ContainsKey(key))
                {
                    broadcasts.Remove(key);
                }
            }
        }

        public static void UnRegister(object target)
        {
            lock (broadcasts)
            {
                List<string> removals = new List<string>();
                foreach (KeyValuePair<string, Broadcasts> entry in broadcasts)
                {
                    Broadcasts _broadcasts = entry.Value;
                    for (int i = 0; i < _broadcasts.Count; i++)
                    {
                        if (_broadcasts[i].target == target)
                        {
                            _broadcasts.RemoveAt(i);
                            i--;
                        }
                    }
                    if (_broadcasts.Count == 0)
                    {
                        removals.Add(entry.Key);
                    }
                }
                foreach (string k in removals)
                {
                    broadcasts.Remove(k);
                }
            }
        }

        private void UnregisterPush(string key, object target)
        {
            lock (broadcasts)
            {
                if (broadcasts.ContainsKey(key))
                {
                    Broadcasts _broadcasts = broadcasts[key];
                    for (int i = 0; i < _broadcasts.Count; i++)
                    {
                        if (_broadcasts[i].target == target)
                        {
                            _broadcasts.RemoveAt(i);
                            i--;
                        }
                    }
                    if (_broadcasts.Count == 0)
                    {
                        broadcasts.Remove(key);
                    }
                }
            }
        }
    }
}
