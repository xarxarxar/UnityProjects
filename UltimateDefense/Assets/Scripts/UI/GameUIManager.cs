using UnityEngine;

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class GameUIManager : ManagerBase<GameUIManager>,IManager
{
    [SerializeField]private GameObject _enterPanel;//进入游戏Panel
    [SerializeField]private GameObject _startPanel;//游戏主界面
    [SerializeField]private GameObject _guidePanel;//导览界面
    [SerializeField]private GameObject _playerInfoPanel;//玩家信息面板
    [SerializeField]private GameObject _chooseDebuffPanel;//通关之后选择debuff的面板
    [SerializeField]private TipPanel _tipPanel;//显示tip的面板

    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.OutBattle;
    }
    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {

    }

    /// <summary>
    /// 显示主菜单
    /// </summary>
    public void ShowMainMenu()
    {
        _startPanel.SetActive(true);
        _guidePanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏主菜单
    /// </summary>
    public void HideMainMenu()
    {
        _startPanel.SetActive(false);
        _guidePanel.SetActive(false);
    }

    /// <summary>
    /// 过关之后选择debuff的面板
    /// </summary>
    public void ShowChooseDebuffPanel()
    {
        _chooseDebuffPanel.SetActive(true);
    }

    /// <summary>
    /// 显示进入面板
    /// </summary>
    public void ShowEnterPanel()
    {
        _enterPanel.SetActive(true);
    }

    /// <summary>
    /// 进入战斗，显示战斗面板
    /// </summary>
    public void ShowBattlePanel()
    {

    }

    /// <summary>
    /// 显示玩家信息面板，总的信息，局外信息
    /// </summary>
    public void ShowPlayerInfoPanel()
    {
        _playerInfoPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏玩家信息面板，总的信息，局外信息
    /// </summary>
    public void HidePlayerInfoPanel()
    {
        _playerInfoPanel.SetActive(false);
    }

    /// <summary>
    /// 显示提示面板
    /// </summary>
    /// <param name="tip"></param>
    public void ShowTipPanel(string tip)
    {
        _tipPanel.gameObject.SetActive(true);
        _tipPanel.ShowTip(tip);
    }
    
    #endregion

    #region 私有方法
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ShowPlayerInfoPanel();
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            HidePlayerInfoPanel();
        }
    }
    #endregion
}
