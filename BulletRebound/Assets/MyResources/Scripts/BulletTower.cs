using System.Collections;
using System.IO.Pipes;
using UnityEngine;

public class BulletTower : MonoBehaviour
{
    [Header("旋转设置")]
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public float baseSpeed = 90f; // 每秒旋转度数

    private float _currentAngle;
    private float _targetAngle;
    private float _direction = 1f; // 旋转方向
    private bool _isFullRotation; // 是否全周旋转

    [Header("发射设置")]
    public GameObject bulletPrefab;// 子弹预制体

    float timer = 0f;//发射计时器
    float _fireInterval = 2.0f;//发射间隔

    void Start()
    {
        InitializeRotation();
    }

    void InitializeRotation()
    {
        minAngle = Mathf.Clamp(minAngle, -360f, 360f);
        maxAngle = Mathf.Clamp(maxAngle, -360f, 360f);

        _currentAngle = minAngle;
        _targetAngle = maxAngle;
        _isFullRotation = Mathf.Abs(maxAngle - minAngle) >= 360f;

        transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
    }

    // 每帧更新旋转角度
    void FixedUpdate()
    {
        RotateSelf();
        FireBullet();
    }

    // 旋转
    private void RotateSelf()
    {
        if (TimeManager.Instance.EffectiveTimeScale <= 0) return;

        // 计算实际速度（应用时间缩放）
        float effectiveSpeed = baseSpeed * TimeManager.Instance.EffectiveTimeScale;

        if (_isFullRotation)
        {
            // 全周旋转模式
            _currentAngle += effectiveSpeed * Time.fixedDeltaTime * _direction;
            _currentAngle = Mathf.Repeat(_currentAngle, 360f);
        }
        else
        {
            // 摆动模式
            _currentAngle = Mathf.MoveTowards(_currentAngle, _targetAngle,
                effectiveSpeed * Time.fixedDeltaTime);

            // 到达目标角度后反转方向
            if (Mathf.Approximately(_currentAngle, _targetAngle))
            {
                _direction *= -1;
                _targetAngle = _direction > 0 ? maxAngle : minAngle;
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
    }

    // 发射子弹
    private void FireBullet()
    {
        // 使用缩放后的增量时间
        timer += TimeManager.Instance.DeltaTime;
        if (timer < _fireInterval)
        {
            return;
        }
        timer = 0f;

        Bullet bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bullet.Init(transform.up);
    }

    

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
