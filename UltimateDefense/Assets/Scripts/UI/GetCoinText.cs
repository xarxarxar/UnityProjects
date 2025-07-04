using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCoinText : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Image _icon;
    [SerializeField] private float _floatDistance = 50f;
    [SerializeField] private float _upDuration = 0.8f;//文字向上飘的时长
    [SerializeField] private float _fadeDuration = 0.8f;//文字变透明的时长

    private CanvasGroup _canvasGroup;

    private float totalUpDuration => _upDuration / BattleManager.Instance.GameSpeed;
    private float totalFadeDuration => _fadeDuration / BattleManager.Instance.GameSpeed;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(Vector3 screenPos, int value)
    {
        _text.transform.localScale = Vector3.one;
        //transform.SetParent(BattleUIManager.Instance.DamageTextParent, false);  // 在 screen-space canvas 上
        transform.position = screenPos;

        _text.text =$" +{value}";

        // 播放动画等
        _canvasGroup.DOFade(1f, 0f); // 设置初始透明度为1

        PlayAnim();
    }


    private void PlayAnim()
    {
        Sequence seq = DOTween.Sequence();

        // 第一步：位置上浮动画
        seq.Append(transform.DOMoveY(transform.position.y + _floatDistance, totalUpDuration).SetEase(Ease.OutCubic));

        // 第二步：淡出动画（上浮完成后再淡出）
        // 整体淡出
        seq.Append(_canvasGroup.DOFade(0f, totalFadeDuration));

        // 动画结束后回收
        seq.OnComplete(() =>
        {
            BattleUIManager.Instance.GetCoinTextPool.Return(this);
            _canvasGroup.alpha = 1f; // 重置 alpha（下次复用）
        });
    }
}
