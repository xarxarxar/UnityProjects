namespace EKStudio
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    
    public enum HoverDirection { Vertical, Horizontal,Forward }
    
    public class HoverAnimation : MonoBehaviour
    {
        public HoverDirection HoverDirection = HoverDirection.Vertical;
        public Ease Ease = Ease.Linear;
        public float Duration = 1f;
        public float HoverDistance = 100f;
        public float Delay = 0f;
    
        private void Start()
        {
            switch (HoverDirection)
            {
                case HoverDirection.Vertical:
                    VerticalMovement();
                    break;
                case HoverDirection.Horizontal:
                    HorizontalMovement();
                    break;
                case HoverDirection.Forward:
                    ForwardMovement();
                    break;
                default:
    
                    break;
            }
        }
        void HorizontalMovement()
        {
            transform.DOLocalMoveX(transform.localPosition.x + HoverDistance * 2f, Duration).SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(-1);
        }
        void VerticalMovement()
        {
            transform.DOLocalMoveY(transform.localPosition.y + HoverDistance * 2f, Duration).SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(-1);
        }
        void ForwardMovement()
        {
            transform.DOLocalMoveZ(transform.localPosition.z + HoverDistance * 2f, Duration).SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(-1, LoopType.Yoyo);
        }
    
    }
    
}
