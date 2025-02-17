using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI大小分辨率控制
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    /// <summary>
    /// 字母牌手牌宽度占屏幕的比例
    /// </summary>
    public static float letterHandPoolWidth = 0.7f * 1080;

    /// <summary>
    /// 字母牌占字母拍手牌宽度的比例
    /// </summary>
    public static float letterCardWidth = 0.2f * letterHandPoolWidth;
}
