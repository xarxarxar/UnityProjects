using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class WechatManager : MonoBehaviour
{
    /// <summary>
    /// фад╩©М╤х
    /// </summary>
    public static double ScreenWidth { get => WX.GetWindowInfo().screenWidth;}
    /// <summary>
    /// фад╩╦ъ╤х
    /// </summary>
    public static double ScreenHeight {  get => WX.GetWindowInfo().screenHeight;}
    
    public static double WindowWidth {  get => WX.GetWindowInfo().windowWidth;}

    public static double WindowHeight {  get => WX.GetWindowInfo().windowHeight;}

    public static double DPR { get=>WX.GetWindowInfo().pixelRatio;}
}
