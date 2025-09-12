using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 前n回合必定暴击
/// </summary>
public class CriticalBless : Bless
{
    /// <summary>
    /// 前n回合必定暴击
    /// </summary>
    /// <param name="rarity"></param>
    public CriticalBless(int rarity) : base(rarity)
    {

    }

    public override void Apply()
    {
        throw new System.NotImplementedException();
    }

}
