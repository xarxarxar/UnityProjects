using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class DamageText : MonoBehaviour
{
    [SerializeField]private Text _text;
    [SerializeField] private float _floatDistance = 50f;
    [SerializeField] private float _upDuration = 0.8f;//文字向上飘的时长
    [SerializeField] private float _fadeDuration = 0.8f;//文字变透明的时长

    private float totalUpDuration => _upDuration;
    private float totalFadeDuration => _fadeDuration;

    public void Init(Vector3 screenPos, bool isCrit, int damage)
    {
        _text.transform.localScale = Vector3.one;
        //transform.SetParent(BattleUIManager.Instance.DamageTextParent, false);  // 在 screen-space canvas 上
        transform.position = screenPos;

        _text.text = damage.ToString();
        _text.transform.localScale = isCrit ? Vector3.one * 1.2f : Vector3.one;
        _text.color = isCrit ? Color.yellow : Color.white;

        // 播放动画等
        _text.DOFade(1f, 0f); // 设置初始透明度为1

        PlayAnim();
    }


    private void PlayAnim()
    {
        Sequence seq = DOTween.Sequence();

        // 第一步：位置上浮动画
        seq.Append(transform.DOMoveY(transform.position.y + _floatDistance, totalUpDuration).SetEase(Ease.OutCubic));

        // 第二步：淡出动画（上浮完成后再淡出）
        seq.Append(_text.DOFade(0f, totalFadeDuration));

        // 动画结束后回收
        seq.OnComplete(() =>
        {
        });
    }

}
