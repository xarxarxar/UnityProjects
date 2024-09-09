using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public static ButtonManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject startScene;//游戏开始的场景
    [Header("开始按钮")]
    public GameObject gameScene;//游戏主场景
    public GameObject cutScenes;//过场动画的UI元素
    public void StartButton()
    {
        startScene.SetActive(false);
        cutScenes.SetActive(true);
        gameScene.SetActive(true);
    }

    [Header("仓库按钮")]
    public GameObject collectScene;//仓库的场景
    public void CollectButton()
    {
        startScene.SetActive(false);
        collectScene.SetActive(true);
    }

    [Header("设置按钮")]
    public GameObject settingScene;//设置面板
    public void SettingButton()
    {
        settingScene.SetActive(true);
    }

    [Header("放弃挑战按钮")]
    public GameObject tipPanel;//提醒是否放弃跳挑战
    public void GiveUpButton()//放弃挑战按钮
    {
        tipPanel.SetActive(true);
    }


    public void ConfrimGiveUp()//确认放弃按钮
    {
        tipPanel.SetActive(false);
        cutScenes.SetActive(true);
        settingScene.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }
    public void ContinueButton()//继续挑战按钮
    {
        tipPanel.SetActive(false);
    }
    public void BackToStart()//从仓库界面返回开始界面
    {
        collectScene.SetActive(false);
        startScene.SetActive(true) ;
    }

    [Header("道具按钮")]
    public  Button addSlotButton;
    public  Button shuffleButton;
    public  Button reliveButton;
    //增加卡槽按钮
    public void AddSlotButton()
    {
        GameProp.instance.AddSlot();//增加卡槽
        GameProp.instance.addSlotCount--;//增加卡槽次数减1

        if (GameProp.instance.addSlotCount < 1)
        {
            addSlotButton.interactable = false;
        }
    }
    //洗牌按钮
    public void ShuffleButton()
    {
        GameProp.instance.Shuffle();//洗牌
        GameProp.instance.shuffleCount--;//洗牌次数减1

        if (GameProp.instance.shuffleCount < 1)
        {
            shuffleButton.interactable = false;
        }
    }
    //复活按钮
    public void ReliveButton()
    {
        GameProp.instance.TransferCards();//移出所有的卡槽里的牌
        GameProp.instance.reliveCount--;//复活数减1

        if (GameProp.instance.reliveCount < 1)
        {
            reliveButton.interactable = false;
        }
    }

    [Header("排名按钮")]
    public GameObject RankInfoPanel;//排名面板
    public GameObject RankInfoPrefab;//排名条预制体
    public Transform RanInfoParent;//排名条父物体
    public void GetRankButton()
    {
        RankInfoPanel.SetActive(true);//打开排名面板
        //CallWechat.instance.GetRankInfo(RankUser);//获取排名信息
        //CallWechat.instance.GetUserWechatIDandAvatar();
        //CallWechat.instance.GetUserInfo();
    }
    public void RankUser(RankInfo rankInfo)//排名
    {
        for (int i = 0; i < rankInfo.data.Count; i++)
        {
            int currentIndex = i;  // 将 i 值保存到局部变量

            Debug.Log("RankUser:"+rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser:"+rankInfo.data[currentIndex].gamedata.UserNickName);

            GameObject tmpRank = Instantiate(RankInfoPrefab, RanInfoParent);
            tmpRank.transform.Find("RankText").GetComponent<Text>().text = (currentIndex + 1).ToString();

            Debug.Log("RankUser01:" + rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser01:" + rankInfo.data[currentIndex].gamedata.UserNickName);

            ResourceManager.instance.LoadImageFromUrl(rankInfo.data[currentIndex].gamedata.AvatarURL, (texture2D) => {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                tmpRank.transform.Find("AvatarImage").GetComponent<Image>().sprite = sprite;
            });
            Debug.Log("RankUser02:" + rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser02:" + rankInfo.data[currentIndex].gamedata.UserNickName);
            tmpRank.transform.Find("NickNameTMP").GetComponent<TextMeshProUGUI>().text = rankInfo.data[currentIndex].gamedata.UserNickName;

            int ownCount = 0;
            for (int j = 0; j < rankInfo.data[currentIndex].gamedata.cardData.Count; j++)
            {
                if (rankInfo.data[currentIndex].gamedata.cardData[j].OwnCard == 1) ownCount += 1;
            }
            tmpRank.transform.Find("UserCollectionText").GetComponent<Text>().text = $"收藏{ownCount}个";
            
        }
    }

    //关闭排行面板按钮
    public void CloseRankPanelButton()
    {
        RankInfoPanel.SetActive(false);
    }

    [Header("排名按钮")]
    public GameObject SuccessPanel;//成功面板
    //成功之后的面板里的确定按钮
    public void SuccessConfirmButton()
    {
        SuccessPanel.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }
}

[System.Serializable]
public class RankInfo
{
    public List<RankUser> data;
}

[System.Serializable]
public class RankUser
{
    public string _id;
    public string openid;
    public GameData gamedata;
    public int ownCardCount;
}

[System.Serializable]
public class GameData
{
    public string AvatarURL;
    public string UserNickName;
    public int UserScore;
    public List<CardData> cardData;
    public myUserInfo userInfo;
}
