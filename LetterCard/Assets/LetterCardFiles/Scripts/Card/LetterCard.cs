using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。
[RequireComponent(typeof(Button))] // 确保按钮组件存在
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
    
    //卡牌的颜色
    private ColorType color;
    public ColorType Color 
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
    Dictionary<ColorType, Color32> colorMap = new Dictionary<ColorType, Color32>
        {
            { ColorType.Red, new Color32(194,24,91,255) },
            { ColorType.Green, new Color32(56,142,60,255) },
            { ColorType.Blue, new Color32(48,63,159,255) },
            { ColorType.Yellow, new Color32(255,162,0,255) }
        };

    

    private void Start()
    {
        OnInstantiate();//实例化之后的操作
    }

    private void OnEnable()
    {
        OnLetterCardShow();
    }

    /// <summary>
    /// 实例化之后的操作
    /// </summary>
    public void OnInstantiate()
    {
        GetComponent<Button>().onClick.AddListener(OnLetterCardChoose);
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
            transform.SetParent(GameObject.Find("CachePool").transform);
            cardToCache?.Invoke();
            isInhand =false;
        }
        else
        {
            transform.SetParent(GameObject.Find("LetterHandPool").transform);
            cardBackHand?.Invoke();
            isInhand = true;
        }
    }
}