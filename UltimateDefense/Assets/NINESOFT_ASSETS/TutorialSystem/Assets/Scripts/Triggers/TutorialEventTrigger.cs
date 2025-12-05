using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialEventTrigger : MonoBehaviour
    {
        public EventType EventType = EventType.ManualCall;
        public StandardAction MyStandardEvent;

        private StandardAction onEnable;
        private StandardAction onDisable;
        private StandardAction onDestroy;
        private StandardAction onClick;

        private void Awake()
        {
            switch (EventType)
            {
                case EventType.None:
                    EventType = EventType.ManualCall;
                    break;
                case EventType.OnEnable:
                    onEnable += CallMyEvent;
                    break;
                case EventType.OnDisable:
                    onDisable += CallMyEvent;
                    break;
                case EventType.OnDestroy:
                    onDestroy += CallMyEvent;
                    break;
                case EventType.OnClick:
                    if (TryGetComponent<Collider>(out var col))
                    {
                        onClick += CallMyEvent;
                    }
                    else
                    {
                        TutorialManager.Instance.DebugLog("Please add a collider! Event Trigger:" + gameObject.name, gameObject);
                    }
                    break;
                case EventType.ManualCall:
                    break;
                default:
                    break;
            }
        }


        public void CallMyEvent()
        {
            MyStandardEvent?.Invoke();
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }
        private void OnDisable()
        {
            onDisable?.Invoke();
        }
        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
        private void OnMouseDown()
        {
            onClick?.Invoke();
        }

    }
}