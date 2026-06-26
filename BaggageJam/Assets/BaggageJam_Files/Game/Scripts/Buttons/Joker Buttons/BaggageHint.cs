namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class BaggageHint : JokerButtonBase
    {
    
        void Start()
        {
            Button.onClick.AddListener(BaggageHintButton);
        }
    
        void BaggageHintButton()
        {
            if (JokerDataSO.JokerCount == 0)
            {
                buyJokerPanel.GetComponentInChildren<JokerBuyButton>().InteractableCheckAndTextUpdate();
                buyJokerPanel.SetActive(true);
                return;
            }
    
    
            List<Baggage> allBaggage = new List<Baggage>();
    
            foreach (var item in ConveyorBelt.Instance.allConveyor)
            {
                if (item.transform.childCount > 0)
                {
                    allBaggage.Add(item.transform.GetChild(0).GetComponent<Baggage>());
                }
            }
    
            foreach (var item in SlotManager.Instance.allBaggage)
            {
                allBaggage.Add(item);
            }
    
            AIManager AIManager = AIManager.Instance;
            BaggageType AIType = BaggageType.None;
    
            for (int i = 0; i < AIManager.allRow.Count; i++)
            {
                if (AIManager.allRow[i].AIRowSlots[0].AIType != BaggageType.None)
                {
                    AIType = AIManager.allRow[i].AIRowSlots[0].AIType;
                }
            }
    
            List<Baggage> mergeBaggages = new List<Baggage>();
            int mergeBaggageCount = 0;
    
            foreach (var item in allBaggage)
            {
                if (item.baggageType == AIType)
                {
                    mergeBaggageCount++;
                    mergeBaggages.Add(item);
                }
            }
    
            if (AIType == BaggageType.None)
            {
                return;
            }
    
            if (mergeBaggageCount > 2)
            {
                SlotManager.Instance.Merge(mergeBaggages);
            }
            else
            {
                for (int i = 0; i < 3 - mergeBaggageCount; i++)
                {
                    Vector3 pos = Camera.main.transform.position + new Vector3(0, -8f, 5 + (i * 2.5f));
    
                    BaggageFactory.SpawnBaggage(AIType, pos, Quaternion.identity, out GameObject factoryProduct);
                    mergeBaggages.Add(factoryProduct.GetComponent<Baggage>());
                }
    
                SlotManager.Instance.Merge(mergeBaggages);
            }
    
            JokerDataSO.JokerCount--;
            EventManager.Broadcast(GameEvent.OnJoker);
        }
    }
    
}
