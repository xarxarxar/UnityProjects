using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DebuffType
{
    AddHP,
    InitialMoneyDecrease,
    EliteEnemyCount,
    CrystalMaxHpDecrease,
    AddCount,
    DamageNullified,
    EnemyMaxBoundCount
}

public class DebuffStruct : MonoBehaviour
{
    public DebuffType _debuffType;//该条对应的debuff
    [SerializeField] public Text _descriptionText;//描述文字
    [SerializeField] public Image _icon;
    [SerializeField] public Text _countText;//个数文本
    public int MAXCOUNT = 10;
    private int _count = 0;

    public static event UnityAction<DebuffType, int> OnChangeDebuffStruct;//选择debuff改变时

    public void Init(DebuffType debuffType)
    {
        _debuffType= debuffType;
        _count = 0;
        _descriptionText.text = Debuff.GetDescription(_debuffType);


        UpdateUI();
    }

    private void ChangeValue(int delta)
    {
        OnChangeDebuffStruct?.Invoke(_debuffType, delta);
        _count += delta;
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool canRemove = _count > 0;
    }
}
