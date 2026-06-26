using System.Collections;
using UnityEngine;

public class NiHongRole : BaseRole
{
    public LineRenderer lineRenderer;

    private Coroutine bigCoro = null;
    private Coroutine updateLaserCoro = null;
    public override void Big()
    {
        
        lineRenderer.enabled = false;
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
        Broadcast.instance.BroadCastNews($"{RoleName}使用大招", roleColor);
        StartLaser();
        yield break;
    }

    void StartLaser()
    {
        lineRenderer.enabled = true;

        if (updateLaserCoro != null)
        {
            StopCoroutine(updateLaserCoro);
            updateLaserCoro = null;
        }

        updateLaserCoro = StartCoroutine(UpdateLaser());
    }

    IEnumerator UpdateLaser()
    {
        float noiseSpeed = 5.0f;       // 噪声变化速度
        float noiseAmplitude = 10f;     // 抖动角度最大偏移（度）
        float laserDuration = 5f;      // 激光总存在时间
        float damageInterval = 0.2f;   // 伤害间隔
        float damageTimer = 0f;
        float elapsedTime = 0f;

        if (otherBaseRole == null)
        {
            // 激光结束，关闭LineRenderer
            lineRenderer.enabled = false;
            yield break;
        }

        Transform target = otherBaseRole.transform;

        while (lineRenderer.enabled && elapsedTime < laserDuration)
        {
            if (otherBaseRole== null)
            {
                // 激光结束，关闭LineRenderer
                lineRenderer.enabled = false;
                yield break;
            }
                

            elapsedTime += Time.deltaTime;
            damageTimer += Time.deltaTime;

            Vector2 startPos = transform.position;

            // 获取目标方向
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            // 计算平滑抖动角度
            float time = Time.time * noiseSpeed;
            float smoothAngleOffset = Mathf.Sin(time) * noiseAmplitude;
            Quaternion rotationOffset = Quaternion.Euler(0, 0, smoothAngleOffset);
            Vector2 finalDirection = rotationOffset * directionToTarget;

            // LayerMask 排除自己
            int ignoreLayer = LayerMask.GetMask("NiHong", "YeLu", "Weapon");
            int layerMask = ~ignoreLayer;

            // 发射射线
            RaycastHit2D hit = Physics2D.Raycast(startPos, finalDirection, 100f, layerMask);

            Vector2 endPos;

            if (hit.collider != null &&
                (hit.collider.CompareTag("wall") || hit.collider.CompareTag("Role")))
            {
                endPos = hit.point;

                // 如果命中Role并且时间到达伤害间隔，造成伤害
                if (hit.collider.CompareTag("Role") && damageTimer >= damageInterval)
                {
                    damageTimer = 0f;
                    hit.collider.GetComponent<BaseRole>().TakeDamage(1);
                }
            }
            else
            {
                endPos = startPos + finalDirection * 100f;
            }
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            yield return null;
        }

        // 激光结束，关闭LineRenderer
        lineRenderer.enabled = false;
        yield break;
    }


    private IEnumerator MoveBallToTarget(GameObject ball)
    {
        while (ball != null && Vector3.Distance(ball.transform.position, transform.position) > 0.1f)
        {
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, transform.position, 50 * Time.deltaTime);
            yield return null;
        }
        yield break;
    }
}
