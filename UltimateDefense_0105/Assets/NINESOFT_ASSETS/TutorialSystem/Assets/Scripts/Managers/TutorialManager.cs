using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] bool DebugMode = true;
        public bool DoNotShowAgainOnceTheTutorialsAreComplete;
        public bool SaveTutorialProgress;
        public bool SaveStageProgress;
        public string SaveKey;
        [Space(10)]
        public List<Tutorial> Tutorials = new List<Tutorial>();
        private int currentTutorialIndex;

        public static TutorialManager Instance = null;
        public StandardAction OnUpdate;

        public UnityEvent OnAllTutorialsCompleted;

        public List<TutorialModuleStruct> TutorialModulePrefabs;

        public bool AllTutorialsCompleted
        {
            get
            {
                return PlayerPrefs.GetInt("ns_tutorialManagerCompleted_" + SaveKey) == 1;
            }
            private set
            {
                PlayerPrefs.SetInt("ns_tutorialManagerCompleted_" + SaveKey, value ? 1 : 0);
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            try
            {
                if (gameObject.activeInHierarchy)
                    PrefabUtility.UnpackPrefabInstance(gameObject, UnityEditor.PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            catch { }

            try
            {
                if (SaveKey.Length < 2)
                {
                    if (gameObject.scene.name != null)
                        SaveKey = Utilities.GetRandomKey();
                }
                if (gameObject.scene.name == null)
                {
                    SaveKey = "0";
                }
            }
            catch { }

            InitializeTutorials();

        }
#endif

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(this);
        }


        private void Start()
        {
            InitializeTutorials();

            if (DoNotShowAgainOnceTheTutorialsAreComplete)
            {
                if (AllTutorialsCompleted)
                {
                    gameObject.SetActive(false);
                    return;
                }

                OnAllTutorialsCompleted.AddListener(() =>
                {
                    AllTutorialsCompleted = true;
                    gameObject.SetActive(false);
                });
            }

            ShowNextTutorial();
        }

        private void InitializeTutorials()
        {
            try
            {
                Tutorials = GetComponentsInChildren<Tutorial>().ToList();

                currentTutorialIndex = 0;
                for (int i = 0; i < Tutorials.Count; i++)
                {
                    Tutorials[i].Init(i);
                }

                if (SaveTutorialProgress)
                {
                    int c = PlayerPrefs.GetInt("ns_savedTutorialIndex_" + SaveKey);
                    for (int i = 0; i < c; i++)
                    {
                        Tutorials[i].OnThisTutorialStarted?.Invoke();

                        if (SaveStageProgress)
                        {
                            int x = PlayerPrefs.GetInt("ns_savedStageIndex_" + i + "_" + SaveKey);
                            for (int j = 0; j <= x; j++)
                            {
                                Tutorials[i].Stages[j].OnStageStart?.Invoke();
                                Tutorials[i].Stages[j].OnStageEnd?.Invoke();
                            }
                            Tutorials[i].CurrentStageIndex = x;
                        }
                        Tutorials[i].OnThisTutorialCompleted?.Invoke();
                    }
                    currentTutorialIndex = c;
                }
            }
            catch
            {

            }
        }


        public void ShowNextTutorial()
        {
            if (currentTutorialIndex > Tutorials.Count - 1)
            {
                OnAllTutorialsCompleted?.Invoke();
                DebugLog("All tutorials completed!", gameObject, DebugType.Successful);
                return;
            }

            currentTutorialIndex++;
            Tutorials[currentTutorialIndex - 1].StartTheTutorial();

            if (SaveTutorialProgress)
                PlayerPrefs.SetInt("ns_savedTutorialIndex_" + SaveKey, currentTutorialIndex - 1 <= 0 ? 0 : currentTutorialIndex - 1);

        }

        public bool StageStarted(int tutorialIndex, int stageIndex)
        {
            if (tutorialIndex < 0 || tutorialIndex > Tutorials.Count - 1)
            {
                DebugLog("Tutorial not found! tutorial index: " + tutorialIndex, gameObject, DebugType.Error);
                return false;
            }

            if (stageIndex < 0 || stageIndex > Tutorials[tutorialIndex].Stages.Count - 1)
            {
                DebugLog("Stage not found! tutorial index: " + tutorialIndex + ", stage index: " + stageIndex, gameObject, DebugType.Error);
                return false;
            }

            if (currentTutorialIndex - 1 == tutorialIndex && Tutorials[tutorialIndex].CurrentStageIndex - 1 == stageIndex)
            {
                Tutorials[tutorialIndex].Stages[stageIndex].StageStarted();
                return true;
            }
            return false;
        }

        public bool StageCompleted(int tutorialIndex, int stageIndex)
        {
            if (tutorialIndex < 0 || tutorialIndex > Tutorials.Count - 1)
            {
                DebugLog("Tutorial not found! tutorial index: " + tutorialIndex, gameObject, DebugType.Error);
                return false;
            }

            if (stageIndex < 0 || stageIndex > Tutorials[tutorialIndex].Stages.Count - 1)
            {
                DebugLog("Stage not found! tutorial index: " + tutorialIndex + ", stage index: " + stageIndex, gameObject, DebugType.Error);
                return false;
            }

            if (currentTutorialIndex - 1 == tutorialIndex && Tutorials[tutorialIndex].CurrentStageIndex - 1 == stageIndex)
            {
                Tutorials[tutorialIndex].Stages[stageIndex].StageCompleted();
                return true;
            }
            return false;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        public bool CompleteTheTutorial(int tutorialIndex)
        {
            if (tutorialIndex > Tutorials.Count - 1 || tutorialIndex < 0) return false;

            Tutorials[tutorialIndex].FinishTheTutorial();
            return true;
        }
        public bool CompleteTheTutorialStage(int tutorialIndex, int stageIndex)
        {
            if (tutorialIndex > Tutorials.Count - 1 || tutorialIndex < 0) return false;
            if (stageIndex > Tutorials[currentTutorialIndex].Stages.Count - 1 || stageIndex < 0) return false;

            Tutorials[tutorialIndex].Stages[stageIndex].StageCompleted();
            return true;
        }
        public void SkipAllTutorials()
        {
            if (AllTutorialsCompleted) return;

            for (int i = 0; i < Tutorials.Count; i++)
            {
                Tutorials[i].SkipTutorial();
            }

            OnAllTutorialsCompleted?.Invoke();
            AllTutorialsCompleted = true;
            DebugLog("Skipped All Tutorials!", gameObject, DebugType.Successful);

            gameObject.SetActive(false);
        }
        public void SkipCurrentTutorial()
        {
            if (currentTutorialIndex - 1 > Tutorials.Count - 1) { SkipAllTutorials(); return; }
            var curTutorial = Tutorials[currentTutorialIndex - 1];
            curTutorial.SkipTutorial();

            if (currentTutorialIndex > Tutorials.Count - 1) return;

            currentTutorialIndex++;
            Tutorials[currentTutorialIndex - 1].StartTheTutorial();

            if (SaveTutorialProgress)
                PlayerPrefs.SetInt("ns_savedTutorialIndex_" + SaveKey, currentTutorialIndex - 1 <= 0 ? 0 : currentTutorialIndex - 1);
        }

        public void DebugLog(string message, GameObject context, DebugType type = DebugType.Normal)
        {
            if (!DebugMode) return;

            string whiteColor = "FFFFFF";
            string redColor = "FF4058";
            string blueColor = "00D2FF";
            string greenColor = "0EFF0B";

            string msg = "<color=#9F7FFF>NINESOFT TUTORIAL SYSTEM:</color>\n";

            msg += "<color=#";
            switch (type)
            {
                case DebugType.None:
                    break;
                case DebugType.Normal:
                    msg += whiteColor;
                    break;
                case DebugType.Info:
                    msg += blueColor;
                    break;
                case DebugType.Successful:
                    msg += greenColor;
                    break;
                case DebugType.Error:
                    msg += redColor;
                    break;
                default:
                    break;
            }
            msg += ">";
            msg += message;
            msg += "</color>";

            if (context != null)
                Debug.Log(msg, context);
            else
                Debug.Log(msg);
        }

#if UNITY_EDITOR
        public string[] FindMyTutorials()
        {
            Tutorials = GetComponentsInChildren<Tutorial>().ToList();
            string[] tutorialNames = new string[Tutorials.Count];
            for (int i = 0; i < tutorialNames.Length; i++)
            {
                tutorialNames[i] = Tutorials[i].TutorialName;
            }
            return tutorialNames;
        }
        public void AddNewTutorialMyChilds()
        {
            string name = "Tutorial " + (transform.childCount + 1);
            GameObject g = new GameObject(name.ToString());
            g.AddComponent<Tutorial>().TutorialName = name;
            g.transform.SetParent(transform);

            UnityEditor.Selection.activeGameObject = g;
        }
#endif
    }


}
