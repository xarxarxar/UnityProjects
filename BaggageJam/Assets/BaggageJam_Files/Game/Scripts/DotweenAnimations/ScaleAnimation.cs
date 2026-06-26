namespace EKStudio
{
    using UnityEngine;
    using DG.Tweening;
    
    public enum ScaleDirection { ScaleUp, ScaleDown }
    
    public class ScaleAnimation : MonoBehaviour
    {
        public ScaleDirection ScaleDirection = ScaleDirection.ScaleUp;
        public Ease Ease = Ease.Linear;
        public float Duration = 1f;
        public float ScaleFactor = 1.1f;
        public float Delay = 0f;
        public int LoopCount = -1; // -1 means infinite loop by default
    
        private Vector3 originalScale;
    
        private void Start()
        {
            originalScale = transform.localScale;
    
            switch (ScaleDirection)
            {
                case ScaleDirection.ScaleUp:
                    ScaleUp();
                    break;
                case ScaleDirection.ScaleDown:
                    ScaleDown();
                    break;
                default:
                    break;
            }
        }
    
        void ScaleUp()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(originalScale * ScaleFactor, Duration))
                .SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(LoopCount, LoopType.Yoyo);
        }
    
        void ScaleDown()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(originalScale * (1 / ScaleFactor), Duration))
                .SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(LoopCount, LoopType.Yoyo);
        }
    }
    
}
