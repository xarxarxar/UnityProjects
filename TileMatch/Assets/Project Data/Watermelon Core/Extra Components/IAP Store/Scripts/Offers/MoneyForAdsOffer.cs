using System.Collections;
using System.Collections.Generic;
using TMPro; // 引入 TextMeshPro 命名空间，用于文本显示
using UnityEngine;
using UnityEngine.UI; // 引入 UI 组件命名空间

namespace Watermelon.IAPStore
{
    // 实现 IIAPStoreOffer 接口，表示这是一个 IAP 商店商品
    public class MoneyForAdsOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int coinsAmount; // 广告观看后获得的金币数量
        [SerializeField] TMP_Text coinsAmountText; // 显示金币数量的文本组件

        [Space]
        [SerializeField] Button button; // 触发观看广告的按钮

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // 云特效生成的位置
        [SerializeField] int floatingElementsAmount = 10; // 云特效中漂浮元素的数量

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        private RectTransform rect; // 当前物体的 RectTransform
        public float Height => rect.sizeDelta.y; // 获取当前物体的高度

        // 在对象初始化时执行
        private void Awake()
        {
            rect = GetComponent<RectTransform>(); // 获取当前物体的 RectTransform 组件
        }

        // 初始化按钮和显示的金币数量
        public void Init()
        {
            button.onClick.AddListener(OnAdButtonClicked); // 为按钮添加点击事件监听器
            coinsAmountText.text = $"x{coinsAmount}"; // 设置金币数量的文本
        }

        // 点击按钮后调用该方法
        private void OnAdButtonClicked()
        {
            // 显示奖励视频广告
            AdsManager.ShowRewardBasedVideo((watched) => {
                if (watched) // 如果观看了广告
                {
                    // 获取 IAP Store 页面并生成货币云特效
                    UIIAPStore iapStore = UIController.GetPage<UIIAPStore>();
                    iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
                    {
                        // 增加玩家的金币数量
                        CurrenciesController.Add(CurrencyType.Coins, coinsAmount);
                    });
                }
            });
        }
    }
}
