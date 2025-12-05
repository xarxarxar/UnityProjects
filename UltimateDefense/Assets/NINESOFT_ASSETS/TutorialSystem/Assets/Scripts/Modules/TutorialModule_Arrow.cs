using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialModule_Arrow : TutorialModule
    {
        [SerializeField] private ArrowMovementType ArrowType;

        public Transform Target;
        [SerializeField] private Vector3 followOffset;
        [SerializeField] private float followSpeed;

        [SerializeField] public TweenData TweenData;
   

        public override IEnumerator ActiveTheModuleEnum()
        {           
            TweenData.TweenAnimation(this);            
            yield return new WaitForEndOfFrame();
        }
                

        private void LateUpdate()
        {
            if (!IsActive) return;
            if (ArrowType == ArrowMovementType.Static) return;
            if (Target == null) { TutorialManager.Instance.DebugLog("(" + gameObject.name + ") Follow Target is null",gameObject, DebugType.Error); return; }

            transform.position = Vector3.Lerp(transform.position, Target.position + followOffset, Time.deltaTime * followSpeed);
        }


    }
}