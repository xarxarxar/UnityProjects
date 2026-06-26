namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using NaughtyAttributes;
    using UnityEngine;
    
    public class EarnCoin : MonoBehaviour
    {
    
        void Start()
        {
    
        }
        [Button]
        public void GetDiamond()
        {
    
            Vector3 MoneyIconPos = Camera.main.transform.GetChild(1).position;
    
            Vector3 randomPos = transform.position + new Vector3(Random.Range(1.5f, -1.5f), 0, Random.Range(1f, .1f));
            transform.DOLocalJump(randomPos, 3f, 1, 1f).OnComplete(() =>
            {
                transform.DOScale(Vector3.one * .4f, .5f).SetDelay(.2f).SetEase(Ease.InCubic);
    
                transform.DOJump(MoneyIconPos, 1, 1, .7f).SetEase(Ease.InCubic).
                OnComplete(() =>
                {
                    Transform coinIcon = UIManager.Instance.MoneyIcon.transform;
                    coinIcon.DOScale(Vector3.one * 1.4f, .1f)
                    .SetEase(Ease.Flash)
                    .OnComplete(() => coinIcon.DOScale(Vector3.one, .1f));
    
                    Destroy(gameObject);
                });
            });
    
        }
    }
}
