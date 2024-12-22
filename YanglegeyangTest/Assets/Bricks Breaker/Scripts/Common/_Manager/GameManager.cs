using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.SocialPlatforms.GameCenter;
#endif


public partial class GameManager : Singleton<GameManager>
{

    public void Initialized()
    {

        SetSystem();
    }

    void SetSystem()
    {
        //Target frame settings
        if (Application.targetFrameRate != 60)
        {
            Application.targetFrameRate = 60;
        }

        //Multi-touch setting
        Input.multiTouchEnabled = false;

        //Local push initialization
        this.gameObject.AddComponent<Localpush>();

        //Canceling sleep mode
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //Language code settings
        string res = Data.GetSystemLanguageLetter();
        res.ToLower();
        GameData.CountryCode = res;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);


#if UNITY_IOS
        //Apple GameCenter Login
        //LogIN();
#endif
    }

    /// <summary>
    /// Apple GameCenter Login
    /// </summary>
    public void LogIN()
    {
        if (Social.localUser.authenticated == true)
        {
            Debug.Log("Success to true");
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Success to authenticate");
                }
                else
                {
                    PlayManager.Instance.commonUI.SetToast("Failed to authenticate");
                }
            });
        }
    }
}

