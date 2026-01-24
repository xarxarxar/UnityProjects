using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_QuestMarker))]
    public class TutorialModule_QuestMarkerEditor : Editor
    {

        public SerializedProperty Target;
        public SerializedProperty Offset;
        public SerializedProperty ShowDistanceText;
        public SerializedProperty PlayerTransform;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }


        private void FindProperties()
        {
            Target = serializedObject.FindProperty("Target");
            Offset = serializedObject.FindProperty("Offset");
            ShowDistanceText = serializedObject.FindProperty("ShowDistanceText");
            PlayerTransform = serializedObject.FindProperty("PlayerTransform");
        }

        private void DrawProperties()
        {
            NSEditorData.DrawComponentTitleBox("QUEST MARKER", NSEditorData.EditorScriptType.ModuleUI);

            if (NSEditorData.DrawTitleBox("QUEST MARKER CONFIG", 0))
            {
                EditorGUILayout.PropertyField(Target);
                EditorGUILayout.PropertyField(Offset);
                EditorGUILayout.Space(10);

                EditorGUILayout.PropertyField(ShowDistanceText);
                if (ShowDistanceText.boolValue)
                {
                    EditorGUILayout.PropertyField(PlayerTransform);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
