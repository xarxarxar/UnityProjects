using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召唤一堵墙
/// </summary>
public class SummonAWallInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 0.5f, 0.75f, 1.0f };

    public SummonAWallInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        Debug.Log("开始高起墙祝福");
        SummonManager.Instance.NormalCrystal.OnWallDestroyed -= OnWallDestroyed;
        SummonManager.Instance.NormalCrystal.OnWallDestroyed += OnWallDestroyed;
        SummonManager.Instance.NormalCrystal.ResetWall();//先Reset一下
        SummonManager.Instance.NormalCrystal.Init(Mathf.RoundToInt(multipliers[rarity]*200));
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, 0);
    }

    public override void End()
    {
        Debug.Log("回收高起墙");
        SummonManager.Instance.NormalCrystal.End();
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);//收回bless的小圆图标
    }

    private void OnWallDestroyed()
    {
        BlessManager.Instance.RemoveBlessInstance(this);//收回bless的小圆图标
    }
}
