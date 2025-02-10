using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 数据容器
[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Game Config/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public string version;  // 本地的版本号
    public List<LevelConfig> levels = new List<LevelConfig>();
}
