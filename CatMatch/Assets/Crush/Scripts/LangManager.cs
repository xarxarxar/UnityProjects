using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangManager : MonoBehaviour
{
    private static LangManager instance;
    public static LangManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("LangManager");
                instance = gameObject.AddComponent<LangManager>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    public static string Language = "en";
    public const string English = "en";
    public const string VietNamese = "vn";

    private Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

    void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as LangManager;
            DontDestroyOnLoad(gameObject);
        }

        switch (LangManager.Language)
        {
            case LangManager.English:
                keyValuePairs["LostMonster"] = "失去{0}只魔兽";
                keyValuePairs["Retreat"] = "撤退";
                keyValuePairs["ContentRetreat"] = "你确定要离开战斗吗？";
                keyValuePairs["Damage"] = "伤害";
                keyValuePairs["DamageTaken"] = "承受伤害";
                keyValuePairs["DamagePerMonster"] = "每只魔兽伤害";
                keyValuePairs["Reward"] = "奖励";
                keyValuePairs["Okay"] = "确定";
                keyValuePairs["Stats"] = "属性";

                keyValuePairs["Atk"] = "攻击";
                keyValuePairs["Hp"] = "生命";
                keyValuePairs["Def"] = "防御";
                keyValuePairs["CritRate"] = "暴击率";
                keyValuePairs["CriteDamage"] = "暴击伤害";
                keyValuePairs["AtkPerSec"] = "每秒攻击";
                keyValuePairs["AtkRange"] = "攻击范围";
                keyValuePairs["MoveSpeed"] = "移动速度";
                keyValuePairs["EffectResistance"] = "效果抵抗";
                keyValuePairs["FrenzyChance"] = "狂暴几率";
                keyValuePairs["DodgeRate"] = "闪避率";
                keyValuePairs["StunChance"] = "眩晕几率";
                keyValuePairs["StunTime"] = "眩晕时间";
                keyValuePairs["AoERadius"] = "范围半径";
                keyValuePairs["AoEDmg"] = "范围伤害";
                keyValuePairs["UltimateAtk"] = "终极攻击";
                keyValuePairs["KnightShieldHP"] = "骑士护盾生命";
                keyValuePairs["BonusLoot"] = "额外战利品";
                keyValuePairs["FreezeChance"] = "冰冻几率";
                keyValuePairs["FreezeTime"] = "冰冻时间";
                keyValuePairs["FreezeExplotionDmg"] = "冰冻爆炸伤害";
                keyValuePairs["BurnChance"] = "灼烧几率";
                keyValuePairs["BurnTime"] = "灼烧时间";
                keyValuePairs["BurnDmg"] = "灼烧伤害";
                keyValuePairs["PoisonChance"] = "中毒几率";
                keyValuePairs["PoisonDmg"] = "中毒伤害";

                keyValuePairs["Passive"] = "被动";
                keyValuePairs["Active"] = "主动";
                keyValuePairs["NoteSkillOn"] = "下次<sprite=0>进化：<color=#FC7374>{0}</color>";
                keyValuePairs["NoteSkillOff"] = "此技能在该魔兽达到{0}<sprite=0>时解锁";

                keyValuePairs["time"] = "次";
                keyValuePairs["times"] = "次";
                keyValuePairs["OutOfPlayAmount"] = "不足<sprite=10>";
                keyValuePairs["OutOfPlayAmountDes"] = "获得{0}个<sprite=10>，价格{1}个<sprite=6>？\n你今天已购买{2}{3}";
                break;
            case LangManager.VietNamese:
                break;
        }

    }

    public string Get(string key)
    {
        if (keyValuePairs.ContainsKey(key)) return keyValuePairs[key];
        return key;
    }
}
