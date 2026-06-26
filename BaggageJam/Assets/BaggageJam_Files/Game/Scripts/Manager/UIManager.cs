namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class UIManager : InstanceManager<UIManager>
    {
        [SerializeField] private GameData data;
    
        [Header("GameObject")]
        public GameObject MoneyIcon;
    
    
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI totalMoney;
        [SerializeField] private TextMeshProUGUI levelCount;
    
    
    
        [Header("Panel")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
    
        void Start()
        {
            InvokeRepeating(nameof(UpdateText), 1f, 1f);
        }
    
        void Update()
        {
    
        }
    
        public void UpdateText()
        {
            totalMoney.text = data.totalMoney.ToString("0");
            levelCount.text = "Lvl." + data.levelCount.ToString("0");
        }
    
    
        public void MoneyButton()
        {
    
            GameManager.Instance.data.totalMoney += 10000;
            EventManager.Broadcast(GameEvent.OnJoker);
        }
        //#################################################EVENTS###########################################
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnWin, OnWin);
            EventManager.AddHandler(GameEvent.OnLose, OnLose);
        }
    
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnWin, OnWin);
            EventManager.RemoveHandler(GameEvent.OnLose, OnLose);
        }
    
        void OnWin()
        {
            winPanel.SetActive(true);
            EventManager.Broadcast(GameEvent.OnPlaySound, "Win");
        }
    
        void OnLose()
        {
            losePanel.SetActive(true);
            EventManager.Broadcast(GameEvent.OnPlaySound, "Lose");
        }
    }
    
}
