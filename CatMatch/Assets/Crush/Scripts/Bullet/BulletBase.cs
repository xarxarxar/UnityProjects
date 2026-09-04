using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public class BulletBase : MonoBehaviour
{
    [HideInInspector]
    public Team bulletTeam;
    [HideInInspector]
    public BulletParam bulletParam;

    public GameObject effPref;
    public GameObject explosionEffPref;
    public GameObject startEffPref;

    private Rigidbody2D rb2D;

    protected bool isActive;
    protected bool fromLeft;

    protected List<ParticleSystem> particles;
    protected float countTimeOffEffect;

    protected virtual void Awake()
    {
        particles = new List<ParticleSystem>();
    }

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public virtual void Setup(Team team, Vector3 startPoint, BulletParam param)
    {
        bulletTeam = team;

        fromLeft = false;
        if (team == Team.My)
        {
            transform.localScale = new Vector3(1, 1, 1);
            fromLeft = true;
        }
        else transform.localScale = new Vector3(-1, 1, 1);

        transform.position = startPoint;

        bulletParam = param;

        isActive = true;

        particles.Clear();
        countTimeOffEffect = 0;
        // Debug.Log("Setup: " + gameObject.GetInstanceID());
    }

    protected virtual void Update()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            var effect = particles[i];
            if (effect.isStopped)
            {
                i--;
                for (int l = 0; l < effect.transform.childCount; l++)
                {
                    var tr = effect.transform.GetChild(l).GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.Clear();
                    }
                }
                particles.Remove(effect);
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }
        if (!isActive && particles.Count == 0 && this.enabled)
        {
            ObjectPool.Instance.ReleaseObject(gameObject);
            if (bulletParam.skill != null)
                ObjectPool.Instance.ReleaseObject(bulletParam.skill);
        }
    }

    // protected void HideParticle(ParticleSystem particle)
    // {
    //     if (particle.gameObject.activeInHierarchy) particle.gameObject.SetActive(false);
    //     if (particle.isPlaying) particle.Stop();
    // }

    // protected void ShowParticle(ParticleSystem particle)
    // {
    //     if (!particle.gameObject.activeInHierarchy) particle.gameObject.SetActive(true);
    //     if (!particle.isPlaying) particle.Play();
    // }

    protected virtual BigIntCSharp GetDamage(AttackerInfo attackerInfo, EntityBase attackedEn, out float critFactor)
    {
        critFactor = 0f;
        var attackedDef = attackedEn.curDef;

        if (attackerInfo.abilityTypes != null && attackerInfo.abilityTypes.Contains(AbilityType.Piercing))
            attackedDef = 0;
        // stupid
        if (attackerInfo.specialSkill == AbilityType.AtkPerSec)
            return 0;

        var attackerElement = attackerInfo.attackerElement;
        if (bulletTeam == Team.My)
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
        // Debug.Log("dmgggggg:" + dmg);

        if (attackerInfo.hitComboTh > 0)
        {
            // var vl = Mathf.Pow(1.05f, attackerInfo.hitComboTh);
            // dmg = BigIntegerHelper.BigMultiplyFloat(dmg, vl);

            var vl = BigIntegerHelper.Pow2((decimal)1.05f, attackerInfo.hitComboTh);
            dmg = dmg * vl.numerator / vl.denominator;
        }
        return dmg;
    }
}
