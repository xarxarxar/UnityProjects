using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UI
/// </summary>
public class StartPanel : MonoBehaviour
{
    #region 私有变量
    [SerializeField] private BindableButton _startBattleButton;  //开始挑战按钮
    [SerializeField] private BindableButton _onlineRewardButton; //在线奖励按钮
    [SerializeField] private BindableButton _signInButton;       //签到按钮
    [SerializeField] private BindableButton _dailyMissionButton; //每日任务按钮
    [SerializeField] private BindableButton _achievementButton;  //成就按钮
    [SerializeField] private BindableButton _settingButton;      //设置按钮
    [SerializeField] private BindableButton _rankButton;         //排行榜按钮
    [SerializeField] private BindableButton _shareButton;        //邀请有礼按钮

    [SerializeField] private BasePanel _onlineRewardPanel;
    [SerializeField] private BasePanel _signInPanel;
    [SerializeField] private BasePanel _dailyMissionPanel;
    [SerializeField] private BasePanel _achievementPanel;
    [SerializeField] private BasePanel _settingPanel;
    [SerializeField] private BasePanel _rankPanel;
    [SerializeField] private BasePanel _sharePanel;
    private Dictionary<BindableButton, BasePanel> _buttonPanelMap;


    #endregion

    #region 公共变量
    /// <summary>
    /// 开始挑战按钮被点击
    /// </summary>
    public static event UnityAction OnSatrtBattle;
    #endregion

    #region 私有方法
    private void Start()
    {
        _startBattleButton.AddListener(StartBattleButton);

        _buttonPanelMap = new Dictionary<BindableButton, BasePanel>
    {
        { _onlineRewardButton, _onlineRewardPanel },
        { _signInButton, _signInPanel },
        { _dailyMissionButton, _dailyMissionPanel },
        { _achievementButton, _achievementPanel },
        { _settingButton, _settingPanel },
        { _rankButton, _rankPanel },
        { _shareButton, _sharePanel }
    };

        foreach (var pair in _buttonPanelMap)
        {
            pair.Key.AddListener(() =>
            {
                pair.Value.gameObject.SetActive(true);
            });
        }
    }

    /// <summary>
    /// 开始挑战按钮的点击方法
    /// </summary>
    private void StartBattleButton()
    {
        OnSatrtBattle?.Invoke();//开始挑战按钮被点击
        GameUIManager.Instance.HideMainMenu();//隐藏主菜单
        GameUIManager.Instance.ShowChooseDebuffPanel();
        AudioManager.Instance.PlayBGM("对局中BGM");
        gameObject.SetActive(false);
    }

    
    #endregion
}
