using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始界面双 Page 翻页切换：不依赖 pages 数组，
/// 只滑动两个物理 Page 容器（pageA / pageB），两容器交替滑入滑出。
///
/// 默认显示第 1 章（CurrentIndex = 0），最大到第 n 章（pageCount = n）；
/// 滑到第一个（第 1 章）时左按钮消失，滑到最后一个（第 n 章）时右按钮消失。
///
/// 用法：
/// 1. 场景中准备两个全屏容器（同一 Canvas 下、铺满屏幕、居中），每个容器内摆放一页内容；
///    拖入 pageA / pageB（pageA 为初始显示页，pageB 待命）；
/// 2. 把左/右按钮拖入 leftButton / rightButton（脚本自动绑定 SwitchPrev / SwitchNext）；
/// 3. 运行即可左右翻页；动画进行中忽略点击（防重入）。
/// </summary>
public class PageSwitcher : MonoBehaviour
{
    [Header("两个物理 Page 容器（同一 Canvas 下、铺满屏幕、居中，各自内含一页内容）")]
    [Tooltip("当前页容器（初始显示页）")]
    public RectTransform pageA;

    [Tooltip("待命容器（切换时从屏幕外滑入）")]
    public RectTransform pageB;

    [Header("左右切换按钮（脚本自动绑定；也可手动在 onClick 里绑定 SwitchPrev/SwitchNext）")]
    public Button leftButton;
    public Button rightButton;

    [Header("标题")]
    [Tooltip("标题文本（随当前章节更新为“第X章”）")]
    public Text titleText;

    [Header("滑动动画")]
    [Tooltip("章节总数 n（默认 5）：第 1 章 ~ 第 n 章，决定 CurrentIndex 的范围与按钮消失边界")]
    public int pageCount = 5;

    [Tooltip("切换动画时长（秒）")]
    public float swipeDuration = 0.4f;

    [Tooltip("切换动画缓动")]
    public Ease swipeEase = Ease.InOutQuad;

    [Tooltip("滑动一屏的宽度（本地单位；-1 时自动取容器宽度）")]
    public float pageWidth = -1f;

    [Tooltip("两个 Page 之间的间距（本地单位；滑动时相邻页的间隔）")]
    public float pageSpacing = 0f;

    /// <summary>当前显示页索引（0..pageCount-1，无限循环）。</summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>当前正在显示内容的容器。</summary>
    private RectTransform _activeContainer;

    /// <summary>待命容器（下一次切换时滑入）。</summary>
    private RectTransform _idleContainer;

    private int _currentIndex;

    /// <summary>切换动画进行中，忽略按钮点击。</summary>
    private bool _animating;

    private float _actualPageWidth = 1920f;

    private void Awake()
    {
        if (pageCount <= 0)
        {
            pageCount = 5;
        }

        _activeContainer = pageA;
        _idleContainer = pageB;
        _currentIndex = 0;
        RefreshPageWidth();

        if (leftButton != null)
        {
            leftButton.onClick.AddListener(SwitchPrev);
        }
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(SwitchNext);
        }

        // 初始化：pageA 显示在中心，pageB 隐藏待命；默认第 1 章
        if (_activeContainer != null)
        {
            _activeContainer.localPosition = Vector3.zero;
            _activeContainer.gameObject.SetActive(true);
        }
        if (_idleContainer != null)
        {
            _idleContainer.gameObject.SetActive(false);
        }

        // 默认第 1 章：左按钮消失，右按钮显示；标题显示"第1章"
        UpdateButtons();
        UpdateTitle();
    }

    private void Start()
    {
        // 布局稳定后再取一次容器宽度（Awake 时 RectTransform 尺寸可能还未计算）
        RefreshPageWidth();
    }

    /// <summary>自动取容器宽度作为一屏滑动距离（pageWidth <= 0 时）。</summary>
    private void RefreshPageWidth()
    {
        if (pageWidth > 0f)
        {
            _actualPageWidth = pageWidth;
            return;
        }
        if (_activeContainer != null && _activeContainer.rect.width > 0f)
        {
            _actualPageWidth = _activeContainer.rect.width;
        }
        else if (_idleContainer != null && _idleContainer.rect.width > 0f)
        {
            _actualPageWidth = _idleContainer.rect.width;
        }
        else
        {
            _actualPageWidth = Screen.width;
        }
    }

    /// <summary>切换下一页（右按钮：下一页从右侧滑入、当前页向左滑出）。</summary>
    public void SwitchNext()
    {
        Switch(1);
    }

    /// <summary>切换上一页（左按钮：上一页从左侧滑入、当前页向右滑出）。</summary>
    public void SwitchPrev()
    {
        Switch(-1);
    }

    /// <summary>按方向翻页：direction = 1 下一页，-1 上一页。边界处不响应。</summary>
    public void Switch(int direction)
    {
        if (_animating || _activeContainer == null || _idleContainer == null)
        {
            return;
        }
        if (direction == 0)
        {
            return;
        }

        // 第 1 章 ~ 第 n 章：超出边界不切换（对应方向的按钮此时应已消失）
        int nextIndex = _currentIndex + direction;
        if (nextIndex < 0 || nextIndex >= pageCount)
        {
            return;
        }
        _animating = true;

        // 滑动距离 = 一屏宽度 + 两页间距（间距由 Inspector 的 pageSpacing 设置）
        float slideDistance = _actualPageWidth + pageSpacing;

        // 当前页向相反方向滑出屏幕
        _activeContainer.DOLocalMove(new Vector3(-direction * slideDistance, 0f, 0f), swipeDuration)
            .SetEase(swipeEase)
            .OnComplete(() =>
            {
                _activeContainer.gameObject.SetActive(false);
                CompleteSwitch(nextIndex);
            });

        // 待命容器从对应侧屏幕外滑入中心
        _idleContainer.localPosition = new Vector3(direction * slideDistance, 0f, 0f);
        _idleContainer.gameObject.SetActive(true);
        _idleContainer.DOLocalMove(Vector3.zero, swipeDuration).SetEase(swipeEase);
    }

    /// <summary>切换完成：容器角色互换（滑入的容器成为当前显示容器），更新页索引并刷新按钮显隐。</summary>
    private void CompleteSwitch(int nextIndex)
    {
        RectTransform tmp = _activeContainer;
        _activeContainer = _idleContainer;
        _idleContainer = tmp;

        _currentIndex = nextIndex;
        _animating = false;

        // 边界处对应按钮消失：第 1 章隐藏左按钮，第 n 章隐藏右按钮
        UpdateButtons();
        UpdateTitle();
    }

    /// <summary>根据当前页刷新左右按钮显隐：边界页隐藏对应方向按钮。</summary>
    private void UpdateButtons()
    {
        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(_currentIndex > 0);
        }
        if (rightButton != null)
        {
            rightButton.gameObject.SetActive(_currentIndex < pageCount - 1);
        }
    }

    /// <summary>根据当前章节刷新标题文本："第X章"。</summary>
    private void UpdateTitle()
    {
        if (titleText != null)
        {
            titleText.text = "第" + (_currentIndex + 1) + "章";
        }
    }
}
