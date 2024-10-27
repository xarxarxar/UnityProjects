using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUUIPurchasePanel : MonoBehaviour, IPopupWindow
    {
        // ����������
        [SerializeField] GameObject powerUpPurchasePanel;

        // ��ȫ���� RectTransform���������䲻ͬ��Ļ
        [SerializeField] RectTransform safeAreaTransform;

        [Space(5)]
        // Ԥ����Ʒͼ��
        [SerializeField] Image powerUpPurchasePreview;

        // ��ʾ�����������ı�
        [SerializeField] TMP_Text powerUpPurchaseAmountText;

        // ��Ʒ�����ı�
        [SerializeField] TMP_Text powerUpPurchaseDescriptionText;

        // ��Ʒ�۸��ı�
        [SerializeField] TMP_Text powerUpPurchasePriceText;

        // ����ͼ��
        [SerializeField] Image powerUpPurchaseIcon;

        [Space(5)]
        // С�رհ�ť�������������Ͻǵ�СX��ť��
        [SerializeField] Button smallCloseButton;

        // ��رհ�ť������ײ���"ȡ��"��ť��
        [SerializeField] Button bigCloseButton;

        // ����ť
        [SerializeField] Button purchaseButton;

        [Space(5)]
        // ������ʾ������Ϣ��UI���
        [SerializeField] CurrencyUIPanelSimple currencyPanel;

        // ��Ʒ������Ϣ
        private PUSettings settings;

        // �Ƿ��Ѵ򿪽���
        private bool isOpened;
        public bool IsOpened => isOpened;

        // �ڽű���ʼ��ʱ���ð�ť�ĵ���¼�
        private void Awake()
        {
            // С�رհ�ť���ʱ���� ClosePurchasePUPanel ����
            smallCloseButton.onClick.AddListener(ClosePurchasePUPanel);

            // ��رհ�ť���ʱ���� ClosePurchasePUPanel ����
            bigCloseButton.onClick.AddListener(ClosePurchasePUPanel);

            // ����ť���ʱ���� PurchasePUButton ����
            purchaseButton.onClick.AddListener(PurchasePUButton);
        }

        // ��ʼ��������ע�ᰲȫ����
        public void Initialise()
        {
            // ע�ᰲȫ���� RectTransform
            NotchSaveArea.RegisterRectTransform(safeAreaTransform);
        }

        // ��ʾ������沢���ý�����Ϣ
        public void Show(PUSettings settings)
        {
            // �洢�������Ʒ������Ϣ
            this.settings = settings;

            // ��ʼ��������Ϣ���
            currencyPanel.Initialise();

            // ��ʾ�������
            powerUpPurchasePanel.SetActive(true);

            // ������ƷԤ��ͼ�ꡢ�������۸�͹�������
            powerUpPurchasePreview.sprite = settings.Icon;
            powerUpPurchaseDescriptionText.text = settings.Description;
            powerUpPurchasePriceText.text = settings.Price.ToString();
            powerUpPurchaseAmountText.text = string.Format("x{0}", settings.PurchaseAmount);

            // ��ȡ����ʾ����ͼ��
            Currency currency = CurrenciesController.GetCurrency(settings.CurrencyType);
            powerUpPurchaseIcon.sprite = currency.Icon;

            // ֪ͨUI�����������µĵ���
            UIController.OnPopupWindowOpened(this);
        }

        // ������ť����¼�
        public void PurchasePUButton()
        {
            // ���ŵ����ť����Ч
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            // ���Թ�����Ʒ���ж��Ƿ�ɹ�
            bool purchaseSuccessful = PUController.PurchasePowerUp(settings.Type);

            // �������ɹ���رչ������
            if (purchaseSuccessful)
                ClosePurchasePUPanel();
        }

        // �رչ������
        public void ClosePurchasePUPanel()
        {
            // ���ع������
            powerUpPurchasePanel.SetActive(false);

            // ֪ͨUI�������ر��˵���
            UIController.OnPopupWindowClosed(this);
        }
    }
}
