using UnityEngine;
using System.Collections.Generic;

public enum BulletType
{
    FollowTarget,
    SinglePenetrate,
    MultiPenetrate
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 50f;
    [SerializeField] private TrailRenderer _trailRenderer;

    private BulletType _bulletType;
    private Enemy _targetEnemy;
    private bool _isCritical;
    private int _damage;
    private Vector3 _direction;
    private float _baseTrailTime = 0.2f;
    private bool _canDamage = true;

    private HashSet<Enemy> _damagedEnemies = new HashSet<Enemy>();

    // ====== 弹射子弹专用字段 ======
    private int _maxChainCount = 0;
    private int _currentChain = 0;
    private float _decayPercent = 0f;
    private float _baseDamage;
    private List<Enemy> _hitEnemies = new List<Enemy>();

    private void Start()
    {
        if (_trailRenderer != null)
            _baseTrailTime = _trailRenderer.time;

        BattleManager.OnEndBattle += OnEndBattle;
    }

    private void OnEnable()
    {
        _canDamage = true;
        _damagedEnemies.Clear();
        _hitEnemies.Clear();
        _currentChain = 0;
    }

    #region 初始化方法

    // 跟踪型子弹
    public void Init(Vector3 position, Enemy enemy, bool isCritical, int damage)
    {
        transform.position = position;
        _bulletType = BulletType.FollowTarget;
        _targetEnemy = enemy;
        _isCritical = isCritical;
        _damage = damage;
        AudioManager.Instance.PlaySFX("开枪");
    }

    // 穿透型子弹（单穿/多穿）
    public void Init(Vector3 position, Vector3 direction, bool isCritical, int damage, BulletType type)
    {
        transform.position = position;
        _direction = direction.normalized;
        _isCritical = isCritical;
        _damage = damage;
        _bulletType = type;
        AudioManager.Instance.PlaySFX("开枪");
    }

    // 弹射子弹（跟踪型 + 弹射次数 + 衰减百分比）
    public void InitChainBullet(Vector3 position, Enemy enemy, bool isCritical, int damage, int maxChain, float decayPercent)
    {
        transform.position = position;
        _bulletType = BulletType.FollowTarget;
        _targetEnemy = enemy;
        _isCritical = isCritical;
        _baseDamage = damage;
        _damage = damage;
        _maxChainCount = maxChain;
        _decayPercent = Mathf.Clamp01(decayPercent);
        _currentChain = 0;
        _hitEnemies.Clear();

        AudioManager.Instance.PlaySFX("开枪");
    }

    #endregion

    private void Update()
    {
        if (BattleManager.Instance.IsPaused.Value)
        {
            if (_trailRenderer != null) _trailRenderer.emitting = false;
            return;
        }

        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = true;
            _trailRenderer.time = _baseTrailTime / BattleManager.Instance.GameSpeed.Value;
        }

        switch (_bulletType)
        {
            case BulletType.FollowTarget:
                MoveToTarget();
                break;
            case BulletType.SinglePenetrate:
            case BulletType.MultiPenetrate:
                MoveForward();
                break;
        }
    }

    private void MoveToTarget()
    {
        if (_targetEnemy == null)
        {
            ReturnToPool();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetEnemy.transform.position,
            _moveSpeed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _targetEnemy.transform.position) < 0.1f)
        {
            ReachTarget();
        }
    }

    private void MoveForward()
    {
        transform.position += _direction * _moveSpeed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime;

        if (_bulletType == BulletType.MultiPenetrate && transform.position.y >= 15f)
        {
            ReturnToPool();
        }
    }

    private void ReachTarget()
    {
        if (_targetEnemy != null)
        {
            _targetEnemy.TakeDamage(_isCritical, _damage);
            _hitEnemies.Add(_targetEnemy);
        }

        if (_currentChain < _maxChainCount)
        {
            Enemy next = FindNextEnemy(_targetEnemy);
            if (next != null)
            {
                _currentChain++;

                // 衰减伤害：damage = baseDamage * (1 - decay)^currentChain
                float factor = 1f - (_decayPercent * _currentChain);
                _damage = Mathf.Max(1, Mathf.RoundToInt(_baseDamage * factor)); // 最低伤害为1

                _targetEnemy = next;
                return; // 不回收，继续追踪下一个
            }
        }

        ReturnToPool();
    }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canDamage || !other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        switch (_bulletType)
        {
            case BulletType.SinglePenetrate:
                enemy.TakeDamage(_isCritical, _damage);
                _canDamage = false;
                ReturnToPool();
                break;

            case BulletType.MultiPenetrate:
                if (!_damagedEnemies.Contains(enemy))
                {
                    _damagedEnemies.Add(enemy);
                    enemy.TakeDamage(_isCritical, _damage);
                }
                break;
        }
    }

    private void ReturnToPool()
    {
        TowerManager.Instance.BulletPool.Return(this);
    }

    private void OnEndBattle(bool success)
    {
        if (gameObject.activeSelf)
        {
            ReturnToPool();
        }
        
    }
}
