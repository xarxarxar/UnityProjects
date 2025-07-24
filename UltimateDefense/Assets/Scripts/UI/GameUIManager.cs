using DG.Tweening;
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
                                                          
    [SerializeField] private BasePanel _onlineRewardPanel;//在线奖励界面
    [SerializeField] private BasePanel _signInPanel;      //签到界面
    [SerializeField] private BasePanel _dailyMissionPanel;//每日任务界面
    [SerializeField] private BasePanel _achievementPanel; //成就界面
    [SerializeField] private BasePanel _settingPanel;     //设置界面
    [SerializeField] private BasePanel _rankPanel;        //排行榜界面
    [SerializeField] private BasePanel _sharePanel;       //邀请有礼界面


    [SerializeField] private GetRewardPanel _getRewardPanel;//获得奖励界面
    [SerializeField] private QuickTipPanel _quickTipPanel;//快速提示

    //流光相关
    public RectTransform canvasTransform; // UI Canvas
    public GameObject flyLightPrefab; // 一个流光粒子图标预制体（UI Image）



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
    /// 显示获取奖励面板
    /// </summary>
    public void ShowGetRewardPanel(params (RewardType type, int count)[] rewards)
    {
        _getRewardPanel.gameObject.SetActive(true);
        _getRewardPanel.SetReward(rewards);
    }

    /// <summary>
    /// 进行快速提示
    /// </summary>
    /// <param name="tip">提示</param>
    public void ShowQuickTip(string tip)
    {
        _quickTipPanel.gameObject.SetActive(true);
        _quickTipPanel.ShowQuickTip(tip);
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
    /// 显示面板，该面板是非全屏的面板
    /// </summary>
    /// <param name="basePanel"></param>
    public void ShowPanel(BasePanel basePanel)
    {

    }

    /// <summary>
    /// 播放流光特效
    /// </summary>
    /// <param name="screenStartPos"></param>
    /// <param name="screenEndPos"></param>
    public void PlayFlyEffect(Vector3 screenStart, Vector3 screenEnd, int count = 3)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject flyCoin = Instantiate(flyLightPrefab, canvasTransform);
            flyCoin.transform.position = screenStart;

            // 计算中间控制点（弧线弯曲点）
            Vector3 midPoint = (screenStart + screenEnd) / 2f;

            // 添加随机偏移，使每个轨迹略有不同
            float horizontalOffset = Random.Range(-100f, 100f); // 左右
            float verticalOffset = Random.Range(100f, 200f);    // 向上更高一点

            midPoint += new Vector3(horizontalOffset, verticalOffset, 0f);

            // 设置路径
            Vector3[] path = new Vector3[] { screenStart, midPoint, screenEnd };

            // 使用 DoTween 路径飞行
            flyCoin.transform
                .DOPath(path, 0.6f, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    Destroy(flyCoin);
                    // 可触发粒子、音效等
                });
        }
    }

    #endregion

    #region 私有方法
    private void Update()
    {

    }
    #endregion
}
