using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("子弹设置")]
    [SerializeField] public float _baseSpeed = 10f;  // 基础速度（单位/秒）
    [SerializeField] private float _maxLifetime = 20f; // 最大存活时间（秒）

    private Rigidbody2D _rb;
    public Vector2 _initialDirection; // 初始方向
    private float _spawnTime;
    private float _originalSpeed;

    public void Init( Vector2 _initialDirection)
    {
        this._initialDirection = _initialDirection;
        _rb = GetComponent<Rigidbody2D>();
        _originalSpeed = _baseSpeed;

        _spawnTime = TimeManager.Instance.EffectiveTime;
        ApplyInitialForce();
    }

    /// <summary>
    /// 初始化子弹方向
    /// </summary>
    public void Initialize(Vector2 direction)
    {
        _initialDirection = direction.normalized;
    }

    /// <summary>
    /// 应用初始速度（在Start中调用）
    /// </summary>
    private void ApplyInitialForce()
    {
        float effectiveSpeed = _originalSpeed * TimeManager.Instance.EffectiveTimeScale;
        _rb.velocity = _initialDirection * effectiveSpeed;
    }

    void FixedUpdate()
    {
        UpdateBulletMovement();
        CheckLifetime();
    }

    // 更新子弹运动（在FixedUpdate中调用）
    void UpdateBulletMovement()
    {
        if (TimeManager.Instance.EffectiveTimeScale <= 0)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        // 获取当前运动方向（保留物理碰撞后的方向）
        Vector2 currentDirection = _rb.velocity.normalized;

        // 如果速度为零（如刚生成时），使用初始方向
        if (currentDirection == Vector2.zero)
        {
            currentDirection = _initialDirection;
        }

        // 计算有效速度并更新
        float effectiveSpeed = _originalSpeed * TimeManager.Instance.EffectiveTimeScale;
        _rb.velocity = currentDirection * effectiveSpeed;
    }

    // 检查子弹存活时间（在FixedUpdate中调用）
    void CheckLifetime()
    {
        if (TimeManager.Instance.EffectiveTime - _spawnTime >= _maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 示例：碰到非玩家物体销毁
        if (!collision.gameObject.CompareTag("Player"))
        {
            //Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // 可在此处添加销毁特效
    }
}
