using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyUIManager : MonoBehaviour
{
    public static EnemyUIManager Instance;

    [Header("UI 设置")]
    public Canvas canvas;
    public GameObject enemyUIPrefab;
    public float hideDelay = 2f; // 血条隐藏延迟秒数
    public Image crossHair;//准心
    private ObjectPool<RectTransform> enemyUIPool;


    class EnemyUIData
    {
        public Transform enemy;
        public RectTransform container;
        public HpSlider healthSlider;
        public Button crossHairButton;
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

    private void Start()
    {
        if(enemyUIPool == null)
        {
            enemyUIPool = new ObjectPool<RectTransform>(enemyUIPrefab.GetComponent<RectTransform>(), 10, canvas.transform);
        }

        crossHair.gameObject.SetActive(false);
        EnemyManager.Instance.CurrentClickedEnemy.OnValueChanged += OnCurrentClickedEnemyChanged;
    }

    void LateUpdate()
    {
        float now = Time.time;

        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            if (data.enemy == null)
            {
                enemyUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
                
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
        GameObject uiObj = enemyUIPool.Get().gameObject;
        uiObj.transform.SetAsFirstSibling();
        //var slider = uiObj.GetComponentInChildren<Slider>();
        var text = uiObj.GetComponentInChildren<Text>();
        HpSlider hpSlider= uiObj.GetComponentInChildren<HpSlider>();
        Button crosshairbutton= uiObj.GetComponentInChildren<Button>();

        // 血条默认隐藏
        if (hpSlider != null)
            hpSlider.gameObject.SetActive(false);

        EnemyUIData newData = new EnemyUIData
        {
            enemy = enemy.transform,
            container = uiObj.GetComponent<RectTransform>(),
            healthSlider = hpSlider,
            crossHairButton = crosshairbutton,
            hpText = text,
            worldOffset = offset,
            hideWhenOffscreen = hideWhenOffscreen,
            lastHitTime = -999f
        };

        uiObj.GetComponent<RectTransform>().sizeDelta = enemy.transform.localScale * 80;

        // 初始化血条和血量文本
        if (hpSlider != null)
            hpSlider.Init(enemy.CurrentHP.Value,enemy.maxHP.Value,enemy.CurrentShield.Value);

        if (text != null)
        {
            int hp = enemy.CurrentHP.Value;
            int display = (hp == 0) ? 0 : ((hp - 1) / 10 + 1);
            text.text = $"{display}";
        }
        crosshairbutton.onClick.RemoveAllListeners();
        crosshairbutton.onClick.AddListener(() =>
        {
            OnEnemyClicked(enemy);
        });

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
                data.healthSlider.UpdateBar(enemy.CurrentHP.Value, enemy.maxHP.Value, enemy.CurrentShield.Value);
            }

            // 更新血量文本
            if (data.hpText != null)
            {
                int hp = enemy.CurrentHP.Value;
                int display = (hp == 0) ? 0 : ((hp - 1) / 10 + 1);
                data.hpText.text = display.ToString();
            }

            // 记录受击时间，使血条显示
            data.lastHitTime = Time.time;
            data.container.sizeDelta = enemy.transform.localScale * 80;
        }
        
    }

    public void RemoveEnemyUI(Transform enemy)
    {
        var data = uiList.Find(d => d.enemy == enemy);
        if (data != null)
        {
            enemyUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }
        if (crossHair.transform.IsChildOf(data.container))
        {
            crossHair.gameObject.SetActive(false);
        }
    }

    private void OnCurrentClickedEnemyChanged(Enemy enemy)
    {
        Debug.Log("点击的对象发生变化1");
        if (enemy == null)
        {
            crossHair.gameObject.SetActive(false);
            return;
        }
        
        var data = uiList.Find(d => d.enemy == enemy.transform);
        if (data != null)
        {
            
            crossHair.transform.SetParent(data.container);
            // 然后重置 RectTransform
            var rect = crossHair.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            // 先停止 crossHair 上之前的 DOTween 动画
            crossHair.transform.DOKill();
            // 重置一下scale，避免受到父节点缩放影响
            crossHair.transform.localScale = Vector3.one * 1.5f;
            crossHair.gameObject.SetActive(true);
            // 用DOTween做缩放动画
            crossHair.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void OnEnemyClicked(Enemy enemy)
    {
        Debug.Log("enemy被点击");
        EnemyManager.Instance.CurrentClickedEnemy.Value = enemy;
        if (EnemyManager.Instance.EnemiesInRange.Contains(enemy))
        {
            EnemyManager.Instance.CurrentTargerEnemy = enemy;
        }
    }
}
