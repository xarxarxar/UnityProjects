using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResolutionManager
{
    public static float ScreenWidth = Screen.width;//��Ļ�Ŀ�
    public static float ScreenHeight = Screen.height;//��Ļ�ĸ�
    public static float CardLengthStandard = Screen.width< Screen.height * 0.5625f ? Screen.width: Screen.height * 0.5625f;
    public static float CardLength = CardLengthStandard / 9;
}

