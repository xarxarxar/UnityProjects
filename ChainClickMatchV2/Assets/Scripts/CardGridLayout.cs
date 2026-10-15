using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 把卡牌 prefab 在 CardRoot 下按 m×n 棋盘式排列，并按章节/关卡随机生成卡牌种类。
/// 间隔 (spacingX, spacingY) 为相邻卡牌中心之间的距离（世界单位），
/// 整个棋盘以本物体（CardRoot）的位置为中心居中，Z 统一为 0（2D 平面）。
///
/// 卡牌种类：由 chapter 指定章节（1~5，对应 Assets/Sprites/icons 下各章节文件夹），
/// 每个章节在 chapters 里配置 16 张种类 sprite（对应 1.png~16.png）；
/// 关卡 level 决定本局种类池大小（默认公式 min(4 + level×2, 16)，第 1 关 6 种、第 6 关起 16 种），
/// 每张卡在池内随机一个种类写入 SingleCard.type 并设置到 spriteRenderer。
///
/// 用法：
/// 1. 在场景中新建空物体并命名为 CardRoot，挂载本脚本；
/// 2. 设置 rows / columns / spacingX / spacingY、chapter / level；
/// 3. 在 chapters 里为每个章节配置 16 张种类 sprite（按 1~16 顺序）；
/// 4. 把 cardPrefab 指向 Assets/Prefabs/卡牌.prefab；
/// 5. 运行游戏（Awake 自动生成）；运行中在 Inspector 修改参数会实时同步棋盘。
///
/// 说明：实时同步依赖编辑器 Inspector 的 OnValidate 机制，仅编辑器内（含 Play 模式）生效；
/// 打包后的正式版本中如需在运行时动态改参数，请调用 RefreshBoard() 手动同步。
/// </summary>
public class CardGridLayout : MonoBehaviour
{
    /// <summary>一个章节的 16 张种类 sprite 配置。</summary>
    [System.Serializable]
    public class ChapterSprites
    {
        [Tooltip("章节号（1~5，对应 icons/1青青草原、2缤纷城市…）")]
        public int chapter = 1;

        [Tooltip("该章节的 16 张种类 sprite（按 1~16 顺序）")]
        public Sprite[] typeSprites = new Sprite[16];
    }

    [Header("棋盘尺寸（m × n）")]
    [Tooltip("行数 m（沿 Y 轴向下）")]
    [Min(1)]
    public int rows = 4;

    [Tooltip("列数 n（沿 X 轴向右）")]
    [Min(1)]
    public int columns = 4;

    [Header("中心间距（世界单位）")]
    [Tooltip("相邻卡牌中心的横向距离 x")]
    public float spacingX = 1.2f;

    [Tooltip("相邻卡牌中心的纵向距离 y")]
    public float spacingY = 1.2f;

    [Header("卡牌 Prefab")]
    [Tooltip("要实例化的卡牌 prefab（Assets/Prefabs/卡牌.prefab）")]
    public GameObject cardPrefab;

    [Header("自动生成（调试用）")]
    [Tooltip("场景加载时自动生成棋盘。默认关闭：正式流程由进入关卡时 StartLevel 才生成；调试棋盘可勾选")]
    public bool autoGenerateOnStart = false;

    [Header("章节与关卡")]
    [Tooltip("章节号（1~5，对应 Assets/Sprites/icons 下各章节文件夹）")]
    [Min(1)]
    public int chapter = 1;

    [Tooltip("关卡号，决定本局卡牌种类池大小（默认 min(4 + level×2, 16) 种）")]
    [Min(1)]
    public int level = 1;

    [Header("章节种类 Sprite（每章节 16 张，按 1~16 顺序）")]
    [Tooltip("按章节配置种类 sprite，生成时按 chapter 自动选用")]
    public List<ChapterSprites> chapters = new List<ChapterSprites>();

    [Header("消除倒计时")]
    [Tooltip("待消除列表 >= 2 张时触发的倒计时时长（秒），列表新增一张时重置为该时长")]
    [Min(0.1f)]
    public float countdownDuration = 5f;

    [Tooltip("实时显示倒计时进度的 Slider（未倒计时时自动隐藏；值为剩余时间占比 1 → 0）")]
    public Slider countdownSlider;

    [Header("消除分数")]
    [Tooltip("消除 2 张卡牌的基础分数")]
    [Min(0f)]
    public float baseScore = 10f;

    [Tooltip("每多消除一张卡牌，本次得分 × 该倍率（无上限）")]
    [Min(0f)]
    public float scoreMultiplier = 1.5f;

    [Tooltip("显示总分的 Text（实时显示为 分数：{score}），未赋值则不显示")]
    public Text scoreText;

    [Header("步数系统")]
    [Tooltip("第 1 关的可用步数")]
    [Min(1)]
    public int baseSteps = 10;

    [Tooltip("每提升 1 关额外增加的步数")]
    [Min(0)]
    public int extraStepsPerLevel = 2;

