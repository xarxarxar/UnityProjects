namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    
    public class AIRowSlot : MonoBehaviour
    {
        public BaggageType AIType;
        public AIStateManager CurrentAI;
    
        public void SetAI(AIStateManager AI)
        {
            CurrentAI = AI;
            AIType = AI.AIType;
        }
    
        public void ClearSlot()
        {
            CurrentAI = null;
            AIType = BaggageType.None;
        }
    }
    
}
