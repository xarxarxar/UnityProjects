using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LetterCard 类是Card的子类，表示字母卡牌。
// 字母卡牌拥有一个字母和大小写的属性。
public class LetterCard : Card
{
    // 字母，表示卡牌上的字母字符，例如 'A'、'b' 等。
    public char letter;

    //卡牌的颜色
    public ColorType color;

    // 是否是大写字母，标识该卡牌上的字母是大写还是小写。
    public bool isUpperCase;
}