    [Tooltip("第 1 关的目标分数")]
    [Min(1)]
    public int baseTargetScore = 50;

    [Tooltip("每提升 1 关额外增加的目标分数")]
    [Min(0)]
    public int extraTargetPerLevel = 30;

    [Tooltip("显示剩余步数的 Text（实时显示为 步数：{remainingSteps}），未赋值则不显示")]
    public Text stepsText;

    [Tooltip("显示目标分数的 Text（实时显示为 目标：{targetScore}），未赋值则不显示")]
    public Text targetText;

    [Header("道具数量（每关开局重置）")]
    [Tooltip("步数道具数量（每关默认 3 个）")]
    [Min(0)]
    public int addStepsPropCount = 3;

    [Tooltip("提示道具数量（每关默认 3 个；且每关只能使用一次）")]
    [Min(0)]
    public int hintPropCount = 3;

    [Tooltip("洗牌道具数量（每关默认 3 个）")]
    [Min(0)]
    public int shufflePropCount = 3;

    [Header("掉落动画")]
    [Tooltip("消除后上方卡牌下落到空位的动画时长（秒），设为 0 则直接瞬移")]
    [Min(0f)]
    public float fallDuration = 0.3f;

    [Tooltip("下落动画的缓动曲线")]
    public Ease fallEase = Ease.OutQuad;

    /// <summary>本脚本生成的卡牌引用，重新生成时只清理这些，不影响手动放置的子物体。</summary>
    private readonly List<GameObject> _generated = new List<GameObject>();

    /// <summary>卡牌对象池：消除的卡牌回池复用，生成/补位时从池取出。</summary>
    private CardPool _pool;

    /// <summary>累计总分（每次得分均向下取整后累加，实际总分为整数）。</summary>
    private int _totalScore;

    /// <summary>当前关卡剩余步数（每次消除消耗 1 步）。</summary>
    public int remainingSteps;

    /// <summary>当前关卡目标分数（步数耗尽时结算：达标成功，未达标失败）。</summary>
    public int targetScore;

    /// <summary>关卡是否已结束（成功或失败），结束后不再响应点击。</summary>
    private bool _gameOver;

    /// <summary>当前关卡是否处于提示状态（提示道具触发，本关有效）。</summary>
    private bool _hintMode;

    /// <summary>提示道具本关是否已使用（每关只能使用一次）。</summary>
    private bool _hintUsedThisLevel;

    /// <summary>待消除列表：当前被选中待消除的卡牌（仅容纳同一种类的卡牌）。</summary>
    private readonly List<SingleCard> _pendingCards = new List<SingleCard>();

    /// <summary>是否正在倒计时。</summary>
    private bool _isCountingDown;

    /// <summary>当前倒计时剩余时间（秒）。</summary>
    private float _countdownRemaining;

    /// <summary>上次生成使用的 prefab，用于检测运行中切换 prefab 需要重建。</summary>
    private GameObject _lastPrefab;

    private int _lastChapter = -1;
    private int _lastLevel = -1;

    /// <summary>运行态是否已真正启动（Start 后为 true）：用于区分"进入 Play 的那次 OnValidate"与"运行中改参数"。</summary>
    private bool _runtimeEntered;

    /// <summary>当前关卡是否已开局（StartLevel 置 true）：未开局时棋盘上的卡牌不可点击/道具不可用。</summary>
    private bool _levelActive;

    /// <summary>关卡成功事件（达到目标分数时立即触发，无需等步数用完；参数为星级 0~3）。</summary>
    public event System.Action<int> OnLevelCleared;

    /// <summary>关卡失败事件（步数耗尽且未达目标分数时触发，供 UI 显示失败面板）。</summary>
    public event System.Action OnLevelFailed;

    private void Awake()
    {
        // 默认不在场景加载时自动生成棋盘（正式流程由进入关卡时 StartLevel 生成）；
        // autoGenerateOnStart 仅作调试用途
        if (autoGenerateOnStart && CountGenerated() != rows * columns)
        {
            GenerateBoard();
        }

        // 初始显示总分、剩余步数与目标分数
        RefreshScoreText();
        RefreshStepText();
        RefreshTargetText();
    }

    private void Start()
    {
        _runtimeEntered = true;
    }

    // 编辑器内字段被修改时回调（编辑态与运行态都会触发）
    private void OnValidate()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
        chapter = Mathf.Max(1, chapter);
        level = Mathf.Max(1, level);

