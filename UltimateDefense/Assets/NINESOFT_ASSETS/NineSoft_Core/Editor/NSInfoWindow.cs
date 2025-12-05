using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace NINESOFT.CORE.EDITOR
{

    public class NSInfoWindow : EditorWindow
    {


        [MenuItem("Tools/NINESOFT/Ninesoft Core")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NSInfoWindow>(utility: true, title: "Ninesoft Core", focus: true);
        }

        private void OnGUI()
        {
            this.minSize = new Vector2(450, 450);
            this.maxSize = new Vector2(450, 450);

            DrawToolBar();
        }

        int toolbarInt = 0;
        private void DrawToolBar()
        {
            GUI.backgroundColor = NSEditorData.TabButtonColor;


            GUIContent[] toolbarStrings = {
                new GUIContent("MANAGE ASSETS", NSEditorData.GetIcon("e_settings")),
                new GUIContent("Our Other Assets", NSEditorData.GetIcon("e_rocket")),
                new GUIContent("Contact", NSEditorData.GetIcon("e_mail")),
            };
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(40));

            switch (toolbarInt)
            {
                case 0:
                    GUI.backgroundColor = NSEditorData.Gray;
                    DrawHome();
                    break;

                case 1:
                    GUI.backgroundColor = NSEditorData.Gray;
                    DrawOurOtherAssets();
                    break;
                case 2:
                    DrawContact();
                    break;

                default:
                    break;
            }
        }

        Vector2 nineAssetsInThisProjectScrollPos;
        private void DrawHome()
        {
            NSPackageManager.InitPackageInfos();


            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(NSEditorData.GetIcon("e_ns_logo")), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label("NINESOFT", NSEditorData.CenteredBoldStyle);
            NSEditorData.DrawUILine();

            GUILayout.Label("Ninesoft Assets In This Project", EditorStyles.label);

            nineAssetsInThisProjectScrollPos = EditorGUILayout.BeginScrollView(nineAssetsInThisProjectScrollPos, GUILayout.Height(120));
            var packages = NSPackageManager.GetPackages();

            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];

                if (package.ID == "NS_CORE") continue;

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(package.PackageName + " v" + package.Version, NSEditorData.CenteredStyle, GUILayout.Height(30));
                EditorGUILayout.Space(10f);
                if (package.EditorWindowPath.Length > 1)
                {
                    if (GUILayout.Button(new GUIContent("MANAGE", NSEditorData.GetIcon("e_settings")), GUILayout.Height(30), GUILayout.Width(100)))
                    {
                        try
                        {
                            EditorWindow.GetWindow(Type.GetType(package.EditorWindowPath), true);
                        }
                        catch { }
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();

            NSEditorData.DrawUILine();
            GUILayout.Label("Thanks For Purchasing!", NSEditorData.CenteredBoldStyle);
            //  NSEditorData.DrawUILine();


            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            NSPackageInfo pI = NSPackageManager.GetPackageInfo("NS_CORE");
            EditorGUILayout.LabelField(pI.PackageName + " v" + pI.Version, EditorStyles.centeredGreyMiniLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }

        private void DrawContact()
        {
            GUILayout.Space(10f);
            EditorGUILayout.LabelField(new GUIContent("~ CONTACT ~"), NSEditorData.CenteredBoldStyle);
            NSEditorData.DrawUILine();

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = NSEditorData.Purple2;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Join Our Discord", NSEditorData.GetIcon("e_discord")), GUILayout.Height(50f), GUILayout.Width(200)))
            {
                Application.OpenURL("https://discord.gg/uXR4UXM8Zj");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(15f);

            GUI.backgroundColor = NSEditorData.Purple;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Visit Our Website", NSEditorData.GetIcon("e_click")), GUILayout.Height(50f), GUILayout.Width(200)))
            {
                Application.OpenURL("https://9ninesoft9.blogspot.com");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(15f);

            GUI.backgroundColor = NSEditorData.Gray;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Asset Store Page", NSEditorData.GetIcon("e_unity_logo")), GUILayout.Height(50f), GUILayout.Width(200)))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/28895");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(15f);

            GUI.backgroundColor = NSEditorData.Gray2;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Send Mail", NSEditorData.GetIcon("e_mail")), GUILayout.Height(50f), GUILayout.Width(200)))
            {
                Application.OpenURL("mailto:9ninesoft9@gmail.com");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();
        }


        List<EditorWebAssetItem> items = new List<EditorWebAssetItem>();
        float percent = 0;
        private void DrawOurOtherAssets()
        {
            GUILayout.Space(10f);
            EditorGUILayout.LabelField(new GUIContent("~ OUR OTHER ASSETS ~"), NSEditorData.CenteredBoldStyle);
            NSEditorData.DrawUILine();

            UpdateProggress();
            GetDataFromLocalAndDraw();

            if (percent == 0)
            {
                GUILayout.Space(20f);
                if (GUILayout.Button(new GUIContent("Refresh Assets", NSEditorData.GetIcon("e_refresh")), GUILayout.Height(30f)))
                {
                    if (EditorUtility.DisplayDialog("Refresh Assets", "The data will be updated over the internet. This may take some time", "Update", "Cancel"))
                    {
                        items = new List<EditorWebAssetItem>();
                        WebRequestManager.Instance.GetData_OurOtherAssets();
                        WebRequestManager.Instance.OnProgress += (per) => { this.percent = per; };
                        WebRequestManager.Instance.OnComplete += () => { percent = 0; ChangeTexturesInFolder(); };
                        percent = .1f;
                    }
                }
            }
        }
        private void UpdateProggress()
        {
            if (percent < 0.05f) return;
            EditorGUILayout.LabelField("Downloading data...");
            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.height = 15f;
            EditorGUI.ProgressBar(rect, percent, (percent * 100) + "%");
            EditorGUILayout.EndHorizontal();
            Repaint();
        }
        private void GetDataFromLocalAndDraw()
        {
            if (percent != 0) return;

            if (items == null || items.Count == 0)
            {
                items = new List<EditorWebAssetItem>();

                string path = WebRequestManager.PATH + @"\data.txt";
                if (!File.Exists(path))
                {
                    Debug.Log("Not found, please click 'Refresh Assets' button ");
                    return;
                }

                string data = File.ReadAllText(path);

                string[] splitData = data.Split("[s;]");
                for (int i = 0; i < splitData.Length; i++)
                {
                    if (splitData[i].Length < 5) continue;
                    var itm = JsonUtility.FromJson<EditorWebAssetItem>(splitData[i]);
                    items.Add(itm);
                }
            }


            if (items != null && items.Count > 0)
            {
                assetItemsPos = EditorGUILayout.BeginScrollView(assetItemsPos);
                for (int i = 0; i < items.Count; i++)
                {
                    var curItem = items[i];
                    DrawWebAssetItem(curItem);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        Vector2 assetItemsPos;
        private void DrawWebAssetItem(EditorWebAssetItem item)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField(new GUIContent(NSEditorData.GetIcon(item.ID)), GUILayout.Width(210), GUILayout.Height(140));

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(item.Name, EditorStyles.whiteLargeLabel);

            GUILayout.Label(item.Description, EditorStyles.wordWrappedLabel);

            if (GUILayout.Button(new GUIContent("More Details", NSEditorData.GetIcon("e_external_link")), GUILayout.Height(30f), GUILayout.Width(150)))
            {
                Application.OpenURL(item.WebUri);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        //------------------------ Convert Default to Sprite (2D and UI)   
        private static void ChangeTexturesInFolder()
        {
            string folderPath = @"Assets\"+WebRequestManager.FolderPath;
                   
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath }); Debug.Log(folderPath+" "+guids.Length);
            foreach (string guid in guids)
            {
               
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter != null)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.SaveAndReimport();                  
                }
            }          
        }
        //------------------------

    }
}
