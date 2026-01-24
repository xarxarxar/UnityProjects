using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Rendering;

/// <summary>
/// 管理单张卡牌的动画
/// </summary>
public class SingleCardAnim : MonoBehaviour
{
    [Header("Shown")]
    public Transform ShownTransform;

    [Header("遮罩")]
    public SpriteRenderer Mask; // 用于合并提示以及等级显示
    public GameObject Click; // 用于点击提示

    [Header("排列")]
    public string DefaultSortingLayer = "Card";
    public string DraggingSortingLayer = "Dragging";
    private SortingGroup _sortingGroup;

    public const float MoveAnimTime = 0.15f;
    public const float SummonAnimTime = 0.2f;
    public const float DisappearAnimTime = 0.2f;

    private DissolveGroupEffect dissolveGroupEffect;

    private void Awake()
    {
        dissolveGroupEffect =GetComponent<DissolveGroupEffect>();
        _sortingGroup=GetComponent<SortingGroup>();
        SetMaskTransparent(1);
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

        Mask.color = canMerge ? new Color32(152, 227, 188, 255) : new Color32(255, 147, 129, 255);
    }
    /// <summary>
    /// 设置点击状态
    /// </summary>
    /// <param name="clicking"></param>
    public void SetClick(bool clicking)
    {
        Click.SetActive(clicking);
    }

    public void SetMaskTransparent(int level)
    {
        if (Mask == null) return;
        
        if (level == 1)
        {
            Mask.color = CardManager.NormalColor;
        }
        else if(level == 2)
        {
            Mask.color = CardManager.SecondColor;
        }
        else
        {
            Mask.color = CardManager.ThirdColor;
        }
    }

    public void SetDraggingLayer()
    {
        _sortingGroup.sortingLayerName = DraggingSortingLayer;
    }

    public void SetDefaultLayer()
    {
        _sortingGroup.sortingLayerName = DefaultSortingLayer;
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
