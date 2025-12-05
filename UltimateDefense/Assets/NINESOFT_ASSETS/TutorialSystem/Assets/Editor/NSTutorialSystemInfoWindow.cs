using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NINESOFT.CORE.EDITOR;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class NSTutorialSystemInfoWindow : EditorWindow
    {
        const string PackageName = "TUTORIAL SYSTEM";// <<--------- change ASSET NAME       

        [MenuItem("Tools/NINESOFT/Tutorial System")]
        public static void ShowTutorialSystemWindow()
        {
            EditorWindow.GetWindow(typeof(NSTutorialSystemInfoWindow), utility: true);
        }

        [MenuItem("GameObject/Create Tutorial Manager with NS")]
        public static void CreateTutorial()
        {
            var obj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<TutorialManager>("Assets/NINESOFT_ASSETS/TutorialSystem/Assets/Prefabs/Managers/TutorialManager.prefab"));
            Selection.activeObject = obj;
        }

        private void OnGUI()
        {
            this.titleContent = new GUIContent("Ninesoft Tutorial System");
            this.minSize = new Vector2(350, 400);
            this.maxSize = new Vector2(350, 400);

            DrawLayout();
        }
        int toolbarInt;
        void DrawLayout()
        {
            GUI.backgroundColor = NINESOFT.CORE.EDITOR.NSEditorData.TabButtonColor;
            DrawAssetInfo();
            /*  GUIContent[] toolbarStrings = {
                  new GUIContent("Asset Info", NSEditorData.GetIcon("e_doc")),
                 new GUIContent("Settings", NSEditorData.GetIcon("e_settings")),
              };
              toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(40));

              switch (toolbarInt)
              {
                  case 0:
                      DrawAssetInfo();
                      break;

                  case 1:
                      //DrawAdmobSettings();
                      break;

                  default:
                      break;
              }*/
        }

        void DrawAssetInfo()
        {
            
            GUILayout.Space(30f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(NSEditorData.GetIcon("tutorial_monster")), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).PackageName + " v" + NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).Version), NINESOFT.CORE.EDITOR.NSEditorData.CenteredBoldStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            EditorGUILayout.LabelField(NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).ID + " | v" + NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).Version + " | Last Update: " + NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).LastUpdateDate, EditorStyles.centeredGreyMiniLabel);

            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("DOCUMENTATION", NSEditorData.GetIcon("e_external_link")), GUILayout.Height(30f), GUILayout.Width(180)))
            {
                Application.OpenURL(NINESOFT.CORE.NSPackageManager.GetPackageInfo(PackageName).DocLink);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.Space(30f);

            NSEditorData.DrawUILine();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("THANKS FOR PURCHASING!", EditorStyles.whiteLargeLabel, GUILayout.Height(20f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            NSEditorData.DrawUILine();
            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(" NINESOFT", NSEditorData.GetIcon("e_ns_logo")), EditorStyles.centeredGreyMiniLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }
}
