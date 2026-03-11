using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlantingPanel : MonoBehaviour
{
    public static PlantingPanel Instance;
    public GameObject panelObj;     // 商店面板物体
    public Transform contentParent; // 按钮的父容器（挂GridLayoutGroup那个）
    public GameObject buttonPrefab; // 按钮预制体

    private LandSlot currentSelectedSlot;

    void Awake() { Instance = this; }

    void Start()
    {
        panelObj.SetActive(false);
        // 注意：这里不要在Start生成，要在GameManager准备好数据后再生成
    }

    public void SetupButtons()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);
        foreach (FruitType type in FruitGameManager.Instance.allFruitTypes)
        {
            GameObject btnGo = Instantiate(buttonPrefab, contentParent);
            btnGo.GetComponentInChildren<Text>().text = $"{type.name}\n${type.buyPrice}";
            FruitType localType = type;
            btnGo.GetComponent<Button>().onClick.AddListener(() => OnClickPlant(localType));
        }
    }

    public void OpenPanel(LandSlot slot)
    {
        currentSelectedSlot = slot;
        panelObj.SetActive(true);
    }

    void OnClickPlant(FruitType data)
    {
        if (FruitGameManager.Instance.currentGold >= data.buyPrice)
        {
            FruitGameManager.Instance.currentGold -= data.buyPrice;
            currentSelectedSlot.PlantTree(data);
            panelObj.SetActive(false);

        }
        else
        {
            Debug.Log("没钱种树！");
        }
    }

    public void ClosePanel() { panelObj.SetActive(false); }
}