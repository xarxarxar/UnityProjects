using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// panel的基础父类
/// </summary>
public class BasePanel : MonoBehaviour
{
    public Button CloseButton;//面板的关闭按钮

    public virtual void OnEnable()
    {
        // 先移除旧的监听，避免重复绑定
        CloseButton.onClick.RemoveListener(OnCloseButton);
        CloseButton.onClick.AddListener(OnCloseButton);

        // 初始缩放设为 0
        transform.localScale = Vector3.zero;

        // 动画缩放到 1（0.5 秒，使用 Ease.OutBack 有弹性感）
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);

        InitPanel();
    }

    public void OnCloseButton()
    {
        // 动画缩放到 0，然后关闭面板
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InQuad)
            .OnComplete(
            () => {
                OnEnd();
            } );
    }

    /// <summary>
    /// 这个是初始化面板的UI状态，每次打开面板的时候用
    /// </summary>
    protected virtual void InitPanel()
    {

    }

    /// <summary>
    /// 初始化面板的数据逻辑，由Manager统一调用
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// 结束的时候调用
    /// </summary>
    public virtual void OnEnd()
    {
        gameObject.SetActive(false);
    }
}
