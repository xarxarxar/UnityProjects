using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FireWallFireManRole : BaseRole
{
    public LineRenderer lineRenderer;
    public Texture[] flameFrames; // 帧图数组
    public float frameRate = 15f; // 每秒多少帧

    private bool isRecovered=false;
    private Coroutine fireCoro=null;
    private Coroutine playFlameLoopCoro = null;
    private Coroutine recoverHealthCoro = null;

    public override void Big()
    {
        
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.enabled = false;
        isRecovered=false;
        if (fireCoro != null)
        {
            StopCoroutine(fireCoro);
            fireCoro=null;
        }
        fireCoro = StartCoroutine(DrawFireWall());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            UseBig();
            RecoverHp(2);
        }
    }

    IEnumerator DrawFireWall()
    {
        yield return new WaitForSeconds(1.0f);

        Vector3 startPos = transform.position;
        Vector3 direction = (otherBaseRole.transform.position - startPos).normalized;
        Vector3 endPos = startPos + direction * 20;
        float duration = 0.5f;
        float time = 0f;

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos); // 初始长度为 0

        if (playFlameLoopCoro != null)
        {
            StopCoroutine(playFlameLoopCoro);
            playFlameLoopCoro = null;
        }
        playFlameLoopCoro = StartCoroutine(PlayFlameLoop());// 火焰的粒子或视觉效果

        // 步骤 1：激光逐步延伸
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 currentEnd = Vector3.Lerp(startPos, endPos, t);
            lineRenderer.transform.position = currentEnd; // 如果你要控制整体位置，否则可以删掉
            lineRenderer.SetPosition(1, currentEnd);
            yield return null;
        }

        // 步骤 2：激光延伸完成，设置终点
        lineRenderer.SetPosition(1, endPos); // 最终精确对齐

        // 步骤 3：持续检测是否有角色被击中，5 秒内持续造成伤害
        GameObject currentTarget = null;
        float damageTimer = 0f;
        float damageInterval = 0.15f;
        int ignoreLayer = LayerMask.GetMask("YeLu");
        int layerMask = ~ignoreLayer;

        float activeTime = 0f;
        float maxDuration = 5.0f;

        while (activeTime < maxDuration)
        {
            activeTime += Time.deltaTime;

            RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, Vector3.Distance(startPos, endPos), layerMask);
            GameObject newTarget = null;

            foreach (RaycastHit2D h in hits)
            {
                if (h.collider != null && h.collider.CompareTag("Role") )
                {
                    if (h.collider.gameObject == gameObject)
                    {
                        if (recoverHealthCoro != null)
                        {
                            StopCoroutine(recoverHealthCoro);
                            recoverHealthCoro = null;
                        }
                        recoverHealthCoro = StartCoroutine(RecoverHealth());
                        continue; // 跳过自己，不当作攻击目标
                    }
                    newTarget = h.collider.gameObject;

                    break;
                }
            }

            if (newTarget != null)
            {
                if (newTarget == currentTarget)
                {
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= damageInterval)
                    {
                        damageTimer = 0f;
                        newTarget.GetComponent<BaseRole>()?.TakeDamage(3);
                        Broadcast.instance.BroadCastNews($"{RoleName}的火墙对 {newTarget.name} 造成了3点伤害", roleColor);
                    }
                }
                else
                {
                    currentTarget = newTarget;
                    damageTimer = 0f;
                    Debug.Log($"锁定新目标：{currentTarget.name}");
                }
            }
            else
            {
                currentTarget = null;
                damageTimer = 0f;
            }

            yield return null;
        }

        // 步骤 4：时间结束，关闭激光
        lineRenderer.enabled = false;
        lineRenderer.gameObject.SetActive(false);
        isRecovered = false;

        if (playFlameLoopCoro != null)
        {
            StopCoroutine(playFlameLoopCoro);
            playFlameLoopCoro = null;
        }

        StopCoroutine(fireCoro);
        fireCoro = null;
        Debug.Log("火墙结束");
        yield break;
    }

    IEnumerator PlayFlameLoop()
    {
        int frame = 0;
        float waitTime = 1f / frameRate;

        while (true)
        {
            lineRenderer.material.mainTexture = flameFrames[frame];
            frame = (frame + 1) % flameFrames.Length;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator RecoverHealth()
    {
        if (isRecovered) yield break;
        Debug.Log("火男回血");
        Broadcast.instance.BroadCastNews($"{RoleName}碰到了火墙，开始回血", roleColor);
        isRecovered = true;

        int times = 6;
        float interval = 0.5f;
        int healAmount = 2;

        for (int i = 0; i < times; i++)
        {
            RecoverHp(healAmount);
            
            yield return new WaitForSeconds(interval);
        }
        yield break;
        
    }
}
