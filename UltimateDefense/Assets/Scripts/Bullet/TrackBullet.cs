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
        base.Init(damage, isCritical,startPos);
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

        //if (!Target.gameObject.activeInHierarchy)
        //{
        //    ReturnToPool();
        //    return;
        //}
        transform.position = Vector3.MoveTowards( transform.position, Target.transform.position, 
            Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime );
        if (Vector3.Distance(transform.position, Target.transform.position) < 0.1f) 
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
