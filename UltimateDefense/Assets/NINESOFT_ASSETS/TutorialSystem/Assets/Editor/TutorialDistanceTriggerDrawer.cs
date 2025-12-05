using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    [CustomPropertyDrawer(typeof(TutorialDistanceTrigger), true)]

    public class TutorialDistanceTriggerDrawer : CustomPropertyDrawerBase, ICustomPropertyDrawer
    {
        public SerializedProperty A;
        public SerializedProperty B;
        public SerializedProperty MinDistance;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SetVariables(position, property, label);
            DrawMyElements(position, property, true);

            EditorGUI.EndProperty();
        }

        //----------------------------
        public void SetVariables(Rect position, SerializedProperty property, GUIContent label)
        {
            TotalLines = 4;

            FindProperties(property);
            property.isExpanded = true;

            currentY = 0;
        }

        public void FindProperties(SerializedProperty property)
        {
            A = property.FindPropertyRelative("A");
            B = property.FindPropertyRelative("B");
            MinDistance = property.FindPropertyRelative("MinDistance");
        }
        public override void DrawingMyElements(Rect position)
        {
            try
            {
                Transform a = (Transform)(A.objectReferenceValue);
                Transform b = (Transform)(B.objectReferenceValue);
                if (a != null && b != null)
                {
                    float dist = Vector3.Distance(a.position, b.position);
                    EditorGUILayout.HelpBox("Current distance: " + dist,MessageType.Info);
                }
            }
            catch { }

            EditorGUI.PropertyField(NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines), A);
            EditorGUI.PropertyField(NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines), B);
            MinDistance.floatValue = EditorGUI.FloatField(NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines, 5), new GUIContent("Min Distance"), MinDistance.floatValue);

        }

        //----------------------------
    }
}