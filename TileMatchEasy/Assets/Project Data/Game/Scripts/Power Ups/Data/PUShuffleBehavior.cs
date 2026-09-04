using UnityEngine;
using System.Collections.Generic;

namespace Watermelon
{
    // Attempt 1 - https://pastebin.com/SKsuyMnk
    public class PUShuffleBehavior : PUBehavior
    {
        private PUShuffleSettings customSettings;

        private TweenCase delayTweenCase;

        public override void Initialise()
        {
            customSettings = (PUShuffleSettings)settings;
        }

        public override bool Activate()
        {
            if (!LevelController.IsBusy)
            {
                LevelRepresentation levelRepresentation = LevelController.LevelRepresentation;

                List<TileBehavior> activeTiles = levelRepresentation.Tiles;
                if (activeTiles != null)
                {
                    if (activeTiles.Count > 1)
                    {
                        List<TileBehavior> allowedToShuffleTiles = new List<TileBehavior>(activeTiles);

                        for (int i = allowedToShuffleTiles.Count - 1; i >= 0; i--)
                        {
                            if (!allowedToShuffleTiles[i].IsShuffleAllowed())
                            {
                                allowedToShuffleTiles.RemoveAt(i);
                            }
                        }

                        if (allowedToShuffleTiles.Count > 1)
                        {
                            IsBusy = true;

                            LevelController.SetBusyState(true);

                            RaycastController.Disable();

                            ElementPosition[] shuffleElements = new ElementPosition[allowedToShuffleTiles.Count];
                            for (int i = 0; i < shuffleElements.Length; i++)
                            {
                                shuffleElements[i] = allowedToShuffleTiles[i].ElementPosition;
                            }

                            shuffleElements.Shuffle();

                            // Reset tiles scale
                            for (int i = 0; i < activeTiles.Count; i++)
                            {
                                activeTiles[i].transform.localScale = Vector3.zero;
                            }

                            // Reposition shuffled tiles
                            for (int i = 0; i < shuffleElements.Length; i++)
                            {
                                allowedToShuffleTiles[i].transform.localScale = Vector3.zero;
                                allowedToShuffleTiles[i].transform.localPosition = LevelScaler.GetPosition(shuffleElements[i]);
                                allowedToShuffleTiles[i].SetPosition(shuffleElements[i]);
                            }

                            levelRepresentation.RelinkTiles();

                            OptimisedTilesScaleTweenCase optimisedShuffleTweenCase = new OptimisedTilesScaleTweenCase(activeTiles, customSettings.ScaleTime, customSettings.ScaleMinDelay, customSettings.ScaleMaxDelay);
                            optimisedShuffleTweenCase.SetEasing(customSettings.ScaleEasingType);
                            optimisedShuffleTweenCase.OnComplete(() =>
                            {
                                RaycastController.Enable();

                                IsBusy = false;

                                LevelController.SetBusyState(false);
                            });

                            optimisedShuffleTweenCase.StartTween();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void ResetBehavior()
        {
            IsBusy = false;

            delayTweenCase.KillActive();
        }
    }
}
