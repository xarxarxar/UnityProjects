
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public struct AttackerInfo
{
    public int attackerIndex;
    public BeastId beastId;
    public Element attackerElement;
    public BeastClass beastClass;
    public MoveType attackerMoveType;

    public BigInteger atk;
    // damage incr
    public float percentBySkill;
    public BigInteger damage;

    public int skillIndex;//1, 2, 3
    public int curStar;
    public float percentByStar;

    public GameObject effectHit;
    public GameObject effectHitSplash;

    public float critRate;
    public float critFactor;

    public List<AbilityType> abilityTypes;
    public int hitComboTh;
    public AbilityType specialSkill;
    public float effectValue;
    public float specialSkillRate;
    public float specialSkillTime;
    public float AOE;

    public float perDmgWhenFreezeExplode;
    public int casterWorldIndex;

    public AttackerInfo(int index, BeastId beastId, MoveType attackerMoveType, BigInteger atk, float percentBySkill, BigInteger damage, GameObject effectHit,
        float critRate, float critFactor, AbilityType spsk, float effectValue, float spskRate, float spskTime, int skillIndex, int curStar, float percentByStar,
        Element attackerElement, float aoe, int hitComboTh, List<AbilityType> abilityTypes, GameObject effectHitSplash,
        BeastClass beastClass, float perDmgWhenFreezeExplode, int casterWorldIndex)
    {
        this.attackerIndex = index;
        this.beastId = beastId;
        this.attackerMoveType = attackerMoveType;

        this.atk = atk;
        this.percentBySkill = percentBySkill;
        this.damage = damage;

        this.effectHit = effectHit;
        this.critRate = critRate;
        this.critFactor = critFactor;

        this.specialSkill = spsk;
        this.effectValue = effectValue;
        this.specialSkillRate = spskRate;
        this.specialSkillTime = spskTime;

        this.skillIndex = skillIndex;
        this.curStar = curStar;
        this.percentByStar = percentByStar;
        this.attackerElement = attackerElement;
        this.AOE = aoe;

        this.hitComboTh = hitComboTh;
        this.abilityTypes = abilityTypes;
        this.effectHitSplash = effectHitSplash;

        this.beastClass = beastClass;
        this.perDmgWhenFreezeExplode = perDmgWhenFreezeExplode;

        this.casterWorldIndex = casterWorldIndex;
    }

    // public AttackerInfo Clone()
    // {
    //     return new AttackerInfo(this.attackerIndex, this.atk, this.percentBySkill, this.damage,
    //         this.effectHit, this.critRate, this.critFactor, this.specialSkill, this.specialSkillRate, this.specialSkillTime, this.skillIndex,
    //         this.curStar, this.percentByStar);
    // }
}

public struct BulletParam
{
    public float speed;
    public float range;
    public bool hitOnFoot;
    public AttackerInfo attackerInfo;

    public int targetIndex;
    public GameObject skill;

    public BulletParam(float speed, float range, AttackerInfo atkerInfo, int target, bool hitOnFoot, GameObject skill)
    {
        this.speed = speed;
        this.range = range;
        this.attackerInfo = atkerInfo;

        this.targetIndex = target;
        this.hitOnFoot = hitOnFoot;

        this.skill = skill;
    }
}