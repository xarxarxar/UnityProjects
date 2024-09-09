using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSuccessManager : MonoBehaviour
{
    /// <summary>
    /// 展示本局获得多少卡片的面板
    /// </summary>
    public GameObject ShowPanel;

    /// <summary>
    /// item的预设体
    /// </summary>
    public GameObject itemPrefab;

    private GameObject itemLayout;//展示item的面板
    private void OnEnable()
    {
        //获取展示item的面板
        itemLayout = ShowPanel.transform.Find("ItemsLayout").gameObject;

        SetElementSize();//设置元素的大小

        GenerateItems();//生成本局获取的卡片
    }

    private void OnDisable()
    {
        DestroyItems();
    }

    //设置元素的大小
    private void SetElementSize()
    {
        float screenWidth = ResolutionManager.ScreenWidth;//屏幕宽度
        float screenHeight = ResolutionManager.ScreenHeight;//屏幕高度

        //设置ShowPanel的大小
        Tools.SetUiSize(ShowPanel, new Vector2(screenWidth * 0.7f, screenHeight * 0.25f));
        Vector2 showPanelSize = Tools.GetUiSize(ShowPanel);//获取ShowPanel的大小

        Tools.SetUiHeight(itemLayout, showPanelSize.y * 0.7f);//设置itemLayout的高度

        Vector2 itemLayoutSize = Tools.GetUiSize(itemLayout);//获取itemLayout的大小
        //设置GridLayoutGroup的cellsize的值
        itemLayout.GetComponent<GridLayoutGroup>().cellSize = new Vector2(itemLayoutSize.x * 0.15f, itemLayoutSize.x * 0.15f);
        itemLayout.GetComponent<GridLayoutGroup>().spacing = new Vector2(itemLayoutSize.x * 0.02f, itemLayoutSize.x * 0.02f);

        //设置按钮背景所占的大小
        GameObject buttonBackground = ShowPanel.transform.Find("ButtonBackground").gameObject;
        Tools.SetUiHeight(buttonBackground, showPanelSize.y * 0.3f);//设置ButtonBackground的高度

    }

    //生成本局获取的卡片
    private void GenerateItems()
    {
        // 使用 foreach 迭代字典
        int i = 0;
        foreach (KeyValuePair<string, Sprite> entry in ResourceManager.instance.imageDict)
        {
            GameObject tmpItem = Instantiate(itemPrefab, itemLayout.transform);
            // 找到子对象 "SpriteImage" 并获取其 Image 组件，然后将 sprite 设置为字典中的 sprite 值
            tmpItem.transform.Find("SpriteImage").GetComponent<Image>().sprite = entry.Value;
            tmpItem.transform.Find("TextBackGround").Find("Text").GetComponent<Text>().text = (CardManager.instance.currentDiffu/10).ToString();
            i++;
        }

    }

    //删除所有的items
    private void DestroyItems()
    {
        Tools.DestroyAllChildren(itemLayout);
    }
}
