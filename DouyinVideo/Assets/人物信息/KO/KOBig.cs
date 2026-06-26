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
            BaseRole targetRole = collision.GetComponent<BaseRole>();
            VideoGameCombatUtility.SetCanUseBigThenBroadcast(targetRole, false, 5, $"{role.RoleName}压制了{targetRole.RoleName},无法使用大招", role.roleColor);
        }
    }
}
