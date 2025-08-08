using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// KO的大招
/// </summary>
public class KOBig : MonoBehaviour
{
    public BaseRole role;
    public bool canYazhi=true;//可以压制
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 你可以根据tag或者组件筛选目标，比如
        if (collision.CompareTag("Role") && collision.gameObject != gameObject )
        {
            collision.GetComponent<BaseRole>().SetCanUseBig(false, 5);
            Broadcast.instance.BroadCastNews($"{role.RoleName}压制了{collision.GetComponent<BaseRole>().RoleName},无法使用大招", role.roleColor);
        }
    }
}
