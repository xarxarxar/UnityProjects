using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 导入 DOTween 命名空间
using System.Collections;


/// <summary>
/// 引导遮罩控制器
/// 此脚本挂载在 Mask (Image/RectTransform) 的父对象上，用于对挖孔遮罩进行持久控制。
/// </summary>
public class TutorialMask : MonoBehaviour
{
    // --- 外部配置 ---
    [Header("核心引用 (拖入遮罩子对象)")]
    [Tooltip("执行挖孔操作的全屏 Image 遮罩子对象。")]
    public Image MaskImage;

    [Header("遮挡鼠标点击事件")]
    public Image MaskRaycast;
    [Tooltip("Image 遮罩子对象的 RectTransform。")]
    public RectTransform MaskRect;

    [Header("目标类型选择")]
    [Tooltip("选择要跟踪的目标类型：2D 世界对象或 UI 元素。")]
    public TargetType targetType = TargetType.World2DObject;

    [Header("挖孔目标")]
    [Tooltip("当目标类型为 World2DObject 时使用。必须拖入带有 SpriteRenderer 的对象。")]
    public Transform World2DTarget;

    [Tooltip("当目标类型为 UIElement 时使用。必须拖入一个 RectTransform。")]
    public RectTransform UITarget;

    [Header("挖孔尺寸偏移")]
    [Tooltip("在屏幕像素单位下，添加到目标边界的额外尺寸 (X 和 Y)。动态修改此值会实时更新挖孔区域。")]
    [SerializeField]
    private Vector2 sizeOffset = new Vector2(50f, 50f); // 私有字段，序列化 (在 Inspector 中可见)

    [Tooltip("挖孔动画的持续时间。")]
    [SerializeField] private float transitionDuration = 0.5f;

    [Header("图像 PPU 控制")]
    [Tooltip("主 Image 组件的 PPU (每单位像素) 乘数。动态修改此值会应用更改。")]
    [SerializeField]
    private float imagePPU = 1.0f; // 私有字段，序列化 (在 Inspector 中可见)

    [Header("点击/交互控制")]
    [Tooltip("如果勾选，点击将穿透遮罩，击中下方的目标对象或 UI。")]
    public bool AllowClickThrough = true;

    [Header("显示/隐藏控制")]
    [Tooltip("遮罩整体淡入/淡出的持续时间。")]
    [SerializeField] private float fadeDuration = 0.3f;

    // --- 内部引用 ---
    private Canvas canvas;
    private Camera mainCamera;
    private Vector2 defaultSize = new Vector2(2000f, 2000f); // 初始大尺寸，用于动画开始
    public CanvasGroup canvasGroup; // 用于控制点击穿透和整体淡出
    private SpriteRenderer targetSpriteRenderer; // 缓存的 2D 对象 SpriteRenderer

    // --- 编辑器/运行时实时更新 ---
    /// <summary>
    /// 当 Inspector 中的值被修改时调用，用于实时更新效果。
    /// </summary>
    private void OnValidate()
    {
        // 实时应用 Image PPU 更改
        if (MaskImage != null)
        {
            MaskImage.pixelsPerUnitMultiplier = imagePPU;
        }

        // SizeOffset 的实时应用在可见时由 UpdateCutoutAnimated 处理。
    }

    // --- 初始化 ---
    void Awake()
    {
        // 核心引用空检查
        if (MaskImage == null || MaskRect == null)
        {
            Debug.LogError("TutorialMaskController 初始化失败：请拖入 MaskImage 和 MaskRect 引用。", this);
            enabled = false;
            return;
        }

        // 确保主遮罩 Image 存在并将其设置为半透明黑色
        if (MaskImage != null)
        {
            MaskImage.color = new Color(0, 0, 0, 0.8f);
            // 确保 PPU 使用序列化字段值进行初始化
            MaskImage.pixelsPerUnitMultiplier = imagePPU;
        }

        // 初始设置：CanvasGroup alpha 为 0，遮罩默认隐藏
        canvasGroup.alpha = 0f;

        // 获取 Canvas 和 Camera (查找父级 Canvas)
        canvas = GetComponentInParent<Canvas>();

        // 尝试获取主摄像机，或者 Canvas 使用的渲染摄像机
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
        {
            mainCamera = canvas.worldCamera;
        }
        else
        {
            mainCamera = Camera.main;
        }

        // 检查核心引用
        if (canvas == null || (mainCamera == null && canvas.renderMode != RenderMode.ScreenSpaceOverlay))
        {
            Debug.LogError("TutorialMaskController 初始化失败：无法获取 Canvas/Camera 引用。", this);
            enabled = false;
            return;
        }

        // 检查 2D 目标的 SpriteRenderer (如果在 Inspector 中设置)
        if (targetType == TargetType.World2DObject && World2DTarget != null)
        {
            targetSpriteRenderer = World2DTarget.GetComponent<SpriteRenderer>();
        }

        // 初始设置为大尺寸，使挖孔区域完全收缩
        MaskRect.sizeDelta = defaultSize;

        // 初始禁用 Mask 子对象，等待 ShowMask() 调用
        MaskImage.gameObject.SetActive(false);
        MaskRaycast.gameObject.SetActive(false);
    }

