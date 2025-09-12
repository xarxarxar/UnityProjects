using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 子弹的攻击方式，跟随目标，还是穿透目标
/// </summary>
public enum BulletType
{
    FollowTarget,
    SinglePenetrate,
    MultiPenetrate
}
/// <summary>
/// 子弹的种类，普通弹还是干冰弹还是火焰弹还是电击弹
/// </summary>
public enum BulletKind
{
    Normal,
    Ice,
    Fire,
    Electric
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 50f;
    [SerializeField] private TrailRenderer _trailRenderer;

    private BulletType _bulletType;
    private BulletKind _bulletKind=>TowerManager.Instance.CurrentBulletKind;
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
    private List<Enemy> _hitEnemies = new List<Enemy>();//已经击中过的敌人

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
        _maxChainCount = 0;
        AudioManager.Instance.PlaySFX("开枪");

        if (_trailRenderer == null)
        {
            _trailRenderer=GetComponent<TrailRenderer>();
        }

        _trailRenderer.Clear();


        switch (_bulletKind)
        {
            case BulletKind.Normal:
                GetComponent<SpriteRenderer>().color=Color.white;
                _trailRenderer.startColor = Color.white;
                break;
            case BulletKind.Ice:
                GetComponent<SpriteRenderer>().color = new Color32(135, 206, 250, 255);
                _trailRenderer.startColor = new Color32(135, 206, 250, 255);
                break;
            case BulletKind.Fire:
                GetComponent<SpriteRenderer>().color = new Color32(255, 99, 71, 255);
                _trailRenderer.startColor = new Color32(255, 99, 71, 255);
                break;
            case BulletKind.Electric:
                GetComponent<SpriteRenderer>().color = new Color32(0, 255, 255, 255);
                _trailRenderer.startColor = new Color32(0, 255, 255, 255);
                break;
        }
    }

    // 穿透型子弹（单穿/多穿）
    public void Init(Vector3 position, Vector3 direction, bool isCritical, int damage, BulletType type)
    {
        transform.position = position;
        _direction = direction.normalized;
        _isCritical = isCritical;
        _damage = damage;
        _maxChainCount = 0;
        _bulletType = type;
        AudioManager.Instance.PlaySFX("开枪");


        if (_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }
        _trailRenderer.Clear();

        switch (_bulletKind)
        {
            case BulletKind.Normal:
                GetComponent<SpriteRenderer>().color = Color.white; break;
            case BulletKind.Ice:
                GetComponent<SpriteRenderer>().color = Color.blue; break;
            case BulletKind.Fire:
                GetComponent<SpriteRenderer>().color = Color.red; break;
            case BulletKind.Electric:
                GetComponent<SpriteRenderer>().color = Color.yellow; break;
        }
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

        if (_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }
        _trailRenderer.Clear();

        switch (_bulletKind)
        {
            case BulletKind.Normal:
                GetComponent<SpriteRenderer>().color = Color.white; break;
            case BulletKind.Ice:
                GetComponent<SpriteRenderer>().color = Color.blue; break;
            case BulletKind.Fire:
                GetComponent<SpriteRenderer>().color = Color.red; break;
            case BulletKind.Electric:
                GetComponent<SpriteRenderer>().color = Color.yellow; break;
        }
    }

    #endregion

    private void Update()
    {
        if (_trailRenderer != null)
        {
            if (BattleManager.Instance.GameSpeed.Value <= 0f)
            {
                // 暂停时不发射拖尾
                _trailRenderer.emitting = false;
            }
            else
            {
                // 恢复
                _trailRenderer.emitting = true;
                _trailRenderer.time = _baseTrailTime / BattleManager.Instance.GameSpeed.Value;
            }
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

        if ((_bulletType == BulletType.MultiPenetrate || _bulletType == BulletType.SinglePenetrate) && transform.position.y >= 15f)
        {
            ReturnToPool();
        }
    }

    private void ReachTarget()
    {
        if (_targetEnemy != null)
        {
            OnHitEnemy(_targetEnemy, _isCritical, _damage);
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
                OnHitEnemy(enemy, _isCritical, _damage);
                _canDamage = false;
                ReturnToPool();
                break;

            case BulletType.MultiPenetrate:
                if (!_damagedEnemies.Contains(enemy))
                {
                    _damagedEnemies.Add(enemy);
                    OnHitEnemy(enemy, _isCritical, _damage);
                }
                break;
        }
    }
    public void OnHitEnemy(Enemy enemy, bool isCritical, int damage)
    {
        if (_bulletKind != BulletKind.Normal)
        {
            Debug.Log($"enemy is{enemy}，enemy.gameObject.activeInHierarchy is{enemy.gameObject.activeInHierarchy}");
        }
        if (!enemy.gameObject.activeInHierarchy) return;
        // 1. 所有子弹都有的基础伤害
        enemy.TakeDamage(isCritical,damage);
        if (!enemy.gameObject.activeInHierarchy) return;
        // 2. 如果敌人有护盾，跳过特殊效果
        if (enemy.CurrentShield.Value>0) return;
        

        // 3. 特殊效果
        switch (_bulletKind)
        {
            case BulletKind.Ice:
                enemy.SetSpeed(0.5f, 3.0f);
                break;
            case BulletKind.Fire:
                enemy.SetBleed(Mathf.RoundToInt(damage/5.0f), 3.0f,0.5f);
                break;
            case BulletKind.Electric:
                enemy.SetSpeed(0,1.0f);
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
