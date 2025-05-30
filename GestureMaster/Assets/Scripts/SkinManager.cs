using DG.Tweening;
using SuperScrollView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public  static SkinManager instance;
    
    public DecalDatabase decalDatabase_meijia;
    public DecalDatabase decalDatabase_tiehua;
    private DecalDatabase current_decalDatabase;

    public bool isEquipMeijia =>
    decalDatabase_meijia != null &&
    decalDatabase_meijia.decalList.Any(d => d.isEquip && !d.IsDefault);//当前游戏中是否装备了美甲
    public bool isEquipTiehua =>
    decalDatabase_tiehua != null &&
    decalDatabase_tiehua.decalList.Any(d => d.isEquip && !d.IsDefault);//当前游戏中是否装备了贴花
    public DecalData meijiaDecal=> decalDatabase_meijia.decalList.FirstOrDefault(d => d.isEquip);//美甲的DecalData
    public DecalData tiehuaDecal=> decalDatabase_tiehua.decalList.FirstOrDefault(d => d.isEquip);//贴花的DecalData


    public GameObject decalItemPrefab_Normal;//普通toggle的预制体

    public ToggleGroup ToggleGroup;
    public Text descriptionText;//描述该道具的Text
    public Text getMothText;//获取该道具的途径
    public Button buyButton;

    public Material targetMaterial_meijia; // 商城中用来显示美甲的材质
    public Material targetMaterial_tiehua; // 商城中用来显示贴花的材质
    public Material myhandMeijiaMaterial; // 我的手的真正游戏中的材质
    public Material myhandTiehuaMaterial; // 我的手的真正游戏中的材质
    public GameObject renderHand;//用来展示的手的模型
    public GameObject nailHand;//用来展示的指甲的模型
    public Camera renderCamera;//用来展示模型的摄像机
    public Text countText;//显示有多少皮肤和皮肤总数

    [Header("基础配置")]
    public LoopGridView mLoopGridView_meijia;
    public LoopGridView mLoopGridView_tiehua;
    public int totalCount => current_decalDatabase.decalList.Count;
    public string prefabName = "ItemPrefab";

    [Header("Item 尺寸设置")]
    public List<ToggleItemAndData> toggleItemAndDatas_meijia=new List<ToggleItemAndData>();
    public List<ToggleItemAndData> toggleItemAndDatas_tiehua = new List<ToggleItemAndData>();
    public List<ToggleItemAndData> toggleItemAndDatas_current = new List<ToggleItemAndData>();
    public float itemWidth = 250f;
    public float itemHeight = 350f;
    public float itemPaddingX = 20f;
    public float itemPaddingY = 20f;

    private bool isMeijiaLoaded=false;//美甲的super scroll view是否已经加载过
    private bool isTiehuaLoaded=false;//贴花的super scroll view是否已经加载过
    private int mColumCount;//scroll view 的列数

    public Toggle openMeijiaToggle;
    public Toggle openTiehuaToggle;
    public static ToggleItem equipItem;//用来记录哪个皮肤是正在装备的
    public static DecalData equipDecal;//用来记录哪个皮肤是正在装备的

    private void Awake()
    {
        instance=this;
    }

    private void Start()
    {
        foreach(DecalData decalData in decalDatabase_meijia.decalList)
        {
            toggleItemAndDatas_meijia.Add(new ToggleItemAndData(decalData, false));
        }
        foreach (DecalData decalData in decalDatabase_tiehua.decalList)
        {
            toggleItemAndDatas_tiehua.Add(new ToggleItemAndData(decalData, false));
        }

        openMeijiaToggle.onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
            {
                ShowSkin(decalDatabase_meijia);
            }
        });
        openTiehuaToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                ShowSkin(decalDatabase_tiehua);
            }
        });

        ToggleItem.OnToggleClick += ShowRenderHand;//显示贴画的效果 
        ToggleItem.OnEquipButtonClick += EquipButtonClick;//装备的皮肤变化

        GameManager.OnDateUpdated += SetNewWeekLoginRewardInfos;
    }

    /// <summary>
    /// 初始化游戏皮肤
    /// </summary>
    public void Init()
    {
        SyncDecalStatusFromDatabase();
        ShowRealHand(meijiaDecal);
        ShowRealHand(tiehuaDecal);
    }

    /// <summary>
    /// 游戏内获取皮肤的操作
    /// </summary>
    public void GetSkin(DecalData decalData)
    {
        decalData.isOwned = true;//拥有该款皮肤
        decalData.isNewest = true;
        if(decalData.type==SkinType.NailArt)//是美甲
        {
            SkinInfo result = GameManager.instance.gameInfo.meijiaDecals.Find(skin => skin.id == decalData.id);
            result.isOwned = true;
        }
        else
        {
            SkinInfo result = GameManager.instance.gameInfo.tiehuaDecals.Find(skin => skin.id == decalData.id);
            result.isOwned = true;
        }
        DataManager.instance.SyncGameInfo();//同步数据
    }


    //展示皮肤
    public void ShowSkin(DecalDatabase decalDatabase)
    {
        current_decalDatabase = decalDatabase;
        if (decalDatabase == decalDatabase_meijia)//加载美甲
        {
            InitGrid(mLoopGridView_meijia);
            toggleItemAndDatas_current = toggleItemAndDatas_meijia;
        }
        else
        {
            InitGrid(mLoopGridView_tiehua);
            toggleItemAndDatas_current = toggleItemAndDatas_tiehua;
        }
        

        // 获取已拥有的皮肤数量
        int ownedCount = decalDatabase.decalList.Count(decal => decal.isOwned);
        countText.text = $"全部：<color=#008DD4>{ownedCount}</color>/{decalDatabase.decalList.Count}";
    }

    /// <summary>
    /// 将本地的皮肤数据与云端的同步
    /// </summary>
    public void SyncDecalStatusFromDatabase()
    {
        if (decalDatabase_meijia == null) return;
        foreach (var matchingDecal in decalDatabase_meijia.decalList)
        {
            var skin = GameManager.instance.gameInfo.meijiaDecals.Find(d => d.id == matchingDecal.id);
            if (skin != null)
            {
                matchingDecal.isOwned = skin.isOwned;
                matchingDecal.isEquip = skin.isEquip;
            }
        }

        if (decalDatabase_tiehua == null) return;

        foreach (var matchingDecal in decalDatabase_tiehua.decalList)
        {
            var skin = GameManager.instance.gameInfo.tiehuaDecals.Find(d => d.id == matchingDecal.id);
            if (skin != null)
            {
                matchingDecal.isOwned = skin.isOwned;
                matchingDecal.isEquip = skin.isEquip;
            }
        }
    }

    //周一重新设置每日登录信息获取的情况
    private void SetNewWeekLoginRewardInfos(int weekday)
    {
        //周一了，并且上一次登录时间和今天不是同一天，则刷新一周的每日登录信息
        if(weekday == 0 && GameManager.instance.gameInfo.lastLoginDate.Date!=GameManager.instance.TodayDate.Date)
        {
            SetGameInfoLoginRewardInfos(GameManager.instance.gameInfo.loginRewardInfos);
        }
    }

    /// <summary>
    /// 将每日登录信息列表重新设置
    /// </summary>
    /// <param name="loginRewardInfos"></param>
    public void SetGameInfoLoginRewardInfos(List<LoginRewardInfo> loginRewardInfos)
    {
        //获取6个美甲
        List<DecalData> meijias = GetRandomSkins(isOwned: false, type: SkinType.NailArt, unlockMethod: UnlockMethod.DailyLogin, count: 6);
        //获取1个贴花
        List<DecalData> tiehua = GetRandomSkins(isOwned: false, type: SkinType.Tattoo, unlockMethod: UnlockMethod.DailyLogin, count: 1);
        // 添加美甲奖励（前6天）
        for (int i = 0; i < meijias.Count; i++)
        {
            LoginRewardInfo tmpLoginRewardInfo = new LoginRewardInfo();
            tmpLoginRewardInfo.skinId = meijias[i]?.id ?? ""; // 防止null
            tmpLoginRewardInfo.weekday = i;
            loginRewardInfos.Add(tmpLoginRewardInfo);
        }

        // 添加第7天的贴花奖励
        LoginRewardInfo lastLoginReward = new LoginRewardInfo();
        lastLoginReward.skinId = tiehua[0]?.id ?? ""; // 防止null
        lastLoginReward.weekday = 6;
        loginRewardInfos.Add(lastLoginReward);
    }

    /// <summary>
    /// 通关id得到皮肤的decaldata
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public DecalData GetSkinById(string id)
    {
        DecalData decalData= decalDatabase_meijia.decalList.FirstOrDefault(d => d.id == id);
        if(decalData == null)
        {
            decalData= decalDatabase_tiehua.decalList.FirstOrDefault(d => d.id == id);
        }
        return decalData;
    }

    /// <summary>
    /// 随机获取皮肤的decaldata
    /// </summary>
    /// <param name="skinType"></param>
    /// <returns></returns>
    public List<DecalData> GetRandomSkins(
    bool? isOwned = null,
    bool? isEquip = null,
    bool? isNewest = null,
    SkinType? type = null,
    UnlockMethod? unlockMethod = null,
    int count = 1
)
    {
        List<DecalData> sourceList = new List<DecalData>();

        // 收集数据源
        if (type == null)
        {
            if (decalDatabase_meijia?.decalList != null)
                sourceList.AddRange(decalDatabase_meijia.decalList);

            if (decalDatabase_tiehua?.decalList != null)
                sourceList.AddRange(decalDatabase_tiehua.decalList);
        }
        else if (type == SkinType.NailArt)
        {
            if (decalDatabase_meijia?.decalList != null)
                sourceList.AddRange(decalDatabase_meijia.decalList);
        }
        else
        {
            if (decalDatabase_tiehua?.decalList != null)
                sourceList.AddRange(decalDatabase_tiehua.decalList);
        }

        // 多条件过滤
        List<DecalData> filteredList = sourceList.FindAll(d =>
            (isOwned == null || d.isOwned == isOwned) &&
            (isEquip == null || d.isEquip == isEquip) &&
            (isNewest == null || d.isNewest == isNewest) &&
            (unlockMethod == null || d.unlockMethod == unlockMethod)
        );

        // 打乱顺序
        for (int i = filteredList.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (filteredList[i], filteredList[j]) = (filteredList[j], filteredList[i]);
        }

        // 构建固定长度的返回列表
        List<DecalData> result = new List<DecalData>();
        for (int i = 0; i < count; i++)
        {
            if (i < filteredList.Count)
            {
                result.Add(filteredList[i]);
            }
            else
            {
                result.Add(null); // 不足则补 null
            }
        }

        return result;
    }


    /// <summary>
    /// 在指定时间内同时移动、旋转和缩放目标物体，返回 Sequence。
    /// </summary>
    /// <param name="target">要操作的物体 Transform</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="targetRotation">目标旋转角度（欧拉角）</param>
    /// <param name="targetScale">目标缩放</param>
    /// <param name="duration">动画持续时间（默认 1 秒）</param>
    /// <returns>返回 DOTween 的 Sequence 对象</returns>
    Sequence MoveRotateScale(Transform target, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale, float duration = 0.75f)
    {
        Sequence sequence = DOTween.Sequence();

        // 使用本地坐标移动
        sequence.Join(target.DOLocalMove(targetPosition, duration));

        // 使用本地旋转
        sequence.Join(target.DOLocalRotate(targetRotation, duration));

        // 本地缩放（这个本来就是 localScale，不用改）
        sequence.Join(target.DOScale(targetScale, duration));

        return sequence;
    }

    /// <summary>
    /// 使用 DOTween 动画改变摄像机的 Orthographic Size。
    /// </summary>
    /// <param name="cam">目标摄像机</param>
    /// <param name="targetSize">目标大小</param>
    /// <param name="duration">持续时间</param>
    /// <returns>返回 Tween 对象</returns>
    public static Tween TweenCameraSize(Camera cam, float targetSize, float duration = 0.75f)
    {
        return DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, targetSize, duration);
    }

    /// <summary>
    /// 从 DecalDatabase 中随机获取一个未拥有的 DecalData
    /// </summary>
    /// <param name="decalDatabase">Decal 数据库对象</param>
    /// <returns>一个未拥有的 DecalData，如果没有则返回 null</returns>
    public DecalData GetRandomUnownedDecal(DecalDatabase decalDatabase)
    {
        // 获取所有未拥有的 Decal
        var unownedDecals = decalDatabase.decalList
            .Where(d => d != null && d.isOwned == false)
            .ToList();

        // 如果没有未拥有的贴花，返回 null
        if (unownedDecals.Count == 0)
            return null;

        // 随机选取一个
        int index =UnityEngine. Random.Range(0, unownedDecals.Count);
        return unownedDecals[index];
    }

    /// <summary>
    /// 展示皮肤效果
    /// </summary>
    /// <param name="category">0为美甲，1为贴画</param>
    /// <param name="decalData">材质贴图</param>
    public void ShowRenderHand(DecalData decalData)
    {
        if (decalData.IsDefault)
        {
            descriptionText.text = "默认";
        }
        else
        {
            descriptionText.text = decalData.description;
        }
        
        if (decalData.type == SkinType.NailArt)//美甲
        {
            nailHand.SetActive(true);
            renderHand.SetActive(false);
            targetMaterial_meijia.SetTexture("_MainTex", decalData.texture);
            TweenCameraSize(renderCamera, 0.5f);
            MoveRotateScale(renderHand.transform, new Vector3(0.32f, -0.68f, 5.53f), new Vector3(0, -9.26f, -0.418f), new Vector3(1, 1, 1));
        }
        else
        {
            nailHand.SetActive(false);
            renderHand.SetActive(true);
            targetMaterial_tiehua.SetTexture("_MainTex", decalData.texture);
            MoveRotateScale(renderHand.transform, new Vector3(-0.16f, 2.91f, 5.58f), new Vector3(0, 0, 3.42f), new Vector3(1, 1, 1));
            TweenCameraSize(renderCamera, 1.5f);
        }
    }

    /// <summary>
    /// 设置游戏内真正的皮肤效果
    /// </summary>
    /// <param name="category">0为美甲，1为贴画</param>
    /// <param name="texture">材质贴图</param>
    public void ShowRealHand(DecalData decalData)
    {
        if (decalData.type == SkinType.NailArt)
        {
            myhandMeijiaMaterial.SetTexture("_MainTex", decalData.texture);
            foreach (var skin in GameManager.instance.gameInfo.meijiaDecals)
            {
                skin.isEquip = (skin.id == decalData.id);
            }
        }
        else
        {
            myhandTiehuaMaterial.SetTexture("_MainTex", decalData.texture);
            foreach (var skin in GameManager.instance.gameInfo.tiehuaDecals)
            {
                skin.isEquip = (skin.id == decalData.id);
            }
        }
    }

    private void EquipButtonClick(ToggleItem toggleItem,DecalData decalData)
    {
        equipItem.transform.Find("是否已装备Label").gameObject.SetActive(false);//获取之前装备的这个
        equipDecal.isEquip = false;

        toggleItem.transform.Find("是否已装备Label").gameObject.SetActive(true);//将当前的这个设为true
        ShowRealHand(decalData);

        equipItem = toggleItem;
        equipDecal = decalData;

        DataManager.instance.SyncGameInfo();//同步数据
    }

    void InitGrid(LoopGridView loopGridView)
    {
        mLoopGridView_meijia.gameObject.SetActive(loopGridView == mLoopGridView_meijia);
        mLoopGridView_tiehua.gameObject.SetActive(loopGridView == mLoopGridView_tiehua);

        DecalData ownedDecal = current_decalDatabase.decalList.Find(decal => decal.isEquip);
        ShowRenderHand(ownedDecal);

        if (loopGridView == mLoopGridView_meijia)
        {
            prefabName = "Toggle_美甲";
            if (isMeijiaLoaded) return;
            else 
            {
                itemWidth = 230;
                itemHeight = 370;
                isMeijiaLoaded = true;
            }
            
        }
        else
        {
            prefabName = "Toggle_贴花";
            if (isTiehuaLoaded) return;
            else
            {
                itemWidth = 250;
                itemHeight = 250;
                isTiehuaLoaded = true;
            } 
        }


        mColumCount = CalculateColumnCount(loopGridView);

        var settingParam = new LoopGridViewSettingParam
        {
            mItemSize = new Vector2(itemWidth, itemHeight),
            mItemPadding = new Vector2(itemPaddingX, itemPaddingY),
            mGridFixedType = GridFixedType.ColumnCountFixed,
            mFixedRowOrColumnCount = mColumCount
        };

        loopGridView.InitGridView(totalCount, OnGetItemByRowColumn, settingParam);
        
    }

    LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
    {
        int realIndex = CalculateIndexByColumnCount(column, row);
        if (index < 0 || index >= decalDatabase_meijia.decalList.Count)
        {
            return null;
        }
        LoopGridViewItem item = gridView.NewListViewItem(prefabName);
        ToggleItem itemScript = item.GetComponent<ToggleItem>();
        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            itemScript.Init(toggleItemAndDatas_current[realIndex]);// here to init the item, such as add button click event listener.
        }
        //否则只需要更新就行
        itemScript.UpdateUI(toggleItemAndDatas_current[realIndex], realIndex);
        return item;
    }

    int CalculateColumnCount(LoopGridView loopGridView)
    {
        
        RectTransform viewport = loopGridView.GetComponent<ScrollRect>().viewport;
        float viewWidth = viewport.rect.width;
        float totalItemWidth = itemWidth + itemPaddingX;
        int col = Mathf.FloorToInt(viewWidth / totalItemWidth);
        return Mathf.Max(1, col); // 最少1列
    }

    int CalculateIndexByColumnCount(int column,int row)
    {
        return row*mColumCount+column;
    }
}


