using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NINESOFT.TUTORIAL_SYSTEM;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditorInternal;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialStage))]
    public class TutorialStageEditor : Editor
    {
        public SerializedProperty TutorialStageName;
        public SerializedProperty startDelay;
        public SerializedProperty endDelay;

        public SerializedProperty StageStartTrigger;
        public SerializedProperty StartCollisionTarget;
        public SerializedProperty StartButtonTarget;
        public SerializedProperty StartEventTriggerTarget;
        public SerializedProperty StartDistanceTriggerTarget;

        public SerializedProperty StageEndTrigger;
        public SerializedProperty EndCollisionTarget;
        public SerializedProperty EndButtonTarget;
        public SerializedProperty EndEventTriggerTarget;
        public SerializedProperty EndDistanceTriggerTarget;

        public SerializedProperty OnStageStart;
        public SerializedProperty OnStageEnd;

        public SerializedProperty MyModules;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Green;

            FindProperties();
            DrawProperties();
        }


        private void FindProperties()
        {
            TutorialStageName = serializedObject.FindProperty("TutorialStageName");

            startDelay = serializedObject.FindProperty("startDelay");
            StageStartTrigger = serializedObject.FindProperty("StageStartTrigger");
            StartCollisionTarget = serializedObject.FindProperty("StartCollisionTarget");
            StartButtonTarget = serializedObject.FindProperty("StartButtonTarget");
            StartEventTriggerTarget = serializedObject.FindProperty("StartEventTriggerTarget");
            StartDistanceTriggerTarget = serializedObject.FindProperty("StartDistanceTriggerTarget");

            endDelay = serializedObject.FindProperty("endDelay");
            StageEndTrigger = serializedObject.FindProperty("StageEndTrigger");
            EndCollisionTarget = serializedObject.FindProperty("EndCollisionTarget");
            EndButtonTarget = serializedObject.FindProperty("EndButtonTarget");
            EndEventTriggerTarget = serializedObject.FindProperty("EndEventTriggerTarget");
            EndDistanceTriggerTarget = serializedObject.FindProperty("EndDistanceTriggerTarget");

            OnStageStart = serializedObject.FindProperty("OnStageStart");
            OnStageEnd = serializedObject.FindProperty("OnStageEnd");

            MyModules = serializedObject.FindProperty("MyModules");

        }


        private void DrawProperties()
        {
            TutorialStage myObject = (TutorialStage)target;

            myObject.TutorialStageName = myObject.gameObject.name;

            NSEditorData.DrawComponentTitleBox(myObject.TutorialStageName, NSEditorData.EditorScriptType.Stage);

            if (NSEditorData.DrawTitleBox("STAGE PROPERTIES", 0))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(TutorialStageName);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("If you want to change the stage name change the name of this game object", MessageType.Info);
            }

            if (NSEditorData.DrawTitleBox("STAGE START", 4))
            {
                EditorGUILayout.PropertyField(startDelay);
                StageStartTrigger.enumValueIndex = (int)(TriggerType)EditorGUILayout.EnumPopup("Start Trigger: ", (TriggerType)(StageStartTrigger.enumValueIndex));
                switch (StageStartTrigger.enumValueIndex)
                {
                    case 2: EditorGUILayout.PropertyField(StartCollisionTarget); break;
                    case 3: EditorGUILayout.PropertyField(StartButtonTarget); break;
                    case 4: EditorGUILayout.PropertyField(StartEventTriggerTarget); break;
                    case 5: EditorGUILayout.PropertyField(StartDistanceTriggerTarget); break;
                    case 6:
                        TutorialManager tm = FindObjectOfType<TutorialManager>();
                        for (int i = 0; i < tm.Tutorials.Count; i++)
                        {
                            tm.Tutorials[i].Init(i);
                        }
                        EditorGUILayout.HelpBox(new GUIContent("\nTutorial Index: " + myObject.TutorialIndex + "\nStage Index: " + myObject.StageIndex + "\n\nUsing this code:\nTutorialManager.Instance.StageStarted(" + myObject.TutorialIndex + "," + myObject.StageIndex + ");\n"));
                        break;
                    default: break;
                }

                NSEditorData.DrawUILine();

                EditorGUILayout.LabelField("STAGE START EVENTS:");
                EditorGUILayout.PropertyField(OnStageStart);

            }

            if (NSEditorData.DrawTitleBox("STAGE END", 5))
            {
                EditorGUILayout.PropertyField(endDelay);
                StageEndTrigger.enumValueIndex = (int)(TriggerType)EditorGUILayout.EnumPopup("End Trigger: ", (TriggerType)(StageEndTrigger.enumValueIndex));
                switch (StageEndTrigger.enumValueIndex)
                {
                    case 2: EditorGUILayout.PropertyField(EndCollisionTarget); break;
                    case 3: EditorGUILayout.PropertyField(EndButtonTarget); break;
                    case 4: EditorGUILayout.PropertyField(EndEventTriggerTarget); break;
                    case 5: EditorGUILayout.PropertyField(EndDistanceTriggerTarget); break;
                    case 6:
                        TutorialManager tm = FindObjectOfType<TutorialManager>();
                        for (int i = 0; i < tm.Tutorials.Count; i++)
                        {
                            tm.Tutorials[i].Init(i);
                        }
                        EditorGUILayout.HelpBox(new GUIContent("\nTutorial Index: " + myObject.TutorialIndex + "\nStage Index: " + myObject.StageIndex + "\n\nUsing this code:\nTutorialManager.Instance.StageCompleted(" + myObject.TutorialIndex + "," + myObject.StageIndex + ");\n"));
                        break;
                    default: break;
                }

                NSEditorData.DrawUILine();

                EditorGUILayout.LabelField("STAGE END EVENTS:");
                EditorGUILayout.PropertyField(OnStageEnd);
            }

            MyModules.isExpanded = NSEditorData.DrawTitleBox("MY MODULES", 12);
            if (MyModules.isExpanded)
            {
                EditorGUILayout.PropertyField(MyModules);
            }

            if (NSEditorData.DrawTitleBox("ADD MODULE", 3))
            {

                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.LabelField("3D MODULES", EditorStyles.boldLabel);

                TutorialManager tm = FindObjectOfType<TutorialManager>();
                for (int i = 0; i < tm.TutorialModulePrefabs.Count; i++)
                {
                    var curPrefab = tm.TutorialModulePrefabs[i];
                    if (curPrefab.WorldType != TransformSpaceType.ThreeD) continue;
                    if (GUILayout.Button(new GUIContent("   " + curPrefab.Name, NSEditorData.GetIcon(3)), GUILayout.Height(25f), GUILayout.Width(Screen.width * .9f)))
                    {
                        var createdModule = PrefabUtility.InstantiatePrefab(curPrefab.TutorialModule, myObject.transform);
                        myObject.MyModules.Add((TutorialModule)createdModule);
                        myObject.MyModules.RemoveAll(x => x == null);
                        createdModule.name += " (" + myObject.TutorialStageName + ")";
                        Selection.activeObject = createdModule;
                        tm.DebugLog("Module: " + createdModule.name + " added to Stage: " + myObject.TutorialStageName, null, DebugType.Successful);
                    }
                }

                NSEditorData.DrawUILine();
                EditorGUILayout.LabelField("UI MODULES", EditorStyles.boldLabel);

                for (int i = 0; i < tm.TutorialModulePrefabs.Count; i++)
                {
                    var curPrefab = tm.TutorialModulePrefabs[i];
                    if (curPrefab.WorldType != TransformSpaceType.UI) continue;
                    if (GUILayout.Button(new GUIContent("   " + curPrefab.Name, NSEditorData.GetIcon(3)), GUILayout.Height(25f), GUILayout.Width(Screen.width * .9f)))
                    {
                        Canvas canvas = FindObjectOfType<Canvas>();
                        if (canvas != null)
                        {
                            var createdModule = PrefabUtility.InstantiatePrefab(curPrefab.TutorialModule, canvas.transform);
                            myObject.MyModules.Add((TutorialModule)createdModule);
                            myObject.MyModules.RemoveAll(x => x == null);
                            createdModule.name += " (" + myObject.TutorialStageName + ")"; 
                            Selection.activeObject = createdModule;
                            tm.DebugLog("Module: " + createdModule.name + " added to Stage: " + myObject.TutorialStageName, null, DebugType.Successful);
                        }
                        else
                        {
                            tm.DebugLog("Canvas Not Found! Failed to add module, please add a canvas to the scene", null, DebugType.Error);
                        }
                    }
                }
                GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            }

           
            serializedObject.ApplyModifiedProperties();
        }

        //----------------------------

    }




}
