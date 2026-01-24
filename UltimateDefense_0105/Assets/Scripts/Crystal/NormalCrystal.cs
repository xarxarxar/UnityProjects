using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 普通城墙
/// </summary>
public class NormalCrystal : MonoBehaviour,IEnemyAttack
{
    private List<Enemy> enemies=new List<Enemy>();//被这堵城墙挡住的Enemy
    private int maxHP = 200;
    private int currentHP = 200;
    [SerializeField] private GameObject WallImage;//墙的2D图片
    // Q 弹相关
    private float stretchAmount = 1.2f; // 拉伸比例
    private float squashDuration = 0.15f; // Q 弹单程时长
    private Vector3 originalScale;

    private Collider2D _collider;
    /// <summary>
    /// 墙被摧毁
    /// </summary>
    public event UnityAction OnWallDestroyed;


    public void Init(int max)
    {
        if(_collider == null)
        {
            _collider = GetComponent<Collider2D>();
        }
        _collider.enabled = false; //关键：先关碰撞
        transform.parent.gameObject.SetActive(true);
        maxHP= max;
        currentHP = maxHP;
        originalScale = new Vector3(1.0f,0.6f,1.0f);
        enemies.Clear();
        Enemy.OnEnemyDie -= OnEnemyDie;
        Enemy.OnEnemyDie += OnEnemyDie;
        transform.parent.gameObject.SetActive(true);


        RiseWall();
    }
    /// <summary>
    /// 城墙结束
    /// </summary>
    public void End()
    {
        Destroyed();
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void ResetWall()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].OnLeaveWall();
        }
        enemies.Clear();
        Enemy.OnEnemyDie -= OnEnemyDie;
        transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 承受伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        //Debug.Log("遭到攻击");
        if (currentHP <= 0)
        {
            return;
        }
        JellySquash();
        AudioManager.Instance.Vibrate("medium");//重震动
        currentHP = Mathf.Max(currentHP - damage, 0);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Destroyed();
        }
    }
    /// <summary>
    /// 城墙被摧毁
    /// </summary>
    private void Destroyed()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].OnLeaveWall();
        }
        enemies.Clear();
        Enemy.OnEnemyDie -= OnEnemyDie;
        OnWallDestroyed?.Invoke();
        transform.parent.gameObject.SetActive(false);
    }

    //敌人死亡了则移除
    private void OnEnemyDie(Enemy enemy)
    {
        if(enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("进入城墙");
        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
            enemy.OnReachWall(this);
        }
    }

    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash()
    {
        if (WallImage == null) return;

        Transform t = WallImage.transform;

        t.DOKill();
        t.localScale = originalScale;

        Sequence seq = DOTween.Sequence();
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        void OnSpeedChanged(int speed)
        {
            if (seq != null) seq.timeScale = speed;
        }

        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        seq.Append(
            t.DOScaleY(originalScale.y * stretchAmount, squashDuration)
             .SetEase(Ease.OutQuad)
        );

        seq.Append(
            t.DOScaleY(originalScale.y, squashDuration)
             .SetEase(Ease.OutBounce)
        );

        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
    private Tween _riseTween;
    private void RiseWall()
    {
        if (WallImage == null) return;

        // 1停止之前的动画
        _riseTween?.Kill();
        _riseTween = null;

        // 2复位到起始位置
        Transform t = WallImage.transform;
        t.localPosition = new Vector3(0f, -1.2f, 0f);

        // 3创建升起动画
        _riseTween = t.DOLocalMoveY(0f, 2.0f)
            .SetEase(Ease.OutBack)
            .SetUpdate(false);

        // 4初始速度
        _riseTween.timeScale = BattleManager.Instance.GameSpeed.Value;

        // 5动态绑定 GameSpeed
        void OnSpeedChanged(int speed)
        {
            if (_riseTween != null)
                _riseTween.timeScale = speed;
        }

        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        // 6动画结束时解绑（非常重要）
        _riseTween.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
            _collider.enabled = true; //动画结束，允许触发
        });

        _riseTween.OnKill(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
}

