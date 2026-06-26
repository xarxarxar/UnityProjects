using System.Collections;
using UnityEngine;

public class QianBaoRole : BaseRole
{
    private static readonly Color32 BulletLineColor = new Color32(255, 210, 0, 255);

    public GameObject DaJu;//大狙
    public VideoGameBulletBase bullet;//大狙的子弹
    public Transform shootPos;//射击点
    public AudioClip shoot;//开枪声

    public LineRenderer lineRenderer;//弹道
    private Vector3 startPos;
    private Coroutine lineCoro = null;
    private int currentBulletCount = 3;

    private Coroutine bigCoro = null;
    public override void Big()
    {
        
        SetDeafult();
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(1.0f);
        DaJu.transform.parent.gameObject.SetActive(true);
        
        while (currentBulletCount > 0)
        {
            yield return new WaitForSeconds(2.0f);
            BaseRole targetRole = otherBaseRole;
            lineRenderer.enabled = true;
            lineRenderer.startColor = BulletLineColor;
            lineRenderer.endColor = BulletLineColor;


            Vector3 gunDirction = (targetRole.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(gunDirction.y, gunDirction.x) * Mathf.Rad2Deg;
            DaJu.transform.parent.rotation = Quaternion.Euler(0, 0, angle + 180);

            float z = DaJu.transform.parent.rotation.eulerAngles.z;
            z = (z > 180f) ? z - 360f : z;
            if (z < 90 && z > -90)
            {
                DaJu.transform.localScale = new Vector3(0.2f, 0.4f, 1);
            }
            else
            {
                DaJu.transform.localScale = new Vector3(0.2f, -0.4f, 1);
            }

            Vector3 direction = (targetRole.transform.position - shootPos.position).normalized;
            PlayAudio(shoot);
            bullet.Init(direction, 0, 80, shootPos.position, this);
            startPos = shootPos.position;
            if (lineCoro != null)
            {
                StopCoroutine(lineCoro);
                lineCoro = null;
            }

            bullet.OnBulletHitRole = (targetRole) =>
            {
                VideoGameCombatUtility.BroadcastThenDamageAndSetSpeed(targetRole, 20, 0.7f, 1.0f, $"{RoleName}的大狙对{targetRole.RoleName}造成了20点伤害并减速", roleColor);
                bullet.Recycle();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, bullet.transform.position);
                lineCoro=StartCoroutine(FadeOutLaser(0.5f));
                //lineCoro = null;
                //lineRenderer.enabled = false;
            };
            bullet.OnBulletHitWall = () =>
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, bullet.transform.position);
                lineCoro=StartCoroutine(FadeOutLaser(0.5f));
            };
            currentBulletCount--;
        }
        yield return new WaitForSeconds(0.5f);
        SetDeafult();
    }

    private IEnumerator FadeOutLaser(float duration)
    {
        float time = 0f;
        Color startColor = lineRenderer.startColor;

        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(startColor.a, 0f, t);

            Color fadedColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lineRenderer.startColor = fadedColor;
            lineRenderer.endColor = fadedColor;

            time += Time.deltaTime;
            yield return null;
        }

        // 最终关闭
        lineRenderer.positionCount = 0;
    }

    public void SetDeafult()
    {
        DaJu.transform.localScale = new Vector3(0.2f, 0.4f, 1);
        DaJu.transform.parent.gameObject.SetActive(false);
        DaJu.transform.parent.localEulerAngles = Vector3.zero;
        currentBulletCount = 3;
        bullet.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            UseBig();
        }
    }
}
