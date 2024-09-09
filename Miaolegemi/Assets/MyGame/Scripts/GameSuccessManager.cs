using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSuccessManager : MonoBehaviour
{
    /// <summary>
    /// չʾ���ֻ�ö��ٿ�Ƭ�����
    /// </summary>
    public GameObject ShowPanel;

    /// <summary>
    /// item��Ԥ����
    /// </summary>
    public GameObject itemPrefab;

    private GameObject itemLayout;//չʾitem�����
    private void OnEnable()
    {
        //��ȡչʾitem�����
        itemLayout = ShowPanel.transform.Find("ItemsLayout").gameObject;

        SetElementSize();//����Ԫ�صĴ�С

        GenerateItems();//���ɱ��ֻ�ȡ�Ŀ�Ƭ
    }

    private void OnDisable()
    {
        DestroyItems();
    }

    //����Ԫ�صĴ�С
    private void SetElementSize()
    {
        float screenWidth = ResolutionManager.ScreenWidth;//��Ļ���
        float screenHeight = ResolutionManager.ScreenHeight;//��Ļ�߶�

        //����ShowPanel�Ĵ�С
        Tools.SetUiSize(ShowPanel, new Vector2(screenWidth * 0.7f, screenHeight * 0.25f));
        Vector2 showPanelSize = Tools.GetUiSize(ShowPanel);//��ȡShowPanel�Ĵ�С

        Tools.SetUiHeight(itemLayout, showPanelSize.y * 0.7f);//����itemLayout�ĸ߶�

        Vector2 itemLayoutSize = Tools.GetUiSize(itemLayout);//��ȡitemLayout�Ĵ�С
        //����GridLayoutGroup��cellsize��ֵ
        itemLayout.GetComponent<GridLayoutGroup>().cellSize = new Vector2(itemLayoutSize.x * 0.15f, itemLayoutSize.x * 0.15f);
        itemLayout.GetComponent<GridLayoutGroup>().spacing = new Vector2(itemLayoutSize.x * 0.02f, itemLayoutSize.x * 0.02f);

        //���ð�ť������ռ�Ĵ�С
        GameObject buttonBackground = ShowPanel.transform.Find("ButtonBackground").gameObject;
        Tools.SetUiHeight(buttonBackground, showPanelSize.y * 0.3f);//����ButtonBackground�ĸ߶�

    }

    //���ɱ��ֻ�ȡ�Ŀ�Ƭ
    private void GenerateItems()
    {
        // ʹ�� foreach �����ֵ�
        int i = 0;
        foreach (KeyValuePair<string, Sprite> entry in ResourceManager.instance.imageDict)
        {
            GameObject tmpItem = Instantiate(itemPrefab, itemLayout.transform);
            // �ҵ��Ӷ��� "SpriteImage" ����ȡ�� Image �����Ȼ�� sprite ����Ϊ�ֵ��е� sprite ֵ
            tmpItem.transform.Find("SpriteImage").GetComponent<Image>().sprite = entry.Value;
            tmpItem.transform.Find("TextBackGround").Find("Text").GetComponent<Text>().text = (CardManager.instance.currentDiffu/10).ToString();
            i++;
        }

    }

    //ɾ�����е�items
    private void DestroyItems()
    {
        Tools.DestroyAllChildren(itemLayout);
    }
}
