using UnityEngine;

/// <summary>
/// 源氏的大刀
/// </summary>
public class YuanShiBigKnief : MonoBehaviour
{
    private int rotateDire=1;//旋转的方向
    private int attackCount=0;//攻击的次数
    public BaseRole role;
    void Update()
    {
        if (VideoGameManager.instance.gameEnd)
        {
            gameObject.SetActive(false);
            return;
        }
        transform.parent.Rotate(0, 0, rotateDire * 160 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Role"))
        {
            BaseRole targetRole = collision.GetComponent<BaseRole>();
            if (targetRole != role && attackCount<3)
            {
                VideoGameCombatUtility.DamageThenBroadcast(targetRole, 20, $"{role.RoleName}的大刀对{targetRole.RoleName}造成了20点伤害", role.roleColor);
                rotateDire *= -1;//旋转方向逆行
                transform.parent.localScale= new Vector3(1, rotateDire,1);
            
                attackCount++;
                if (attackCount >= 3)
                {
                    BigKniefDisappear();
                }

            }
        }
    }

    public void BigKniefDisappear()
    {
        rotateDire = 1;
        attackCount = 0;
        transform.parent.localScale = Vector3.one;
        transform.parent.gameObject.SetActive(false);
    }
}
