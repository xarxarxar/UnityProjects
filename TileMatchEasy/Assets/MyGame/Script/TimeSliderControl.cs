using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Watermelon;

public class TimeSliderControl : MonoBehaviour
{
    private Slider slider; // Slider 对象
    public Image targetImage; // 要变化颜色的目标 Image

    public Image shakeImage; // 震动的图片
    public float shakeThreshold = 0.1f; // 当 slider 小于等于最大值的百分比值时触发震动
    public float shakeDuration = 0.5f; // 震动持续时间
    public float shakeStrength = 20f; // 震动角度范围，正负值决定旋转的最大角度
    public int shakeRepetitions = 2; // 旋转的次数
    [SerializeField]private bool isShaked = false; // 是否已经震动过

    // 使用 Color32 定义颜色（RGB 值范围是 0-255）
    public Color32 colorAtZero = new Color32(220, 87, 20, 255); // 红色 (RGB: 255, 0, 0, A: 255)
    public Color32 colorAtHalf = new Color32(198, 218, 26, 255); // 绿色 (RGB: 0, 255, 0, A: 255)
    public Color32 colorAtOne = new Color32(34, 184, 207, 255); // 蓝色 (RGB: 0, 0, 255, A: 255)

    public static event UnityAction OnTimerEnded;

    private float sliderMaxValue = 0;
        public float maxTime = 60f;
        private static TimeSliderControl instance;

    void OnEnable()
    {
        
        isShaked = false;
        instance = this;
        slider = GetComponent<Slider>();
        slider.maxValue = maxTime;
        sliderMaxValue = slider.maxValue;
        slider.value = sliderMaxValue;
        // 给 Slider 添加值变化监听器
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // 初始化时更新颜色
        OnSliderValueChanged(slider.value);

        // 开始减值的 Coroutine
        StartCoroutine(DecreaseSliderValueOverTime());
        StartCoroutine(PlaySoundWhenSliderLow());//播放滴滴声的协程
        ShakeImage();//震动沙漏
    }

    void OnSliderValueChanged(float value)
    {
        // 使用 DoTween 动画改变颜色
        targetImage.DOColor(GetColorForSliderValue(value), 0.5f); // 0.5f 表示颜色渐变的持续时间

        
        // 检查 Slider 的值是否小于等于阈值，如果是则震动图片
        if (value <= shakeThreshold* sliderMaxValue && !isShaked)
        {
            Debug.LogWarning($"slider的值是{value}" +
                $"shakeThreshold* sliderMaxValue 是{shakeThreshold * sliderMaxValue}");
            isShaked = true;
            ShakeImage();
        }
    }

    void ShakeImage()
    {
        // 使用 DoTween 的 DOShakeRotation 方法来进行左右旋转的震动
        shakeImage.transform.DORotate(new Vector3(0, 0, shakeStrength), shakeDuration, RotateMode.FastBeyond360)
                             .SetLoops(shakeRepetitions, LoopType.Yoyo); // Yoyo 使旋转来回震动

        // 使用 DOScale 来进行放大和缩小动画
        shakeImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1), shakeDuration) // 放大
                             .SetLoops(2, LoopType.Yoyo) // 放大缩小两次
                             .SetEase(DG.Tweening.Ease.OutQuad); // 使用 Ease，使动画看起来更平滑
    }

    Color GetColorForSliderValue(float value)
    {
        // 获取 slider 的最大值
        float maxValue = sliderMaxValue;

        if (value == 0)
            return colorAtZero;
        else if (value == maxValue)
            return colorAtOne;
        else if (value == 0.5f * maxValue)
            return colorAtHalf;

        // 插值在 colorAtZero, colorAtHalf 和 colorAtOne 之间
        if (value < 0.5f * maxValue)
        {
            // 插值从 colorAtZero 到 colorAtHalf
            // 归一化 value 范围到 [0, 1] 之间
            float normalizedValue = value / (0.5f * maxValue); // 归一化值
            return Color.Lerp(colorAtZero, colorAtHalf, normalizedValue);
        }
        else
        {
            // 插值从 colorAtHalf 到 colorAtOne
            // 归一化 value 范围到 [0, 1] 之间
            float normalizedValue = (value - 0.5f * maxValue) / (0.5f * maxValue); // 归一化值
            return Color.Lerp(colorAtHalf, colorAtOne, normalizedValue);
        }
    }

    // Coroutine 每秒减少 1，直到 Slider 为 0
    IEnumerator DecreaseSliderValueOverTime()
    {
        // 每秒减少 1，直到 Slider 的值为 0
        while (slider.value > 0)
        {
            slider.value -= 0.1f;
            yield return new WaitForSeconds(0.1f); // 每秒减少 1
        }
        LevelController.instance.OnSlotsFilled();//调用卡槽已满的方法
    }

    IEnumerator PlaySoundWhenSliderLow()
    {
        // 当 slider.value 小于 5 时，每秒播放一次音效
        while (slider.value > 0)
        {
            if (slider.value < 5)
            {
                AudioController.PlaySound(AudioController.Sounds.didi);
            }
            yield return new WaitForSeconds(1f); // 每秒播放一次音效
        }
    }

    // 3-match restores countdown time (capped at maxTime).
    public static void AddTime(float amount)
    {
        if (instance == null || instance.slider == null) return;

        instance.slider.value = Mathf.Min(instance.slider.value + amount, instance.slider.maxValue);
    }
}