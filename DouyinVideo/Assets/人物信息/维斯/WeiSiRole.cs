using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiSiRole : BaseRole
{
    public GameObject Jingji;//荆棘的范围
    private Coroutine bigCoro = null;

    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(0.1f);
        //
        Jingji.SetActive(true);
        Jingji.transform.SetParent(null);
        yield return new WaitForSeconds(3);
        // 获取圆形范围参数
        Vector2 center = Jingji.transform.position;
        float radius = Jingji.GetComponent<CircleCollider2D>().radius * Jingji.transform.lossyScale.x;

        // 检测该范围内所有碰撞体
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D col in hits)
        {
            Debug.Log("在范围内的物体：" + col.name);
            // 你可以根据tag或者组件筛选目标，比如
            if (col.CompareTag("Role") && col.gameObject!=gameObject)
            {
                col.GetComponent<BaseRole>().SetCanGetWeapon(false,10);
                Broadcast.instance.BroadCastNews($"{RoleName}缴械了{col.GetComponent<BaseRole>().RoleName},无法拾取武器", roleColor);
            }
        }
        SetSpeed(1.2f,5);
        Jingji.SetActive(false);
        Jingji.transform.SetParent(transform);
        Jingji.transform.localPosition = Vector3.zero;
        yield break;
    }
}
