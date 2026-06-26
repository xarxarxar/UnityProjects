namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;
    
    public class BaggageBackBand : JokerButtonBase
    {
    
        void Start()
        {
            Button.onClick.AddListener(BaggageBack);
        }
    
        void BaggageBack()
        {
            if (JokerDataSO.JokerCount == 0)
            {
                buyJokerPanel.GetComponentInChildren<JokerBuyButton>().InteractableCheckAndTextUpdate();
                buyJokerPanel.SetActive(true);
                return;
            }
    
    
            SlotManager slotManager = SlotManager.Instance;
    
            List<Transform> conveyorList = new List<Transform>();
    
            while (conveyorList.Count < slotManager.allBaggage.Count)
            {
                int randomIndex = Random.Range(0, ConveyorBelt.Instance.allConveyor.Count);
    
                bool isContains = conveyorList.Contains(ConveyorBelt.Instance.allConveyor[randomIndex].transform);
                bool isChildNull = ConveyorBelt.Instance.allConveyor[randomIndex].transform.childCount == 0;
    
                if (!isContains && isChildNull)
                {
                    conveyorList.Add(ConveyorBelt.Instance.allConveyor[randomIndex].transform);
                }
            }
    
            for (int i = 0; i < slotManager.allBaggage.Count; i++)
            {
                Baggage baggage = slotManager.allBaggage[i];
    
                baggage.CurrentSlot.ClearSlot();
    
                baggage.transform.parent = conveyorList[i];
    
                baggage.transform.DOLocalRotate(new Vector3(90f, 0f, 90f), .5f).SetDelay(i * .1f);
    
                baggage.transform.DOLocalMove(Vector3.zero, .5f).SetDelay(i * .1f).
                OnComplete(() =>
                {
                    baggage.isHaveSlot = false;
                    baggage.isSelect = false;
    
                    baggage.GetComponent<Collider>().enabled = true;
                    baggage.GetComponent<Collider>().isTrigger = false;
    
                    baggage.GetComponent<Rigidbody>().isKinematic = false;
                });
    
            }
    
            slotManager.allBaggage.Clear();
    
            JokerDataSO.JokerCount--;
            EventManager.Broadcast(GameEvent.OnJoker);
        }
    }
    
}
