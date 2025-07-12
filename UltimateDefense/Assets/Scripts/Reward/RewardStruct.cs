using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardStruct : MonoBehaviour
{
    public Image iconImage;     //奖励的图标所在的Image
    public Text countText;  //奖励的个数的Text
    public RewardType type; //奖励的类型


    public void Init(int count)
    {
        countText.text= count.ToString();
    }

    public void Init(RewardType type,int count)
    {
        countText.text = count.ToString();
        this.type = type;
        iconImage.sprite = RewardManager.IconMap[type];
    }

}