public enum UnlockMethod
{
    CoinPurchase,    // 使用金币购买
    ShareToUnlock,   // 分享解锁
    Achievement,     // 成就解锁
    DailyLogin,      // 连续登录赠送
    EventReward,     // 活动奖励
    //AdsUnlock        // 看广告获得
}
public enum SkinType
{
    NailArt,    // 美甲
    Tattoo,     // 贴画
    // Future: Ring, Bracelet, etc.
}

[System.Serializable]
public class DecalData
{
    // 使用 WeakReference 缓存 Sprite，防止 Sprite 长期驻留内存，导致内存占用过高
    static Dictionary<Texture2D, WeakReference<Sprite>> _spriteCache = new();
    [SerializeField] private bool isDefault;
    public bool IsDefault => isDefault; // 外部只读
    public string id;
    public Texture2D texture;
    public string description;
    public int price;
    public bool isOwned;
    public bool isEquip;
    public bool isNewest;//是否是最新获得的
    public SkinType type;

    public UnlockMethod unlockMethod = UnlockMethod.CoinPurchase; // 默认金币购买
    public string unlockInfo; // 附加信息，如成就ID、活动ID、分享标题等

    private static readonly List<string> meijiaDescription = new List<string>
    {
        "",
        "每轮倒计时长+0.3秒",
        "每轮倒计时长+0.6秒",
        "每轮倒计时长+1秒"
    };

    private static readonly List<string> tiehuaDescription = new List<string>
    {
        "",
        "每轮PK胜利时额外+1金币",
        "每轮PK胜利时额外+2金币",
        "每轮PK胜利时额外+5金币"
    };


    /// <summary>
    /// 获取对应的 Sprite 缩略图，若已缓存则复用，否则动态创建并缓存（使用 WeakReference）
    /// </summary>
    /// <returns>缩略图 Sprite</returns>
    public Sprite GetThumbnail()
    {
        // 若纹理为空，返回空
        if (texture == null)
            return null;

        // 尝试从缓存中获取 Sprite
        if (_spriteCache.TryGetValue(texture, out var weakRef))
        {
            if (weakRef.TryGetTarget(out var cachedSprite) && cachedSprite != null)
            {
                return cachedSprite; // 缓存命中，直接返回
            }
        }

        // 创建新的 Sprite（注意：像素单位设为100，可根据项目需求调整）
        var sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f) // 设定 pivot 为中心
        );

        // 将 Sprite 缓存起来（使用弱引用）
        _spriteCache[texture] = new WeakReference<Sprite>(sprite);
        return sprite;
    }
}

