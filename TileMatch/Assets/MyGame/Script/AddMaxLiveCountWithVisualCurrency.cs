using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using Watermelon.IAPStore;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class AddMaxLiveCountWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] LivesData data; // �������ݵ������࣬���ڴ洢����������������ָ�ʱ������Ϣ
        [SerializeField] int price = 1000; // ����Ʒ�ۼ�
        [Space]
        [SerializeField] TMP_Text priceText; // ��ʾ�ۼ۵��ı����

        [Space]
        [SerializeField] string description = "�������������ֵ"; // ��ʾ�������ı�
        [SerializeField] TMP_Text descriptionText; // ��ʾ�������ı����

        [Space]
        [SerializeField] Button button; // ����İ�ť

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
            description = $"��������1�������ֵ";
            descriptionText.text = description.ToString();

            //�����ǰ���������ֵ�Ѿ�����99�ˣ�����ť���ɵ��
            if (data.customedMaxLivesCount >= 99)
            {
                button.interactable = false;
                priceText.text = "���";
            }
        }

        // ��ť����¼�����
        private void OnAdButtonClicked()
        {
            ReduceLiveInterval();
            //�����ǰ���������ֵ�Ѿ�����99�ˣ�����ť���ɵ��
            if (data.customedMaxLivesCount >= 99)
            {
                button.interactable = false;
                priceText.text = "���";
            }
        }

        //ʾ��
        private void ReduceLiveInterval()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            //�����㹻������²��ܹ���
            if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
            {
                // ������ҵĽ������
                CurrenciesController.Substract(CurrencyType.Coins, price);
                LivesManager.AddMaxLife();//���һ���������ֵ
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>{});
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            }
            else
            {
                Tools.BlinkRedThreeTimes(priceText, 0.5f);//������˸��ʾ���Ҳ���
            }
            //LivesManager.AddMaxLife();//���һ���������ֵ
        }
    }
}
