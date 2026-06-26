namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public abstract class JokerButtonBase : ButtonBase
    {
        public JokerDataSO JokerDataSO;
        public GameObject buyJokerPanel;
        public GameObject Corner_Count_Add;
        public TextMeshProUGUI Corner_Count;
    
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnJoker, OnJoker);
        }
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnJoker, OnJoker);
        }
    
        void OnJoker()
        {
            Corner_Count.text = JokerDataSO.JokerCount.ToString();
    
            Corner_Count.gameObject.SetActive(JokerDataSO.JokerCount > 0);
    
            Corner_Count_Add.SetActive(JokerDataSO.JokerCount == 0);
        }
    }
    
}
