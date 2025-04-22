using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using UnityEngine.AddressableAssets;
using System;


public class GameFailCanvas : MonoBehaviour
{
    public static GameFailCanvas instance;
    public Transform relifeButtonParent;
    public Transform getTipButtonParent;
    public  Button relifeButton;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public IEnumerator LoadLua()
    {
        yield return Addressables.InitializeAsync();

        // 加载资源
        var loadHandle = Addressables.LoadAssetAsync<GameObject>("Assets/LetterCardFiles/AssetPackages/Prefabs/UI/Buttons/复活按钮.prefab");
        var getTipButtonHandle = Addressables.LoadAssetAsync<GameObject>("Assets/LetterCardFiles/AssetPackages/Prefabs/UI/Buttons/分享获取金币Button.prefab");
        var loadLuaString = Addressables.LoadAssetAsync<TextAsset>("Assets/LetterCardFiles/Scripts/Luas/WatchAds.lua.txt");
        yield return loadHandle;
        yield return getTipButtonHandle;
        yield return loadLuaString;

        GameObject relifeButtonObject = loadHandle.Result;
        relifeButton = Instantiate(relifeButtonObject, relifeButtonParent).GetComponent<Button>();
        LuaManager.instance.luaString = loadLuaString.Result.text;

        // 获取 Lua 返回的 table（就是上面那个 M）
        LuaTable luaScript = LuaManager.instance.luaEnv.DoString(LuaManager.instance.luaString)[0] as LuaTable;

        //执行AdsInit
        luaScript.Get<Action<GameFailCanvas>>("InitAds")?.Invoke(this);

        // 传入 relifeButton 和 this（GameFailCanvas 实例）
        luaScript.Get<Action<Button, GameFailCanvas>>("BindRelifeButton")?.Invoke(relifeButton, this);


        GameObject getTipButtonObject = getTipButtonHandle.Result;
        GetRewards.Instance.getTipButton = Instantiate(getTipButtonObject, getTipButtonParent).GetComponent<Button>();
        // 传入 relifeButton 和 this（GameFailCanvas 实例）
        luaScript.Get<Action<Button, GetRewards>>("BindGetTipButton")?.Invoke(GetRewards.Instance.getTipButton, GetRewards.Instance);
    }


    /// <summary>
    /// 回到主页
    /// </summary>
    public void BackToMenu()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        DeckManager.instance.ClearHandCards();
        GameEntrance.instance.GetComponent<Canvas>().enabled = true;
        GameManager.Instance.GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 重新挑战
    /// </summary>
    public void RestartChallenge()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GameManager.Instance.StartChallenge();
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 继续挑战
    /// </summary>
    public void ContinueChallenge()
    {
        //GameManager.Instance.ContinueChallenge();
        //GameManager.Instance.DoubleScoreRoundCount = 5;
        
        Debug.Log("分享");
        AudioManager.instance.PlaySoundEffect("ClickButton");
        WechatManager.ShareApp(() =>
        {
            GameManager.Instance.ContinueChallenge();
            GameManager.Instance.DoubleScoreRoundCount = 5;
        });

    }

    public void InitAds(string gridUnitId, string bannerUnitId, string interstitialUnitId, string inspireUnitId)
    {
        WXAdsManager.Instance.gridUnitId = gridUnitId;
        WXAdsManager.Instance.bannerUnitId = gridUnitId;
        WXAdsManager.Instance.interstitialUnitId = interstitialUnitId;
        WXAdsManager.Instance.inspireUnitId = inspireUnitId;
        WXAdsManager.Instance.Init();
    }

    /// <summary>
    /// 通过看广告继续挑战
    /// </summary>
    public void ContinueChallengeByAd()
    {
        WXAdsManager.Instance.ShowAd((isOver) =>
        {
            if(isOver)
            {
                GameManager.Instance.ContinueChallenge();
                GameManager.Instance.DoubleScoreRoundCount = 5;
            }
            else
            {
                ShowTipManager.instance.ShowTip("广告未观看完毕");
            }
        });
    }

}
