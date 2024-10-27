using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;
using Unity.VisualScripting;


namespace Watermelon.IAPStore
{
    public class ReduceLiveIntervaWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] LivesData data; // �������ݵ������࣬���ڴ洢����������������ָ�ʱ������Ϣ
        [SerializeField] int price = 1000; // ����Ʒ�ۼ�
        [Space]
        [SerializeField] TMP_Text priceText; // ��ʾ�ۼ۵��ı����

        [Space]
        [SerializeField] int timeReduce=10; // ���ٵ�ʱ��ֵ������Ϊ��λ

        [Space]
        [SerializeField] string description = "���ü��ٻָ�ʱ��ֵ"; // ��ʾ�������ı�
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
            description = $"���ü���{timeReduce}�밮�Ļָ�ʱ��";
            descriptionText.text = description.ToString();

            //�����ǰ��ʱ�����Ѿ�С��5�����ˣ�����ť���ɵ��
            if (data.oneLifeRestorationDuration <= 300)
            {
                button.interactable = false;
                priceText.text = "���";
            }
        }

        // ��ť����¼�����
        private void OnAdButtonClicked()
        {
            ReduceLiveInterval();
            //�����ǰ��ʱ�����Ѿ�С��5�����ˣ�����ť���ɵ��
            if (data.oneLifeRestorationDuration <= 300)
            {
                button.interactable = false;
                priceText.text = "���";
            }
        }

        //ʾ��
        private void ReduceLiveInterval()
        {
            //�����㹻������²��ܹ���
            if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
            {
                // ������ҵĽ������
                CurrenciesController.Substract(CurrencyType.Coins, price);
                LivesManager.RemoveOneLifeTime(timeReduce);//����ʱ��ָ����
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>{});
            }
            else
            {
                Tools.BlinkRedThreeTimes(priceText, 1);//������˸��ʾ���Ҳ���
            }


            //LivesManager.AddMaxLife();//���һ���������ֵ
        }
    }
}
