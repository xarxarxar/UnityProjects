using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] public List<Slot> allSlots = new List<Slot>();
    public int SlotCount => allSlots.Count;

    public void Init()
    {

    }

    public void ResetSlotManager()
    {
        for(int i = 0;i< allSlots.Count; i++)
        {
            allSlots[i].ResetSlot();
        }
    }

    public Slot GetSlot(int index)
    {
        if (index < 0 || index >= allSlots.Count)
            return null;

        return allSlots[index];
    }

    /// <summary>
    /// 获取最靠前（index 最小）的未被占用 Slot
    /// </summary>
    public bool GetFirstEmptySlot(out Slot slot)
    {
        slot= null;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (!allSlots[i].IsOccupied)
            {
                slot = allSlots[i];
                return true;
            }
                
        }
        return false;
    }

    /// <summary>
    /// 随机获取 1 个空的 Slot
    /// </summary>
    public bool TryGetRandomEmpty(out Slot emptySlot)
    {
        emptySlot = null;

        int count = 0;

        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i].IsOccupied)
                continue;

            count++;

            // 以 1/count 的概率选中当前 Slot
            if (Random.Range(0, count) == 0)
            {
                emptySlot = allSlots[i];
            }
        }

        return emptySlot != null;
    }
    
}
