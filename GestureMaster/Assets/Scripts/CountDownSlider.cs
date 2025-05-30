// CountDownSlider.cs
// 控制倒计时滑动条
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class CountDownSlider : MonoBehaviour
{
    public Slider sliderWaitTime;
    public Slider sliderAdditionTime;

    //public float waitTime => LevelConfig.instance.WaitTime;
    public bool isOver=false;
    public static event UnityAction CountDownOver; // 倒计时结束事件
    public float SliderRatioOnInterrupt { get; private set; } = 0f; // 外部可以访问的比值
    public Text additionalTimeText;//额外时长的显示

    public void OnDisable()
    {

    }

    public IEnumerator CountDown(float waittime,int value)//value==0时，slider为sliderWaitTime，==1时 为sliderAdditionTime
    {
        Slider slider=value==0? sliderWaitTime : sliderAdditionTime;

        slider.gameObject.SetActive(true);
        slider.maxValue = waittime;
        slider.value = waittime;
        isOver = false;
        if (isOver)
        {
            slider.gameObject.SetActive(false);
            yield break;
        }

        while (slider.value > 0)
        {
            if (value == 1)
            {
                additionalTimeText.text=$"额外{slider.value.ToString("f1")}秒";
            }

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
        if (!isOver)
        {
            SliderRatioOnInterrupt = 0;
        }
        slider.gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public IEnumerator EndlessCountDown(float maxValue, int value)
    {
        
        Slider slider = value == 0 ? sliderWaitTime : sliderAdditionTime;
        slider.gameObject.SetActive(true);
        slider.maxValue = maxValue;
        slider.value = maxValue;
        isOver = false;
        if (isOver)
        {
            slider.gameObject.SetActive(false);
            yield break;
        }
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
        slider.gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public IEnumerator GuideCountDown(float maxValue, int value)
    {
        Slider slider = value == 0 ? sliderWaitTime : sliderAdditionTime;
        slider.gameObject.SetActive(true);
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
        if (!isOver)
        {
            SliderRatioOnInterrupt = 0;
        }
        slider.gameObject.SetActive(false);
        CountDownOver?.Invoke();
        yield break;
    }

    public void SetIsOver()
    {
        isOver = true;
    }
}