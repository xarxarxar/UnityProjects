using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏内通用目标选择工具。
/// </summary>
public static class VideoGameTargetUtility
{
    /// <summary>
    /// 从角色列表中随机选择一个不是自己的角色。
    /// </summary>
    /// <param name="roles">候选角色列表。</param>
    /// <param name="self">需要排除的自身角色。</param>
    /// <returns>随机到的其他角色；如果没有可选目标则返回 null。</returns>
    public static BaseRole GetRandomOther(IList<BaseRole> roles, BaseRole self)
    {
        if (roles == null || roles.Count <= 1)
        {
            return null;
        }

        int otherCount = 0;
        for (int i = 0; i < roles.Count; i++)
        {
            if (roles[i] != self)
            {
                otherCount++;
            }
        }

        if (otherCount == 0)
        {
            return null;
        }

        int index = Random.Range(0, otherCount);
        for (int i = 0; i < roles.Count; i++)
        {
            if (roles[i] == self)
            {
                continue;
            }

            if (index == 0)
            {
                return roles[i];
            }

            index--;
        }

        return null;
    }
}
