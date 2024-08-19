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

    //����ӦUI
    private void AdaptWidthAndHeight()
    {
        float cellWidth = ResolutionManager.ScreenWidth / 2;
        float cellHeight = cellWidth /2;
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellWidth, cellHeight);
    }

    //��ȡ�������Ϣ�ɹ�֮����õĺ���
    private void GetUserdataSuccess(OnlineUserdata userdata)
    {
        for (int i = 0; i < userdata.data.cardData.Count; i++)
        {
            Instantiate(collectionCard,transform);
        }
    }
}
