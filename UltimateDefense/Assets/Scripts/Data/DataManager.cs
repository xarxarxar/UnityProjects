

public class DataManager : ManagerBase<DataManager>,IManager
{
    private PlayerInfo _playerInfo;//全局的玩家信息

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
#if UNITY_EDITOR
        _playerInfo=new PlayerInfo();
#endif

    }
    #endregion
}
