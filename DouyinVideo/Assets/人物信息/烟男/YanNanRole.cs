using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YanNanRole : BaseRole
{
    public GameObject bigCircle;
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
        yield return new WaitForSeconds(1.0f);

        Broadcast.instance.BroadCastNews($"{RoleName}使用了大招", roleColor);
        bigCircle.transform.SetParent(null);
        bigCircle.transform.position = otherBaseRole.transform.position;
        bigCircle.SetActive(true);

        float duration = 5f;
        float interval = 0.4f;
        float timer = 0f;

        Collider2D bigCircleCollider = bigCircle.GetComponent<Collider2D>();
        Collider2D selfCollider = GetComponent<Collider2D>();
        Collider2D otherCollider = otherBaseRole.GetComponent<Collider2D>();
        yield return new WaitForSeconds(0.4f);

        while (timer < duration)
        {
            // 检查是否和other重叠
            if (bigCircleCollider.bounds.Intersects(otherCollider.bounds))
            {
                if (otherCollider.gameObject.layer != LayerMask.NameToLayer("YeLu")) 
                {
                    Broadcast.instance.BroadCastNews($"{RoleName}的大招对{otherBaseRole.RoleName}造成了10点伤害", roleColor);
                    otherCollider.GetComponent<BaseRole>().TakeDamage(10);
                } 
            }

            // 检查是否和自己重叠
            if (bigCircleCollider.bounds.Intersects(selfCollider.bounds))
            {
                Broadcast.instance.BroadCastNews($"{RoleName}的大招对自己造成了5点伤害", roleColor);
                TakeDamage(5);
            }

            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        bigCircle.SetActive(false);
        bigCircle.transform.SetParent(transform);
        yield break;
    }
}
