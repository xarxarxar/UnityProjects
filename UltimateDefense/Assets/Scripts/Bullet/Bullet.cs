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
    public virtual float Speed => 10f;
    public DamageEffect DamageEffect=>TowerPlatformDataManager.Instance.GetCurrentTowerPlatformData().damageEffect;
    public HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();//击中过的敌人
    [SerializeField] public TrailRenderer _trailRenderer;//拖尾
    private float _baseTrailTime = 0.2f;

    [SerializeField] private float _lifeTime = 5f; // 子弹最多存在 5 秒
    private float _lifeTimer;

    // 初始化
    public virtual void Init(int damage, bool isCritical,Vector3 startPos)
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
        transform.position = startPos;
    }

    private void Update()
    {
        UpdateLifeTime();
        UpdateTrail();
        OnUpdate();
    }

    private void UpdateLifeTime()
    {
        // 更新生命周期
        _lifeTimer -= Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
            return;
        }
    }

    private void UpdateTrail()
    {
        //更新拖尾
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

    /// <summary>
    /// 子类在这里使用update
    /// </summary>
    protected virtual void OnUpdate() { }

    public void ReturnToPool()
    {
        TowerManager.Instance.ReturnBullet(this);
    }

}
