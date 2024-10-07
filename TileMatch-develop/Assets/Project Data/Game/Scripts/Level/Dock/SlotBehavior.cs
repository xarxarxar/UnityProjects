using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace Watermelon
{
    public class SlotBehavior : MonoBehaviour
    {
        [SerializeField] Vector3 offset;

        [Space]
        [SerializeField] Transform visuals;
        [SerializeField] SpriteRenderer backImage;
        [SerializeField] SpriteRenderer outlineImage;

        private Transform SlotablePos { get; set; }

        public Vector3 Scale { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Vector3 Position => transform.position;

        public SlotCase SlotCase { get; private set; }

        public bool IsOccupied => SlotCase != null;

        private TweenCase colorCase;

        private int spriteSortOrder;

        private bool isTemp = false;

        Coroutine stickCoroutine;

        private static GameData gameData;

        private void Awake()
        {
            SlotablePos = new GameObject("Slotable Position").transform;
            SlotablePos.SetParent(transform);
            SlotablePos.position = transform.position + offset;
        }

        public static SlotBehavior GetTempSlot(SlotBehavior lastSlot, SlotBehavior prevLastSlot)
        {
            var tempSlot = new GameObject("Temp Slot").AddComponent<SlotBehavior>();

            tempSlot.SlotablePos = new GameObject("Slotable Position").transform;
            tempSlot.SlotablePos.SetParent(tempSlot.transform);
            tempSlot.SlotablePos.position = lastSlot.SlotablePos.position + (lastSlot.SlotablePos.position - prevLastSlot.SlotablePos.position);
            tempSlot.Scale = Vector3.one;

            tempSlot.spriteSortOrder = lastSlot.spriteSortOrder + 5;

            tempSlot.isTemp = true;

            return tempSlot;
        }

        public void Init(int spriteSortOrder, Vector3 position, Vector3 scale)
        {
            transform.position = position;
            var smallerSize = scale.x > scale.y ? scale.y : scale.x;
            visuals.localScale = Vector3.one * smallerSize;

            Scale = transform.localScale;
            Rotation = transform.eulerAngles;

            this.spriteSortOrder = spriteSortOrder * 5 + 10;
        }

        public void Assign(SlotCase slotCase, bool instant = false)
        {
            SlotCase = slotCase;

            if (isTemp)
            {
                SlotCase.SubmitMove(SlotablePos.position, Scale, Rotation, spriteSortOrder, instant);
            } else
            {
                SlotCase.SubmitMove(SlotablePos, Scale, Rotation, spriteSortOrder, instant);
            }
        }

        public void AssingFast(SlotCase slotCase)
        {
            SlotCase = slotCase;

            slotCase.Behavior.SetSortingOrder(spriteSortOrder);

            SlotCase.ShiftMove(SlotablePos);
        }

        public void AssingWithoutMove(SlotCase slotCase)
        {
            SlotCase = slotCase;

            slotCase.Behavior.SetSortingOrder(spriteSortOrder);
        }

        public void Move()
        {
            if (SlotCase != null)
            {
                SlotCase.ShiftMove(SlotablePos);

                SlotCase.Behavior.SetSortingOrder(spriteSortOrder);
            }
        }

        public void ChangePosition(Vector3 newPosition)
        {
            if (SlotCase != null && !SlotCase.IsMoving)
            {
                stickCoroutine = StartCoroutine(StickToSlotCoroutine());
            }
            transform.DOMove(newPosition, 0.1f).OnComplete(() => {
                if (SlotCase != null && !SlotCase.IsMoving)
                {
                    StopCoroutine(stickCoroutine);
                    SlotCase.Behavior.Transform.position = SlotablePos.position;
                }
            });
         
        }

        
        private IEnumerator StickToSlotCoroutine()
        {
            while (true)
            {
                yield return null;
                SlotCase.Behavior.Transform.position = SlotablePos.position;
            }
        }

        public SlotCase RemoveSlot()
        {
            var slotCase = SlotCase;
            SlotCase = null;
            return slotCase;
        }

        public void ChangeColor(Color color)
        {
            if (outlineImage == null || backImage == null) return;

            colorCase.KillActive();

            outlineImage.color = color;
            outlineImage.transform.localScale = Vector3.zero;
            outlineImage.gameObject.SetActive(true);

            colorCase = outlineImage.DOScale(1, 0.15f).OnComplete(() => {
                backImage.color = color;
                outlineImage.gameObject.SetActive(false);
            });
        }

        public void RestoreColor(Color color)
        {
            if (outlineImage == null || backImage == null) return;

            colorCase.KillActive();

            outlineImage.color = backImage.color;
            outlineImage.transform.localScale = Vector3.one;
            outlineImage.gameObject.SetActive(true);

            backImage.color = color;

            colorCase = outlineImage.DOScale(0, 0.15f).OnComplete(() => {
                outlineImage.gameObject.SetActive(false);
            });
        }

        public void ClearColor()
        {
            colorCase.KillActive();
            if (backImage != null) backImage.color = Color.white;

            if (outlineImage != null) outlineImage.gameObject.SetActive(false);
        }

        public void Clear()
        {
            ClearColor();
            if (SlotCase != null) SlotCase.Clear();
            SlotCase = null;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (gameData == null)
            {
                var guid = AssetDatabase.FindAssets("t:GameData")[0];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
            }

            if (gameData == null || Application.isPlaying) return;

            var size = new Vector3(gameData.TileSize.x, gameData.TileSize.y, 0.01f);
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, size);
#endif
        }
    }
}