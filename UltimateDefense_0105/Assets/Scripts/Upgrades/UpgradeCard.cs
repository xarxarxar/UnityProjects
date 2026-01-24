using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private Text _descriptionText;//描述文本
    [SerializeField] private Text _costText;//描述文本
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;//滚动区域
    private UpgradeBase _upgradeBase;
    private Bindable<bool> isStopped = new Bindable<bool> { Value=false};

    /// <summary>
    /// 外界只能读取
    /// </summary>
    public UpgradeBase UpgradeBase { get => _upgradeBase;}

    public void Init()
    {
        UpgradeBase upgradeBase = UpgradeFactory.GetRandomUpgrades(1)[0];
        _upgradeBase = upgradeBase;
        _descriptionText.text= UpgradeBase.Description;
        _costText.text= UpgradeBase.Cost.ToString();
        UpdateTextColor();
    }

    /// <summary>
    /// 直接设置售价
    /// </summary>
    /// <param name="cost"></param>
    public void SetCost(int cost)
    {
        _costText.text = cost.ToString();
        UpgradeBase.Cost=cost;
        UpdateTextColor();
    }

    /// <summary>
    /// 在 n 秒内平滑动画方式设置售价
    /// </summary>
    /// <param name="discount">打的折扣</param>
    /// <param name="duration">动画时长（秒）</param>
    public void SetCost(float discount, float duration, UnityAction onComplete = null)
    {
        UpgradeBase.Cost = Mathf.RoundToInt(_upgradeBase.Cost * discount);

        int startValue = 0;
        int.TryParse(_costText.text, out startValue); // 更稳妥
        int endValue = UpgradeBase.Cost;

        DOTween.Kill(_costText);
        DOTween.Kill(_costText.transform);

        _costText.transform.localScale = Vector3.one * 2.0f;
        UpdateTextColor();

        DOTween.To(() => startValue, x =>
        {
            _costText.text = x.ToString();
        },
        endValue, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            _costText.transform.DOScale(Vector3.one, 0.1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _costText.color = new Color32(239, 241, 245, 255);
                    UpdateTextColor();
                    onComplete?.Invoke();//回调触发
                });
        });
    }

    public void UpdateTextColor()
    {
        _costText.color=CurrencyManager.Instance.Gold >= _upgradeBase .Cost? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255);
    }
}
