using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透
/// </summary>
public class PierceBullet : Bullet
{
    private Vector3 _direction;//运动方向
    private bool isPierce = true;//是否穿透，为false的话即打到敌人即消失

    public void Init(int damage, bool isCritical, Vector3 startPos,Vector3 dir,bool isPie=true)
    {
        gameObject.SetActive(false);
        base.Init(damage, isCritical, startPos);
        OnGetFromPool();
        gameObject.SetActive(true);
        _direction = dir.normalized;
        isPierce= isPie;

        SetRotationByDirection(_direction);
    }

    protected override void OnUpdate()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        transform.position += _direction * Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        Enemy enemy = collision.GetComponent<Enemy>(); if (enemy == null) return;

        if (_hitEnemies.Contains(enemy)) return;
        enemy.TakeDamage(IsCritical, Damage);
        if(!isPierce)
        {
            ReturnToPool();
        }
    }

    private void SetRotationByDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
