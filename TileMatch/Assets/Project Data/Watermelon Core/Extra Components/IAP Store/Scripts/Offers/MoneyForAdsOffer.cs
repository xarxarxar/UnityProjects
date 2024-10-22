using System.Collections;
using System.Collections.Generic;
using TMPro; // ���� TextMeshPro �����ռ䣬�����ı���ʾ
using UnityEngine;
using UnityEngine.UI; // ���� UI ��������ռ�

namespace Watermelon.IAPStore
{
    // ʵ�� IIAPStoreOffer �ӿڣ���ʾ����һ�� IAP �̵���Ʒ
    public class MoneyForAdsOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int coinsAmount; // ���ۿ����õĽ������
        [SerializeField] TMP_Text coinsAmountText; // ��ʾ����������ı����

        [Space]
        [SerializeField] Button button; // �����ۿ����İ�ť

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // ����Ч���ɵ�λ��
        [SerializeField] int floatingElementsAmount = 10; // ����Ч��Ư��Ԫ�ص�����

        public GameObject GameObject => gameObject; // ��ȡ��ǰ��Ϸ����

        private RectTransform rect; // ��ǰ����� RectTransform
        public float Height => rect.sizeDelta.y; // ��ȡ��ǰ����ĸ߶�

        // �ڶ����ʼ��ʱִ��
        private void Awake()
        {
            rect = GetComponent<RectTransform>(); // ��ȡ��ǰ����� RectTransform ���
        }

        // ��ʼ����ť����ʾ�Ľ������
        public void Init()
        {
            button.onClick.AddListener(OnAdButtonClicked); // Ϊ��ť��ӵ���¼�������
            coinsAmountText.text = $"x{coinsAmount}"; // ���ý���������ı�
        }

        // �����ť����ø÷���
        private void OnAdButtonClicked()
        {
            // ��ʾ������Ƶ���
            AdsManager.ShowRewardBasedVideo((watched) => {
                if (watched) // ����ۿ��˹��
                {
                    // ��ȡ IAP Store ҳ�沢���ɻ�������Ч
                    UIIAPStore iapStore = UIController.GetPage<UIIAPStore>();
                    iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
                    {
                        // ������ҵĽ������
                        CurrenciesController.Add(CurrencyType.Coins, coinsAmount);
                    });
                }
            });
        }
    }
}
