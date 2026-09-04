using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 一键清空游戏存档工具：清空 PlayerPrefs + 删除 persistentDataPath/_SAVE 本地存档。
/// 用法：Unity 菜单 Tools > Clear All Data (PlayerPrefs + Save)
/// 注意：请在非 Play 模式下执行（停止运行后再点），否则运行中的代码会重新写入存档。
/// </summary>
public static class ClearAllData
{
    [MenuItem("Tools/Clear All Data (PlayerPrefs + Save)")]
    public static void ClearAll()
    {
        bool ok = EditorUtility.DisplayDialog("确认清空",
            "将清空 PlayerPrefs 和本地存档(_SAVE)，游戏进度将回到首次启动状态。\n确定继续吗？",
            "确定清空", "取消");
        if (!ok) return;

        // 1. 清空 PlayerPrefs
        PlayerPrefs.DeleteAll();

        // 2. 删除本地存档文件夹
        string saveFolder = Application.persistentDataPath + "/_SAVE/";
        if (Directory.Exists(saveFolder))
        {
            Directory.Delete(saveFolder, true);
        }

        Debug.Log("[ClearAll] 已清空 PlayerPrefs");
        Debug.Log("[ClearAll] 已删除存档目录: " + saveFolder);
        EditorUtility.DisplayDialog("完成", "存档已清空，游戏将回到首次启动状态。\n再次点击 Play 即为全新进度。", "好的");
    }
}
