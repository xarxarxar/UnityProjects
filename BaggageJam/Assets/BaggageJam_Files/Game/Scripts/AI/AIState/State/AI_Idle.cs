namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class AI_Idle : AIBaseState
    {
        public override void EnterState(AIStateManager State)
        {
            State.Animator.SetBool("Walk", false);
            if (State.targetTransform != null)
            {
                SlotManager.Instance.MergeControl();
            }
        }
    
    
    }
    
}
