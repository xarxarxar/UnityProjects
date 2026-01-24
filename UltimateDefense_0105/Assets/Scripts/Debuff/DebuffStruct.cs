using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DebuffType
{
    AddHP,
    InitialMoneyDecrease,
    EliteEnemyCount,
    CrystalMaxHpDecrease,
    AddSpeed,
    DamageNullified,
}

public class DebuffStruct : MonoBehaviour
{
    public DebuffType _debuffType;//该条对应的debuff
    [SerializeField] public Text _countText;//个数文本
    public int MAXCOUNT = 10;
}
