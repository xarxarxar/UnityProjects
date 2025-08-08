using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class BaseRole : MonoBehaviour
{
    public string RoleName;
    [Tooltip("血量")]
    public int HP = 100;
    [Tooltip("角色头像")]
    public Image roleImage;
    [Tooltip("角色上方信息")]
    [HideInInspector]public RoleUI roleUI;
    [Tooltip("受伤音频")]
    public AudioClip getHurt;
    [Tooltip("大招音频")]
    public AudioClip bigAudio;

    public Color32 roleColor;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private int bigCount;//大招点数
    private float speedPercent=1;//速度的百分比

    private bool isDie=false;
    public bool canGetWeapon = true;//是否能拾取武器
    public bool canUseBig = true;//是否可以使用技能

    public BaseRole otherBaseRole=> GetRandomOther(this);

    public bool CantSelected=false;

    //拉屎
    //这个值最大100，满100的时候会强制拉一坨屎并扣10滴血，
    //满80的时候如果没有进厕所拉屎会缓缓扣血，直到拉了屎，拉一次屎这个值会掉50
    //踩到了别人的屎这个值会瞬间增加10
    //这个值小于50的时候，哪怕进厕所也不拉屎，拉屎的时候速度为0
    private int lashiValue;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource =GetComponent<AudioSource>();
        StartCoroutine(ApplyRandomForce());
        StartGame();
    }

    public virtual void StartGame()
    {
        
    }

    void FixedUpdate()
    {
        if(isDie)
        {
            return;
        }
        if (VideoGameManager.instance.gameEnd)
        {
            if (GetComponent<Rigidbody2D>())
            {
                rb.velocity = Vector3.zero;
                Destroy(rb);
            }
            return;
        }
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = rb.velocity.normalized;

            // 添加一个非常小的随机偏移，避免轨迹锁死
            Vector2 randomOffset = new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            dir += randomOffset;
            dir.Normalize();

            rb.velocity = dir * VideoGameManager.instance.ballSpeed* speedPercent;
        }
    }

    /// <summary>
    /// 设置速度的百分比
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="duration">持续时长</param>
    public void SetSpeed(float percent,float duration=0)
    {
        if(percent<0)
        {
            return;
        }
        if (duration != 0)
        {
            StartCoroutine(SetSpeedCoro(duration));
        }
        else
        {
            if (speedPercent == 0)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection, ForceMode2D.Impulse);
            }
        }
        speedPercent = percent;
    }

    /// <summary>
    /// 当速度等于0了之后，得手动指示方向
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SetSpeedCoro(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (speedPercent == 0)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDirection, ForceMode2D.Impulse);
        }
        speedPercent = 1;
        yield break;
    }

    /// <summary>
    /// 设置是否可以捡起武器
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="duration">持续时长</param>
    public void SetCanGetWeapon(bool CanGetWeapon, float duration = 0)
    {
        if (transform.Find("Canvas").Find("负面效果").Find("不能拾取武器(Clone)"))
        {
            Destroy(transform.Find("Canvas").Find("负面效果").Find("不能拾取武器(Clone)").gameObject);
        }
        canGetWeapon = CanGetWeapon;
        Instantiate(VideoGameManager.instance.CantGetWeaponPrefab,transform.Find("Canvas").Find("负面效果"));
        if (duration != 0)
        {
            StartCoroutine(SetCanGetWeaponCoro(duration));
        }
    }

    /// <summary>
    /// 设置不能拾取武器
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SetCanGetWeaponCoro(float duration)
    {
        yield return new WaitForSeconds(duration);
        canGetWeapon = true;

        if (transform.Find("Canvas").Find("负面效果").Find("不能拾取武器(Clone)"))
        {
            Destroy(transform.Find("Canvas").Find("负面效果").Find("不能拾取武器(Clone)").gameObject);
        }
        yield break;
    }


    /// <summary>
    /// 设置是否可以使用技能
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="duration">持续时长</param>
    public void SetCanUseBig(bool CanUseBig, float duration = 0)
    {
        if (transform.Find("Canvas").Find("负面效果").Find("KO压制UI(Clone)"))
        {
            Destroy(transform.Find("Canvas").Find("负面效果").Find("KO压制UI(Clone)").gameObject);
        }
        canUseBig = CanUseBig;
        Instantiate(VideoGameManager.instance.CantUseBigPrefab, transform.Find("Canvas").Find("负面效果"));
        if (duration != 0)
        {
            StartCoroutine(SetCanUseBigCoro(duration));
        }
    }

    /// <summary>
    /// 能否使用技能
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SetCanUseBigCoro(float duration)
    {
        yield return new WaitForSeconds(duration);
        canUseBig = true;

        if (transform.Find("Canvas").Find("负面效果").Find("KO压制UI(Clone)"))
        {
            Destroy(transform.Find("Canvas").Find("负面效果").Find("KO压制UI(Clone)").gameObject);
        }
        yield break;
    }

    ///恢复血量
    public void RecoverHp(int value)
    {
        if (HP + value >= 100)
        {
            HP = 100;
        }
        else
        {
            HP += value;
        }
        roleUI.TakeDamage(HP);
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if(isDie) { return; }
        HP -= damage;

        roleUI.TakeDamage(HP);
        if (HP <= 0)
        {
            Debug.Log($"{RoleName}死了");
            HP = 0;
            Broadcast.instance.BroadCastNews($"{RoleName}淘汰", roleColor);
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
            // 停止正在进行的颜色动画
            roleImage.DOKill(); // 关键代码
            roleImage.color = new Color32(100, 100, 100, 255);
            Debug.Log($"{RoleName}的image颜色为{roleImage.color}");
            isDie = true;
            transform.Find("Canvas").GetComponent<Canvas>().sortingOrder--;
            VideoGameManager.instance.RoleDie(this);
            //Destroy(gameObject); // 下一帧销毁
        }
        else
        {
            Debug.Log($"{RoleName}受到了伤害,HP为{HP}");
            // 闪红效果
            roleImage.DOColor(Color.red, 0.1f)
                     .OnComplete(() => roleImage.DOColor(Color.white, 0.2f));
        }
        PlayAudio(getHurt);
    }


    /// <summary>
    /// 增加大招点数
    /// </summary>
    public void AddBig()
    {
        bigCount++;
        if (bigCount >= 3)
        {
            bigCount = 3;
        }
        roleUI.AttackOther(transform.position);
        if (!canUseBig) return;
        if (bigCount >= 3)
        {
            UseBig();
            bigCount = 0;
        }
    }

    /// <summary>
    /// 使用大招
    /// </summary>
    public virtual void UseBig()
    {
        Debug.Log("使用大招");
        PlayAudio(bigAudio);
        Big();

        roleUI.ClearBig();
    }

    public abstract void Big();

    public void PlayAudio(AudioClip audioClip)
    {
        //audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioClip);
    }

    /// <summary>
    /// 给物体施加一个随机方向的力（2D）
    /// </summary>
    private  IEnumerator ApplyRandomForce()
    {
        yield return new WaitForSeconds(3);//暂时修改
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection, ForceMode2D.Impulse);
    }

    public BaseRole GetRandomOther(BaseRole self)
    {
        if (VideoGameManager.instance.circleRoles == null || VideoGameManager.instance.circleRoles.Count <= 1)
            return null; // 没有其他角色了

        // 先过滤掉自己
        List<BaseRole> others = VideoGameManager.instance.circleRoles.FindAll(role => role != self);

        if (others.Count == 0)
            return null;

        // 随机选择一个
        int index = Random.Range(0, others.Count);
        return others[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBall"))
        {
            AddBig();
            //Broadcast.instance.BroadCastNews($"{RoleName}吃到了能量球", roleColor);
            AudioManager.Instance.PlaySFX("吃到球");
            Destroy(collision.gameObject);
            //VideoGameManager.instance.currentReward.SetActive(false);
            //VideoGameManager.instance.SpawnWeapons();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySFX("撞击");
    }
}
