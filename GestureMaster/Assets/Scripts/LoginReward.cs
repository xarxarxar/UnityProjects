using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginReward : MonoBehaviour
{
    public static LoginReward instance;
    public Text mDateText;//上方显示的时间
    public Button mGetButton;//领取按钮
    public Button mMakeUpButton;//补签按钮
    public GameObject mReachDate;//未到时间标签
    public GameObject mAlreadyHas;//已经领取的标签
    public GameObject mShowHand;//展示皮肤的物体
    public GameObject mNoSkinCanGet;//显示已无可领取的美甲

    public GameObject mLoginRewardPanel;//登录领取奖励面板
    public GameObject mGetLoginRewardPanel;//登录领取奖励面板，恭喜获取

    public Button mPreButton;
    public Button mNextButton;

    private string mCurrentId;
    [SerializeField] private int mCurrentIndex;//当前显示的index

    public List<LoginRewardInfo> loginRewardInfos = new List<LoginRewardInfo>();

    private GameInfo PlayerInfo=>GameEntrance.instance.PlayerInfo;
    private int WeekDay=> GameEntrance.WeekDay;

    private void Awake()
    {
        instance=this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        loginRewardInfos= PlayerInfo.loginRewardInfos;
        mGetButton.onClick.AddListener(GetSkinButton);
        mMakeUpButton.onClick.AddListener(ShareForLoginSkin);
    }

    //打开每日登陆奖励面板
    public void OpenLoginRewardPanel()
    {
        mLoginRewardPanel.SetActive(true);
        mCurrentIndex = WeekDay;
        SetRewardSkin(loginRewardInfos[mCurrentIndex]);
    }

    public void PageButton(int direction)//翻页的方向，-1代表往前，1代表往后
    {
        mCurrentIndex += direction;
        SetRewardSkin(loginRewardInfos[mCurrentIndex]);
        mNextButton.gameObject.SetActive(mCurrentIndex+1 < loginRewardInfos.Count);
        mPreButton.gameObject.SetActive(mCurrentIndex > 0);
    }

    public void SetRewardSkin(LoginRewardInfo skinInfo)
    {
        if (skinInfo.skinId == "")
        {
            mShowHand.gameObject.SetActive(false);
            mNoSkinCanGet.gameObject.SetActive(true);

            if (mCurrentIndex == 6)//周日
            {
                mNoSkinCanGet.GetComponent<Text>().text = "暂无可领取的美甲,请静候更新";
            }
            else
            {
                mNoSkinCanGet.GetComponent<Text>().text = "暂无可领取的贴花,请静候更新";
            }
        }
        else
        {
            mShowHand.gameObject.SetActive(true);
            mNoSkinCanGet.gameObject.SetActive(false);
            SkinManager.instance.ShowRenderHand(SkinManager.instance.GetSkinById(skinInfo.skinId));
        }

        mCurrentId= skinInfo.skinId;

        mDateText.text ="周"+ ConvertNumberToChinese(mCurrentIndex);
        //时间为今天，且没有拥有这个皮肤
        mGetButton.gameObject.SetActive(skinInfo.weekday== WeekDay
            && !SkinManager.instance.GetSkinById(skinInfo.skinId).isOwned
            && skinInfo.skinId != "");
        //时间已经过了而且没有这个皮肤
        mMakeUpButton.gameObject.SetActive(skinInfo.weekday < WeekDay && !SkinManager.instance.GetSkinById(skinInfo.skinId).isOwned);
        mReachDate.gameObject.SetActive(skinInfo.weekday > WeekDay);
        mAlreadyHas.gameObject.SetActive(SkinManager.instance.GetSkinById(skinInfo.skinId).isOwned);
    }

    //领取皮肤按钮
    private void GetSkinButton()
    {
        SkinManager.instance.GetSkin(SkinManager.instance.GetSkinById(mCurrentId));
        mGetLoginRewardPanel.SetActive(true);
        mGetButton.gameObject.SetActive(false);
        mAlreadyHas.gameObject .SetActive(true);
    }

    private void ShareForLoginSkin()
    {
        WechatManager.ShareApp(() =>
        {
            SkinManager.instance.GetSkin(SkinManager.instance.GetSkinById(mCurrentId));
            mAlreadyHas.gameObject.SetActive(true);
            mGetLoginRewardPanel.SetActive(true);
            mMakeUpButton.gameObject.SetActive(false);
        });
    }

    private string ConvertNumberToChinese(int number)
    {
        string[] chineseDigits = { "一", "二", "三", "四", "五", "六", "日"};
        if(number < 0 || number>= loginRewardInfos.Count)
        {
            return null;
        }
        return chineseDigits[number];
    }
}

[System.Serializable]
public class LoginRewardInfo
{
    public int weekday;//周几
    public string skinId;
}
