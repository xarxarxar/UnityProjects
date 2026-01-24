using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class NSEditorData : MonoBehaviour
    {
        public enum EditorScriptType { None, Module3D, ModuleUI, Component, Stage, Tutorial, Manager }

        public static Color32 Purple = new Color32(146, 113, 217, 255);
        public static Color32 Purple2 = new Color32(153, 153, 255, 255);
        public static Color32 Blue = new Color32(142, 231, 255, 255);
        public static Color32 Green = new Color32(168, 255, 176, 255);
        public static Color32 Red = new Color32(255, 35, 84, 255);
        public static Color32 Gray = new Color32(250, 250, 250, 255);

        public static float HEIGHT => EditorGUIUtility.singleLineHeight;
        public static float WidthPercent = .95f;
        public static float LeftPaddingPercent = .025f;

        private static Dictionary<string, bool> boolListForTitleBoxes = new Dictionary<string, bool>();
        private static bool allExpanded;

         public static void ShowPackageWindow()
         {
             if (PlayerPrefs.GetInt(NSPackageInfo.PackageName + "_showed") == 1) return;
             PlayerPrefs.SetInt(NSPackageInfo.PackageName + "_showed", 1);
            NSTutorialSystemInfoWindow.ShowTutorialSystemWindow();         
         }

        public static float GetElementPositionX(Rect position)
        {
            float x = position.min.x + (position.size.x * LeftPaddingPercent);
            return x;
        }
        public static float GetElementWidth(Rect position)
        {
            float width = position.size.x * WidthPercent;
            return width;
        }

        public static Rect GetDrawArea(Rect position, ref float currentY, ref int totalLines, float space = 0f, int lineCount = 1)
        {
            Rect r = new Rect(GetElementPositionX(position), position.min.y + (currentY += HEIGHT + space), GetElementWidth(position), (HEIGHT) * .95f);
            currentY += (HEIGHT * (lineCount - 1));
            totalLines += lineCount - 1;
            return r;
        }

        public static void DrawBg(Rect position, bool isExpanded, int totalLines)
        {
            Rect drawArea = new Rect(position.min.x, position.min.y, position.size.x, HEIGHT * (totalLines + 1));
            if (!isExpanded) drawArea = new Rect(position.min.x, position.min.y, position.size.x, HEIGHT);
            EditorGUI.HelpBox(drawArea, "", MessageType.None);

        }

        public static void DrawUILine(int thickness = 1, int padding = 10)
        {
            GUILayout.Space(5);
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width -= 2;
            EditorGUI.DrawRect(r, new Color(.4f, .4f, .4f, 1));
            GUILayout.Space(5);
        }

        public static void DrawComponentTitleBox(string title, EditorScriptType type)
        {
            GUILayout.Space(5);
            string iconName = "";
            string titleName = "";
            switch (type)
            {
                case EditorScriptType.None:
                    break;
                case EditorScriptType.Module3D:
                    iconName = "modules";
                    titleName = "MODULE (3D)";
                    break;
                case EditorScriptType.ModuleUI:
                    iconName = "modules";
                    titleName = "MODULE (UI)";
                    break;
                case EditorScriptType.Component:
                    iconName = "component";
                    titleName = "COMPONENT";
                    break;
                case EditorScriptType.Manager:
                    iconName = "manager";
                    titleName = "MANAGER";
                    break;
                case EditorScriptType.Stage:
                    iconName = "stages";
                    titleName = "STAGE";
                    break;
                case EditorScriptType.Tutorial:
                    iconName = "tutorial";
                    titleName = "TUTORIAL";
                    break;
                default:
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            title = "   " + titleName + "\n   " + title;

            GUIContent gui = new GUIContent(title, GetIcon(iconName));
            EditorGUILayout.LabelField(gui, EditorStyles.whiteLargeLabel, GUILayout.Height(50f), GUILayout.Width(Screen.width * .8f));

            Color currentColor = GUI.backgroundColor;
            GUI.backgroundColor = Gray;
            if (GUILayout.Button(GetIcon(allExpanded ? "double_arrow_down" : "double_arrow_right"), GUILayout.Height(30f), GUILayout.Width(Screen.width * .1f)))
            {
                allExpanded = !allExpanded;
                for (int i = 0; i < boolListForTitleBoxes.Count; i++)
                {
                    string key = boolListForTitleBoxes.ElementAt(i).Key;
                    boolListForTitleBoxes[key] = allExpanded;
                }
            }
            GUI.backgroundColor = currentColor;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            ShowPackageWindow();
            NINESOFT.CORE.EDITOR.NSEditorData.ShowPackageWindow();
        }

        public static Texture GetIcon(int idx = -1)
        {
            return idx != -1 ? Resources.Load<Texture>(idx.ToString()) : null;
        }

        public static Texture GetIcon(string name)
        {
            return Resources.Load<Texture>(name);
        }

        public static bool DrawTitleBox(string title, int iconID = -1)
        {

            if (!boolListForTitleBoxes.ContainsKey(title))
            {
                boolListForTitleBoxes.Add(title, false);
            }

            GUILayout.Space(5);
            Color currentColor = GUI.backgroundColor;
            GUI.backgroundColor = Gray;

            Texture arrowIcon = Resources.Load<Texture>(boolListForTitleBoxes[title] ? "arrow_down" : "arrow_right");

            EditorGUILayout.BeginHorizontal();

            float width = Screen.width;

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(new GUIContent(title, GetIcon(iconID)), GUILayout.Height(30), GUILayout.Width(width * .8f)))
            {
                boolListForTitleBoxes[title] = !boolListForTitleBoxes[title];
            }

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button(arrowIcon, GUILayout.Height(30), GUILayout.Width(width * .1f)))
            {
                boolListForTitleBoxes[title] = !boolListForTitleBoxes[title];
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = currentColor;

            GUILayout.Space(5);
            return boolListForTitleBoxes[title];
        }

        public static bool DrawToggle(string title, bool value)
        {
            int lineCount = title.Split('\n').Length;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("", GetIcon(value ? "toggle_on" : "toggle_off")), EditorStyles.boldLabel, GUILayout.Height(30), GUILayout.Width(30)))
            {
                value = !value;
            }

            if (GUILayout.Button(title, EditorStyles.whiteLargeLabel, GUILayout.Height(lineCount * 20), GUILayout.Width(Screen.width * .8f)))
            {
                value = !value;
            }

            EditorGUILayout.EndHorizontal();

            return value;
        }

    }
}
