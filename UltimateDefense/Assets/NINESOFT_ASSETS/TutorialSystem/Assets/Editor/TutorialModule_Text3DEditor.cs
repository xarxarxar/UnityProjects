using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_Text3D))]
    public class TutorialModule_Text3DEditor : Editor
    {
        public SerializedProperty text;
        public SerializedProperty textContent;

        public SerializedProperty TweenData;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }

        private void FindProperties()
        {
            text = serializedObject.FindProperty("text");
            textContent = serializedObject.FindProperty("textContent");
                        
            TweenData = serializedObject.FindProperty("TweenData");
        }

        private void DrawProperties()
        {

            NSEditorData.DrawComponentTitleBox("TEXT 3D", NSEditorData.EditorScriptType.Module3D);

            if (NSEditorData.DrawTitleBox("TEXT", 10))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Content Text:", GUILayout.Width(Screen.width * .3f));
                text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.Height(50f), GUILayout.Width(Screen.width * .6f));
                EditorGUILayout.EndHorizontal();

                NSEditorData.DrawUILine();
                EditorGUILayout.PropertyField(textContent);
            }

            TweenData.isExpanded = NSEditorData.DrawTitleBox("TEXT ANIMATION", 7);
            if (TweenData.isExpanded)
            {
                EditorGUILayout.PropertyField(TweenData);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
