using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 发送数据给谷歌分析
/// </summary>
public class AdmobGA : MonoBehaviour
{

    public static AdmobGA load;     // instance of this class
    public GoogleAnalyticsV3 GA;    // instance of Google Analytics 

    void Awake()
    {
        if (load == null)
        {
            // Makes the object target not be destroyed automatically when loading a new scene
            DontDestroyOnLoad(gameObject); 
            load = this;
        }
        else if (load != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //// Start Session Google Analytics
        //GA.StartSession();
        //// Request banner Google Admob
        //GoogleMobileAdsScript.advertise.RequestBanner();
    }
}
