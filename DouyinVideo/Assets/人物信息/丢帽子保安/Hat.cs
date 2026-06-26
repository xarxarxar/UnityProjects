using UnityEngine;
using UnityEngine.Events;

public class Hat : MonoBehaviour
{
    private float speed = 0;
    private Vector3 direction;
    public Rigidbody2D rb;
    private Transform hatVisual;
    private float zRotation = 0f;

    /// <summary>
    /// 碰到人物
    /// </summary>
    public UnityAction<BaseRole> OnHitRole;

    /// <summary>
    /// 碰到墙
    /// </summary>
    public UnityAction<GameObject> OnHitWall;

    private void Awake()
    {
        hatVisual = transform.GetChild(0);
    }

    public void Init(Vector3 dir,float spe)
    {
        direction = dir;
        speed = spe;
        rb.AddForce(direction, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (VideoGameManager.instance.gameEnd)
        {
            gameObject.SetActive(false);
        }

        zRotation += 180f * Time.deltaTime; // 每秒旋转10度
        hatVisual.eulerAngles = new Vector3(0, 0, zRotation);
    }

    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = rb.velocity.normalized;

            // 添加一个非常小的随机偏移，避免轨迹锁死
            Vector2 randomOffset = new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            dir += randomOffset;
            dir.Normalize();

            rb.velocity = dir * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Role"))
        {
            BaseRole hitRole= collision.gameObject.GetComponent<BaseRole>();
            OnHitRole?.Invoke(hitRole);
        }

        if (collision.gameObject.CompareTag("wall"))
        {
            OnHitWall?.Invoke(collision.gameObject);
        }
    }
}
