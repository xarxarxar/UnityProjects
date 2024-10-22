using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Watermelon.IAPStore
{
    // CurrencyOffer ��̳��� IAPStoreOffer�����ڴ���������ҵĹ���
    public class CurrencyOffer : IAPStoreOffer
    {
        [SerializeField] int coinsAmount; // ������ҵ�����
        [SerializeField] TMP_Text currencyAmountText; // ��ʾ�����������ı����

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // ����Ч���ɵ�λ��
        [SerializeField] int floatingElementsAmount = 10; // Ư��Ԫ�ص�����

        // �� Awake �����г�ʼ��������������ʾ
        protected override void Awake()
        {
            base.Awake();
            currencyAmountText.text = $"x{coinsAmount}"; // ���û��������ı�
        }

        // ������ɹ�ʱ���ô˷���
        protected override void ApplyOffer()
        {
            UIIAPStore iapStore = UIController.GetPage<UIIAPStore>(); // ��ȡ IAP Store ����
            // ���ɻ�����Ч��������Ч����ɺ���ӻ���
            iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
            {
                CurrenciesController.Add(CurrencyType.Coins, coinsAmount); // ���������ҵ��û��˻�
            });
        }

        // ����Ӧ���ѹ������Ʒʱ���ô˷���
        protected override void ReapplyOffer()
        {
            // ������������ǿ�������Ʒ��ͨ�����û��״ι������Ҫ����Ӧ�ô˷���
            // ��˴˷���Ϊ�գ������������߼���Ҫ�ڴ˴�����
        }
    }
}
