using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 所有英雄共同基础脚本，负责角色运动、生命、大招点、状态和通用碰撞入口。
/// </summary>
[RequireComponent(typeof(AudioSource))]
public abstract class BaseRole : MonoBehaviour
{
    #region 常量

    private const string CanvasName = "Canvas";
    private const string NegativeEffectName = "负面效果";
    private const string CantGetWeaponIconName = "不能拾取武器(Clone)";
    private const string CantUseBigIconName = "KO压制UI(Clone)";

    #endregion

    #region Inspector 字段

    public string RoleName;

    [Tooltip("血量")]
    public int HP = 100;

    [Tooltip("角色头像")]
    public Image roleImage;

    [Tooltip("角色上方信息")]
    [HideInInspector] public RoleUI roleUI;

    [Tooltip("受伤音频")]
    public AudioClip getHurt;

    [Tooltip("大招音频")]
    public AudioClip bigAudio;

    public Color32 roleColor;

    #endregion

    #region 运行时状态

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Collider2D roleCollider;
    private Canvas roleCanvas;
    private Transform negativeEffectParent;
    private int bigCount;//大招点数
    private float speedPercent = 1;//速度的百分比

    private bool isDie = false;
    public bool canGetWeapon = true;//是否能拾取武器
    public bool canUseBig = true;//是否可以使用技能

    public BaseRole otherBaseRole => GetRandomOther(this);

    public bool CantSelected = false;

    // 旧娱乐机制草稿字段：当前未接入正式逻辑，暂不删除，方便后续回溯。
    private int lashiValue;

    #endregion

    #region Unity 生命周期

