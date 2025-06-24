using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示一座塔的行为，如攻击、范围、升级等。
/// </summary>
public class Tower : MonoBehaviour
{
    #region 配置参数
    [Header("基本属性")]
    public float attackRange = 5f;         // 攻击范围
    public float baseFireRate = 1f;        // 攻击频率（每秒几次）
    public float criticalProb = 0.1f;       //初始暴击概率
    public int baseDamage = 10;            // 初始单次攻击伤害
    
    

    [Header("升级属性")]
    public int level = 1;                  // 当前等级
    public int upgradeCost = 50;           // 升级花费金币

    //总属性
    public int totalDamage => Mathf.RoundToInt(baseDamage * (1 + TowerManager.Instance.GlobalAttackBonus));
    public float totalAttackRate => baseFireRate*(1+TowerManager.Instance.GlobalAttackSpeedMultiplier);
    public float totalCriticalProb => criticalProb + TowerManager.Instance.GlobalCriticalShotProb;
    #endregion

    #region 私有字段
    private float reloadTime = 0f;//装弹时间
    private Enemy currentTarget;//当前的攻击目标
    private bool _isPaused=>BattleManager.Instance.IsPaused;//是否暂停
    private List<Enemy> _enemiesInRange=>EnemyManager.Instance.EnemiesInRange;//在攻击范围内的所有敌人
    private float _gameSpeed=>BattleManager.Instance.GameSpeed;
    [SerializeField]private Bullet _bullet;//子弹
    #endregion

    #region Unity 生命周期
    private void OnEnable()
    {
        StartCoroutine(AttackIE());//开始攻击
    }

    private void Update()
    {
        
    }
    #endregion

    #region 公共方法

    /// <summary>
    /// 升级塔：消耗金币，增加伤害与攻速。这里逻辑不对，应该是三个合成进行升级
    /// </summary>
    public void Upgrade()
    {
        if (CurrencyManager.Instance.SpendCoin(upgradeCost))
        {
            level++;
            baseDamage = 20;
            baseFireRate = 0.5f;
            upgradeCost += 50;
        }
    }
    /// <summary>
    /// 摧毁该防御塔
    /// </summary>
    public void Destroy()
    {

    }

    #endregion

    #region 私有方法

    private bool IsTargetInRange(Enemy target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    private void Attack(Enemy target)
    {
        if (target != null)
        {
            Bullet bullet= TowerManager.Instance.BulletPool.Get();//从对象池拿取

            if (Random.value<totalCriticalProb)//暴击
            {
                bullet.Init(transform.position, target,true, Mathf.RoundToInt(totalDamage*TowerManager.Instance.GlobalCriticalMultiplier));
            }
            else
            {
                bullet.Init(transform.position, target, false, totalDamage);
            }
            
        }
    }

    private IEnumerator AttackIE()
    {
        yield return null;
        while (true)
        {
            if (_isPaused)
                yield return null;

            if(_enemiesInRange.Count!=0)
            {
                currentTarget = _enemiesInRange[0];
                Attack(currentTarget);
            }
            yield return new WaitForSeconds((1/ totalAttackRate) / _gameSpeed);
        }
    }

    #endregion
}
