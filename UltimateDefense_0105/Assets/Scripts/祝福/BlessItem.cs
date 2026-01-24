using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于在对局外的祝福仓库按钮显示的祝福
/// </summary>
public class BlessItem : MonoBehaviour
{
    [SerializeField] private Text NameText;
    [SerializeField] private Text DesText;
    [SerializeField] private Text RemainText;//仓库剩余多少个
    [SerializeField] private Image IconImage;//图标
    [SerializeField] private Image BackgroundImage;//背景图片
    [SerializeField] private Image CircleImage;//圆圈的图片
    [SerializeField] private Image NameBackgroundImage;//名称背景的图片
    [SerializeField] private Image BlackBgImage;//名称背景的图片

    private Bless _bless;
    private int _rarity;
    private int _count;

    public void Init()
    {

    }

    public void SetItem(Bless bless,int rarity,int count)
    {
        _bless=bless;
        _rarity=rarity;
        _count=count;

        NameText.text= _bless.BlessName;
        DesText.text = _bless.Descriptions[rarity];
        if(rarity == 0)
        {
            RemainText.text = $"仓库剩余无限个";//∞
        }
        else
        {
            RemainText.text = $"仓库剩余{_count}个";
        }
        
        IconImage.sprite=_bless.sprite;

        if (rarity == 0)
        {
            BackgroundImage.color = BlessDataManager.Instance.normalColor;
            CircleImage.color = BlessDataManager.Instance.normalColor;
            NameBackgroundImage.color = BlessDataManager.Instance.normalColor;
            RemainText.color = BlessDataManager.Instance.normalColor;
        }
        else if (rarity == 1)
        {
            BackgroundImage.color = BlessDataManager.Instance.rareColor;
            CircleImage.color = BlessDataManager.Instance.rareColor;
            NameBackgroundImage.color = BlessDataManager.Instance.rareColor;
            RemainText.color = BlessDataManager.Instance.rareColor;
        }
        else if (rarity == 2)
        {
            BackgroundImage.color = BlessDataManager.Instance.epicColor;
            CircleImage.color = BlessDataManager.Instance.epicColor;
            NameBackgroundImage.color = BlessDataManager.Instance.epicColor;
            RemainText.color = BlessDataManager.Instance.epicColor;
        }
        BlackBgImage.gameObject.SetActive(rarity!=0&& count<=0);
    }
}
