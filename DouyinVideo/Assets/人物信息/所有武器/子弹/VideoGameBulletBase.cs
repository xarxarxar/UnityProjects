using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class VideoGameBulletBase : MonoBehaviour
{
    private Vector3 direction;
    private float speed = 50f;
    private BaseRole _baseRole;
    private string[] tags;

    /// <summary>
    /// 子弹命中角色后
    /// </summary>
    public UnityAction<BaseRole> OnBulletHitRole;

    /// <summary>
    /// 子弹命中墙后
    /// </summary>
    public UnityAction OnBulletHitWall;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="angle"></param>
    /// <param name="speed"></param>
    /// <param name="bulleetDamage"></param>
    /// <param name="startPos"></param>
    /// <param name="baseRole"></param>
    /// <param name="name"></param>
    /// <param name="tags">需要避开的tag</param>
    public void Init(Vector3 dir,float angle,int speed,Vector3 startPos,
        BaseRole baseRole,string[] tags=null)
    {
        direction = dir.normalized;
        this.speed = speed;
        transform.parent.position = startPos;
        _baseRole = baseRole;
        this.tags = tags;

        transform.parent.rotation = Quaternion.Euler(0, 0, angle);
        transform.parent.gameObject.SetActive(true);
        // 设置父物体的速度
        Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * this.speed;
        }
        else
        {
            Debug.LogWarning("父物体没有 Rigidbody2D 组件！");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果该物体的 tag 在 tags 数组中，则忽略
        if (tags != null && tags.Contains(collision.tag))
        {
            return;
        }

        if (collision.CompareTag("Role") && collision.gameObject!= _baseRole.gameObject)
        {
            var role = collision.GetComponent<BaseRole>();
            if (role != null)
            {
                OnBulletHitRole?.Invoke(role);
                //Recycle();
            }
        }
        else if (collision.CompareTag("wall"))
        {
            OnBulletHitWall?.Invoke();
            Recycle();
        }
    }

    /// <summary>
    /// 回收子弹
    /// </summary>
    public void Recycle()
    {
        StopAllCoroutines();
        transform.parent.gameObject.SetActive(false);
    }
}
