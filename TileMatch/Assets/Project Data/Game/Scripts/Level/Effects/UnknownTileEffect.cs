using UnityEngine;

namespace Watermelon
{

    public class UnknownTileEffect : TileEffect
    {
        [SerializeField] Sprite unknownSprite;

        private Sprite storedSprite;
        private SpriteRenderer spriteRenderer;

        public override void OnCreated(TileBehavior tileBehavior)
        {
            spriteRenderer = linkedTile.IconSpriteRenderer;

            storedSprite = spriteRenderer.sprite;
            spriteRenderer.sprite = unknownSprite;
        }

        public override void OnDisabled(TileBehavior tileBehavior)
        {
            spriteRenderer.sprite = storedSprite;
        }

        public override void OnTileSubmitted()
        {
            DisableEffect();
        }

        public override void Clear()
        {
            spriteRenderer.sprite = storedSprite;
        }
    }
}
