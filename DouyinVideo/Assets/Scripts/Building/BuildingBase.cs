using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 建筑类基类
/// </summary>
public abstract class BuildingBase : MonoBehaviour
{
    [Header("血量设置")]
    [SerializeField] protected int _currentHP = 100;          // 当前生命值

    /// <summary>
    /// 建筑的图标
    /// </summary>
    public Sprite icon;


    /// <summary>
    /// 建筑的唯一名称或类型，由子类提供
    /// </summary>
    [field: SerializeField]
    public string BuildingName { get; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    [field: SerializeField]
    public int MaxHP { get; set; }

    /// <summary>
    /// 建筑被摧毁的实例事件，订阅者可接收到本实例
    /// </summary>
    public event UnityAction<BuildingBase> OnDestroyed;

    /// <summary>
    /// 初始化函数，由子类实现
    /// </summary>
    /// <param name="maxHP">最大生命值</param>
    public virtual void Init(int maxHP)
    {
        _currentHP = MaxHP = maxHP;
    }

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
            Die();
        }
    }

    //建筑物死亡
    public virtual void Die()
    {
        Destroy(gameObject);
        OnDestroyed?.Invoke(this);
    }
}
