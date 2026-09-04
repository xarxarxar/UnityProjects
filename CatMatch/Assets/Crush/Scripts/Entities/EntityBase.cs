

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public class EntityBase : MonoBehaviour
{
    public Transform centerPoint;
    public HealthBar healthBar;
    public Transform statusContainer;
    public GameObject statusPref;
    public Transform beforePoint;

    public int hp = 500;
    public int subHp = 0;

    public int def = 10;
    public float doge = 10f;
    public float dogeEffect = 10f;
    public Element element;
    public EntityType entityType = EntityType.Beast;

    [HideInInspector]
    public bool isDie;

    [HideInInspector]
    public int curIndex;

    [HideInInspector]
    public Team currentTeam;
    [HideInInspector]
    public BigInteger curHp;
    [HideInInspector]
    public BigInteger curSubHp;
    [HideInInspector]
    public BigInteger curDef;
    protected float curDoge;
    protected float curDogeEffect;

    protected List<ParticleSystem> hitGraphicEffects;
    protected List<ParticleSystem> followEffects;

    public BigIntCSharp maxHP;
    protected BigIntCSharp maxSubHP;

    protected virtual void Awake()
    {
        hitGraphicEffects = new List<ParticleSystem>();
        followEffects = new List<ParticleSystem>();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        for (int i = 0; i < hitGraphicEffects.Count; i++)
        {
            var effect = hitGraphicEffects[i];
            if (effect.isStopped)
            {
                hitGraphicEffects.Remove(effect);
                i--;
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }
    }

    public virtual void Setup(int index, BigIntCSharp newHp, BigIntCSharp newSubHp, BigIntCSharp newDef)
    {
        hitGraphicEffects.Clear();
        followEffects.Clear();

        curHp = newHp;
        curDef = newDef;
        curDoge = doge;
        curDogeEffect = dogeEffect;
        curSubHp = newSubHp;

        maxHP = newHp;
        maxSubHP = newSubHp;

        isDie = false;
        curIndex = index;

        if (healthBar != null) healthBar.SetMaxHealth(curHp, currentTeam, curSubHp);
    }

    // ignoreDoge: true khi bị đánh lan
    public virtual BigIntCSharp TakeDamage(AttackParam attackParam, bool isSplash)
    {
        if (isDie || GameManager.instance.IsGameOver()) return 0;

        // var dogeRd = UnityEngine.Random.Range(0, 100);
        // if (!attackParam.alwayHit && dogeRd < curDoge)
        // {
        //     attackParam.attackerInfo.damage = 0;
        // }

        // for (int k = 0; k < attackParam.attackerInfo.critFactor; k++)
        // if (attackParam.attackerInfo.abilityTypes.Contains(AbilityType.CriticalDamageFlyingTargets) && moveType == MoveType.Fly)
        //     attackerInfo.critRate = 100;
        var rd = UnityEngine.Random.Range(1, 101);
        if (rd > attackParam.attackerInfo.critRate || attackParam.attackerInfo.critFactor <= 0)
            attackParam.attackerInfo.critFactor = 1;

        var dmg = (int)attackParam.attackerInfo.damage;
        var totalDmg = 0;
        for (int k = 0; k < attackParam.attackerInfo.critFactor; k++)
            totalDmg += dmg;

        if (curSubHp > 0) curSubHp -= totalDmg;
        else curHp -= dmg;

        // Debug.Log("dmg:" + dmg + " curHp:" + curHp);

        if (dmg > 0)
        {
            BeastBase bb = null;
            // var caster = GameManager.instance.GetEntity(attackParam.attackerInfo.casterWorldIndex);
            // if (caster != null) bb = caster as BeastBase;
            // else
            bb = this as BeastBase;
            if (bb != null)
            {
                // Debug.Log(name + "attacked damage:" + totalDmg);
                GameManager.instance.UpdateBeastItemView(bb.currentTeam, bb.beastId, 0, totalDmg, curHp <= 0);
            }
        }

        if (dmg > 0)
        {
            var caster = GameManager.instance.GetEntity(attackParam.attackerInfo.casterWorldIndex);
            if (caster != null)
            {
                var bb = caster as BeastBase;
                if (bb != null)
                {
                    GameManager.instance.UpdateBeastItemView(bb.currentTeam, bb.beastId, totalDmg, 0, false);
                }
            }
            else
            {
                var attacker = GameManager.instance.GetEntity(attackParam.attackerInfo.attackerIndex);
                if (attacker != null)
                    GameManager.instance.UpdateBeastItemView(attacker.currentTeam, attackParam.attackerInfo.beastId, totalDmg, 0, false);
            }
        }

        OnTakeDamage(attackParam, dmg == 0, this.element);

        if (curHp <= 0)
        {
            isDie = true;

            Remove();
        }
        return dmg;
    }

    // protected virtual void OnTakeDamage(float dmg, GameObject effectPef, Vector3 collidePosition, bool critical,
    //     bool doge, SpecialSkill spSk, float spSkRate, float spSkTime)
    protected virtual void OnTakeDamage(AttackParam attackParam, bool isDodged, Element attackedElement)
    {
        if (attackParam.attackerInfo.effectHit != null)
        {
            // Debug.Log("OnTakeDamage:" + gameObject.name + " effect:" + effectPref.name);
            if (attackParam.collidePosition != Constant.Vector3Default)
            {
                GenGraphicEffect(attackParam.attackerInfo.effectHit, attackParam.collidePosition);
            }
            else
            {
                // Debug.Log("OnTakeDamage:" + gameObject.name + " centerPoint.position:" + centerPoint.position);
                GenGraphicEffect(attackParam.attackerInfo.effectHit, centerPoint.position);
            }
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            var attackerInfo = attackParam.attackerInfo;
            healthBar.SetHealth(Constant.Vector3Default, curHp, curSubHp, attackerInfo.critFactor > 1, isDodged, attackParam.attackerInfo.attackerElement, attackedElement);
        }
    }

    public virtual void Remove()
    {
        // Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision2D)
    {
        // if (collision2D.CompareTag("Bullet"))
        // {
        //     var bullet = collision2D.GetComponent<BulletBase>();
        //     var attackerInfo = bullet.bulletParam.attackerInfo;
        //     // var entityTarget = GameManager.instance.GetEntity(bullet.bulletParam.targetIndex);
        //     if (attackerInfo.AOE == 0f)
        //     {
        //         if (bullet.bulletTeam != currentTeam && !isDie)
        //         {
        //             attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
        //             attackerInfo.damage = GetDamage(attackerInfo, this.curDef, this.element);
        //             // Debug.Log("attackerInfo.damage:" + attackerInfo.damage);

        //             var attackParam = new AttackParam(bullet.transform.position, attackerInfo);
        //             TakeDamage(attackParam, false);
        //         }
        //     }
        //     else
        //     {
        //         var colliders = Physics2D.OverlapCircleAll(transform.position, attackerInfo.AOE);
        //         for (int i = 0; i < colliders.Length; i++)
        //         {
        //             var entityBase = colliders[i].gameObject.GetComponent<EntityBase>();
        //             if (entityBase != null && bullet.bulletTeam != entityBase.currentTeam && !entityBase.isDie)
        //             {
        //                 attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
        //                 attackerInfo.damage = GetDamage(attackerInfo, entityBase.curDef, entityBase.element);

        //                 var attackParam = new AttackParam(bullet.transform.position, attackerInfo);
        //                 entityBase.TakeDamage(attackParam, this != entityBase);
        //             }
        //         }
        //     }
        // }
    }

    protected ParticleSystem GenGraphicEffect(GameObject effectPref, UnityEngine.Vector3 position, bool needRemove = false)
    {
        var effGo = ObjectPool.Instance.GetGameObject(effectPref, position, UnityEngine.Quaternion.identity);
        // Debug.Log("GenEffect: " + effGo.activeSelf + " goName:" + gameObject.name + " effectPos:" + position);
        var sc = effGo.transform.localScale;
        if (currentTeam == Team.Enemy)
            sc.x = Math.Abs(sc.x);
        else
            sc.x = -Math.Abs(sc.x);
        effGo.transform.localScale = sc;

        var eff = effGo.GetComponent<ParticleSystem>();
        eff.Play();
        if (!needRemove) hitGraphicEffects.Add(eff);
        return eff;
    }

    protected virtual BigIntCSharp GetDamage(AttackerInfo attackerInfo, EntityBase attackedEn, out float critFactor)
    {
        var attackedDef = attackedEn.curDef;
        if (attackerInfo.abilityTypes != null && attackerInfo.abilityTypes.Contains(AbilityType.Piercing))
            attackedDef = 0;

        var attackerElement = attackerInfo.attackerElement;
        if (currentTeam == Team.My)
        {
            // use skill change element
            if (GameManager.instance.MyTeamElement != Element.None)
                attackerElement = GameManager.instance.MyTeamElement;
        }

        if (attackerInfo.abilityTypes.Contains(AbilityType.CriticalDamageFlyingTargets))
        {
            var attackedBeast = attackedEn as BeastBase;
            if (attackedBeast != null && attackedBeast.moveType == MoveType.Fly)
                attackerInfo.critFactor += 100;
        }

        attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
        critFactor = attackerInfo.critFactor;

        var dmg = StatHelper.GetDamageSkill(attackerInfo.atk, attackedDef,
                     attackerInfo.percentBySkill, attackerInfo.percentByStar,
                     attackerInfo.skillIndex, attackerInfo.curStar, attackerInfo.critFactor,
                     attackerElement, attackedEn.element);
        if (attackerInfo.hitComboTh > 0)
        {
            // var vl = Mathf.Pow(1.05f, attackerInfo.hitComboTh);
            // dmg = BigIntegerHelper.BigMultiplyFloat(dmg, vl);

            var vl = BigIntegerHelper.Pow2((decimal)1.05f, attackerInfo.hitComboTh);
            dmg = dmg * vl.numerator / vl.denominator;
        }
        return dmg;
    }

    public UnityEngine.Vector3 GetCenterWorldPos()
    {
        if (entityType == EntityType.Gate)
            return centerPoint.position;
        else
        {
            var beast = this as BeastBase;
            var target_tip = beast.skeletonAnimation.skeleton.FindBone("target_tip");
            if (target_tip != null)
            {
                var targetTipPos = beast.skeletonAnimation.transform.TransformPoint(new UnityEngine.Vector3(target_tip.WorldX, target_tip.WorldY, 0f));
                return targetTipPos;
            }
        }
        return centerPoint.position;
    }

    public UnityEngine.Vector3 GetCenterLocalPos()
    {
        if (entityType == EntityType.Gate)
            return centerPoint.localPosition;
        else
        {
            var beast = this as BeastBase;
            var target_tip = beast.skeletonAnimation.skeleton.FindBone("target_tip");
            if (target_tip != null)
            {
                var targetTipPos = new UnityEngine.Vector3(target_tip.X, target_tip.Y, 0f);
                return targetTipPos;
            }
        }
        return centerPoint.localPosition;
    }

    public void AddHp(BigIntCSharp hp)
    {
        curHp += hp;
        if (curHp > maxHP) curHp = maxHP;
    }
}