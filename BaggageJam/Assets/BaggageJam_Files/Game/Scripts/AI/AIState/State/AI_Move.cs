namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class AI_Move : AIBaseState
    {
        public override void EnterState(AIStateManager State)
        {
            State.Animator.SetBool("Walk", true);
    
            if (State.targetTransform == null)
            {
                State.SetAIRowTarget(AIManager.Instance.AvailableSlot());
            }
    
            State.AgentSetDestination(State.targetTransform);
        }
    
        public override void UpdateState(AIStateManager State)
        {
            if (State.Agent.remainingDistance < .1f && !State.Agent.pathPending)
            {
                State.SwitchState(State.AI_Idle);
            }
        }
    }
    
}
