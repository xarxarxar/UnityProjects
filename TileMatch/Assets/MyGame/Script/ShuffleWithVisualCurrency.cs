using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class ShuffleWithVisualCurrency : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] int price = 1000; // 该物品售价

        [Space]
        [SerializeField] TMP_Text priceText; // 显示售价的文本组件

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

            descriptionText.text = description.ToString();
        }

        // 按钮点击事件处理
        private void OnAdButtonClicked()
        {
            AddPowerUp();
        }

        //示例
        private void AddPowerUp()
        {
            //货币足够的情况下才能购买
            if (CurrenciesController.HasAmount(CurrencyType.Coins, price))
            {
                // 减少玩家的金币数量
                CurrenciesController.Substract(CurrencyType.Coins, price);
                Tools.MoveAndShrinkUI(imageObject, 0.5f, () =>
                {
                    PUController.AddPowerUp(PUType.Shuffle, 1);//添加一个“提示”道具的事例
                });
            }
            else
            {
                Tools.BlinkRedThreeTimes(priceText, 1);//文字闪烁提示货币不足
            }


            //LivesManager.AddMaxLife();//添加一条最大生命值
        }
    }
}
