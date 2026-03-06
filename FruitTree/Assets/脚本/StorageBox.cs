using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : MonoBehaviour
{
    const int maxCount = 10;
    public FruitType fruitType = null;
    public int currentCount = 0;

    /// <summary>
    /// 初始化箱子
    /// </summary>
    public void InitBox()
    {
        fruitType = null;
        currentCount = 0;
    }

    /// <summary>
    /// 尝试增加一个水果
    /// </summary>
    /// <returns>是否能成功增加</returns>
    public bool TryAddFruit(FruitType type)
    {
        //箱子有东西，并且两者水果品种不同
        if(fruitType!=null && fruitType.name!= type.name)
        {
            return false;
        }
        if (currentCount >= maxCount)
        {
            return false;
        }

        currentCount++;
        return true;
    }

    /// <summary>
    /// 尝试从箱子中拿取一个水果
    /// </summary>
    /// <param name="type">拿到的水果</param>
    /// <returns>是否拿取成功</returns>
    public bool TryGetFruit(out FruitType type)
    {
        if (fruitType == null || currentCount <= 0)
        {
            type = null;
            return false;
        }

        currentCount--;
        type = fruitType;
        return true;
    }
}
