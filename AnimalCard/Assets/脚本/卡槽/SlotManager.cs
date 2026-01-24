using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] public List<List<Slot> > allSlots =new List<List<Slot>>();
    public int SlotCount => Width* Height;
    public int Width = 8;
    public int Height = 10;

    public void Init(int width,int height,Transform slotParent)
    {
        Width=width;
        Height = height;
        allSlots.Clear();
        
        for(int i=0;i< Height; i++)
        {
            List<Slot> slots = new List<Slot>();
            for (int j = 0; j < Width; j++)
            {
                Slot slot = PoolManager.Instance.SlotPool.Get();
                slot.transform.parent = slotParent;
                slot.transform.localPosition = new Vector3(j,i,-1);
                slot.Init(i,j);
                slot.name = $"{i}_{j}";
                slots.Add(slot);
            }
            allSlots.Add(slots);
        }
        
    }

    public void ResetSlotManager()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            for (int j = 0; j < allSlots[i].Count; j++)
            {
                allSlots[i][j].ResetSlot();
            }
        }
    }
    /// <summary>
    /// 获取 SingleCard 当前 Slot 所在的整行 Slot
    /// </summary>
    public List<Slot> GetRowSlots(SingleCard card)
    {
        if (card == null || card.CurrentSlot == null)
            return null;

        int row = card.CurrentSlot.Row;

        if (row < 0 || row >= allSlots.Count)
            return null;

        // 直接返回这一行（如需防止外部修改，可 new 一份）
        return allSlots[row];
    }

    /// <summary>
    /// 获取 SingleCard 当前 Slot 所在的整列 Slot
    /// </summary>
    public List<Slot> GetColumnSlots(SingleCard card)
    {
        if (card == null || card.CurrentSlot == null)
            return null;

        int col = card.CurrentSlot.Col;

        List<Slot> columnSlots = new List<Slot>();

        for (int r = 0; r < allSlots.Count; r++)
        {
            if (col < 0 || col >= allSlots[r].Count)
                continue;

            columnSlots.Add(allSlots[r][col]);
        }

        return columnSlots;
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
        int cur = 0;

        for (int i = 0; i < allSlots.Count; i++)
        {
            var row = allSlots[i];
            if (index < cur + row.Count)
            {
                return row[index - cur];
            }
            cur += row.Count;
        }

        return null;
    }
    /// <summary>
    /// 获取Slot，第几行第几列
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public Slot GetSlot(int row, int col)
    {
        if (row < 0 || row >= allSlots.Count)
            return null;

        if (col < 0 || col >= allSlots[row].Count)
            return null;

        return allSlots[row][col];
    }
    /// <summary>
    /// 获取某个Slot是否在这个里面，在第几行第几列
    /// </summary>
    /// <param name="target"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public bool TryGetSlotIndex(Slot target, out int row, out int col)
    {
        row = -1;
        col = -1;

        for (int r = 0; r < allSlots.Count; r++)
        {
            var rowSlots = allSlots[r];
            for (int c = 0; c < rowSlots.Count; c++)
            {
                if (rowSlots[c] == target)
                {
                    row = r;
                    col = c;
                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// 获取最靠前（row → col 顺序）的未被占用 Slot
    /// </summary>
    public bool GetFirstEmptySlot(out Slot slot)
    {
        slot = null;

        for (int row = 0; row < allSlots.Count; row++)
        {
            var rowSlots = allSlots[row];
            for (int col = 0; col < rowSlots.Count; col++)
            {
                if (!rowSlots[col].IsOccupied)
                {
                    slot = rowSlots[col];
                    return true;
                }
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

        for (int row = 0; row < allSlots.Count; row++)
        {
            var rowSlots = allSlots[row];
            for (int col = 0; col < rowSlots.Count; col++)
            {
                Slot slot = rowSlots[col];
                if (slot.IsOccupied)
                    continue;

                count++;

                // 以 1/count 的概率选中当前 Slot
                if (Random.Range(0, count) == 0)
                {
                    emptySlot = slot;
                }
            }
        }

        return emptySlot != null;
    }

    /// <summary>
    /// 获取所有未被占用的 Slot（row → col 顺序）
    /// </summary>
    public List<Slot> GetAllEmptySlots()
    {
        List<Slot> result = new List<Slot>();

        for (int row = 0; row < allSlots.Count; row++)
        {
            var rowSlots = allSlots[row];
            for (int col = 0; col < rowSlots.Count; col++)
            {
                Slot slot = rowSlots[col];
                if (!slot.IsOccupied)
                {
                    result.Add(slot);
                }
            }
        }

        return result;
    }

}
