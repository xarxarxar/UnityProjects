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
    
}