    void Start()
    {
        // Start 方法仅用于设置初始状态，不会立即开始动画
        SetClickThrough(AllowClickThrough);
    }

    // ----------------------------------------------------------------------
    // 重命名和合并的功能：ShowMask (以前是 SetNewTarget + Show)
    // ----------------------------------------------------------------------

    /// <summary>
    /// 设置一个新的目标 (针对 2D 世界对象)，并通过淡入和挖孔动画显示遮罩。
    /// </summary>
    /// <param name="newWorld2DTarget">新的 2D 世界对象 Transform。</param>
    /// <param name="instant">如果为 true，则跳过淡入和过渡时间。</param>
    public void ShowMask(Transform newWorld2DTarget,Vector2 offset, bool instant = true)
    {
        // 1. 设置目标逻辑
        World2DTarget = newWorld2DTarget;
        UITarget = null; // 清除另一个引用
        targetType = TargetType.World2DObject;
        targetSpriteRenderer = null;
        sizeOffset = offset;

        if (World2DTarget == null)
        {
            Debug.LogError("ShowMask 失败：新的 World2DTarget 为空。正在隐藏遮罩。");
            Hide(true);
            return;
        }

        // 尝试获取 SpriteRenderer
        targetSpriteRenderer = World2DTarget.GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer == null)
        {
            Debug.LogError($"目标 '{World2DTarget.name}' 缺少 SpriteRenderer 组件，不能用作 2D 目标。正在隐藏遮罩。", World2DTarget);
            Hide(true);
            return;
        }

        // 2. 显示遮罩逻辑 (来自旧的 Show 方法)
        if (canvasGroup == null) return;

        // 激活 Mask 子对象以确保渲染和事件监听
        MaskImage.gameObject.SetActive(true);
        MaskRaycast.gameObject.SetActive(true);

        // 停止 Canvas Group 上的所有 DOTween 动画
        canvasGroup.DOKill();
        MaskRect.DOKill(true);
        StopAllCoroutines(); // 停止任何待处理的遮罩协程

        // 确保点击穿透状态正确
        SetClickThrough(AllowClickThrough);

