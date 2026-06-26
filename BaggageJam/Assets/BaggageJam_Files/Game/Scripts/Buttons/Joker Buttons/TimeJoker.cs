namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using Dreamteck.Splines;
    using UnityEngine;
    
    public class TimeJoker : JokerButtonBase
    {
    
        void Start()
        {
            Button.onClick.AddListener(TimeStop);
        }
    
        void TimeStop()
        {
            if (JokerDataSO.JokerCount == 0)
            {
                buyJokerPanel.GetComponentInChildren<JokerBuyButton>().InteractableCheckAndTextUpdate();
                buyJokerPanel.SetActive(true);
                return;
            }
    
    
            ConveyorBelt conveyor = ConveyorBelt.Instance;
    
            foreach (GameObject item in conveyor.allRampBaggage)
            {
                item.GetComponent<SplineFollower>().followSpeed = 0;
            }
    
            foreach (GameObject item in conveyor.allConveyor)
            {
                item.GetComponent<SplineFollower>().followSpeed = 0;
            }
    
            DOVirtual.DelayedCall(3, () => TimeContinue());
    
    
            JokerDataSO.JokerCount--;
    
            EventManager.Broadcast(GameEvent.OnJoker);
        }
    
        void TimeContinue()
        {
            ConveyorBelt conveyor = ConveyorBelt.Instance;
    
            foreach (GameObject item in conveyor.allRampBaggage)
            {
                item.GetComponent<SplineFollower>().followSpeed = 1;
            }
    
            foreach (GameObject item in conveyor.allConveyor)
            {
                item.GetComponent<SplineFollower>().followSpeed = 1;
            }
        }
    }
    
}
