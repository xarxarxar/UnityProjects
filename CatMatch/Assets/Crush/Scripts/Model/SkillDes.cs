
using System;
using UnityEngine;

[System.Serializable]
public class SkillDes
{
    public string skillName;
    public string skillDes;
    public Sprite skillSp;

    public float percentDmgByStar = 100;// percent dmg by star

    public AbilityType effect;
    public float effectValue;
    public float effectTime;
    public float chance;
    public float AOE;
    public float percentDmgBySkill;
    public int countDown;
}