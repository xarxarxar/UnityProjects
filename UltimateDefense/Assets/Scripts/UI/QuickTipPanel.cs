using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QuickTipPanel : MonoBehaviour
{
    [SerializeField] private GameObject _quickTipObject;
    [SerializeField] private Text _quickTiptext;
    [SerializeField] private float stayTime;//停留时长

    private Sequence _currentSequence;

    /// <summary>
    /// 显示快速提示文字，并播放缩放动画
    /// </summary>
    /// <param name="message">要显示的提示内容</param>
    public void ShowQuickTip(string message)
    {
        // 如果还有旧动画未完成，先终止它
        if (_currentSequence != null && _currentSequence.IsActive())
        {
            _currentSequence.Kill(); // 立即杀掉动画
        }

        _quickTiptext.text = message;
        _quickTipObject.transform.localScale = Vector3.zero;
        _quickTipObject.SetActive(true);
        gameObject.SetActive(true); // 如果面板本身也是动态显示的

        // 创建新的动画序列
        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(_quickTipObject.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
                        .AppendInterval(stayTime)
                        .Append(_quickTipObject.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack))
                        .OnComplete(() =>
                        {
                            _quickTipObject.SetActive(false);
                            gameObject.SetActive(false);
                        });
    }
}
