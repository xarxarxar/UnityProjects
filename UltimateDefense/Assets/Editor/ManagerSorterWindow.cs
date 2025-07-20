using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ManagerSorterWindow : EditorWindow
{
    private Dictionary<InitStage, List<ManagerBase?>> groupedManagers = new();
    private Dictionary<InitStage, ReorderableList> reorderableLists = new();

    [MenuItem("Tools/Manager Sorter")]
    public static void ShowWindow()
    {
        GetWindow<ManagerSorterWindow>("Manager Sorter");
    }

    private void OnEnable()
    {
        RefreshManagers();
        BuildReorderableLists();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("刷新场景中的 Manager"))
        {
            RefreshManagers();
            BuildReorderableLists();
        }

        EditorGUILayout.Space();

        foreach (var kvp in reorderableLists)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($" 阶段: {kvp.Key}", EditorStyles.boldLabel);
            kvp.Value.DoLayoutList();
        }

        if (GUILayout.Button("根据当前顺序设置 Index"))
        {
            ApplyIndex();
        }
    }

    private void RefreshManagers()
    {
        groupedManagers.Clear();

        var all = FindObjectsOfType<ManagerBase>(true);
        foreach (InitStage stage in System.Enum.GetValues(typeof(InitStage)))
        {
            var list = all.Where(m => m.Stage == stage).OrderBy(m => m.Index).Cast<ManagerBase?>().ToList();

            // 插入断点
            List<ManagerBase?> withBreaks = new();
            var grouped = list.GroupBy(m => m.Index).OrderBy(g => g.Key);
            foreach (var group in grouped)
            {
                withBreaks.AddRange(group);
                if (!group.Equals(grouped.Last()))
                {
                    withBreaks.Add(null); // 断点
                }
            }

            groupedManagers[stage] = withBreaks;
        }
    }

    private void BuildReorderableLists()
    {
        reorderableLists.Clear();

        foreach (var stage in groupedManagers.Keys)
        {
            var list = groupedManagers[stage];

            var reorderableList = new ReorderableList(list, typeof(ManagerBase), true, true, true, true);
            reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, $"Manager 列表（{stage} 阶段）");
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var manager = list[index];

                if (manager == null)
                {
                    int groupIndex = 0;
                    for (int i = 0; i < index; i++)
                        if (list[i] == null) groupIndex++;

                    var boldLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = Color.cyan }
                    };

                    EditorGUI.LabelField(rect, $"==== Index {groupIndex} 分组断点 ====", boldLabelStyle);
                }
                else
                {
                    float buttonWidth = 50f;
                    float typeWidth = 180f;
                    float indexWidth = 70f;
                    float descWidth = rect.width - buttonWidth - typeWidth - indexWidth;

                    var buttonRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
                    var typeRect = new Rect(buttonRect.xMax, rect.y, typeWidth, rect.height);
                    var indexRect = new Rect(typeRect.xMax, rect.y, indexWidth, rect.height);
                    var descRect = new Rect(indexRect.xMax, rect.y, descWidth, rect.height);

                    EditorGUI.LabelField(typeRect, manager.GetType().Name);
                    EditorGUI.LabelField(indexRect, $"顺序: {manager.Index}");
                    EditorGUI.LabelField(descRect, $"== {manager.Description}");

                    if (GUI.Button(buttonRect, "打开"))
                    {
                        var monoScript = MonoScript.FromMonoBehaviour(manager as MonoBehaviour);
                        if (monoScript != null)
                            AssetDatabase.OpenAsset(monoScript);
                    }
                }
            };

            reorderableList.elementHeightCallback = index =>
            {
                return list[index] == null ? EditorGUIUtility.singleLineHeight + 4 : EditorGUIUtility.singleLineHeight;
            };

            reorderableList.onAddCallback = l =>
            {
                list.Add(null); // 添加断点
                reorderableList.list = list;
            };

            reorderableList.onReorderCallback = l =>
            {
                groupedManagers[stage] = reorderableList.list.Cast<ManagerBase?>().ToList();
                Repaint();
            };

            reorderableLists[stage] = reorderableList;
        }
    }

    private void ApplyIndex()
    {
        foreach (var kvp in groupedManagers)
        {
            int currentIndex = 0;
            var managers = kvp.Value;

            Undo.RecordObjects(managers.Where(m => m != null).Cast<ManagerBase>().ToArray(), $"Set Index ({kvp.Key})");

            foreach (var m in managers)
            {
                if (m == null)
                {
                    currentIndex++;
                }
                else
                {
                    m.Index = currentIndex;
                    EditorUtility.SetDirty(m);
                }
            }
        }

        Debug.Log("所有阶段 Index 赋值完成。");
    }
}
