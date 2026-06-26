namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using Dreamteck.Splines;
    using UnityEngine;
    
    public class Baggage : MonoBehaviour, ISelectable
    {
        public SplineFollower SplineFollower => GetComponent<SplineFollower>();
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public Collider Collider => GetComponent<Collider>();
    
        [HideInInspector] public Slot CurrentSlot;
    
        public BaggageType baggageType;
    
        [SerializeField] private bool isMoveBand;
        public bool isSelect;
        public bool isHaveSlot;
    
    
        private Vector3 unSelectedPos;
    
    
        void Update()
        {
            FollowConveyor();
        }
    
        void FollowConveyor()
        {
            if (isMoveBand && !isHaveSlot)
            {
    
                float dist = Vector3.Distance(transform.position, transform.parent.position);
                // if (dist > 1f)
                // {
                //     isMoveBand = false;
                // }
    
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, transform.localPosition.y, 0), Time.deltaTime * dist * 2f);
            }
        }
    
        public void ISelect()
        {
            if (isHaveSlot)
                return;
    
            // print("ISelect");
            isSelect = true;
            isMoveBand = false;
    
            transform.parent = null;
    
            unSelectedPos = transform.position;
            transform.position = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized + transform.position;
    
            transform.DOScale(Vector3.one * 1.5f, .2f).SetEase(Ease.Linear);
    
            transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 360, transform.localEulerAngles.z), 5f, RotateMode.FastBeyond360)
             .SetDelay(.1f)
             .SetEase(Ease.Linear)
             .SetLoops(-1, LoopType.Restart);
    
    
            RaySelectionManager.Instance.currentBaggage = transform;
            Rigidbody.isKinematic = true;
            Collider.isTrigger = true;
            EventManager.Broadcast(GameEvent.OnPlaySound, "Select");
        }

        public void ISelected()
        {
            // print("ISelected");
            isHaveSlot = true;
            transform.parent = null;
    
            Rigidbody.isKinematic = true;
            Collider.enabled = false;
    
            DOTween.Kill(transform);
    
            transform.DORotate(new Vector3(0, 90, 0), .2f).SetEase(Ease.Linear);
    
            EventManager.Broadcast(GameEvent.OnSetSlotBaggage, this);
        }
    
        public void INotSelected()
        {
            if (isHaveSlot)
                return;
    
            // print("INotSelected");
    
            DOTween.Kill(transform);
    
            transform.DOScale(Vector3.one, .2f).SetEase(Ease.Linear);
    
            transform.DOMove(unSelectedPos, .2f).SetEase(Ease.Linear).
            OnComplete(() =>
            {
            });
            Rigidbody.isKinematic = false;
            Collider.isTrigger = false;
            isMoveBand = false;
            isSelect = false;
        }
    
    
        public void SetSlot(Slot slot)
        {
            if (CurrentSlot != null)
            {
                CurrentSlot.ClearSlot();
            }
    
            float duration = .2f;
    
            transform.DOMove(slot.point.transform.position, duration).SetEase(Ease.Flash);
    
            transform.DOScale(Vector3.one * .9f, duration);
    
            slot.SetBaggage(this);
            CurrentSlot = slot;
        }
    
        public void Merge(Vector3 Pos, bool isMergeBaggage)
        {
            Collider.isTrigger = true;
            Rigidbody.isKinematic = true;
    
            isSelect = true;
            isHaveSlot = true;
            isMoveBand = false;
    
            transform.parent = null;
    
            if (CurrentSlot != null)
            {
                CurrentSlot.ClearSlot();
            }
    
            float duration = .2f;
    
            AIStateManager targetAI = AIManager.Instance.AvailableAI(baggageType);
    
            if (!isMergeBaggage)
            {
                targetAI.SetBaggage();
            }
    
            transform.DORotate(Vector3.zero, duration).SetEase(Ease.InOutBack);
    
            transform.DOMove(Pos, duration).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                if (isMergeBaggage)
                {
                    Destroy(gameObject);
                }
                else
                {
                    ParticleFactory.SpawnParticle(ParticleType.BaggageMerge, transform.position, null);
    
                    transform.DOScale(Vector3.one * .6f, .1f).SetEase(Ease.InOutBack).OnComplete(() =>
                   {
                       transform.parent = targetAI.baggageSlot.transform;
                       transform.localRotation = Quaternion.identity;
    
                       transform.DOLocalRotate(new Vector3(40, 0, 0), .3f);
    
                       transform.DOLocalMove(Vector3.zero, .35f);
    
                   });
                }
            });
        }
    
        void OnTriggerStay(Collider other)
        {
            switch (other.tag)
            {
                case string value when value == TagHolder.Conveyor_Tag && other.transform.childCount == 0 && !isMoveBand && !isSelect:
    
                    if (SplineFollower != null)
                    {
                        if (SplineFollower.GetPercent() > .9f)
                        {
                            transform.parent = other.transform;
                            other.transform.GetComponent<SplineFollower>().followSpeed = GameManager.Instance.data.conveyorSpeed;
                            isMoveBand = true;
                            Collider.isTrigger = false;
    
                            if (GetComponent<SplineFollower>() != null)
                            {
                                ConveyorBelt.Instance.allRampBaggage.Remove(gameObject);
                                Destroy(GetComponent<BaggageRay>());
                                Destroy(SplineFollower);
                            }
                        }
                    }
                    else
                    {
                        transform.parent = other.transform;
    
                        isMoveBand = true;
                        Collider.isTrigger = false;
                    }
                    break;
            }
        }
    }
    
}
