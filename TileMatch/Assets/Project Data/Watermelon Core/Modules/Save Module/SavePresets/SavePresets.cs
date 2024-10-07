using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using UnityEngine;
using System.IO;

namespace Watermelon
{
    public class SavePresets
    {
        private const string PRESET_PREFIX = "savePreset_";
        private const string PRESET_FOLDER_PREFIX = "SavePresets/";
        private const string PRESETS_ORDER_FILE = "presetsOrderFile";
        private const string PRESETS_FOLDER_NAME = "SavePresets";
        private const string SAVE_FILE_NAME = "save";
        public static bool saveDataMofied = false;

        public static void LoadSave(string name, SavePresetType savePresetType = SavePresetType.Custom)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("[Save Presets]: Preset can't be activated in playmode!");

                return;
            }

            if (EditorApplication.isCompiling)
            {
                Debug.LogError("[Save Presets]: Preset can't be activated during compiling!");

                return;
            }

            string presetPath = GetPresetPath(name, savePresetType);

            if (!File.Exists(presetPath))
            {
                Debug.LogError(string.Format("[Save Presets]: Preset with name {0} doesn’t  exist!", name));

                return;
            }

            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (currentSceneName.Equals("Init") || (currentSceneName.Equals("LevelEditor")))
            {
                EditorSceneManager.OpenScene(@"Assets\Project Data\Game\Scenes\Game.unity");
            }

            // Replace current save file with the preset
            File.Copy(presetPath, GetSavePath(), true);

            // Start game
            EditorApplication.isPlaying = true;
#endif
        }

        public static void CreateSave(string name, SavePresetType savePresetType = SavePresetType.Custom)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                SaveController.Save(true);

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[Save Presets]: Preset name can't be empty!");
                return;
            }

            if (!Directory.Exists(GetDirectoryPath()))
            {
                Directory.CreateDirectory(GetDirectoryPath());
            }

            string savePath = GetSavePath();

            if (!File.Exists(savePath))
            {
                Debug.LogError("[Save Presets]: Save file doesn’t exist!");
                return;
            }

            string presetPath = GetPresetPath(name, savePresetType);

            if (EditorApplication.isPlaying)
            {
                SaveController.PresetsSave(PRESET_FOLDER_PREFIX + GetPresetPrefix(savePresetType) + name);
            }
            else
            {
                File.Copy(savePath, presetPath, true);
            }

            File.SetCreationTime(presetPath, DateTime.Now);
            saveDataMofied = true;
#endif
        }

        public static void RemoveSave(string name, SavePresetType savePresetType = SavePresetType.Custom)
        {
            string presetPath = GetPresetPath(name, savePresetType);
            File.Delete(presetPath);
            saveDataMofied = true;
        }

        public static bool IsSaveExist(string name, SavePresetType savePresetType = SavePresetType.Custom)
        {
            string presetPath = GetPresetPath(name, savePresetType);
            return File.Exists(presetPath);
        }

        public static string[] GetAllSaveNames(SavePresetType savePresetType = SavePresetType.Custom)
        {
            List<string> result = new List<string>();
            string directoryPath = GetDirectoryPath();
            string presetPrefix = GetPresetPrefix(savePresetType);

            if (!Directory.Exists(directoryPath))
            {
                return result.ToArray();
            }


            string[] fileEntries = Directory.GetFiles(directoryPath);

            for (int i = 0; i < fileEntries.Length; i++)
            {
                if (fileEntries[i].Contains(presetPrefix))
                {
                    result.Add(GetPresetName(fileEntries[i], presetPrefix));
                }
            }

            return result.ToArray();
        }


        public static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }

        public static string GetPresetPath(string name, SavePresetType savePresetType = SavePresetType.Custom)
        {
            return Path.Combine(Application.persistentDataPath, PRESETS_FOLDER_NAME, GetPresetPrefix(savePresetType) + name);
        }

        public static string GetOrderFilePath()
        {
            return Path.Combine(Application.persistentDataPath, PRESETS_FOLDER_NAME, PRESETS_ORDER_FILE);
        }

        public static string GetDirectoryPath()
        {
            return Path.Combine(Application.persistentDataPath, PRESETS_FOLDER_NAME);
        }

        public static string GetPresetName(string path, string presetPrefix)
        {
            return Path.GetFileName(path).Replace(presetPrefix, string.Empty);
        }

        public static string GetPresetPrefix(SavePresetType savePresetType)
        {
            if (savePresetType == SavePresetType.Custom)
            {
                return PRESET_PREFIX;
            }
            else
            {
                return FirstCharacterToLower(Enum.GetName(typeof(SavePresetType), savePresetType));
            }
        }

        public static string FirstCharacterToLower(string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsLower(s, 0))
            {
                return s;
            }

            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
