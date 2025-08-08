using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 奥丁
/// </summary>
public class AoDing : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip kaiqiangAudio;//开枪声
    public string GunName = "奥丁";
    public int bulletCount = 15;
    private int currentCount = 0;
    public int bulletDamage = 5;
    public BaseRole role;
    public BaseRole targetRole;
    public Transform shootPos;
    public VideoGameBulletBase bullet;

    private Vector3 direction;
    private bool isAttached = false;


    public void AttachToRole(BaseRole baseRole, BaseRole target)
    {
        
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        isAttached = true;

        role = baseRole;
        role.SetSpeed(0.2f);
        targetRole = target;
        transform.parent.position = role.transform.position;
        transform.parent.SetParent(role.transform);
        currentCount = bulletCount;
        StartCoroutine(Shoot());
    }

    public IEnumerator Shoot()
    {
        while (currentCount > 0)
        {
            yield return new WaitForSeconds(0.17f);
            transform.localScale = new Vector3(1.4f, 1.8f, 1);
            audioSource.PlayOneShot(kaiqiangAudio);
            Vector3 gunDirction = (targetRole.transform.position - role.transform.position).normalized;
            float angle = Mathf.Atan2(gunDirction.y, gunDirction.x) * Mathf.Rad2Deg;
            transform.parent.rotation = Quaternion.Euler(0, 0, angle + 180);

            float z = transform.parent.rotation.eulerAngles.z;
            z = (z > 180f) ? z - 360f : z;
            if (z < 90 && z > -90)
            {
                transform.localScale = new Vector3(1.4f, 1.8f, 1);
            }
            else
            {
                transform.localScale = new Vector3(1.4f, -1.8f, 1);
            }
            direction = (targetRole.transform.position - shootPos.position).normalized;

            bullet.Init(direction, 0, 70, shootPos.position, role);
            bullet.OnBulletHitRole = (targetRole) =>
            {
                Broadcast.instance.BroadCastNews($"{role.RoleName}的奥丁击中了{targetRole.RoleName}造成了5点伤害", role.roleColor);
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
        transform.localScale = new Vector3(1.4f, 1.8f, 1);
        transform.parent.gameObject.SetActive(false);
        transform.parent.localEulerAngles = Vector3.zero;
        currentCount = bulletCount;
        bullet.transform.parent.gameObject.SetActive(false);
        isAttached = false;

        role.SetSpeed(1.0f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttached)
        {
            if (collision.CompareTag("Role") && collision.GetComponent<BaseRole>().canGetWeapon)
            {
                Broadcast.instance.BroadCastNews($"{collision.GetComponent<BaseRole>().RoleName}捡到了奥丁", collision.GetComponent<BaseRole>().roleColor);
                AttachToRole(collision.GetComponent<BaseRole>(), collision.GetComponent<BaseRole>().otherBaseRole);
            }
        }
    }
}
