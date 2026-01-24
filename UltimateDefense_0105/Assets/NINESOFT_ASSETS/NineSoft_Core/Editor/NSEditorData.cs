using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NINESOFT.CORE.EDITOR
{
    public class NSEditorData : MonoBehaviour
    {

        public static Color32 TabButtonColor = new Color32(208, 204, 255, 255);

        public static Color32 Purple = new Color32(208, 204, 255, 255);
        public static Color32 Purple2 = new Color32(153, 153, 255, 255);
        public static Color32 Blue = new Color32(142, 231, 255, 255);
        public static Color32 Green = new Color32(168, 255, 176, 255);
        public static Color32 Red = new Color32(255, 35, 84, 255);
        public static Color32 Gray = new Color32(80, 80, 80, 255);
        public static Color32 Gray2 = new Color32(150, 150, 150, 255);

        public static float HEIGHT => EditorGUIUtility.singleLineHeight;
        public static float WidthPercent = .95f;
        public static float LeftPaddingPercent = .025f;

        public static GUIStyle CenteredBoldStyle
        {
            get
            {
                var centeredStyle = new GUIStyle(GUI.skin.label);
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 16;
                centeredStyle.fontStyle = FontStyle.Bold;
                return centeredStyle;
            }

        }

        public static GUIStyle CenteredStyle
        {
            get
            {
                var centeredStyle = new GUIStyle(GUI.skin.label);
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 14;
                centeredStyle.fontStyle = FontStyle.Normal;
                return centeredStyle;
            }

        }



        public static void ShowPackageWindow()
        {
            if (PlayerPrefs.GetInt("ns_window_showed") == 1) return;
            PlayerPrefs.SetInt("ns_window_showed", 1);
            NSInfoWindow.ShowWindow();
        }


        public static void DrawUILine(int thickness = 1, int padding = 10,int rightLeftPadding=2)
        {
            GUILayout.Space(5);
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x += rightLeftPadding;
            r.width -= rightLeftPadding*2;
            EditorGUI.DrawRect(r, new Color(.4f, .4f, .4f, 1));
            GUILayout.Space(5);
        }

        public static Texture GetIcon(int idx = -1)
        {
            return idx != -1 ? Resources.Load<Texture>(idx.ToString()) : null;
        }

        public static Texture GetIcon(string name)
        {
            return Resources.Load<Texture>(name);
        }




    }
}
