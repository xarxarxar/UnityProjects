
using System;
using System.Numerics;
using UnityEngine;

public class StatHelper
{
    // public static BigInteger GetDamage(BigInteger atk, BigInteger opDef, float incr)
    // {
    //     var damage = (200 * atk) / (200 + opDef);
    //     if (incr > 0)
    //     {
    //         damage = BigIntegerHelper.BigMultiplyFloat(damage, incr);
    //     }

    //     return damage;
    //     // return 1;
    // }

    public static BigInteger GetBaseAtribute(BigInteger atk, int curStar, BigInteger curLevel, out BigInteger newBase, out BigInteger moreValue)
    {
        if (atk == 0) return 0;
        if (curStar == 0) curStar = 1;

        newBase = atk * BigInteger.Pow(2, curStar - 1);
        // newBase = atk;

        var vl = BigIntegerHelper.Pow2((decimal)1.05f, curLevel);
        var totalValue = newBase * vl.numerator / vl.denominator;

        moreValue = totalValue - newBase;

        return totalValue;
    }

    public static float GetCritFactor(float critRate, float critFactor)
    {
        float critF = 1;
        var rd = UnityEngine.Random.Range(0, 100);
        if (rd < critRate && critFactor > 0)
        {
            critF = critFactor;
        }
        return critF;
    }

    public static BigInteger GetDamageSkill(BigInteger atk, BigInteger opDef, float percentBySkill, float percentByStar, int unlockStar, int curStar,
        float critFactor, Element attackerElement, Element attackedElement)
    {
        // Debug.Log("curStar:" + curStar + " unlockStar:" + unlockStar);

        if (curStar < unlockStar) return 0;

        var damage = (200 * atk) / (200 + opDef);

        var percentByCurStar = (curStar - unlockStar) * percentByStar;
        percentByCurStar = 0;

        var perCent = (percentBySkill + percentByCurStar) * 0.01f;

        if (perCent > 0) damage = BigIntegerHelper.BigMultiplyFloat(damage, perCent);

        // if (percentBySkill > 0 && percentBySkill != 100) damage = BigIntegerHelper.BigMultiplyFloat(damage, percentBySkill * 0.01f);
        // Debug.Log("attackerElement:" + attackerElement + " attackedElement:" + attackedElement);

        var damage1 = damage;
        damage = BigIntegerHelper.BigMultiplyFloat(damage, critFactor);

        // Debug.Log("percentBySkill:" + percentBySkill + " percentByCurStar:" + percentByCurStar + " perCent:" + perCent + " damage1:" + damage1 + " damage:" + damage);

        var key = (int)attackerElement * 100 + (int)attackedElement;

        if (Constant.damageByElementPair.ContainsKey(key))
        {
            var xxx = Constant.damageByElementPair[key];
            damage = BigIntegerHelper.BigMultiplyFloat(damage, xxx);
            // Debug.Log(attackerElement + " " + attackedElement + " xxx:" + xxx + " damage:" + damage);
        }

        if (GameManager.instance != null && GameManager.instance.fakeDamage)
            return 1;
        return damage;
    }

    public static BigInteger GetDamageAtk(BigInteger atk, float x)
    {
        BigInteger damage = BigIntegerHelper.BigMultiplyFloat(atk, x);
        if (GameManager.instance != null && GameManager.instance.fakeDamage)
            return 1;
        return damage;
    }

}