using UnityEngine;

namespace Watermelon
{

    public class IceTileEffect : TileEffect
    {
        [SerializeField] SpriteRenderer iceSpriteRenderer;
        [SerializeField] Sprite[] stageSprites;
        [SerializeField] Particle hitParticle;

        private SpriteRenderer iconSpriteRenderer;
        private TweenCase shakeTweenCase;

        private int stage;

        public override void Initialise()
        {
            ParticlesController.RegisterParticle(hitParticle);
        }

        public override void OnCreated(TileBehavior tileBehavior)
        {
            iconSpriteRenderer = linkedTile.IconSpriteRenderer;

            stage = 0;
            iceSpriteRenderer.sprite = stageSprites[0];
            iceSpriteRenderer.sortingOrder = iconSpriteRenderer.sortingOrder + 1;
        }

        public override void OnSortingOrderChanged(int order)
        {
            iceSpriteRenderer.sortingOrder = linkedTile.IconSpriteRenderer.sortingOrder + 1;
        }

        public override void OnDisabled(TileBehavior tileBehavior)
        {
            shakeTweenCase.KillActive();
        }

        public override void OnAnyTileSubmitted()
        {
            if (linkedTile.IsClickable)
            {
                stage++;

                hitParticle.Play().SetPosition(transform.position);

                shakeTweenCase = linkedTile.transform.DOShake(0.05f, 0.08f);

                if (stageSprites.IsInRange(stage))
                {
                    iceSpriteRenderer.sprite = stageSprites[stage];
                }
                else
                {
                    DisableEffect();
                }

                AudioController.PlaySound(AudioController.Sounds.iceCrackSound, volumePercentage: 0.5f, minDelay: 0.3f);
            }
            else
            {
                if (!LevelController.IsLevelCompletable())
                {
                    hitParticle.Play().SetPosition(transform.position);

                    shakeTweenCase = linkedTile.transform.DOShake(0.05f, 0.08f);

                    DisableEffect();
                }
            }
        }

        public override void Clear()
        {
            shakeTweenCase.KillActive();

            Destroy(gameObject);
        }

        public override bool IsClickAllowed()
        {
            return false;
        }
    }
}
