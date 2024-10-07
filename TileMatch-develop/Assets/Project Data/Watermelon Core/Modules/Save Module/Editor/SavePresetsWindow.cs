using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Watermelon
{
    public class SavePresetsWindow : EditorWindow
    {
        private const string PRESET_PREFIX = "savePreset_";
        private const string PRESET_FOLDER_PREFIX = "SavePresets/";
        private const string PRESETS_ORDER_FILE = "presetsOrderFile";


        private const int UPDATE_BUTTON_WIDTH = 80;
        private const int ACTIVATE_BUTTON_WIDTH = 80;
        private const int SHARE_BUTTON_WIDTH = 18;
        private const int DATE_LABEL_WIDTH = 50;
        private const int DEFAULT_SPACE = 8;

        private static readonly Vector2 WINDOW_SIZE = new Vector2(490, 495);
        private static readonly string WINDOW_TITLE = "Save Presets";
        private int tempTabIndex;
        private Vector2 scrollView;

        private List<SavePreset> allSavePresets;
        private string tempPresetName;
        private List<SavePreset> selectedSavePresets;
        private List<SavePreset> unselectedSavePresets;
        private List<SavePreset> sortedSavePresets;
        private ReorderableList savePresetsList;
        private Rect workRect;
        private string[] presetsPrefixes;
        private SavePresetType[] presetTypeValues;
        private string[] tabNames;
        private SavePresetType[] tabValues;
        private int selectedTabIndex;
        private GUIContent shareButtonContent;
        private GUIContent importContent;

        [MenuItem("Tools/Save Presets")]
        [MenuItem("Window/Save Presets")]
        static void ShowWindow()
        {
            SavePresetsWindow tempWindow = (SavePresetsWindow)GetWindow(typeof(SavePresetsWindow), false, WINDOW_TITLE);
            tempWindow.minSize = WINDOW_SIZE;
            tempWindow.titleContent = new GUIContent(WINDOW_TITLE, WatermelonEditor.Styles.GetIcon("icon_title"));
        }

        protected void OnEnable()
        {
            SavePresets.saveDataMofied = false;
            allSavePresets = new List<SavePreset>();
            List<SavePreset> unsortedSavePresets = new List<SavePreset>();
            string directoryPath = SavePresets.GetDirectoryPath();


            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string[] fileEntries = Directory.GetFiles(Application.persistentDataPath);
            string[] order = new string[0];
            string name;

            //move files into new folder
            for (int i = 0; i < fileEntries.Length; i++)
            {
                if (fileEntries[i].Contains(PRESET_PREFIX))
                {
                    name = Path.GetFileName(fileEntries[i]).Replace(PRESET_PREFIX, "");
                    File.Move(fileEntries[i], SavePresets.GetPresetPath(name));

                }

                if (fileEntries[i].Contains(PRESETS_ORDER_FILE))
                {
                    File.Move(fileEntries[i], SavePresets.GetOrderFilePath());
                }
            }

            fileEntries = Directory.GetFiles(directoryPath);
            CollectPresetPrefixes();

            //load
            for (int i = 0; i < fileEntries.Length; i++)
            {
                for (int j = 0; j < presetsPrefixes.Length; j++)
                {
                    if (fileEntries[i].Contains(presetsPrefixes[j]))
                    {
                        unsortedSavePresets.Add(new SavePreset(File.GetCreationTimeUtc(fileEntries[i]), fileEntries[i], presetsPrefixes[j], presetTypeValues[j]));
                    }
                }
                

                if (fileEntries[i].Contains(PRESETS_ORDER_FILE))
                {
                    order = File.ReadAllLines(fileEntries[i]);
                }
            }

            for (int i = 0; i < order.Length; i++)
            {
                for (int j = 0; j < unsortedSavePresets.Count; j++)
                {
                    if (order[i].Equals(SavePresets.GetPresetPrefix(unsortedSavePresets[j].presetType) + unsortedSavePresets[j].name))
                    {
                        allSavePresets.Add(unsortedSavePresets[j]);
                        unsortedSavePresets.RemoveAt(j);
                        break;
                    }
                }
            }

            allSavePresets.AddRange(unsortedSavePresets);
            selectedSavePresets = new List<SavePreset>();
            unselectedSavePresets = new List<SavePreset>();
            sortedSavePresets = new List<SavePreset>();

            List<SavePresetType> tempTabValues = new List<SavePresetType>();
            List<string> tempTabNames = new List<string>();

            tempTabValues.Add(SavePresetType.Custom); // make sure it is a first tab
            tempTabNames.Add(Enum.GetName(typeof(SavePresetType), SavePresetType.Custom));

            for (int i = 0; i < allSavePresets.Count; i++)
            {
                if (!tempTabValues.Contains(allSavePresets[i].presetType))
                {
                    tempTabValues.Add(allSavePresets[i].presetType);
                    tempTabNames.Add(Enum.GetName(typeof(SavePresetType), allSavePresets[i].presetType));
                }
            }

            tabNames = tempTabNames.ToArray();
            tabValues = tempTabValues.ToArray();
            SelectTab(0);

            savePresetsList = new ReorderableList(selectedSavePresets, typeof(SavePreset), true, false, false, true);
            savePresetsList.elementHeight = 26;
            savePresetsList.drawElementCallback = DrawElement;
            savePresetsList.onRemoveCallback = RemoveCallback;
            savePresetsList.onChangedCallback = ListChangedCallback;
            workRect = new Rect();
            shareButtonContent = new GUIContent(WatermelonEditor.Styles.GetIcon("icon_share"));
            importContent = EditorGUIUtility.IconContent("d_Import");
        }

        private void SelectTab(int index)
        {
            selectedTabIndex = index;
            selectedSavePresets.Clear();
            unselectedSavePresets.Clear();

            for (int i = 0; i < allSavePresets.Count; i++)
            {
                if (allSavePresets[i].presetType == tabValues[selectedTabIndex])
                {
                    selectedSavePresets.Add(allSavePresets[i]);
                }
                else
                {
                    unselectedSavePresets.Add(allSavePresets[i]);
                }
            }
        }

        private void RemoveCallback(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("This preset will be removed!", "Are you sure?", "Remove", "Cancel"))
            {
                RemovePreset(savePresetsList.index);
                savePresetsList.ClearSelection();
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            workRect.Set(rect.x + rect.width, rect.y + 4, 0, 18);

            workRect.x -= SHARE_BUTTON_WIDTH + DEFAULT_SPACE;
            workRect.width = SHARE_BUTTON_WIDTH;

            if(GUI.Button(workRect, shareButtonContent))
            {
                EditorUtility.RevealInFinder(selectedSavePresets[index].path);
            }


            if (selectedTabIndex == 0)
            {
                workRect.x -= UPDATE_BUTTON_WIDTH + DEFAULT_SPACE;
                workRect.width = UPDATE_BUTTON_WIDTH;


                if (GUI.Button(workRect, "Update"))
                {
                    if (EditorUtility.DisplayDialog("This preset will rewrited!", "Are you sure?", "Rewrite", "Cancel"))
                    {
                        UpdatePreset(selectedSavePresets[index]);
                    }
                }
            }

            workRect.x -= ACTIVATE_BUTTON_WIDTH + DEFAULT_SPACE;
            workRect.width = ACTIVATE_BUTTON_WIDTH;

            if (GUI.Button(workRect, "Activate", WatermelonEditor.Styles.button_03))
            {
                SavePresets.LoadSave(selectedSavePresets[index].name, tabValues[selectedTabIndex]);
            }

            workRect.x -= DATE_LABEL_WIDTH + DEFAULT_SPACE;
            workRect.width = DATE_LABEL_WIDTH;

            GUI.Label(workRect, selectedSavePresets[index].creationDate.ToString("dd.MM"));

            workRect.x -= DEFAULT_SPACE;
            workRect.width = workRect.x - rect.x;
            workRect.x = rect.x;

            GUI.Label(workRect, selectedSavePresets[index].name);
        }

        private void ListChangedCallback(ReorderableList list)
        {
            SaveListOrder();
        }

        private void OnDisable()
        {

        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(WatermelonEditor.Styles.Skin.box);

            tempTabIndex = GUILayout.Toolbar(selectedTabIndex, tabNames);

            if(tempTabIndex != selectedTabIndex)
            {
                SelectTab(tempTabIndex);
            }

            scrollView = EditorGUILayoutCustom.BeginScrollView(scrollView);
            savePresetsList.DoLayoutList();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove all"))
            {
                if (EditorUtility.DisplayDialog("All presets will be removed!", "Are you sure?", "Remove", "Cancel"))
                {
                    savePresetsList.ClearSelection();

                    for (int i = selectedSavePresets.Count - 1; i >= 0; i--)
                    {
                        RemovePreset(i);
                    }
                }

            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            if (selectedTabIndex == 0)
            {
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal(WatermelonEditor.Styles.Skin.box);

                if (GUILayout.Button(importContent, GUILayout.Width(32)))
                {
                    ImportFile();
                }


                tempPresetName = EditorGUILayout.TextField(tempPresetName);

                if (GUILayout.Button("Add"))
                {
                    AddNewPreset(tempPresetName);

                    tempPresetName = "";

                    SavePresets.saveDataMofied = true;
                    GUI.FocusControl(null);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            if (SavePresets.saveDataMofied)
            {
                Reload();
            }
        }

        private void ImportFile()
        {
            string originalFilePath =  EditorUtility.OpenFilePanel("Select save to import", string.Empty, string.Empty);

            if(originalFilePath.Length == 0)
            {
                Debug.LogError("Path can`t be empty.");
                return;
            }

            string name = originalFilePath.Substring(originalFilePath.LastIndexOf('/') + 1);

            if (name.Contains('.'))
            {
                Debug.LogError("Selected invalid save preset.");
                return;
            }

            for (int i = 0; i < presetsPrefixes.Length; i++)
            {
                if (name.StartsWith(presetsPrefixes[i]))
                {
                    name = name.Substring(presetsPrefixes[i].Length);
                    break;
                }
            }

            name = SavePresets.GetPresetPrefix(SavePresetType.Custom) + name;
            string newFilePath = Path.Combine(SavePresets.GetDirectoryPath(), name);
            File.Copy(originalFilePath, newFilePath, true);
            SavePresets.saveDataMofied = true;
        }

        public void Reload()
        {
            int tabIndex = selectedTabIndex;
            OnEnable();
            SelectTab(tabIndex);
        }

        private void RemovePreset(int index)
        {
            if (selectedSavePresets.IsInRange(index))
            {
                
                // Delete preset file
                File.Delete(selectedSavePresets[index].path);

                // Remove preset from the lists
                allSavePresets.Remove(selectedSavePresets[index]);
                selectedSavePresets.RemoveAt(index);
            }
        }

        private void UpdatePreset(SavePreset savePreset)
        {
            if (EditorApplication.isPlaying)
                SaveController.Save(true);

            string savePath = SavePresets.GetSavePath();
            if (!File.Exists(savePath))
            {
                Debug.LogError("[Save Presets]: Save file doesn’t  exist!");
                return;
            }

            if (savePreset != null)
            {
                savePreset.creationDate = DateTime.Now;

                if (EditorApplication.isPlaying)
                {
                    SaveController.PresetsSave(PRESET_FOLDER_PREFIX + SavePresets.GetPresetPrefix(savePreset.presetType) + savePreset.name);
                }
                else
                {
                    File.Copy(savePath, savePreset.path, true);
                }

                File.SetCreationTime(savePreset.path, DateTime.Now);
            }
        }

        private void OnDestroy()
        {
            SaveListOrder();
        }

        private void SaveListOrder()
        {
            sortedSavePresets.Clear();
            sortedSavePresets.AddRange(selectedSavePresets);
            sortedSavePresets.AddRange(unselectedSavePresets);
            sortedSavePresets.Sort(TypeSort);
            allSavePresets.Clear();
            allSavePresets.AddRange(sortedSavePresets);

            string[] order = new string[allSavePresets.Count];

            for (int i = 0; i < order.Length; i++)
            {
                order[i] = SavePresets.GetPresetPrefix(sortedSavePresets[i].presetType) +  sortedSavePresets[i].name;
            }

            File.WriteAllLines(SavePresets.GetOrderFilePath(), order);
        }

        private int TypeSort(SavePreset x, SavePreset y)
        {
            return x.presetType.CompareTo(y.presetType);
        }

        private void AddNewPreset(string name)
        {
            if (EditorApplication.isPlaying)
                SaveController.Save(true);

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[Save Presets]: Preset name can't be empty!");
                return;
            }

            if (allSavePresets.FindIndex(x => x.name == name) != -1)
            {
                Debug.LogError(string.Format("[Save Presets]: Preset with name {0} already exist!", name));
                return;
            }

            string savePath = SavePresets.GetSavePath();

            if (!File.Exists(savePath))
            {
                Debug.LogError("[Save Presets]: Save file doesn’t exist!");

                return;
            }

            string presetPath = SavePresets.GetPresetPath(name);


            if (EditorApplication.isPlaying)
            {
                SaveController.PresetsSave(PRESET_FOLDER_PREFIX + PRESET_PREFIX + name);
            }
            else
            {
                File.Copy(savePath, presetPath, true);
            }
        }


        private void CollectPresetPrefixes()
        {
            presetTypeValues = (SavePresetType[])Enum.GetValues(typeof(SavePresetType));
            presetsPrefixes = new string[presetTypeValues.Length];

            for (int i = 0; i < presetTypeValues.Length; i++)
            {
                presetsPrefixes[i] = SavePresets.GetPresetPrefix(presetTypeValues[i]);
            }
        }

        private class SavePreset
        {
            public string name;
            public DateTime creationDate;
            public string path;
            public SavePresetType presetType;

            public SavePreset(DateTime lastModifiedDate, string path)
            {
                creationDate = lastModifiedDate;
                this.path = path;
                name = SavePresets.GetPresetName(path, PRESET_PREFIX);
                presetType = SavePresetType.Custom;
            }

            public SavePreset(DateTime lastModifiedDate, string path, string presetPrefix, SavePresetType savePresetType)
            {
                creationDate = lastModifiedDate;
                this.path = path;
                name = SavePresets.GetPresetName(path, presetPrefix);
                presetType = savePresetType;
            }
        }
    }
}