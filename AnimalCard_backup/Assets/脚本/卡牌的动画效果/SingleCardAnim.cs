using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 管理单张卡牌的动画
/// </summary>
public class SingleCardAnim : MonoBehaviour
{
    [Header("Shown")]
    public Transform ShownTransform;

    [Header("遮罩")]
    public SpriteRenderer Mask; // 用于合并提示

    [Header("排列")]
    public string DefaultSortingLayer = "Card";
    public string DraggingSortingLayer = "Dragging";
    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();

    public const float MoveAnimTime = 0.15f;
    public const float SummonAnimTime = 0.2f;
    public const float DisappearAnimTime = 0.2f;

    private DissolveGroupEffect dissolveGroupEffect;

    private void Awake()
    {
        _renderers.AddRange(ShownTransform.GetComponentsInChildren<SpriteRenderer>());
        dissolveGroupEffect =GetComponent<DissolveGroupEffect>();
        SetMaskTransparent();
    }

    #region Move
    /// <summary>
    /// 移动动画
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="onStart"></param>
    /// <param name="onComplete"></param>
    public void PlayMove(
        Vector3 from,
        Vector3 to,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        ShownTransform.DOKill();

        ShownTransform.position = from;

        onStart?.Invoke();

        ShownTransform.DOMove(to, MoveAnimTime)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    #endregion

    #region Summon
    /// <summary>
    /// 召唤动画
    /// </summary>
    /// <param name="onComplete"></param>
    public void PlaySummon(System.Action onComplete = null)
    {
        ShownTransform.DOKill();

        ShownTransform.localScale = Vector3.zero;

        ShownTransform.DOScale(Vector3.one, SummonAnimTime)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
    #endregion

    #region Disappear
    /// <summary>
    /// 消失动画
    /// </summary>
    /// <param name="onComplete"></param>
    public void PlayDisappear(System.Action onComplete = null)
    {
        ShownTransform.DOKill();
        dissolveGroupEffect.Play(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void SetMaskMergeable(bool canMerge)
    {
        if (Mask == null) return;

        Mask.color = canMerge ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    public void SetMaskTransparent()
    {
        if (Mask == null) return;
        Mask.color = new Color(1, 1, 1, 0);
    }

    public void SetDraggingLayer()
    {
        foreach (var r in _renderers)
            r.sortingLayerName = DraggingSortingLayer;
    }

    public void SetDefaultLayer()
    {
        foreach (var r in _renderers)
            r.sortingLayerName = DefaultSortingLayer;
    }

    #endregion

    #region Force Finish

    public void FinishImmediately()
    {
        ShownTransform.DOComplete(true);
        ShownTransform.DOKill();
    }

    #endregion
}
