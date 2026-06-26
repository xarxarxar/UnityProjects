namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GameManager : InstanceManager<GameManager>
    {
        public GameData data;
    
        void Awake()
        {
    #if !UNITY_EDITOR
        AllSaveManager.Load(data);
    #endif
    
        }
    
        void Start()
        {
            InvokeRepeating(nameof(SaveData), 1, .1f);
    
            EventManager.Broadcast(GameEvent.OnJoker);
    
            Time.timeScale = 20;
            Invoke(nameof(TimeScaleNormal), 20);
        }
    
        void Update()
        {
    
        }
        private void SaveData()
        {
            AllSaveManager.Save(data);
        }
    
        private void TimeScaleNormal()
        {
            Time.timeScale = 1;
        }
    }
    
}
