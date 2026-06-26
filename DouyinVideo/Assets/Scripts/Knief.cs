using System.Collections;
using UnityEngine;

/// <summary>
/// 小刀武器脚本，负责拾取后的旋转、命中和超时回收。
/// </summary>
public class Knief : MonoBehaviour
{
    #region 常量配置

    private const string WeaponLayerName = "Weapon";
    private const string DefaultLayerName = "Default";
    private static readonly Vector3 DefaultLocalPosition = new Vector3(-2.214f, 0, 0);

    #endregion

    #region 运行状态

    private Transform ball;
    private bool isAttached = false;

    #endregion

    #region 武器流程

    /// <summary>
    /// 将小刀挂载到拾取角色身上。
    /// </summary>
    /// <param name="ballTransform">拾取小刀的角色 Transform。</param>
    public void AttachToBall(Transform ballTransform)
    {
        gameObject.layer = LayerMask.NameToLayer(WeaponLayerName);
        ball = ballTransform;
        isAttached = true;
        Transform knifeRoot = transform.parent;
        knifeRoot.position = ball.position;
        //Broadcast.instance.BroadCastNews($"{ball.GetComponent<BaseRole>().RoleName}拾取了小刀", ball.GetComponent<BaseRole>().roleColor);
        // 取消父子关系更好，避免嵌套旋转干扰
        // 或者保留，看需求
        knifeRoot.SetParent(ball);

        StartCoroutine(DestroyThis());
    }

    void Update()
    {
        if (VideoGameManager.instance.gameEnd)
        {
            gameObject.SetActive(false);
            return;
        }
        if (isAttached && ball != null)
        {
            transform.parent.Rotate(0, 0, VideoGameManager.instance.KniefRotateSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region 拾取与命中入口

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttached)
        {
            if (collision.CompareTag("Role"))
            {
                BaseRole role = collision.GetComponent<BaseRole>();
                if (role.canGetWeapon)
                {
                    AttachToBall(collision.transform);
                }
            }
        }
        else
        {
            if (collision.CompareTag("Role")&& collision.transform!= ball)
            {
                BaseRole targetRole = collision.GetComponent<BaseRole>();
                targetRole.TakeDamage(10);

                BaseRole ownerRole = ball.GetComponent<BaseRole>();
                Broadcast.instance.BroadCastNews($"{ownerRole.RoleName}" +
                    $"攻击了{targetRole.RoleName}，获得一点能量", ownerRole.roleColor);

                VideoGameManager.instance.KniefHoldTime += 1;
                VideoGameManager.instance.KniefRotateSpeed += 5;

                ownerRole.AddBig();
                gameObject.layer = LayerMask.NameToLayer(DefaultLayerName);
                gameObject.transform.parent.gameObject.SetActive(false);
                isAttached = false;
                gameObject.transform.localPosition = DefaultLocalPosition;
                gameObject.transform.parent.localEulerAngles = Vector3.zero;
                VideoGameManager.instance.SpawnWeapons();
                
            }
        }
    }

    #endregion

    #region 回收流程

    /// <summary>
    /// 持有时间结束后回收小刀。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(VideoGameManager.instance.KniefHoldTime);
        gameObject.layer = LayerMask.NameToLayer(DefaultLayerName);
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.transform.localPosition = DefaultLocalPosition;
        gameObject.transform.parent.localEulerAngles = Vector3.zero;
        isAttached =false;
        VideoGameManager.instance.SpawnWeapons();
        //Broadcast.instance.BroadCastNews($"{ball.GetComponent<BaseRole>().RoleName}的小刀消失了", ball.GetComponent<BaseRole>().roleColor);
        VideoGameManager.instance.KniefHoldTime += 1f;
        VideoGameManager.instance.KniefRotateSpeed += 5;

        yield break;
    }

    #endregion
}