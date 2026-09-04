using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;
using System.Linq;
using BigIntCSharp = System.Numerics.BigInteger;

public partial class BeastBase : EntityBase
{
    [HideInInspector]
    public int curLaneIndex;

    public BeastId beastId;

    public float speed = 2f;
    public int atk = 45;
    public float attackRate = 1f;// số viên đạn trên s
    public float attackRange = 2f;
    public float viewRange = 3f;

    public float animAttackTS = 1f;
    public float animMoveTs = 0.7f;
    public float critRate = 25f;
    public float critFactor = 2f;

    public TeamBuff teamBuff;
    public float teamBuffValue;
    public float teamBuffChance = 100;
    public Element buffForElement;
    public BeastClass buffForClass = BeastClass.None;

    public BeastClass beastClass;
    public MoveType moveType;
    public GenderType genderType;

    public Rarity rarity;

    // [SerializeField] private float offsetHealthBarBySkill02;

    public List<AbilityModel> abilityModels;

    [Header("Skill Des")]
    public List<SkillDes> skillDes;
    [Header("Sprites")]
    public List<Sprite> icons;

    public SkillBase skill01;
    public SkillBase skill02;

    [HideInInspector]
    public int level;

    [Header("Debug")]
    [HideInInspector]
    public GameObject currentTaget;
    [HideInInspector]
    public int instanceId;
    [HideInInspector]
    public BeastState state;

    [HideInInspector]
    public int curStar;

    [HideInInspector]
    public BigIntCSharp curAtk;

    [HideInInspector]
    public SkeletonAnimation skeletonAnimation;
    private TrackEntry curTrackEntry;
    private Dictionary<Status, ParticleSystem> statusGraphicEffects;
    private IEnumerator attackIE;
    private string nickName;

    private int cntSkill01;

    private int trackIndexMove = 1, trackIndexAtk = 0, trackIndexDie = 2;
    private int trackIndexIdle = 3;

    [HideInInspector]
    public float perDmgWhenFreezeExplode;

    [HideInInspector]
    public float curSpeed, curAttackRate;
    [HideInInspector]
    public Vector3 firstPos;
    [HideInInspector]
    public bool activeOnScreen;

    protected float curAttackRange, curSp2MOD, curCritRate, curCritFactor;

    private float curSpeedTmp, animAttackTsTmp, animMoveTsTmp;
    private DateTime lastAttackTime;
    private int numTakeDamage;
    private int skill01Order, skill02Order;
    protected Dictionary<ShieldType, bool> shieldTypes;

    public BeastBase caster;
    private bool endAttackAnim = true;
    private IEnumerator skill2IE;

    protected override void Awake()
    {
        base.Awake();

        // speed *= 1.5f;
        statusGraphicEffects = new Dictionary<Status, ParticleSystem>();
        attackRate = attackRate >= 1 ? attackRate * 0.7f : attackRate;
        shieldTypes = new Dictionary<ShieldType, bool>();
        listStatus = new List<Status>();
        effectBySkills = new List<EffectBySkill>();

        hp = (int)(hp * GameManager.instance.HpRatio);
        def = (int)(def * GameManager.instance.DefRatio);
        atk = (int)(atk * GameManager.instance.AtkRatio);
    }

    protected override void Start()
    {
        base.Start();

        instanceId = gameObject.GetInstanceID();
    }

    public void Setup(Team team, int lane, int worldIndex, BigIntCSharp newHp, BigIntCSharp newSubHp, BigIntCSharp newDef, BigIntCSharp newAtk, int curStar)
    {
        base.Setup(worldIndex, newHp, newSubHp, newDef);
        nickName = Helper.WordFilt(beastId.ToString(), out int numSpace);

        statusGraphicEffects.Clear();
        shieldTypes.Clear();
        listStatus.Clear();
        effectBySkills.Clear();

        this.caster = null;
        currentTaget = null;
        currentTeam = team;
        cntSkill01 = 0;
        curTrackEntry = null;
        state = BeastState.None;

        curLaneIndex = lane;

        curAtk = newAtk;
        curSpeed = speed;
        curAttackRange = attackRange;
        curAttackRate = attackRate;
        // curSp2MOD = sp2MOD;
        curCritRate = critRate;
        curCritFactor = critFactor;
        // curSpecialSkillTime = specialSkillTime;
        // curSpecialSkillRate = specialSkillRate;

        // animAttackTS = 1f;
        // animMoveTs = curSpeed * 0.5f;

        animAttackTsTmp = animAttackTS;
        animMoveTsTmp = animMoveTs;
        curSpeedTmp = curSpeed;

        this.curStar = curStar;
        Debug.Log("curStarrrrrrrrrrr:" + curStar);

        if (attackIE != null) StopCoroutine(attackIE);

        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
            skeletonAnimation.AnimationState.Complete += HandleAnimationOnComplete;
        }

