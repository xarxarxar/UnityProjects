using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对局内金币管理
/// </summary>
public class BattleGoldManager : MonoBehaviour
{
    public static BattleGoldManager Instance;

    [Header("初始金币")]
    public int startGold = 0;
    public int CurrentGold { get; private set; }
    // UI 更新事件（UI监听这个事件）
    public event Action<int> OnGoldChanged;
    [SerializeField] private Text text; 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LevelManager.OnInit += Init;
    }

    /// <summary>
    /// 对局开始时调用
    /// </summary>
    public void Init()
    {
        CurrentGold = startGold;
        text.text = CurrentGold.ToString();
        OnGoldChanged?.Invoke(CurrentGold);
    }

    /// <summary>
    /// 增加金币（可以为负）
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount == 0) return;

        CurrentGold += amount;
        if (CurrentGold < 0) CurrentGold = 0;
        text.text = CurrentGold.ToString();
        OnGoldChanged?.Invoke(CurrentGold);
    }

    /// <summary>
    /// 减少金币，如果不够返回 false
    /// </summary>
    public bool TrySpendGold(int cost)
    {
        if (CurrentGold < cost)
            return false;

        CurrentGold -= cost;
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    /// <summary>
    /// 花费金币（如果你要强制修改）
    /// </summary>
    public void SetGold(int value)
    {
        CurrentGold = Mathf.Max(0, value);
        OnGoldChanged?.Invoke(CurrentGold);
    }
}
