using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。
[RequireComponent(typeof(Button))] // 确保按钮组件存在
public class LetterCard : Card, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField] private Text largeLetter;//中间的大字母
    [SerializeField] private Text smallLetter;//左上角的小字母

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
                largeLetter.text = value.ToString();
                smallLetter.text = value.ToString();
            }
        }
    }

    RectTransform rectTransform;

    private Vector3 originalPosition;  // 记录初始位置，以便拖动结束时恢复
    private bool alreadyDrag = false;//是否已经在拖动
    public  RectTransform dropArea;  // 目标区域（拖动物体需要进入的区域）
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
                largeLetter.color = colorMap[value];
                smallLetter.color = colorMap[value];
            }

        } 
    }

    

    // 是否是大写字母，标识该卡牌上的字母是大写还是小写。
    public bool isUpperCase;

    //是否在手里，如果不在手里则在暂存池里等待出牌
    private bool isInhand=true;

    public event UnityAction cardToCache;//字母牌到暂存池中去的事件
    public event UnityAction cardBackHand;//字母牌回到暂存池中的事件;


    // 使用字典映射 ColorType 到 Color
    Dictionary<char, Color32> colorMap = new Dictionary<char, Color32>
        {
            { 'R', new Color32(194,24,91,255) },
            { 'G', new Color32(56,142,60,255) },
            { 'B', new Color32(48,63,159,255) },
            { 'Y', new Color32(255,162,0,255) }
        };

    

    private void Start()
    {
        OnInstantiate();//实例化之后的操作
    }

    private void OnEnable()
    {
        base.OnEnable();
        OnLetterCardShow();
    }

    /// <summary>
    /// 实例化之后的操作
    /// </summary>
    public void OnInstantiate()
    {
        GetComponent<Button>().onClick.AddListener(OnLetterCardChoose);
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cardWidth, rectTransform.sizeDelta.y);

        dropArea = ShowTipManager.instance.dustbinGameobject.GetComponent<RectTransform>();
        //Debug.Log($"ScreenWidth为{WechatManager.ScreenWidth},ScreenHeight为{WechatManager.ScreenHeight},windowWidth={WechatManager.WindowWidth},windowHeight={WechatManager.WindowHeight},dpr为{WechatManager.DPR}");
    }

    /// <summary>
    /// 字母牌出现之后的操作，将字母设置为设定的字母
    /// </summary>
    private void OnLetterCardShow()
    {
        smallLetter.text = Letter.ToString();
        largeLetter.text = Letter.ToString();

        smallLetter.color = colorMap[Color];
        largeLetter.color = colorMap[Color];
    }

    private void OnLetterCardChoose()
    {
        if (isInhand)
        {
            if (!alreadyDrag)
            {
                transform.SetParent(DeckManager.instance.cachePool, worldPositionStays: false);
                CacheText.AddCharacterWithColor(color, letter);
                cardToCache?.Invoke();
                isInhand = false;
            }
            
        }
        else
        {
            transform.SetParent(DeckManager.instance.LetterHandCard, worldPositionStays: false);
            CacheText.RemoveCharacterWithColor(color, letter);
            cardBackHand?.Invoke();
            isInhand = true;
        }
    }

    // 拖动过程中更新 UI 元素的位置
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("拖动中");
        SpecialCardState.instance.IsDeleting = true;
        if (!alreadyDrag)//如果刚开始拖动
        {
            originalPosition= rectTransform.anchoredPosition;
            alreadyDrag=true;
        }

        rectTransform.position = eventData.position;

        // 确保使用世界空间的坐标来进行判断
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
        if (RectTransformUtility.RectangleContainsScreenPoint(dropArea, screenPoint, Camera.main))
        {
            // 触发指定区域的成功事件（你可以在这里调用方法，或者改变 UI）
            Debug.Log("有交叉");
            ShowTipManager.instance.DustbinRed();
        }
        else
        {
            ShowTipManager.instance.DustbinWhite();
        }
    }

    // 拖动结束时检查是否进入指定区域
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("拖动结束");
        SpecialCardState.instance.IsDeleting = false;
        //// 检查是否在目标区域内
        ///// 确保使用世界空间的坐标来进行判断
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
        if (RectTransformUtility.RectangleContainsScreenPoint(dropArea, screenPoint, Camera.main))
        {
            // 触发指定区域的成功事件（你可以在这里调用方法，或者改变 UI）
            DeckManager.instance.cardPool.ReturnCard(this);
        }
        else
        {
            // 如果没有进入指定区域，可以选择将物体放回初始位置
            MoveBackToOriginalPosition();
        }
        
        
    }

    // 点击抬起时触发的方法
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("抬起");
        //// 检查是否在目标区域内
        //if (RectTransformUtility.RectangleContainsScreenPoint(dropArea, eventData.position, Camera.main))
        //{
        //    // 触发成功的方法
        //    TriggerSuccess();
        //}
        //else
        //{
        //    // 如果没有在指定区域，可以选择将物体放回初始位置
        //    rectTransform.position = originalPosition;
        //}
    }

    // 成功拖动到目标区域时调用的方法
    private void TriggerSuccess()
    {
        Debug.Log("成功拖动到指定区域！");
        //if (successMessage != null)
        //{
        //    successMessage.SetActive(true);  // 显示成功消息
        //}
        //// 你可以在这里添加任何你需要触发的逻辑或动画
    }

    // 使用 DOTween 将物体平滑移动回原位置
    private void MoveBackToOriginalPosition()
    {
        rectTransform.DOAnchorPos(originalPosition, 0.5f)  // 平滑移动回原位置
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                alreadyDrag = false;//设置为不在拖动中
            });  // 设置缓动效果
    }
}