        // 仅"运行中（Start 之后）"修改参数才实时刷新棋盘；
        // 进入 Play 模式触发的首次 OnValidate 不生成（正式流程由 StartLevel 开局）
        if (Application.isPlaying && _runtimeEntered && cardPrefab != null)
        {
            RefreshBoard();
        }
    }

    /// <summary>
    /// 按当前参数同步棋盘：prefab / 章节 / 关卡 / 数量变化则重建，否则仅重排位置。
    /// 运行中修改参数后可调用（含打包版本）。
    /// </summary>
    public void RefreshBoard()
    {
        if (cardPrefab == null)
        {
            ClearGenerated();
            _lastPrefab = null;
            _lastChapter = -1;
            _lastLevel = -1;
            return;
        }

        bool rebuild = _lastPrefab != cardPrefab
            || _lastChapter != chapter
            || _lastLevel != level
            || CountGenerated() != rows * columns;

        if (rebuild)
        {
            _lastPrefab = cardPrefab;
            _lastChapter = chapter;
            _lastLevel = level;
            GenerateBoard();
            return;
        }

        ApplyLayout();
    }

    /// <summary>
    /// 开始一局游戏（进入关卡时调用）：设置章节/关卡并全量重置——
    /// 总分清零、待消除列表清空、倒计时停止、棋盘重建、步数/目标/道具/提示状态/结束状态全部复位。
    /// </summary>
    public void StartLevel(int newChapter, int newLevel)
    {
        chapter = Mathf.Max(1, newChapter);
        level = Mathf.Max(1, newLevel);

        _levelActive = true;
        _totalScore = 0;
        RefreshScoreText();

        // GenerateBoard 内部会重置 remainingSteps/targetScore/_gameOver/道具数量/提示状态，
        // 并清空旧棋盘（含待消除列表与倒计时）、重建新棋盘
        GenerateBoard();
    }

    /// <summary>
    /// 退出/结束当前局时清理现场：整副棋盘回池（CardRoot 下不再有卡牌）、
    /// 清空待消除列表、停止倒计时、停止所有呼吸、复位提示与结束状态。
    /// 供返回关卡选择等退出场景调用，避免残留状态影响下一局。
    /// </summary>
    public void CleanupLevel()
    {
        _levelActive = false;
        // 棋盘全部回池（SetActive(false)、效果复位、不销毁，规避插件 OnDestroy 连坐材质问题）；
        // 内部同时清空待消除列表并停止倒计时，旧卡效果由 ReleaseCardToPool → InitCard 复位
        ClearGenerated();
        StopAllBreathEffects();
        _hintMode = false;
        _gameOver = false; // 复位通关/失败结束状态（StartLevel 也会重置，双保险）
    }

    /// <summary>清空上一次生成结果，并按当前参数重新生成 m×n 棋盘。</summary>
    [ContextMenu("重新生成棋盘")]
    public void GenerateBoard()
    {
        ClearGenerated();

        if (cardPrefab == null)
        {
            Debug.LogError($"[CardGridLayout] {name}：cardPrefab 未赋值，无法生成棋盘。", this);
            return;
        }

        int total = rows * columns;
        if (total <= 0)
        {
            Debug.LogWarning($"[CardGridLayout] {name}：rows×columns 必须大于 0（当前 rows={rows}, columns={columns}）。", this);
            return;
        }

        Sprite[] chapterSprites = GetChapterSprites(chapter);
        int poolSize = GetPoolSize(level);

        EnsurePool();

        _generated.Capacity = total;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject card = GetCardFromPool();
                card.name = $"卡牌_{r}_{c}";
                card.transform.localRotation = Quaternion.identity;
                card.transform.localScale = Vector3.one;

                ApplyCardIdentity(card, r, c, chapterSprites, poolSize);
                PlaceCardAt(card, r, c);
                _generated.Add(card);
            }
        }

        _lastPrefab = cardPrefab;
        _lastChapter = chapter;
        _lastLevel = level;

        Debug.Log($"[CardGridLayout] {name}：已生成 {rows}×{columns} 共 {total} 张卡牌，章节 {chapter}、关卡 {level}（种类池 {poolSize} 种），中心间距 ({spacingX}, {spacingY})。", this);

        // 关卡开局：重置步数、目标分数与结束状态；道具数量从存档读取（跨关保留，首次无存档默认 3）
        remainingSteps = GetLevelSteps(level);
        targetScore = GetTargetScore(level);
        _gameOver = false;
        DataManager dm = DataManager.Instance;
        addStepsPropCount = dm != null ? dm.AddStepsPropCount : 3;
        hintPropCount = dm != null ? dm.HintPropCount : 3;
        shufflePropCount = dm != null ? dm.ShufflePropCount : 3;
        _hintMode = false;
        _hintUsedThisLevel = false;
        RefreshStepText();
        RefreshTargetText();
        Debug.Log($"[CardGridLayout] {name}：第 {level} 关开始，剩余步数 {remainingSteps}，目标分数 {targetScore}。", this);

        // 棋盘生成后，统一刷新所有卡牌背景的 Order In Layer
        RefreshSortingOrder();
    }

    /// <summary>给单张卡牌写入章节/关卡身份、棋盘行列号，并在种类池内随机分配种类 sprite。</summary>
    private void ApplyCardIdentity(GameObject card, int boardRow, int boardCol, Sprite[] chapterSprites, int poolSize)
    {
        SingleCard sc = card.GetComponent<SingleCard>();
        if (sc == null)
        {
            Debug.LogWarning($"[CardGridLayout] {name}：卡牌 prefab 上未找到 SingleCard 脚本，卡牌 {card.name} 未分配章节/关卡/种类。", this);
            return;
        }

        sc.chapter = chapter;
        sc.level = level;
        sc.boardRow = boardRow;
        sc.boardCol = boardCol;
        sc.type = Random.Range(1, poolSize + 1);

        if (chapterSprites == null)
        {
            Debug.LogWarning($"[CardGridLayout] {name}：未配置章节 {chapter} 的种类 sprite（chapters 列表缺少该项），卡牌 {card.name} 保持默认图标。", this);
            return;
        }

        int spriteIndex = sc.type - 1;
        if (spriteIndex < 0 || spriteIndex >= chapterSprites.Length || chapterSprites[spriteIndex] == null)
        {
            Debug.LogWarning($"[CardGridLayout] {name}：章节 {chapter} 缺少第 {sc.type} 张种类 sprite，卡牌 {card.name} 保持默认图标。", this);
            return;
        }

        if (sc.spriteRenderer == null)
        {
            Debug.LogWarning($"[CardGridLayout] {name}：卡牌 {card.name} 的 spriteRenderer 未找到（SingleCard 需挂在卡牌根物体上，且根/子物体需有 SpriteRenderer）。", this);
            return;
        }

        sc.spriteRenderer.sprite = chapterSprites[spriteIndex];
    }

    /// <summary>按章节号查找配置好的 16 张种类 sprite。</summary>
    private Sprite[] GetChapterSprites(int chapterId)
    {
        for (int i = 0; i < chapters.Count; i++)
        {
            ChapterSprites set = chapters[i];
            if (set != null && set.chapter == chapterId)
            {
                return set.typeSprites;
            }
        }
        return null;
    }

    /// <summary>关卡决定种类池大小：第 L 关使用 min(4 + L×2, 16) 种（可按需调整公式）。</summary>
    private int GetPoolSize(int lvl)
    {
        return Mathf.Clamp(4 + lvl * 2, 1, 16);
    }

    /// <summary>
    /// 卡牌点击回调（由 SingleCard.OnMouseDown 触发）：
    /// - 点击的卡牌与待消除列表种类相同（或列表为空）→ 加入列表；
    /// - 点击了不同种类的卡牌 → 清空列表（旧卡牌取消选中光效、恢复正着），替换为当前点击的卡牌；
    /// - 列表内所有卡牌统一刷新选中效果，strength = 当前列表内卡牌个数。
    /// </summary>
    public void OnCardClicked(SingleCard card)
    {
        // 未开局（! _levelActive）或已结束（_gameOver）时忽略点击
        if (card == null || !_levelActive || _gameOver)
        {
            return;
        }

        // 已在列表中：忽略重复点击同一张卡牌（呼吸提示保持上一次点击的状态，无需刷新）
        if (_pendingCards.Contains(card))
        {
            return;
        }

        bool sameType = _pendingCards.Count == 0 || _pendingCards[0].type == card.type;
        if (!sameType)
        {
            // 点击了不同类型的卡牌：清空待消除列表，旧卡牌取消选中效果
            foreach (SingleCard c in _pendingCards)
            {
                c.ChangeChooseEffect(0);
            }
            _pendingCards.Clear();
        }

        _pendingCards.Add(card);

        // 列表内所有卡牌统一刷新效果，strength = 待消除列表中的卡牌个数
        int strength = _pendingCards.Count;
        foreach (SingleCard c in _pendingCards)
        {
            c.ChangeChooseEffect(strength);
        }

        // 呼吸提示必须在卡牌加入列表之后刷新：
        // 待消除列表中的卡牌不呼吸（它们已有 HighLight + ShakeSway），
        // 仅棋盘上同种类且不在列表中的卡牌呼吸提示。
        if (_hintMode)
        {
            UpdateHintBreath(card.type);
        }
        else
        {
            StopAllBreathEffects();
        }

        // 待消除列表 >= 2 张时触发/重置倒计时；不足 2 张时停止倒计时（隐藏 Slider）
        if (_pendingCards.Count >= 2)
        {
            StartCountdown();
        }
        else
        {
            StopCountdown();
        }
    }

    // ---------- 道具（供三个 Button 的 onClick 绑定） ----------

    /// <summary>道具①：当前步数 +3。每关默认 3 个。</summary>
    public void UseAddStepsProp()
    {
        if (!_levelActive || _gameOver)
        {
            return;
        }
        if (addStepsPropCount <= 0)
        {
            Debug.Log($"[CardGridLayout] {name}：步数道具已用完。", this);
            return;
        }
        addStepsPropCount--;
        remainingSteps += 3;
        RefreshStepText();
        PersistPropCounts();
        Debug.Log($"[CardGridLayout] {name}：使用步数道具（剩余 {addStepsPropCount} 个），剩余步数 +3 = {remainingSteps}。", this);
    }

    /// <summary>
    /// 道具②：提示——进入提示状态（本关有效，且每关只能使用一次）。
    /// 提示状态下，点击卡牌后：待消除列表中的卡牌照常 HighLight + ShakeSway，
    /// 棋盘上不在列表中但同种类的卡牌会呼吸提示玩家。
    /// </summary>
    public void UseHintProp()
    {
        if (!_levelActive || _gameOver)
        {
            return;
        }
        if (_hintUsedThisLevel)
        {
            Debug.Log($"[CardGridLayout] {name}：提示道具每关只能使用一次，本关已使用。", this);
            return;
        }
        if (hintPropCount <= 0)
        {
            Debug.Log($"[CardGridLayout] {name}：提示道具数量已用完。", this);
            return;
        }

        hintPropCount--;
        _hintUsedThisLevel = true;
        _hintMode = true;
        PersistPropCounts();
        Debug.Log($"[CardGridLayout] {name}：进入提示状态（剩余 {hintPropCount} 个），点击卡牌后同种卡将呼吸提示。", this);

        // 若玩家已点过卡（列表非空），立即以列表中的种类刷新呼吸
        if (_pendingCards.Count > 0 && _pendingCards[0] != null)
        {
            UpdateHintBreath(_pendingCards[0].type);
        }
    }

    /// <summary>提示状态下刷新呼吸：与指定种类相同且不在待消除列表中的卡牌呼吸提示，其余关闭。</summary>
    private void UpdateHintBreath(int type)
    {
        foreach (GameObject go in _generated)
        {
            if (go == null)
            {
                continue;
            }
            SingleCard sc = go.GetComponent<SingleCard>();
            if (sc == null)
            {
                continue;
            }
            bool inList = _pendingCards.Contains(sc);
            sc.BreathEffect(!inList && sc.type == type);
        }
    }

    /// <summary>道具③：随机洗牌——棋盘上所有存活卡牌的种类打乱重排（Fisher-Yates，保持各种类数量不变）。每关默认 3 个。</summary>
    public void UseShuffleProp()
    {
        if (!_levelActive || _gameOver)
        {
            return;
        }
        if (shufflePropCount <= 0)
        {
            Debug.Log($"[CardGridLayout] {name}：洗牌道具已用完。", this);
            return;
        }
        shufflePropCount--;
        PersistPropCounts();

        // 洗牌后种类已变，旧的待消除列表失效：清空并取消选中效果、停止倒计时
        foreach (SingleCard c in _pendingCards)
        {
            if (c != null)
            {
                c.ChangeChooseEffect(0);
            }
        }
        _pendingCards.Clear();
        StopCountdown();
        StopAllBreathEffects();

        // 收集棋盘上所有存活卡牌
        List<SingleCard> cards = new List<SingleCard>();
        foreach (GameObject card in _generated)
        {
            if (card == null || !card.activeSelf)
            {
                continue;
            }
            SingleCard sc = card.GetComponent<SingleCard>();
            if (sc != null)
            {
                cards.Add(sc);
            }
        }
        if (cards.Count < 2)
        {
            return;
        }

        // Fisher-Yates 打乱种类顺序，再按打乱后的顺序写回（种类构成不变）
        int[] types = new int[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            types[i] = cards[i].type;
        }
        for (int i = types.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = types[i];
            types[i] = types[j];
            types[j] = tmp;
        }

        Sprite[] chapterSprites = GetChapterSprites(chapter);
        for (int i = 0; i < cards.Count; i++)
        {
            SingleCard sc = cards[i];
            sc.type = types[i];
            int spriteIndex = sc.type - 1;
            if (chapterSprites != null && spriteIndex >= 0 && spriteIndex < chapterSprites.Length
                && chapterSprites[spriteIndex] != null && sc.spriteRenderer != null)
            {
                sc.spriteRenderer.sprite = chapterSprites[spriteIndex];
            }
        }
        Debug.Log($"[CardGridLayout] {name}：使用洗牌道具，{cards.Count} 张卡牌种类已重新打乱。", this);
    }

    /// <summary>道具数量变化后写回存档（跨关保留）。</summary>
    private void PersistPropCounts()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePropCounts(addStepsPropCount, hintPropCount, shufflePropCount);
        }
        else if (DataManager.Instance != null)
        {
            DataManager.Instance.SavePropCounts(addStepsPropCount, hintPropCount, shufflePropCount);
        }
    }

    /// <summary>停止棋盘上所有卡牌的呼吸效果。</summary>
    private void StopAllBreathEffects()
    {
        foreach (GameObject card in _generated)
        {
            if (card == null)
            {
                continue;
            }
            SingleCard sc = card.GetComponent<SingleCard>();
            if (sc != null)
            {
                sc.BreathEffect(false);
            }
        }
    }

    /// <summary>启动/重置倒计时为 countdownDuration 秒，并显示 Slider。</summary>
    private void StartCountdown()
    {
        _isCountingDown = true;
        _countdownRemaining = Mathf.Max(countdownDuration, 0.1f);
        SetSliderVisible(true);
    }

    /// <summary>停止倒计时并隐藏 Slider。</summary>
    private void StopCountdown()
    {
        _isCountingDown = false;
        _countdownRemaining = 0f;
        if (countdownSlider != null)
        {
            countdownSlider.value = 0f;
        }
        SetSliderVisible(false);
    }

    /// <summary>倒计时结束：待消除列表中的所有卡牌消失（回收到对象池），结算分数，列表清空，隐藏 Slider，并触发掉落补位。</summary>
    private void EliminatePendingCards()
    {
        int eliminatedCount = _pendingCards.Count;
        foreach (SingleCard c in _pendingCards)
        {
            if (c != null)
            {
                ReleaseCardToPool(c.gameObject);
            }
        }
        _pendingCards.Clear();
        StopCountdown();

        // 消除后待消除列表已空，提示呼吸一并清除（列表内/列表外的卡都不再呼吸），
        // 若仍处于提示状态，下一次点击卡牌时会按新点击的种类重新刷新
        StopAllBreathEffects();

        // 结算本次得分：2 张 = baseScore，每多一张 × scoreMultiplier，无上限（本次得分向下取整）
        _totalScore += CalculateEliminateScore(eliminatedCount);
        RefreshScoreText();

        // 每次消除消耗 1 步
        remainingSteps = Mathf.Max(0, remainingSteps - 1);
        RefreshStepText();

        // 通关/失败判定：
        // - 达到目标分数 → 立即通关（无需等步数用完），星级按剩余步数（0/1/2/>=3 → 0/1/2/3 星）；
        // - 步数耗尽仍未达标 → 失败。
        if (!_gameOver)
        {
            if (_totalScore >= targetScore)
            {
                FinishLevel(true);
            }
            else if (remainingSteps <= 0)
            {
                FinishLevel(false);
            }
        }

        FillGapsAfterElimination();
    }

    /// <summary>按消除数量计算本次得分并向下取整：1 张不计分；2 张 = baseScore；每多一张 ×scoreMultiplier（无上限）。</summary>
    private int CalculateEliminateScore(int cardCount)
    {
        if (cardCount <= 1)
        {
            return 0;
        }

        float score = baseScore;
        for (int i = 2; i < cardCount; i++)
        {
            score *= scoreMultiplier;
        }
        return Mathf.FloorToInt(score);
    }

    /// <summary>把总分实时显示到 scoreText（格式：分数：{score}，总分为每次得分取整后的累加）。</summary>
    private void RefreshScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"分数：{_totalScore}";
        }
    }

    /// <summary>把剩余步数实时显示到 stepsText（格式：步数：{remainingSteps}）。</summary>
    private void RefreshStepText()
    {
        if (stepsText != null)
        {
            stepsText.text = $"步数：{remainingSteps}";
        }
    }

    /// <summary>把目标分数实时显示到 targetText（格式：目标：{targetScore}）。</summary>
    private void RefreshTargetText()
    {
        if (targetText != null)
        {
            targetText.text = $"目标：{targetScore}";
        }
    }

    /// <summary>关卡步数：第 L 关 = baseSteps + extraStepsPerLevel × (L-1)。</summary>
    private int GetLevelSteps(int lvl)
    {
        return baseSteps + extraStepsPerLevel * Mathf.Max(0, lvl - 1);
    }

    /// <summary>关卡目标分数：第 L 关 = baseTargetScore + extraTargetPerLevel × (L-1)。</summary>
    private int GetTargetScore(int lvl)
    {
        return baseTargetScore + extraTargetPerLevel * Mathf.Max(0, lvl - 1);
    }

    /// <summary>
    /// 结算关卡：成功（达标）或失败（步数耗尽未达标），结束后锁定棋盘（不再响应点击）。
    /// 成功时按剩余步数计算星级并触发 OnLevelCleared(stars)；失败触发 OnLevelFailed。
    /// </summary>
    private void FinishLevel(bool success)
    {
        if (_gameOver)
        {
            return;
        }

        _gameOver = true;
        StopCountdown();

        if (success)
        {
            int stars = GetStars(remainingSteps);
            Debug.Log($"[CardGridLayout] {name}：关卡通关！总分 {_totalScore} ≥ 目标 {targetScore}，剩余步数 {remainingSteps}，{stars} 星。", this);
            OnLevelCleared?.Invoke(stars);
        }
        else
        {
            Debug.Log($"[CardGridLayout] {name}：关卡失败！步数耗尽时总分 {_totalScore} < 目标 {targetScore}。", this);
            OnLevelFailed?.Invoke();
        }
    }

    /// <summary>按剩余步数计算星级：0 步 0 星，1 步 1 星，2 步 2 星，≥3 步 3 星。</summary>
    private int GetStars(int remaining)
    {
        if (remaining >= 3)
        {
            return 3;
        }
        if (remaining == 2)
        {
            return 2;
        }
        if (remaining == 1)
        {
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// 消除后掉落补位：逐列处理——列内空位上方存活的卡牌向下掉落补齐（DOTween 动画），
    /// 列顶空出的位置从对象池取出新卡牌补上，种类仍在当前章节 sprite 数组 + 关卡种类池内随机。
    /// </summary>
    private void FillGapsAfterElimination()
    {
        if (_pool == null || _generated.Count == 0)
        {
            return;
        }

        Sprite[] chapterSprites = GetChapterSprites(chapter);
        int poolSize = GetPoolSize(level);

        for (int c = 0; c < columns; c++)
        {
            // 从下到上收集该列存活的卡牌（alive[0] 为该列最下面一张）
            List<GameObject> alive = new List<GameObject>(rows);
            for (int r = rows - 1; r >= 0; r--)
            {
                GameObject card = GetSlotCard(r, c);
                if (card != null && card.activeSelf)
                {
                    alive.Add(card);
                }
            }

            // 存活卡从列底开始重新放置：原本在空位上方的卡向下掉落
            int targetRow = rows - 1;
            for (int i = 0; i < alive.Count; i++)
            {
                GameObject card = alive[i];
                SingleCard sc = card.GetComponent<SingleCard>();
                int fromRow = sc != null ? sc.boardRow : -1;
                MoveCardToSlot(card, targetRow, c, fromRow != targetRow);
                targetRow--;
            }

            // 列顶剩余空位补新卡：新卡按目标行在列顶上方依次向上堆叠（起点行 = 目标行 - 补卡数），
            // 该列所有新卡下落距离相同，保持堆叠形状一起顺势落入棋盘
            int refillCount = rows - alive.Count;
            while (targetRow >= 0)
            {
                GameObject card = GetCardFromPool();
                card.name = $"卡牌_{targetRow}_{c}";
                card.transform.localScale = Vector3.one;
                ApplyCardIdentity(card, targetRow, c, chapterSprites, poolSize);

                // 起点：列顶上方，多张新卡按目标行依次向上堆叠（保持间距），再动画掉落到目标槽位
                int startRow = targetRow - refillCount;
                card.transform.localPosition = GetSlotLocalPosition(startRow, c);
                MoveCardToSlot(card, targetRow, c, true);
                targetRow--;
            }
        }

        // 消除掉落补位完成后，统一刷新所有卡牌背景的 Order In Layer
        RefreshSortingOrder();
    }

    /// <summary>卡牌背景 Order In Layer 的起始值（-999），按棋盘从左到右、从上到下依次 +1。</summary>
    private const int TileSortingBase = -999;

    /// <summary>从对象池取一张卡牌并复位状态（关闭光效、停止摇摆并恢复正着）。</summary>
    private GameObject GetCardFromPool()
    {
        GameObject card = _pool.Get();
        SingleCard sc = card.GetComponent<SingleCard>();
        if (sc != null)
        {
            sc.InitCard();
        }
        return card;
    }

    /// <summary>卡牌消失：清除其在棋盘槽位中的引用（标记空位）、复位效果、清理残留动画后回收到对象池（SetActive(false)）。</summary>
    private void ReleaseCardToPool(GameObject card)
    {
        if (card == null)
        {
            return;
        }
        SingleCard sc = card.GetComponent<SingleCard>();
        if (sc != null)
        {
            // 关键：先把它原来槽位的引用清掉（置 null = 空位）。
            // 否则该卡被池复用到其他槽位后，旧槽位仍引用它且 activeSelf=true，
            // 后续列处理会把它误判为该列存活卡，导致卡牌跨列移动。
            int index = sc.boardRow * columns + sc.boardCol;
            if (index >= 0 && index < _generated.Count && _generated[index] == card)
            {
                _generated[index] = null;
            }

            sc.InitCard(); // 关闭光效、停止摇摆并复位旋转
        }
        DOTween.Kill(card.transform); // 清理可能未完成的下落动画等残留 tween
        _pool.Release(card);
    }

    /// <summary>
    /// 确保对象池已存在且与当前 cardPrefab 匹配。
    /// 仅当 prefab 切换时才清池销毁旧池卡（销毁会触发插件 OnDestroy，仅限手动改 prefab 的调试场景）。
    /// </summary>
    private void EnsurePool()
    {
        if (cardPrefab == null)
        {
            return;
        }
        if (_pool == null || _pool.Prefab != cardPrefab)
        {
            if (_pool != null)
            {
                _pool.Clear();
            }
            _pool = new CardPool(cardPrefab, transform);
        }
    }

    /// <summary>
    /// 重新设置所有卡牌背景（tile）的 Order In Layer：从 -999 开始，
    /// 按棋盘从左到右（列 0→n）、从上到下（行 0→m）依次 +1（即行主序 index）。
    /// 每次棋盘变化（生成 / 消除掉落补位 / 运行中重排）后调用。
    /// </summary>
    private void RefreshSortingOrder()
    {
        for (int i = 0; i < _generated.Count; i++)
        {
            GameObject card = _generated[i];
            if (card == null)
            {
                continue;
            }
            SingleCard sc = card.GetComponent<SingleCard>();
            if (sc != null)
            {
                sc.SetTileSortingOrder(TileSortingBase + i);
            }
        }
    }

    /// <summary>把卡牌移动到 (r, c) 槽位；animated 时用 DOTween 下落动画，否则直接设置位置。同时更新行列号与棋盘引用。</summary>
    private void MoveCardToSlot(GameObject card, int r, int c, bool animated)
    {
        if (card == null)
        {
            return;
        }

        Vector3 pos = GetSlotLocalPosition(r, c);
        if (animated && fallDuration > 0f)
        {
            card.transform.DOLocalMove(pos, fallDuration).SetEase(fallEase);
        }
        else
        {
            card.transform.localPosition = pos;
        }

        UpdateSlot(card, r, c);
    }

    /// <summary>新卡直接放置到 (r, c) 槽位（无动画），并登记行列号。</summary>
    private void PlaceCardAt(GameObject card, int r, int c)
    {
        card.transform.localPosition = GetSlotLocalPosition(r, c);
        UpdateSlot(card, r, c);
    }

    /// <summary>把卡牌登记到 (r, c) 槽位：更新棋盘引用与卡牌自身的行列号。</summary>
    private void UpdateSlot(GameObject card, int r, int c)
    {
        int index = r * columns + c;
        if (index >= 0 && index < _generated.Count)
        {
            _generated[index] = card;
        }
        SingleCard sc = card != null ? card.GetComponent<SingleCard>() : null;
        if (sc != null)
        {
            sc.boardRow = r;
            sc.boardCol = c;
        }
    }

    /// <summary>读取 (r, c) 槽位当前的卡牌（越界返回 null）。</summary>
    private GameObject GetSlotCard(int r, int c)
    {
        int index = r * columns + c;
        if (index < 0 || index >= _generated.Count)
        {
            return null;
        }
        return _generated[index];
    }

    /// <summary>槽位 (r, c) 的局部坐标：第 0 列在最左、第 0 行在最上，棋盘整体以 CardRoot 位置为中心。</summary>
    private Vector3 GetSlotLocalPosition(int r, int c)
    {
        float x = (c - (columns - 1) * 0.5f) * spacingX;
        float y = -((r - (rows - 1) * 0.5f) * spacingY);
        return new Vector3(x, y, 0f);
    }

    private void Update()
    {
        if (!_isCountingDown)
        {
            return;
        }

        _countdownRemaining -= Time.deltaTime;

        // Slider 实时显示进度：剩余时间占比（1 → 0）
        if (countdownSlider != null)
        {
            countdownSlider.value = countdownDuration > 0f ? Mathf.Clamp01(_countdownRemaining / countdownDuration) : 0f;
        }

        if (_countdownRemaining <= 0f)
        {
            _countdownRemaining = 0f;
            EliminatePendingCards();
        }
    }

    private void SetSliderVisible(bool visible)
    {
        if (countdownSlider != null && countdownSlider.gameObject.activeSelf != visible)
        {
            countdownSlider.gameObject.SetActive(visible);
        }
    }

    /// <summary>只按当前参数重排已有卡牌位置（数量不变时的快速路径）。</summary>
    private void ApplyLayout()
    {
        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (index >= _generated.Count)
                {
                    return;
                }

                GameObject card = _generated[index++];
                if (card == null)
                {
                    continue;
                }

                // 居中：第 0 列在最左、第 0 行在最上，棋盘整体以 CardRoot 位置为中心
                card.transform.localPosition = GetSlotLocalPosition(r, c);
            }
        }

        // 重排后统一刷新所有卡牌背景的 Order In Layer（运行中调参实时同步）
        RefreshSortingOrder();
    }

    private int CountGenerated()
    {
        int count = 0;
        for (int i = 0; i < _generated.Count; i++)
        {
            if (_generated[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 清空当前棋盘：所有存活卡牌回收到对象池（SetActive(false)，不销毁）。
    ///
    /// 说明：刻意"回池复用"而不是 Destroy——销毁卡牌会触发卡牌上 HighlightPlus2D 的 OnDestroy，
    /// 插件会无条件销毁其占位共享材质（static dummyMaterial），导致后续新卡引用已销毁材质而报错
    /// （MissingReferenceException: Material has been destroyed）。复用 GameObject 则完全绕开该问题。
    /// </summary>
    private void ClearGenerated()
    {
        EnsurePool();
        for (int i = _generated.Count - 1; i >= 0; i--)
        {
            GameObject card = _generated[i];
            if (card == null)
            {
                continue;
            }
            ReleaseCardToPool(card);
        }
        _generated.Clear();

        // 棋盘重建时，清空待消除列表并停止倒计时（旧卡牌已回池禁用，无需再刷新效果）
        _pendingCards.Clear();
        StopCountdown();
    }
}
