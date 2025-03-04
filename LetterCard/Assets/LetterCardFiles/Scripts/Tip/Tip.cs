using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class Tip : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;  // 你的UI元素（通常是RectTransform）
    private Vector3 targetPosition;  // 最终目标位置
    public float duration = 20.0f;     // 动画时长
    public string showString;//显示的文字
    public Text showText;//显示的文字

    public Text countText;//数量文字
    public int countNumber;//数量文字

    void OnEnable()
    {
        targetPosition = uiElement.position+new Vector3(0,100,0);
        
        // 调用方法，开始动画
        AnimateUI();
    }

    void AnimateUI()
    {
        // 动画组合，UI元素向下移动、变小、变透明
        uiElement.DOAnchorPosY(uiElement.anchoredPosition.y+ 200, duration)        // 向目标位置移动
            .SetEase(Ease.OutQuad)                  // 设置缓动效果
            .OnStart(() => {
                showText.text = showString;
                countText.text = $"+{countNumber}";
                uiElement.localScale =0.5f* Vector3.one;  // 确保开始时 UI 为正常大小

                //float textWidth = showText.preferredWidth == 0 ? -50 : showText.preferredWidth;
                //float textHeight = showText.preferredHeight == 0 ? -50 : showText.preferredHeight;
                // 调整背景图片的大小
                GetComponent<RectTransform>().sizeDelta = new Vector2(700 , 150); // 设置宽度，保持高度不变
            })
            .OnComplete(() => {
                // 动画完成后可以执行的操作（例如销毁UI等）
                Destroy(gameObject); // 示例：销毁UI元素
            });

        uiElement.DOScale(1.0f, duration/2)                   // 变小到 0
           .SetEase(Ease.OutQuad);                  // 设置缓动效果
    }
}
