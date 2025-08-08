using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 捷风的脚本
/// </summary>
public class JieFengRole : BaseRole
{
    public List<VideoGameBulletBase> kniefs = new List<VideoGameBulletBase>();
    private List<VideoGameBulletBase> usedKniefs = new List<VideoGameBulletBase>();//已经使用过的小刀
    public AudioClip kniefAudio;
    public VideoGameBulletBase currentKnief =null;

    private Coroutine bigCoro = null;
    private Coroutine addUsedKniefsCoro = null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        if(addUsedKniefsCoro != null)
        {
            StopCoroutine(addUsedKniefsCoro);
            addUsedKniefsCoro = null;
        }

        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        for(int i = 0; i < usedKniefs.Count; i++)
        {
            Destroy(usedKniefs[i].transform.parent.GetComponent<Rigidbody2D>());
            kniefs.Add(usedKniefs[i]);
        }
        usedKniefs.Clear();

        yield return new WaitForSeconds(0.8f);
        for(int i=0; i < kniefs.Count; i++)
        {
            kniefs[i].transform.parent.gameObject.SetActive(true);
            kniefs[i].transform.parent.localPosition = Vector3.zero;
            kniefs[i].transform.localPosition = new Vector3(-2, 0, 0);
            kniefs[i].transform.localEulerAngles = Vector3.zero;
            kniefs[i].GetComponent<Collider2D>().enabled = false;
        }
        SetKniefsRotate();
        Broadcast.instance.BroadCastNews($"{RoleName}使用了大招", roleColor);
        //依次发射
        for (int i = kniefs.Count-1; i >=0; i--)
        {
            yield return new WaitForSeconds(0.5f);
            PlayAudio(kniefAudio);
            Vector3 target = otherBaseRole.transform.position;
            Vector3 direction = (target - transform.position).normalized;
            Debug.Log($"target is {otherBaseRole.RoleName},pos is {otherBaseRole.transform.position}");
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            kniefs[i].GetComponent<Collider2D>().enabled = true;
            if (kniefs[i].transform.parent.GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D  rb= kniefs[i].transform.parent.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.angularDrag = 0;
                rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
            kniefs[i].Init(direction, angle+180,70, kniefs[i].transform.position,this);
            kniefs[i].OnBulletHitRole = (targetRole) =>
            {
                Broadcast.instance.BroadCastNews($"{RoleName}的飞镖射中了{targetRole.RoleName}造成了10点伤害", roleColor);
                targetRole.TakeDamage(10);
                kniefs[i].Recycle();
            };
            currentKnief = kniefs[i];
            AddUsedKniefs(kniefs[i]);
            kniefs.RemoveAt(i);
            SetKniefsRotate();
        }

        yield break;
    }

   


    private void SetKniefsRotate()
    {
        for (int i = 0; i < kniefs.Count; i++)
        {
            Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f + i * (360/ kniefs.Count));
            kniefs[i].transform.parent.transform.rotation = rotation;
        }
    }

    private void AddUsedKniefs(VideoGameBulletBase knief)
    {
        usedKniefs.Add(knief);
        addUsedKniefsCoro=StartCoroutine(AddUsedKniefsCoro(knief));
    }

    private IEnumerator AddUsedKniefsCoro(VideoGameBulletBase knief)
    {
        yield return new WaitForSeconds(3);
        knief.transform.parent.gameObject.SetActive(false);
    }
}
