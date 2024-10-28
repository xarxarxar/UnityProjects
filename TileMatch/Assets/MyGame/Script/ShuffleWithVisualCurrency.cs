using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;
using static Watermelon.Currency;
using System;

namespace Watermelon.IAPStore
{
    public class ShuffleWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int price = 1000; // ����Ʒ�ۼ�

        [Space]
        [SerializeField] GameObject currencyImage; // ���ͼ��
        [SerializeField] GameObject shareImage; // ����ͼ��

        [Space]
        [SerializeField] TMP_Text priceText; // ��ʾ�ۼ۵��ı����

        [Space]
        [SerializeField] int timerDurationInMinutes; // ��ʱ���ĳ���ʱ�䣨���ӣ�

        [Space]
        [SerializeField] string description = "����һ��ϴ��"; // ��ʾ�������ı�
        [SerializeField] TMP_Text descriptionText; // ��ʾ�������ı����

        [Space]
        [SerializeField] Button button; // ��ȡ��ҵİ�ť

        [Space]
        [SerializeField] GameObject imageObject; // ��Ʒ��ͼƬ

        public GameObject GameObject => gameObject; // ��ȡ��ǰ��Ϸ����

        private RectTransform rect; // ��ǰ����� RectTransform
        public float Height => rect.sizeDelta.y; // ��ȡ����ĸ߶�

        SimpleLongSave save; // �����ʱ��״̬�Ķ���
        DateTime timerStartTime; // ��ʱ����ʼ��ʱ��
        private bool canShare = true;

        private void Awake()
        {
            // ��ȡ��ǰ����� RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {

            // �ӱ����������ȡ��ʱ��״̬
            save = SaveController.GetSaveObject<SimpleLongSave>("Shuffle With Share");
            // �ӱ���Ķ����������лָ���ʱ����ʼ��ʱ��
            timerStartTime = DateTime.FromBinary(save.Value);

            // Ϊ��ť��ӵ���¼�������`
            button.onClick.AddListener(OnAdButtonClicked);

            priceText.text = price.ToString();

            descriptionText.text = description.ToString();
        }

        // ��ť����¼�����
        private void OnAdButtonClicked()
        {
            AddPowerUp();
        }

        private void Update()
        {
            // ���㵱ǰʱ�����ʱ����ʼʱ��Ĳ�ֵ
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // �����ʱ����
            {
                canShare = true;
                priceText.text = "����"; // ��ʾ�ı�
                shareImage.SetActive(true);
                currencyImage.SetActive(false);
            }
            else // �����ʱδ����
            {
                canShare = false;
                priceText.text = price.ToString(); // ��ʾ�ı�
                shareImage.SetActive(false);
                currencyImage.SetActive(true);

                //var timeLeft = duration - timer; // ����ʣ��ʱ��
            }
        }

        //ʾ��
        private void AddPowerUp()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            if (canShare)
            {
                CallWechat.ShareApp(() =>
                {
                    // ���±���ļ�ʱ����ʼʱ��
                    save.Value = DateTime.Now.ToBinary();
                    timerStartTime = DateTime.Now;
                    AudioController.PlaySound(AudioController.Sounds.buySuccess);
                    Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                    {
                        PUController.AddPowerUp(PUType.Shuffle, 1);//���һ������ʾ�����ߵ�����
                    });
                });
            }
            else
            {
                //�����㹻������²��ܹ���
                if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
                {
                    // ������ҵĽ������
                    CurrenciesController.Substract(CurrencyType.Coins, price);
                    AudioController.PlaySound(AudioController.Sounds.buySuccess);
                    Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                    {
                        PUController.AddPowerUp(PUType.Shuffle, 1);//���һ������ʾ�����ߵ�����
                    });
                }
                else
                {
                    Tools.BlinkRedThreeTimes(priceText, 0.5f);//������˸��ʾ���Ҳ���
                }
            }


            //LivesManager.AddMaxLife();//���һ���������ֵ
        }
    }
}
