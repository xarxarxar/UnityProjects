using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class OptimisedTilesSimpleScaleTweenCase : TweenCase
    {
        private LevelRepresentation levelRepresentation;
        private List<TileBehavior> tiles;
        private LayersMatrix layers;

        private float[] startTime;
        private float[] endTime;

        private float[] layerTime;

        private Vector3 startScale;
        public Vector3 targetScale;

        private int currentLayer;

        public OptimisedTilesSimpleScaleTweenCase(LevelRepresentation levelRepresentation, float scaleDuration, float elementDelay, float layerDelay)
        {
            this.levelRepresentation = levelRepresentation;

            layers = levelRepresentation.Layers;

            currentLayer = layers.Count - 1;

            startScale = new Vector3(0.01f, 0.01f, 0.01f);
            targetScale = new Vector3(1, 1, 1);

            tiles = levelRepresentation.GetSortedTiles();

            startTime = new float[tiles.Count];
            endTime = new float[tiles.Count];
            layerTime = new float[layers.Count];

            float longestDelay = 0;

            float delay = 0;
            int tileIndex = 0;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                LayerGrid layer = layers[i];

                for (int x = 0; x < layer.Width; x++)
                {
                    bool isPlaced = false;
                    for (int y = 0; y < layer.Height; y++)
                    {
                        if (layer[x, y].State)
                        {
                            TileBehavior tile = layer[x, y].Tile;

                            tile.transform.localScale = Vector3.zero;
                            tile.SetState(false, false);

                            startTime[tileIndex] = delay;
                            endTime[tileIndex] = delay + scaleDuration;

                            if (longestDelay <= endTime[tileIndex])
                                longestDelay = endTime[tileIndex];

                            isPlaced = true;

                            tileIndex++;
                        }
                    }

                    if (isPlaced)
                    {
                        delay += elementDelay;
                    }
                }

                layerTime[i] = delay;

                delay += layerDelay;
            }

            // Set total duration based on animation duration + longest delay
            duration = longestDelay;

            // Recalculate delays
            for (int i = 0; i < startTime.Length; i++)
            {
                startTime[i] = startTime[i] / duration;
                endTime[i] = endTime[i] / duration;
            }

            // Recalculate layert delays
            for(int i = 0; i < layerTime.Length; i++)
            {
                layerTime[i] = layerTime[i] / duration;
            }
        }

        public override void DefaultComplete()
        {
            for (int i = 0; i < startTime.Length; i++)
            {
                tiles[i].transform.localScale = Vector3.one;
            }
        }

        public override void Invoke(float deltaTime)
        {
            for (int i = 0; i < startTime.Length; i++)
            {
                float reclampedState = Mathf.InverseLerp(startTime[i], endTime[i], state);

                tiles[i].transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, Interpolate(reclampedState));
            }

            if (layerTime.IsInRange(currentLayer) && state >= layerTime[currentLayer])
            {
                LayerGrid layer = layers[currentLayer];
                for (int x = 0; x < layer.Width; x++)
                {
                    for (int y = 0; y < layer.Height; y++)
                    {
                        if (layer[x, y].State)
                        {
                            TileBehavior tile = layer[x, y].Tile;
                            tile.SetState(levelRepresentation.IsTileUnconcealed(tile.ElementPosition));
                        }
                    }
                }

                currentLayer--;
            }
        }

        public override bool Validate()
        {
            return true;
        }
    }
}