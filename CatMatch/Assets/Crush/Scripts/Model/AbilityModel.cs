using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityModel
{
    public AbilityType abilityType;
    public float chance;
    public float time;
    public float value;
}

[System.Serializable]
public class EffectBySkill
{
    public AbilityType abilityType;
    // public float chance;
    public DateTime dateTime;
    public float value;
}

