using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThingItemPanel : BasePanel
{
    public Image ShowImage;
    //public Image PieceShowImage;//碎片显示的Image
    public Text NameTetx;
    public Text DescriptionTetx;

    /// <summary>
    /// 设置物品描述
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="isPiece">是否是碎片</param>
    public void Set(Sprite sprite,string name,string description)//,bool isPiece=false)
    {
        
        NameTetx.text = name;
        DescriptionTetx.text = description;
        //if(isPiece)
        //{
        //    PieceShowImage.transform.parent.gameObject.SetActive(true);
        //    ShowImage.gameObject.SetActive(false);
        //    PieceShowImage.sprite = sprite;
        //}
        //else
        //{
            //PieceShowImage.transform.parent.gameObject.SetActive(false);
            //ShowImage.gameObject.SetActive(true);
            ShowImage.sprite = sprite;
        //}

    }
}
