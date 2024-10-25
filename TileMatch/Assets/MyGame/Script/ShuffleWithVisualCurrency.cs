using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class ShuffleWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int price = 1000; // ����Ʒ�ۼ�

        [Space]
        [SerializeField] TMP_Text priceText; // ��ʾ�ۼ۵��ı����

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


        private void Awake()
        {
            // ��ȡ��ǰ����� RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
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

        //ʾ��
        private void AddPowerUp()
        {
            //�����㹻������²��ܹ���
            if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
            {
                // ������ҵĽ������
                CurrenciesController.Substract(CurrencyType.Coins, price);
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                {
                    PUController.AddPowerUp(PUType.Shuffle, 1);//���һ������ʾ�����ߵ�����
                });
            }
            else
            {
                Tools.BlinkRedThreeTimes(priceText, 1);//������˸��ʾ���Ҳ���
            }


            //LivesManager.AddMaxLife();//���һ���������ֵ
        }
    }
}
