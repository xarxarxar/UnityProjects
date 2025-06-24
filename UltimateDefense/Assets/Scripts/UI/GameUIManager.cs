using UnityEngine;

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class GameUIManager : ManagerBase<GameUIManager>,IManager
{
    [SerializeField]private GameObject _enterPanel;//进入游戏Panel
    [SerializeField]private GameObject _mainPanel;//游戏主界面


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
        _mainPanel.SetActive(true);
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
    #endregion

    #region 私有方法

    #endregion
}
