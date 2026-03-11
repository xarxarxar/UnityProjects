using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class FruitTree : FruitSource
{
    public FruitType fruitType = null;
    const int maxLevel = 5;//暂定所有最大等级都为5

    public float growTime;//单颗果实的生长时间
    public int maxFruitCount;//最大果实数量
    public int fruitSoldPrice;//单颗果实的售价
    public Text showText;

    public int level = 1;//当前等级
    private int currentFruitCount = 0;//当前果实数量
    private float timer = 0;//计时器
    private List<Bee> reserveBees = new List<Bee>();
    public override FruitType FruitType { get=> fruitType; }//它的水果类型

    public override int RipedCount { get=> currentFruitCount; }//成熟的果实数量

    //预定的蜜蜂，如果数量大于等于RipedCount，那么就表示这里无法获取成熟的果实，按理说数量不应该会大于RipedCount
    public override List<Bee> ReserveBees { get=> reserveBees; }

    /// <summary>
    /// 初始化果树
    /// </summary>
    /// <param name="_fruitType"></param>
    public void InitTree(FruitType _fruitType)
    {
        fruitType = _fruitType;

        growTime = fruitType.defaultGrowTime;
        maxFruitCount = fruitType.defaultMaxFruits;
        fruitSoldPrice = fruitType.defaultSoldPrice;

        level = 1;
        currentFruitCount = 0;
        timer = 0;
    }


    /// <summary>
    /// 只管升级，不管金币是否足够
    /// </summary>
    /// <returns>返回是否升级成功，如果没成功，外部就不用扣钱了</returns>
    public bool UpgradeLevel()
    {
        if (level < maxLevel)
        {
            level++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除这棵树
    /// </summary>
    public void RemoveTree()
    {
        fruitType = null;
        level = 1;
        currentFruitCount = 0;
        timer = 0;
    }

    /// <summary>
    /// 移除预定的小蜜蜂
    /// </summary>
    /// <param name="bee"></param>
    public override void RemoveBee(Bee bee)
    {

    }
    /// <summary>
    /// 尝试预定水果
    /// </summary>
    /// <param name="bee"></param>
    /// <returns></returns>
    public override bool TryReserveFruit(Bee bee)
    {
        return false;
    }
    /// <summary>
    /// 拿走预定的水果
    /// </summary>
    /// <param name="fruit"></param>
    public override void TakeReservedFruit(FruitType fruit)
    {

    }

    private void Update()
    {
        if (!FruitGameManager.isGaming) return;
        //计时器更新，果实生长
        if(currentFruitCount< maxFruitCount)
        {
            timer += Time.deltaTime;
            if (timer >= growTime)
            {
                timer = 0;
                currentFruitCount++;
                TriggerRipedCountChanged(this);
            }
        }
        showText.text = $"{fruitType.name}\n{currentFruitCount}/{maxFruitCount}";
    }

    private void OnMouseUp()
    {
        PickFruit();//采摘果实
    }
    //采摘这棵树的果实
    private void PickFruit()
    {
        if (currentFruitCount > 0)
        {
            // 询问管理器：是否有货车需要此果子
            if (FruitGameManager.Instance.TryDeliverFruit(fruitType))
            {
                currentFruitCount--;
                // 摘果成功逻辑（计时器不需要重置，会继续长下一颗）
            }
        }
    }

}
