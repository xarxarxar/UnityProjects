using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangsuoRole : BaseRole
{
    public Hat daibu;//激光终端逮捕器
    public GameObject startPos;//激光起点等待的那个东西
    public LineRenderer lineRenderer;//激光
    public AudioClip getFail;//捕捉失败
    public AudioClip getSucceed;//捕捉成功
    public int currentReflections = 0; // 当前反弹次数
    private List<Vector3> hitPoints=new List<Vector3>();

    private bool isGetRole=false;//是否拉到人了

    private Coroutine bigCoro = null;
    private Coroutine linerenderMoveCoro = null;
    private Coroutine moveToPointsCoro = null;
    private Coroutine judgeIfSetLinerenderFalseCoro = null;


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            UseBig();
        }
    }

    public override void Big()
    {
        
        SetDefault();
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(2.5f);
        canUseBig = false;
        lineRenderer.enabled = true;
        hitPoints.Add(startPos.transform.position);
        lineRenderer.positionCount = 2;
        daibu.transform.SetParent(null);
        Vector3 direction = (otherBaseRole.transform.position - transform.position).normalized;
        daibu.transform.position = transform.position + direction * 2.0f;
        daibu.gameObject.SetActive(true);

        daibu.Init(direction, 60);


        if (linerenderMoveCoro != null)
        {
            StopCoroutine(linerenderMoveCoro);
            linerenderMoveCoro = null;
        }

        linerenderMoveCoro = StartCoroutine(LinerenderMove());

        daibu.OnHitRole = (role) =>
        {
            if (role != this)
            {
                
                isGetRole =true;
                daibu.transform.position = role.transform.position;
                daibu.GetComponent<Collider2D>().enabled = false;
                daibu.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;

                hitPoints.Add(daibu.transform.position);
                role.SetSpeed(0);
                role.GetComponent<Collider2D>().enabled = false;
                role.roleImage.color=Color.blue;

                if (moveToPointsCoro != null)
                {
                    StopCoroutine(moveToPointsCoro);
                    moveToPointsCoro = null;
                }

                moveToPointsCoro = StartCoroutine(MoveToPoints(role));
            }
        };

        daibu.OnHitWall = (wall) =>
        {
            lineRenderer.positionCount++;
            hitPoints.Add(daibu.transform.position);
            lineRenderer.SetPosition(lineRenderer.positionCount - 2, daibu.transform.position);
        };

        if (judgeIfSetLinerenderFalseCoro != null)
        {
            StopCoroutine(judgeIfSetLinerenderFalseCoro);
            judgeIfSetLinerenderFalseCoro = null;
        }

        judgeIfSetLinerenderFalseCoro = StartCoroutine(JudgeIfSetLinerenderFalse());
        
    }

    private IEnumerator LinerenderMove()
    {
        
        lineRenderer.SetPosition(0, startPos.transform.position);
        lineRenderer.SetPosition(1, startPos.transform.position); // 初始长度为 0
        while (true)
        {
            lineRenderer.SetPosition(lineRenderer.positionCount-1, daibu.transform.position);
            yield return null;
        }
    }

    /// <summary>
    /// 开始将角色往回收
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToPoints(BaseRole role)
    {
        if (hitPoints.Count < 2)
            yield break;
        PlayAudio(getSucceed);
        // 1. 计算总路径长度
        float totalDistance = 0f;
        List<float> segmentLengths = new List<float>();

        for (int i = hitPoints.Count - 1; i > 0; i--)
        {
            float segment = Vector3.Distance(hitPoints[i], hitPoints[i - 1]);
            segmentLengths.Add(segment);
            totalDistance += segment;
        }

        // 2. 倒序移动
        for (int i = hitPoints.Count - 1; i > 0; i--)
        {
            Vector3 start = hitPoints[i];
            Vector3 end = hitPoints[i - 1];
            float segmentLength = segmentLengths[hitPoints.Count - 1 - i];
            float segmentDuration = (segmentLength / totalDistance) * 3;

            yield return StartCoroutine(MoveToPointWithTime(role, start, end, segmentDuration));
        }
        SetDefault();
        role.roleImage.color = Color.white;
        role.TakeDamage(50);
        Broadcast.instance.BroadCastNews($"{RoleName}的大招对{role.RoleName}造成了50点伤害", roleColor);
        // 移动完成后的逻辑
        
        role.SetSpeed(1); // 或恢复移动
        role.GetComponent<Collider2D>().enabled = true;
    }

    private IEnumerator MoveToPointWithTime(BaseRole role, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            role.transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        role.transform.position = end; // 精确对齐
    }

    /// <summary>
    /// 判断是否要将激光设置为false
    /// </summary>
    /// <returns></returns>
    private IEnumerator JudgeIfSetLinerenderFalse()
    {
        float timer = 0f;
        float duration = 1f;

        while (timer < duration)
        {
            if (isGetRole)
                yield break; // 拉到人就提前退出

            timer += Time.deltaTime;
            yield return null; // 等一帧
        }
        PlayAudio(getFail);
        // 三秒内没拉到人，执行隐藏线条
        SetDefault();
    }

    private void SetDefault()
    {
        canUseBig = true;
        isGetRole = false;
        daibu.gameObject.SetActive(false);
        daibu.GetComponent<Collider2D>().enabled = true;
        daibu.transform.SetParent(transform);

        lineRenderer.enabled = false;
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;

        hitPoints.Clear();
    }
       
}
