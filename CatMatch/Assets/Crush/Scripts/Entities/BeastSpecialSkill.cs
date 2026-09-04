using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;
using System.Linq;
using UnityEngine.UI;

enum EffectPos
{
    HpTip,
    TargetTip
}

public partial class BeastBase : EntityBase
{
    Dictionary<AbilityType, AbilityType> antiPairs = new Dictionary<AbilityType, AbilityType>()
    {
        {AbilityType.Burn, AbilityType.AntiBurn},{AbilityType.Freeze, AbilityType.AntiFreeze},
        {AbilityType.KnockBack, AbilityType.AntiKnockBack},{AbilityType.Poison, AbilityType.AntiPoison},
        {AbilityType.Stun, AbilityType.AntiStun}
    };

    private IEnumerator stunIE, poisonIE, burnIE, freezeExplodeIE, freezeIE, knockBackIE, slowdowIE, slowSpeedIE;
    private IEnumerator rootIE;
    private bool inStun, inPoinson, inBurn, inFreezeExplode, inFreeze, inKockBack, inSlowdow, inSlowSpeed, inRoot;

    private List<Status> listStatus;
    private float timeStun;
    [HideInInspector]
    public List<EffectBySkill> effectBySkills;

    private void AttackedBySpecialSkill(AttackParam attackParam)
    {
        // if (attackParam.attackerInfo.damage != 0)
        // {
        // Debug.Log(attackParam.attackerInfo.specialSkill + " rate:" + attackParam.attackerInfo.specialSkillRate);

        if (attackParam.attackerInfo.specialSkill != AbilityType.None)
        {
            var rd = UnityEngine.Random.Range(0, 100);
            // Debug.Log(attackParam.attackerInfo.specialSkill + " rd Doge Effect:" + rd);

            if (rd < curDogeEffect)
                return;

            rd = UnityEngine.Random.Range(0, 100);
            // Debug.Log(attackParam.attackerInfo.skillIndex + " random:" + rd);

            if (rd < attackParam.attackerInfo.specialSkillRate)
            {
                // Debug.Log(name + " AttackedBySpecialSkill:" + attackParam.attackerInfo.specialSkill);

                if (HasAnti(attackParam.attackerInfo.specialSkill, out var anti))
                {
                    healthBar.OnSpecialSkill(anti);
                    return;
                }

                var startNew = false;
                switch (attackParam.attackerInfo.specialSkill)
                {
                    case AbilityType.Stun:
                        startNew = Stun(attackParam);
                        break;
                    case AbilityType.Poison:
                        startNew = Poison(attackParam);
                        break;
                    case AbilityType.Burn:
                        startNew = Burn(attackParam);
                        break;
                    case AbilityType.Freeze:
                        startNew = Freeze(attackParam);
                        break;
                    case AbilityType.FreezeExplode:
                        startNew = FreezeExplode(attackParam);
                        break;
                    case AbilityType.KnockBack:
                        startNew = KnockBack(attackParam);
                        break;
                    case AbilityType.Slowdown:
                        startNew = SlowDown(attackParam);
                        break;
                    case AbilityType.Root:
                        startNew = Root(attackParam);
                        break;
                    case AbilityType.AtkPerSec:
                        var aps = effectBySkills.FirstOrDefault(ebs => ebs.abilityType == AbilityType.AtkPerSec);
                        if (aps == null)
                        {
                            effectBySkills.Add(new EffectBySkill()
                            {
                                abilityType = AbilityType.AtkPerSec,
                                dateTime = DateTime.Now.AddSeconds(attackParam.attackerInfo.specialSkillTime),
                                value = attackParam.attackerInfo.effectValue
                            });
                        }
                        break;
                }
                if (healthBar != null && startNew
                    &&
                    (
                        attackParam.attackerInfo.specialSkill != AbilityType.Poison
                        && attackParam.attackerInfo.specialSkill != AbilityType.Burn
                        && attackParam.attackerInfo.specialSkill != AbilityType.Slowdown
                        && attackParam.attackerInfo.specialSkill != AbilityType.Freeze
                         && attackParam.attackerInfo.specialSkill != AbilityType.FreezeExplode
                        && attackParam.attackerInfo.specialSkill != AbilityType.Stun
                    )
                 )
                {
                    healthBar.OnSpecialSkill(attackParam.attackerInfo.specialSkill);
                }
            }
        }
        // }
    }

