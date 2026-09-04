using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Shuffle Spawn Animation", menuName = "Content/Level/Shuffle Spawn Animation")]
    public class ShuffleSpawnAnimation : LevelSpawnAnimation
    {
        [SerializeField] float scaleTime = 0.5f;
        [SerializeField] Ease.Type scaleEasing = Ease.Type.BackOut;

        [Space]
        [SerializeField] float minDelay = 0.05f;
        [SerializeField] float maxDelay = 0.4f;

        private OptimisedTilesScaleTweenCase optimisedShuffleTweenCase;

        protected override IEnumerator SpawnLevelCoroutine(LevelRepresentation levelRepresentation, SimpleCallback onAnimationCompleted)
        {
            // Reset objects
            List<TileBehavior> tiles = levelRepresentation.GetSortedTiles();

            optimisedShuffleTweenCase = new OptimisedTilesScaleTweenCase(tiles, scaleTime, minDelay, maxDelay);
            optimisedShuffleTweenCase.SetEasing(scaleEasing);
            optimisedShuffleTweenCase.OnComplete(() => onAnimationCompleted?.Invoke());
            optimisedShuffleTweenCase.StartTween();

            foreach(TileBehavior tile in tiles)
            {
                tile.SetState(levelRepresentation.IsTileUnconcealed(tile.ElementPosition), true);

                yield return null;
            }
        }

        public override void Clear()
        {
            base.Clear();

            optimisedShuffleTweenCase.KillActive();
        }
    }
}