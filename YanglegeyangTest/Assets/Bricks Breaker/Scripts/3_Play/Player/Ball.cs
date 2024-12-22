using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    // 小球的刚体，用于物理计算
    public Rigidbody2D rb;

    // 标记小球是否为第一次进入区域
    [HideInInspector] public bool isFirst = false;

    // 小球的速度
    private float speed = 1f;

    // 是否已重置过
    private bool isReset = false;

    // 小球的伤害值
    public int damage = 1;

    // 小球的Sprite渲染器，用于显示小球的图像
    public SpriteRenderer spriteBall;

    // 设置小球的伤害值和初始状态
    public void SetData(int damage)
    {
        // 设置伤害值
        this.damage = damage;

        // 重置小球状态
        isReset = false;
        _isDestoryOn = false;

        // 向小球施加一个相对的力，使其向前发射
        rb.AddRelativeForce(Player.instance.shotRot.transform.up.normalized * speed, ForceMode2D.Impulse);

        // 启用小球的碰撞体
        GetComponent<CircleCollider2D>().enabled = true;
    }

    // 标记小球是否已经准备销毁
    bool _isDestoryOn = false;

    // 小球与其他物体发生碰撞时的处理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 播放射击音效
        CtrGame.instance.ShotSound();
        // 启动销毁标记
        _isDestoryOn = true;
    }

    // 小球进入触发器时的处理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDestoryOn)
        {
            // 如果小球还没有被销毁，且碰撞到了标记为 "InTrigger" 的触发器
            if (collision.CompareTag("InTrigger"))
            {
                // 如果这是第一次进入触发器区域
                if (!Player.instance.isFirst)
                {
                    // 标记为第一次
                    Player.instance.isFirst = true;
                    // 设置下一个发射位置
                    Player.instance.SetNextPositionX(transform.position.x);
                    // 重置小球
                    Reset();
                }
                else
                {
                    // 如果不是第一次，则继续移动小球
                    MoveBall();
                }
            }
        }
    }

    // 小球在触发器区域停留时的处理
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isDestoryOn)
        {
            if (collision.CompareTag("InTrigger"))
            {
                // 同上，判断是否第一次进入区域
                if (!Player.instance.isFirst)
                {
                    Player.instance.isFirst = true;
                    Player.instance.SetNextPositionX(transform.position.x);
                    Reset();
                }
                else
                {
                    MoveBall();
                }
            }
        }
    }

    // 小球移动到指定位置（用于回收等操作）
    public void MoveBall()
    {
        // 禁用碰撞体，防止再次发生碰撞
        GetComponent<CircleCollider2D>().enabled = false;

        // 清除当前小球的速度
        rb.velocity = Vector3.zero;

        // 停止当前的所有动画
        transform.DOKill();

        // 使用DOTween将小球平滑地移动到指定位置
        transform.DOMove(Player.instance.nextPosition, 0.15f).SetEase(Ease.OutCubic).OnComplete(() => { Reset(); });
    }

    // 回收小球的方法
    public void ReturnBall()
    {
        // 停止当前的小球运动
        rb.velocity = Vector3.zero;

        // 禁用碰撞体
        GetComponent<CircleCollider2D>().enabled = false;

        // 使用DOTween将小球平滑地移动回指定位置
        transform.DOMove(Player.instance.nextPosition, 0.25f).SetEase(Ease.OutCubic).OnComplete(() => { Reset(); });
    }

    // 重置小球状态
    private void Reset()
    {
        // 如果已经重置过，就不再重复重置
        if (!isReset)
        {
            // 标记为已重置
            isReset = true;

            // 重置第一次进入标记
            isFirst = false;

            // 取消销毁标记
            _isDestoryOn = false;

            // 从玩家的活跃小球列表中移除该小球
            Player.instance.activeBall.Remove(this);

            // 将小球返回对象池进行复用
            PoolManager.Despawn(this.gameObject);
        }
    }
}
