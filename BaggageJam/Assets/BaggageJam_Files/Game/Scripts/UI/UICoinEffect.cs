namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using NaughtyAttributes;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public class UICoinEffect : InstanceManager<UICoinEffect>
    {
        [SerializeField] private Transform CoinImage;
        [SerializeField] private Transform coinBg;
        [SerializeField] private Vector2[] initialPos;
        [SerializeField] private Quaternion[] initialRotation;
        [SerializeField] private int coinsAmount;
        void Start()
        {
    
            if (coinsAmount == 0)
                coinsAmount = 10; // you need to change this value based on the number of coins in the inspector
    
            if (coinsAmount >= transform.childCount)
            {
                coinsAmount = transform.childCount;
            }
    
            initialPos = new Vector2[coinsAmount];
            initialRotation = new Quaternion[coinsAmount];
    
            for (int i = 0; i < coinsAmount; i++)
            {
                initialPos[i] = transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
                initialRotation[i] = transform.GetChild(i).GetComponent<RectTransform>().rotation;
            }
        }
    
    
        [Button]
        public void CountCoins()
        {
            var delay = 0f;
    
            for (int i = 0; i < coinsAmount; i++)
            {
                transform.GetChild(0).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
    
                transform.GetChild(0).DOMove(CoinImage.position, 0.8f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);
    
    
                transform.GetChild(0).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                    .SetEase(Ease.Flash);
    
    
                transform.GetChild(0).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);
    
                transform.GetChild(0).parent = coinBg.GetChild(0);
    
                delay += 0.1f;
    
            }
            StartCoroutine(CountDollars(delay, GameManager.Instance.data.levelCount));
        }
    
        IEnumerator CountDollars(float delay, int level)
        {
            yield return new WaitForSecondsRealtime(delay);
    
            Transform coinIcon = UIManager.Instance.MoneyIcon.transform;
    
            coinIcon.DOScale(Vector3.one * 1.4f, .15f).SetDelay(.2f)
            .SetEase(Ease.Flash)
            .OnComplete(() => coinIcon.DOScale(Vector3.one, .1f));
    
    
            yield return new WaitForSecondsRealtime(.5f);
    
            GameData data = GameManager.Instance.data;

            float money = data.totalMoney + data.AllSo.LevelDataSO.GetWinMoney(level);

            DOTween.To(x => data.totalMoney = x, data.totalMoney, money, 1)
            .OnUpdate(() =>
            {
                UIManager.Instance.UpdateText();
            })
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.1f, () => SceneManager.LoadScene(0));
            });
    
        }
    }
    
}