        var localScale = transform.localScale;
        if (currentTeam == Team.My)
        {
            localScale = new Vector3(-1, 1, 1);
            healthBar.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            localScale = new Vector3(1, 1, 1);
            healthBar.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        transform.localScale = localScale;

        // IncrAtribute(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        numTakeDamage = 0;
    }

    public virtual void Active(bool activeOnScreen, Team team, Vector3 position, int lane, int worldIndex)
    {
        hitGraphicEffects.Clear();
        followEffects.Clear();
        curSubHp = maxSubHP;
        curHp = maxHP;

        isDie = false;
        curIndex = worldIndex;
        if (healthBar != null) healthBar.SetMaxHealth(curHp, team, curSubHp);


        statusGraphicEffects.Clear();
        shieldTypes.Clear();
        listStatus.Clear();
        effectBySkills.Clear();

        this.caster = null;
        currentTaget = null;
        currentTeam = team;
        cntSkill01 = 0;
        curTrackEntry = null;
        state = BeastState.None;

        curLaneIndex = lane;
        transform.position = position;
        this.activeOnScreen = activeOnScreen;

        animAttackTS = animAttackTsTmp;
        animMoveTs = animMoveTsTmp;
        curSpeed = curSpeedTmp;

        if (activeOnScreen)
        {
            if (curStar >= 2)
            {
                float time = 0f;
                if (beastId == BeastId.GrooVine) time = 10f;
                else if (beastId == BeastId.Circle) time = 10f;
                if (time > 0)
                {
                    skill2IE = Skill2(time);
                    StartCoroutine(skill2IE);
                }
            }
        }
        else
        {
            if (skill2IE != null) StopCoroutine(skill2IE);
        }
    }

