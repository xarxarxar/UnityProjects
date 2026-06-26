namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class JokerBuyButton : ButtonBase
    {
        public JokerDataSO JokerDataSO;
        [SerializeField] private TextMeshProUGUI moneyText;
    
        void Start()
        {
            Button.onClick.AddListener(JokerBuy);
        }
    
        void JokerBuy()
        {
            JokerDataSO.JokerCount += 3;
    
            GameManager.Instance.data.totalMoney -= JokerDataSO.PriceList[JokerDataSO.PriceLevel];
    
            JokerDataSO.PriceLevel++;
    
            EventManager.Broadcast(GameEvent.OnJoker);
            
            InteractableCheckAndTextUpdate();
        }
    
    
        public void InteractableCheckAndTextUpdate()
        {
            moneyText.text = JokerDataSO.PriceList[JokerDataSO.PriceLevel].ToString();
            Button.interactable = GameManager.Instance.data.totalMoney >= JokerDataSO.PriceList[JokerDataSO.PriceLevel];
        }
    
    }
    
}
