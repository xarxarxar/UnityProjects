using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理Manager类的注册
/// </summary>
public class ManagerRegistry
{
    private static List<IManager> managers = new List<IManager>();//所有的Manager类

    /// <summary>
    /// 注册Manager类
    /// </summary>
    /// <param name="manager"></param>
    public static void Register(IManager manager)
    {
        if (manager == null)
        {
            Debug.LogError("尝试注册空的 IManager！");
            return;
        }

        if (!managers.Contains(manager))
            managers.Add(manager);
        else
            Debug.LogWarning($"{manager.GetType().Name} 已注册过，重复注册被忽略。");
    }

    /// <summary>
    /// 初始化所有对应阶段的Manager
    /// </summary>
    /// <param name="initStage">Manager的阶段</param>
    public static void InitManagers(InitStage initStage)
    {
        foreach (var m in managers)
        {
            if(m.Stage == initStage)
            {
                m.Init();
            }
        }
    }

    public static void InitWithConfig(ManagerInitConfig config)
    {
        foreach (var group in config.initOrder)
        {
            foreach (var behaviour in group.managers)
            {
                if (behaviour is IManager manager && managers.Contains(manager))
                {
                    manager.Init();
                }
                else
                {
                    Debug.LogWarning($"{behaviour.name} 未注册或不实现 IManager。");
                }
            }
        }
    }
}


public class ManagerBase : MonoBehaviour
{
    public int Index;
    [field: SerializeField]
    public virtual string Description { get; }= "默认Manager";
    [SerializeField] protected InitStage _stage;
    public InitStage Stage { get => _stage; }//Manager类必须InitStage变量，用来表示该Manager类是用于什么时候
}


/// <summary>
/// 管理类基类，包含了只读属性Intance
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManagerBase<T> : ManagerBase, IManager where T : MonoBehaviour,IManager
{
    private static T _instance;
    protected bool _isRegistry=false;
    
    public static T Instance => _instance;

    

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        if (!_isRegistry)
        {
            ManagerRegistry.Register(this);
            _isRegistry = true;// 防止重复注册
        }
    }
    //Manager类必须有Init函数
    public virtual void Init()
    {

    }
}

public interface IManager
{
    InitStage Stage { get;}//Manager类必须InitStage变量，用来表示该Manager类是用于什么时候
    void Init();
}


public enum InitStage
{
    OutBattle, //局外
    InBattle, //局内
    // 可拓展更多：LoginStart, SceneLoaded, etc.
}