    private IEnumerator Skill2(float delay)
    {
        // Debug.Log("skill2");
        var originDelay = delay;
        while (true)
        {
            yield return new WaitForSeconds(delay);

            // Debug.Log(name + " cntSkill01:" + cntSkill01);
            while (!CanAttack())
            {
                yield return null;
            }

            while (InSpecialEffect(Status.Stun) || InSpecialEffect(Status.Freeze)
                        || InSpecialEffect(Status.KnockBack) || InSpecialEffect(Status.Root))
            {
                yield return null;
            }
            skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);

            if (beastId == BeastId.Circle)
            {
                var opTeam = currentTeam == Team.My ? Team.Enemy : Team.My;
                var enemies = GameManager.instance.GetBeastInTeam(opTeam);
                enemies = enemies.Where(en => en.beastId != BeastId.Circle && en.caster == null).ToList();
                if (enemies.Count() == 0)
                {
                    delay = 0.1f;
                    continue;
                }
                delay = originDelay;
            }

            endAttackAnim = false;

            curTrackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndexAtk, Constant.SkillPairs[(int)SkillType.skill02][0], false);
            curTrackEntry.TimeScale = animAttackTS;
        }
    }

    public void IncrAtribute(float perHp, float perDef, float perAtk, float perDoge, float perMoveSpeed, float perAttackRange,
        float perAttackRate, float perSp2MOD, float perCritRate, float perCritFactor, float perSpSkTime, float perSpSkRate)
    {
        if (perHp != 0) curHp = curHp + BigIntegerHelper.BigMultiplyFloat(curHp, perHp * 1f / 100);
        if (perDef != 0) curDef = curDef + BigIntegerHelper.BigMultiplyFloat(curDef, perDef * 1f / 100);
        if (perAtk != 0) curAtk = curAtk + BigIntegerHelper.BigMultiplyFloat(curAtk, perAtk * 1f / 100);

        if (perDoge != 0) curDoge = (curDoge + curDoge * perDoge / 100);
        if (perMoveSpeed != 0) curSpeed = (curSpeed + curSpeed * perMoveSpeed / 100);
        if (perMoveSpeed != 0) animMoveTs = (animMoveTs + animMoveTs * perMoveSpeed / 100);

        if (perAttackRange != 0) curAttackRange = (curAttackRange + curAttackRange * perAttackRange / 100);
        if (perAttackRate != 0) curAttackRate = (curAttackRate + curAttackRate * perAttackRate / 100);
        if (perAttackRate != 0) animAttackTS = (animAttackTS + animAttackTS * perAttackRate / 100);

        // if (perSp2MOD != 0) curSp2MOD = (sp2MOD + sp2MOD * perSp2MOD / 100);
        if (perCritRate != 0) curCritRate = (curCritRate + curCritRate * perCritRate / 100);
        if (perCritFactor != 0) curCritFactor = (curCritFactor + curCritFactor * perCritFactor / 100);
        // if (perSpSkTime != 0) curSpecialSkillTime = (specialSkillTime + specialSkillTime * perSpSkTime / 100);
        // if (perSpSkRate != 0) curSpecialSkillRate = (specialSkillRate + specialSkillRate * perSpSkRate / 100);

        // animMoveTs = curSpeed * 0.5f;
        // animAttackTS = 1f;

        animAttackTsTmp = animAttackTS;
        animMoveTsTmp = animMoveTs;
        curSpeedTmp = curSpeed;

        if (beastClass == BeastClass.Knight)
            curSubHp = BigIntegerHelper.BigMultiplyFloat(curHp, 0.2f);
        maxHP = curHp;
        maxSubHP = curSubHp;

        Debug.Log("maxHP:" + maxHP);

        if (healthBar != null) healthBar.SetMaxHealth(curHp, currentTeam, curSubHp);
    }

    public void DebugAtribute(string note)
    {
        Debug.Log(name + " " + curIndex + " " + note + $" curHp:{curHp} curDef:{curDef} curAtk:{curAtk} curDoge:{curDoge} curSpeed:{curSpeed}"
            + $" curAttackRange:{curAttackRange} curAttackRate:{curAttackRate} curSp2MOD:{curSp2MOD} curCritRate:{curCritRate}"
            + $" curCritRate:{curCritRate} curCritFactor:{curCritFactor}");
    }

    private void HandleAnimationOnComplete(TrackEntry trackEntry)
    {
        HandleAnimComplete(trackEntry, 1);
        HandleAnimComplete(trackEntry, 2);
        HandleAnimComplete(trackEntry, 3);

        if (trackEntry.Animation.Name == Constant.AnimDieName)
        {
            OnEndDieAnim();
        }
    }

    private void HandleAnimComplete(TrackEntry trackEntry, int skillIndex)
    {
        // Debug.Log("Id: " + instanceId + " skillIndex: " + skillIndex);
        if (trackEntry.Animation.Name == Constant.SkillPairs[skillIndex][0])
        {
            // Debug.Log("Id: " + instanceId + " skillIndex 1: " + skillIndex);
            curTrackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndexAtk, Constant.SkillPairs[skillIndex][1], false);
            curTrackEntry.TimeScale = animAttackTS;
        }
        else if (trackEntry.Animation.Name == Constant.SkillPairs[skillIndex][1])
        {
            // Debug.Log("Id: " + instanceId + " skillIndex 2: " + skillIndex);
            curTrackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndexAtk, Constant.SkillPairs[skillIndex][2], false);
            curTrackEntry.TimeScale = animAttackTS;
        }
        else if (trackEntry.Animation.Name == Constant.SkillPairs[skillIndex][2])
        {
            endAttackAnim = true;
            if (skillIndex == 2)
            {
                if (healthBar != null) healthBar.ResetY();
            }
        }
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (currentTaget == null || isDie) return;

        if (e.Data.Name == "shoot")
        {
            endAttackAnim = false;
            if (trackEntry.Animation.Name == Constant.SkillPairs[1][1])
            {
                OnSkill01(skeletonAnimation, currentTaget);
            }
            else if (trackEntry.Animation.Name == Constant.SkillPairs[2][1])
            {
                OnSkill02(skeletonAnimation, currentTaget);
            }
            else if (trackEntry.Animation.Name == Constant.SkillPairs[3][1])
            {
                OnSkill03(skeletonAnimation, currentTaget);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, viewRange);
        // Gizmos.DrawLine(transform.position + new Vector3(-viewRange, 0.6f, 0), transform.position + new Vector3(viewRange, 0.6f, 0));

        Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawLine(transform.position + new Vector3(-curAttackRange, 0.3f, 0), transform.position + new Vector3(curAttackRange, 0.3f, 0));

        if (currentTaget != null)
        {
            Gizmos.color = Color.green;
            // Gizmos.DrawWireSphere(currentTaget.transform.position, AOE);
            // Gizmos.DrawLine(transform.position + new Vector3(-AOE, 0.1f, 0), transform.position + new Vector3(AOE, 0.1f, 0));
        }
    }

    protected virtual void Move(GameObject target)
    {
        if (state != BeastState.Move)
        {
            state = BeastState.Move;

            skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexAtk, 0);

            // Debug.Log("id: " + instanceId + " move:" + animMoveTs);
            skeletonAnimation.AnimationState.SetAnimation(trackIndexMove, Constant.AnimMoveName, true).TimeScale = animMoveTs; ;
        }

        var tmpSpeed = (currentTeam == Team.Enemy ? -curSpeed : curSpeed);
        transform.position += new Vector3(tmpSpeed * Time.deltaTime, 0, 0);
    }

    public virtual void Attack(GameObject target)
    {
        if (currentTaget == null || currentTaget.GetComponent<EntityBase>().isDie
            || !GameManager.instance.AllEntities.ContainsKey(currentTaget.GetComponent<EntityBase>().curIndex)) currentTaget = target;

        if (state != BeastState.Attack)
        {
            state = BeastState.Attack;

            if (attackIE != null) StopCoroutine(attackIE);
            attackIE = Skill();
            StartCoroutine(attackIE);
        }
    }

    public void StopAttack()
    {
        state = BeastState.None;
        if (attackIE != null) StopCoroutine(attackIE);
        currentTaget = null;
    }

    private IEnumerator Skill()
    {
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);

        var entityBase = currentTaget.GetComponent<EntityBase>();
        while (
            state == BeastState.Attack && !GameManager.instance.IsGameOver() && currentTaget != null
            && !entityBase.isDie
            && entityBase.currentTeam != this.currentTeam
            && GameManager.instance.AllEntities.ContainsKey(entityBase.curIndex)
            && Mathf.Abs(currentTaget.transform.position.x - transform.position.x) <= curAttackRange
            && (
                (currentTeam == Team.My && entityBase.transform.position.x > transform.position.x)
                || (currentTeam == Team.Enemy && entityBase.transform.position.x < transform.position.x))
            )
        {
            bool started = false;

            // Debug.Log(name + " cntSkill01:" + cntSkill01);

            if (cntSkill01 >= GameManager.instance.fakeSkill2 && curStar >= 2
                && beastId != BeastId.Circle && beastId != BeastId.GrooVine)
            {
                if (CanAttack())
                {
                    cntSkill01 = 0;

                    if (beastId == BeastId.Merlinus)
                    {
                        var myEns = GameManager.instance.GetEntityInTeam(currentTeam, true);
                        // for (int k = 0; k < myEns.Count; k++)
                        // {
                        //     Debug.Log(myEns[k].name + " hp:" + myEns[k].curHp + " maxHp:" + myEns[k].maxHP);
                        // }
                        var myBeasts = myEns.Where(en => en.curIndex != curIndex && en.curHp < en.maxHP).ToList();
                        if (myBeasts.Count == 0) continue;
                    }

                    started = true;
                    endAttackAnim = false;
                    curTrackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndexAtk, Constant.SkillPairs[(int)SkillType.skill02][0], false);
                    curTrackEntry.TimeScale = animAttackTS;
                }
            }
            else if (cntSkill01 >= 0 && CanAttack())
            {
                cntSkill01++;

                started = true;
                endAttackAnim = false;

                curTrackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndexAtk, Constant.SkillPairs[(int)SkillType.skill01][0], false);
                curTrackEntry.TimeScale = animAttackTS;
            }
            // Debug.Log(name + " cntSkill01 2:" + cntSkill01);

            if (started) yield return new WaitForSeconds(1f / curAttackRate);
            else yield return new WaitForSeconds(0.2f);
        }
        currentTaget = null;
        state = BeastState.None;
    }

    protected virtual void OnSkill01(SkeletonAnimation skeleton, GameObject target)
    {
        if (GameManager.instance.offSkill0102) return;

        skill02Order = 0;
        skill01?.Active(1, this, target);
    }

    protected virtual void OnSkill02(SkeletonAnimation skeleton, GameObject target)
    {
        if (GameManager.instance.offSkill0102) return;

        skill02Order++;
        if (beastId == BeastId.GrooVine && skill02Order > 1) return;
        skill02?.Active(skill02Order, this, target);
    }

    protected virtual void OnSkill03(SkeletonAnimation skeleton, GameObject target)
    {
    }

    protected override void Update()
    {
        base.Update();

        if (!activeOnScreen) return;

        SetStatusPosition();

        foreach (var followEffect in followEffects)
        {
            followEffect.transform.position = centerPoint.position;
        }

        // if (GameManager.instance.IsGameOver()) return;

        if (InSpecialEffect(Status.Stun) || InSpecialEffect(Status.Freeze)
            || InSpecialEffect(Status.KnockBack) || InSpecialEffect(Status.Root))
        {
            currentTaget = null;
            return;
        }

        if (state == BeastState.Pause) return;

        if (currentTaget != null)
        {
            var entity = currentTaget.GetComponent<EntityBase>();
            if (!entity.isDie && GameManager.instance.AllEntities.ContainsKey(entity.curIndex)) return;
        }

        currentTaget = null;

        var nearestGo = FindNearestGameObject(out var minDistance);
        // if (name.Contains("Atlantus"))
        //     nearestGo = null;
        if (nearestGo != null)
        {
            // Debug.Log(minDistance <= curAttackRange);
            if (minDistance <= curAttackRange)
            {
                Attack(nearestGo);
            }
            else
            {
                Move(null);
            }
        }
        else
        {
            Move(null);
            // Debug.Log("target null");
        }
    }

    private void LateUpdate()
    {
        if (GameManager.instance.IsGameOver()) return;

        var tmpTransfromPos = transform.position;
        tmpTransfromPos.z = transform.position.y + curIndex * 0.01f;
        transform.position = tmpTransfromPos;
    }

    private GameObject FindNearestGameObject(out float minDistance)
    {
        // GameObject res = GameManager.instance.enemyGate.gameObject;

        // if (currentTeam == Team.Enemy) res = GameManager.instance.myGate.gameObject;

        // minDistance = Mathf.Abs(res.transform.position.x - transform.position.x);

        minDistance = float.MaxValue;
        GameObject res = null;

        var keys = GameManager.instance.AllEntities.Keys;
        for (int i = 0; i < keys.Count; i++)
        {
            var entity = GameManager.instance.AllEntities[keys.ElementAt(i)];

            if (entity != null && !entity.GetComponent<EntityBase>().isDie && entity.currentTeam != currentTeam
                )
            {
                var beast = entity as BeastBase;
                if (moveType == MoveType.Ground && beast != null && beast.moveType == MoveType.Fly
                    && (this.beastClass != BeastClass.Archer && beastClass != BeastClass.Gunner && beastClass != BeastClass.Mage
                    && beastClass != BeastClass.Ranger && beastClass != BeastClass.Support)) continue;

                var distance = Mathf.Abs(entity.transform.position.x - transform.position.x);
                if (minDistance >= distance)
                {
                    if (currentTeam == Team.My && entity.transform.position.x <= transform.position.x
                        || (currentTeam == Team.Enemy && entity.transform.position.x >= transform.position.x))
                        continue;

                    minDistance = distance;
                    res = entity.gameObject;
                }
            }
        }

        return res;
    }

    private bool CanAttack()
    {
        return curTrackEntry == null || curTrackEntry.Animation == null
                 || (
                         curTrackEntry.IsComplete &&
                         (curTrackEntry.Animation.Name == Constant.AnimGetHitName || curTrackEntry.Animation.Name.EndsWith("_3"))
                     );
    }

    public override void Remove()
    {
        // base.Remove();

        // Debug.Log("dieeeeeeeee");
        curSpeed = 0;

        AudioManager.instance.MonsterDie();

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexAtk, 0);
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);
        if (skeletonAnimation.Skeleton.Data.FindAnimation(Constant.AnimDieName) != null)
            skeletonAnimation.AnimationState.SetAnimation(trackIndexDie, Constant.AnimDieName, false).TimeScale = animAttackTS;
        else
            OnEndDieAnim();
    }

    private void OnEndDieAnim()
    {
        for (int i = 0; i < hitGraphicEffects.Count; i++)
        {
            var effect = hitGraphicEffects[i];
            if (effect.isPlaying)
            {
                effect.Stop();
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }
        for (int i = 0; i < followEffects.Count; i++)
        {
            var effect = hitGraphicEffects[i];
            if (effect.isPlaying)
            {
                effect.Stop();
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }
        
        var keys = statusGraphicEffects.Keys;
        for (int i = 0; i < keys.Count(); i++)
        {
            var effect = statusGraphicEffects[keys.ElementAt(i)];
            if (effect.isPlaying)
            {
                effect.Stop();
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexDie, 0);
        skeletonAnimation.skeleton.SetColor(Color.white);

        if (this.caster != null)
        {
            var opTeam = currentTeam == Team.My ? Team.Enemy : Team.My;
            currentTeam = opTeam;
        }

        GameManager.instance.AllEntities.Remove(curIndex);
        GameManager.BeastRemove.Invoke(this);

        // if (currentTeam == Team.Enemy)
        //     ObjectPool.Instance.ReleaseObject(gameObject);
    }

    public void RaiseDamage(GameObject target, AttackParam attackParam, bool ignoreDoge)
    {
        if (target == null || target.GetComponent<EntityBase>().isDie)
        {
            Debug.Log("die or null");
            return;
        }

        var attackedEn = target.GetComponent<EntityBase>();
        RaiseDamage(attackedEn, attackParam, ignoreDoge);
    }

    public BigIntCSharp RaiseDamage(EntityBase attackedEn, AttackParam attackParam, bool ignoreDoge)
    {
        if (attackedEn.currentTeam != currentTeam)
        {
            var attackerInfoClone = attackParam.attackerInfo;
            // attackParam.attackerInfo.critFactor = StatHelper.GetCritFactor(curCritRate, curCritFactor);

            attackParam.attackerInfo.damage = GetDamage(attackerInfoClone, attackedEn, out var critFactor);
            attackParam.attackerInfo.critFactor = critFactor;

            return attackedEn.TakeDamage(attackParam, ignoreDoge);
        }
        return 0;
    }

    public override BigIntCSharp TakeDamage(AttackParam attackParam, bool isSplash)
    {
        if (isDie || GameManager.instance.IsGameOver()) return 0;
        var attackerInfo = attackParam.attackerInfo;

        var attacker = GameManager.instance.GetEntity(attackerInfo.attackerIndex);
        if (attacker == null || attacker.currentTeam == currentTeam) return 0;

        if (AbilityAvoidDamage(attackParam.attackerInfo)) return 0;



        // if (name.Contains("Atlantus"))
        // {
        //     Debug.Log("Atlantus:" + attackerInfo.damage);
        // }
        int rd;

        bool isDoged = false;
        var damage = attackerInfo.damage;
        if (!attackParam.alwayHit)
        {
            rd = UnityEngine.Random.Range(0, 100);
            if (!isSplash)
            {
                var exist = abilityModels.Exists(am => am.abilityType == AbilityType.Evade || am.abilityType == AbilityType.BulletEvade);
                if (rd < curDoge && exist)
                {
                    damage = 0;
                    isDoged = true;
                }
            }
            else// bị đánh lan
            {
                var abilityModel = GetAbilityModel(AbilityType.EvadeSplashDamage);
                if (abilityModel != null)
                {
                    if (rd < abilityModel.chance)
                    {
                        healthBar.PushMessage("闪避");
                        damage = 0;
                    }
                }
            }
        }


        if (attackerInfo.abilityTypes.Contains(AbilityType.CriticalDamageFlyingTargets) && moveType == MoveType.Fly)
            attackerInfo.critRate = 100;
        rd = UnityEngine.Random.Range(1, 101);
        if (rd > attackerInfo.critRate || attackerInfo.critFactor <= 0) attackerInfo.critFactor = 1;

        int totalDmg = 0;
        for (int k = 0; k < attackerInfo.critFactor; k++)
            totalDmg += (int)damage;

        // Debug.Log(name + " isDoged:" + isDoged + " damage:" + damage + " critFactor:" + attackerInfo.critFactor);

        // shields
        if (shieldTypes.ContainsKey(ShieldType.ShieldMerlinus))
        {
            var myEns = GameManager.instance.GetEntityInTeam(currentTeam, true);
            foreach (var myEn in myEns)
            {
                var myBeast = myEn as BeastBase;
                if (myBeast.beastId == BeastId.Merlinus && beastId != BeastId.Merlinus)
                {
                    var hpShield = myBeast.maxHP;
                    totalDmg = totalDmg > hpShield ? totalDmg - (int)hpShield : 0;
                    if (attackerInfo.attackerElement == Element.Light)
                        totalDmg = (int)(totalDmg * 0.5f);
                    break;
                }
            }
        }

        if (curSubHp > 0) curSubHp -= totalDmg;
        else curHp -= totalDmg;

        if (totalDmg > 0)
        {
            BeastBase bb = null;
            // var caster = GameManager.instance.GetEntity(attackerInfo.casterWorldIndex);
            if (this.caster != null)
            {
                Debug.Log(name + " Caster is:" + caster.gameObject.name + " team:" + caster.currentTeam);
                bb = caster as BeastBase;
            }
            else
            {
                Debug.Log(name + " no caster :" + this.gameObject.name + " team:" + this.currentTeam);
                bb = this as BeastBase;
            }
            if (bb != null)
            {
                // Debug.Log(name + "attacked damage:" + totalDmg);
                GameManager.instance.UpdateBeastItemView(bb.currentTeam, bb.beastId, 0, totalDmg, curHp <= 0);
            }
        }

        if (totalDmg > 0)
        {
            var caster = GameManager.instance.GetEntity(attackerInfo.casterWorldIndex);
            if (caster != null)
            {
                var bb = caster as BeastBase;
                if (bb != null) GameManager.instance.UpdateBeastItemView(bb.currentTeam, bb.beastId, totalDmg, 0, false);
            }
            else
            {
                GameManager.instance.UpdateBeastItemView(attacker.currentTeam, attackerInfo.beastId, totalDmg, 0, false);
            }
        }

        // var attacker = GameManager.instance.GetEntity(attackParam.attackerInfo.attackerIndex);
        // if (totalDmg > 0)
        // {
        // Debug.Log(attacker.name + "attacker damage:" + totalDmg);
        //     GameManager.instance.UpdateBeastItemView(this.currentTeam == Team.My ? Team.Enemy : Team.My, attackerInfo.beastId, totalDmg, 0, false);
        // }

        OnTakeDamage(attackParam, isDoged, this.element);

        if (curHp <= 0)
        {
            isDie = true;
            Remove();
        }

        return totalDmg;
    }

    protected int GetHitComboTh()
    {
        var abilityModel = GetAbilityModel(AbilityType.HitCombo);
        if (abilityModel != null)
        {
            if (lastAttackTime == default) lastAttackTime = DateTime.Now;
            var delTime = (DateTime.Now - lastAttackTime).TotalSeconds;
            if (delTime <= abilityModel.time)
            {
                numTakeDamage++;
            }
            else
            {
                numTakeDamage = 1;
            }
            if (numTakeDamage >= 30) numTakeDamage = 30;
            healthBar.PushMessage("连击×" + numTakeDamage);
            lastAttackTime = DateTime.Now;

            // Debug.Log("lastAttackTime:" + lastAttackTime);

            return numTakeDamage;
        }
        return 0;
    }

    private bool AbilityAvoidDamage(AttackerInfo attackerInfo)
    {
        var ats = abilityModels.Select(abc => abc.abilityType);
        // if (ats.Contains(AbilityType.CriticalDamageFlyingTargets) && attackerInfo.attackerMoveType == MoveType.Fly)
        // {
        //     healthBar.OnSpecialSkill(AbilityType.CriticalDamageFlyingTargets);
        //     return true;
        // }
        if (ats.Contains(AbilityType.BulletEvade) && attackerInfo.beastClass == BeastClass.Gunner)
        {
            var ability = abilityModels.FirstOrDefault(ab => ab.abilityType == AbilityType.BulletEvade);
            if (ability != null && ability.chance >= UnityEngine.Random.Range(1, 101))
            {
                healthBar.OnSpecialSkill(AbilityType.BulletEvade);
                return true;
            }
        }
        if (ats.Contains(AbilityType.Evade))
        {
            var ability = abilityModels.FirstOrDefault(ab => ab.abilityType == AbilityType.Evade);
            if (ability != null && ability.chance >= UnityEngine.Random.Range(1, 101))
            {
                healthBar.OnSpecialSkill(AbilityType.Evade);
                return true;
            }
        }
        return false;
    }

    public AbilityModel GetAbilityModel(AbilityType abilityType)
    {
        var ats = abilityModels.FirstOrDefault(abc => abc.abilityType == abilityType);
        return ats;
    }

    protected override void OnTakeDamage(AttackParam attackParam, bool isDodged, Element attackedElement)
    {
        // base.OnTakeDamage(effectPref, collidePosition);
        // Debug.Log("attackerInfo.damage:" + attackerInfo.damage);

        ShowHealthBar(attackParam.attackerInfo.critFactor > 1, isDodged, attackParam.attackerInfo.attackerElement, this.element);

        if (attackParam.attackerInfo.damage != 0)
        {
            // Debug.Log(name + " OnTakeDamage:" + effectPref.name);
            if (skeletonAnimation.skeleton.GetColor() != Color.red)
            {
                skeletonAnimation.skeleton.SetColor(Color.red);
                StartCoroutine(RemoveColor());

            }
        }

        if (attackParam.attackerInfo.effectHit != null)
        {
            // Debug.Log("OnTakeDamage:" + gameObject.name + " effect:" + effectPref.name);
            if (attackParam.collidePosition != Constant.Vector3Default)
            {
                GenGraphicEffect(attackParam.attackerInfo.effectHit, attackParam.collidePosition);
            }
            else
            {
                var targetTip = skeletonAnimation.skeleton.FindBone("target_tip");
                if (targetTip != null)
                {
                    var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));
                    // Debug.Log("OnTakeDamage:" + gameObject.name + " targetTip:" + startPos);
                    GenGraphicEffect(attackParam.attackerInfo.effectHit, startPos);
                }
                else
                {
                    // Debug.Log("OnTakeDamage:" + gameObject.name + " centerPoint.position:" + centerPoint.position);
                    GenGraphicEffect(attackParam.attackerInfo.effectHit, centerPoint.position);
                }
            }
        }

        AttackedBySpecialSkill(attackParam);
    }

    private void ShowHealthBar(bool critical, bool doge, Element attackerElement, Element attackedElement)
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            var hp_tip = skeletonAnimation.skeleton.FindBone("hp_tip");
            if (hp_tip != null)
            {
                var hpTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(hp_tip.WorldX, hp_tip.WorldY, 0f));
                healthBar.SetHealth(hpTipPos, curHp, curSubHp, critical, doge, attackerElement, attackedElement);
            }
            else
            {
                healthBar.SetHealth(Constant.Vector3Default, curHp, curSubHp, critical, doge, attackerElement, attackedElement);
                // if (curTrackEntry != null && curTrackEntry.animation != null
                //                 && curTrackEntry.Animation.Name == Constant.SkillPairs[2][1])
                //     healthBar.SetHealth(curHp, offsetHealthBarBySkill02, critical, doge,
                //         attackerElement, attackedElement);
                // else
                //     healthBar.SetHealth(curHp, 0, critical, doge, attackerElement, attackedElement);
            }
        }
    }

    private IEnumerator RemoveColor()
    {
        yield return new WaitForSeconds(0.1f);

        skeletonAnimation.skeleton.SetColor(Color.white);
    }

    public AttackerInfo GetAttackerInfo(GameObject effect, GameObject effectHitSplash, int skillIndex, bool onHitCombo = true)
    {
        var abilityTypes = abilityModels?.Select(am => am.abilityType).ToList();

        BigIntCSharp atkTmp = this.curAtk;
        var aps = effectBySkills.FirstOrDefault(ebs => ebs.abilityType == AbilityType.AtkPerSec);
        if (aps != null)
        {
            Debug.Log("GetAttackerInfo atkTmp 1:" + atkTmp);

            if (aps.dateTime > DateTime.Now)
                atkTmp = BigIntegerHelper.BigMultiplyFloat(atkTmp, (1 + aps.value * 0.01f));
            else effectBySkills.Remove(aps);

            Debug.Log("GetAttackerInfo atkTmp 2:" + atkTmp);
        }
        var skDes = skillDes[skillIndex - 1];

        // Debug.Log(name + "skillIndex:" + skillIndex);

        var attackerInfo = new AttackerInfo(this.curIndex, beastId, moveType, atkTmp, skDes.percentDmgBySkill, 0, effect,
            this.curCritRate, this.curCritFactor,
            skDes.effect, skDes.effectValue, skDes.chance, skDes.effectTime,
            skillIndex, curStar, skDes.percentDmgByStar,
            this.element, skDes.AOE, onHitCombo ? GetHitComboTh() : 0, abilityTypes,
            effectHitSplash, this.beastClass, this.perDmgWhenFreezeExplode,
            this.caster == null ? 0 : this.caster.curIndex);

        return attackerInfo;
    }

    private bool InSpecialEffect(Status bsk)
    {
        return listStatus.Contains(bsk);
    }

    public void StopAll()
    {
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexAtk, 0);
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexDie, 0);
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
    }

    public void OnShield(ShieldType shieldType, GameObject shieldPref, float time)
    {
        StartCoroutine(OnShieldIE(shieldType, shieldPref, time));
    }

    protected IEnumerator OnShieldIE(ShieldType shieldType, GameObject shieldPref, float time)
    {
        if (shieldTypes.ContainsKey(shieldType)) yield break;
        shieldTypes.Add(shieldType, true);

        // var targetTip = skeletonAnimation.skeleton.FindBone("target_tip");
        var position = centerPoint.position;
        // if (targetTip != null) position = skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));
        var effGo = ObjectPool.Instance.GetGameObject(shieldPref, position, Quaternion.identity);
        // Debug.Log("GenEffect: " + effGo.activeSelf + " goName:" + gameObject.name + " effectPos:" + position);

        var shieldEff = effGo.GetComponent<ParticleSystem>();
        shieldEff.Play();

        followEffects.Add(shieldEff);

        yield return new WaitForSeconds(time);

        shieldEff.Stop();

        followEffects.Remove(shieldEff);

        ObjectPool.Instance.ReleaseObject(shieldEff.gameObject);
        shieldEff = null;

        shieldTypes.Remove(shieldType);
    }

    public void ConvertTeam(BeastBase caster, float delay)
    {
        if (this.caster != null) return;
        StartCoroutine(ConvertTeamIE(caster, delay));
    }

    IEnumerator ConvertTeamIE(BeastBase caster, float delay)
    {
        while (!endAttackAnim)
        {
            yield return null;
        }
        StopAttack();
        state = BeastState.Pause;

        yield return new WaitForSeconds(0.1f);
        this.caster = caster;

        var oldTeam = currentTeam;
        var opTeam = currentTeam == Team.My ? Team.Enemy : Team.My;

        currentTeam = opTeam;
        var curLocalScale = transform.localScale;
        transform.localScale = new Vector3(-curLocalScale.x, curLocalScale.y, curLocalScale.z);

        state = BeastState.None;

        Debug.Log(name + " Start ConvertTeam currentTeam:" + currentTeam + " oldTeam:" + oldTeam);

        yield return new WaitForSeconds(delay);

        // Debug.Log("End ConvertTeam 1");
        while (!endAttackAnim)
        {
            yield return null;
        }
        StopAttack();
        state = BeastState.Pause;

        yield return new WaitForSeconds(0.1f);

        // Debug.Log("End ConvertTeam 2");
        Debug.Log(name + " End ConvertTeam currentTeam:" + currentTeam + " oldTeam:" + oldTeam);

        state = BeastState.None;

        currentTeam = oldTeam;
        transform.localScale = curLocalScale;

        this.caster = null;
    }
}
