namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;
    
    public class AIRowControl : MonoBehaviour
    {
        public List<AIRowSlot> AIRowSlots;
        public GameObject lockArea;
    
        public bool CanBaggageMerge(BaggageType baggageType)
        {
            if (baggageType == AIRowSlots[0].AIType) return true;
            return false;
        }
    
    
        public void UpdateRow()
        {
            if (AIRowSlots[0].AIType != BaggageType.None)
                return;
    
            List<AIStateManager> AllAI = new List<AIStateManager>();
    
            foreach (var item in AIRowSlots)
            {
                if (item.CurrentAI != null)
                {
                    AllAI.Add(item.CurrentAI);
                }
            }
    
            for (int i = 0; i < AIRowSlots.Count; i++)
            {
                if (i < AllAI.Count)
                {
                    AIRowSlots[i].SetAI(AllAI[i]);
    
                    AllAI[i].targetTransform = AIRowSlots[i].transform;
                    AllAI[i].SwitchState(AllAI[i].AI_Move);
                }
                else
                {
                    AIRowSlots[i].ClearSlot();
                }
            }
        }
    }
    
}
