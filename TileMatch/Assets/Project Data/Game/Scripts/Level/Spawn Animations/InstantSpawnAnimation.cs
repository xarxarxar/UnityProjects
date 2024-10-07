using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Instant Spawn Animation", menuName = "Content/Level/Instant Spawn Animation")]
    public class InstantSpawnAnimation : LevelSpawnAnimation
    {
        protected override IEnumerator SpawnLevelCoroutine(LevelRepresentation levelRepresentation, SimpleCallback onAnimationCompleted)
        {
            List<TileBehavior> tiles = levelRepresentation.Tiles;
            foreach(TileBehavior tile in tiles)
            {
                tile.transform.localScale = Vector3.one;
                tile.SetState(levelRepresentation.IsTileUnconcealed(tile.ElementPosition), true);
            }

            yield return null;

            onAnimationCompleted?.Invoke();
        }
    }
}