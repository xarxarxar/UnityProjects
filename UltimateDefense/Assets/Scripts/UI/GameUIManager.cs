using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class GameUIManager : ManagerBase<GameUIManager>
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
    public GameObject flyCoinPrefab; // 一个飞翔的计比图标预制体（UI Image）
    



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
        AudioManager.Instance.PlayBGM("主界面BGM");
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
    /// 打开设置面板
    /// </summary>
    public void ShowSettingPanel()
    {
        _settingPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏设置面板
    /// </summary>
    public void HideSettingPanel()
    {
        _settingPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 播放流光特效
    /// </summary>
    /// <param name="screenStartPos"></param>
    /// <param name="screenEndPos"></param>
    // 修改 PlayFlyEffect 返回一个 Tween 或者 Coroutine 可等待
    public IEnumerator PlayFlyEffectAsync(Vector3 screenStart, Vector3 screenEnd,
        int count = 3, float duration = 0.6f)
    {
        
        int finishedCount = 0;

        for (int i = 0; i < count; i++)
        {
            GameObject flyCoin = Instantiate(flyLightPrefab, canvasTransform);
            flyCoin.transform.position = screenStart;

            Vector3 midPoint = (screenStart + screenEnd) / 2f;
            float horizontalOffset = Random.Range(-100f, 100f);
            float verticalOffset = Random.Range(100f, 200f);
            midPoint += new Vector3(horizontalOffset, verticalOffset, 0f);

            Vector3[] path = new Vector3[] { screenStart, midPoint, screenEnd };

            flyCoin.transform
                .DOPath(path, duration, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    Destroy(flyCoin);
                    finishedCount++;
                });
        }
        // 等待所有特效完成
        while (finishedCount < count)
        {
            yield return null;
        }
    }


    /// <summary>
    /// 播放流光特效（金币先散开再飞向目标，依次飞过去）
    /// </summary>
    public void PlayFlyCoinEffect(Vector3 screenStart, Vector3 screenEnd, int count = 1,
        UnityAction signleCallback=null,UnityAction totalCallback=null)
    {
        int finishedCount = 0; // 记录已完成的金币数量
        for (int i = 0; i < count; i++)
        {
            GameObject flyCoin = Instantiate(flyCoinPrefab, canvasTransform);

            // 初始生成位置：在 screenStart 附近随机偏移
            Vector3 startOffset = Vector3.zero;
            if (count > 1)
            {
                startOffset = new Vector3(
                    Random.Range(-50f, 50f),   // 左右散开范围
                    Random.Range(-50f, 50f),   // 上下散开范围
                    0f
                );
            }

            flyCoin.transform.position = screenStart + startOffset;

            // 散开后的目标点（离初始点再远一点，让它有向外扩的效果）
            Vector3 spreadTarget = flyCoin.transform.position + new Vector3(
                Random.Range(-80f, 80f),
                Random.Range(50f, 100f),
                0f
            );

            // 计算中间控制点（飞向 screenEnd 的弧线弯曲点）
            Vector3 midPoint = (spreadTarget + screenEnd) / 2f;
            midPoint += new Vector3(Random.Range(-100f, 100f), Random.Range(100f, 200f), 0f);

            Vector3[] path = new Vector3[] { spreadTarget, midPoint, screenEnd };

            // 动画序列
            Sequence seq = DOTween.Sequence();

            // 1. 先散开（0.2 秒）
            seq.Append(flyCoin.transform.DOMove(spreadTarget, 0.2f / BattleManager.Instance.GameSpeed.Value).SetEase(Ease.OutQuad));

            // 2. 停顿 0.2 秒
            seq.AppendInterval(0.2f/BattleManager.Instance.GameSpeed.Value);

            // 3. 再飞向终点
            seq.Append(flyCoin.transform
                .DOPath(path, 0.6f / BattleManager.Instance.GameSpeed.Value, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
            );

            // 4. 完成时销毁
            seq.OnComplete(() =>
            {
                Destroy(flyCoin);
                signleCallback?.Invoke();
                finishedCount++;
                if (finishedCount >= count) // 所有金币完成
                {
                    totalCallback?.Invoke();
                }
                // 触发音效或粒子
            });

            // 让每个金币依次延迟 0.02 秒开始
            seq.SetDelay(i * 0.05f / BattleManager.Instance.GameSpeed.Value);
        }
    }

    #endregion

}
