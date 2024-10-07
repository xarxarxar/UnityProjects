using System.Collections;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Simple Spawn Animation", menuName = "Content/Level/Simple Spawn Animation")]
    public class SimpleSpawnAnimation : LevelSpawnAnimation
    {
        [SerializeField] float scaleTime = 0.4f;
        [SerializeField] Ease.Type scaleEasing = Ease.Type.BackOut;

        [Space]
        [SerializeField] float elementDelay = 0.07f;
        [SerializeField] float layerDelay = 0.2f;
        
        private OptimisedTilesSimpleScaleTweenCase optimisedScaleTweenCase;

        protected override IEnumerator SpawnLevelCoroutine(LevelRepresentation levelRepresentation, SimpleCallback onAnimationCompleted)
        {
            optimisedScaleTweenCase = new OptimisedTilesSimpleScaleTweenCase(levelRepresentation, scaleTime, elementDelay, layerDelay);
            optimisedScaleTweenCase.SetEasing(scaleEasing);
            optimisedScaleTweenCase.OnComplete(() => onAnimationCompleted?.Invoke());
            optimisedScaleTweenCase.StartTween();

            yield return null;
        }

        public override void Clear()
        {
            base.Clear();

            optimisedScaleTweenCase.KillActive();
        }
    }
}