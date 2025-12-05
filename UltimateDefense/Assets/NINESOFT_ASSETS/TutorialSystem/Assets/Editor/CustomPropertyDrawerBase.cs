using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    public interface ICustomPropertyDrawer
    {
        public void SetVariables(Rect position, SerializedProperty property, GUIContent label);
        public void FindProperties(SerializedProperty property);
        public void DrawMyElements(Rect position, SerializedProperty property, bool drawBG = false);
    }

    public class CustomPropertyDrawerBase : PropertyDrawer
    {
        protected int TotalLines = 1;
        protected float currentY = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; 
            if (property.isExpanded)
            {
                totalLines += TotalLines;
            }
           
            return NSEditorData.HEIGHT * totalLines;
        }


        public void DrawMyElements(Rect position, SerializedProperty property, bool drawBG = false)
        {
            if (property.isExpanded)
            {
                DrawingMyElements(position);
                if (drawBG) NSEditorData.DrawBg(position, true, TotalLines);
            }
            else
            {
                NSEditorData.DrawBg(position, false, TotalLines);
            }
        }

        public virtual void DrawingMyElements(Rect position)
        {

        }


    }
}
