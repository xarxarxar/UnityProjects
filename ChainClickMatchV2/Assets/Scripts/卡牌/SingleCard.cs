using DG.Tweening;
using HighlightPlus2D;
using UnityEngine;

/// <summary>
/// 单张卡牌：持有渲染器引用与卡牌身份（章节 / 关卡 / 种类）。
/// spriteRenderer 未在 prefab 中赋值时，自动回退查找子物体 icon 的 SpriteRenderer。
/// </summary>
public class SingleCard : MonoBehaviour
{
    [Tooltip("卡牌种类 sprite 渲染器（prefab 中未赋值时自动取子物体 icon 的 SpriteRenderer）")]
    public SpriteRenderer spriteRenderer;

    [Header("卡牌身份")]
    [Tooltip("章节（1~5，对应 Assets/Sprites/icons 下各章节文件夹）")]
    public int chapter = 1;

    [Tooltip("关卡")]
    public int level = 1;

    [Tooltip("卡牌种类（1~16，对应章节内 1.png~16.png）")]
    public int type = 1;

    [Header("棋盘位置")]
    [Tooltip("所在行（0 起，由 CardGridLayout 生成时写入）")]
    public int boardRow = -1;

    [Tooltip("所在列（0 起，由 CardGridLayout 生成时写入）")]
    public int boardCol = -1;

    public HighlightEffect2D highlightEffect;

    public int maxEffectStrength = 5;

    public Transform shakeTransform;

    //卡牌背景
    public SpriteRenderer tileSpriteRenderer;

    [Header("呼吸提示效果")]
    [Tooltip("呼吸缩放的下限（提示道具用）")]
    public float breathMinScale = 0.9f;

    [Tooltip("呼吸缩放的上限（提示道具用）")]
    public float breathMaxScale = 1.1f;

    [Tooltip("呼吸单程时长（秒，提示道具用）")]
    public float breathDuration = 0.4f;

    /// <summary>当前抖动动画引用，重复触发时先杀掉旧动画避免叠加。</summary>
    private Tweener _shakeTween;

    /// <summary>当前呼吸动画引用，重复触发时先杀掉旧动画避免叠加。</summary>
    private Tweener _breathTween;

    private void Awake()
    {
        // 解析 spriteRenderer（prefab 未赋值时自动回退查找）
        if (spriteRenderer == null)
        {
            // 优先取子物体 icon 的 SpriteRenderer（卡牌种类图标所在）
            Transform icon = transform.Find("icon");
            if (icon != null)
            {
                spriteRenderer = icon.GetComponent<SpriteRenderer>();
            }

            // 兜底：取子物体中 sortingOrder 最高的 SpriteRenderer
            if (spriteRenderer == null)
            {
                SpriteRenderer best = null;
                foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (best == null || sr.sortingOrder > best.sortingOrder)
                    {
                        best = sr;
                    }
                }
                spriteRenderer = best;
            }
        }

        // 解析 tileSpriteRenderer（卡牌背景，prefab 未赋值时自动取子物体 Tile 的 SpriteRenderer）
        if (tileSpriteRenderer == null)
        {
            Transform tile = transform.Find("Tile");
            if (tile != null)
            {
                tileSpriteRenderer = tile.GetComponent<SpriteRenderer>();
            }
        }

        // 初始化：HighLightEffect 与 ShakeSway 均复位为 0（关闭选中光效、卡牌恢复正着）
        InitCard();
    }

    /// <summary>鼠标点击卡牌（依赖根物体上的 BoxCollider2D）：交由 CardRoot 上的 CardGridLayout 处理待消除列表。</summary>
    private void OnMouseDown()
    {
        CardGridLayout board = GetComponentInParent<CardGridLayout>();
        if (board != null)
        {
            board.OnCardClicked(this);
        }
    }

    //修改卡牌被点击时的效果强度：strength=0 表示不在待消除列表（取消光效并恢复正着），>0 表示在列表中（光效强度随列表数量变化）
    public void ChangeChooseEffect(int strength)
    {
        strength=Mathf.Clamp( strength, 0, maxEffectStrength );

        ShakeSway(strength);
        HighLightEffect(strength);
    }

    /// <summary>初始化卡牌：HighLightEffect 与 ShakeSway 均置为 0（关闭选中光效、恢复正着状态），呼吸关闭并复位缩放。</summary>
    public void InitCard()
    {
        HighLightEffect(0);
        ShakeSway(0);
        BreathEffect(false);
    }

    /// <summary>
    /// 呼吸提示效果：对 shakeTransform（未赋值时兜底用卡牌自身）的 scale 在
    /// breathMinScale ~ breathMaxScale 之间来回缩放（DOTween 无限 Yoyo）。
    /// on=true 开启呼吸；on=false 停止呼吸并复位 scale 为 1。
    /// </summary>
    public void BreathEffect(bool on)
    {
        Transform target = shakeTransform != null ? shakeTransform : transform;

        if (!on)
        {
            _breathTween?.Kill();
            _breathTween = null;
            target.localScale = Vector3.one;
            return;
        }

        // 已处于呼吸状态（动画仍在播放）则保持现状，不重新开始
        if (_breathTween != null && _breathTween.IsActive())
        {
            return;
        }

        _breathTween?.Kill();
        // 从下限开始，缩放到上限后 Yoyo 返回，无限循环形成呼吸感
        target.localScale = new Vector3(breathMinScale, breathMinScale, 1f);
        _breathTween = target.DOScale(new Vector3(breathMaxScale, breathMaxScale, 1f), breathDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>设置卡牌背景（tile）的 Order In Layer（sortingOrder）。tileSpriteRenderer 未赋值时静默跳过。</summary>
    public void SetTileSortingOrder(int order)
    {
        if (tileSpriteRenderer != null)
        {
            tileSpriteRenderer.sortingOrder = order;
        }
    }

    public void HighLightEffect(int strength)
    {
        if (highlightEffect == null)
        {
            return;
        }
        if(strength <= 0)
        {
            highlightEffect.highlighted = false;
            return;
        }
        // strength=0 时关闭选中光效，>0 时点亮并随列表数量增强
        highlightEffect.highlighted = true;
        highlightEffect.glow = 0.15f * strength;
        highlightEffect.glowWidth=0.5f+0.2f*strength;
    }

    /// <summary>
    /// 左右摇摆（持续）：对 shakeTransform 绕 Z 轴在 +strength ~ -strength 之间来回摆动，无限循环（一直摇摆）。
    /// strength 控制单侧倾斜角度（度），duration 控制单边摆动耗时（秒，默认 0.5）。
    /// strength <= 0 时不摇摆，恢复初始正着（直立）状态；连续调用会先停止上一次摆动再播放新的。
    /// shakeTransform 未赋值时兜底用卡牌自身 transform，避免空引用。
    /// </summary>
    public void ShakeSway(float strength, float duration = 0.5f)
    {
        Transform target = shakeTransform != null ? shakeTransform : transform;

        _shakeTween?.Kill();

        // strength <= 0：不摇摆，恢复初始正着（直立）状态
        if (strength <= 0f)
        {
            _shakeTween = null;
            target.localRotation = Quaternion.identity;
            return;
        }

        // 先摆到右侧 +strength，再在 +strength ~ -strength 之间来回摆动（Yoyo 无限循环 = 一直摇摆）
        target.localRotation = Quaternion.Euler(0f, 0f, 5+1.2f*strength);
        _shakeTween = target.DOLocalRotate(new Vector3(0f, 0f, -5 - 1.2f * strength), duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        _shakeTween?.Kill();
        _shakeTween = null;
        _breathTween?.Kill();
        _breathTween = null;
    }
}
