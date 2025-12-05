using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_Arrow))]
    public class TutorialModule_ArrowEditor : Editor
    {
        public SerializedProperty ArrowMovementType;

        public SerializedProperty Target;
        public SerializedProperty followOffset;
        public SerializedProperty followSpeed;

        public SerializedProperty TweenData;

        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }


        private void FindProperties()
        {
            ArrowMovementType = serializedObject.FindProperty("ArrowType");

            Target = serializedObject.FindProperty("Target");
            followOffset = serializedObject.FindProperty("followOffset");
            followSpeed = serializedObject.FindProperty("followSpeed");

            TweenData = serializedObject.FindProperty("TweenData");
        }

        private void DrawProperties()
        {

            NSEditorData.DrawComponentTitleBox("MARKING ARROW",NSEditorData.EditorScriptType.Module3D);

            if (NSEditorData.DrawTitleBox("ARROW CONFIG", 0))
            {
                if (Application.isPlaying == false)
                    this.ArrowMovementType.enumValueIndex = (int)(ArrowMovementType)EditorGUILayout.EnumPopup("Arrow Follow Type:", (ArrowMovementType)ArrowMovementType.enumValueIndex);

                switch (this.ArrowMovementType.enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.Space(10);
                        EditorGUILayout.PropertyField(Target);
                        EditorGUILayout.PropertyField(followSpeed);
                        EditorGUILayout.PropertyField(followOffset);
                        EditorGUILayout.Space(10);
                        break;
                    case 1:
                        break;
                }
            }

            TweenData.isExpanded = NSEditorData.DrawTitleBox("ARROW ANIMATION",7);
            if (TweenData.isExpanded)
            {
                EditorGUILayout.PropertyField(TweenData);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
