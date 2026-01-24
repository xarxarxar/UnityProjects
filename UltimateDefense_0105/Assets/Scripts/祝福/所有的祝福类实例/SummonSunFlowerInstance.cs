using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召唤n个太阳花，
/// </summary>
public class SummonSunFlowerInstance : BlessInstance
{
    private Coroutine coroutine;

    public SummonSunFlowerInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {   
        if(rarity == 0)
        {
            Vector3 pos = new Vector3(0,-3,0);
            SunFlower sunFlower = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos);
        }
        else if(rarity == 1)
        {
            Vector3 pos1 = new Vector3(-4, -3, 0);
            SunFlower sunFlower1 = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower1.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos1);

            Vector3 pos2 = new Vector3(4, -3, 0);
            SunFlower sunFlower2 = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower2.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos2);
        }
        else
        {
            Vector3 pos1 = new Vector3(-5, -3, 0);
            SunFlower sunFlower1 = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower1.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos1);

            Vector3 pos2 = new Vector3(0, -3, 0);
            SunFlower sunFlower2 = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower2.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos2);

            Vector3 pos3 = new Vector3(5, -3, 0);
            SunFlower sunFlower3 = GameObject.Instantiate(SummonManager.Instance.SunFlower, SummonManager.Instance.SummonParent);
            sunFlower3.Init(200, TowerManager.Instance.CurrentTower.BaseDamage, pos3);
        }
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        Debug.Log("开始太阳花祝福");
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, 10);
        yield return TimerUtility.WaitForGameSeconds(10);//太阳花存活时间只有10秒
        blessBuffShow.Close();
        End();
    }


    public override void End()
    {
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }
}
