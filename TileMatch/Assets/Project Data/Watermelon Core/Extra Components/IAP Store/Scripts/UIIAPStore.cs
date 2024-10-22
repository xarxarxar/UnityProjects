using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class UIIAPStore : UIPage
    {
        [SerializeField] VerticalLayoutGroup layout; // ����������Ʒ�Ĵ�ֱ������
        [SerializeField] RectTransform safeAreaTransform; // ��ȫ����ľ��α任�����䲻ͬ�豸�ı�Ե
        [SerializeField] RectTransform content; // �����Ʒ����������
        [SerializeField] Button closeButton; // �ر��̵�İ�ť
        [SerializeField] CurrencyUIPanelSimple coinsUI; // ��ʾ��ҽ�ҵ� UI ���

        private TweenCase[] appearTweenCases; // �洢���ֶ����� Tween ����

        public static bool IsStoreAvailable { get; private set; } = false; // �洢�̵��Ƿ���õ�״̬

        private List<IIAPStoreOffer> offers = new List<IIAPStoreOffer>(); // �洢�̵��е���Ʒ

        private void Awake()
        {
            // ��ȡ�Ӷ����е����� IIAPStoreOffer �ӿ�ʵ��
            content.GetComponentsInChildren(offers);//���ַ�ʽ������� offers �б������Ƿ����µ�������б����ַ������᷵��ֵ�����ǽ��ҵ��������ӵ������ List �С�
            closeButton.onClick.AddListener(OnCloseButtonClicked); // �󶨹رհ�ť�ĵ���¼�
        }

        public override void Initialise()
        {
            // ���Ĺ���ģ���ʼ�����¼�
            IAPManager.SubscribeOnPurchaseModuleInitted(InitOffers);

            // ע�ᰲȫ������α任
            NotchSaveArea.RegisterRectTransform(safeAreaTransform);

            // ��ʼ����� UI
            coinsUI.Initialise();
        }

        private void InitOffers()
        {
            // ��ʼ��ÿ����Ʒ
            foreach (var offer in offers)
            {
                offer.Init();
            }

            IsStoreAvailable = true; // �����̵�Ϊ����

            // ����һ֡������������ĸ߶�
            Tween.NextFrame(() => {
                float height = layout.padding.top + layout.padding.bottom; // �����ܸ߶ȣ������ϱ߾���±߾�

                for (int i = 0; i < offers.Count; i++)
                {
                    var offer = offers[i];
                    if (offer.GameObject.activeSelf) // ������ɼ���Ʒ�ĸ߶�
                    {
                        height += offer.Height; // ������Ʒ�߶�
                        if (i != offers.Count - 1) height += layout.spacing; // ����Ʒ�����Ӽ��
                    }
                }

                // ������������ĸ߶�
                content.sizeDelta = content.sizeDelta.SetY(height + 200);
            });
        }

        public override void PlayHideAnimation()
        {
            // ���̵�����ʱ��֪ͨҳ��ر�
            UIController.OnPageClosed(this);
        }

        public override void PlayShowAnimation()
        {
            appearTweenCases.KillActive(); // ��ֹ�κ����ڽ��еĶ���

            appearTweenCases = new TweenCase[offers.Count];
            for (int i = 0; i < offers.Count; i++)
            {
                Transform offerTransform = offers[i].GameObject.transform;
                offerTransform.transform.localScale = Vector3.zero; // ��ʼ������Ϊ��
                appearTweenCases[i] = offerTransform.transform.DOScale(1.0f, 0.3f, i * 0.05f).SetEasing(Ease.Type.CircOut); // ʹ�� Tween �����Ŵ���Ʒ
            }

            // �رհ�ť�����Ŷ���
            closeButton.transform.localScale = Vector3.zero;
            closeButton.transform.DOScale(1.0f, 0.3f, 0.2f).SetEasing(Ease.Type.BackOut);

            content.anchoredPosition = Vector2.zero; // ������������λ��

            // ������ɺ�֪ͨҳ���Ѵ�
            appearTweenCases[^1].OnComplete(() =>
            {
                UIController.OnPageOpened(this);
            });
        }

        public void Hide()
        {
            appearTweenCases.KillActive(); // ��ֹ�κ����ڽ��еĶ���

            // ���� IAP Store ҳ��
            UIController.HidePage<UIIAPStore>();
        }

        private void OnCloseButtonClicked()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound); // ���Ű�ť�����Ч

            // ���� IAP Store ҳ��
            UIController.HidePage<UIIAPStore>();
        }

        public void SpawnCurrencyCloud(RectTransform spawnRectTransform, CurrencyType currencyType, int amount, SimpleCallback completeCallback = null)
        {
            // ���ɻ�������Ч
            FloatingCloud.SpawnCurrency(currencyType.ToString(), spawnRectTransform, coinsUI.RectTransform, amount, null, completeCallback);
        }
    }
}

// -----------------
// IAP Store v1.2
// -----------------

// ������־
// v 1.2
// ? ������ƶ��豸�İ���ƫ��֧��
// ? �������Ѽ�ʱ��ҽ���
// ? ����˹���ҽ���
// v 1.1
// ? �������Ʒ�ӿ�
// ? ��ƷԤ�Ƽ�������
// v 1.0
// ? �����߼�
