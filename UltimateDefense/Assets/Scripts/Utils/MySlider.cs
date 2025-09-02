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
    private float _gameSpeed => BattleManager.Instance.GameSpeed.Value;

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
        //gameObject.SetActive(true);

        slider.value = value;
    }
    /// <summary>
    /// 开始倒计时
    /// </summary>
    /// <param name="value"></param>
    public void StartCountDown(float value)
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        gameObject.SetActive(true);
        StartCountdown(value);
    }



    /// <summary>
    /// 开始倒计时
    /// </summary>
    /// <param name="duration">倒计时总时长（秒）</param>
    private void StartCountdown(float duration)
    {

        if (_countdownCoroutine != null)
            StopCoroutine(_countdownCoroutine);

        _countdownCoroutine = StartCoroutine(CountdownIE(duration));
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

    private IEnumerator CountdownIE(float duration)
    {
        if (slider == null)
        {
            Debug.Log("slider为空");
        }
        slider.value = slider.maxValue;

        float timer = 0f;

        while (timer < duration)
        {
            // 若游戏暂停，则不更新 timer 和 slider
            if (!BattleManager.Instance.IsPaused.Value)
            {
                timer += Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
                slider.value = Mathf.Clamp01(1f - timer / duration)*(slider.maxValue-slider.minValue)+ slider.minValue;
            }

            yield return null;
        }

        slider.value = 0f;
        _countdownCoroutine = null;
        gameObject.SetActive(false); // 可选：倒计时结束后隐藏
    }
}
