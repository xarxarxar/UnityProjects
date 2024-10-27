using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class LivesManager : MonoBehaviour
    {
        private static LivesManager instance; // 单例模式，用于全局访问该管理器

        [SerializeField] LivesData data; // 生命数据的配置类，用于存储最大生命数和生命恢复时长等信息

        private static LivesSave save; // 保存玩家生命数据的类，包括当前生命数、无限生命状态等

        // 单次恢复时间间隔
        public static int OneLifeInterval;
        // 使用属性管理生命数，当设置生命数时会调用SetLifes方法
        public static int Lives { get => save.livesCount; private set => SetLifes(value); }
        private static DateTime LivesDate { get => save.date; set => save.date = value; } // 记录生命恢复的起始时间

        private static Coroutine livesCoroutine; // 生命恢复的协程

        private static List<LivesIndicator> indicators = new List<LivesIndicator>(); // 所有用于显示生命的指示器列表
        private static List<AddLivesPanel> addLivesPanels = new List<AddLivesPanel>(); // 所有添加生命的面板列表

        // 判断当前生命数是否达到最大生命值
        public static bool IsMaxLives => Lives == instance.data.customedMaxLivesCount;

        private void Awake()
        {
            // 在Awake方法中可以进行一些初始化工作（当前未实现）
        }

        private void Start()
        {
            instance = this; // 初始化单例
            OneLifeInterval = instance.data.oneLifeRestorationDuration;//获取单次恢复时间间隔
            save = SaveController.GetSaveObject<LivesSave>("Lives"); // 获取并初始化保存的生命数据
            save.Init(data); // 使用生命数据初始化保存对象

            // 初始化生命显示
            SetLifes(Lives);

            if (save.infiniteLives)
            {
                // 如果玩家有无限生命，启动无限生命的倒计时
                Tween.InvokeCoroutine(InfiniteLivesCoroutine());
            }
            else if (Lives < data.customedMaxLivesCount)
            {
                // 如果生命值小于最大生命值，启动恢复生命的倒计时
                livesCoroutine = Tween.InvokeCoroutine(LivesCoroutine());
            }
        }

        /// <summary>
        /// 添加一个生命补充面板到列表中。
        /// </summary>
        public static void AddPanel(AddLivesPanel panel)
        {
            if (!addLivesPanels.Contains(panel)) addLivesPanels.Add(panel); // 如果列表中没有该面板，添加到列表

            if (instance == null)
            {
                Tween.NextFrame(() => {
                    panel.SetLivesCount(Lives); // 在下一帧设置面板显示的生命数量
                });
            }
            else
            {
                panel.SetLivesCount(Lives); // 立即设置面板显示的生命数量
            }
        }

        /// <summary>
        /// 从列表中移除生命补充面板。
        /// </summary>
        public static void RemovePanel(AddLivesPanel panel)
        {
            addLivesPanels.Remove(panel); // 移除指定面板
        }

        /// <summary>
        /// 添加一个生命指示器到列表中。
        /// </summary>
        public static void AddIndicator(LivesIndicator indicator)
        {
            if (!indicators.Contains(indicator)) indicators.Add(indicator); // 如果列表中没有该指示器，添加到列表

            if (instance == null)
            {
                Tween.NextFrame(() => {
                    indicator.Init(instance.data); // 初始化指示器

                    indicator.SetLivesCount(Lives); // 设置当前生命数
                    indicator.SetInfinite(save.infiniteLives); // 设置是否为无限生命
                });
            }
            else
            {
                indicator.Init(instance.data); // 初始化指示器

                indicator.SetLivesCount(Lives); // 设置当前生命数
                indicator.SetInfinite(save.infiniteLives); // 设置是否为无限生命
            }
        }

        /// <summary>
        /// 从列表中移除生命指示器。
        /// </summary>
        public static void RemoveIndicator(LivesIndicator indicator)
        {
            indicators.Remove(indicator); // 移除指定指示器
        }

        /// <summary>
        /// 设置当前生命数量，并通知所有指示器和面板更新显示。
        /// </summary>
        private static void SetLifes(int value)
        {
            save.livesCount = value; // 更新保存的生命数

            // 通知所有指示器更新生命数和无限生命状态
            foreach (var indicator in indicators)
            {
                indicator.SetLivesCount(Lives);
                indicator.SetInfinite(save.infiniteLives);
            }

            // 通知所有面板更新生命数
            foreach (var panel in addLivesPanels)
            {
                panel.SetLivesCount(value);
            }
        }

        /// <summary>
        /// 减少一条生命。如果有无限生命或者不可减少生命，则不会减少。
        /// </summary>
        public static void RemoveLife()
        {
            if (save.infiniteLives || !DoNotSpendLivesMenu.CanLivesBeSpent()) return; // 如果是无限生命或者不允许减少生命，直接返回

            Lives--; // 生命数减一

            if (Lives < 0)
                Lives = 0; // 生命不能小于0

            if (livesCoroutine == null)
            {
                LivesDate = DateTime.Now; // 记录减少生命的时间
                livesCoroutine = Tween.InvokeCoroutine(instance.LivesCoroutine()); // 启动恢复生命的协程
            }
        }

        /// <summary>
        /// 增加一条生命，如果没有达到最大生命数。
        /// </summary>
        public static void AddLife()
        {
            if (Lives < instance.data.customedMaxLivesCount)
                Lives++; // 生命数加一
        }

        /// <summary>
        /// 无限生命的协程，直到无限生命时间结束。
        /// </summary>
        private IEnumerator InfiniteLivesCoroutine()
        {
            var wait = new WaitForSeconds(0.25f); // 每0.25秒检查一次
            while (DateTime.Now < save.date) // 无限生命还未结束
            {
                var span = save.date - DateTime.Now; // 计算剩余时间

                // 通知所有指示器更新剩余时间
                foreach (var indicator in indicators)
                {
                    indicator.SetDuration(span);
                }

                yield return wait; // 等待
            }

            save.infiniteLives = false; // 无限生命结束

            SetLifes(data.customedMaxLivesCount); // 设置生命数为最大值

            // 更新所有指示器和面板的满生命状态
            foreach (var indicator in indicators)
            {
                indicator.FullText();
            }

            foreach (var panel in addLivesPanels)
            {
                panel.SetTime(data.fullText);
            }

            yield return LivesCoroutine(); // 继续启动生命恢复协程
        }

        /// <summary>
        /// 恢复生命的协程，每隔一定时间恢复一条生命。
        /// </summary>
        private IEnumerator LivesCoroutine()
        {
            var wait = new WaitForSeconds(0.25f); // 每0.25秒检查一次
            while (Lives < data.customedMaxLivesCount) // 当生命数未达到最大值
            {
                var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration);
                var timespan = DateTime.Now - LivesDate; // 计算上次生命减少到现在的时间

                if (timespan >= TimeSpan.FromSeconds(OneLifeInterval))// 恢复一条生命需要的时间
                {
                    Lives++; // 增加一条生命

                    LivesDate = DateTime.Now; // 更新恢复时间
                }

                // 通知所有指示器和面板更新倒计时
                foreach (var indicator in indicators)
                {
                    indicator.SetDuration(oneLifeSpan - timespan);
                }

                foreach (var panel in addLivesPanels)
                {
                    panel.SetTime(string.Format(data.timespanFormat, oneLifeSpan - timespan));
                }

                yield return wait; // 等待0.25秒
            }

            // 恢复到满生命时，通知所有指示器和面板更新满生命状态
            foreach (var indicator in indicators)
            {
                indicator.FullText();
            }

            foreach (var panel in addLivesPanels)
            {
                panel.SetTime(data.fullText);
            }

            livesCoroutine = null; // 停止协程
        }

        /// <summary>
        /// 永久减少单次恢复爱心的时间间隔，20241024添加
        /// </summary>
        /// <param name="seconds"></param>
        public static  void RemoveOneLifeTime(int seconds)
        {
            OneLifeInterval -= seconds;
            instance.data.oneLifeRestorationDuration-=seconds;
        }

        /// <summary>
        /// 永久增加一条最大生命值，20241024添加
        /// </summary>
        public static  void AddMaxLife()
        {
            instance.data.customedMaxLivesCount++;
        }

        /// <summary>
        /// 获取当前的时间恢复间隔，20241026添加
        /// </summary>
        public static int CurrentLiveInterval()
        {
            return instance.data.oneLifeRestorationDuration;
        }

        /// <summary>
        /// 获取当前的最大生命值，20241026添加
        /// </summary>
        public static int CurrentMaxLiveCount()
        {
            return instance.data.customedMaxLivesCount;
        }

        /// <summary>
        /// 启动无限生命，持续指定的秒数。
        /// </summary>
        public static void StartInfiniteLives(float duration)
        {
            instance.InfiniteLives(duration);
        }

        /// <summary>
        /// 内部方法，启动无限生命，并设置结束时间。
        /// </summary>
        private void InfiniteLives(float duration)
        {
            save.infiniteLives = true; // 设置为无限生命
            save.date = DateTime.Now + TimeSpan.FromSeconds(duration); // 设置无限生命的结束时间

            SetLifes(data.customedMaxLivesCount); // 设置生命数为最大值

            if (livesCoroutine != null)
            {
                Tween.StopCustomCoroutine(livesCoroutine); // 停止之前的生命恢复协程
                livesCoroutine = null;
            }
            Tween.InvokeCoroutine(InfiniteLivesCoroutine()); // 启动无限生命协程
        }

        // 保存类，用于序列化保存生命信息
        private class LivesSave : ISaveObject
        {
            public int livesCount; // 记录当前的生命数
            public bool infiniteLives; // 是否处于无限生命状态

            public long dateBinary; // 二进制形式保存的时间戳
            public DateTime date; // 恢复时间

            [SerializeField] bool firstTime = true; // 是否是第一次进入游戏

            public void Init(LivesData data)
            {
                if (firstTime)
                {
                    firstTime = false;

                    livesCount = data.maxLivesCount; // 初始化为最大生命数
                    data.customedMaxLivesCount = data.maxLivesCount;//初始化默认的最大生命值
                    data.oneLifeRestorationDuration = data.defaultLifeRestorationDuration;//初始化默认的体力恢复间隔
                    date = DateTime.Now; // 记录当前时间
                }
                else
                {
                    date = DateTime.FromBinary(dateBinary); // 从二进制恢复时间

                    if (infiniteLives)
                    {
                        livesCount = data.customedMaxLivesCount; // 如果是无限生命状态，设置生命数为最大值

                        if (DateTime.Now >= date) infiniteLives = false; // 检查无限生命是否已过期
                    }

                    if (livesCount < data.customedMaxLivesCount)
                    {
                        var timeDif = DateTime.Now - date; // 计算恢复时间差

                        var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration); // 每条生命恢复需要的时间

                        // 持续恢复生命，直到恢复满或者没有足够的时间差
                        while (timeDif >= oneLifeSpan && livesCount < data.customedMaxLivesCount)
                        {
                            timeDif -= oneLifeSpan;
                            date += oneLifeSpan;

                            livesCount++;
                        }
                    }
                }
            }

            public void Flush()
            {
                dateBinary = date.ToBinary(); // 将当前时间保存为二进制
            }
        }



        #region Development

        // 开发调试按钮，手动添加一条生命
        [Button("Add Life")]
        private void AddLifeDev()
        {
            AddLife();
        }

        // 开发调试按钮，手动减少一条生命
        [Button("Remove Life")]
        private void RemoveLifeDev()
        {
            RemoveLife();
        }

        #endregion
    }
}
