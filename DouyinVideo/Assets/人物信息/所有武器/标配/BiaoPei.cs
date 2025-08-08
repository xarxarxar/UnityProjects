using System.Collections;
using UnityEngine;

/// <summary>
/// 标配手枪的脚本
/// </summary>
public class BiaoPei : MonoBehaviour
{   
    public AudioSource audioSource;
    public AudioClip kaiqiangAudio;//开枪声
    public string GunName = "标配";
    public int bulletCount = 4;
    private int currentCount = 0;
    public int bulletDamage = 5;
    public BaseRole role;
    public BaseRole targetRole;
    public Transform shootPos;
    public VideoGameBulletBase bullet;

    private Vector3 direction;
    private bool isAttached=false;


    public void AttachToRole(BaseRole baseRole,BaseRole target)
    {
        Debug.Log("执行了配枪");
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        isAttached =true;

        role =baseRole;
        targetRole=target;
        transform.parent.position= role.transform.position; 
        transform.parent.SetParent(role.transform);
        currentCount=bulletCount;
        StartCoroutine(Shoot());
    }

    public IEnumerator Shoot()
    {
        while(currentCount > 0)
        {
            yield return new WaitForSeconds(1);
            transform.localScale = new Vector3(1.8f, 1.8f, 1);
            audioSource.PlayOneShot(kaiqiangAudio);
            Vector3 gunDirction= (targetRole.transform.position - role.transform.position).normalized;
            float angle = Mathf.Atan2(gunDirction.y, gunDirction.x) * Mathf.Rad2Deg;
            transform.parent.rotation = Quaternion.Euler(0, 0, angle+180);

            float z = transform.parent.rotation.eulerAngles.z;
            z = (z > 180f) ? z - 360f : z;
            if (z < 90 && z > -90)
            {
                transform.localScale= new Vector3(1.8f, 1.8f, 1);
            }
            else
            {
                transform.localScale = new Vector3(1.8f, -1.8f, 1);
            }
            direction = (targetRole.transform.position - shootPos.position).normalized;
           
            bullet.Init(direction,0,50, shootPos.position, role);
            bullet.OnBulletHitRole = (targetRole) =>
            {
                Broadcast.instance.BroadCastNews($"{role.RoleName}的标配击中了{targetRole.RoleName}造成了5点伤害", role.roleColor);
                targetRole.TakeDamage(5);
                bullet.Recycle();
            };
            currentCount--;
        }
        yield return new WaitForSeconds(0.5f);
        
        SetDeafult();
        VideoGameManager.instance.SpawnWeapons();
        yield break;
    }

    public void SetDeafult()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        transform.localScale = new Vector3(1.8f, 1.8f, 1);
        transform.parent.gameObject.SetActive(false);
        transform.parent.localEulerAngles = Vector3.zero;
        currentCount = bulletCount;
        bullet.transform.parent.gameObject.SetActive(false);
        isAttached = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttached)
        {
            if (collision.CompareTag("Role") && collision.GetComponent<BaseRole>().canGetWeapon)
            {
                Broadcast.instance.BroadCastNews($"{collision.GetComponent<BaseRole>().RoleName}捡到了标配", collision.GetComponent<BaseRole>().roleColor);
                AttachToRole(collision.GetComponent<BaseRole>(), collision.GetComponent<BaseRole>().otherBaseRole);
            }
        }
    }
}
