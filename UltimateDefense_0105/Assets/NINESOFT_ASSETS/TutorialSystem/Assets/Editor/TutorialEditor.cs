using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(Tutorial))]
    public class TutorialEditor : Editor
    {

        public SerializedProperty TutorialName;
        public SerializedProperty OnThisTutorialStarted;
        public SerializedProperty OnThisTutorialCompleted;


        int stageCount;


        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Blue;

            FindProperties();
            DrawProperties();
        }
        public override VisualElement CreateInspectorGUI()
        {
            stageCount = ((Tutorial)target).FindMyTutorialStages().Length;
            return base.CreateInspectorGUI();
        }

        private void FindProperties()
        {
            TutorialName = serializedObject.FindProperty("TutorialName");
            OnThisTutorialStarted = serializedObject.FindProperty("OnThisTutorialStarted");
            OnThisTutorialCompleted = serializedObject.FindProperty("OnThisTutorialCompleted");
        }

        private void DrawProperties()
        {
            Tutorial myObject = (Tutorial)target;
            myObject.TutorialName = myObject.gameObject.name;

            NSEditorData.DrawComponentTitleBox(myObject.TutorialName, NSEditorData.EditorScriptType.Tutorial);

            if (NSEditorData.DrawTitleBox("PROPERTIES", 0))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(TutorialName);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("If you want to change the tutorial name change the name of this game object", MessageType.Info);
            }


            if (NSEditorData.DrawTitleBox("TUTORIAL EVENTS", 2))
            {
                EditorGUILayout.PropertyField(OnThisTutorialStarted);
                EditorGUILayout.PropertyField(OnThisTutorialCompleted);
            }


            if (NSEditorData.DrawTitleBox("ADD NEW STAGE", 3))
            {
                if (stageCount > 0)
                {
                    string msg = stageCount.ToString() + " stages found in this tutorial\n\n";
                    var tutorialNames = myObject.FindMyTutorialStages();
                    for (int i = 0; i < tutorialNames.Length; i++)
                    {
                        msg += "--> " + tutorialNames[i] + "\n";
                    }
                    EditorGUILayout.HelpBox(msg, MessageType.Info);
                }

                if (GUILayout.Button(new GUIContent("Add New Stage", NSEditorData.GetIcon(3)), GUILayout.Height(50)))
                {
                    myObject.AddNewTutorialStageMyChilds();
                    stageCount = myObject.FindMyTutorialStages().Length;
                }

            }


            serializedObject.ApplyModifiedProperties();
        }
    }

}