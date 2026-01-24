using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private List<Slot> allSlots = new List<Slot>();

    public Slot GetSlot(int index)
    {
        if (index < 0 || index >= allSlots.Count)
            return null;

        return allSlots[index];
    }

    /// <summary>
    /// 获取最靠前（index 最小）的未被占用 Slot
    /// </summary>
    public Slot GetFirstEmptySlot()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (!allSlots[i].IsOccupied)
                return allSlots[i];
        }
        return null;
    }


    public int SlotCount => allSlots.Count;
}
