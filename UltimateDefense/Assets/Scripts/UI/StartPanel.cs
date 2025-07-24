using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class StartPanel : MonoBehaviour
{
    #region 私有变量
    [SerializeField] private Button _startBattleButton;  //开始挑战按钮
    [SerializeField] private Button _onlineRewardButton; //在线奖励按钮
    [SerializeField] private Button _signInButton;       //签到按钮
    [SerializeField] private Button _dailyMissionButton; //每日任务按钮
    [SerializeField] private Button _achievementButton;  //成就按钮
    [SerializeField] private Button _settingButton;      //设置按钮
    [SerializeField] private Button _rankButton;         //排行榜按钮
    [SerializeField] private Button _shareButton;        //邀请有礼按钮

    [SerializeField] private BasePanel _onlineRewardPanel;
    [SerializeField] private BasePanel _signInPanel;
    [SerializeField] private BasePanel _dailyMissionPanel;
    [SerializeField] private BasePanel _achievementPanel;
    [SerializeField] private BasePanel _settingPanel;
    [SerializeField] private BasePanel _rankPanel;
    [SerializeField] private BasePanel _sharePanel;
    private Dictionary<Button, BasePanel> _buttonPanelMap;


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
        _startBattleButton.onClick.AddListener(StartBattleButton);

        _buttonPanelMap = new Dictionary<Button, BasePanel>
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
            pair.Key.onClick.AddListener(() =>
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
        Debug.Log("点击开始挑战按钮");
        OnSatrtBattle?.Invoke();//开始挑战按钮被点击
        GameUIManager.Instance.HideMainMenu();//隐藏主菜单
        GameUIManager.Instance.ShowChooseDebuffPanel();
        gameObject.SetActive(false);
    }

    
    #endregion
}
