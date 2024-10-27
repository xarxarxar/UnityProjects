using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUUIPurchasePanel : MonoBehaviour, IPopupWindow
    {
        // 购买界面面板
        [SerializeField] GameObject powerUpPurchasePanel;

        // 安全区域 RectTransform，用于适配不同屏幕
        [SerializeField] RectTransform safeAreaTransform;

        [Space(5)]
        // 预览商品图像
        [SerializeField] Image powerUpPurchasePreview;

        // 显示购买数量的文本
        [SerializeField] TMP_Text powerUpPurchaseAmountText;

        // 商品描述文本
        [SerializeField] TMP_Text powerUpPurchaseDescriptionText;

        // 商品价格文本
        [SerializeField] TMP_Text powerUpPurchasePriceText;

        // 货币图标
        [SerializeField] Image powerUpPurchaseIcon;

        [Space(5)]
        // 小关闭按钮（例如用于右上角的小X按钮）
        [SerializeField] Button smallCloseButton;

        // 大关闭按钮（例如底部的"取消"按钮）
        [SerializeField] Button bigCloseButton;

        // 购买按钮
        [SerializeField] Button purchaseButton;

        [Space(5)]
        // 用于显示货币信息的UI面板
        [SerializeField] CurrencyUIPanelSimple currencyPanel;

        // 商品设置信息
        private PUSettings settings;

        // 是否已打开界面
        private bool isOpened;
        public bool IsOpened => isOpened;

        // 在脚本初始化时设置按钮的点击事件
        private void Awake()
        {
            // 小关闭按钮点击时调用 ClosePurchasePUPanel 方法
            smallCloseButton.onClick.AddListener(ClosePurchasePUPanel);

            // 大关闭按钮点击时调用 ClosePurchasePUPanel 方法
            bigCloseButton.onClick.AddListener(ClosePurchasePUPanel);

            // 购买按钮点击时调用 PurchasePUButton 方法
            purchaseButton.onClick.AddListener(PurchasePUButton);
        }

        // 初始化方法，注册安全区域
        public void Initialise()
        {
            // 注册安全区域 RectTransform
            NotchSaveArea.RegisterRectTransform(safeAreaTransform);
        }

        // 显示购买界面并设置界面信息
        public void Show(PUSettings settings)
        {
            // 存储传入的商品设置信息
            this.settings = settings;

            // 初始化货币信息面板
            currencyPanel.Initialise();

            // 显示购买界面
            powerUpPurchasePanel.SetActive(true);

            // 设置商品预览图标、描述、价格和购买数量
            powerUpPurchasePreview.sprite = settings.Icon;
            powerUpPurchaseDescriptionText.text = settings.Description;
            powerUpPurchasePriceText.text = settings.Price.ToString();
            powerUpPurchaseAmountText.text = string.Format("x{0}", settings.PurchaseAmount);

            // 获取并显示货币图标
            Currency currency = CurrenciesController.GetCurrency(settings.CurrencyType);
            powerUpPurchaseIcon.sprite = currency.Icon;

            // 通知UI控制器打开了新的弹窗
            UIController.OnPopupWindowOpened(this);
        }

        // 处理购买按钮点击事件
        public void PurchasePUButton()
        {
            // 播放点击按钮的音效
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            // 尝试购买商品并判断是否成功
            bool purchaseSuccessful = PUController.PurchasePowerUp(settings.Type);

            // 如果购买成功则关闭购买界面
            if (purchaseSuccessful)
                ClosePurchasePUPanel();
        }

        // 关闭购买界面
        public void ClosePurchasePUPanel()
        {
            // 隐藏购买界面
            powerUpPurchasePanel.SetActive(false);

            // 通知UI控制器关闭了弹窗
            UIController.OnPopupWindowClosed(this);
        }
    }
}
