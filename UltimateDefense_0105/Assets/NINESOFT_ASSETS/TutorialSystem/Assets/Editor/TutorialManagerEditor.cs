using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : Editor
    {

        public SerializedProperty DebugMode;

        public SerializedProperty DoNotShowAgainOnceTheTutorialsAreComplete;
        public SerializedProperty SaveTutorialProgress;
        public SerializedProperty SaveStageProgress;
        public SerializedProperty SaveKey;

        public SerializedProperty OnAllTutorialsCompleted;

        public SerializedProperty TutorialModulePrefabs;

        int tutorialCount;
        bool deleteButtonClicked;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple;

            FindProperties();
            DrawProperties();
        }

        public override VisualElement CreateInspectorGUI()
        {
            tutorialCount = ((TutorialManager)target).FindMyTutorials().Length;
            return base.CreateInspectorGUI();
        }
        private void FindProperties()
        {
            DebugMode = serializedObject.FindProperty("DebugMode");
            DoNotShowAgainOnceTheTutorialsAreComplete = serializedObject.FindProperty("DoNotShowAgainOnceTheTutorialsAreComplete");
            SaveTutorialProgress = serializedObject.FindProperty("SaveTutorialProgress");
            SaveStageProgress = serializedObject.FindProperty("SaveStageProgress");
            SaveKey = serializedObject.FindProperty("SaveKey");
            OnAllTutorialsCompleted = serializedObject.FindProperty("OnAllTutorialsCompleted");
            TutorialModulePrefabs = serializedObject.FindProperty("TutorialModulePrefabs");
        }

        private void DrawProperties()
        {

            TutorialManager myObject = (TutorialManager)target;

            NSEditorData.DrawComponentTitleBox("TUTORIAL MANAGER", NSEditorData.EditorScriptType.Manager);


            if (NSEditorData.DrawTitleBox("DEBUG MODE", 0))
            {
                DebugMode.boolValue = NSEditorData.DrawToggle("Debug Mode", DebugMode.boolValue);
            }

            if (NSEditorData.DrawTitleBox("SAVE PROGRESS", 1))
            {
                DoNotShowAgainOnceTheTutorialsAreComplete.boolValue = NSEditorData.DrawToggle("Do not show again\nonce the tutorials\nare complete", DoNotShowAgainOnceTheTutorialsAreComplete.boolValue);

                NSEditorData.DrawUILine();

                SaveTutorialProgress.boolValue = NSEditorData.DrawToggle("Save Tutorial Progress", SaveTutorialProgress.boolValue);

                if (SaveTutorialProgress.boolValue)
                {
                    SaveStageProgress.boolValue = NSEditorData.DrawToggle("Save Stage Progress", SaveStageProgress.boolValue);
                }

                NSEditorData.DrawUILine();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(SaveKey, GUILayout.Width(Screen.width * .8f), GUILayout.Height(30f));
                if (GUILayout.Button(NSEditorData.GetIcon(13), GUILayout.Width(Screen.width * .1f), GUILayout.Height(30f)))
                {
                    SaveKey.stringValue = Utilities.GetRandomKey();
                }
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = NSEditorData.Red;
                if (GUILayout.Button(new GUIContent("DELETE SAVED PROGRESS DATA", NSEditorData.GetIcon(6)), GUILayout.Height(30f)))
                {
                    PlayerPrefs.DeleteKey("ns_tutorialManagerCompleted_" + myObject.SaveKey);
                    PlayerPrefs.DeleteKey("ns_savedTutorialIndex_" + myObject.SaveKey);
                    for (int i = 0; i < myObject.Tutorials.Count; i++)
                    {
                        PlayerPrefs.DeleteKey("ns_savedStageIndex_" + myObject.Tutorials[i].TutorialIndex + "_" + myObject.SaveKey);
                    }
                    deleteButtonClicked = true;
                }
                GUI.backgroundColor = NSEditorData.Purple;
                if (deleteButtonClicked) EditorGUILayout.HelpBox("Saved Data Deleted", MessageType.Info);


            }

            if (NSEditorData.DrawTitleBox("COMPLETE EVENTS", 2))
            {
                EditorGUILayout.PropertyField(OnAllTutorialsCompleted);
            }

            TutorialModulePrefabs.isExpanded = NSEditorData.DrawTitleBox("MODULE PREFABS", 12);
            if (TutorialModulePrefabs.isExpanded)
            {
                EditorGUILayout.PropertyField(TutorialModulePrefabs);
            }


            if (NSEditorData.DrawTitleBox("ADD NEW TUTORIAL", 3))
            {
                if (tutorialCount > 0)
                {
                    string msg = tutorialCount.ToString() + " tutorials found in child objects\n\n";
                    var tutorialNames = myObject.FindMyTutorials();
                    for (int i = 0; i < tutorialNames.Length; i++)
                    {
                        msg += "--> " + tutorialNames[i] + "\n";
                    }
                    EditorGUILayout.HelpBox(msg, MessageType.Info);
                }

                if (GUILayout.Button(new GUIContent("Add New Tutorial", NSEditorData.GetIcon(3)), GUILayout.Height(50)))
                {
                    myObject.AddNewTutorialMyChilds();
                    tutorialCount = myObject.FindMyTutorials().Length;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


    }
}
