using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lean.Touch;


// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。

public class LetterCard : Card
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
    //public bool isUpperCase;

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

    float cardWidth = 900 / 7.0f;

    private float lastClickTime = 0;
    private const float doubleClickTime = 0.3f; // 双击最大间隔时间
    private Vector3 dragOffset;
    private bool waitingForSecondClick = false;

    private void Start()
    {
        OnInstantiate();//实例化之后的操作
    }

    private void OnEnable()
    {
        base.OnEnable();
        OnLetterCardShow();
        LeanTouch.OnFingerSwipe += OnFingerSwipe;

    }

    /// <summary>
    /// 实例化之后的操作
    /// </summary>
    public void OnInstantiate()
    {

        dropArea = ShowTipManager.instance.dustbinGameobject.GetComponent<RectTransform>();
        //Debug.Log($"ScreenWidth为{WechatManager.ScreenWidth},ScreenHeight为{WechatManager.ScreenHeight},windowWidth={WechatManager.WindowWidth},windowHeight={WechatManager.WindowHeight},dpr为{WechatManager.DPR}");
    }

    // 处理滑动事件
    private void OnFingerSwipe(LeanFinger finger)
    {
        Debug.Log("click");
        if (finger.IsOverGui) return; // 忽略 UI 上的操作
        // 将屏幕坐标转换为世界坐标
        Vector3 worldPos = finger.GetWorldPosition(10f); // Z 深度
        transform.position = worldPos;
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
























}