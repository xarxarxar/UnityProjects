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


    private void OnEnable()
    {
        simpleTouch = GetComponent<SimpleTouch>();
        simpleTouch.onBeginDrag += OnCardBeginDrag;//开始拖拽的方法
        simpleTouch.onDrag += OnCardDrag;//拖拽中的方法
        simpleTouch.onEndDrag += OnCardEndDrag;//拖拽中的方法

        OnLetterCardShow();
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
 
    
}