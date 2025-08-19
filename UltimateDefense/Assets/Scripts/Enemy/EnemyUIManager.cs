using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIManager : MonoBehaviour
{
    public static EnemyUIManager Instance;

    [Header("UI 设置")]
    public Canvas canvas;
    public GameObject enemyUIPrefab;
    public float hideDelay = 2f; // 血条隐藏延迟秒数

    class EnemyUIData
    {
        public Transform enemy;
        public RectTransform container;
        public Slider healthSlider;
        public GameObject sliderBackground;
        public GameObject sliderFill;
        public Text hpText;
        public Vector3 worldOffset;
        public bool hideWhenOffscreen;
        public float lastHitTime;  // 上次受击时间
    }

    private List<EnemyUIData> uiList = new List<EnemyUIData>();
    private Camera mainCam;

    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        float now = Time.time;

        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            if (data.enemy == null)
            {
                Destroy(data.container.gameObject);
                uiList.RemoveAt(i);
                i--;
                continue;
            }

            // 位置更新
            Vector3 worldPos = data.enemy.position + data.worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

            if (data.hideWhenOffscreen && screenPos.z < 0f)
            {
                data.container.gameObject.SetActive(false);
                continue;
            }

            Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, (Vector2)screenPos, uiCam, out Vector2 localPoint))
            {
                data.container.anchoredPosition = localPoint;
            }

            // 血条显示/隐藏逻辑
            if (data.healthSlider != null)
            {
                bool shouldShow = (now - data.lastHitTime) < hideDelay;
                data.healthSlider.gameObject.SetActive(shouldShow);
            }
        }
    }

    /// <summary>
    /// 注册敌人 UI
    /// </summary>
    public void RegisterEnemyUI(Enemy enemy, Vector3 offset, bool hideWhenOffscreen = true)
    {
        GameObject uiObj = Instantiate(enemyUIPrefab, canvas.transform);
        uiObj.transform.SetAsFirstSibling();
        var slider = uiObj.GetComponentInChildren<Slider>();
        var text = uiObj.GetComponentInChildren<Text>();
        GameObject background = slider.transform.Find("Background").gameObject;
        GameObject fill = slider.transform.Find("Fill Area").Find("Fill").gameObject;

        // 血条默认隐藏
        if (slider != null)
            slider.gameObject.SetActive(false);

        EnemyUIData newData = new EnemyUIData
        {
            enemy = enemy.transform,
            container = uiObj.GetComponent<RectTransform>(),
            healthSlider = slider,
            sliderBackground= background,
            sliderFill= fill,
            hpText = text,
            worldOffset = offset,
            hideWhenOffscreen = hideWhenOffscreen,
            lastHitTime = -999f
        };

        // 初始化血条和血量文本
        if (slider != null)
            slider.value = Mathf.Clamp01(enemy.CurrentHP.Value / enemy.maxHP.Value);

        if (text != null)
            text.text = $"{Mathf.RoundToInt(enemy.CurrentHP.Value)}";

        uiList.Add(newData);
    }

    /// <summary>
    /// 更新敌人血量（受击时调用）
    /// </summary>
    public void UpdateEnemyHealth(Enemy enemy)
    {
        var data = uiList.Find(d => d.enemy == enemy.transform);
        if (data != null)
        {
            // 更新血条
            if (data.healthSlider != null)
            {
                if (enemy.CurrentShield.Value > 0)
                {
                    data.sliderFill.GetComponent<Image>().color = new Color32(70,130,180,255);
                    data.sliderBackground.GetComponent<Image>().color = new Color32(255,165,0,255);

                    data.healthSlider.value = Mathf.Clamp01((float)enemy.CurrentShield.Value/enemy.maxShield.Value);
                }
                else
                {
                    data.sliderFill.GetComponent<Image>().color = new Color32(255, 165, 0, 255);
                    data.sliderBackground.GetComponent<Image>().color = new Color32(94, 94, 94, 255);

                    data.healthSlider.value = Mathf.Clamp01((float)enemy.CurrentHP.Value / enemy.maxHP.Value);
                }
            }

            // 更新血量文本
            if (data.hpText != null)
            {
                data.hpText.text = $"{Mathf.RoundToInt(enemy.CurrentHP.Value)}";
            }

            // 记录受击时间，使血条显示
            data.lastHitTime = Time.time;
        }
    }

    public void RemoveEnemyUI(Transform enemy)
    {
        var data = uiList.Find(d => d.enemy == enemy);
        if (data != null)
        {
            Destroy(data.container.gameObject);
            uiList.Remove(data);
        }
    }
}
