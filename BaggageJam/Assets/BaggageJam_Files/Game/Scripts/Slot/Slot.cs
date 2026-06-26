namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Slot : MonoBehaviour
    {
        public bool isLock;
        public float lockOpenLevel;
        public Baggage Baggage;
        public BaggageType baggageType;
        public Transform point;
        public GameObject lockArea;
    
        void Start()
        {
            if (GameManager.Instance.data.levelCount >= lockOpenLevel)
            {
                isLock = false;
    
                if (lockArea != null)
                {
                    lockArea.SetActive(false);
                }
            }
        }
    
        public void SetBaggage(Baggage baggage)
        {
            Baggage = baggage;
            baggageType = Baggage.baggageType;
        }
    
        public void ClearSlot()
        {
            Baggage.CurrentSlot = null;
            Baggage = null;
            baggageType = BaggageType.None;
        }
    }
    
}
