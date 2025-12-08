using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透
/// </summary>
public class PierceBullet : Bullet
{
    private Vector3 _direction;//运动方向

    public void Init(int damage, bool isCritical, Vector3 startPos,Vector3 dir)
    {
        base.Init(damage, isCritical, startPos);
        _direction = dir.normalized;
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
    }
}
