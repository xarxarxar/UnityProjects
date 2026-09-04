using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Watermelon
{
    [System.Serializable]
    public class DockBehavior : MonoBehaviour
    {
        private static DockBehavior instance;

        [SerializeField] GameObject trailPrefab;
        [SerializeField] AnimationCurve positionYCurve;
        [SerializeField] GameObject slotPrefab;

        private static List<SlotBehavior> slots;

        private Pool trailPool;
        private LevelController levelController;
        private Vector3 defaultContainerPosition;

        private ISlotable lastPickedObject;

        public bool IsFilled => slots[^1].IsOccupied;
        public bool IsEmpty => !slots[0].IsOccupied;

        private TweenCase delayTweenCase;
        private int addedDepth = 0;

        public static ISlotable LastPickedObject => instance.lastPickedObject;
        public static AnimationCurve PositionYCurve => instance.positionYCurve;

        public static event Action<ISlotable> ElementAdded;
        public static event Action<List<ISlotable>> MatchCombined;

        public void Initialise(LevelController levelController)
        {
            instance = this;

            this.levelController = levelController;

            defaultContainerPosition = transform.position;

            lastPickedObject = null;

            trailPool = new Pool(new PoolSettings(trailPrefab, 1, true));

            slots = new List<SlotBehavior>();
            transform.GetComponentsInChildren(slots);

            slots.Sort((slot1, slot2) => (int)((slot2.Position.x - slot1.Position.x) * 100));

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                var position = slot.transform.position.SetX(-LevelScaler.SlotSize.x * 7f / 2f + (i + 0.5f) * LevelScaler.SlotSize.x);
                var scale = Vector3.one * LevelScaler.SlotSize;

                slot.Init(i, position, scale);
            }
        }

        public void PlayAppearAnimation()
        {
            HideSlots();

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].DOScale(1f, 0.3f, Random.Range(0f, 0.2f)).SetEasing(Ease.Type.CubicOut);
            }
        }

        public void HideSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.localScale = Vector3.zero;
            }

            if(slots.Count > 7)
            {
                var additionalSlot = slots[^1];

                additionalSlot.Clear();
                Destroy(additionalSlot.gameObject);

                slots.RemoveAt(slots.Count - 1);

                for(int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];

                    var position = slot.transform.position.SetX(-LevelScaler.SlotSize.x * 7f / 2f + (i + 0.5f) * LevelScaler.SlotSize.x);
                    slot.transform.position = position;
                }
            }
        }

        public void DisposeQuickly()
        {
            delayTweenCase.KillActive();

            lastPickedObject = null;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                slot.Clear();
            }

            for (int i = 0; i < addedDepth * 3; i++)
            {
                var slot = slots[slots.Count - 1];
                slots.RemoveAt(slots.Count - 1);

                slot.Clear();
                Destroy(slot.gameObject);
            }

            addedDepth = 0;
        }

        public void DisableRevert()
        {
            lastPickedObject = null;
        }

        private void RemoveMatch(List<ISlotable> charactersToRemove)
        {
            lastPickedObject = null;

            var slotsToRemove = new List<SlotCase>();
            for (int i = 0; i < slots.Count; i++)
            {
                var slotCase = slots[i].SlotCase;

                if (slotCase != null && charactersToRemove.Contains(slotCase.Behavior))
                {
                    slotsToRemove.Add(slotCase);
                    slotCase.IsBeingRemoved = true;
                }
            }

            for(int i = 0; i < slotsToRemove.Count; i++)
            {
                var slotCase = slotsToRemove[i];
                slotCase.Behavior.MatchAnimation(i * 0.05f);
            }

            AudioController.PlaySound(AudioController.Sounds.mergeSound, 0.5f);

            Tween.DelayedCall(0.4f, () =>
            {
                for (int i = 0; i < slotsToRemove.Count; i++)
                {
                    var slotCase = slotsToRemove[i];
                    var element = slotCase.Behavior;

                    slotCase.IsBeingRemoved = false;

                    element.Clear();

                    for (int j = 0; j < slots.Count; j++)
                    {
                        var slot = slots[j];

                        if (slot.SlotCase == slotCase)
                        {
                            slot.RemoveSlot();
                            break;
                        }
                    }
                }

                ShiftAllLeft();

                if (addedDepth > 0)
                {
                    addedDepth--;
                    for (int i = 0; i < 3; i++)
                    {
                        var tempSlot = slots[^1];
                        slots.RemoveAt(slots.Count - 1);

                        tempSlot.Clear();
                        Destroy(tempSlot.gameObject);
                    }
                }
                levelController.OnMatchCompleted();
            });
        }

        private bool CheckMatch(bool remove = true)
        {
            if (IsEmpty) return false;

            return CheckDockMatch(remove);
        }

        public static List<ISlotable> GetHintSlots()
        {
            SlotBehavior[] elementsArray = slots.FindAll(x => x.IsOccupied).GroupBy(x => x.SlotCase.Behavior.UniqueElementID).OrderByDescending(g => g.Count()).SelectMany(g => g).ToArray();

            if (!elementsArray.IsNullOrEmpty())
            {
                List<ISlotable> tempSlotElements = new List<ISlotable>();
                tempSlotElements.Add(elementsArray[0].SlotCase.Behavior);

                for (int i = 1; i < elementsArray.Length; i++)
                {
                    if (elementsArray[i].SlotCase.Behavior.IsSameType(elementsArray[0].SlotCase.Behavior))
                    {
                        tempSlotElements.Add(elementsArray[i].SlotCase.Behavior);
                    }
                }

                return tempSlotElements;
            }

            return null;
        }

        private bool CheckDockMatch(bool remove = true)
        {
            int counter = 1;
            var comparableRefference = slots[0].SlotCase.Behavior;
            var list = new List<ISlotable> { slots[0].SlotCase.Behavior };

            for (int i = 1; i < slots.Count; i++)
            {
                var slot = slots[i];

                if (!slot.IsOccupied) return false;

                var slotCase = slot.SlotCase;
                var element = slotCase.Behavior;

                if (counter == 0)
                {
                    comparableRefference = element;
                }

                if (element.IsSameType(comparableRefference) && !slotCase.IsBeingRemoved && (!slot.SlotCase.IsMoving || !remove))
                {
                    counter++;
                    list.Add(element);

                    if (counter == 3)
                    {
                        if (remove)
                        {
                            RemoveMatch(list);

                            MatchCombined?.Invoke(list);
                        }

                        return true;
                    }
                }
                else if (!slotCase.IsBeingRemoved)
                {
                    counter = 1;
                    comparableRefference = element;
                    list = new List<ISlotable> { element };
                }
                else
                {
                    counter = 0;
                    list = new List<ISlotable>();
                }
            }

            return false;
        }

        private int CalculateIndexSlots(ISlotable tileBehavior)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (!slot.IsOccupied) return i;

                if (tileBehavior.IsSameType(slot.SlotCase.Behavior))
                {
                    for (int j = i + 1; j < slots.Count; j++)
                    {
                        var nextSlot = slots[j];

                        if (!nextSlot.IsOccupied) return j;

                        if (!slot.IsOccupied || !tileBehavior.IsSameType(nextSlot.SlotCase.Behavior))
                        {
                            return j;
                        }
                    }
                }
            }

            return -1;
        }

        public static int GetSlotsAvailable()
        {
            int counter = 0;
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                if (!slots[i].IsOccupied) counter++;
            }

            return counter;
        }

        public void SetOverlayPosition()
        {
            transform.position = defaultContainerPosition.SetZ(1.0f);
        }

        public void SetDefaultPosition()
        {
            transform.position = defaultContainerPosition;
        }

        public bool SubmitToSlot(ISlotable element, bool instant)
        {
            int index = CalculateIndexSlots(element);

            if (index == -1)
            {
                levelController.OnSlotsFilled();
                return true;
            }

            SlotCase slotCase = new SlotCase(element);
            slotCase.AddTrail(trailPool.GetPooledObject());

            slotCase.IsMoving = true;
            slotCase.MoveType = DockMoveType.Submit;

            if (slots[index].IsOccupied)
            {
                Insert(slotCase, index, instant);
            }
            else
            {
                slots[index].Assign(slotCase, instant);
            }

            lastPickedObject = element;

            ElementAdded?.Invoke(element);

            if (CheckMatch(false))
            {
                if (addedDepth < 2)
                {
                    addedDepth++;
                    for (int i = 0; i < 3; i++)
                    {
                        slots.Add(SlotBehavior.GetTempSlot(slots[^1], slots[^2]));
                    }
                }

            }
            else if (IsFilled)
            {
                levelController.OnSlotsFilled();
            }

            return true;
        }

        public static List<ISlotable> RemoveObjects(int count)
        {
            List<ISlotable> removedTiles = new List<ISlotable>();

            for (int i = slots.Count - 1; i >= 0; i--)
            {
                SlotBehavior slot = slots[i];
                if (slot.IsOccupied)
                {
                    ISlotable tileBehavior = slot.SlotCase.Behavior;

                    slot.SlotCase.Clear(false);
                    slot.RemoveSlot();

                    if (instance.lastPickedObject == tileBehavior)
                        instance.lastPickedObject = null;

                    removedTiles.Add(tileBehavior);

                    if (removedTiles.Count >= count)
                        break;
                }
            }

            instance.ShiftAllLeft();

            return removedTiles;
        }

        public static ISlotable RemoveLastPicked()
        {
            if (instance.lastPickedObject == null) return null;

            var objToReturn = instance.lastPickedObject;
            instance.lastPickedObject = null;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                if (slot.IsOccupied && slot.SlotCase.Behavior == objToReturn)
                {
                    slot.SlotCase.Clear(false);
                    slot.RemoveSlot();

                    instance.ShiftAllLeft();

                    break;
                }
            }

            return instance.lastPickedObject;
        }

        public void Insert(SlotCase slotCase, int index, bool instant = false)
        {
            var freeCase = slotCase;

            for (int i = index; i < slots.Count; i++)
            {
                var slot = slots[i];

                var caseToShift = slot.RemoveSlot();

                if (freeCase != null)
                {
                    if (freeCase.IsMoving && freeCase.MoveType == DockMoveType.Submit)
                    {
                        slot.Assign(freeCase, instant);
                    }
                    else
                    {
                        slot.AssingFast(freeCase);
                    }
                }

                freeCase = caseToShift;
            }
        }
        public void ShiftAllLeft()
        {
            var lastIndex = -1;

            for (int i = 0; i < slots.Count - 1; i++)
            {
                var recepient = slots[i];

                if (recepient.IsOccupied) continue;

                bool found = false;
                for (int j = i + 1; j < slots.Count; j++)
                {
                    var donor = slots[j];

                    if (!donor.IsOccupied) continue;

                    var slotCase = donor.RemoveSlot();
                    if (slotCase.IsMoving && slotCase.MoveType == DockMoveType.Submit)
                    {
                        recepient.Assign(slotCase);
                    }
                    else
                    {
                        recepient.AssingFast(slotCase);
                    }

                    found = true;

                    break;
                }

                lastIndex = i;
                if (!found) break;
            }

            if (lastIndex == -1) return;

            for (int i = lastIndex; i < slots.Count; i++)
            {
                var slot = slots[i];
                slot.RestoreColor(Color.white);
            }
        }

        public void LateUpdate()
        {
            if (Time.frameCount % 15 == 0 && !IsEmpty)
            {
                if (!CheckMatch() && IsFilled)
                {
                    levelController.OnSlotsFilled();
                }
            }
        }

        public static void OnMovementEnded(SlotCase slotCase, DockMoveType moveType)
        {
            instance.OnMoveEnded(slotCase, moveType);
        }

        public void OnMoveEnded(SlotCase slotCase, DockMoveType moveType)
        {
            CheckMatch();

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                if (slot.IsOccupied && (slot.SlotCase.IsMoving || slot.SlotCase.IsBeingRemoved)) return;
            }

            ShiftAllLeft();

            for (int i = 0; i < addedDepth * 3; i++)
            {
                var slot = slots[slots.Count - 1];
                slots.RemoveAt(slots.Count - 1);

                slot.Clear();
                Destroy(slot.gameObject);
            }

            addedDepth = 0;
        }

        public void AddExtraSlot()
        {
            LevelController.IsRaycastEnabled = false;

            if (addedDepth == 0)
            {
                SpawnExtraSlot();
            }
            else
            {
                StartCoroutine(WaitAndSpawnExtraSlotCoroutine());
            }
        }

        private IEnumerator WaitAndSpawnExtraSlotCoroutine()
        {
            while (addedDepth != 0) yield return null;

            SpawnExtraSlot();
        }

        private void SpawnExtraSlot()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                slot.ChangePosition(slot.transform.position.SetX(-LevelScaler.SlotSize.x * 8f / 2f + (i + 0.5f) * LevelScaler.SlotSize.x));
            }

            var newSlot = Instantiate(slotPrefab).GetComponent<SlotBehavior>();

            var position = slots[0].transform.position.SetX(-LevelScaler.SlotSize.x * 8f / 2f + (7 + 0.5f) * LevelScaler.SlotSize.x);
            var scale = Vector3.one * LevelScaler.SlotSize;

            newSlot.Init(7, position, scale);

            newSlot.transform.localScale = Vector3.zero;
            newSlot.transform.DOScale(1, 0.1f, 0.025f);

            slots.Add(newSlot);

            Tween.DelayedCall(0.1f, () => LevelController.IsRaycastEnabled = true);
        }

        public int CountTiles(TileBehavior tile)
        {
            var counter = 0;
            for(int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                if (slot.IsOccupied)
                {
                    if (tile.IsSameType(slot.SlotCase.Behavior))
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }
    }
}
