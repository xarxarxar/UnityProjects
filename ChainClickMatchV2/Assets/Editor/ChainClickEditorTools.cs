using UnityEditor;
using UnityEngine;

/// <summary>
/// ChainClickMatch 编辑器工具（顶部菜单栏 → ChainClickMatch）。
/// 测试期清空本机持久化数据：删除 DataManager 的游戏存档（PlayerPrefs key）。
/// </summary>
public static class ChainClickEditorTools
{
    /// <summary>清空游戏存档（关卡进度/星级/道具数量），带确认对话框。</summary>
    [MenuItem("ChainClickMatch/清空持久化数据（测试）", priority = 100)]
    public static void ClearPersistentData()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "清空持久化数据",
            "确定要删除游戏存档吗？\n\n存档 key：" + DataManager.SaveKey + "\n\n将重置：关卡进度、每关星级、道具剩余数量。",
            "清空",
            "取消");
        if (!confirmed)
        {
            return;
        }

        PlayerPrefs.DeleteKey(DataManager.SaveKey);
        PlayerPrefs.Save();
        Debug.Log("[ChainClickEditorTools] 已清空持久化存档：" + DataManager.SaveKey);
    }

    /// <summary>查看当前持久化数据（存档 JSON 打印到 Console，弹窗预览）。</summary>
    [MenuItem("ChainClickMatch/查看当前持久化数据", priority = 101)]
    public static void ViewPersistentData()
    {
        string key = DataManager.SaveKey;
        string json = PlayerPrefs.GetString(key, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            EditorUtility.DisplayDialog("查看持久化数据", "当前没有存档（key: " + key + "）", "确定");
            Debug.Log("[ChainClickEditorTools] 当前没有存档：" + key);
            return;
        }

        Debug.Log("[ChainClickEditorTools] 当前存档（" + key + "）：\n" + json);
        // 弹窗预览：超长文本截断（完整内容在 Console，可复制）
        string preview = json.Length > 4000 ? json.Substring(0, 4000) + "\n…（已截断，完整内容见 Console）" : json;
        EditorUtility.DisplayDialog("查看持久化数据", "key: " + key + "\n\n" + preview, "确定");
    }
}
