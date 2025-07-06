using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DebuffType
{
    AddHP,
    AddSpeed,
    AddCount,
    DamageNullified
}

public class DebuffStruct : MonoBehaviour
{
    private DebuffType _debuffType;//该条对应的debuff
    [SerializeField] private Text _descriptionText;//描述文字
    [SerializeField] private Text _levelText;//等级
    [SerializeField] private Button _addButton;//增加按钮
    [SerializeField] private Button _removeButton;//移除按钮
    private int _count = 0;


    public static event UnityAction<DebuffType, int> OnChangeDebuffStruct;//选择debuff改变时

    public void Init(DebuffType debuffType)
    {
        _debuffType= debuffType;
        _count = 0;
        _descriptionText.text = Debuff.GetDescription(_debuffType);

        _addButton.onClick.AddListener(() => ChangeValue(1));
        _removeButton.onClick.AddListener(() => ChangeValue(-1));

        UpdateUI();
    }

    private void OnDisable()
    {
        _addButton.onClick.RemoveAllListeners();
        _removeButton.onClick.RemoveAllListeners();
    }

    private void ChangeValue(int delta)
    {
        OnChangeDebuffStruct?.Invoke(_debuffType, delta);
        _count += delta;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _levelText.text = $"{_count}";
        bool canRemove = _count > 0;
        _removeButton.interactable = canRemove;
        //设置CanvasGroup的透明度
        CanvasGroup removeGroup = _removeButton.GetComponent<CanvasGroup>();
        if (removeGroup != null)
            removeGroup.alpha = canRemove ? 1f : 0.5f;
    }
}
