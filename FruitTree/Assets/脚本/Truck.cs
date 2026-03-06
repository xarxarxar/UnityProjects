using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Truck : MonoBehaviour
{
    public int needAmount;
    public int currentAmount;
    public float remainTime;
    public int rewardGold;
    public Text truckInfoText;

    public List<Mission> missions = new List<Mission>();//货车所携带的任务

    private bool isDone = false;

    public void Init(float time, int gold,List<Mission> _missions)
    {
        currentAmount = 0;
        remainTime = time;
        rewardGold = gold;

        missions.Clear();          // 清空当前列表
        missions.AddRange(_missions); // 复制元素
    }

    void Update()
    {
        if (!FruitGameManager.isGaming) return;
        if (isDone) return;

        remainTime -= Time.deltaTime;
        //truckInfoText.text = $"{targetFruitName}\n{currentAmount}/{needAmount}\n{remainTime:F0}s";

        if (remainTime <= 0)
        {
            Finish(false);
        }
    }

    public bool ReceiveFruit(FruitType fruitType)
    {
        //if (isDone || fruitName != targetFruitName) return false;如果任务列表中没有需要这种果实的，则返回false

        currentAmount++;
        if (currentAmount >= needAmount)
        {
            Finish(true);
        }
        return true;
    }

    void Finish(bool success)
    {
        isDone = true;
        if (success) FruitGameManager.Instance.AddGold(rewardGold);
        FruitGameManager.Instance.RemoveTruck(this);
        Destroy(gameObject);
    }
}