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
    // 字母，表示卡牌上的字母字符，例如 'A'、'b' 等。
    public char letter;

    //卡牌的颜色
    public ColorType color;

    // 是否是大写字母，标识该卡牌上的字母是大写还是小写。
    public bool isUpperCase;

    //是否在手里，如果不在手里则在暂存池里等待出牌
    private bool isInhand=true;

    public event UnityAction cardToCache;//字母牌到暂存池中去的事件
    public event UnityAction cardBackHand;//字母牌回到暂存池中的事件;

    private void Start()
    {
        OnInstantiate();
    }

    /// <summary>
    /// 实例化之后的操作
    /// </summary>
    public void OnInstantiate()
    {
        GetComponent<Button>().onClick.AddListener(OnLetterCardChoose);
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