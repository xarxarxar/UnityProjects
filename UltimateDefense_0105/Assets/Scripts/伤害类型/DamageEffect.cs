using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DamageEffect : ScriptableObject
{
    public abstract void ApplyEffect(Enemy enemy, float damage);
}
