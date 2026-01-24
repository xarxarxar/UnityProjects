using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_PopUp))]
    public class TutorialModule_PopUpModuleEditor : Editor
    {
        public SerializedProperty PopUpType;
        public SerializedProperty startDelay;
        public SerializedProperty speed;

        public SerializedProperty OnNextButtonClicked;
        public SerializedProperty OnPopUpOpened;
        public SerializedProperty OnPopUpClosed;

        public SerializedProperty content;
        public SerializedProperty button;

        public SerializedProperty contentText;
        public SerializedProperty buttonText;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }

        private void FindProperties()
        {
            PopUpType = serializedObject.FindProperty("PopUpType");
            startDelay = serializedObject.FindProperty("startDelay");
            speed = serializedObject.FindProperty("speed");

            OnNextButtonClicked = serializedObject.FindProperty("OnNextButtonClicked");
            OnPopUpOpened = serializedObject.FindProperty("OnPopUpOpened");
            OnPopUpClosed = serializedObject.FindProperty("OnPopUpClosed");

            content = serializedObject.FindProperty("content");
            button = serializedObject.FindProperty("button");

            contentText = serializedObject.FindProperty("contentText");
            buttonText = serializedObject.FindProperty("buttonText");
        }

        private void DrawProperties()
        {
            NSEditorData.DrawComponentTitleBox("POPUP", NSEditorData.EditorScriptType.ModuleUI);

          

            if (NSEditorData.DrawTitleBox("POPUP TEXTS", 10))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Content Text:", GUILayout.Width(Screen.width * .3f));
                content.stringValue = EditorGUILayout.TextArea(content.stringValue, GUILayout.Height(50f), GUILayout.Width(Screen.width * .6f));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Button Text:", GUILayout.Width(Screen.width * .3f));
                button.stringValue = EditorGUILayout.TextField(button.stringValue, GUILayout.Width(Screen.width * .6f));
                EditorGUILayout.EndHorizontal();

                NSEditorData.DrawUILine();
                EditorGUILayout.PropertyField(contentText);
                EditorGUILayout.PropertyField(buttonText);
            } 
            
            if (NSEditorData.DrawTitleBox("POPUP ANIMATION", 7))
            {
                EditorGUILayout.PropertyField(PopUpType);
                EditorGUILayout.PropertyField(startDelay);
                EditorGUILayout.PropertyField(speed);
            }

            if (NSEditorData.DrawTitleBox("POPUP EVENTS", 2))
            {            
                EditorGUILayout.PropertyField(OnPopUpOpened);       
                EditorGUILayout.PropertyField(OnPopUpClosed);
                EditorGUILayout.PropertyField(OnNextButtonClicked);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
