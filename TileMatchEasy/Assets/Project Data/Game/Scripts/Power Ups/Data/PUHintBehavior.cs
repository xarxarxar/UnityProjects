using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PUHintBehavior : PUBehavior
    {
        private PUHintSettings customSettings;

        private TweenCase[] scaleTweenCases;
        private TweenCase disableTweenCase;

        public override void Initialise()
        {
            customSettings = (PUHintSettings)settings;
        }

        public override bool Activate()
        {
            if(!LevelController.IsBusy)
            {
                int requiredElementsCount = 3;
                TileData tileData = null;

                List<ISlotable> dockTiles = DockBehavior.GetHintSlots();
                if (dockTiles.IsNullOrEmpty())
                {
                    List<TileBehavior> activeTiles = LevelController.GetActiveTiles(true);
                    if (!activeTiles.IsNullOrEmpty())
                    {
                        tileData = activeTiles[Random.Range(0, activeTiles.Count - 1)].TileData;
                    }
                }
                else
                {
                    tileData = (dockTiles[0] as TileBehavior).TileData;

                    requiredElementsCount -= dockTiles.Count;
                }

                if (tileData != null)
                {
                    if (DockBehavior.GetSlotsAvailable() < requiredElementsCount)
                    {
                        return false;
                    }

                    IsBusy = true;

                    LevelController.SetBusyState(true);

                    List<TileBehavior> targetTiles = new List<TileBehavior>(LevelController.GetTilesByType(tileData, requiredElementsCount));

                    scaleTweenCases = new TweenCase[targetTiles.Count];
                    for (int i = 0; i < scaleTweenCases.Length; i++)
                    {
                        TileBehavior targetTile = targetTiles[i];

                        targetTile.MarkAsSubmitted();
                        targetTile.DisableEffect();
                        targetTile.SetState(true, false);
                    }

                    LevelController.SubmitElements(targetTiles);

                    disableTweenCase = Tween.DelayedCall(1.0f, () =>
                    {
                        IsBusy = false;

                        LevelController.SetBusyState(false);
                    });

                    return true;
                }
            }

            return false;
        }

        public override void ResetBehavior()
        {
            if (!scaleTweenCases.IsNullOrEmpty())
            {
                for (int i = 0; i < scaleTweenCases.Length; i++)
                {
                    scaleTweenCases[i].KillActive();
                }
            }

            disableTweenCase.KillActive();

            IsBusy = false;
        }
    }
}
