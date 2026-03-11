using UnityEngine;
using System.Collections.Generic;



/// <summary>
/// 这是子弹的基类
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{

    public int Damage { get; private set; }
    public bool IsCritical { get; private set; }
    public readonly float _speed = 10.0f;
    public DamageEffect DamageEffect=>TowerPlatformDataManager.Instance.GetCurrentTowerPlatformData().damageEffect;
    public HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();//击中过的敌人
    [SerializeField] public TrailRenderer _trailRenderer;//拖尾
    private float _baseTrailTime = 0.2f;

    [SerializeField] private float _lifeTime = 5f; // 子弹最多存在 5 秒
    private float _lifeTimer;

    // 初始化
    public virtual void Init(int damage, bool isCritical)
    {
        if (_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }
        _baseTrailTime = _trailRenderer.time;
        Damage = damage;
        IsCritical = isCritical;
        _hitEnemies.Clear();
        _lifeTimer = _lifeTime;
    }

    private void Update()
    {
        // 更新拖尾等其他逻辑...

        _lifeTimer -= Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
            return;
        }

        if (_trailRenderer != null)
        {
            if (BattleManager.Instance.GameSpeed.Value <= 0f)
            { // 暂停时不发射拖尾
                _trailRenderer.emitting = false;
            }
            else
            {
                // 恢复
                _trailRenderer.emitting = true;
                _trailRenderer.time = _baseTrailTime / BattleManager.Instance.GameSpeed.Value;
            }
        }

    }

    // ============ 子弹不自行移动，Tower 会调用这个方法 ============
    public void Move(Vector3 delta)
    {
        transform.position += delta;
    }

    // ============ 子弹命中逻辑 ============
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        // 防止多次命中（如果允许穿透，Tower 决定是否清空 _hitEnemies）
        if (_hitEnemies.Contains(enemy)) return;
        _hitEnemies.Add(enemy);

        OnHit(enemy);
    }

    public void OnHit(Enemy enemy)
    {
        if (enemy == null) return;

        // 1. 基础伤害
        enemy.TakeDamage(IsCritical, Damage);

        // 2. 特效
        if (DamageEffect != null)
            DamageEffect.ApplyEffect(enemy, Damage);
    }

    public void ReturnToPool()
    {
        TowerManager.Instance.BulletPool.Return(this);
    }

}
