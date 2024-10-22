using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��������� IAP ģ�飬������ Unity Purchasing �����ռ�
#if MODULE_IAP
using UnityEngine.Purchasing;
#endif

namespace Watermelon.IAPStore
{
    // ����һ�������࣬�̳��� MonoBehaviour����ʵ�� IIAPStoreOffer �ӿ�
    public abstract class IAPStoreOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] ProductKeyType productKey; // ��Ʒ�ļ����ͣ�����Ψһ��ʶÿ����Ʒ

        [Space]
        [SerializeField] IAPButton purchaseButton; // ����ť������

        public GameObject GameObject => gameObject; // ��ȡ��ǰ��Ϸ����

        private RectTransform rect; // RectTransform ���ڻ�ȡ�ö���ĸ߶�
        public float Height => rect.sizeDelta.y; // ��ȡ�ö���ĸ߶�

        private SimpleBoolSave save; // ���湺��״̬�ı���
        protected bool Bought => !save.Value; // �ж���Ʒ�Ƿ��Ѿ�����

        private ProductData product; // ��Ʒ���ݶ���

        // �ڶ����ʼ��ʱִ��
        protected virtual void Awake()
        {
            rect = GetComponent<RectTransform>(); // ��ȡ��ǰ����� RectTransform ���
        }

        // ��ʼ����Ʒ
        public void Init()
        {
            // ���� productKey ��ȡ��Ӧ�ı������ݶ���
            save = SaveController.GetSaveObject<SimpleBoolSave>($"Product_{productKey}");

            // ��ȡ��Ʒ����
            product = IAPManager.GetProductData(productKey);

            // �����Ʒ�Ƿ��ѹ�����Ƿ�����Ʒ���ѱ�����
            if (product.IsPurchased || product.ProductType == ProductType.NonConsumable && save.Value)
            {
                ReapplyOffer(); // ����Ӧ����ƷЧ��
                if (product.ProductType == ProductType.NonConsumable)
                    Disable(); // ������Ʒ������Ƿ�����Ʒ
                else
                    purchaseButton.Init(productKey); // ��ʼ������ť
            }
            else
            {
                purchaseButton.Init(productKey); // ��ʼ������ť
                IAPManager.OnPurchaseComplete += OnPurchaseComplete; // ���Ĺ�������¼�
            }
        }

        // ������Ʒ��ֹͣ������������¼�
        public void Disable()
        {
            IAPManager.OnPurchaseComplete -= OnPurchaseComplete; // ȡ�������¼�
            gameObject.SetActive(false); // ���ص�ǰ��Ϸ����
        }

        // ������ɺ����
        private void OnPurchaseComplete(ProductKeyType key)
        {
            if (productKey == key) // ��鵱ǰ��Ʒ�Ƿ��빺����ɵ���Ʒƥ��
            {
                ApplyOffer(); // Ӧ�ù�����Ч��

                if (product.ProductType == ProductType.NonConsumable) Disable(); // ����Ƿ�����Ʒ�������Ʒ

                save.Value = true; // ���±���״̬Ϊ�ѹ���
            }
        }

        // ���󷽷���Ӧ�ù�����Ч��
        protected abstract void ApplyOffer();

        // ���󷽷�������Ӧ����ƷЧ��
        protected abstract void ReapplyOffer();
    }
}
