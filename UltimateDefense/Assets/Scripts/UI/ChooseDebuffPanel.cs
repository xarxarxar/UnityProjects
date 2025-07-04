using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseDebuffPanel : MonoBehaviour
{
    public static event UnityAction<Debuff> OnDebuffChooseEnd;//debuff选择完毕
    [SerializeField] private Text _titleText;//标题
    [SerializeField]private Debuff _debuff;
    [SerializeField]private Button startChallengeButton;//开始挑战按钮
    [SerializeField]private Button _giveupChallengeButton;//放弃挑战按钮
    [SerializeField]private DebuffStruct _debuffStructPrefab;//debuffStruct预制体
    [SerializeField]private Transform _debuffStructParent;//debuffStruct预制体生成的父物体
    [SerializeField]private List<DebuffStruct> debuffStructs=new List<DebuffStruct>();//所有的DebuffStruct
    private int _chooseCount = 0;//选择debuff的个数，个数不能大于通关次数

    private void OnEnable()
    {
        if (DataManager.Instance.PlayerInfo.PassCount == 0)//如果通关次数为0，则直接跳过这一步
        {
            OnDebuffChooseEnd?.Invoke(new Debuff(_debuff));
            gameObject.SetActive(false);
        }
        _debuff = new Debuff();
        _chooseCount = 0;
        _titleText.text = $"您已通关了{DataManager.Instance.PlayerInfo.PassCount}次\r\n请至少选择{DataManager.Instance.PlayerInfo.PassCount}个Debuff再进行挑战";

        SetButtonStatus(startChallengeButton, false);
        InitAllDebuffStructs();//初始化debuffstruct
        startChallengeButton.onClick.AddListener(StartChallengeButton);
        _giveupChallengeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            GameUIManager.Instance.ShowMainMenu();
        });

        DebuffStruct.OnChangeDebuffStruct += OnChangeDebuffStruct;
    }

    private void OnDisable()
    {
        debuffStructs.Clear();
        foreach (Transform child in _debuffStructParent)
        {
            Destroy(child.gameObject);
        }


        startChallengeButton.onClick.RemoveAllListeners();
        DebuffStruct.OnChangeDebuffStruct -= OnChangeDebuffStruct;
    }

    //开始挑战
    private void StartChallengeButton()
    {
        OnDebuffChooseEnd?.Invoke(new Debuff(_debuff));
        gameObject.SetActive(false);
    }

    //初始化debuffstruct
    private void InitAllDebuffStructs()
    {
        debuffStructs.Clear();
        foreach (Transform child in _debuffStructParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < System.Enum.GetValues(typeof(DebuffType)).Length; i++)
        {
            DebuffStruct debuffStruct= Instantiate(_debuffStructPrefab, _debuffStructParent);
            DebuffType type = (DebuffType)i;
            debuffStructs.Add(debuffStruct);
            debuffStruct.Init(type);
        }
    }

    //debuffstruct改变时
    private void OnChangeDebuffStruct(DebuffType debuffType,int delta)
    {
        Debug.Log($"debuffType is {debuffType},delta is {debuffType}");
        switch (debuffType)
        {
            case DebuffType.AddHP:
                _debuff.AddHP = Mathf.Max(0, _debuff.AddHP + delta);
                break;
            case DebuffType.AddSpeed:
                _debuff.AddSpeed = Mathf.Max(0, _debuff.AddSpeed + delta);
                break;
            case DebuffType.AddCount:
                _debuff.AddCount = Mathf.Max(0, _debuff.AddCount + delta);
                break;
        }
        _chooseCount = _debuff.AddHP + _debuff.AddSpeed + _debuff.AddCount;

        //设置开始挑战按钮的状态
        SetButtonStatus(startChallengeButton, _chooseCount >= DataManager.Instance.PlayerInfo.PassCount);
        
    }

    //设置按钮的状态
    private void SetButtonStatus(Button button,bool active)
    {
        button.interactable = active;

        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup)
        {
            canvasGroup.alpha = active ? 1 : 0.5f;
        }
    }
}

/// <summary>
/// 通关一次之后选择的debuff
/// </summary>
[System.Serializable]
public class Debuff
{
    /// <summary>
    /// 敌人增加10%的血量的个数
    /// </summary>
    public int AddHP;

    /// <summary>
    /// 敌人增加10%的移速的个数
    /// </summary>
    public int AddSpeed;

    /// <summary>
    /// 敌人增加10%的数量的个数
    /// </summary>
    public int AddCount;

    public Debuff()
    {
        AddHP = 0; AddSpeed=0; AddCount = 0;
    }

    //拷贝一份
    public Debuff (Debuff debuff)
    {
        var fields = typeof(Debuff).GetFields();
        foreach (var field in fields)
        {
            field.SetValue(this, field.GetValue(debuff));
        }
    }

    public static string GetDescription(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.AddHP:
                return "敌人生命值增加 10%";
            case DebuffType.AddSpeed:
                return "敌人移动速度增加 10%";
            case DebuffType.AddCount:
                return "敌人数量增加 10%";
            default:
                return "未知效果";
        }
    }
}