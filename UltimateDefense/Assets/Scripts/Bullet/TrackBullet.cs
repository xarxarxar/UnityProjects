using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 追踪弹
/// </summary>
public class TrackBullet : Bullet
{
    public Transform Target;//目标

    public void Init(int damage, bool isCritical,Transform target)
    {
        base.Init(damage, isCritical);
        Target=target;
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget() 
    { 
        if (Target == null)
        { 
            ReturnToPool();
            return; 
        } 
        transform.position = Vector3.MoveTowards( transform.position, Target.transform.position, 
            _speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime );
        if (Vector3.Distance(transform.position, Target.transform.position) < 0.1f) 
        { 
            ReachTarget(); 
        } 
    }

    private void ReachTarget()
    {
        if (Target != null && Target.GetComponent<Enemy>()!=null) 
        { 
            Enemy enemy= Target.GetComponent<Enemy>();
            OnHitEnemy(enemy, IsCritical, Damage);
            _hitEnemies.Add(enemy); 
        }
        ReturnToPool(); 
    }

    public void OnHitEnemy(Enemy enemy, bool isCritical, int damage)
    {
        if (!enemy.gameObject.activeInHierarchy) return; 
        // 1. 所有子弹都有的基础伤害
        enemy.TakeDamage(isCritical,damage);
        if (!enemy.gameObject.activeInHierarchy) return;
        // 2. 如果敌人有护盾，跳过特殊效果
        if (enemy.CurrentShield.Value>0) return; 
    }

}
