using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 追踪弹
/// </summary>
public class TrackBullet : Bullet
{
    [HideInInspector]public Enemy Target;//目标
    private int _targetID;//敌人的ID

    public void Init(int damage, bool isCritical,Vector3 startPos,Enemy target)
    {
        gameObject.SetActive(false);
        base.Init(damage, isCritical,startPos);
        OnGetFromPool();
        gameObject.SetActive(true);
        Target=target;
        _targetID = Target.UniqueID;
    }

    protected override void OnUpdate()
    {
        MoveToTarget();
    }


    private void MoveToTarget()
    {
        if (Target == null || Target.UniqueID != _targetID)
        {
            ReturnToPool();
            return;
        }

        Vector3 targetPos = Target.transform.position;
        Vector3 dir = targetPos - transform.position;

        // 移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime
        );

        // 朝向运动方向（核心）
        if (dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 到达目标
        if (dir.sqrMagnitude < 0.01f)
        {
            ReachTarget();
        }
    }

    private void ReachTarget()
    {
        if (Target != null) 
        { 
            Target.TakeDamage(IsCritical, Damage);
            _hitEnemies.Add(Target); 
        }
        ReturnToPool(); 
    }

}
