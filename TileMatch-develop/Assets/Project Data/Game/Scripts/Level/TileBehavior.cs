#pragma warning disable 0649

using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public class TileBehavior : MonoBehaviour, IClickableObject, ISlotable
    {
        private static readonly int OVERLAY_STRENGTH_ID = Shader.PropertyToID("_OverlayStrength");

        [SerializeField] SpriteRenderer iconSpriteRenderer;
        public SpriteRenderer IconSpriteRenderer => iconSpriteRenderer;
        [SerializeField] SpriteRenderer backgroundSpriteRenderer;
        public SpriteRenderer BackgroundSpriteRenderer => backgroundSpriteRenderer;

        [SerializeField] Transform visuals;
        public Transform VisualsTransform => visuals;

        [SerializeField] BoxCollider boxCollider;

        public Transform Transform => transform;

        private bool isClickable;
        public bool IsClickable => isClickable;

        public int LayerID => elementPosition.LayerId;

        protected ElementPosition elementPosition;
        public ElementPosition ElementPosition => elementPosition;

        protected bool isSubmitted;
        public bool IsSubmitted => isSubmitted;

        protected bool isBlocked;
        public bool IsBlocked => isBlocked;

        protected TileData tileData;

        public TileData TileData => tileData;

        private TweenCase shakeTweenCase;

        protected TileEffect effect;
        public TileEffect Effect => effect;

        public int UniqueElementID => tileData.GetHashCode();

        private TweenCase backgroundColorTweenCase;
        private TweenCase iconColorTweenCase;

        private static GameData gameData;

        public static implicit operator ElementPosition(TileBehavior tileBehavior) => tileBehavior.elementPosition;

        public virtual void Initialise(TileData tileData, ElementPosition elementPosition)
        {
            this.tileData = tileData;

            isBlocked = false;

            SetPosition(elementPosition);
        }

        public void SetPosition(ElementPosition elementPosition)
        {
            this.elementPosition = elementPosition;

            SetSortingOrder(-(elementPosition.LayerId + 1) * 100 + elementPosition.X - elementPosition.Y);
        }

        public void SetScale(Vector2 scale)
        {
            var smallerSize = scale.x > scale.y ? scale.y : scale.x;
            visuals.localScale = Vector3.one * smallerSize;
            boxCollider.size = scale;
        }

        public void SetSortingOrder(int order)
        {
            iconSpriteRenderer.sortingOrder = order * 3 - 1;
            backgroundSpriteRenderer.sortingOrder = order * 3 - 2;

            if (effect != null)
                effect.OnSortingOrderChanged(order * 3);
        }

        public void DockShuffleMove(Transform position, SimpleCallback onCompleted)
        {
            transform.DOMove(position, 0.15f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        public void SubmitMove(Transform position, Vector3 scale, SimpleCallback onCompleted)
        {
            SetSortingOrder(0);

            transform.DORotate(Quaternion.Euler(new Vector3(0, 0, Random.value * 5 + 15 * (Random.value > 0.5f ? -1 : 1))), 0.05f);
            transform.DOScale(1.4f, 0.05f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
            {
                var smallerSize = scale.x > scale.y ? scale.y : scale.x;
                visuals.DOScale(smallerSize, 0.15f);
                transform.DOScale(1, 0.15f);

                transform.DOMove(position, 0.15f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
                {
                    transform.DORotate(Quaternion.Euler(Vector3.zero), 0.15f).SetCustomEasing(Ease.GetCustomEasingFunction("Dock Rotation Easing"));
                    onCompleted?.Invoke();
                });
            });
        }

        public void SubmitMove(Vector3 position, Vector3 scale, SimpleCallback onCompleted)
        {
            SetSortingOrder(0);

            transform.DORotate(Quaternion.Euler(new Vector3(0, 0, Random.value * 5 + 15 * (Random.value > 0.5f ? -1 : 1))), 0.05f);
            transform.DOScale(1.4f, 0.05f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
            {
                var smallerSize = scale.x > scale.y ? scale.y : scale.x;
                visuals.DOScale(smallerSize, 0.15f);
                transform.DOScale(1, 0.15f);

                transform.DOMove(position, 0.15f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
                {
                    transform.DORotate(Quaternion.Euler(Vector3.zero), 0.15f).SetCustomEasing(Ease.GetCustomEasingFunction("Dock Rotation Easing"));
                    onCompleted?.Invoke();
                });
            });
        }

        public void SetState(bool state, bool withAnimation = true)
        {
            if (isBlocked) return;

            if(isClickable != state && effect != null)
            {
                effect.OnStateChanged(state);
            }

            isClickable = state;

            var tintColor = (Color.white * (state ? 1f : 0.7f)).SetAlpha(1f);

            SetColor(tintColor, withAnimation);
        }

        public void SetColor(Color color, bool withAnimation)
        {
            backgroundColorTweenCase.KillActive();
            iconColorTweenCase.KillActive();

            if(withAnimation)
            {
                if(iconSpriteRenderer.color != color)
                    iconColorTweenCase = iconSpriteRenderer.DOColor(color, 0.25f);

                if (backgroundSpriteRenderer.color != color)
                    backgroundColorTweenCase = backgroundSpriteRenderer.DOColor(color, 0.25f);
            }
            else
            {
                iconSpriteRenderer.color = color;
                backgroundSpriteRenderer.color = color;
            }
        }

        public virtual void OnObjectClicked()
        {
            if (isSubmitted)
                return;

            bool isClickAllowed = true;

            if (isBlocked) isClickAllowed = false;
            if (!LevelController.SubmitIsAllowed()) isClickAllowed = false;
            if (!isClickable) isClickAllowed = false;
            if (effect != null && !effect.IsClickAllowed()) isClickAllowed = false;

            if (isClickAllowed)
            {
                if(Effect == null || Effect.BeforeTileSubmitted())
                {
                    LevelController.SubmitElement(this);
                }

                AudioController.PlaySound(AudioController.Sounds.tileClick, 1, Random.Range(0.8f, 1.2f));

                Vibration.Vibrate(VibrationIntensity.Light);
            }
            else
            {
                AudioController.PlaySound(AudioController.Sounds.tileClickBlocked);

                shakeTweenCase = transform.DOShake(0.05f, 0.08f);
            }

            Vibration.Vibrate(VibrationIntensity.Medium);
        }

        public void MarkAsSubmitted()
        {
            isSubmitted = true;

            LevelController.OnTileSubmitted(this);
        }

        public void ResetSubmitState()
        {
            isSubmitted = false;
        }

        public void Clear()
        {
            gameObject.SetActive(false);

            transform.SetParent(PoolManager.ObjectsContainerTransform);

            if (effect != null)
            {
                effect.Clear();

                Destroy(effect.gameObject);
            }

            ResetSubmitState();
        }

        public void PlaySpawnAnimation(float delay = 0)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f, delay).SetEasing(Ease.Type.BackOut);
        }

        public bool IsSameType(ISlotable other)
        {
            if (other == null)
                return false;

            var otherTile = (TileBehavior)other;
            if (otherTile == null)
                return false;

            return otherTile.tileData == this.tileData;
        }

        public bool IsShuffleAllowed()
        {
            if (effect != null)
                return effect.IsShuffleAllowed();

            return true;
        }

        public void MatchAnimation(float delay)
        {
            transform.DOScale(0, 0.25f, delay).SetCustomEasing(Ease.GetCustomEasingFunction("Dock Easing"));
            Tween.DelayedCall(0.15f + delay, () => ParticlesController.PlayParticle("Slot Highlight").SetDuration(1).SetPosition(transform.position + Vector3.back * 0.2f).SetRotation(Quaternion.Euler(Vector3.right * -90f)));
        }

        public void SetBlockState(bool state)
        {
            isBlocked = state;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (gameData == null)
            {
                var guid = AssetDatabase.FindAssets("t:GameData")[0];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
            }

            if (gameData == null || Application.isPlaying) return;

            var size = new Vector3(gameData.TileSize.x, gameData.TileSize.y, 0.01f);
            if(boxCollider == null) boxCollider.size = size;
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, size);
#endif
        }

        #region Effect
        public void ApplyEffect(TileEffect specialEffect)
        {
            DisableEffect();

            this.effect = specialEffect;

            specialEffect.OnCreated(this);
        }

        public void DisableEffect()
        {
            if (effect != null)
            {
                TileEffect tempEffect = effect;

                effect = null;

                tempEffect.OnDisabled(this);

                Destroy(tempEffect.gameObject);
            }
        }

        [Button]
        public void DebugTile()
        {
            Debug.Log(LevelController.LevelRepresentation.IsTileUnconcealed(this));
        }
        #endregion
    }
}
