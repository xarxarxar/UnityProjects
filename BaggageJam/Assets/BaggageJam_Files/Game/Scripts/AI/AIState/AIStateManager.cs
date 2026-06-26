namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.AI;
    
    public class AIStateManager : MonoBehaviour
    {
        public AIBaseState currentState;
    
        public AI_Idle AI_Idle = new AI_Idle();
        public AI_Move AI_Move = new AI_Move();
        public AI_BaggageReceiving AI_BaggageReceiving = new AI_BaggageReceiving();
    
        public Animator Animator => GetComponent<Animator>();
        public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    
        public BaggageType AIType;
        public bool isHaveBaggage;
        public Transform baggageSlot;
        public Transform targetTransform;
    
        void Start()
        {
            currentState = AI_Move;
    
            currentState.EnterState(this);
        }
    
        void Update()
        {
            currentState.UpdateState(this);
            SetSpeed();
        }
    
        public void SetSpeed()
        {
            Animator.speed = Agent.speed;
        }
    
        public void SwitchState(AIBaseState state)
        {
            currentState = state;
            currentState.EnterState(this);
        }
    
        public void SetAIRowTarget(AIRowSlot Slot)
        {
            targetTransform = Slot.transform;
            Slot.SetAI(this);
        }
    
        public void AgentSetDestination(Transform target)
        {
            Agent.SetDestination(new Vector3(target.position.x, transform.position.y, target.position.z));
        }
    
        public void SetBaggage()
        {
            isHaveBaggage = true;
            targetTransform.GetComponent<AIRowSlot>().ClearSlot();
    
            EventManager.Broadcast(GameEvent.OnBaggageReceiving);
        }
    
    
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnBaggageReceiving, OnBaggageReceiving);
        }
    
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnBaggageReceiving, OnBaggageReceiving);
        }
    
        void OnBaggageReceiving()
        {
            if (isHaveBaggage)
            {
                AIManager.Instance.UpdateRow();
                SwitchState(AI_BaggageReceiving);
    
                bool isLevelComp = LevelManager.Instance.stickmanList.Count == 0;
                bool isWin = AIManager.Instance.IsAllSlotNull();
    
                // print("isWin: " + isWin);
                // print("IsAllSlotNull: " + AIManager.Instance.IsAllSlotNull());
                // print("allBaggageCount: " + SlotManager.Instance.allBaggage.Count);
                // print("isLevelComp: " + isLevelComp);
    
                if (isLevelComp && isWin)
                {
                    print("win");
                    DOVirtual.DelayedCall(1f, () => EventManager.Broadcast(GameEvent.OnWin));

                }
            }
        }
    }
    
}
