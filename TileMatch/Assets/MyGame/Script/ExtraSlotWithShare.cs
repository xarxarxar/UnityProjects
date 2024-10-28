using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class ExtraSlotWithShare : MonoBehaviour, IIAPStoreOffer
    {
        [Space]
        [SerializeField] string description = "分享之后获取一个额外卡槽道具"; // 显示描述的文本
        [SerializeField] TMP_Text descriptionText; // 显示描述的文本组件

        [Space]
        [SerializeField] TMP_Text timerText; // 显示计时器的文本组件
        [SerializeField] int timerDurationInMinutes; // 计时器的持续时间（分钟）

        [Space]
        [SerializeField] Button button; // 领取金币的按钮

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
            save = SaveController.GetSaveObject<SimpleLongSave>("Shuffle With Share");

            // 从保存的二进制数据中恢复计时器开始的时间
            timerStartTime = DateTime.FromBinary(save.Value);

            // 为按钮添加点击事件监听器
            button.onClick.AddListener(OnAdButtonClicked);

            descriptionText.text = description.ToString();
        }

        private void Update()
        {
            // 计算当前时间与计时器开始时间的差值
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // 如果计时结束
            {
                button.enabled = true; // 启用按钮
                button.interactable = true;
                timerText.text = "分享"; // 显示文本
            }
            else // 如果计时未结束
            {
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

        // 按钮点击事件处理
        private void OnAdButtonClicked()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            CallWechat.ShareApp(() =>
            {
                // 更新保存的计时器开始时间
                save.Value = DateTime.Now.ToBinary();
                timerStartTime = DateTime.Now;
                PUController.AddPowerUp(PUType.ExtraSlot, 1);//添加一个“提示”道具的事例
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            });
        }
    }
}
