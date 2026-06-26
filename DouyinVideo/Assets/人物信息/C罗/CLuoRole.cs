using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLuoRole : BaseRole
{
    public Hat luntai;//siu
    public AudioClip luntaiBoom;//siu爆炸
    private Coroutine bigCoro = null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(HitHat());
    }

    #region 开发调试入口

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            UseBig();
        }
    }

    #endregion

    private IEnumerator HitHat()
    {
        yield return new WaitForSeconds(1.5f);
        Broadcast.instance.BroadCastNews($"{RoleName}发射了一个siu", roleColor);
        luntai.transform.SetParent(null);
        Vector3 direction = (otherBaseRole.transform.position - transform.position).normalized;
        luntai.transform.position = transform.position + direction * 2.0f;
        luntai.gameObject.SetActive(true);

        luntai.Init(direction, 20);
        SetSpeed(0, 0);
        luntai.OnHitRole = (hitRole) =>
        {
            if (hitRole != this)
            {
                VideoGameCombatUtility.DamageThenBroadcast(hitRole, 30, $"{RoleName}的siu对{hitRole.RoleName}造成了30点伤害", roleColor);
                PlayAudio(luntaiBoom);
                luntai.gameObject.SetActive(false);
                luntai.transform.SetParent(transform);
                SetSpeed(1, 0);
            }
        };
        yield return new WaitForSeconds(3.0f);
        SetSpeed(1, 0);
        luntai.gameObject.SetActive(false);
        luntai.transform.SetParent(transform);
    }
}
