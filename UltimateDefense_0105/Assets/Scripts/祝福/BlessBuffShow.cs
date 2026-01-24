using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 在局内显示的祝福Buff
/// </summary>
public class BlessBuffShow : MonoBehaviour, IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
{
    private string _description;
    private float _duration;//祝福持续的时间是多久
    [SerializeField] private Image ShowImage;//显示的图片
    [SerializeField] private MySlider CountdownSlider;//倒计时slider

    public void Init(string description,Sprite sprite,float duration)
    {
        _description=description;
        ShowImage.sprite= sprite;
        _duration=duration;
        CountdownSlider.StartCountDown(_duration);
    }

    /// <summary>
    ///关闭这个buff显示的小圆标
    /// </summary>
    public void Close()
    {
        BlessManager.Instance.BlessBuffShowPool.Return(this);
    }

    private void OnDisable()
    {
        if(GameUIManager.Instance._currentBlessBuff == this)
        {
            GameUIManager.Instance.HideBlessBuffDes();
            GameUIManager.Instance._currentBlessBuff = null;
        }
    }

    //手指按住
    public void OnPointerDown(PointerEventData eventData)
    {
        GameUIManager.Instance.ShowBlessBuffDes(_description);
        GameUIManager.Instance._currentBlessBuff=this;
    }

    //手指抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        GameUIManager.Instance.HideBlessBuffDes();
        GameUIManager.Instance._currentBlessBuff = null;
    }

    // 手指滑出按钮区域，也要关闭
    public void OnPointerExit(PointerEventData eventData)
    {
        GameUIManager.Instance.HideBlessBuffDes();
        GameUIManager.Instance._currentBlessBuff = null;
    }
}
