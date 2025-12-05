using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_VideoPanel))]
    public class TutorialModule_VideoPanelEditor : Editor
    {
        public SerializedProperty buttonActivationDelay;

        public SerializedProperty videoSize;
        public SerializedProperty videoDepth;

        public SerializedProperty videoClip;
        public SerializedProperty videoImage;

        public SerializedProperty timeBar;
        public SerializedProperty okButton;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }


        private void FindProperties()
        {
            buttonActivationDelay = serializedObject.FindProperty("buttonActivationDelay");

            videoSize = serializedObject.FindProperty("videoSize");
            videoDepth = serializedObject.FindProperty("videoDepth");

            videoClip = serializedObject.FindProperty("videoClip");
            videoImage = serializedObject.FindProperty("videoImage");

            timeBar = serializedObject.FindProperty("timeBar");
            okButton = serializedObject.FindProperty("okButton");
        }

        private void DrawProperties()
        {

            NSEditorData.DrawComponentTitleBox("VIDEO PANEL", NSEditorData.EditorScriptType.ModuleUI);

            if (NSEditorData.DrawTitleBox("VIDEO CONFIG", 11))
            {               
                EditorGUILayout.PropertyField(videoClip);
                EditorGUILayout.PropertyField(videoSize, new GUIContent("Video Resolution"));
                EditorGUILayout.PropertyField(videoDepth);
                NSEditorData.DrawUILine();
                EditorGUILayout.PropertyField(buttonActivationDelay, new GUIContent("Btn Activation Delay"));
            }
            if (NSEditorData.DrawTitleBox("UI ELEMENTS", 9))
            {
                EditorGUILayout.PropertyField(timeBar);
                EditorGUILayout.PropertyField(okButton);
                EditorGUILayout.PropertyField(videoImage);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
