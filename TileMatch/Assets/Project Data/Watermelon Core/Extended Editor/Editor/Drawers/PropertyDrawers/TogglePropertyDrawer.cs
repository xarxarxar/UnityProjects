using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    [PropertyDrawer(typeof(ToggleAttribute))]
    public class TogglePropertyDrawer : PropertyDrawer
    {
        private int selectedToolBarIndex;
        string[] toolBarOptions = { "On", "Off" };

        public override void DrawProperty(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            selectedToolBarIndex = property.boolValue ? 0 : 1;
            EditorGUILayout.PrefixLabel(property.displayName);
            selectedToolBarIndex = GUILayout.Toolbar(selectedToolBarIndex, toolBarOptions);
            property.boolValue = (selectedToolBarIndex == 0);
            EditorGUILayout.EndHorizontal();
        }



        
    }
}
