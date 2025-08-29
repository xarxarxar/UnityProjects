using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterPanel : MonoBehaviour
{
    public Button EnterButton;//进入游戏按钮

    private void Start()
    {
        AudioManager.Instance.PlayBGM("登陆界面BGM");
        EnterButton.onClick.AddListener(EnterGameButton);
        DataManager.OnDataLoaded+= OnDataLoaded;
    }

    private void OnDisable()
    {
        EnterButton.onClick.RemoveAllListeners();
        DataManager.OnDataLoaded -= OnDataLoaded;
    }

    /// <summary>
    /// 进入游戏按钮点击方法
    /// </summary>
    private void EnterGameButton()
    {
        Debug.Log("点击进入游戏按钮");
        DataManager.Instance.LoadPlayerInfo();
    }


    //异步加载新场景
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

    private void OnDataLoaded()
    {
        StartCoroutine(LoadNewSceneAsyncIE("GameScene"));  // 把场景名字改成你要加载的
        AudioManager.Instance.PlayBGM("主界面BGM");
    }
}
