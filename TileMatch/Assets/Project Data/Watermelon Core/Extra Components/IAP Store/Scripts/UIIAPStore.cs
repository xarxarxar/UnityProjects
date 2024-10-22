using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class UIIAPStore : UIPage
    {
        [SerializeField] VerticalLayoutGroup layout; // 用于排列商品的垂直布局组
        [SerializeField] RectTransform safeAreaTransform; // 安全区域的矩形变换，适配不同设备的边缘
        [SerializeField] RectTransform content; // 存放商品的内容区域
        [SerializeField] Button closeButton; // 关闭商店的按钮
        [SerializeField] CurrencyUIPanelSimple coinsUI; // 显示玩家金币的 UI 面板

        private TweenCase[] appearTweenCases; // 存储出现动画的 Tween 对象

        public static bool IsStoreAvailable { get; private set; } = false; // 存储商店是否可用的状态

        private List<IIAPStoreOffer> offers = new List<IIAPStoreOffer>(); // 存储商店中的商品

        private void Awake()
        {
            // 获取子对象中的所有 IIAPStoreOffer 接口实现
            content.GetComponentsInChildren(offers);//这种方式将会填充 offers 列表，而不是返回新的数组或列表。这种方法不会返回值，而是将找到的组件添加到传入的 List 中。
            closeButton.onClick.AddListener(OnCloseButtonClicked); // 绑定关闭按钮的点击事件
        }

        public override void Initialise()
        {
            // 订阅购买模块初始化的事件
            IAPManager.SubscribeOnPurchaseModuleInitted(InitOffers);

            // 注册安全区域矩形变换
            NotchSaveArea.RegisterRectTransform(safeAreaTransform);

            // 初始化金币 UI
            coinsUI.Initialise();
        }

        private void InitOffers()
        {
            // 初始化每个商品
            foreach (var offer in offers)
            {
                offer.Init();
            }

            IsStoreAvailable = true; // 设置商店为可用

            // 在下一帧计算内容区域的高度
            Tween.NextFrame(() => {
                float height = layout.padding.top + layout.padding.bottom; // 计算总高度，包括上边距和下边距

                for (int i = 0; i < offers.Count; i++)
                {
                    var offer = offers[i];
                    if (offer.GameObject.activeSelf) // 仅计算可见商品的高度
                    {
                        height += offer.Height; // 增加商品高度
                        if (i != offers.Count - 1) height += layout.spacing; // 在商品间增加间距
                    }
                }

                // 设置内容区域的高度
                content.sizeDelta = content.sizeDelta.SetY(height + 200);
            });
        }

        public override void PlayHideAnimation()
        {
            // 当商店隐藏时，通知页面关闭
            UIController.OnPageClosed(this);
        }

        public override void PlayShowAnimation()
        {
            appearTweenCases.KillActive(); // 终止任何正在进行的动画

            appearTweenCases = new TweenCase[offers.Count];
            for (int i = 0; i < offers.Count; i++)
            {
                Transform offerTransform = offers[i].GameObject.transform;
                offerTransform.transform.localScale = Vector3.zero; // 初始化缩放为零
                appearTweenCases[i] = offerTransform.transform.DOScale(1.0f, 0.3f, i * 0.05f).SetEasing(Ease.Type.CircOut); // 使用 Tween 动画放大商品
            }

            // 关闭按钮的缩放动画
            closeButton.transform.localScale = Vector3.zero;
            closeButton.transform.DOScale(1.0f, 0.3f, 0.2f).SetEasing(Ease.Type.BackOut);

            content.anchoredPosition = Vector2.zero; // 重置内容区域位置

            // 动画完成后，通知页面已打开
            appearTweenCases[^1].OnComplete(() =>
            {
                UIController.OnPageOpened(this);
            });
        }

        public void Hide()
        {
            appearTweenCases.KillActive(); // 终止任何正在进行的动画

            // 隐藏 IAP Store 页面
            UIController.HidePage<UIIAPStore>();
        }

        private void OnCloseButtonClicked()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound); // 播放按钮点击音效

            // 隐藏 IAP Store 页面
            UIController.HidePage<UIIAPStore>();
        }

        public void SpawnCurrencyCloud(RectTransform spawnRectTransform, CurrencyType currencyType, int amount, SimpleCallback completeCallback = null)
        {
            // 生成货币云特效
            FloatingCloud.SpawnCurrency(currencyType.ToString(), spawnRectTransform, coinsUI.RectTransform, amount, null, completeCallback);
        }
    }
}

// -----------------
// IAP Store v1.2
// -----------------

// 更新日志
// v 1.2
// ? 添加了移动设备的凹口偏移支持
// ? 添加了免费计时金币奖励
// ? 添加了广告金币奖励
// v 1.1
// ? 添加了商品接口
// ? 商品预制件重命名
// v 1.0
// ? 基本逻辑
