using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 生成关卡，初始化关卡，
/// </summary>
public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    public List<GameObject> lights = new List<GameObject>();
    public GameObject lightPrefab; // 灯光预制体
    public Transform lightPrefabParent; // 灯光父物体

    private Color32 normalColor_1=new Color32(208,235,255,255);
    private Color32 greenColor_1 = new Color32(28, 194, 177, 255);
    private Color32 redColor_1 = new Color32(255, 80, 80, 255);
    private Color32 normalColor_2 = new Color32(255, 235, 255, 255);
    private Color32 greenColor_2 = new Color32(71, 217, 185, 255);
    private Color32 redColor_2 = new Color32(255, 106, 106, 255);

    private void Awake() => instance = this;

    private void Update()
    {

    }

    public void GenerateLevel(int level)
    {
        if (level <= 1)
        {
            GenerateGuideLevel();
            return;
        } 

        ClearLights();

        LevelConfig.instance.RoundCount = Random.Range(LevelConfig.minRoundCount, LevelConfig.maxRoundCount);

        LevelConfig.instance.gestureCategory.Clear();
        for (int i = 0; i < LevelConfig.instance.RoundCount; i++)
        {
            LevelConfig.instance.gestureCategory.Add(Random.Range(0, 3));
            lights.Add(Instantiate(lightPrefab, lightPrefabParent));
        }
    }

    /// <summary>
    /// 生成第一关，也就是新手指引
    /// </summary>
    public void GenerateGuideLevel()
    {
        ClearLights();
        LevelConfig.instance.RoundCount = 3;//三个手势都来一遍
        LevelConfig.instance.gestureCategory.Clear();
        //三个手势都来一遍
        LevelConfig.instance.gestureCategory.Add(0);
        LevelConfig.instance.gestureCategory.Add(1);
        LevelConfig.instance.gestureCategory.Add(2);

        for (int i = 0; i < LevelConfig.instance.RoundCount; i++)
        {
            lights.Add(Instantiate(lightPrefab, lightPrefabParent));
        }
    }

    public void GenerateEndlessMode()
    {

        ClearLights();

        for (int i = 0; i < 5; i++)//无尽模式失败超过5次就结束
        {
            lights.Add(Instantiate(lightPrefab, lightPrefabParent));
        }
    }

    //清除所有light
    private void ClearLights()
    {
        foreach (Transform child in lightPrefabParent)
            Destroy(child.gameObject);
        lights.Clear();
    }

    //设置light的颜色
    public void SetLightColor(int index, bool win)
    {
        if (index >= 0 && index < lights.Count)
        {
            if(win)
            {
                lights[index].transform.GetChild(0).GetComponent<Image>().color= greenColor_1; 
                lights[index].transform.GetChild(1).GetComponent<Image>().color= greenColor_2; 
            }
            else
            {
                lights[index].transform.GetChild(0).GetComponent<Image>().color = redColor_1;
                lights[index].transform.GetChild(1).GetComponent<Image>().color = redColor_2;
            }
        }

    }

}