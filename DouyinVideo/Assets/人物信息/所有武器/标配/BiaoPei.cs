using System.Collections;
using UnityEngine;

/// <summary>
/// 标配手枪脚本，负责拾取、挂载、射击和回收。
/// </summary>
public class BiaoPei : MonoBehaviour
{
    #region 常量配置

    private const string WeaponLayerName = "Weapon";
    private const string DefaultLayerName = "Default";
    private const string RoleTag = "Role";
    private const float ShootInterval = 1f;
    private const float FinishWaitSeconds = 0.5f;
    private const float WeaponScaleX = 1.8f;
    private const float WeaponScaleY = 1.8f;
    private const int BulletSpeed = 50;
    private const int HitDamage = 5;

    #endregion

    #region 字段配置

    [Tooltip("开枪音源")]
    public AudioSource audioSource;

    [Tooltip("开枪音效")]
    public AudioClip kaiqiangAudio;//开枪声

    [Tooltip("武器名称")]
    public string GunName = "标配";

    [Tooltip("本轮子弹数量")]
    public int bulletCount = 4;

    [Tooltip("子弹伤害")]
    public int bulletDamage = 5;

    [Tooltip("当前持有武器的角色")]
    public BaseRole role;

    [Tooltip("当前射击目标")]
    public BaseRole targetRole;

    [Tooltip("射击点")]
    public Transform shootPos;

    [Tooltip("子弹脚本")]
    public VideoGameBulletBase bullet;

    #endregion

    #region 运行状态

    private int currentCount = 0;
    private Vector3 direction;
    private bool isAttached=false;

    #endregion

    #region 武器流程

    /// <summary>
    /// 将标配挂载到拾取角色身上并开始射击。
    /// </summary>
    /// <param name="baseRole">拾取标配的角色。</param>
    /// <param name="target">本轮射击目标。</param>
    public void AttachToRole(BaseRole baseRole,BaseRole target)
    {
        Debug.Log("执行了配枪");
        MarkAsAttached(baseRole);
        SetTargetAndAttachRoot(target);
        StartCoroutine(Shoot());
    }

    /// <summary>
    /// 执行标配射击流程。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    public IEnumerator Shoot()
    {
        while(currentCount > 0)
        {
            yield return new WaitForSeconds(ShootInterval);
            transform.localScale = new Vector3(WeaponScaleX, WeaponScaleY, 1);
            audioSource.PlayOneShot(kaiqiangAudio);
            direction = GetCurrentShootDirection();
           
            bullet.Init(direction,0,BulletSpeed, shootPos.position, role);
            bullet.OnBulletHitRole = HandleBulletHitRole;
            currentCount--;
        }
        yield return new WaitForSeconds(FinishWaitSeconds);
        
        SetDeafult();
        VideoGameManager.instance.SpawnWeapons();
        yield break;
    }

    /// <summary>
    /// 复位标配状态。
    /// </summary>
    public void SetDeafult()
    {
        gameObject.layer = LayerMask.NameToLayer(DefaultLayerName);
        transform.localScale = new Vector3(WeaponScaleX, WeaponScaleY, 1);
        VideoGameWeaponUtility.HideRootAndResetRotation(transform.parent);
        currentCount = bulletCount;
        VideoGameWeaponUtility.HideBulletRoot(bullet);
        isAttached = false;
    }

    #endregion

    #region 拾取入口

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttached)
        {
            if (collision.CompareTag(RoleTag))
            {
                BaseRole pickerRole = collision.GetComponent<BaseRole>();
                if (pickerRole.canGetWeapon)
                {
                    Broadcast.instance.BroadCastNews($"{pickerRole.RoleName}捡到了标配", pickerRole.roleColor);
                    AttachToRole(pickerRole, pickerRole.otherBaseRole);
                }
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 标记标配进入持有状态。
    /// </summary>
    /// <param name="baseRole">拾取标配的角色。</param>
    private void MarkAsAttached(BaseRole baseRole)
    {
        gameObject.layer = LayerMask.NameToLayer(WeaponLayerName);
        isAttached =true;
        role =baseRole;
    }

    /// <summary>
    /// 设置本轮目标，并把武器根物体挂到持有者身上。
    /// </summary>
    /// <param name="target">本轮射击目标。</param>
    private void SetTargetAndAttachRoot(BaseRole target)
    {
        targetRole=target;
        VideoGameWeaponUtility.AttachRootToRole(transform.parent, role.transform);
        currentCount=bulletCount;
    }

    /// <summary>
    /// 按当前角色和目标位置计算本发子弹方向。
    /// </summary>
    /// <returns>本发子弹的单位方向。</returns>
    private Vector3 GetCurrentShootDirection()
    {
        Vector3 rolePosition = role.transform.position;
        Vector3 targetPosition = targetRole.transform.position;
        return VideoGameWeaponUtility.AimAndGetShootDirection(
            transform.parent,
            transform,
            shootPos,
            rolePosition,
            targetPosition,
            WeaponScaleX,
            WeaponScaleY);
    }

    /// <summary>
    /// 处理标配子弹命中角色后的播报、伤害和回收。
    /// </summary>
    /// <param name="targetRole">被标配命中的角色。</param>
    private void HandleBulletHitRole(BaseRole targetRole)
    {
        VideoGameCombatUtility.BroadcastThenDamage(targetRole, HitDamage, $"{role.RoleName}的标配击中了{targetRole.RoleName}造成了5点伤害", role.roleColor);
        bullet.Recycle();
    }

    #endregion
}
