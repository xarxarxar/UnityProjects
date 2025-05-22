// CountDownSlider.cs
// 控制倒计时滑动条
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class CountDownSlider : MonoBehaviour
{
    private Slider slider;
    public float waitTime => LevelConfig.instance.WaitTime;
    public bool isOver=false;
    public Text tipText;
    public static event UnityAction CountDownOver; // 倒计时结束事件
    public float SliderRatioOnInterrupt { get; private set; } = 0f; // 外部可以访问的比值

    private void Awake() => slider = GetComponent<Slider>();

    public void OnEnable()
    {
        slider.value=slider.maxValue;
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator CountDown(int round)
    {
        tipText.text = "";
        float tmp = RealWaitTime(round);
        tmp=Mathf.Clamp(tmp, 2, waitTime);
        //Debug.Log($"tmp is {tmp}");
        slider.maxValue = tmp;
        slider.value = tmp;
        isOver = false;

        while (slider.value > 0)
        {
            slider.value -= Time.deltaTime;
            // 暂停处理
            while (LevelPlaying.isPaused) yield return null;
            yield return null;

            if(isOver )
            {
                // 记录中断时的比值
                SliderRatioOnInterrupt = slider.value / slider.maxValue;
                slider.value = 0;
                break;
            }
        }
        slider.value = 0;
        gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public IEnumerator EndlessCountDown(float maxValue)
    {
        tipText.text = "";
        slider.maxValue = maxValue;
        slider.value = maxValue;
        isOver = false;

        while (slider.value > 0)
        {
            slider.value -= Time.deltaTime;
            // 暂停处理
            while (LevelPlaying.isPaused) yield return null;
            yield return null;

            if (isOver)
            {
                // 记录中断时的比值
                SliderRatioOnInterrupt = slider.value / slider.maxValue;
                slider.value = 0;
                break;
            }
        }
        slider.value = 0;
        gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public IEnumerator GuideCountDown(float maxValue)
    {
        tipText.text = "";
        slider.maxValue = maxValue;
        slider.value = maxValue;
        isOver = false;

        while (slider.value > 0)
        {
            slider.value -= Time.deltaTime;
            // 暂停处理
            while (LevelPlaying.tmpPaused) yield return null;
            yield return null;

            if (isOver)
            {
                // 记录中断时的比值
                SliderRatioOnInterrupt = slider.value / slider.maxValue;
                slider.value = 0;
                break;
            }
        }
        slider.value = 0;
        gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public void SetIsOver()
    {
        isOver = true;
    }

    private float RealWaitTime(int round)
    {
        float tmp = waitTime;
        if (round >= 3 && round < 6)
        {
            tmp = tmp - 1.0f;
            if (tipText.text != "速度变快")
            {
                tipText.text = "速度变快";
                tipText.color = Color.yellow;
                tipText.rectTransform.localScale = Vector3.one * 1.2f;

                tipText.rectTransform.DOScale(Vector3.one * 1.0f, 0.5f)
                    .SetEase(Ease.OutBack);
            }
            
        }
        if (round >= 6)
        {
            tmp = tmp - 2.0f;
            if(tipText.text != "速度更快")
            {
                tipText.text = "速度更快";
                tipText.color = Color.red;
                tipText.rectTransform.localScale = Vector3.one * 4.0f;

                tipText.rectTransform.DOScale(Vector3.one * 1.0f, 2.0f)
                    .SetEase(Ease.OutBack);
            }
            
        }
        Debug.Log($"tmp is {tmp}");
        return tmp;
    }
}