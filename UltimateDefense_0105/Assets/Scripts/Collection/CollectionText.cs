using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 收集的文字
/// </summary>
public class CollectionText : MonoBehaviour
{
    public Text txt;//文字
    public Color32 color;
    public Text Count;//数量
    public GameObject background;

    //设置该文字的文本和颜色
    public void SetCollectionText(char c,Color32 color,int count)
    {
        txt.text=c.ToString();
        txt.color= color;
        Count.text = count.ToString();
        if (count <= 0)
        {
            background.SetActive(true);
            Count.gameObject.SetActive(false);
        }
        else 
        {
            background.SetActive(false);
            Count.gameObject.SetActive(true);
        }
    }
}
