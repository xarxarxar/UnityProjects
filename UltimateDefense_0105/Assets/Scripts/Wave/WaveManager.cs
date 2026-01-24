using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理怪物波次生成流程
/// </summary>
public class WaveManager : ManagerBase<WaveManager>
{
    #region 私有属性
    private int _currentRound=0;// 当前回合数（1–100）
    [SerializeField]private int _maxRound = 50;//最大回合数
    [SerializeField]private float _spawnWaveInterval=5.0f;//每波敌人生成的间隔时间
    #endregion

    #region 公共静态事件
    /// <summary>
    /// 波次更新事件
    /// </summary>
    public static event UnityAction<int> OnWaveChanged;
    #endregion

    #region 公开属性

    /// <summary>
    /// 只读属性，暴露当前回合编号
    /// </summary>
    public int CurrentRound { get { return _currentRound; } 
        set 
        { 
            if(_currentRound != value)
            {
                _currentRound = value;
                OnWaveChanged?.Invoke(value);
            }
        } 
    }

    /// <summary>
    /// 只读属性，每波敌人生成的间隔时间
    /// </summary>
    public float SpawnWaveInterval { get => _spawnWaveInterval; }
    /// <summary>
    /// 最大回合数
    /// </summary>
    public int MaxRound { get => _maxRound;}
    /// <summary>
    /// 每波生成敌人的个数
    /// </summary>
    public int WaveEnemyCount => OneWaveEnemyCount();
    /// <summary>
    /// 单个敌人生成的间隔
    /// </summary>
    public float SignleEnemyInterval => OneEnemyInterval();
    #endregion


    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("wavemanager 初始化");
        _currentRound = 0;
    }


    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 初始化 _keyWaves 与 _poolConfig 引用
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        Index = 2;
    }

    private int OneWaveEnemyCount()
    {
        //刚开始最少，越到后面越多
        int count =Mathf.RoundToInt(2 + _currentRound*1.5f);
        if (_currentRound % 5 == 0)
        {
            count +=10;
        }
        if(_currentRound>= _maxRound)
        {
            count += 10;
        }
        return count;
    }

    private float OneEnemyInterval()
    {
        //刚开始间隔大，越到后面越小
        float interval = 5.0f - (_currentRound*0.2f);
        interval = Mathf.Max(interval,2.0f);
        return interval;
    }

    #endregion
}
