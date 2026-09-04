using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class LayerRow
    {
        [SerializeField] CellData[] cells;

        public int AmountOfCells => cells.Length;

        public bool this[int i]
        {
            get => cells[i].IsFilled;
        }

        public CellData GetCell(int i)
        {
            if (i < AmountOfCells && i >= 0) return cells[i];

            return null;
        }

        public int GetAmountOfFilledCells()
        {
            int counter = 0;

            for (int i = 0; i < AmountOfCells; i++)
            {
                if (cells[i].IsFilled) counter++;
            }

            return counter;
        }
    }
}
