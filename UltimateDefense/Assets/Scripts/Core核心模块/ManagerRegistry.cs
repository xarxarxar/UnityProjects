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
}

/// <summary>
/// 管理类基类，包含了只读属性Intance
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ManagerBase<T> : MonoBehaviour, IManager where T : MonoBehaviour,IManager
{
    private static T _instance;
    protected bool _isRegistry=false;
    [SerializeField]protected InitStage _stage;
    public static T Instance => _instance;

    public InitStage Stage { get=> _stage; }//Manager类必须InitStage变量，用来表示该Manager类是用于什么时候

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (!_isRegistry)
        {
            ManagerRegistry.Register(this);
            _isRegistry = true;// 防止重复注册
        }
    }

    public abstract void Init();//Manager类必须有Init函数
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
