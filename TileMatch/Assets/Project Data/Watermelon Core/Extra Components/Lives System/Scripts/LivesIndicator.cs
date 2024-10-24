using System;
using TMPro; // 用于处理TextMeshPro文本
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // 用于处理UI按钮和图像

namespace Watermelon
{
    public class LivesIndicator : MonoBehaviour
    {
        [Space] // 在Inspector中分隔字段
        [SerializeField] TextMeshProUGUI livesCountText; // 显示生命数量的TextMeshPro文本组件
        [SerializeField] Image infinityImage; // 显示无限生命时的图像
        [SerializeField] TextMeshProUGUI durationText; // 显示倒计时时间或满生命状态的文本组件

        [Space]
        [SerializeField] Button addButton; // 添加生命的按钮
        [SerializeField] AddLivesPanel addLivesPanel; // 控制显示补充生命面板的组件

        // 用于存储当前的生命数据
        private LivesData Data { get; set; }

        // 标志是否已初始化，防止重复初始化
        private bool isInitialised;

        /// <summary>
        /// 初始化生命指示器，传入LivesData数据。
        /// 如果已经初始化过，则不再重复初始化。
        /// </summary>
        public void Init(LivesData data)
        {
            if (isInitialised) return; // 如果已经初始化，则直接返回

            Data = data;

            // 如果addLivesPanel存在，显示添加按钮并设置点击事件
            if (addLivesPanel != null)
            {
                addButton.gameObject.SetActive(true); // 激活按钮
                addButton.onClick.AddListener(() => addLivesPanel.Show()); // 设置点击时显示添加生命面板
            }
            else
            {
                addButton.gameObject.SetActive(false); // 否则隐藏按钮
            }

            isInitialised = true; // 标记为已初始化
        }

        /// <summary>
        /// 设置无限生命的显示状态。
        /// 当无限生命启用时，显示无限生命图标，隐藏生命数量文本。
        /// </summary>
        public void SetInfinite(bool isInfinite)
        {
            infinityImage.gameObject.SetActive(isInfinite); // 显示或隐藏无限生命图标
            livesCountText.gameObject.SetActive(!isInfinite); // 相应地隐藏或显示生命数量文本
        }

        /// <summary>
        /// 设置当前生命数量并更新UI。
        /// 如果生命数达到最大值，调整按钮和显示文本。
        /// </summary>
        public void SetLivesCount(int count)
        {
            if (!isInitialised) return; // 如果尚未初始化，直接返回

            livesCountText.text = count.ToString(); // 更新生命数量文本

            // 如果生命数小于最大值，且addLivesPanel存在，则显示添加按钮
            addButton.gameObject.SetActive(count != Data.maxLivesCount && addLivesPanel != null);

            // 如果生命数达到最大值，显示“满生命”状态
            if (count == Data.maxLivesCount)
            {
                FullText(); // 调用FullText方法设置显示文本
            }
        }

        /// <summary>
        /// 设置倒计时时间，并根据倒计时时长格式化文本。
        /// </summary>
        public void SetDuration(TimeSpan duration)
        {
            if (!isInitialised) return; // 如果尚未初始化，直接返回

            // 根据时长选择不同的时间格式进行显示
            if (duration >= TimeSpan.FromHours(1))
            {
                durationText.text = string.Format(Data.longTimespanFormat, duration);
            }
            else
            {
                durationText.text = string.Format(Data.timespanFormat, duration);
            }

            // 调整文本框大小，避免按钮遮挡
            SetTextSize(!addButton.gameObject.activeSelf);
        }

        /// <summary>
        /// 显示满生命时的文本信息。
        /// </summary>
        public void FullText()
        {
            if (!isInitialised) return; // 如果尚未初始化，直接返回

            durationText.text = Data.fullText; // 设置为“满生命”文本

            SetTextSize(true); // 调整文本框大小为全屏状态
        }

        /// <summary>
        /// 根据是否处于全屏状态调整文本框的大小。
        /// </summary>
        private void SetTextSize(bool fullPanel)
        {
            if (fullPanel)
            {
                durationText.rectTransform.offsetMin = new Vector2(70, 0); // 调整左边距
                durationText.rectTransform.offsetMax = new Vector2(-38, 0); // 调整右边距
            }
            else
            {
                durationText.rectTransform.offsetMin = new Vector2(95, 0);
                durationText.rectTransform.offsetMax = new Vector2(-100, 0);
            }
        }

        /// <summary>
        /// 当该组件启用时，注册到LivesManager中。
        /// </summary>
        private void OnEnable()
        {
            LivesManager.AddIndicator(this); // 注册当前生命指示器
        }

        /// <summary>
        /// 当该组件禁用时，从LivesManager中移除。
        /// </summary>
        private void OnDisable()
        {
            LivesManager.RemoveIndicator(this); // 移除当前生命指示器
        }
    }
}
