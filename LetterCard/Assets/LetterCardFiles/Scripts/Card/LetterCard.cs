using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。
public class LetterCard : Card
{
    [SerializeField] private Text letterText;//中间的文字
    [SerializeField] private GameObject backSide;//卡牌的背面
    [SerializeField] private GameObject frontSide;//卡牌的正面
    [SerializeField] private Transform canvasFather;//卡牌正反面的父物体
    [SerializeField] private bool isFront=true;//是否是正面
    

    // 字母，表示卡牌上的字母字符，例如 'A'、'b' 等。
    private char letter;
    public char Letter 
    { 
        get => letter; 
        set 
        {
            if (letter != value)
            {
                letter = value;
                letterText.text = value.ToString();
            }
        }
    }

    //卡牌的颜色
    private char color='R';
    public char Color 
    { 
        get => color; 
        set
        {
            if (color != value)
            {
                color = value;
                letterText.color = GameConfig.colorMap[value];
            }

        } 
    }

    SimpleTouch simpleTouch;
    private Vector3 offset; // 偏移量，用来保持拖动时鼠标和物体之间的相对位置


    private new void OnEnable()
    {
        base.OnEnable();
        simpleTouch = GetComponent<SimpleTouch>();
        simpleTouch.onBeginDrag += OnCardBeginDrag;//开始拖拽的方法
        simpleTouch.onDrag += OnCardDrag;//拖拽中的方法
        simpleTouch.onEndDrag += OnCardEndDrag;//拖拽中的方法

        OnLetterCardShow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            FlipCardToBack(1.0f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlipCardToFront(1.0f);
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    private void OnCardBeginDrag(PointerEventData eventData)
    {
        transform.DOScale(1.2f * Vector3.one, 0.2f).SetEase(Ease.OutQuad);
        // 将屏幕点击位置转换为世界坐标
        Vector3 clickWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        clickWorldPos.z = transform.position.z; // 保持 Z 轴一致

        // 计算偏移量：物体当前位置 - 点击位置的世界坐标
        offset = transform.position - clickWorldPos;
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    private void OnCardDrag(PointerEventData eventData)
    {
        // 将当前鼠标位置转换为世界坐标
        Vector3 currentWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        currentWorldPos.z = transform.position.z; // 保持 Z 轴一致

        // 更新物体位置：当前鼠标位置 + 初始偏移量
        transform.position = currentWorldPos + offset;
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    private void OnCardEndDrag(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 字母牌出现之后的操作，将字母设置为设定的字母
    /// </summary>
    private void OnLetterCardShow()
    {
        letterText.text = Letter.ToString();
        letterText.color = GameConfig.colorMap[Color];
    }
 
    /// <summary>
    /// 旋转到背面
    /// </summary>
    public Sequence FlipCardToBack(float duration)
    {
        if (!isFront)
        {
            return null; // 已经是背面
        }

        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(frontSide.transform.DOScaleX(0, duration)
            .SetEase(Ease.Linear));

        // 第二个旋转动画：从目标角度旋转回零角度
        sequence.Append(backSide.transform.DOScaleX(1, duration)
            .SetEase(Ease.Linear));

        // 在第二个旋转动画完成后更新状态
        sequence.AppendCallback(() =>
        {
            isFront = false; // 现在是背面
        });

        return sequence;
    }

    /// <summary>
    /// 旋转到正面
    /// </summary>
    public Sequence FlipCardToFront(float duration)
    {
        Debug.Log($"isFront is {isFront}");
        if (isFront)
        {
            return null; // 已经是背面
        }

        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        // 第二个旋转动画：从目标角度旋转回零角度
        sequence.Append(backSide.transform.DOScaleX(0, duration)
            .SetEase(Ease.Linear));

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(frontSide.transform.DOScaleX(1, duration)
            .SetEase(Ease.Linear));

        // 在第二个旋转动画完成后更新状态
        sequence.AppendCallback(() =>
        {
            isFront = true; // 现在是背面
        });

        return sequence;
    }
}