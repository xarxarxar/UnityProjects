using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MySlider : MonoBehaviour
{
    [SerializeField]private Text text;
    [SerializeField] private Slider slider;
    private Coroutine _countdownCoroutine;//倒计时协程

    /// <summary>
    /// 倒计时结束
    /// </summary>
    public event UnityAction OnCountDownEnd;

    private float _currentValue;
    private float _maxValue;

    public float CurrentValue 
    { 
        get => _currentValue; 
        set
        {
            if (value != _currentValue)
            {
                _currentValue = value;
                slider.value = Mathf.Clamp(_currentValue, 0, 1);
            }
        }
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    private void OnEnable()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.maxValue = 1;
        slider.minValue = 0;
        StopCountdown();
    }



    /// <summary>
    /// 设置文本的值
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string txt)
    {
        if (text == null)
        {
            return;
        }
        text.text = txt;
    }

    /// <summary>
    /// 设置MySlider的值
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value)
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        Debug.Log($"修改slider is{value}");
        CurrentValue = value;
    }
    /// <summary>
    /// 设置Slider最大值
    /// </summary>
    public void SetSliderMaxValue(float value)
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.maxValue = value;
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    /// <param name="value"></param>
    public void StartCountDown(float value,bool useGameSpeed=true)
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.maxValue = 1;
        slider.value=1;
        CurrentValue = 1;
        _maxValue=value;


        gameObject.SetActive(true);
        if (_countdownCoroutine != null)
            StopCoroutine(_countdownCoroutine);

        _countdownCoroutine = StartCoroutine(CountdownIE(useGameSpeed));
    }

    /// <summary>
    /// 停止倒计时
    /// </summary>
    private void StopCountdown()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
    }

    private IEnumerator CountdownIE(bool useGameSpeed = true)
    {
        if (slider == null)
        {
            Debug.Log("slider为空");
        }
        slider.value = slider.maxValue;

        while (CurrentValue >=0)
        {
            if (useGameSpeed)
            {
                CurrentValue -= (Time.deltaTime * BattleManager.Instance.GameSpeed.Value)/_maxValue;
            }
            else
            {
                CurrentValue -= Time.deltaTime / _maxValue;
            }
            
            yield return null;
        }
        OnCountDownEnd?.Invoke();
        _countdownCoroutine = null;
        gameObject.SetActive(false); // 可选：倒计时结束后隐藏
    }
}
