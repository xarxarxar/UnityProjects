using UnityEngine;
using UnityEngine.UI;

public class EnemyUIFollower : MonoBehaviour
{
    [Header("跟随目标与偏移")]
    public Transform target;                     // 敌人（或敌人头顶的空物体）
    public Vector3 worldOffset = new Vector3(0, 1.6f, 0); // 敌人头顶的世界偏移

    [Header("UI引用")]
    public Canvas canvas;                        // 该UI所属Canvas
    public RectTransform container;              // 父物体（同时包含血条与等级文本）
    public Slider healthSlider;                  // 血条（建议 0~1）
    public Text levelText;                       // 等级文本（或换成TMP_Text）

    [Header("可选：屏幕空间下微调像素偏移")]
    public Vector2 screenSpaceOffset = new Vector2(0, 0);

    [Header("可选：不在屏幕内时隐藏")]
    public bool hideWhenOffscreen = true;

    Camera mainCam;

    void Awake()
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (container == null) container = transform as RectTransform;
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || canvas == null || container == null || mainCam == null) return;

        // World Space Canvas 直接用世界坐标
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            container.position = target.position + worldOffset;
            if (hideWhenOffscreen)
            {
                // 简单视野检测：在相机前方才显示
                bool inFront = Vector3.Dot(mainCam.transform.forward, (container.position - mainCam.transform.position)) > 0f;
                container.gameObject.SetActive(inFront);
            }
            return;
        }

        // Screen Space（Overlay/Camera）：世界->屏幕->Canvas本地坐标
        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position + worldOffset);

        // 在相机后面则隐藏
        if (hideWhenOffscreen && screenPos.z < 0f)
        {
            container.gameObject.SetActive(false);
            return;
        }
        else
        {
            container.gameObject.SetActive(true);
        }

        Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        RectTransform canvasRect = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, (Vector2)screenPos, uiCam, out Vector2 localPoint))
        {
            container.anchoredPosition = localPoint + screenSpaceOffset;
        }
    }

    /// <summary>把 current/max 映射到 0~1，并更新血条</summary>
    public void UpdateHealth(float current, float max)
    {
        if (healthSlider == null) return;
        if (max <= 0f) { healthSlider.value = 0f; return; }

        // 如果你的Slider是0~1，直接赋归一化值；如果是0~max，请改：healthSlider.maxValue = max; healthSlider.value = current;
        healthSlider.value = Mathf.Clamp01(current / max);
    }

    /// <summary>更新等级文本</summary>
    public void UpdateLevel(int level)
    {
        if (levelText != null)
            levelText.text = $"Lv.{level}";
    }
}
