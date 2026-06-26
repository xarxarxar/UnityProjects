namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class AI_BaggageReceiving : AIBaseState
    {
        public override void EnterState(AIStateManager State)
        {
            State.Animator.SetTrigger("Carry");
    
            GameObject[] exitPoints = GameObject.FindGameObjectsWithTag(TagHolder.AIExitPoint_Tag);
    
            float dist1 = Vector3.Distance(State.transform.position, exitPoints[0].transform.position);
            float dist2 = Vector3.Distance(State.transform.position, exitPoints[1].transform.position);
    
            Transform targetTransform = null;
            if (dist1 < dist2)
            {
                targetTransform = exitPoints[0].transform;
            }
            else
            {
                targetTransform = exitPoints[1].transform;
            }
    
            State.targetTransform = targetTransform;
            State.AgentSetDestination(State.targetTransform);
        }
    
        public override void UpdateState(AIStateManager State)
        {
    
        }
    
    }
    
}
