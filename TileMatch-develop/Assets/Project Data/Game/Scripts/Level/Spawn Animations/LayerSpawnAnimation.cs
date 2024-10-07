using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Layer Spawn Animation", menuName = "Content/Level/Layer Spawn Animation")]
    public class LayerSpawnAnimation : LevelSpawnAnimation
    {
        private readonly Vector3[] OFFSET = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(-1, 0, 0), new Vector3(0, -1, 0) };

        [SerializeField] float offsetMultiplier = 5;
        [SerializeField] float moveTime = 0.4f;
        [SerializeField] Ease.Type moveEasing = Ease.Type.BackOut;

        [Space]
        [SerializeField] float layerDelay = 0.4f;

        private TweenCase[] layerTweenCase;

        protected override IEnumerator SpawnLevelCoroutine(LevelRepresentation levelRepresentation, SimpleCallback onAnimationCompleted)
        {
            int offsetDirectionIndex = 0;

            // Reset objects
            List<TileBehavior> tileBehaviors = levelRepresentation.Tiles;
            foreach (TileBehavior tileBehavior in tileBehaviors)
            {
                tileBehavior.transform.localScale = Vector3.one;
                tileBehavior.SetState(false, false);
            }

            LayersMatrix layers = levelRepresentation.Layers;

            // Reset positions
            Vector3[] storedPositions = new Vector3[layers.Count];
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                Transform layerTransform = layers[i].LayerObject.transform;

                storedPositions[i] = layerTransform.localPosition;

                layerTransform.localPosition = OFFSET[offsetDirectionIndex % OFFSET.Length] * offsetMultiplier;

                offsetDirectionIndex++;
            }

            layerTweenCase = new TweenCase[storedPositions.Length];

            WaitForSeconds layerDelayYieldInstruction = new WaitForSeconds(layerDelay);

            // Reset offset index
            offsetDirectionIndex = 0;

            // Start animation
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                LayerGrid layer = layers[i];
                List<TileBehavior> layerTiles = new List<TileBehavior>();

                for (int x = 0; x < layer.Width; x++)
                {
                    for (int y = 0; y < layer.Height; y++)
                    {
                        if (layer[x, y].State)
                        {
                            layerTiles.Add(layer[x, y].Tile);
                        }
                    }
                }

                Transform layerTransform = layers[i].LayerObject.transform;
                layerTweenCase[i] = layerTransform.DOLocalMove(storedPositions[i], moveTime).SetEasing(moveEasing);

                for (int m = 0; m < layerTiles.Count; m++)
                {
                    layerTiles[m].SetState(levelRepresentation.IsTileUnconcealed(layerTiles[m]));
                }

                yield return layerDelayYieldInstruction;

                offsetDirectionIndex++;
            }

            onAnimationCompleted?.Invoke();
        }

        public override void Clear()
        {
            base.Clear();

            layerTweenCase.KillActive();
        }
    }
}