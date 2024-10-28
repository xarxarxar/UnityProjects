using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;
using static Watermelon.Currency;
using System;

namespace Watermelon.IAPStore
{
    public class ShuffleWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int price = 1000; // 该物品售价

        [Space]
        [SerializeField] GameObject currencyImage; // 金币图标
        [SerializeField] GameObject shareImage; // 分享图标

        [Space]
        [SerializeField] TMP_Text priceText; // 显示售价的文本组件

        [Space]
        [SerializeField] int timerDurationInMinutes; // 计时器的持续时间（分钟）

        [Space]
        [SerializeField] string description = "进行一次洗牌"; // 显示描述的文本
        [SerializeField] TMP_Text descriptionText; // 显示描述的文本组件

        [Space]
        [SerializeField] Button button; // 领取金币的按钮

        [Space]
        [SerializeField] GameObject imageObject; // 商品的图片

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        private RectTransform rect; // 当前组件的 RectTransform
        public float Height => rect.sizeDelta.y; // 获取组件的高度

        SimpleLongSave save; // 保存计时器状态的对象
        DateTime timerStartTime; // 计时器开始的时间
        private bool canShare = true;

        private void Awake()
        {
            // 获取当前组件的 RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {

            // 从保存控制器获取计时器状态
            save = SaveController.GetSaveObject<SimpleLongSave>("Shuffle With Share");
            // 从保存的二进制数据中恢复计时器开始的时间
            timerStartTime = DateTime.FromBinary(save.Value);

            // 为按钮添加点击事件监听器`
            button.onClick.AddListener(OnAdButtonClicked);

            priceText.text = price.ToString();

            descriptionText.text = description.ToString();
        }

        // 按钮点击事件处理
        private void OnAdButtonClicked()
        {
            AddPowerUp();
        }

        private void Update()
        {
            // 计算当前时间与计时器开始时间的差值
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // 如果计时结束
            {
                canShare = true;
                priceText.text = "分享"; // 显示文本
                shareImage.SetActive(true);
                currencyImage.SetActive(false);
            }
            else // 如果计时未结束
            {
                canShare = false;
                priceText.text = price.ToString(); // 显示文本
                shareImage.SetActive(false);
                currencyImage.SetActive(true);

                //var timeLeft = duration - timer; // 计算剩余时间
            }
        }

        //示例
        private void AddPowerUp()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            if (canShare)
            {
                CallWechat.ShareApp(() =>
                {
                    // 更新保存的计时器开始时间
                    save.Value = DateTime.Now.ToBinary();
                    timerStartTime = DateTime.Now;
                    AudioController.PlaySound(AudioController.Sounds.buySuccess);
                    Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                    {
                        PUController.AddPowerUp(PUType.Shuffle, 1);//添加一个“提示”道具的事例
                    });
                });
            }
            else
            {
                //货币足够的情况下才能购买
                if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
                {
                    // 减少玩家的金币数量
                    CurrenciesController.Substract(CurrencyType.Coins, price);
                    AudioController.PlaySound(AudioController.Sounds.buySuccess);
                    Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                    {
                        PUController.AddPowerUp(PUType.Shuffle, 1);//添加一个“提示”道具的事例
                    });
                }
                else
                {
                    Tools.BlinkRedThreeTimes(priceText, 0.5f);//文字闪烁提示货币不足
                }
            }


            //LivesManager.AddMaxLife();//添加一条最大生命值
        }
    }
}
