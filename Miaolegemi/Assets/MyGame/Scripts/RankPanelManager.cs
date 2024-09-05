using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankPanelManager : MonoBehaviour
{

    public GameObject RankInfoPrefab;//排名条预制体
    public Transform RanInfoParent;//排名条父物体
    private void OnEnable()
    {
        ClearRankPrefab();//清空排名条
        CallWechat.instance.GetRankInfo(RankUser);//获取排名信息
    }

    private void OnDisable()
    {
        ClearRankPrefab();//清空排名条
    }

    //生成排名条
    public void RankUser(RankInfo rankInfo)//排名
    {
        for (int i = 0; i < rankInfo.data.Count; i++)
        {
            int currentIndex = i;  // 将 i 值保存到局部变量

            GameObject tmpRank = Instantiate(RankInfoPrefab, RanInfoParent);
            tmpRank.transform.Find("RankText").GetComponent<Text>().text = (currentIndex + 1).ToString();


            ResourceManager.instance.LoadImageFromUrl(rankInfo.data[currentIndex].gamedata.AvatarURL, (texture2D) => {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                tmpRank.transform.Find("AvatarImage").GetComponent<Image>().sprite = sprite;
            });

            tmpRank.transform.Find("NickNameTMP").GetComponent<TextMeshProUGUI>().text = rankInfo.data[currentIndex].gamedata.UserNickName;

            int ownCount = 0;
            for (int j = 0; j < rankInfo.data[currentIndex].gamedata.cardData.Count; j++)
            {
                if (rankInfo.data[currentIndex].gamedata.cardData[j].OwnCard == 1) ownCount += 1;
            }
            tmpRank.transform.Find("UserCollectionText").GetComponent<Text>().text = $"收藏{ownCount}个";

        }
    }

    //清除所有排名条
    public void ClearRankPrefab()
    {
        // 使用循环来销毁子物体
        for (int i = RanInfoParent.childCount - 1; i >= 0; i--)
        {
            // 获取子物体
            Transform child = RanInfoParent.GetChild(i);
            // 销毁子物体
            Destroy(child.gameObject);
        }
    }
}
