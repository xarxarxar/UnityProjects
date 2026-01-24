using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹跳
/// </summary>
public class BounceBullet : Bullet
{
    private readonly int bounceCount=3;//一共攻击几次敌人，包括第一次
    private Enemy targetEnemy;
    private int _targetID;
    private int currentBounceCount = 0;

    public void Init(int damage, bool isCritical, Vector3 startPos,Enemy target)
    {
        gameObject.SetActive(false);
        base.Init(damage, isCritical, startPos);
        OnGetFromPool();
        gameObject.SetActive(true);
        targetEnemy = target;
        _targetID = target.UniqueID;
        currentBounceCount = 0;
    }

    protected override void OnUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (targetEnemy == null || targetEnemy.UniqueID != _targetID)
        {
            ReturnToPool();
            return;
        }
        Vector3 targetPos = targetEnemy.transform.position;
        Vector3 dir = targetPos - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPos,
            Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime);

        
        // 朝向移动方向（核心）
        if (dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (dir.magnitude < 0.1f)
        {
            ReachTarget();
        }
    }
    private void ReachTarget()
    {
        currentBounceCount++;
        Debug.Log($"currentBounceCount增加，当前为{currentBounceCount}");
        if (targetEnemy != null && targetEnemy.UniqueID == _targetID)
        {
            targetEnemy.TakeDamage(IsCritical, Damage);
            _hitEnemies.Add(targetEnemy);
        }

        if(currentBounceCount>= bounceCount)
        {
            ReturnToPool();
        }
        else
        {
            targetEnemy = FindNextEnemy(targetEnemy);
            if (targetEnemy == null) return;
            _targetID = targetEnemy.UniqueID;
        }

    }

    /// <summary>
    /// 找到下一个敌人（离当前敌人最近的敌人）
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    private Enemy FindNextEnemy(Enemy current)
    { 
        IReadOnlyList<Enemy> candidates = EnemyManager.Instance.AllEnemies;
        Enemy closest = null;
        float minDist = float.MaxValue;
        Vector3 from = current.transform.position;
        foreach (var enemy in candidates)
        { 
            if (enemy == null || _hitEnemies.Contains(enemy)) continue;
            float dist = Vector3.Distance(from, enemy.transform.position);
            if (dist < minDist)
            { 
                minDist = dist; 
                closest = enemy;
            }
        }
        return closest;
    }
}
