using UnityEngine;
using System.Collections.Generic;

namespace Watermelon
{
    public class OptimisedTilesScaleTweenCase : TweenCase
    {
        private List<TileBehavior> tiles;
        private float[] delays;
        private float scaleDuration;

        private Vector3 startScale;
        public Vector3 targetScale;

        public OptimisedTilesScaleTweenCase(List<TileBehavior> tiles, float scaleDuration, float minDelay, float maxDelay)
        {
            this.tiles = tiles;
            this.scaleDuration = scaleDuration;

            startScale = new Vector3(0.01f, 0.01f, 0.01f);
            targetScale = new Vector3(1, 1, 1);

            delays = new float[tiles.Count];

            float longestDelay = 0;
            for(int i = 0; i < delays.Length; i++)
            {
                float delay = Random.Range(minDelay, maxDelay);

                if (delay > longestDelay)
                    longestDelay = delay;

                delays[i] = delay;
            }

            // Set total duration based on animation duration + longest delay
            duration = scaleDuration + longestDelay;

            // Recalculate delays
            for (int i = 0; i < delays.Length; i++)
            {
                delays[i] = delays[i] / duration;
            }
        }

        public override void DefaultComplete()
        {
            for (int i = 0; i < delays.Length; i++)
            {
                tiles[i].transform.localScale = Vector3.one;
            }
        }

        public override void Invoke(float deltaTime)
        {
            for (int i = 0; i < delays.Length; i++)
            {
                float reclampedState = Mathf.InverseLerp(delays[i], delays[i] + scaleDuration, state);

                tiles[i].transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, Interpolate(reclampedState));
            }
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
