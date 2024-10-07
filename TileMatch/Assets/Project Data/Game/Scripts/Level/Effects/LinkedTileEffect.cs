using UnityEngine;
using System.Collections.Generic;

namespace Watermelon
{
    public class LinkedTileEffect : TileEffect
    {
        [SerializeField] SpriteRenderer linkSpriteRenderer;
        [SerializeField] Particle pickUpParticle;

        private TileBehavior secondLinkedTile;

        public override void Initialise()
        {
            ParticlesController.RegisterParticle(pickUpParticle);
        }

        public override void OnCreated(TileBehavior tileBehavior)
        {
            if (tileBehavior == secondLinkedTile)
                return;

            List<TileBehavior> neighbourTiles = LevelController.GetNeighbourTiles(linkedTile.ElementPosition);

            if (neighbourTiles.IsNullOrEmpty())
            {
                DisableEffect();

                return;
            }

            secondLinkedTile = neighbourTiles.GetRandomItem();
            secondLinkedTile.ApplyEffect(this);

            Vector3 direction = secondLinkedTile.transform.position - linkedTile.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Round the angle to the nearest multiple of 90
            float roundedAngle = Mathf.Round(angle / 90) * -90;

            linkSpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, roundedAngle);
            linkSpriteRenderer.transform.position = Vector3.Lerp(secondLinkedTile.IconSpriteRenderer.transform.position, linkedTile.IconSpriteRenderer.transform.position, 0.5f);

            linkSpriteRenderer.sortingOrder = Mathf.Max(secondLinkedTile.IconSpriteRenderer.sortingOrder, linkedTile.IconSpriteRenderer.sortingOrder) + 1;

            linkSpriteRenderer.transform.localScale = Vector3.zero;
            linkSpriteRenderer.transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);
        }

        public override void OnSortingOrderChanged(int order)
        {
            if (secondLinkedTile != null && linkedTile != null)
            {
                linkSpriteRenderer.sortingOrder = Mathf.Max(secondLinkedTile.IconSpriteRenderer.sortingOrder, linkedTile.IconSpriteRenderer.sortingOrder) + 1;
            }
        }

        public override void OnDisabled(TileBehavior tileBehavior)
        {
            if (tileBehavior == linkedTile)
            {
                secondLinkedTile.DisableEffect();
            }
            else
            {
                linkedTile.DisableEffect();
            }
        }

        public override bool BeforeTileSubmitted()
        {
            if (LevelController.Dock.CountTiles(linkedTile) == 2)
            {
                LevelController.SubmitElement(linkedTile);

                return false;
            } else if (LevelController.Dock.CountTiles(secondLinkedTile) == 2)
            {
                LevelController.SubmitElement(secondLinkedTile);

                return false;
            }


            return true;
        }

        public override void OnTileSubmitted()
        {
            if (!linkedTile.IsSubmitted)
            {
                Tween.NextFrame(() =>
                {
                    if (GameController.IsGameActive)
                    {
                        LevelController.SubmitElement(linkedTile);
                    }
                }
                );
            }

            if (!secondLinkedTile.IsSubmitted)
            {
                Tween.NextFrame(() =>
                {
                    if (GameController.IsGameActive)
                    {
                        LevelController.SubmitElement(secondLinkedTile);
                    }
                }
                );
            }

            linkedTile.DisableEffect();

            AudioController.PlaySound(AudioController.Sounds.rubberSound, volumePercentage: 0.5f);
        }

        public override bool IsClickAllowed()
        {
            return linkedTile.IsClickable && secondLinkedTile.IsClickable;
        }

        public override bool IsShuffleAllowed()
        {
            return false;
        }

        public override void Clear()
        {

        }
    }
}
