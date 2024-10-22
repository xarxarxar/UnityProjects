#pragma warning disable 0649
#pragma warning disable 0414

using UnityEngine;

namespace Watermelon
{
    // 可序列化类，用于管理屏幕设置
    [System.Serializable]
    public class ScreenSettings
    {
        [Header("Frame Rate")] // 帧率设置标题
        [SerializeField] bool setFrameRateAutomatically = false; // 是否自动设置帧率

        [Space] // 空格分隔
        [SerializeField] AllowedFrameRates defaultFrameRate = AllowedFrameRates.Rate60; // 默认帧率
        [SerializeField] AllowedFrameRates batterySaveFrameRate = AllowedFrameRates.Rate30; // 低电量模式帧率

        [Header("Sleep")] // 睡眠设置标题
        [SerializeField] int sleepTimeout = -1; // 屏幕休眠超时，-1表示不更改

        // 初始化屏幕设置
        public void Initialise()
        {
            Screen.sleepTimeout = sleepTimeout; // 设置屏幕休眠超时

            // 检查是否自动设置帧率
            if (setFrameRateAutomatically)
            {
                uint numerator = Screen.currentResolution.refreshRateRatio.numerator; // 获取刷新率分子
                uint denominator = Screen.currentResolution.refreshRateRatio.denominator; // 获取刷新率分母

                // 计算目标帧率
                if (numerator != 0 && denominator != 0)
                {
                    Application.targetFrameRate = Mathf.RoundToInt(numerator / denominator); // 设置目标帧率
                }
                else
                {
                    Application.targetFrameRate = (int)defaultFrameRate; // 使用默认帧率
                }
            }
            else
            {
#if UNITY_IOS
                // 在iOS设备上，根据低电量模式设置帧率
                if (UnityEngine.iOS.Device.lowPowerModeEnabled)
                {
                    Application.targetFrameRate = (int)batterySaveFrameRate; // 设置为低电量模式帧率
                }
                else
                {
                    Application.targetFrameRate = (int)defaultFrameRate; // 设置为默认帧率
                }    
#else
                Application.targetFrameRate = (int)defaultFrameRate; // 在其他平台上设置为默认帧率
#endif
            }
        }

        // 定义允许的帧率
        private enum AllowedFrameRates
        {
            Rate30 = 30, // 30帧
            Rate60 = 60, // 60帧
            Rate90 = 90, // 90帧
            Rate120 = 120, // 120帧
        }
    }
}
