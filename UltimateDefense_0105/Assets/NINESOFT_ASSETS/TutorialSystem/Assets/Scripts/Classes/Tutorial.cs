using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    [System.Serializable]
    public class Tutorial : MonoBehaviour
    {

        public string TutorialName;
        public int TutorialIndex { get; private set; }
        private bool TutorialIsCompleted;

        [HideInInspector] public int CurrentStageIndex;
        [Space(10)]
        public List<TutorialStage> Stages = new List<TutorialStage>();

        public UnityEvent OnThisTutorialStarted;
        public UnityEvent OnThisTutorialCompleted;

        public void Init(int tutorialIndex)
        {
            Stages = GetComponentsInChildren<TutorialStage>().ToList();

            TutorialIndex = tutorialIndex;
            for (int i = 0; i < Stages.Count; i++)
            {
                Stages[i].Init(TutorialIndex, i);
            }
        }

        public void StartTheTutorial()
        {
            if (TutorialIsCompleted)
            {
                TutorialManager.Instance.ShowNextTutorial();
                return;
            }
            OnThisTutorialStarted?.Invoke();
            CurrentStageIndex = 0;
            NextStage();
        }

        public void NextStage()
        {
            if (CurrentStageIndex > Stages.Count - 1)
            {
                FinishTheTutorial();
                return;
            }

            CurrentStageIndex++;
            Stages[CurrentStageIndex - 1].StartTheStage();


            if (TutorialManager.Instance.SaveTutorialProgress && TutorialManager.Instance.SaveStageProgress)
                PlayerPrefs.SetInt("ns_savedStageIndex_" + TutorialIndex + "_" + TutorialManager.Instance.SaveKey, CurrentStageIndex-1 <= 0 ? 0 : CurrentStageIndex-1);
        }

        public void FinishTheTutorial()
        {
            if (TutorialIsCompleted) return;
            TutorialIsCompleted = true;

            OnThisTutorialCompleted?.Invoke();
            TutorialManager.Instance.DebugLog("Tutorial: " + TutorialName + " completed", gameObject, DebugType.Successful);
            TutorialManager.Instance.ShowNextTutorial();
        }

        public void SkipTutorial()
        {
            if (TutorialIsCompleted) return;
            OnThisTutorialStarted?.Invoke();
            for (int i = 0; i < Stages.Count; i++)
            {
                Stages[i].OnStageStart?.Invoke();
                Stages[i].StageIsCompleted = true;
                Stages[i].OnStageEnd?.Invoke();
                Stages[i].ModulesActiveStatus(false);
            }
            TutorialIsCompleted = true;
            OnThisTutorialCompleted?.Invoke();
            TutorialManager.Instance.DebugLog("Tutorial: " + TutorialName + " skipped", gameObject, DebugType.Successful);
        }

#if UNITY_EDITOR


        public string[] FindMyTutorialStages()
        {
            Stages = GetComponentsInChildren<TutorialStage>().ToList();
            string[] stageNames = new string[Stages.Count];
            for (int i = 0; i < stageNames.Length; i++)
            {
                stageNames[i] = Stages[i].TutorialStageName;
            }
            return stageNames;
        }
        public void AddNewTutorialStageMyChilds()
        {
            string name = (transform.name) + " - Stage " + (transform.childCount + 1);
            GameObject g = new GameObject(name.ToString());
            g.AddComponent<TutorialStage>().TutorialStageName = name;
            g.transform.SetParent(transform);
            UnityEditor.Selection.activeGameObject = g;
        }
#endif
    }
}