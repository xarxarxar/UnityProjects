using UnityEngine;
using UnityEngine.Events;

public class DataManager : ManagerBase<DataManager>,IManager
{
    [SerializeField] private PlayerInfo _playerInfo;//全局的玩家信息

    public static event UnityAction<int> OnPassCountChanged;//通关次数变化
    public static event UnityAction OnDataLoaded;//数据加载完毕
    /// <summary>
    /// 玩家全局信息
    /// </summary>
    public PlayerInfo PlayerInfo { get => _playerInfo;}


    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.OutBattle;//局外Manager
    }

    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("DataManager init");
#if UNITY_EDITOR
        _playerInfo=new PlayerInfo();
        //_playerInfo.PassCount = 5;
        _playerInfo.UnlockCount = 1;
        _playerInfo.CoinCount = 100;
        _playerInfo.PassCount = 10;
        OnDataLoaded?.Invoke();
#endif
        BattleManager.OnEndBattle += OnEndBattle;
    }

    #endregion

    #region 私有方法
    //结束挑战
    private void OnEndBattle(bool success)
    {
        if(success)
        {
            _playerInfo.PassCount++;
            OnPassCountChanged?.Invoke(_playerInfo.PassCount);
        }
    }
    #endregion
}
