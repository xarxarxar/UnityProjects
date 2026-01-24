using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [CustomEditor(typeof(TutorialModule_DynamicHand))]
    public class TutorialModule_DynamicHandEditor : Editor
    {

        public SerializedProperty Points;
        public SerializedProperty TransformSpace;

        public SerializedProperty speed;
        public SerializedProperty waitingTime;

        public SerializedProperty waitTimeForReplay;
        public SerializedProperty hideBeforeReplay;

        public SerializedProperty normalHand;
        public SerializedProperty clickHand;
        public SerializedProperty hand;
        public SerializedProperty handImage;


        public override void OnInspectorGUI()
        {
            GUI.backgroundColor = NSEditorData.Purple2;

            FindProperties();
            DrawProperties();
        }


        private void FindProperties()
        {

            Points = serializedObject.FindProperty("Points");
            TransformSpace = serializedObject.FindProperty("TransformSpace");

            speed = serializedObject.FindProperty("speed");
            waitingTime = serializedObject.FindProperty("waitingTime");
            waitTimeForReplay = serializedObject.FindProperty("waitTimeForReplay");
            hideBeforeReplay = serializedObject.FindProperty("hideBeforeReplay");

            normalHand = serializedObject.FindProperty("normalHand");
            clickHand = serializedObject.FindProperty("clickHand");
            hand = serializedObject.FindProperty("hand");
            handImage = serializedObject.FindProperty("handImage");

        }

        private void DrawProperties()
        {

            NSEditorData.DrawComponentTitleBox("DYNAMIC HAND", TransformSpace.enumValueIndex == 0 ? NSEditorData.EditorScriptType.Module3D : NSEditorData.EditorScriptType.ModuleUI);

            Points.isExpanded = NSEditorData.DrawTitleBox("HAND CONFIG", 0);
            if (Points.isExpanded)
            {
                EditorGUILayout.PropertyField(Points);

                NSEditorData.DrawUILine();

                EditorGUILayout.PropertyField(speed);
                EditorGUILayout.PropertyField(waitingTime);
                EditorGUILayout.PropertyField(waitTimeForReplay);
                EditorGUILayout.PropertyField(hideBeforeReplay);
            }


            if (NSEditorData.DrawTitleBox("HAND SPRITES", 9))
            {
                switch (this.TransformSpace.enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.PropertyField(hand);
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(handImage);
                        break;
                }

                EditorGUILayout.PropertyField(normalHand);
                EditorGUILayout.PropertyField(clickHand);
            }

            if (NSEditorData.DrawTitleBox("---", 8))
            {
                EditorGUI.BeginDisabledGroup(true);
                GUI.backgroundColor = NSEditorData.Red;
                EditorGUILayout.HelpBox("DO NOT CHANGE THIS FIELD", MessageType.Warning);
                EditorGUILayout.PropertyField(TransformSpace);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
