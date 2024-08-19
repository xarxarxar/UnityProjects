using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    public GameObject collectionCard;
    private void OnEnable()
    {
        AdaptWidthAndHeight();
        CallWechat.instance.GetCardData(GetUserdataSuccess);
    }

    //自适应UI
    private void AdaptWidthAndHeight()
    {
        float cellWidth = ResolutionManager.ScreenWidth / 2;
        float cellHeight = cellWidth /2;
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellWidth, cellHeight);
    }

    //获取该玩家信息成功之后调用的函数
    private void GetUserdataSuccess(OnlineUserdata userdata)
    {
        for (int i = 0; i < userdata.data.cardData.Count; i++)
        {
            Instantiate(collectionCard,transform);
        }
    }
}
