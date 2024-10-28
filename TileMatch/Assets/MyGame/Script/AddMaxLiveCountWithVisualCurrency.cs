using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using Watermelon.IAPStore;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class AddMaxLiveCountWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] LivesData data; // 生命数据的配置类，用于存储最大生命数和生命恢复时长等信息
        [SerializeField] int price = 1000; // 该物品售价
        [Space]
        [SerializeField] TMP_Text priceText; // 显示售价的文本组件

        [Space]
        [SerializeField] string description = "永久增加最大爱心值"; // 显示描述的文本
        [SerializeField] TMP_Text descriptionText; // 显示描述的文本组件

        [Space]
        [SerializeField] Button button; // 购买的按钮

        [Space]
        [SerializeField] GameObject imageObject; // 商品的图片

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        private RectTransform rect; // 当前组件的 RectTransform
        public float Height => rect.sizeDelta.y; // 获取组件的高度


        private void Awake()
        {
            // 获取当前组件的 RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
            // 为按钮添加点击事件监听器`
            button.onClick.AddListener(OnAdButtonClicked);

            priceText.text = price.ToString();
            description = $"永久增加1个最大爱心值";
            descriptionText.text = description.ToString();

            //如果当前的最大生命值已经大于99了，则购买按钮不可点击
            if (data.customedMaxLivesCount >= 99)
            {
                button.interactable = false;
                priceText.text = "最大";
            }
        }

        // 按钮点击事件处理
        private void OnAdButtonClicked()
        {
            ReduceLiveInterval();
            //如果当前的最大生命值已经大于99了，则购买按钮不可点击
            if (data.customedMaxLivesCount >= 99)
            {
                button.interactable = false;
                priceText.text = "最大";
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
                LivesManager.AddMaxLife();//添加一条最大生命值
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>{});
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            }
            else
            {
                Tools.BlinkRedThreeTimes(priceText, 0.5f);//文字闪烁提示货币不足
            }
            //LivesManager.AddMaxLife();//添加一条最大生命值
        }
    }
}