        // 整体淡入
        if (instant || fadeDuration <= 0f)
        {
            canvasGroup.alpha = 1f;
            // 立即开始挖孔动画
            UpdateCutoutAnimated();
        }
        else
        {
            // 淡入 CanvasGroup
            canvasGroup.DOFade(1f, fadeDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    // 在淡入完成后开始挖孔动画
                    UpdateCutoutAnimated();
                });
        }
    }

    /// <summary>
    /// 设置一个新的目标 (针对 UI 元素)，并通过淡入和挖孔动画显示遮罩。
    /// </summary>
    /// <param name="newUITarget">新的 UI 元素 RectTransform。</param>
    /// <param name="instant">如果为 true，则跳过淡入和过渡时间。</param>
    public void ShowMask(RectTransform newUITarget, Vector2 offset, bool instant = true)
    {
        // 1. 设置目标逻辑
        UITarget = newUITarget;
        World2DTarget = null; // 清除另一个引用
        targetSpriteRenderer = null; // 清除缓存
        targetType = TargetType.UIElement;
        sizeOffset = offset;

        if (UITarget == null)
        {
            Debug.LogError("ShowMask 失败：新的 UITarget 为空。正在隐藏遮罩。");
            Hide(true);
            return;
        }

        // 2. 显示遮罩逻辑 (来自旧的 Show 方法)
        if (canvasGroup == null) return;

        // 激活 Mask 子对象以确保渲染和事件监听
        MaskImage.gameObject.SetActive(true);
        MaskRaycast.gameObject.SetActive(true);

        // 停止 Canvas Group 上的所有 DOTween 动画
        canvasGroup.DOKill();
        MaskRect.DOKill(true);
        StopAllCoroutines(); // 停止任何待处理的遮罩协程

        // 确保点击穿透状态正确
        SetClickThrough(AllowClickThrough);

        // 整体淡入
        if (instant || fadeDuration <= 0f)
        {
            canvasGroup.alpha = 1f;
            // 立即开始挖孔动画
            UpdateCutoutAnimated();
        }
        else
        {
            // 淡入 CanvasGroup
            canvasGroup.DOFade(1f, fadeDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    // 在淡入完成后开始挖孔动画
                    UpdateCutoutAnimated();
                });
        }
    }

    /// <summary>
    /// 隐藏遮罩，带有淡出动画。
    /// </summary>
    public void Hide(bool instant = false)
    {
        if (canvasGroup == null) return;

        // 停止所有 DOTween 动画
        canvasGroup.DOKill();
        MaskRect.DOKill(true); // 停止挖孔尺寸动画
        StopAllCoroutines(); // 停止 AnimateMaskRoutine 协程

        // 1. 整体淡出
        if (instant || fadeDuration <= 0f)
        {
            canvasGroup.alpha = 0f;
            // 同时将挖孔区域重置为默认大尺寸，防止下次 Show 时闪烁
            MaskRect.sizeDelta = defaultSize;
            // 隐藏 Mask 子对象
            MaskImage.gameObject.SetActive(false);
            MaskRaycast.gameObject.SetActive(false);
        }
        else
        {
            // 动画：将挖孔区域扩大到默认大尺寸 (创建 "消失" 效果)
            MaskRect.DOSizeDelta(defaultSize, transitionDuration * 0.5f)
                .SetEase(Ease.OutSine);

            // 淡出 CanvasGroup
            canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    // 在淡出完成后隐藏 Mask 子对象
                    MaskImage.gameObject.SetActive(false);
                    MaskRaycast.gameObject.SetActive(false);
                });
        }
    }

    /// <summary>
    /// 设置遮罩是否允许点击穿透。
    /// </summary>
    public void SetClickThrough(bool allow)
    {
        // blocksRaycasts 控制是否阻挡点击，interactable 控制是否接收事件
        canvasGroup.blocksRaycasts = !allow;
        canvasGroup.interactable = !allow;
    }

    /// <summary>
    /// 计算目标尺寸并执行 DoTween 动画。
    /// </summary>
    public Coroutine UpdateCutoutAnimated()
    {
        // 停止可能正在运行的旧 DOTween 动画
        MaskRect.DOKill(true);
        // 停止可能正在运行的旧协程
        StopAllCoroutines();
        return StartCoroutine(AnimateMaskRoutine());
    }

    private IEnumerator AnimateMaskRoutine()
    {
        // 等待一帧，确保所有 Start 方法和布局计算完成
        yield return null;

        // 1. 计算目标位置和尺寸
        Vector2 targetPosition;
        Vector2 targetSize;

        bool success = false;

        if (targetType == TargetType.World2DObject)
        {
            // 针对 2D 世界对象
            success = CalculateWorldToUICoords(out targetPosition, out targetSize);
        }
        else // TargetType.UIElement
        {
            // 针对 UI 元素 (使用鲁棒的坐标转换)
            success = CalculateUIToUICoUICoords(out targetPosition, out targetSize);
        }

        if (!success)
        {
            Debug.LogError("无法计算目标坐标，动画中止。", this);
            // 如果计算失败，将挖孔区域重置为大尺寸
            MaskRect.sizeDelta = defaultSize;
            yield break;
        }

        // 2. DOTween 动画

        // 动画：将挖孔区域缩小到目标尺寸
        MaskRect.DOSizeDelta(targetSize, transitionDuration)
            .SetEase(Ease.OutQuad);

        // 动画：将挖孔区域移动到目标位置
        MaskRect.DOAnchorPos(targetPosition, transitionDuration)
            .SetEase(Ease.OutQuad);
    }

    // ----------------------------------------------------------------------
    // 核心功能 1: 坐标转换 (针对 2D 世界对象)
    // ----------------------------------------------------------------------

    private bool CalculateWorldToUICoords(out Vector2 localPoint, out Vector2 sizeDelta)
    {
        localPoint = Vector2.zero;
        sizeDelta = Vector2.zero;

        if (targetSpriteRenderer == null)
        {
            return false;
        }

        // 1. 获取对象的 World Space Bounds (世界空间边界)
        Bounds worldBounds = targetSpriteRenderer.bounds;

        // 2. 将世界边界的中心和两个角转换为屏幕像素坐标
        Vector3 screenCenter = mainCamera.WorldToScreenPoint(worldBounds.center);
        Vector3 screenMin = mainCamera.WorldToScreenPoint(worldBounds.min);
        Vector3 screenMax = mainCamera.WorldToScreenPoint(worldBounds.max);

        // 3. 计算屏幕上的原始宽度和高度 (以像素为单位)
        float originalScreenWidth = screenMax.x - screenMin.x;
        float originalScreenHeight = screenMax.y - screenMin.y;

        // 4. 将屏幕中心点转换为 Canvas 上的本地位置 (localPoint)
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenCenter,
            eventCamera,
            out localPoint))
        {
            // 5. 应用尺寸计算

            // 获取 Canvas 缩放因子。这决定了像素值如何转换为 Canvas 本地单位。
            float canvasScaleFactor = canvasRect.localScale.x;

            // 原始尺寸 (Canvas 本地单位)
            float canvasLocalWidth = originalScreenWidth / canvasScaleFactor;
            float canvasLocalHeight = originalScreenHeight / canvasScaleFactor;

            // 像素偏移 (转换为 Canvas 本地单位)
            float offsetX = sizeOffset.x / canvasScaleFactor;
            float offsetY = sizeOffset.y / canvasScaleFactor;

            sizeDelta = new Vector2(
                canvasLocalWidth + offsetX,
                canvasLocalHeight + offsetY
            );
            return true;
        }

        return false;
    }

    // ----------------------------------------------------------------------
    // 核心功能 2: 鲁棒的坐标转换 (针对 UI 元素)
    // ----------------------------------------------------------------------
    private bool CalculateUIToUICoUICoords(out Vector2 localPoint, out Vector2 sizeDelta)
    {
        localPoint = Vector2.zero;
        sizeDelta = Vector2.zero;

        if (UITarget == null) return false;

        // 1. 获取目标的八个世界坐标角
        Vector3[] corners = new Vector3[4];
        UITarget.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera;

        // 2. 将角点转换为屏幕像素坐标 (屏幕空间)
        Vector3 screenBL = RectTransformUtility.WorldToScreenPoint(eventCamera, corners[0]); // 左下
        Vector3 screenTR = RectTransformUtility.WorldToScreenPoint(eventCamera, corners[2]); // 右上

        // 3. 计算屏幕上的原始宽度和高度 (以像素为单位)
        float originalScreenWidth = screenTR.x - screenBL.x;
        float originalScreenHeight = screenTR.y - screenBL.y;

        // 4. 计算屏幕中心点
        Vector3 screenCenter = (screenBL + screenTR) * 0.5f;

        // 5. 将屏幕中心点转换为 Canvas 上的本地位置 (localPoint)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenCenter,
            eventCamera,
            out localPoint))
        {
            // 6. 计算最终尺寸

            // 获取 Canvas 缩放因子。这决定了像素值如何转换为 Canvas 本地单位。
            float canvasScaleFactor = canvasRect.localScale.x;

            // 原始尺寸 (Canvas 本地单位)
            float canvasLocalWidth = originalScreenWidth / canvasScaleFactor;
            float canvasLocalHeight = originalScreenHeight / canvasScaleFactor;

            // 像素偏移 (转换为 Canvas 本地单位)
            float offsetX = sizeOffset.x / canvasScaleFactor;
            float offsetY = sizeOffset.y / canvasScaleFactor;

            sizeDelta = new Vector2(
                canvasLocalWidth + offsetX,
                canvasLocalHeight + offsetY
            );

            return true;
        }

        return false;
    }
}