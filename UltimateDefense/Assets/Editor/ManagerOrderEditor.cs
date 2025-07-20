using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ManagerOrderEditor : EditorWindow
{
    [SerializeField]
    private List<ManagerBase> managers = new List<ManagerBase>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Manager 排序工具")]
    public static void ShowWindow()
    {
        GetWindow<ManagerOrderEditor>("Manager 排序工具");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("请将所有需要排序的 ManagerBase 拖进来", EditorStyles.boldLabel);

        // 拖拽区域
        EditorGUILayout.HelpBox("拖拽 ManagerBase 脚本挂载对象到下方列表进行排序", MessageType.Info);

        SerializedObject so = new SerializedObject(this);
        SerializedProperty listProp = so.FindProperty("managers");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.PropertyField(listProp, true);
        EditorGUILayout.EndScrollView();

        so.ApplyModifiedProperties();

        EditorGUILayout.Space();

        if (GUILayout.Button("↑ 上移选中项"))
        {
            MoveSelectedItem(-1);
        }

        if (GUILayout.Button("↓ 下移选中项"))
        {
            MoveSelectedItem(1);
        }

        if (GUILayout.Button("应用排序并写入 Index"))
        {
            ApplyOrderToIndex();
        }
    }

    private void MoveSelectedItem(int direction)
    {
        int selectedIndex = -1;
        for (int i = 0; i < managers.Count; i++)
        {
            if (Selection.activeGameObject == managers[i]?.gameObject)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex != -1)
        {
            int newIndex = selectedIndex + direction;
            if (newIndex >= 0 && newIndex < managers.Count)
            {
                var temp = managers[newIndex];
                managers[newIndex] = managers[selectedIndex];
                managers[selectedIndex] = temp;
            }
        }
        else
        {
            Debug.LogWarning("请在 Hierarchy 中选中一个 Manager 对象以移动顺序。");
        }
    }

    private void ApplyOrderToIndex()
    {
        for (int i = 0; i < managers.Count; i++)
        {
            if (managers[i] != null)
            {
                Undo.RecordObject(managers[i], "修改 Manager Index");
                managers[i].Index = i; // 可修改成你想要的分组逻辑
                EditorUtility.SetDirty(managers[i]);
            }
        }

        Debug.Log("已将 Index 应用至所有 Manager。");
    }
}
