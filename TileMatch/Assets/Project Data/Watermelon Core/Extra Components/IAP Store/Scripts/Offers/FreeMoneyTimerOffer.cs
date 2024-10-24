using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Watermelon.Currency;

namespace Watermelon.IAPStore
{
    // ʵ�� IIAPStoreOffer �ӿڵ� FreeMoneyTimerOffer ��
    public class FreeMoneyTimerOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int coinsAmount; // ��ҿ�����ȡ�Ľ������
        [SerializeField] TMP_Text coinsAmountText; // ��ʾ����������ı����

        [Space]
        [SerializeField] TMP_Text timerText; // ��ʾ��ʱ�����ı����
        [SerializeField] int timerDurationInMinutes; // ��ʱ���ĳ���ʱ�䣨���ӣ�

        [Space]
        [SerializeField] Button button; // ��ȡ��ҵİ�ť

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // ��Ʈ��Ч��������λ��
        [SerializeField] int floatingElementsAmount = 10; // ����Ư��Ԫ�ص�����

        public GameObject GameObject => gameObject; // ��ȡ��ǰ��Ϸ����

        private RectTransform rect; // ��ǰ����� RectTransform
        public float Height => rect.sizeDelta.y; // ��ȡ����ĸ߶�

        SimpleLongSave save; // �����ʱ��״̬�Ķ���
        DateTime timerStartTime; // ��ʱ����ʼ��ʱ��

        private void Awake()
        {
            // ��ȡ��ǰ����� RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
            // �ӱ����������ȡ��ʱ��״̬
            save = SaveController.GetSaveObject<SimpleLongSave>("Free Money Timer");

            // �ӱ���Ķ����������лָ���ʱ����ʼ��ʱ��
            timerStartTime = DateTime.FromBinary(save.Value);

            // Ϊ��ť��ӵ���¼�������
            button.onClick.AddListener(OnAdButtonClicked);
            // ��ʾ����ȡ�Ľ������
            coinsAmountText.text = $"x{coinsAmount}";
        }

        private void Update()
        {
            // ���㵱ǰʱ�����ʱ����ʼʱ��Ĳ�ֵ
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // �����ʱ����
            {
                button.enabled = true; // ���ð�ť
                timerText.text = "���!"; // ��ʾ"��ѻ�ȡ"�ı�
            }
            else // �����ʱδ����
            {
                button.enabled = false; // ���ð�ť

                var timeLeft = duration - timer; // ����ʣ��ʱ��

                // ��ʽ��ʣ��ʱ�䲢������ʾ
                if (timeLeft.Hours > 0)
                {
                    timerText.text = string.Format("{0:hh\\:mm\\:ss}", timeLeft);
                }
                else
                {
                    timerText.text = string.Format("{0:mm\\:ss}", timeLeft);
                }

                // ��̬������ʱ�ı��Ͱ�ť�Ŀ��
                var prefferedWidth = timerText.preferredWidth;
                if (prefferedWidth < 270) prefferedWidth = 270;

                timerText.rectTransform.sizeDelta = timerText.rectTransform.sizeDelta.SetX(prefferedWidth + 5);
                button.image.rectTransform.sizeDelta = button.image.rectTransform.sizeDelta.SetX(prefferedWidth + 10);
            }
        }

        // ��ť����¼�����
        private void OnAdButtonClicked()
        {
            // ���±���ļ�ʱ����ʼʱ��
            save.Value = DateTime.Now.ToBinary();
            timerStartTime = DateTime.Now;

            // ��ȡ IAP Store ҳ�沢���ɻ�������Ч����
            UIIAPStore iapStore = UIController.GetPage<UIIAPStore>();
            iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
            {
                // ������ҵĽ������
                CurrenciesController.Add(CurrencyType.Coins, coinsAmount);
            });
        }
    }
}
