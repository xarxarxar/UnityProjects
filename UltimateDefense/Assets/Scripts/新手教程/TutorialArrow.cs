using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening; // 引入 DOTween 命名空间

/// <summary>
/// 箭头指示方向
/// </summary>
public enum ArrowDirection
{
    Up,      // 箭头向上指 (箭头在目标下方)
    Down,    // 箭头向下指 (箭头在目标上方)
    Left,    // 箭头向左指 (箭头在目标右方)
    Right,   // 箭头向右指 (箭头在目标左方)
}

/// <summary>
/// 指示箭头
/// </summary>
public class TutorialArrow : MonoBehaviour
{
    // --- 外部配置 ---
    [Header("箭头 GameObject")]
    [Tooltip("场景中的箭头指示器 GameObject (子物体)。它必须在 Canvas 下，并拥有 RectTransform。")]
    public Transform arrow;

    [Header("目标配置")]
    [Tooltip("选择要追踪的目标类型。")]
    public TargetType targetType = TargetType.UIElement;

    [Tooltip("当目标类型为 World2DObject 时使用。")]
    public Transform World2DTarget;

    [Tooltip("当目标类型为 UIElement 时使用。")]
    public RectTransform UITarget;

    [Header("箭头指示设置 (应用于父物体)")]
    [Tooltip("箭头相对于目标的朝向。")]
    public ArrowDirection direction = ArrowDirection.Up;

    [Tooltip("箭头与目标中心点的距离偏移。")]
    public float distanceOffset = 0f;

    [Tooltip("箭头最终位置的微调偏移。")]
    public Vector2 localExtraOffset = Vector2.zero;

    [Header("DOTween 动画设置 (应用于子物体)")]
    [Tooltip("是否启用箭头移动动画。")]
    public bool enableAnimation = true;
    [Tooltip("箭头在动画中移动的局部距离。")]
    public float animationDistance = 15f;
    [Tooltip("单程动画所需时间。")]
    public float animationDuration = 0.5f;

    // --- 内部引用 (缓存) ---
    public RectTransform parentRect; // 箭头父物体 (本脚本挂载的 RectTransform)
    private RectTransform arrowRect;  // 箭头子物体 (视觉元素的 RectTransform)
    private RectTransform canvasRect;
    private Camera mainCamera;
    private SpriteRenderer targetSpriteRenderer; // 缓存 2D 物体 SpriteRenderer
    // 用于缓存 UI 目标的四个角的世界坐标
    private Vector3[] targetCorners = new Vector3[4];

    void Awake()
    {
        if (parentRect == null)
        {
            Debug.LogError("TutorialArrow 脚本必须挂载在一个 RectTransform 上。", this);
            enabled = false;
            return;
        }

        // 2. 检查并获取子物体的 RectTransform (动画用)
        if (arrow == null)
        {
            Debug.LogError("Arrow 引用未设置。请在 Inspector 中拖入场景中的箭头 UI 对象。", this);
            enabled = false;
            return;
        }

        arrowRect = arrow.GetComponent<RectTransform>();
        if (arrowRect == null)
        {
            Debug.LogError("箭头 UI 对象缺少 RectTransform 组件。", this);
            enabled = false;
            return;
        }

        // 确保箭头的 pivot 设置为中心 (0.5, 0.5)
        arrowRect.pivot = new Vector2(0.5f, 0.5f);

        // 3. 获取 Canvas 引用
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("TutorialArrow 必须位于 Canvas 下。", this);
            enabled = false;
            return;
        }

        canvasRect = canvas.transform as RectTransform;

