using System.Collections;
using System.Collections.Generic;
using NINESOFT.TUTORIAL_SYSTEM;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    [CustomPropertyDrawer(typeof(TutorialColliderTrigger), true)]

    public class TutorialColliderTriggerDrawer : CustomPropertyDrawerBase, ICustomPropertyDrawer
    {
        public SerializedProperty CollisionTag;
        public SerializedProperty IsTrigger;
        public SerializedProperty Collider2D;

        public SerializedProperty YourCollider;
        public SerializedProperty YourCollider2D;

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
            TotalLines = 6;

            FindProperties(property);
            property.isExpanded = true;

            currentY = 0;
        }

        public void FindProperties(SerializedProperty property)
        {
            CollisionTag = property.FindPropertyRelative("CollisionTag");
            IsTrigger = property.FindPropertyRelative("IsTrigger");
            YourCollider = property.FindPropertyRelative("YourCollider");
            YourCollider2D = property.FindPropertyRelative("YourCollider2D");
            Collider2D = property.FindPropertyRelative("Collider2D");
        }
        public override void DrawingMyElements(Rect position)
        {
            DrawPopupCollider2Dor3D(position);
            if (Collider2D.boolValue == false)
                DrawTargetCollider(position);
            else
                DrawTargetCollider2D(position);

            DrawTargetTag(position);
            DrawIsTrigger(position);

        }

        //----------------------------

        private void DrawPopupCollider2Dor3D(Rect position)
        {
            Rect drawArea = NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines);
            Collider2D.boolValue = EditorGUI.Popup(drawArea, Collider2D.boolValue ? 1 : 0, new string[] { "Collider 3D", "Collider 2D" }) == 0 ? false : true;
        }
        private void DrawTargetTag(Rect position)
        {
            Rect drawArea = NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines, 10);
            EditorGUI.PropertyField(drawArea, CollisionTag);
        }
        private void DrawTargetCollider(Rect position)
        {
            Rect drawArea = NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines, 10);
            EditorGUI.PropertyField(drawArea, YourCollider);
        }
        private void DrawTargetCollider2D(Rect position)
        {
            Rect drawArea = NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines, 10);
            EditorGUI.PropertyField(drawArea, YourCollider2D);
        }
        private void DrawIsTrigger(Rect position)
        {
            Rect drawArea = NSEditorData.GetDrawArea(position, ref currentY, ref TotalLines);
            EditorGUI.PropertyField(drawArea, IsTrigger);
        }


    }

}