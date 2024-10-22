using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Watermelon.IAPStore
{
    // CurrencyOffer 类继承自 IAPStoreOffer，用于处理虚拟货币的购买
    public class CurrencyOffer : IAPStoreOffer
    {
        [SerializeField] int coinsAmount; // 虚拟货币的数量
        [SerializeField] TMP_Text currencyAmountText; // 显示货币数量的文本组件

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // 云特效生成的位置
        [SerializeField] int floatingElementsAmount = 10; // 漂浮元素的数量

        // 在 Awake 方法中初始化货币数量的显示
        protected override void Awake()
        {
            base.Awake();
            currencyAmountText.text = $"x{coinsAmount}"; // 设置货币数量文本
        }

        // 当购买成功时调用此方法
        protected override void ApplyOffer()
        {
            UIIAPStore iapStore = UIController.GetPage<UIIAPStore>(); // 获取 IAP Store 界面
            // 生成货币云效果，并在效果完成后添加货币
            iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
            {
                CurrenciesController.Add(CurrencyType.Coins, coinsAmount); // 添加虚拟货币到用户账户
            });
        }

        // 重新应用已购买的商品时调用此方法
        protected override void ReapplyOffer()
        {
            // 由于虚拟货币是可消费商品，通常在用户首次购买后不需要重新应用此方法
            // 因此此方法为空，除非有其他逻辑需要在此处处理
        }
    }
}