    /// <summary>
    /// 初始化角色运行时组件，并启动开局随机推动。
    /// </summary>
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        roleCollider = GetComponent<Collider2D>();
        StartCoroutine(ApplyRandomForce());
        StartGame();
    }

    /// <summary>
    /// 英雄初始化扩展入口；当前没有子类覆盖。
    /// </summary>
    public virtual void StartGame()
    {

    }

    /// <summary>
    /// 按当前全局球速和角色速度百分比持续修正移动速度。
    /// </summary>
    private void FixedUpdate()
    {
        if (isDie)
        {
            return;
        }
        if (VideoGameManager.instance.gameEnd)
        {
            StopMovementWhenGameEnded();
            return;
        }

        UpdateMoveVelocity();
    }

    /// <summary>
    /// 游戏结束时停止角色运动并移除刚体。
    /// </summary>
    private void StopMovementWhenGameEnded()
    {
        if (rb)
        {
            rb.velocity = Vector3.zero;
            Destroy(rb);
        }
    }

    /// <summary>
    /// 按当前速度公式刷新角色移动速度。
    /// </summary>
    private void UpdateMoveVelocity()
    {
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = rb.velocity.normalized;

            // 添加一个非常小的随机偏移，避免轨迹锁死
            Vector2 randomOffset = new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            dir += randomOffset;
            dir.Normalize();

            rb.velocity = dir * VideoGameManager.instance.ballSpeed * speedPercent;
        }
    }

    #endregion

    #region 速度控制

    /// <summary>
    /// 设置角色速度百分比，可选持续时间后恢复为正常速度。
    /// </summary>
    /// <param name="percent">速度百分比，小于 0 时不处理。</param>
    /// <param name="duration">持续时长，为 0 时不会自动恢复。</param>
    public void SetSpeed(float percent, float duration = 0)
    {
        if (percent < 0)
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
    /// 等待指定时长后恢复角色速度百分比。
    /// </summary>
    /// <param name="duration">等待恢复的时长。</param>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator SetSpeedCoro(float duration)
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

    #endregion

    #region 状态限制

    /// <summary>
    /// 设置角色是否可以拾取武器，可选持续时间后恢复。
    /// </summary>
    /// <param name="CanGetWeapon">是否可以拾取武器。</param>
    /// <param name="duration">持续时长，为 0 时不会自动恢复。</param>
    public void SetCanGetWeapon(bool CanGetWeapon, float duration = 0)
    {
        Transform effectParent = PrepareAbilityIconParent(CantGetWeaponIconName);
        canGetWeapon = CanGetWeapon;
        CreateAbilityIcon(VideoGameManager.instance.CantGetWeaponPrefab, effectParent);
        if (duration != 0)
        {
            StartCoroutine(SetCanGetWeaponCoro(duration));
        }
    }

    /// <summary>
    /// 等待指定时长后恢复武器拾取能力。
    /// </summary>
    /// <param name="duration">等待恢复的时长。</param>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator SetCanGetWeaponCoro(float duration)
    {
        yield return new WaitForSeconds(duration);
        RestoreAbilityState(ref canGetWeapon, CantGetWeaponIconName);
        yield break;
    }

    /// <summary>
    /// 设置角色是否可以使用大招，可选持续时间后恢复。
    /// </summary>
    /// <param name="CanUseBig">是否可以使用大招。</param>
    /// <param name="duration">持续时长，为 0 时不会自动恢复。</param>
    public void SetCanUseBig(bool CanUseBig, float duration = 0)
    {
        Transform effectParent = PrepareAbilityIconParent(CantUseBigIconName);
        canUseBig = CanUseBig;
        CreateAbilityIcon(VideoGameManager.instance.CantUseBigPrefab, effectParent);
        if (duration != 0)
        {
            StartCoroutine(SetCanUseBigCoro(duration));
        }
    }

    /// <summary>
    /// 等待指定时长后恢复大招使用能力。
    /// </summary>
    /// <param name="duration">等待恢复的时长。</param>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator SetCanUseBigCoro(float duration)
    {
        yield return new WaitForSeconds(duration);
        RestoreAbilityState(ref canUseBig, CantUseBigIconName);
        yield break;
    }

    /// <summary>
    /// 移除旧状态图标并返回状态图标父节点。
    /// </summary>
    /// <param name="iconName">需要移除的旧状态图标名称。</param>
    /// <returns>状态图标父节点。</returns>
    private Transform PrepareAbilityIconParent(string iconName)
    {
        Transform effectParent = GetNegativeEffectParent();
        RemoveEffectIcon(effectParent, iconName);
        return effectParent;
    }

    /// <summary>
    /// 创建新的状态图标。
    /// </summary>
    /// <param name="iconPrefab">需要创建的状态图标预制体。</param>
    /// <param name="effectParent">状态图标父节点。</param>
    private void CreateAbilityIcon(GameObject iconPrefab, Transform effectParent)
    {
        Instantiate(iconPrefab, effectParent);
    }

    /// <summary>
    /// 恢复状态值并移除状态图标。
    /// </summary>
    /// <param name="state">需要恢复为 true 的状态字段。</param>
    /// <param name="iconName">需要移除的状态图标名称。</param>
    private void RestoreAbilityState(ref bool state, string iconName)
    {
        state = true;
        RemoveEffectIcon(GetNegativeEffectParent(), iconName);
    }

    #endregion

    #region 生命值

    /// <summary>
    /// 恢复角色生命值，最高恢复到 100。
    /// </summary>
    /// <param name="value">恢复的生命值。</param>
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
    /// 让角色受到伤害，并在生命值归零时执行死亡流程。
    /// </summary>
    /// <param name="damage">受到的伤害值。</param>
    public void TakeDamage(int damage)
    {
        if (isDie) { return; }
        HP -= damage;

        roleUI.TakeDamage(HP);
        if (HP <= 0)
        {
            HandleDeath();
        }
        else
        {
            PlayHitFlash();
        }
        PlayAudio(getHurt);
    }

    /// <summary>
    /// 执行角色死亡流程。
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log($"{RoleName}死了");
        HP = 0;
        Broadcast.instance.BroadCastNews($"{RoleName}淘汰", roleColor);
        DisablePhysicsOnDeath();
        // 停止正在进行的颜色动画
        roleImage.DOKill(); // 关键代码
        roleImage.color = new Color32(100, 100, 100, 255);
        Debug.Log($"{RoleName}的image颜色为{roleImage.color}");
        isDie = true;
        GetRoleCanvas().sortingOrder--;
        VideoGameManager.instance.RoleDie(this);
        //Destroy(gameObject); // 下一帧销毁
    }

    /// <summary>
    /// 死亡时停止并移除物理组件。
    /// </summary>
    private void DisablePhysicsOnDeath()
    {
        Rigidbody2D currentRb = rb ? rb : GetComponent<Rigidbody2D>();
        if (currentRb)
        {
            currentRb.velocity = Vector3.zero;
            Destroy(currentRb);
        }
        Collider2D currentCollider = roleCollider ? roleCollider : GetComponent<Collider2D>();
        if (currentCollider)
        {
            Destroy(currentCollider);
        }
    }

    /// <summary>
    /// 未死亡时播放受伤闪红表现。
    /// </summary>
    private void PlayHitFlash()
    {
        Debug.Log($"{RoleName}受到了伤害,HP为{HP}");
        // 闪红效果
        roleImage.DOColor(Color.red, 0.1f)
                 .OnComplete(() => roleImage.DOColor(Color.white, 0.2f));
    }

    #endregion

    #region 大招

    /// <summary>
    /// 增加大招点数，满 3 点时尝试释放大招。
    /// </summary>
    public void AddBig()
    {
        bigCount++;
        ClampBigCountToMax();
        roleUI.AttackOther(transform.position);
        TryUseBigWhenReady();
    }

    /// <summary>
    /// 将大招点数限制在最大值。
    /// </summary>
    private void ClampBigCountToMax()
    {
        if (bigCount >= 3)
        {
            bigCount = 3;
        }
    }

    /// <summary>
    /// 大招点满且允许使用时释放大招。
    /// </summary>
    private void TryUseBigWhenReady()
    {
        if (!canUseBig) return;
        if (bigCount >= 3)
        {
            UseBig();
            bigCount = 0;
        }
    }

    /// <summary>
    /// 播放大招音效并调用具体英雄的大招实现。
    /// </summary>
    public virtual void UseBig()
    {
        Debug.Log("使用大招");
        PlayAudio(bigAudio);
        Big();

        roleUI.ClearBig();
    }

    /// <summary>
    /// 具体英雄的大招实现入口。
    /// </summary>
    public abstract void Big();

    #endregion

    #region 音频

    /// <summary>
    /// 使用角色自身 AudioSource 播放一次音频。
    /// </summary>
    /// <param name="audioClip">需要播放的音频片段。</param>
    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip == null)
            return;
        //audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioClip);
    }

    #endregion

    #region 目标选择

    /// <summary>
    /// 从当前存活角色中随机获取一个非自身目标。
    /// </summary>
    /// <param name="self">需要排除的自身角色。</param>
    /// <returns>随机到的其它角色。</returns>
    public BaseRole GetRandomOther(BaseRole self)
    {
        return VideoGameTargetUtility.GetRandomOther(VideoGameManager.instance.circleRoles, self);
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 开局延迟后给角色施加随机方向冲量。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator ApplyRandomForce()
    {
        yield return new WaitForSeconds(3);//暂时修改
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 获取并缓存角色 Canvas。
    /// </summary>
    /// <returns>角色身上的 Canvas。</returns>
    private Canvas GetRoleCanvas()
    {
        if (roleCanvas == null)
        {
            roleCanvas = transform.Find(CanvasName).GetComponent<Canvas>();
        }

        return roleCanvas;
    }

    /// <summary>
    /// 获取并缓存负面效果图标父节点。
    /// </summary>
    /// <returns>负面效果图标父节点。</returns>
    private Transform GetNegativeEffectParent()
    {
        if (negativeEffectParent == null)
        {
            negativeEffectParent = transform.Find(CanvasName).Find(NegativeEffectName);
        }

        return negativeEffectParent;
    }

    /// <summary>
    /// 移除指定名称的状态图标。
    /// </summary>
    /// <param name="effectParent">状态图标父节点。</param>
    /// <param name="iconName">需要移除的图标名称。</param>
    private void RemoveEffectIcon(Transform effectParent, string iconName)
    {
        Transform icon = effectParent.Find(iconName);
        if (icon)
        {
            Destroy(icon.gameObject);
        }
    }

    #endregion

    #region 碰撞触发

    /// <summary>
    /// 处理大招能量球拾取。
    /// </summary>
    /// <param name="collision">进入触发器的碰撞体。</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBall"))
        {
            HandleBigBallTrigger(collision);
        }
    }

    /// <summary>
    /// 按原顺序处理大招能量球效果。
    /// </summary>
    /// <param name="collision">大招能量球碰撞体。</param>
    private void HandleBigBallTrigger(Collider2D collision)
    {
        AddBig();
        //Broadcast.instance.BroadCastNews($"{RoleName}吃到了能量球", roleColor);
        AudioManager.Instance.PlaySFX("吃到球");
        Destroy(collision.gameObject);
        //VideoGameManager.instance.currentReward.SetActive(false);
        //VideoGameManager.instance.SpawnWeapons();
    }

    /// <summary>
    /// 处理角色碰撞音效。
    /// </summary>
    /// <param name="collision">碰撞信息。</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySFX("撞击");
    }

    #endregion
}