    private bool HasAnti(AbilityType abilityType, out AbilityType antiAbility)
    {
        antiAbility = AbilityType.None;
        if (!antiPairs.ContainsKey(abilityType))
        {
            return false;
        }

        var anti = antiPairs[abilityType];
        var ats = abilityModels.Select(abc => abc.abilityType);
        if (ats.Contains(anti))
        {
            antiAbility = anti;
            return true;
        }
        return false;
    }

    private void AddStatusUI(Status status)
    {
        listStatus.Add(status);
        var sprite = GameData.Instance.statusSprites.GetStatusSprite(status);
        if (sprite != null && statusPref != null)
        {
            GameObject go = null;
            BeastStatusItemView bsIV = null;
            for (int k = 0; k < statusContainer.childCount; k++)
            {
                go = statusContainer.GetChild(k).gameObject;
                bsIV = go.GetComponent<BeastStatusItemView>();
                if (bsIV.beastStatus == status)
                    return;
            }

            go = ObjectPool.Instance.GetGameObject(statusPref, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(statusContainer);
            go.transform.localScale = new Vector3(1, 1, 1);

            bsIV = go.GetComponent<BeastStatusItemView>();
            bsIV.beastStatus = status;
            bsIV.image.sprite = sprite;
        }
    }

    private void RemoveStatusUI(Status status)
    {
        var sid = listStatus.IndexOf(status);
        if (sid < 0) return;
        listStatus.RemoveAt(sid);
        if (listStatus.Contains(status)) return;

        var sprite = GameData.Instance.statusSprites.GetStatusSprite(status);
        if (sprite == null || statusPref == null) return;

        for (int k = 0; k < statusContainer.childCount; k++)
        {
            var go = statusContainer.GetChild(k).gameObject;
            var bsIV = go.GetComponent<BeastStatusItemView>();
            if (bsIV.beastStatus == status)
                ObjectPool.Instance.ReleaseObject(go);
        }
    }

    private void AddStatusParticleEffect(Status status, EffectPos effectPos)
    {
        Bone targetTip = null;
        if (effectPos == EffectPos.TargetTip) targetTip = skeletonAnimation.skeleton.FindBone("target_tip");
        if (effectPos == EffectPos.HpTip) targetTip = skeletonAnimation.skeleton.FindBone("hp_tip");

        var effect = GameData.Instance.statusSprites.GetStatusPref(status);
        if (targetTip != null)
        {
            var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));
            // Debug.Log("OnTakeDamage:" + gameObject.name + " targetTip:" + startPos);
            var eff = GenGraphicEffect(effect, startPos, true);
            statusGraphicEffects.Add(status, eff);
        }
        else
        {
            // Debug.Log("OnTakeDamage:" + gameObject.name + " centerPoint.position:" + centerPoint.position);
            float delY = 0;
            if (effectPos == EffectPos.HpTip)
                delY = (centerPoint.position - transform.position).y;
            var eff = GenGraphicEffect(effect, centerPoint.position + new Vector3(0, delY, 0), true);
            statusGraphicEffects.Add(status, eff);
        }
    }

    private void FreezeExplodeEffect(AttackParam attackParam)
    {
        var targetTip = skeletonAnimation.skeleton.FindBone("target_tip");
        var effect = GameData.Instance.statusSprites.GetStatusPref(Status.FreezeExplode);
        var pos = centerPoint.position;
        if (targetTip != null) pos = skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));
        GenGraphicEffect(effect, pos, false);

        var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            if (collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                var enemy = collider.GetComponent<EntityBase>();


                if (enemy != null && !enemy.isDie && enemy.currentTeam == currentTeam)
                {
                    Debug.Log(this.name + " FreezeExplodeIE attacked: " + enemy.gameObject.name);

                    enemy.TakeDamage(attackParam, true);
                }
            }
        }
    }

    void RemoveStatusParticleEffect(Status status)
    {
        if (!statusGraphicEffects.ContainsKey(status)) return;

        var eff = statusGraphicEffects[status];
        eff.Stop();
        ObjectPool.Instance.ReleaseObject(eff.gameObject);
        statusGraphicEffects.Remove(status);
    }

    private float oldPosY = -100;
    private void SetStatusPosition()
    {
        // var hp_tip = skeletonAnimation.skeleton.FindBone("hp_tip");
        // if (hp_tip != null)
        // {
        //     var hpTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(hp_tip.WorldX, hp_tip.WorldY, 0f));
        //     statusContainer.position = hpTipPos + new Vector3(0, 0.3f, 0);
        // }
        // else
        // {
        //     if (oldPosY == -100) oldPosY = statusContainer.position.y;
        //     var pos = oldPosY;
        //     if (curTrackEntry != null && curTrackEntry.animation != null
        //                        && curTrackEntry.Animation.Name == Constant.SkillPairs[2][1])
        //         pos = oldPosY + offsetHealthBarBySkill02;
        //     statusContainer.position = new Vector3(statusContainer.position.x, pos, 0);
        // }
    }

    public bool Stun(AttackParam attackParam)
    {
        StopAttack();

        timeStun = timeStun > attackParam.attackerInfo.specialSkillTime ? timeStun : attackParam.attackerInfo.specialSkillTime;
        if (!InSpecialEffect(Status.Stun))
        {
            stunIE = StunIE(attackParam);
            StartCoroutine(stunIE);
            return true;
        }
        return false;
    }

    private IEnumerator StunIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start StunIE");

        skeletonAnimation.AnimationState.SetAnimation(trackIndexIdle, "idle", false);
        AddStatusUI(Status.Stun);
        AddStatusParticleEffect(Status.Stun, EffectPos.HpTip);
        inStun = true;

        while (timeStun > 0 && !isDie && inStun)
        {
            yield return null;
            timeStun -= Time.deltaTime;
        }

        RemoveStatusParticleEffect(Status.Stun);

        timeStun = 0;
        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
        RemoveStatusUI(Status.Stun);

        // Debug.Log(name + " out StunIE");
    }

    private bool Poison(AttackParam attackParam)
    {
        poisonIE = PoisonIE(attackParam);
        StartCoroutine(poisonIE);
        return true;
    }

    private IEnumerator PoisonIE(AttackParam attackParam)
    {
        Debug.Log(name + " start PoisonIE");

        attackParam.attackerInfo.damage = StatHelper.GetDamageAtk(attackParam.attackerInfo.atk, attackParam.attackerInfo.effectValue);
        if (attackParam.attackerInfo.damage <= 0) yield break;
        AddStatusUI(Status.Poison);
        inPoinson = true;

        attackParam.attackerInfo.effectHit = null;
        attackParam.attackerInfo.specialSkill = AbilityType.None;
        attackParam.attackerInfo.critFactor = 1;
        attackParam.alwayHit = true;

        float countTime = attackParam.attackerInfo.specialSkillTime;
        while (countTime > 0 && !isDie && inPoinson)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(attackParam, true);
            countTime -= 1f;
        }

        RemoveStatusUI(Status.Poison);

        Debug.Log(name + " out PoisonIE");
    }

    private bool Burn(AttackParam attackParam)
    {
        burnIE = BurnIE(attackParam);
        StartCoroutine(burnIE);
        return true;
    }

    private IEnumerator BurnIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start BurnIE");


        attackParam.attackerInfo.damage = StatHelper.GetDamageAtk(attackParam.attackerInfo.atk, attackParam.attackerInfo.effectValue);
        if (attackParam.attackerInfo.damage <= 0) yield break;
        AddStatusUI(Status.Burn);
        inBurn = true;
        // var damage = attackParam.attackerInfo.damage;
        // attackParam.attackerInfo.damage = BigIntegerHelper.BigMultiplyFloat(damage, attackParam.attackerInfo.effectValue);
        // if (attackParam.attackerInfo.damage <= 0) yield break;

        attackParam.attackerInfo.effectHit = null;
        attackParam.attackerInfo.specialSkill = AbilityType.None;
        attackParam.attackerInfo.critFactor = 1;
        attackParam.alwayHit = true;

        float countTime = attackParam.attackerInfo.specialSkillTime;
        while (countTime > 0 && !isDie && inBurn)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(attackParam, true);
            countTime -= 1f;
        }

        RemoveStatusUI(Status.Burn);

        // Debug.Log(name + " out BurnIE");
    }

    private bool FreezeExplode(AttackParam attackParam)
    {
        StopAttack();

        if (!InSpecialEffect(Status.FreezeExplode)
            && !InSpecialEffect(Status.Freeze))
        {
            freezeExplodeIE = FreezeExplodeIE(attackParam);
            StartCoroutine(freezeExplodeIE);
            return true;
        }
        return false;
    }

    private IEnumerator FreezeExplodeIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start FreezeExplodeIE");

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);
        skeletonAnimation.AnimationState.SetAnimation(trackIndexIdle, "idle", false);
        AddStatusUI(Status.FreezeExplode);
        AddStatusParticleEffect(Status.Freeze, EffectPos.TargetTip);
        inFreezeExplode = true;
        // attackParam.attackerInfo.effectHit = null;
        attackParam.attackerInfo.specialSkill = AbilityType.None;
        attackParam.attackerInfo.critFactor = 1;
        attackParam.alwayHit = true;
        float countTime = attackParam.attackerInfo.specialSkillTime;
        while (countTime > 0 && !isDie && inFreezeExplode)
        {
            yield return null;
            countTime -= Time.deltaTime;
        }

        RemoveStatusUI(Status.Freeze);
        FreezeExplodeEffect(attackParam);

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
        RemoveStatusUI(Status.FreezeExplode);

        // Debug.Log(name + " out FreezeExplodeIE");
    }

    private bool Freeze(AttackParam attackParam)
    {
        StopAttack();

        if (!InSpecialEffect(Status.FreezeExplode)
            && !InSpecialEffect(Status.Freeze))
        {
            freezeIE = FreezeIE(attackParam);
            StartCoroutine(freezeIE);
            return true;
        }
        return false;
    }

    private IEnumerator FreezeIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start FreezeIE");

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexMove, 0);
        skeletonAnimation.AnimationState.SetAnimation(trackIndexIdle, "idle", false);
        AddStatusUI(Status.Freeze);
        AddStatusParticleEffect(Status.Freeze, EffectPos.TargetTip);
        inFreeze = true;

        // attackParam.attackerInfo.effectHit = null;
        attackParam.attackerInfo.specialSkill = AbilityType.None;
        attackParam.attackerInfo.critFactor = 1;
        attackParam.alwayHit = true;
        float countTime = attackParam.attackerInfo.specialSkillTime;
        while (countTime > 0 && !isDie && inFreeze)
        {
            yield return null;
            countTime -= Time.deltaTime;
        }

        RemoveStatusParticleEffect(Status.Freeze);
        if (attackParam.attackerInfo.perDmgWhenFreezeExplode > 0)
        {
            var perDmg = attackParam.attackerInfo.perDmgWhenFreezeExplode;
            var dmg = attackParam.attackerInfo.damage;

            if (perDmg != 1)//fake
            {
                dmg += BigIntegerHelper.BigMultiplyFloat(dmg, attackParam.attackerInfo.perDmgWhenFreezeExplode / 100);
            }
            Debug.Log("FreezeIE perDmgWhenFreezeExplode new dmg:" + dmg);

            attackParam.attackerInfo.damage = dmg;
            FreezeExplodeEffect(attackParam);
        }

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
        RemoveStatusUI(Status.Freeze);

        // Debug.Log(name + " out FreezeIE");
    }

    private bool KnockBack(AttackParam attackParam)
    {
        StopAttack();

        if (!InSpecialEffect(Status.KnockBack))
        {
            knockBackIE = KnockBackIE(attackParam);
            StartCoroutine(knockBackIE);
            return true;
        }
        return false;
    }

    private IEnumerator KnockBackIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start KnockBackIE");

        skeletonAnimation.AnimationState.SetAnimation(trackIndexIdle, "idle", false);
        AddStatusUI(Status.KnockBack);
        inKockBack = true;

        float countTime = 0f;
        while (true && !isDie && inKockBack)
        {
            yield return null;

            countTime += Time.deltaTime;

            var tempPos = transform.position;
            if (currentTeam == Team.My)
                tempPos.x -= 3 * Time.deltaTime;
            else
                tempPos.x += 3 * Time.deltaTime;

            if (currentTeam == Team.My && tempPos.x < GameManager.instance.myGate.position.x) break;
            else if (tempPos.x > GameManager.instance.enemyGate.position.x) break;

            transform.position = tempPos;

            if (countTime > 0.5f) break;
        }

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
        RemoveStatusUI(Status.KnockBack);

        // Debug.Log(name + " out KnockBackIE");
    }

    private bool SlowDown(AttackParam attackParam)
    {
        if (!InSpecialEffect(Status.SlowDown)
            && !InSpecialEffect(Status.SlowSpeed))
        {
            slowdowIE = SlowDownIE(attackParam);
            StartCoroutine(slowdowIE);
            return true;
        }
        return false;
    }

    private IEnumerator SlowDownIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start SlowDownIE");

        AddStatusUI(Status.SlowDown);
        inSlowdow = true;
        var curSpeed2 = curSpeed;
        var animAttackTS2 = animAttackTS;
        var animMoveTs2 = animMoveTs;

        var timeCount = attackParam.attackerInfo.specialSkillTime;

        // Debug.Log("curSpeed:" + curSpeed);
        // Debug.Log("animAttackTS:" + animAttackTS);

        while (timeCount > 0 && !isDie && inSlowdow)
        {
            yield return null;

            curSpeed = curSpeed2 * attackParam.attackerInfo.effectValue;
            animAttackTS = animAttackTS2 * attackParam.attackerInfo.effectValue;
            animMoveTs = animMoveTs2 * attackParam.attackerInfo.effectValue;

            // time += Time.deltaTime;
            // if (time >= 1)
            // {
            //     time -= 1;
            //     Debug.Log("curSpeed 2:" + curSpeed);
            //     Debug.Log("animAttackTS 2:" + animAttackTS);
            // }
            timeCount -= Time.deltaTime;
        }
        curSpeed = curSpeedTmp;
        animAttackTS = animAttackTsTmp;
        animMoveTs = animMoveTsTmp;

        RemoveStatusUI(Status.SlowDown);

        // Debug.Log(name + " out SlowDownIE:" + string.Join(",", listStatus));
    }

    private bool SlowSpeed(AttackParam attackParam)
    {
        if (!InSpecialEffect(Status.SlowDown)
           && !InSpecialEffect(Status.SlowSpeed))
        {
            slowSpeedIE = SlowSpeedIE(attackParam);
            StartCoroutine(slowSpeedIE);
            return true;
        }
        return false;
    }

    private IEnumerator SlowSpeedIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start SlowSpeedIE");

        AddStatusUI(Status.SlowSpeed);
        inSlowSpeed = true;

        var curSpeed2 = curSpeed;
        var animMoveTs2 = animMoveTs;

        var timeCount = attackParam.attackerInfo.specialSkillTime;
        while (timeCount > 0 && !isDie && inSlowSpeed)
        {
            yield return null;

            curSpeed = curSpeed2 * attackParam.attackerInfo.effectValue;
            animMoveTs = animMoveTs2 * attackParam.attackerInfo.effectValue;

            timeCount -= Time.deltaTime;
        }
        curSpeed = curSpeedTmp;
        curSpeed = animMoveTsTmp;

        RemoveStatusUI(Status.SlowSpeed);

        // Debug.Log(name + " out SlowSpeedIE");
    }

    public bool Root(AttackParam attackParam)
    {
        StopAttack();

        if (!InSpecialEffect(Status.Root))
        {
            rootIE = RootIE(attackParam);
            StartCoroutine(rootIE);
            return true;
        }
        return false;
    }

    private IEnumerator RootIE(AttackParam attackParam)
    {
        // Debug.Log(name + " start RootIE");

        skeletonAnimation.AnimationState.SetAnimation(trackIndexIdle, "idle", false);

        attackParam.attackerInfo.damage = StatHelper.GetDamageAtk(attackParam.attackerInfo.atk, attackParam.attackerInfo.effectValue);
        if (attackParam.attackerInfo.damage <= 0) yield break;

        AddStatusUI(Status.Root);
        inRoot = true;

        attackParam.attackerInfo.effectHit = null;
        attackParam.attackerInfo.specialSkill = AbilityType.None;
        attackParam.attackerInfo.critFactor = 1;
        attackParam.alwayHit = true;

        float countTime = attackParam.attackerInfo.specialSkillTime;
        while (countTime > 0 && !isDie && inRoot)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(attackParam, true);
            countTime -= 1f;
        }

        skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
        RemoveStatusUI(Status.Root);

        // Debug.Log(name + " out RootIE");
    }

    public void RemoveAllStatus()
    {
        StartCoroutine(RemoveAllStatusIE());
        IEnumerator RemoveAllStatusIE()
        {
            yield return new WaitForEndOfFrame();
            inStun = inPoinson = inBurn = inFreezeExplode = inFreeze = inKockBack = inSlowdow = inSlowSpeed = inRoot = false;
        }

        // if (stunIE != null) StopCoroutine(stunIE);
        // RemoveEffectStatus(Status.Stun);
        // RemoveUIStatus(Status.Stun);
        // timeStun = 0;

        // if (poisonIE != null) StopCoroutine(poisonIE);
        // RemoveEffectStatus(Status.Poison);
        // RemoveUIStatus(Status.Poison);

        // if (burnIE != null) StopCoroutine(burnIE);
        // RemoveEffectStatus(Status.Burn);
        // RemoveUIStatus(Status.Burn);

        // if (freezeExplodeIE != null) StopCoroutine(freezeExplodeIE);
        // RemoveEffectStatus(Status.FreezeExplode);
        // RemoveUIStatus(Status.FreezeExplode);

        // if (freezeIE != null) StopCoroutine(freezeIE);
        // RemoveEffectStatus(Status.Freeze);
        // RemoveUIStatus(Status.Freeze);

        // if (knockBackIE != null) StopCoroutine(knockBackIE);
        // RemoveEffectStatus(Status.KnockBack);
        // RemoveUIStatus(Status.KnockBack);

        // if (slowdowIE != null) StopCoroutine(slowdowIE);
        // RemoveEffectStatus(Status.SlowDown);
        // RemoveUIStatus(Status.SlowDown);

        // if (slowSpeedIE != null) StopCoroutine(slowSpeedIE);
        // RemoveEffectStatus(Status.SlowSpeed);
        // RemoveUIStatus(Status.SlowSpeed);

        // if (rootIE != null) StopCoroutine(rootIE);
        // RemoveEffectStatus(Status.Root);
        // RemoveUIStatus(Status.Root);

        // skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndexIdle, 0);
    }
}