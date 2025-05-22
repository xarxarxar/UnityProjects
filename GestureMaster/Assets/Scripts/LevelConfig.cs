// LevelConfig.cs
// 存储关卡配置（回合数、倒计时、手势类型）
using UnityEngine;
using System.Collections.Generic;

public class LevelConfig : MonoBehaviour
{
    public static LevelConfig instance;

    public const int minRoundCount = 10;
    public const int maxRoundCount = 10;

    [HideInInspector] public int RoundCount;
     public float WaitTime=4.0f;
    public List<int> gestureCategory = new(); // 每一回合的手势玩法（0=相同, 1=相反, 2=RPS）

    private void Awake() => instance = this;
}