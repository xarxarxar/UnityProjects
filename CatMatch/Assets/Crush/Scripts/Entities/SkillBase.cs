using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public GameObject bulletPref;
    public GameObject effectHitPref;
    public GameObject effectHitSplashPref;
    public float speed = 7f;

    public virtual void Active(int order, BeastBase attacker, GameObject target)
    {

    }

    public virtual void Passive(BeastBase attacker)
    {
    }
}