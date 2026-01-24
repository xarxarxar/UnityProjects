using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomerUIManager : MonoBehaviour
{
    public static CustomerUIManager Instance;
    private Camera mainCam;
    public Canvas canvas;
    public GameObject customerUIPrefab;
    private ObjectPool<RectTransform> customerUIPool;

    private float dialogShowDuration = 5f; // 每段对白显示时长


    [System.Serializable]
    class CustomerUIData
    {
        public Transform customer;
        public Slider satietySlider;//饱食度slider
        public Slider patienceSlider;//耐心值slider
        public RectTransform container;
        public Vector3 worldOffset;
        public Transform dialogBox;//说话框
        public Text dialogText;//说话文字
        public bool isDialogShowing = false;//是否正在说话中 
        public float dialogTimer = 0f; //新增：对话计时器
    }
    private List<CustomerUIData> uiList = new List<CustomerUIData>();
    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    private void Start()
    {
        LevelManager.OnInit += Init;
        LevelManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        if (customerUIPool == null)
        {
            customerUIPool = new ObjectPool<RectTransform>(customerUIPrefab.GetComponent<RectTransform>(), 10, canvas.transform);
        }
    }

    //关卡结束
    private void OnEndBattle()
    {

    }


    void LateUpdate()
    {

        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            if (data.customer == null)
            {
                customerUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
                uiList.RemoveAt(i);
                i--;
                continue;
            }

            // 位置更新
            Vector3 worldPos = data.customer.position + data.worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

            Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, (Vector2)screenPos, uiCam, out Vector2 localPoint))
            {
                data.container.anchoredPosition = localPoint;
            }

            // 【对白计时与隐藏】
            if (data.isDialogShowing && !LevelManager.Instance.isPaused)
            {
                if (!LevelManager.Instance.isPaused)
                {
                    data.dialogTimer -= Time.deltaTime; // 使用 Time.deltaTime，暂停时不累加
                    if (data.dialogTimer <= 0f)
                    {
                        HideDialog(data);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 注册顾客 UI
    /// </summary>
    public void RegisterCustomerUI(Customer customer, Vector3 offset)
    {
        if (uiList.Any(d => d.customer == customer.transform))
        {
            Debug.LogWarning($"重复注册 {customer.name} 的UI，忽略");
            return;
        }

        GameObject uiObj = customerUIPool.Get().gameObject;
        uiObj.transform.SetAsFirstSibling();

        CustomerUIData newData = new CustomerUIData
        {
            customer = customer.transform,
            container = uiObj.GetComponent<RectTransform>(),
            satietySlider = uiObj.transform.Find("饱食度Slider").GetComponent<Slider>(),
            patienceSlider = uiObj.transform.Find("耐心值Slider").GetComponent<Slider>(),
            worldOffset = offset,
            dialogBox = uiObj.transform.Find("对话框"),
            dialogText = uiObj.transform.Find("对话框/Text").GetComponent<Text>(),
            isDialogShowing =false,
            dialogTimer = 0f
        };
        // 默认隐藏对白框
        newData.dialogBox.gameObject.SetActive(false);


        uiList.Add(newData);
    }

    /// <summary>
    /// 移除一个顾客的UI
    /// </summary>
    public void RemoveCustomerUI(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data != null)
        {
            customerUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }
    }

    /// <summary>
    /// 更新顾客的 Slider（饱食度与耐心）
    /// Slider 被隐藏时不更新
    /// </summary>
    public void UpdateCustomerSlider(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data == null) return;

        // --- Slider 不显示就不更新 ---
        if (!data.satietySlider.gameObject.activeSelf &&
            !data.patienceSlider.gameObject.activeSelf)
        {
            return;
        }

        // 显示了的就更新
        if (data.satietySlider.gameObject.activeSelf)
            data.satietySlider.value = customer.currentSatiety;

        if (data.patienceSlider.gameObject.activeSelf)
        {
            data.patienceSlider.value = customer.currentPatience;

            float ratio = customer.currentPatience / customer.maxPatience;
            Image fill = data.patienceSlider.transform
                .Find("Fill Area/Fill")
                .GetComponent<Image>();

            if (ratio <= CustomerManager.Instance.LowPatienceThreshold)
                fill.color = new Color32(229, 115, 115, 255);   // Red
            else if (ratio <= CustomerManager.Instance.MidPatienceThreshold)
                fill.color = new Color32(255, 202, 40, 255);   // Yellow
            else
                fill.color = new Color32(129, 199, 132, 255);   // Green
        }
    }

    /// <summary>
    /// 仅显示顾客的 Slider（饱食度 + 耐心）
    /// 不影响对话框
    /// </summary>
    public void ShowCustomerSlider(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data != null)
        {
            data.satietySlider.gameObject.SetActive(true);
            data.patienceSlider.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 仅隐藏顾客的 Slider（饱食度 + 耐心）
    /// 不影响对话框
    /// </summary>
    public void HideCustomerSlider(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data != null)
        {
            data.satietySlider.gameObject.SetActive(false);
            data.patienceSlider.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 判断指定顾客当前是否正在说话
    /// </summary>
    /// <param name="customer">顾客</param>
    /// <returns>true = 正在说话，false = 没有说话</returns>
    public bool IsCustomerTalking(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data != null)
        {
            return data.isDialogShowing;
        }
        return false; // 没有找到顾客的UI，视为未说话
    }
    /// <summary>
    /// 显示对白
    /// </summary>
    public void ShowCustomerDialog(Customer customer, string txt, bool interrupt = false)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data == null) return;

        // 正在说话 && 不强制打断 -> 忽略
        if (data.isDialogShowing && !interrupt)
            return;

        // 强制打断：立即隐藏旧对白
        if (interrupt && data.isDialogShowing)
        {
            HideDialog(data);
        }

        data.dialogText.text = txt;
        data.dialogBox.gameObject.SetActive(true);
        data.isDialogShowing = true;
        data.dialogTimer = dialogShowDuration; // 重置计时
    }

    /// <summary>
    /// 隐藏对白（内部调用）
    /// </summary>
    private void HideDialog(CustomerUIData data)
    {
        data.dialogBox.gameObject.SetActive(false);
        data.isDialogShowing = false;
        data.dialogTimer = 0f;
    }

    /// <summary>
    /// 隐藏指定顾客对白（对外 API）
    /// </summary>
    public void HideCustomerDialog(Customer customer)
    {
        var data = uiList.Find(d => d.customer == customer.transform);
        if (data != null)
            HideDialog(data);
    }
}
