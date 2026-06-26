namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using NaughtyAttributes;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public class AIManager : InstanceManager<AIManager>
    {
        public List<AIRowControl> allRow;
        public List<GameObject> allRowObj;
    
        void Awake()
        {
            SetAllRowList();
        }
    
        void SetAllRowList()
        {
            //List<GameObject> allRowObj = GameObject.FindGameObjectsWithTag(TagHolder.AIRowController_Tag).ToList();
    
            int levelCount = GameManager.Instance.data.levelCount;
    
            int activeRowCount = levelCount < 5 ? 1 : levelCount < 15 ? 2 : 3;
            //int activeRowCount = 3;
    
    
            for (int i = 0; i < allRowObj.Count; i++)
            {
                if (activeRowCount - 1 >= i)
                {
                    allRow.Add(allRowObj[i].GetComponent<AIRowControl>());
                    allRowObj[i].GetComponent<AIRowControl>().lockArea.SetActive(false);
                }
                else
                {
                    allRowObj[i].GetComponent<AIRowControl>().lockArea.SetActive(true);
                }
            }
    
        }
        public void UpdateRow()
        {
            for (int i = 0; i < allRow.Count; i++)
            {
                allRow[i].UpdateRow();
            }
        }
    
        public AIRowSlot AvailableSlot()
        {
            int totalSlot = 0;
            for (int i = 0; i < allRow.Count; i++)
            {
                for (int j = 0; j < allRow[i].AIRowSlots.Count; j++)
                {
                    totalSlot++;
                }
            }
    
            AIRowSlot slot = null;
            int slotIndex = 0;
            for (int i = 0; i < totalSlot; i++)
            {
                if (i % allRow.Count == 0 && i != 0)
                {
                    slotIndex++;
                }
    
                if (allRow[i % allRow.Count].AIRowSlots[slotIndex].AIType == BaggageType.None)
                {
                    slot = allRow[i % allRow.Count].AIRowSlots[slotIndex];
                    break;
                }
            }
            return slot;
        }
    
        public AIStateManager AvailableAI(BaggageType baggageType)
        {
            for (int i = 0; i < allRow.Count; i++)
            {
                if (allRow[i].AIRowSlots[0].AIType == baggageType)
                {
                    return allRow[i].AIRowSlots[0].CurrentAI;
                }
            }
            return null;
        }
    
        public bool CanMerge(BaggageType baggageType)
        {
            for (int i = 0; i < allRow.Count; i++)
            {
                if (allRow[i].AIRowSlots[0].AIType == baggageType)
                {
                    return true;
                }
            }
            return false;
        }
    
        public List<BaggageType> FrontAITypes()
        {
            List<BaggageType> frontAITypes = new List<BaggageType>();
    
            for (int i = 0; i < allRow.Count; i++)
            {
                frontAITypes.Add(allRow[i].AIRowSlots[0].AIType);
            }
    
            return frontAITypes;
        }
    
        public void SpawnAI()
        {
            if (AvailableSlot() == null || LevelManager.Instance.stickmanList.Count == 0)
                return;
    
            AIFactory.SpawnAI(LevelManager.Instance.stickmanList[0], transform.position, Quaternion.identity, out GameObject factoryProduct);
    
            LevelManager.Instance.stickmanList.RemoveAt(0);
        }
    
    
        public bool IsAllSlotNull()
        {
            int totalSlot = 0;
            for (int i = 0; i < allRow.Count; i++)
            {
                for (int j = 0; j < allRow[i].AIRowSlots.Count; j++)
                {
                    totalSlot++;
                }
            }
    
            int slotIndex = 0;
            for (int i = 0; i < totalSlot; i++)
            {
                if (i % allRow.Count == 0 && i != 0)
                {
                    slotIndex++;
                }
    
                if (allRow[i % allRow.Count].AIRowSlots[slotIndex].AIType != BaggageType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
