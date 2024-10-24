using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Watermelon.Currency;

namespace Watermelon.IAPStore
{
    // 实现 IIAPStoreOffer 接口的 FreeMoneyTimerOffer 类
    public class FreeMoneyTimerOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int coinsAmount; // 玩家可以领取的金币数量
        [SerializeField] TMP_Text coinsAmountText; // 显示金币数量的文本组件

        [Space]
        [SerializeField] TMP_Text timerText; // 显示计时器的文本组件
        [SerializeField] int timerDurationInMinutes; // 计时器的持续时间（分钟）

        [Space]
        [SerializeField] Button button; // 领取金币的按钮

        [Space]
        [SerializeField] RectTransform cloudSpawnRectTransform; // 云飘动效果的生成位置
        [SerializeField] int floatingElementsAmount = 10; // 云中漂浮元素的数量

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        private RectTransform rect; // 当前组件的 RectTransform
        public float Height => rect.sizeDelta.y; // 获取组件的高度

        SimpleLongSave save; // 保存计时器状态的对象
        DateTime timerStartTime; // 计时器开始的时间

        private void Awake()
        {
            // 获取当前组件的 RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
            // 从保存控制器获取计时器状态
            save = SaveController.GetSaveObject<SimpleLongSave>("Free Money Timer");

            // 从保存的二进制数据中恢复计时器开始的时间
            timerStartTime = DateTime.FromBinary(save.Value);

            // 为按钮添加点击事件监听器
            button.onClick.AddListener(OnAdButtonClicked);
            // 显示可领取的金币数量
            coinsAmountText.text = $"x{coinsAmount}";
        }

        private void Update()
        {
            // 计算当前时间与计时器开始时间的差值
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // 如果计时结束
            {
                button.enabled = true; // 启用按钮
                timerText.text = "免费!"; // 显示"免费获取"文本
            }
            else // 如果计时未结束
            {
                button.enabled = false; // 禁用按钮

                var timeLeft = duration - timer; // 计算剩余时间

                // 格式化剩余时间并更新显示
                if (timeLeft.Hours > 0)
                {
                    timerText.text = string.Format("{0:hh\\:mm\\:ss}", timeLeft);
                }
                else
                {
                    timerText.text = string.Format("{0:mm\\:ss}", timeLeft);
                }

                // 动态调整计时文本和按钮的宽度
                var prefferedWidth = timerText.preferredWidth;
                if (prefferedWidth < 270) prefferedWidth = 270;

                timerText.rectTransform.sizeDelta = timerText.rectTransform.sizeDelta.SetX(prefferedWidth + 5);
                button.image.rectTransform.sizeDelta = button.image.rectTransform.sizeDelta.SetX(prefferedWidth + 10);
            }
        }

        // 按钮点击事件处理
        private void OnAdButtonClicked()
        {
            // 更新保存的计时器开始时间
            save.Value = DateTime.Now.ToBinary();
            timerStartTime = DateTime.Now;

            // 获取 IAP Store 页面并生成货币云特效动画
            UIIAPStore iapStore = UIController.GetPage<UIIAPStore>();
            iapStore.SpawnCurrencyCloud(cloudSpawnRectTransform, CurrencyType.Coins, floatingElementsAmount, () =>
            {
                // 增加玩家的金币数量
                CurrenciesController.Add(CurrencyType.Coins, coinsAmount);
            });
        }
    }
}
