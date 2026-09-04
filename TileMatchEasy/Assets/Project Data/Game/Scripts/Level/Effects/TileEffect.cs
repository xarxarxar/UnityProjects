using UnityEngine;

namespace Watermelon
{
    public abstract class TileEffect : MonoBehaviour
    {
        [SerializeField] TileEffectType effectType;
        public TileEffectType EffectType => effectType;

        protected TileBehavior linkedTile;

        public virtual void Initialise() { }

        public abstract void OnCreated(TileBehavior tileBehavior);
        public abstract void OnDisabled(TileBehavior tileBehavior);

        public abstract void Clear();

        public virtual void OnStateChanged(bool state) { }
        public virtual bool BeforeTileSubmitted() { return true; }
        public virtual void OnTileSubmitted() { }
        public virtual void OnAnyTileSubmitted() { }

        public virtual void OnSortingOrderChanged(int order)
        {

        }

        public virtual bool IsClickAllowed()
        {
            return true;
        }

        public virtual bool IsShuffleAllowed()
        {
            return true;
        }

        public void DisableEffect()
        {
            if (linkedTile != null)
            {
                linkedTile.DisableEffect();
            }
        }

        public void ApplyEffect(TileBehavior tileBehavior)
        {
            GameObject effectObject = Instantiate(gameObject);
            effectObject.transform.SetParent(tileBehavior.VisualsTransform);
            effectObject.transform.ResetLocal();

            TileEffect tileEffect = effectObject.GetComponent<TileEffect>();
            tileEffect.linkedTile = tileBehavior;

            tileBehavior.ApplyEffect(tileEffect);
        }
    }
}
