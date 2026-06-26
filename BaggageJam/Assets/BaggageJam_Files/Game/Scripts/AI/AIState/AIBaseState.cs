namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class AIBaseState
    {
        public abstract void EnterState(AIStateManager State);
        public virtual void UpdateState(AIStateManager State) { }
        public virtual void OnTriggerEnter(AIStateManager State, Collider other) { }
        public virtual void OnCollisionEnter(AIStateManager State, Collision other) { }
    }
    
}
