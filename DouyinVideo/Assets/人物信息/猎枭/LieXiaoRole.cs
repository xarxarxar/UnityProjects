using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// 猎枭脚本
/// </summary>
public class LieXiaoRole : BaseRole
{
    public LineRenderer lineRenderer;
    private Coroutine bigCoro = null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(ShootAndExtendLaser());
    }

    private IEnumerator ShootAndExtendLaser()
    {
        yield return new WaitForSeconds(0.5f);
        Transform target = otherBaseRole.transform;

        Vector2 startPos = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;

        float maxDistance = 50f;
        Vector2 endPos = startPos + direction * maxDistance;

        Debug.Log($"start is {startPos}, end is {endPos}");

        // 激光初始化
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos); // 初始长度为0

        float elapsed = 0f;
        float duration = 0.5f;

        bool hitSomething = false;

        BaseRole hitRole = null;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector2 currentEnd = Vector2.Lerp(startPos, endPos, t);

            // LayerMask 排除自己
            int ignoreLayer = LayerMask.GetMask("YeLu");
            int layerMask = ~ignoreLayer;

            // 若还未命中，则尝试检测命中
            if (!hitSomething)
            {
                RaycastHit2D hit = Physics2D.Raycast(startPos, direction, Vector2.Distance(startPos, currentEnd), layerMask);
                if (hit.collider != null && hit.collider.CompareTag("Role") && hit.collider.gameObject != gameObject)
                {
                    hitRole = hit.collider.GetComponent<BaseRole>();
                    hitSomething = true;

                    // 立即造成一次伤害
                    hitRole?.TakeDamage(50);
                    Broadcast.instance.BroadCastNews($"{RoleName}使用大招对{hitRole.RoleName}造成了50点伤害", roleColor);
                }
            }

            // 激光末端继续延伸
            lineRenderer.SetPosition(1, currentEnd);
            yield return null;
        }

        // 最终固定在 endPos
        lineRenderer.SetPosition(1, endPos);

        yield return new WaitForSeconds(1f);
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            UseBig();
        }
    }

}
