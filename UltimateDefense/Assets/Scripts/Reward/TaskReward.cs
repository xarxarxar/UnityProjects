using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务奖励
/// </summary>
public class TaskReward : MonoBehaviour
{
    [SerializeField] private Button _receiveButton;//领取按钮
    [SerializeField] private Slider _taskSlider;//任务slider
    [SerializeField] private RewardStruct _rewardStruct;//奖励
    [SerializeField] private Text _desText;//描述Text
    [SerializeField] private GameObject _finishMask;//已完成遮罩
    private bool _isFinished=false;//是否完成这个任务
    private int _needValue;//该任务需要的值
    private int _currentValue;//当前值

    private Color32 _normalColor = new Color32(0, 153, 249, 255);
    private Color32 _finishColor = new Color32(85, 241, 133, 255);

    /// <summary>
    /// 该任务需要的值
    /// </summary>
    public int NeedValue { get => _needValue; }
    /// <summary>
    /// 当前值
    /// </summary>
    public int CurrentValue { get => _currentValue; }

    private void OnEnable()
    {
        
    }

    /// <summary>
    /// 初始化任务
    /// </summary>
    /// <param name="rewardType">奖励的类型</param>
    /// <param name="count">奖励的数量</param>
    /// <param name="value">任务需要的数量</param>
    /// <param name="description">任务的描述</param>
    public void Init(RewardType rewardType,int count,string description,int needValue,int currentValue)
    {
        _rewardStruct.Init(rewardType, count);
        _desText.text = description;
        _taskSlider.interactable = false;

        if (needValue == -1)//代表已领取该任务奖励
        {
            _taskSlider.value = _taskSlider.maxValue;
            _taskSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = _finishColor;
            _taskSlider.transform.Find("SliderText").GetComponent<Text>().text = $"完成";
            _isFinished = true;
            _finishMask.SetActive(true);
            return;
        }
        _needValue= needValue;
        _currentValue = currentValue;
        
        _taskSlider.maxValue = needValue;
        _taskSlider.minValue = 0;
        _taskSlider.wholeNumbers = true;
        _taskSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = _normalColor;
        _taskSlider.transform.Find("SliderText").GetComponent<Text>().text = $"{_currentValue}/{_needValue}";
        _finishMask.SetActive(false);
        _isFinished = false;
        UpdateStatus(_currentValue);

        _receiveButton.onClick.AddListener(ReceiveReward);
    }

    private void OnDisable()
    {
        //_receiveButton.onClick.RemoveAllListeners();
    }

    //获取奖励按钮
    private void ReceiveReward()
    {
        Debug.Log($"_currentValue is {_currentValue},_needValue is {_needValue},_isFinished is {_isFinished}");
        if (!_isFinished) return;
        MetaCurrencyManager.Instance.AddMetaCoin(_rewardStruct.type, _rewardStruct.count);
        _taskSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color= _finishColor;
        _taskSlider.transform.Find("SliderText").GetComponent<Text>().text= "完成";
        _finishMask.SetActive(true);
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="value"></param>
    public void UpdateStatus(int value)
    {
        if (_isFinished)
        {
            return;
        }

        _currentValue = value;
        
        if (_currentValue >= _needValue)
        {
            _currentValue = _needValue;
            _isFinished=true;
        }

        _taskSlider.value = _currentValue;
        _taskSlider.transform.Find("SliderText").GetComponent<Text>().text = $"{_currentValue}/{_needValue}";
    }
}
