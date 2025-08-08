using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 负责管理，点击造成的伤害
/// </summary>
public class PlayerClickDamageManager : ManagerBase<PlayerClickDamageManager>,IManager
{
    private int _damage=10;//点击造成的伤害
    private float _criticalProb = 0.3f;//点击造成伤害暴击的概率
    private float _clickInterval=0.3f;//点击造成伤害的隐藏CD时长
    private float _lastClickTime = -999f; // 初始CD为很早之前

    /// <summary>
    /// 玩家点击敌人造成的伤害
    /// </summary>
    public int Damage { get => _damage; set => _damage = value; }
    /// <summary>
    /// 点击造成伤害暴击的概率
    /// </summary>
    public float CriticalProb { get => _criticalProb; set => _criticalProb = value; }
    /// <summary>
    /// 冷却时间
    /// </summary>
    private float ClickInterval => _clickInterval / BattleManager.Instance.GameSpeed.Value;


    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyClicked -= OnEnemyClicked;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Enemy.OnEnemyClicked += OnEnemyClicked;
    }

    //敌人被点击事件
    private void OnEnemyClicked(Enemy enemy)
    {
        if (Time.time - _lastClickTime < ClickInterval) return;//冷却时间没到
        _lastClickTime = Time.time;//刷新上次点击时间

        if (enemy != null)
        {
            if (Random.value < _criticalProb)//暴击
            {
                enemy.TakeDamage(true, Mathf.RoundToInt(_damage * TowerManager.Instance.BonusCritMult.Value));
            }
            else
            {
                enemy.TakeDamage(false, _damage);
            }

        }
    }
}
