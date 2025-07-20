using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制整个的游戏流程，游戏的入口
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//单例
    /// <summary>
    /// GameManger单例
    /// </summary>
    public static GameManager Instance { get => _instance;}


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 游戏启动
    /// </summary>
    private void Start()
    {
        //GameUIManager.Instance.ShowEnterPanel();//显示进入游戏面板
        Init();
    }

    #region 公共方法
    /// <summary>
    /// 初始化游戏
    /// </summary>
    public void Init()
    {
        ManagerRegistry.InitManagers(InitStage.OutBattle);//初始化所有局外的Manager

        StartCoroutine(TrackOnlineTime());//开始在线时长的统计
    }

    /// <summary>
    /// 加载新场景
    /// </summary>
    public void LoadNewSceneAsync(string sceneName)
    {
        // 启动协程，异步加载场景
        StartCoroutine(LoadNewSceneAsyncIE(sceneName));  // 把场景名字改成你要加载的
    }
    #endregion

    #region 私有方法
    //同步玩家信息
    private void SyncPlayerInfo()
    {

    }

    private IEnumerator LoadNewSceneAsyncIE(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // 禁止自动跳转，等我们自己处理完
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // asyncOperation.progress 范围是 0 ~ 0.9
            float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // 更新UI
            

            // 如果加载到 90%，可以让玩家点击“继续”按钮激活场景
            if (asyncOperation.progress >= 0.9f)
            {
                // 这里可以显示“点击继续”按钮，等玩家确认后再执行
                // 比如你可以加一个bool变量： if (playerClickContinue) { asyncOperation.allowSceneActivation = true; }

                // 这里演示自动进入场景
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    //计算在线时长
    private IEnumerator TrackOnlineTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 等待60秒
            DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value += 1;
            Debug.Log("在线时间 +1 分钟，总在线分钟：" + DataManager.Instance.PlayerInfo.TodayOnlineMinutes);
        }
    }
    #endregion


}
