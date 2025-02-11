#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class LevelConfigEditor : EditorWindow
{
    private LevelDatabase database;
    private Vector2 scrollPos;
    private string jsonPath;

    [MenuItem("Tools/关卡编辑器")]
    public static void ShowWindow()
    {
        GetWindow<LevelConfigEditor>("关卡配置");
    }

    void OnEnable()
    {
        LoadDatabase();
        SaveDatabase();  // 自动保存数据库
    }

    void OnDisable()
    {
        SaveDatabase();  // 离开时自动保存数据库
    }


    // 存储当前选中的关卡索引
    private int selectedLevelIndex = -1;
    void OnGUI()
    {
        // 顶部显示标题和保存按钮
        EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("关卡配置", EditorStyles.boldLabel);
        
        // 数据库引用
        database = (LevelDatabase)EditorGUILayout.ObjectField(
            "Database文件",
            database,
            typeof(LevelDatabase),
            false);
        if (GUILayout.Button("保存", GUILayout.Width(80)))
        {
            SaveDatabase();
        }
        if (database == null)
        {
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal(); // 开始左右布局

        // 左侧部分：显示所有关卡的 ID 列表
        EditorGUILayout.BeginVertical(GUILayout.Width(200)); // 限制左侧宽度
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); 

        // 显示所有关卡的 ID，并为每个关卡 ID 添加按钮
        for (int i = 0; i < database.levels.Count; i++)
        {
            LevelConfig config = database.levels[i];

            if (GUILayout.Button($"关卡 {config.levelId}"))
            {
                // 当点击某个关卡 ID 按钮时，选中该关卡并显示其详细信息
                selectedLevelIndex = i;
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space(20);
        // 新关卡按钮
        // 设置按钮的颜色
        GUI.backgroundColor = Color.green; // 你可以选择任何颜色

        if (GUILayout.Button("添加关卡"))
        {
            AddNewLevel();
        }

        // 恢复按钮颜色为默认
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        // 右侧部分：显示选中关卡的详细信息
        EditorGUILayout.BeginVertical("Box");
        if (selectedLevelIndex >= 0 && selectedLevelIndex < database.levels.Count)
        {
            LevelConfig selectedConfig = database.levels[selectedLevelIndex];

            // 显示并确保 levelId 为只读
            EditorGUILayout.LabelField("关卡 ID", selectedConfig.levelId.ToString());

            // 如果 levelName 为空，设置默认值为 "关卡{levelId}"
            if (string.IsNullOrEmpty(selectedConfig.levelName))
            {
                selectedConfig.levelName = $"关卡{selectedConfig.levelId}";
            }

            // 显示并允许用户编辑关卡名称
            selectedConfig.levelName = EditorGUILayout.TextField("关卡名称", selectedConfig.levelName);
            selectedConfig.rounds = EditorGUILayout.IntField("回合数量", selectedConfig.rounds);
            selectedConfig.targetScore = EditorGUILayout.IntField("目标分数", selectedConfig.targetScore);
            selectedConfig.specialCardProbability = EditorGUILayout.Slider(
                "特殊牌出现概率",
                selectedConfig.specialCardProbability,
                0, 1);
            selectedConfig.maxNormalCards = EditorGUILayout.IntField("字母牌初始最大手牌数", selectedConfig.maxNormalCards);
            selectedConfig.maxSpecialCards = EditorGUILayout.IntField("特殊牌初始最大手牌数", selectedConfig.maxSpecialCards);

            DrawMissions(selectedConfig);

            EditorGUILayout.Space(20);
            GUI.backgroundColor = Color.red; // 你可以选择任何颜色
            // 删除关卡按钮
            if (GUILayout.Button("移除关卡"))
            {
                RemoveLevel(selectedLevelIndex);
            }
            // 恢复按钮颜色为默认
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal(); // 结束左右布局

        

        // JSON导出设置
        EditorGUILayout.Space(20);
        GUILayout.Label("JSON导出设置", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();  // 开始横向布局
                                            // 创建一个文本框来显示路径
        jsonPath = EditorGUILayout.TextField("导出路径", jsonPath);

        // 添加一个按钮来选择文件
        if (GUILayout.Button("选择文件", GUILayout.Width(100))) // 按钮宽度设置为100
        {
            // 打开文件选择器，返回用户选择的文件路径
            string selectedFile = EditorUtility.OpenFilePanel("选择导出文件", "", "json");

            if (!string.IsNullOrEmpty(selectedFile))
            {
                jsonPath = selectedFile; // 如果用户选择了文件，则更新路径文本框
            }
        }

        EditorGUILayout.EndHorizontal();  // 结束横向布局


        EditorGUILayout.Space(10);
        if (GUILayout.Button("导出为JSON"))
        {
            ExportToJson();
        }
    }


    // 单独的函数来处理添加新关卡
    void AddNewLevel()
    {
        LevelConfig newLevel = new LevelConfig()
        {
            levelId = database.levels.Count + 1, // 新关卡ID自动递增
        };
        database.levels.Add(newLevel);
    }

    // 单独的函数来处理移除关卡
    void RemoveLevel(int index)
    {
        // 删除关卡后，剩余关卡的ID会自动更新
        database.levels.RemoveAt(index);
        // 更新所有关卡的ID，保证它们递增
        for (int i = 0; i < database.levels.Count; i++)
        {
            database.levels[i].levelId = i + 1;
        }
    }

    void LoadDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelDatabase");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(path);
        }
    }

    void SaveDatabase()
    {
        if (database == null) return;
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Database saved at: {AssetDatabase.GetAssetPath(database)}");
    }

    void ExportToJson()
    {
        if (string.IsNullOrEmpty(jsonPath))
        {
            jsonPath = Application.dataPath + "/Resources/LevelConfigs/levels.json";
        }

        LevelDataWrapper wrapper = new LevelDataWrapper()
        {
            levels = database.levels
        };

        string json = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(jsonPath, json);
        Debug.Log($"JSON exported to: {jsonPath}");
    }


    void DrawMissions(LevelConfig config)
    {
        EditorGUILayout.LabelField("Special Missions", EditorStyles.boldLabel);

        for (int i = 0; i < config.specialMissions.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");
            SpecialMission mission = config.specialMissions[i];

            mission.conditionDescription = EditorGUILayout.TextField("任务描述", mission.conditionDescription);
            mission.missionType = (MissionType)EditorGUILayout.EnumPopup("Mission Type", mission.missionType);

            switch (mission.missionType)
            {
                case MissionType.SpecificCombination:
                    mission.targetLetters = EditorGUILayout.TextField("Target Letters", mission.targetLetters);
                    break;

                case MissionType.ColorSet:
                    mission.requiredColor = (ColorType)EditorGUILayout.EnumPopup("Required Color", mission.requiredColor);
                    mission.minCount = EditorGUILayout.IntField("Min Cards", mission.minCount);
                    break;

                case MissionType.LetterCount:
                    mission.targetLetters = EditorGUILayout.TextField("Target Letter", mission.targetLetters);
                    mission.minCount = EditorGUILayout.IntField("Min Count", mission.minCount);
                    break;

                case MissionType.MixedCondition:
                    mission.targetLetters = EditorGUILayout.TextField("Contains Letters", mission.targetLetters);
                    mission.requiredColor = (ColorType)EditorGUILayout.EnumPopup("Required Color", mission.requiredColor);
                    mission.minCount = EditorGUILayout.IntField("Min Cards", mission.minCount);
                    break;
            }

            mission.bonusScore = EditorGUILayout.IntField("Bonus Score", mission.bonusScore);

            if (GUILayout.Button("Remove Mission"))
            {
                config.specialMissions.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add New Mission"))
        {
            config.specialMissions.Add(new SpecialMission()
            {
                bonusScore = 100,
                minCount = 1
            });
        }
    }


    [System.Serializable]
    private class LevelDataWrapper
    {
        public List<LevelConfig> levels;
    }
}
#endif