using System.Collections;
using UnityEngine;

/// <summary>
/// 丢帽子的保安
/// </summary>
public class DropHatBaoAnRole : BaseRole
{
    public Hat hat;
    private Coroutine bigCoro;
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
        Broadcast.instance.BroadCastNews($"{RoleName}发射了一个帽子", roleColor);
        hat.transform.SetParent(null);
        Vector3 direction=(otherBaseRole.transform.position-transform.position).normalized;
        hat.transform.position = transform.position+direction*2.0f;
        hat.gameObject.SetActive(true);

        hat.Init(direction,40);
        hat.OnHitRole = (hitRole) =>
        {
            if (hitRole != this)
            {
                VideoGameCombatUtility.DamageThenBroadcast(hitRole, 10, $"{RoleName}的帽子对{hitRole.RoleName}造成了10点伤害", roleColor);
            }
        };

        yield return new WaitForSeconds(5.0f);
        hat.gameObject.SetActive(false);
        hat.transform.SetParent(transform);
    }
}
