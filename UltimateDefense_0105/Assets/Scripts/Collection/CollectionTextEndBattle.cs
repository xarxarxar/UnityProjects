using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionTextEndBattle : MonoBehaviour
{
    [HideInInspector]public int index;//文字
    public Text txt;
    [HideInInspector] public Color32 color;
    public Text CountText;//数量
    [HideInInspector] public int Count;

    //设置该文字的文本和颜色
    public void SetCollectionText(int i, Color32 color,int count)
    {
        index = i;
        txt.text = CollectionManager.Chars[i].ToString();
        txt.color = color;
        Count=count;
        CountText.text = count.ToString();
    }
}
