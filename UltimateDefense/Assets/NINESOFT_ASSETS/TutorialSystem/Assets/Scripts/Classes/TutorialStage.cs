using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    [System.Serializable]


    public class TutorialStage : MonoBehaviour
    {

        //  [Header("- STAGE SETTINGS -")]
        public string TutorialStageName;

        [SerializeField] private float startDelay = 0f;
        [SerializeField] private float endDelay = 0f;

        public int TutorialIndex;
        public int StageIndex;
        [HideInInspector]public bool StageIsCompleted;


        //  [Header("- STAGE TRIGGERS -")]

        [SerializeField] public TriggerType StageStartTrigger = TriggerType.AutomaticRun;
        [SerializeField] public TutorialColliderTrigger StartCollisionTarget;
        [SerializeField] public Button StartButtonTarget;
        [SerializeField] public TutorialEventTrigger StartEventTriggerTarget;
        [SerializeField] public TutorialDistanceTrigger StartDistanceTriggerTarget;


        [SerializeField] public TriggerType StageEndTrigger;
        [SerializeField] public TutorialColliderTrigger EndCollisionTarget;
        [SerializeField] public Button EndButtonTarget;
        [SerializeField] public TutorialEventTrigger EndEventTriggerTarget;
        [SerializeField] public TutorialDistanceTrigger EndDistanceTriggerTarget;

        // [Header("- STAGE EVENTS -")]       

        public UnityEvent OnStageStart;
        public UnityEvent OnStageEnd;

        // [Header("- MODULES -")]      

        public List<TutorialModule> MyModules = new List<TutorialModule>();


        public void Init(int tutorialIndex, int stageIndex)
        {
            TutorialIndex = tutorialIndex;
            StageIndex = stageIndex;

            ModulesActiveStatus(false);
        }

        public void StartTheStage()
        {
            switch (StageStartTrigger)
            {
                case TriggerType.None:
                    TutorialManager.Instance.DebugLog("Stage: " + TutorialStageName + ", Please select a Trigger Type for StageStartTrigger. ('None' is invalid for StartTrigger)", gameObject, DebugType.Error);
                    break;
                case TriggerType.AutomaticRun:
                    StageStarted();
                    break;
                case TriggerType.Collision:
                    StartCollisionTarget.Initialize(TutorialIndex, StageIndex, true);
                    break;
                case TriggerType.ButtonClick:
                    StartButtonTarget.onClick.AddListener(() => StageStarted());
                    break;
                case TriggerType.Event:
                    StartEventTriggerTarget.MyStandardEvent += () => StageStarted();
                    break;
                case TriggerType.Distance:
                    TutorialManager.Instance.OnUpdate -= StartDistanceTriggerTarget.CheckDistance;
                    TutorialManager.Instance.OnUpdate += StartDistanceTriggerTarget.CheckDistance;
                    StartDistanceTriggerTarget.OnApproached += () => StageStarted();
                    break;
                default:
                    break;
            }

            switch (StageEndTrigger)
            {
                case TriggerType.None:
                    TutorialManager.Instance.DebugLog("Stage: " + TutorialStageName + ", Please select a Trigger Type for StageEndTrigger. ('None' is invalid for EndTrigger)", gameObject, DebugType.Error);
                    break;
                case TriggerType.AutomaticRun:
                    TutorialManager.Instance.StartCoroutine(StageCompletedEnum());
                    break;
                case TriggerType.Collision:
                    EndCollisionTarget.Initialize(TutorialIndex, StageIndex, false);
                    break;
                case TriggerType.ButtonClick:
                    EndButtonTarget.onClick.AddListener(() => StageCompleted());
                    break;
                case TriggerType.Event:
                    EndEventTriggerTarget.MyStandardEvent += () => StageCompleted();
                    break;
                case TriggerType.Distance:
                    TutorialManager.Instance.OnUpdate -= EndDistanceTriggerTarget.CheckDistance;
                    TutorialManager.Instance.OnUpdate += EndDistanceTriggerTarget.CheckDistance;
                    EndDistanceTriggerTarget.OnApproached += () => StageCompleted();
                    break;
                default:
                    break;
            }
        }

        public void StageStarted()
        {
            TutorialManager.Instance.StartCoroutine(StageStartedEnum());
        }

        private IEnumerator StageStartedEnum()
        {
            if (StageIsCompleted) { TutorialManager.Instance.Tutorials[TutorialIndex].NextStage(); yield break; }
            yield return new WaitForSeconds(startDelay);

            TutorialManager.Instance.DebugLog("Stage: " + TutorialStageName + " started", gameObject, DebugType.Info);
            OnStageStart?.Invoke();

            MyModules.RemoveAll(m => m == null);
            ModulesActiveStatus(true);
        }

        private IEnumerator StageCompletedEnum()
        {
            yield return new WaitForSeconds(endDelay + .05f);
            StageCompleted();
        }

        public void StageCompleted()
        {
            if (StageIsCompleted) return;
            StageIsCompleted = true;

            TutorialManager.Instance.DebugLog("Stage: " + TutorialStageName + " completed", gameObject, DebugType.Successful);
            OnStageEnd?.Invoke();

            ModulesActiveStatus(false);

            TutorialManager.Instance.Tutorials[TutorialIndex].NextStage();
        }

       

        public void ModulesActiveStatus(bool active)
        {
            for (int i = 0; i < MyModules.Count; i++)
            {
                if (MyModules[i] != null)
                    MyModules[i].gameObject.SetActive(active);
            }
        }

    }

}