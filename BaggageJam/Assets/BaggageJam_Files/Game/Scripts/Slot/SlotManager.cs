namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class SlotManager : InstanceManager<SlotManager>
    {
        [SerializeField] private List<Slot> allSlot;
        public List<Baggage> allBaggage;
    
        [Button]
        public void MergeControl()
        {
            Dictionary<BaggageType, int> baggageKey = new Dictionary<BaggageType, int>();
    
            foreach (Baggage baggage in allBaggage)
            {
                if (!baggageKey.ContainsKey(baggage.baggageType))
                {
                    baggageKey.Add(baggage.baggageType, 0);
                }
    
                baggageKey[baggage.baggageType]++;
            }
    
            BaggageType totalBaggageType = BaggageType.None;
            int totalBaggage = 0;
    
            List<BaggageType> mergeBaggageTypeList = new List<BaggageType>();
    
            foreach (KeyValuePair<BaggageType, int> BaggageCount in baggageKey)
            {
                if (BaggageCount.Value >= 3)
                {
                    mergeBaggageTypeList.Add(BaggageCount.Key);
    
                    totalBaggage = BaggageCount.Value;
                    totalBaggageType = BaggageCount.Key;
                }
            }

            // Debug.Log("Colors in the list that are 3 or greater: " + string.Join(", ", mergeBaggageTypeList));  
            // Debug.Log("The most frequent color in the list: " + totalBaggageType + ", count: " + totalBaggage);

            while (!AIManager.Instance.FrontAITypes().Contains(totalBaggageType) && mergeBaggageTypeList.Count > 1)
            {
                if (mergeBaggageTypeList.Contains(totalBaggageType))
                {
                    mergeBaggageTypeList.Remove(totalBaggageType);
    
                    totalBaggageType = mergeBaggageTypeList[0];
                }
            }
    
            if (totalBaggage >= 3)
            {
                List<Baggage> mergeObjs = new List<Baggage>();
                foreach (var mergeBaggage in allBaggage)
                {
                    if (mergeBaggage.baggageType == totalBaggageType && mergeObjs.Count < 3)
                    {
                        mergeObjs.Add(mergeBaggage);
                    }
                }
    
                if (AIManager.Instance.CanMerge(totalBaggageType))
                {
                    Merge(mergeObjs);
                }
                else if (!CanAddBaggage())
                {
                    EventManager.Broadcast(GameEvent.OnLose);
                }
            }
            else if (!CanAddBaggage())
            {
                EventManager.Broadcast(GameEvent.OnLose);
            }
    
        }
    
        public bool CanAddBaggage()
        {
            for (int i = 0; i < allSlot.Count; i++)
            {
                if (!allSlot[i].isLock && allSlot[i].baggageType == BaggageType.None)
                {
                    return true;
                }
            }
            return false;
        }
    
        public void Merge(List<Baggage> mergeObj)
        {
            Vector3 Pos = mergeObj[1].transform.position;
    
            foreach (var item in mergeObj)
            {
                allBaggage.Remove(item);
            }
    
            for (int i = 0; i < 3; i++)
            {
                Baggage mergeBaggage = mergeObj[i];
    
    
                mergeBaggage.Merge(Pos, i % 2 == 0);
            }
    
            Invoke(nameof(SlotSort), .3f);
            EventManager.Broadcast(GameEvent.OnPlaySound, "Merge");
        }

        void SlotSort()
        {
            allBaggage = allBaggage.Where(item => item != null).ToList();
    
            int startIndex = 0;
            for (int i = 0; i < allSlot.Count; i++)
            {
                if (!allSlot[i].isLock)
                {
                    startIndex = i;
                    break;
                }
            }
    
            for (int i = 0; i < allBaggage.Count; i++)
            {
                allBaggage[i].SetSlot(allSlot[i + startIndex]);
            }
        }
    
        //##############################Events##############################
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnSetSlotBaggage, OnSetSlotBaggage);
        }
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnSetSlotBaggage, OnSetSlotBaggage);
        }
    
        void OnSetSlotBaggage(object baggage)
        {
            Baggage currentBaggage = (Baggage)baggage;
            allBaggage.Add((Baggage)baggage);
    
            Invoke(nameof(MergeControl), .2f);
    
            for (int i = 0; i < allSlot.Count; i++)
            {
                if (allSlot[i].baggageType == BaggageType.None && !allSlot[i].isLock)
                {
                    currentBaggage.SetSlot(allSlot[i]);
                    break;
                }
                else if (allSlot[i].baggageType == currentBaggage.baggageType && allSlot[i + 1].baggageType != currentBaggage.baggageType)
                {
                    for (int j = allSlot.Count - 2; j > i; j--)
                    {
                        if (allSlot[j].Baggage != null)
                            allSlot[j].Baggage.SetSlot(allSlot[j + 1]);
                    }
    
                    currentBaggage.SetSlot(allSlot[i + 1]);
                    break;
                }
            }
        }
    }
    
}
