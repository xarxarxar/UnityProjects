using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人的卡槽管理其
/// </summary>
public class NpcSlotManager : MonoBehaviour
{
    public List<Slot> NpcSlos = new List<Slot>();
    public int SlotCount = 0;
    public Transform slotParent;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="count"></param>
    public void Init(int count)
    {
        SlotCount=count;
        NpcSlos.Clear();
        for (int i = 0; i < SlotCount; i++)
        {
            Slot slot = PoolManager.Instance.SlotPool.Get();
            slot.transform.parent = slotParent;
            slot.transform.localPosition = new Vector3(i,0,0);
            slot.Init(0, i);
            NpcSlos.Add(slot);
        }
    }

    /// <summary>
    /// 获取Slot，通过一个索引
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Slot GetSlot(int index)
    {
        if (index < 0)
            return null;
        if(index >= NpcSlos.Count) return null;
        Slot slot = NpcSlos[index];

        return slot;
    }

    public Slot GetFirstEmptySlot()
    {
        for (int i = 0;i < NpcSlos.Count; i++)
        {
            if (!NpcSlos[i].IsOccupied)
            {
                return NpcSlos[i];
            }
        }
        return null;
    }

    public void ResetSlotManager()
    {
        for (int i = 0; i < NpcSlos.Count; i++)
        {
            NpcSlos[i].ResetSlot();
        }
    }
}
