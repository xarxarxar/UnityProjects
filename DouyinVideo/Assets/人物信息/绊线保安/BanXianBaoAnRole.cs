using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 会放绊线的保安
/// </summary>
public class BanXianBaoAnRole : BaseRole
{
    public GameObject laserEmitter;//激光发射器
    public GameObject laserReceiver;//激光接收器
    public LineRenderer laserLineRenderer;//激光

    public AudioClip bandaorenAudio;//绊线绊倒人的音效

    private Color32 normalLaser = new Color32(0,237,255,255);
    private Color32 hitLaser = new Color32(190,74,63,255);
    private bool laserEmitterAttached=false;


    private Coroutine bigCoro = null;
    private Coroutine drawLaserOverTimeCoro = null;

    public override void Big()
    {
        ShootLaserEmitter();
    }


    

    //发射一个激光发射器
    private void ShootLaserEmitter()
    {
        Vector3 direction=(otherBaseRole.transform.position- transform.position).normalized;
        LayerMask wallLayerMask = LayerMask.GetMask("Wall");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100, wallLayerMask);
        if (hit.collider != null)
        {
            // 命中墙壁
            //laserEmitter.transform.position = hit.point;
            //laserEmitter.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal); // 让发射器“朝向”墙面法线
            laserEmitter.SetActive(true);
            laserReceiver.SetActive(true);
            laserEmitterAttached = true;
            laserEmitter.transform.SetParent(null);
            laserReceiver.transform.SetParent(null);
            //laserEmitter.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal); // 让发射器“朝向”墙面法线

            if (bigCoro != null)
            {
                StopCoroutine(bigCoro);
                bigCoro = null;
            }

            bigCoro = StartCoroutine(MoveLaserEmitter(transform.position, hit.point, 70.0f, hit));
        }
    }

    IEnumerator MoveLaserEmitter(Vector3 startPos, Vector3 targetPos, float speed, RaycastHit2D hit)
    {
        float distance = Vector3.Distance(startPos, targetPos);
        float moved = 0f;

        while (moved < distance)
        {
            float step = speed * Time.deltaTime;
            moved += step;
            float t = Mathf.Clamp01(moved / distance);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            laserEmitter.transform.position = currentPos;
            laserReceiver.transform.position = currentPos;
            yield return null;
        }

        laserEmitter.transform.position = targetPos;
        laserReceiver.transform.position = targetPos;


        if (laserEmitterAttached)
        {
            LayerMask wallLayerMask = LayerMask.GetMask("Wall");

            Vector3 laserDirection = hit.normal; // 关键：使用命中的法线作为激光方向
            RaycastHit2D laserHit = Physics2D.Raycast(laserEmitter.transform.position + laserDirection * 0.01f, laserDirection, 100, wallLayerMask);
            if (laserHit.collider != null)
            {
                if (drawLaserOverTimeCoro != null)
                {
                    StopCoroutine(drawLaserOverTimeCoro);
                    drawLaserOverTimeCoro = null;
                }

                drawLaserOverTimeCoro = StartCoroutine(DrawLaserOverTime(laserEmitter.transform.position, laserHit.point, 0.1f));
            }
            else
            {
                if (drawLaserOverTimeCoro != null)
                {
                    StopCoroutine(drawLaserOverTimeCoro);
                    drawLaserOverTimeCoro = null;
                }

                drawLaserOverTimeCoro = StartCoroutine(DrawLaserOverTime(laserEmitter.transform.position, laserEmitter.transform.position + laserDirection * 100, 0.1f));
            }
        }
    }

    IEnumerator DrawLaserOverTime(Vector3 start, Vector3 end, float duration)
    {
        float time = 0f;
        laserLineRenderer.enabled = true;
        laserLineRenderer.positionCount = 2;
        laserLineRenderer.SetPosition(0, start);
        laserLineRenderer.SetPosition(1, start); // 初始长度为 0
        laserLineRenderer.startColor = normalLaser;
        laserLineRenderer.endColor = normalLaser;

        // 步骤 1：激光逐步延伸
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 currentEnd = Vector3.Lerp(start, end, t);
            laserReceiver.transform.position=currentEnd;
            laserLineRenderer.SetPosition(1, currentEnd);
            yield return null;
        }
        // 步骤 2：激光延伸完成，设置终点
        laserLineRenderer.SetPosition(1, end); // 最终精确对齐

        while (true)
        {
            // 步骤 3：开始检测是否击中了 Role
            // LayerMask 排除自己
            int ignoreLayer = LayerMask.GetMask("YeLu");
            int layerMask = ~ignoreLayer;

            RaycastHit2D[] hits = Physics2D.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), layerMask);
            GameObject hitRole = null;

            foreach (RaycastHit2D h in hits)
            {
                if (h.collider != null && h.collider.CompareTag("Role") && h.collider.gameObject != gameObject)
                {
                    hitRole = h.collider.gameObject;

                    Debug.Log($"击中了{hitRole.name}");
                    break;
                }
            }

            // 步骤 4：如果击中了 Role，则形成折线
            if (hitRole != null)
            {
                float tmpTime = 0;
                laserLineRenderer.startColor = hitLaser;
                laserLineRenderer.endColor = hitLaser;

                hitRole.GetComponent<BaseRole>().SetSpeed(0.3f,3);
                Broadcast.instance.BroadCastNews($"{RoleName}的绊线对{hitRole.GetComponent<BaseRole>().RoleName}造成了减速", roleColor);
                PlayAudio(bandaorenAudio);
                while (tmpTime<1.5f)
                {
                    laserLineRenderer.positionCount = 3;
                    laserLineRenderer.SetPosition(0, start);
                    laserLineRenderer.SetPosition(1, hitRole.transform.position);
                    laserLineRenderer.SetPosition(2, end);
                    tmpTime += Time.deltaTime;
                    yield return null;
                }

                hitRole.GetComponent<BaseRole>().TakeDamage(10);
                Broadcast.instance.BroadCastNews($"{RoleName}的绊线对{hitRole.GetComponent<BaseRole>().RoleName}造成了10点伤害", roleColor);

                laserLineRenderer.enabled = false;
                laserEmitter.transform.SetParent(transform);
                laserReceiver.transform.SetParent(transform);
                laserEmitter.SetActive(false);
                laserReceiver.SetActive(false);

                yield break;
            }
            yield return null;
        }

        
    }
}