        // 4. 获取 Camera 引用
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
        {
            mainCamera = canvas.worldCamera;
        }
        else
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("无法找到主摄像机。", this);
            enabled = false;
            return;
        }

        // 5. 【修正点 1】：在启动时隐藏父物体
        parentRect.gameObject.SetActive(false);
    }

    // 实时更新箭头位置和旋转
    void Update()
    {
        // 只有当父物体可见时才执行更新逻辑
        if (parentRect.gameObject.activeInHierarchy)
        {
            UpdateArrowPositionAndRotation();
        }
    }

    /// <summary>
    /// 功能 1 & 2: 重新定位父物体，并更新其朝向和偏移。子物体启动动画。
    /// </summary>
    public void UpdateArrowPositionAndRotation()
    {
        if (parentRect == null || arrowRect == null || canvasRect == null) return;

        Vector2 targetCanvasCenter = Vector2.zero;
        bool success = false;

        // 1. 获取目标中心点的 Canvas 局部坐标
        if (targetType == TargetType.World2DObject && World2DTarget != null)
        {
            targetSpriteRenderer = World2DTarget.GetComponent<SpriteRenderer>();
            if (targetSpriteRenderer == null)
            {
                // 如果目标不存在或组件丢失，则视为失败
                success = false;
            }
            else
            {
                success = CalculateWorldCenterToUICoords(targetSpriteRenderer.bounds.center, out targetCanvasCenter);
            }
        }
        else if (targetType == TargetType.UIElement && UITarget != null)
        {
            success = CalculateUICenterToUICoords(UITarget, out targetCanvasCenter);
        }

        if (!success)
        {
            // 如果目标为空或无法转换坐标，停止动画并隐藏箭头
            arrow.DOKill(true);
            parentRect.gameObject.SetActive(false); // 隐藏父物体
            return;
        }

        parentRect.gameObject.SetActive(true); // 显示父物体 (SetNewTarget已经确保了这一点，但这里是容错)

        // 2. 根据朝向计算最终位置和旋转 (计算父物体应处于的起始位置)
        Vector2 finalPosition = targetCanvasCenter;
        float rotationZ = 0f;

        // 获取 Canvas 的缩放因子
        float canvasScaleFactor = canvasRect.localScale.x;
        float scaledOffset = distanceOffset / canvasScaleFactor;

        switch (direction)
        {
            case ArrowDirection.Up:
                // 箭头在目标下方，指向目标中心。默认向下 sprite 需要旋转 180 度指向上方。
                finalPosition.y -= scaledOffset;
                rotationZ = 180f;
                break;
            case ArrowDirection.Down:
                // 箭头在目标上方，指向目标中心。默认向下 sprite 无需旋转 (0度)。
                finalPosition.y += scaledOffset;
                rotationZ = 0f;
                break;
            case ArrowDirection.Left:
                // 箭头在目标右方，指向目标中心。默认向下 sprite 需要旋转 90 度指向左方。
                finalPosition.x -= scaledOffset;
                rotationZ = 90f;
                break;
            case ArrowDirection.Right:
                // 箭头在目标左方，指向目标中心。默认向下 sprite 需要旋转 270 度指向右方。
                finalPosition.x += scaledOffset;
                rotationZ = 270f;
                break;
        }

        // 3. 应用微调偏移
        finalPosition += localExtraOffset;

        // 4. 应用位置和旋转 (应用于父物体)
        parentRect.anchoredPosition = finalPosition;
        parentRect.localRotation = Quaternion.Euler(0f, 0f, rotationZ);

        // 确保子物体从父物体的中心位置开始动画
        // 如果子物体已经被动画移动了，需要先重置到动画的起点
        if (!DOTween.IsTweening(arrow))
        {
            arrowRect.anchoredPosition = Vector2.zero;
        }


        // 5. 启动或更新动画 (仅在子物体上执行局部移动)
        if (enableAnimation)
        {
            StartMoveAnimation();
        }
        else
        {
            // 动画禁用时，停止所有运行中的 Tween 并重置位置
            arrow.DOKill(true);
            arrowRect.anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// 启动一个循环移动动画，使箭头指向目标更明显。
    /// 动画只在子物体 (arrowRect) 上执行，相对于父物体做局部移动。
    /// 运动方向是沿子物体局部 Y 轴 (即指向轴) 来回移动。
    /// </summary>
    private void StartMoveAnimation()
    {
        // 如果动画正在运行，则无需再次启动
        if (DOTween.IsTweening(arrow))
        {
            return;
        }

        // 计算目标局部位置。沿局部 Y 轴移动 animationDistance 距离。
        // 子物体的 (0,0) 是动画的起始/中心点。
        Vector2 endPosition = Vector2.up * animationDistance;

        // 创建来回移动的动画。从当前位置 (Vector2.zero) 移动到 endPosition，然后 SetLoops(Yoyo) 会自动返回。
        arrowRect.DOAnchorPos(endPosition, animationDuration)
            .SetLoops(-1, LoopType.Yoyo) // 无限循环，来回移动
            .SetEase(Ease.InOutSine) // 使用平滑的缓动函数
            .SetTarget(arrow); // 设置 Target，以便 DOKill(arrow) 能正确停止它
    }


    // ----------------------------------------------------------------------
    // 核心功能 3：动态切换目标 (重载方法)
    // ----------------------------------------------------------------------

    /// <summary>
    /// 设置新的目标 (针对 2D 世界物体)
    /// </summary>
    /// <param name="newWorld2DTarget">新的 2D 世界物体 Transform。</param>
    public void SetNewTarget(Transform newWorld2DTarget,ArrowDirection arrowDirection, float distance)
    {
        // 切换目标时，先停止旧的动画
        arrow.DOKill(true);

        World2DTarget = newWorld2DTarget;
        UITarget = null; // 清空另一个引用
        targetType = TargetType.World2DObject;

        if (World2DTarget == null)
        {
            parentRect.gameObject.SetActive(false);
            Debug.LogWarning("SetNewTarget 失败：新的 World2DTarget 为空。");
            return;
        }

        // 尝试获取 SpriteRenderer
        targetSpriteRenderer = World2DTarget.GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer == null)
        {
            parentRect.gameObject.SetActive(false);
            Debug.LogError($"Target '{World2DTarget.name}' 缺少 SpriteRenderer 组件，无法作为 2D 目标。", World2DTarget);
            return;
        }

        // 【修正点 2】：在这里显示父物体
        if (parentRect != null)
        {
            parentRect.gameObject.SetActive(true);
        }
        direction = arrowDirection;
        distanceOffset = distance;
        // 立即更新一次位置
        UpdateArrowPositionAndRotation();
    }

    /// <summary>
    /// 设置新的目标 (针对 UI 元素)
    /// </summary>
    /// <param name="newUITarget">新的 UI 元素 RectTransform。</param>
    public void SetNewTarget(RectTransform newUITarget, ArrowDirection arrowDirection,float distance)
    {
        // 切换目标时，先停止旧的动画
        arrow.DOKill(true);

        UITarget = newUITarget;
        World2DTarget = null; // 清空另一个引用
        targetSpriteRenderer = null; // 清空缓存
        targetType = TargetType.UIElement;

        if (UITarget == null)
        {
            parentRect.gameObject.SetActive(false);
            Debug.LogWarning("SetNewTarget 失败：新的 UITarget 为空。");
            return;
        }

        // 【修正点 3】：在这里显示父物体
        if (parentRect != null)
        {
            parentRect.gameObject.SetActive(true);
        }
        direction = arrowDirection;
        distanceOffset = distance;
        // 立即更新一次位置
        UpdateArrowPositionAndRotation();
    }

    /// <summary>
    /// 隐藏指示箭头
    /// </summary>
    public void HideArrow()
    {
        if (arrow != null)
        {
            arrow.DOKill(true); // 隐藏时停止动画
        }
        if (parentRect != null)
        {
            parentRect.gameObject.SetActive(false);
        }
    }

    // ----------------------------------------------------------------------
    // 核心功能 4：坐标转换助手 (将世界/UI中心点转换为 Canvas 局部坐标)
    // ----------------------------------------------------------------------

    /// <summary>
    /// 将世界坐标转换为 Canvas 的局部坐标。 (已采纳修正)
    /// </summary>
    private bool CalculateWorldCenterToUICoords(Vector3 worldCenter, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;

        // 检查 mainCamera 是否已缓存且有效
        if (mainCamera == null)
        {
            Debug.LogError("无法执行世界坐标转换：mainCamera 为空。");
            return false;
        }

        // 确定用于将屏幕点转换为 Canvas 局部点的 eventCamera
        // Screen Space - Overlay 时为 null，其他模式为 mainCamera。
        Canvas parentCanvas = canvasRect.GetComponentInParent<Canvas>();
        Camera eventCameraForRect = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera;

        // 1. 将世界中心点转换为屏幕像素坐标
        // 始终使用主摄像机 (mainCamera) 来进行世界到屏幕的转换
        Vector3 screenCenter = mainCamera.WorldToScreenPoint(worldCenter);

        // 2. 将屏幕中心点转换为 Canvas 上的局部位置 (localPoint)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenCenter,
                eventCameraForRect, // 这里使用 eventCameraForRect (Overlay模式为null)
                out localPoint))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取 UI 元素的中心点，并将其转换为父级 Canvas 的局部坐标。
    /// </summary>
    private bool CalculateUICenterToUICoords(RectTransform target, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;

        // 确定用于坐标转换的相机 (Overlay 模式下为 null, Camera/World 模式下为 mainCamera)
        Canvas parentCanvas = canvasRect.GetComponentInParent<Canvas>();
        Camera eventCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera;

        // --- 核心修正：使用 GetWorldCorners 获取 UI 元素的真实视觉中心 ---
        // 角标：0=左下, 1=左上, 2=右上, 3=右下
        target.GetWorldCorners(targetCorners);
        Vector3 targetWorldCenter = (targetCorners[0] + targetCorners[2]) * 0.5f;

        // 关键修复: 确保使用正确的 eventCamera (Overlay时为null) 来将世界坐标转换为屏幕坐标
        Vector3 screenCenter = RectTransformUtility.WorldToScreenPoint(eventCamera, targetWorldCenter);

        // 将屏幕中心点转换为 Canvas 上的局部位置 (localPoint)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenCenter,
            eventCamera,
            out localPoint))
        {
            return true;
        }

        return false;
    }
}