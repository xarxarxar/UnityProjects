using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;
using Unity.VisualScripting;
using static Watermelon.Currency;
using System;


namespace Watermelon.IAPStore
{
    public class ReduceLiveIntervaWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] LivesData data; // 生命数据的配置类，用于存储最大生命数和生命恢复时长等信息
        [SerializeField] int price = 1000; // 该物品售价
        [Space]
        [SerializeField] TMP_Text timerText; // 显示售价的文本组件
        [SerializeField] int timerDurationInMinutes; // 计时器的持续时间（分钟）

        [Space]
        [SerializeField] int timeReduce=10; // 减少的时间值，以秒为单位

        [Space]
        [SerializeField] string description = "永久减少恢复时间值"; // 显示描述的文本
        [SerializeField] TMP_Text descriptionText; // 显示描述的文本组件

        [Space]
        [SerializeField] Button button; // 购买的按钮

        [Space]
        [SerializeField] GameObject imageObject; // 商品的图片
        [SerializeField] GameObject targetObject; // 目标图片
        [SerializeField] GameObject AdsIcon; // 广告图标
        bool canWatchAds = true;

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        DateTime timerStartTime; // 计时器开始的时间
        SimpleLongSave save; // 保存计时器状态的对象

        private RectTransform rect; // 当前组件的 RectTransform
        public float Height => rect.sizeDelta.y; // 获取组件的高度


        private void Awake()
        {
            // 获取当前组件的 RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
            // 从保存控制器获取计时器状态
            save = SaveController.GetSaveObject<SimpleLongSave>("Reduce Live Interval Timer");

            // 从保存的二进制数据中恢复计时器开始的时间
            timerStartTime = DateTime.FromBinary(save.Value);

            // 为按钮添加点击事件监听器`
            button.onClick.AddListener(OnAdButtonClicked);

            timerText.text = "获得";
            description = $"永久减少{timeReduce}秒爱心恢复时间";
            descriptionText.text = description.ToString();

            //如果当前的时间间隔已经小于5分钟了，则购买按钮不可点击
            if (data.oneLifeRestorationDuration <= 300)
            {
                button.interactable = false;
                timerText.text = "最大";
            }
        }

        private void Update()
        {
            // 计算当前时间与计时器开始时间的差值
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // 如果计时结束
            {
                canWatchAds = true;
                button.enabled = true; // 启用按钮
                button.interactable = true;
                AdsIcon.SetActive(true);
                timerText.gameObject.SetActive(false);
            }
            else // 如果计时未结束
            {
                canWatchAds = false;
                AdsIcon.SetActive(false);
                timerText.gameObject.SetActive(true);
                button.enabled = false; // 禁用按钮
                button.interactable = false;
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


        private void OnAdButtonClicked()
        {
            PlayAds();
            //如果当前的时间间隔已经小于5分钟了，则购买按钮不可点击
            if (data.oneLifeRestorationDuration <= 300)
            {
                button.interactable = false;
                timerText.text = "最大";
            }
        }

        //示例
        private void ReduceLiveInterval()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            //货币足够的情况下才能购买
            if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
            {
                // 减少玩家的金币数量
                CurrenciesController.Substract(CurrencyType.Coins, price);
                LivesManager.RemoveOneLifeTime(timeReduce);//减少时间恢复间隔
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>{});
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            }
            else
            {
                Tools.BlinkRedThreeTimes(timerText, 0.5f);//文字闪烁提示货币不足
            }


            //LivesManager.AddMaxLife();//添加一条最大生命值
        }

        private void PlayAds()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            WXAdsManager.Instance.ShowAd((isEnd) =>
            {
                if (isEnd)
                {
                    // 更新保存的计时器开始时间
                    save.Value = DateTime.Now.ToBinary();
                    timerStartTime = DateTime.Now;
                    LivesManager.RemoveOneLifeTime(timeReduce);//减少时间恢复间隔
                    Tools.MoveAndShrinkUI(imageObject, targetObject, 1.0f, () => { });
                    AudioController.PlaySound(AudioController.Sounds.buySuccess);
                }
                else
                {
                    FloatingMessage.ShowMessage("广告未观看完毕");
                }
            });
        }
    }
}
