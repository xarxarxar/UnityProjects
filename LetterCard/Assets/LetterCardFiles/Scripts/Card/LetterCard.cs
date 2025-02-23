using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardState
{
    Front,
    Back
}
// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。
public class LetterCard : Card
{
    [SerializeField] private Text largeLetter;//中间的大字母
    [SerializeField] private Text smallLetter;//左上角的小字母
    [SerializeField] private GameObject backSide;//卡牌的背面
    [SerializeField] private GameObject frontSide;//卡牌的正面
    private bool isFront=false;//是否是正面

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

    private Vector3 originalPosition;  // 记录初始位置，以便拖动结束时恢复
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
                largeLetter.color = GameConfig.colorMap[value];
                smallLetter.color = GameConfig.colorMap[value];
            }

        } 
    }

    //是否在手里，如果不在手里则在暂存池里等待出牌
    private bool isInhand=true;

    public event UnityAction cardToCache;//字母牌到暂存池中去的事件
    public event UnityAction cardBackHand;//字母牌回到暂存池中的事件;

    float cardWidth = 900 / 7.0f;
    


    private void OnEnable()
    {
        base.OnEnable();
        OnLetterCardShow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlipCardToBack();
        }
    }


    /// <summary>
    /// 字母牌出现之后的操作，将字母设置为设定的字母
    /// </summary>
    private void OnLetterCardShow()
    {
        smallLetter.text = Letter.ToString();
        largeLetter.text = Letter.ToString();

        smallLetter.color = GameConfig.colorMap[Color];
        largeLetter.color = GameConfig.colorMap[Color];
    }
 
    /// <summary>
    /// 旋转到背面
    /// </summary>
    private void FlipCardToBack()
    {
        if (!isFront)
        {
            return;//已经是背面
        }
        Vector3 targetRotation = new Vector3(0, 90, 0);
        float duration = 0.5f;
        // 单次旋转（从当前角度到目标角度）
        transform.DORotate(targetRotation, duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                frontSide.SetActive(false);
                backSide.SetActive(true);
                // 单次旋转（从当前角度到目标角度）
                transform.DORotate(Vector3.zero, duration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear);
            });  // 使用线性过渡
        
    }
}