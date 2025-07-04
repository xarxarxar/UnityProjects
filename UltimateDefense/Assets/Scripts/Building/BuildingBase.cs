using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 建筑类基类
/// </summary>
public abstract class BuildingBase : MonoBehaviour
{
    [Header("血量设置")]
    [SerializeField] private int maxHP = 100;              // 最大生命值
    [SerializeField] protected int _currentHP = 100;          // 当前生命值

    /// <summary>
    /// 建筑的唯一名称或类型，由子类提供
    /// </summary>
    public abstract string BuildingName { get; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHP { get => maxHP; set => maxHP = value; }

    /// <summary>
    /// 建筑被摧毁的实例事件，订阅者可接收到本实例
    /// </summary>
    public event UnityAction<BuildingBase> OnDestroyed;

    /// <summary>
    /// 初始化函数，由子类实现
    /// </summary>
    public abstract void Init(int maxHP, int recoverHpPerSecond=0);

    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        _currentHP -= damage;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
        }
    }
}
