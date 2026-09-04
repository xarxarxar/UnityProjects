using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class SlotCase
    {
        public ISlotable Behavior { get; private set; }

        public TrailRenderer TrailRenderer;

        public bool IsBeingRemoved { get; set; }

        public bool IsMoving { get; set; }
        public DockMoveType MoveType { get; set; }

        private TweenCaseCollection elementMoveCases;

        public void AddTrail(GameObject trailObject)
        {
            trailObject.transform.SetParent(Behavior.VisualsTransform);
            trailObject.transform.ResetLocal();
            trailObject.transform.localPosition = new Vector3(0, 0, 0.4f);

            TrailRenderer trailRenderer = trailObject.GetComponent<TrailRenderer>();
            trailRenderer.Clear();

            TrailRenderer = trailRenderer;
        }

        public SlotCase(ISlotable behavior)
        {
            Behavior = behavior;
        }

        public void SubmitMove(Transform position, Vector3 scale, Vector3 rotation, int spriteSortOrder, bool instant = false)
        {
            IsMoving = true;
            MoveType = DockMoveType.Submit;

            var distance = Vector3.Distance(position.position, Behavior.Transform.position);
            var duration = Mathf.Clamp(distance / 5f, 0.5f, 5f);

            if (elementMoveCases != null && !elementMoveCases.IsComplete()) elementMoveCases.Kill();
            elementMoveCases = Tween.BeginTweenCaseCollection();

            if (!instant)
            {
                Behavior.SubmitMove(position, Vector3.one * LevelScaler.SlotSize, () => { OnMoveEnded(true, spriteSortOrder); });
                TrailRenderer.sortingOrder = spriteSortOrder - 1;
            }
            else
            {
                Behavior.Transform.position = position.position;

                Tween.NextFrame(() => OnMoveEnded(true, spriteSortOrder));
            }

            Tween.EndTweenCaseCollection();
        }

        public void SubmitMove(Vector3 position, Vector3 scale, Vector3 rotation, int spriteSortOrder, bool instant = false)
        {
            IsMoving = true;
            MoveType = DockMoveType.Submit;

            var distance = Vector3.Distance(position, Behavior.Transform.position);
            var duration = Mathf.Clamp(distance / 5f, 0.5f, 5f);

            if (elementMoveCases != null && !elementMoveCases.IsComplete()) elementMoveCases.Kill();
            elementMoveCases = Tween.BeginTweenCaseCollection();

            if (!instant)
            {
                Behavior.SubmitMove(position, Vector3.one * LevelScaler.SlotSize, () => { OnMoveEnded(true, spriteSortOrder); });
                TrailRenderer.sortingOrder = spriteSortOrder - 1;
            }
            else
            {
                Behavior.Transform.position = position;

                Tween.NextFrame(() => OnMoveEnded(true, spriteSortOrder));
            }

            Tween.EndTweenCaseCollection();
        }

        private void OnMoveEnded(bool rotate, int spriteSortOrder = -1)
        {
            TrailRenderer.gameObject.SetActive(false);
            TrailRenderer.transform.SetParent(null);

            IsMoving = false;

            DockBehavior.OnMovementEnded(this, MoveType);

            if(spriteSortOrder >= 0) Behavior.SetSortingOrder(spriteSortOrder);
        }

        public void ShiftMove(Transform position)
        {
            IsMoving = true;
            MoveType = DockMoveType.Shift;

            if (elementMoveCases != null && !elementMoveCases.IsComplete()) elementMoveCases.Kill();

            elementMoveCases = Tween.BeginTweenCaseCollection();

            Behavior.DockShuffleMove(position, () => { OnMoveEnded(false); });

            Tween.EndTweenCaseCollection();
        }

        public void Clear(bool disableBlock = true)
        {
            if (elementMoveCases != null && !elementMoveCases.IsComplete()) elementMoveCases.Kill();

            if (disableBlock)
            {
                Behavior.Clear();
            }
        }
    }
}