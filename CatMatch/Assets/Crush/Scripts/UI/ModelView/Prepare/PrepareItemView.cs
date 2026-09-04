using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PrepareSlotType
{
    None,
    Empty,
    Lock,
    DontHaveBeast
}

public class PrepareItemView : MonoBehaviour
{
    public Image bg;
    public Image icon;
    public Image starImg;
    public Text lvlTxt;
    public Text starTxt;

    public Sprite emptySp;
    public Sprite lockSp;
    public Sprite dontHaveBeastSp;

    public void SetUp(PrepareSlotType prepareSlotType, Sprite sprite, string lvl, string star)
    {
        icon.gameObject.SetActive(sprite != null);
        lvlTxt.gameObject.SetActive(sprite != null);
        starTxt.gameObject.SetActive(sprite != null);
        starImg.gameObject.SetActive(sprite != null);

        if (sprite != null)
        {
            bg.sprite = emptySp;

            icon.sprite = sprite;
            lvlTxt.text = "等级" + lvl;
            starTxt.text = star;
        }
        else
        {
            if (prepareSlotType == PrepareSlotType.Empty)
                bg.sprite = emptySp;
            if (prepareSlotType == PrepareSlotType.Lock)
                bg.sprite = lockSp;
            if (prepareSlotType == PrepareSlotType.DontHaveBeast)
                bg.sprite = dontHaveBeastSp;
        }
    }
}
