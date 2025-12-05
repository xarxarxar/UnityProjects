using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterPanel : MonoBehaviour
{
    [SerializeField] private Button EnterButton;//进入游戏按钮
    [SerializeField] private Slider _progressSlider;//进度条
    [SerializeField] private Text _progressSliderText;//slider的value文本
    [SerializeField] private Text _progressText;//进入游戏时，显示的文本

    [SerializeField] private LoadingProgressChannelSO _loadingReward;//进入游戏时的加载数据

    //剪影无限轮播
    public RectTransform imageA;
    public RectTransform imageB;
    public float speed = 100f; // 每秒移动多少像素
    private float imageWidth;

    private void Start()
    {
        // 获取图片宽度（假设两张图相同）
        imageWidth = imageA.rect.width;
        // 初始化位置（B 紧接在 A 的右边）
        imageA.anchoredPosition = Vector2.zero;
        imageB.anchoredPosition = new Vector2(imageWidth, 0);

        AudioManager.Instance.PlayBGM("登陆界面BGM");
        EnterButton.gameObject.SetActive(false);
        EnterButton.onClick.AddListener(EnterGameButton);
        DataManager.OnDataLoaded+= OnDataLoaded;
        _loadingReward.OnProgressChanged += OnProgressChanged;
        _loadingReward.OnVisibilityChanged += OnVisibilityChanged;
        _loadingReward.Raise(0,"");
    }

    void Update()
    {
        float move = speed * Time.deltaTime;

        // 左移两张图
        imageA.anchoredPosition -= new Vector2(move, 0);
        imageB.anchoredPosition -= new Vector2(move, 0);

        // 如果某张图完全移出屏幕左边，就把它移到右侧
        if (imageA.anchoredPosition.x <= -imageWidth)
            imageA.anchoredPosition += new Vector2(imageWidth * 2, 0);

        if (imageB.anchoredPosition.x <= -imageWidth)
            imageB.anchoredPosition += new Vector2(imageWidth * 2, 0);
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

    private void OnProgressChanged(float progress,string message)
    {
        _progressSlider.value= progress;
        _progressSliderText.text = $"{progress}%";
        _progressText.text = message;
    }

    private void OnVisibilityChanged(bool isShown)
    {
        _progressSlider.gameObject.SetActive(isShown);
        EnterButton.gameObject.SetActive(!isShown);
    }
}
