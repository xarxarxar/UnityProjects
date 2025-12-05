using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class GameUIManager : ManagerBase<GameUIManager>
{
    [SerializeField]private GameObject _startPanel;//游戏主界面
    [SerializeField]private GameObject _guidePanel;//导览界面
    [SerializeField]private GameObject _chooseDebuffPanel;//通关之后选择debuff的面板
    [SerializeField]private CollectionTextLight _collectionPrefab;//收集物品的prefab
                                                          
    [SerializeField] private BasePanel _settingPanel;     //设置界面

    [SerializeField] private GetRewardPanel _getRewardPanel;//获得奖励界面

    //流光相关
    public RectTransform canvasTransform; // UI Canvas
    private ObjectPool<Image> flyingLightPool;//流光粒子对象池
    public Image flyLightPrefab; // 一个流光粒子图标预制体（UI Image）
    private ObjectPool<Image> flyingCoinPool;//金币图标对象池
    private ObjectPool<CollectionTextLight> collectionPrefabPool;//收集物品的对象池
    public Image flyCoinPrefab; // 一个飞翔的金币图标预制体（UI Image）

    //排行榜相关
    public RawImage RankBody;
    public Image imageLeftTop;
    public Image imageRightBottom;
    public GameObject RankObject;

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
        if (flyingLightPool == null)
        {
            flyingLightPool = new ObjectPool<Image>(flyLightPrefab, 5, canvasTransform);
        }

        if (flyingCoinPool == null)
        {
            flyingCoinPool = new ObjectPool<Image>(flyCoinPrefab,10, canvasTransform);
        }
        if (collectionPrefabPool == null)
        {
            collectionPrefabPool = new ObjectPool<CollectionTextLight>(_collectionPrefab, 5, canvasTransform);
        }
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
    /// 过关之后选择debuff的面板
    /// </summary>
    public void ShowChooseDebuffPanel()
    {
        Debug.Log("显示抽取debuff界面");
        _chooseDebuffPanel.SetActive(true);
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    public void ShowSettingPanel()
    {
        _settingPanel.gameObject.SetActive(true);
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
            Image flyCoin = flyingLightPool.Get();
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
                    flyingLightPool.Return(flyCoin);
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
            Image flyCoin = flyingCoinPool.Get();

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
            seq.Append(flyCoin.transform.DOMove(spreadTarget, 0.2f).SetEase(Ease.OutQuad));

            // 2. 停顿 0.2 秒
            seq.AppendInterval(0.2f);

            // 3. 再飞向终点
            seq.Append(flyCoin.transform
                .DOPath(path, 0.6f, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
            );

            // 4. 完成时销毁
            seq.OnComplete(() =>
            {
                // 解绑订阅
                BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;

                flyingCoinPool.Return(flyCoin);
                signleCallback?.Invoke();
                finishedCount++;
                if (finishedCount >= count) // 所有金币完成
                {
                    totalCallback?.Invoke();
                }
                // 触发音效或粒子
            });

            // 让每个金币依次延迟 0.05 秒开始
            seq.SetDelay(i * 0.05f);

            // 初始化时同步 GameSpeed
            seq.timeScale = BattleManager.Instance.GameSpeed.Value;

            // 临时订阅方法
            void OnSpeedChanged(int speed)
            {
                if (seq != null && seq.IsActive())
                    seq.timeScale = speed;
            }
            BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;
        }
    }

    //播放获取收集物品的动画
    public void PlayCollectionAnim(int index, Vector3 screenStart)
    {
        // 1. 取字符
        if (index < 0 || index >= CollectionManager.Chars.Length)
        {
            Debug.LogError($"PlayCollectionAnim index 超出范围: {index}");
            return;
        }

        char c = CollectionManager.Chars[index];

        // 2. 取颜色
        Color32 color = CollectionManager.Instance.GetColorByIndex(index);

        // 3. 从 Prefab 生成收集物品
        CollectionTextLight item = collectionPrefabPool.Get();
        item.transform.position = screenStart;

        // 设置字符与颜色（你自己的 CollectionText 中需要对应字段）
        item.SetCollectionText(c, color);

        // 4. 终点位置 → 屏幕上方中央
        Vector3 screenTopCenter = new Vector3(
            Screen.width * 0.5f,
            Screen.height * 0.9f,
            0f
        );

        // 5. DOTween 动画
        Sequence seq = DOTween.Sequence();

        // 停留 1 秒
        seq.AppendInterval(2f);

        // 飞到顶部中心（0.6 秒）
        seq.Append(item.transform.DOMove(screenTopCenter, 1.0f).SetEase(Ease.OutCubic));

        // 6. 动画完成后销毁物体
        seq.OnComplete(() =>
        {
            Destroy(item.gameObject);
        });

        // 7. 同步 GameSpeed（可选）
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        void OnSpeedChanged(int speed)
        {
            if (seq != null && seq.IsActive())
                seq.timeScale = speed;
        }

        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        // 4. 完成时销毁
        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
            collectionPrefabPool.Return(item);
        });

    }

    /// <summary>
    /// 好友排行榜按钮
    /// </summary>
    public void RankButton()
    {
        UploadScore(DataManager.Instance.PlayerInfo.TotalPassCount.Value);
        //RankObject.SetActive(true);
        ShowScore();
    }

    /// <summary>
    /// 上传分数
    /// </summary>
    /// <param name="score"></param>
    public static void UploadScore(int score)
    {
        MyOpendataMessage message = new MyOpendataMessage();
        message.type = "setUserRecord";
        message.score = score;
        string msg = JsonUtility.ToJson(message);
        WX.GetOpenDataContext().PostMessage(msg);
    }

    /// <summary>
    /// 获取微信端的分数数据，并显示
    /// </summary>
    public void ShowScore()
    {
        var p = RankBody.transform.position;
        float h = (float)Screen.height * 1080 / Screen.width;
        float delta = Screen.height - h;
        float y = (h - (p.y - delta) /*- (buttonPosition.rect.height / 2)*/);
        RectTransform rt = RankBody.GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;
        WX.ShowOpenData(RankBody.texture,
            (int)imageLeftTop.transform.position.x,
            Screen.height - (int)imageLeftTop.transform.position.y,
        (int)width, (int)height); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点
        MyOpendataMessage msgData = new MyOpendataMessage();
        msgData.type = "showFriendsRank";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
    }

    //获取排行榜显示区域的宽
    int GetWidth()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);

        Debug.Log($"宽度为{(int)horizontalDistance}");
        return (int)horizontalDistance;
    }
    //获取排行榜显示区域的高
    int GetHeight()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);

        Debug.Log($"长度为{(int)verticalDistance}");
        return (int)verticalDistance;
    }
    #endregion

}
