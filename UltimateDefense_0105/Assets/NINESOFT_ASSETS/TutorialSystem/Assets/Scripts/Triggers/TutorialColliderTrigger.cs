using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [System.Serializable]
    public class TutorialColliderTrigger
    {
        public string CollisionTag;

        public bool IsTrigger;
        public bool Collider2D;

        public Collider YourCollider;
        public Collider2D YourCollider2D;

        public void Initialize(int tutorialIndex, int stageIndex, bool startTheStage)
        {
            CollisionListener listener = null;

            if (Collider2D)
            {

                if (YourCollider2D.gameObject.TryGetComponent<Rigidbody2D>(out var r) == false)
                {
                    if (r == null)
                    {
                        YourCollider2D.gameObject.AddComponent<Rigidbody2D>();
                        TutorialManager.Instance.DebugLog("Added rigidbody2D to Target Collider(name:" + YourCollider2D.gameObject.name + ") 2D in stage '" + TutorialManager.Instance.Tutorials[tutorialIndex].Stages[stageIndex].TutorialStageName + "'", YourCollider2D.gameObject, DebugType.Info);
                    }
                }

                if (YourCollider2D.gameObject.TryGetComponent<CollisionListener>(out listener) == false)
                {
                    listener = YourCollider2D.gameObject.AddComponent<CollisionListener>();
                }
                YourCollider2D.isTrigger = IsTrigger;
                if (IsTrigger)
                {
                    listener.OnTrigger2D += (tag) =>
                    {
                        if (tag.Equals(CollisionTag))
                        {
                            if (startTheStage)
                                TutorialManager.Instance.StageStarted(tutorialIndex, stageIndex);
                            else
                                TutorialManager.Instance.StageCompleted(tutorialIndex, stageIndex);
                        }
                    };
                }
                else
                {
                    listener.OnCollision2D += (tag) =>
                    {
                        if (tag.Equals(CollisionTag))
                        {
                            if (startTheStage)
                                TutorialManager.Instance.StageStarted(tutorialIndex, stageIndex);
                            else
                                TutorialManager.Instance.StageCompleted(tutorialIndex, stageIndex);
                        }
                    };
                }
            }
            else
            {

                if (YourCollider.gameObject.TryGetComponent<Rigidbody>(out var r) == false)
                {
                    if (r == null)
                    {
                        YourCollider.gameObject.AddComponent<Rigidbody>();
                        TutorialManager.Instance.DebugLog("Added rigidbody to Target Collider(name:" + YourCollider.gameObject.name + ") in stage '" + TutorialManager.Instance.Tutorials[tutorialIndex].Stages[stageIndex].TutorialStageName + "'", YourCollider.gameObject, DebugType.Info);
                    }
                }

                if (YourCollider.gameObject.TryGetComponent<CollisionListener>(out listener) == false)
                {
                    listener = YourCollider.gameObject.AddComponent<CollisionListener>();
                }                
                YourCollider.isTrigger = IsTrigger;
                if (IsTrigger)
                {
                    listener.OnTrigger += (tag) =>
                    {
                        if (tag.Equals(CollisionTag))
                        {
                            if (startTheStage)
                                TutorialManager.Instance.StageStarted(tutorialIndex, stageIndex);
                            else
                                TutorialManager.Instance.StageCompleted(tutorialIndex, stageIndex);
                        }
                    };
                }
                else
                {
                    listener.OnCollision += (tag) =>
                    {
                        if (tag.Equals(CollisionTag))
                        {
                            if (startTheStage)
                                TutorialManager.Instance.StageStarted(tutorialIndex, stageIndex);
                            else
                                TutorialManager.Instance.StageCompleted(tutorialIndex, stageIndex);
                        }
                    };
                }
            }
        }


    }
}