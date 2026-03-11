using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    class CardUI
    {
        public SingleCard singleCard;
        public RectTransform container;
        public Slider countDownSlider;
        public Vector3 worldOffset;
    }

    [SerializeField] private List<CardUI> uiList = new List<CardUI>();


    void LateUpdate()
    {

        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            if (data.singleCard == null)
            {
                PoolManager.Instance.cardDataUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());

                uiList.RemoveAt(i);
                i--;
                continue;
            }

            // 位置更新
            Vector3 worldPos = data.singleCard.transform.position + data.worldOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            data.container.transform.position = screenPos;
        }
    }

    /// <summary>
    /// 注册卡牌信息 UI
    /// </summary>
    public void RegisterCardUI(SingleCard card, Vector3 offset)
    {
        if (uiList.Any(d => d.singleCard== card))
        {
            Debug.LogWarning($"重复注册UI，忽略");
            return;
        }
        RectTransform rectTransform = PoolManager.Instance.cardDataUIPool.Get();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(card.transform.position);
        rectTransform.position = screenPos;

        GameObject uiObj = rectTransform.gameObject;
        Slider slider = uiObj.GetComponentInChildren<Slider>(true);

        CardUI newData = new CardUI
        {
            singleCard = card.GetComponent<SingleCard>(),
            container = uiObj.GetComponent<RectTransform>(),
            countDownSlider = slider,
            worldOffset = offset,
        };

        // 初始化血条和血量文本
        if (slider != null)
        {
            slider.value = 1;
        }

        uiList.Add(newData);
    }

    /// <summary>
    /// 更新卡牌信息UI
    /// </summary>
    public void UpdateCardUI(SingleCard card, float value)
    {
        var data = uiList.Find(d => d.singleCard == card);
        if (data != null)
        {
            // 更新血条
            if (data.countDownSlider != null)
            {
                data.countDownSlider.value = value;
            }
        }

    }
    /// <summary>
    /// 移除卡牌信息UI
    /// </summary>
    /// <param name="role"></param>
    public void RemoveCardUI(SingleCard card)
    {
        var data = uiList.Find(d => d.singleCard == card);
        if (data != null)
        {
            PoolManager.Instance.cardDataUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }

    }

    /// <summary>
    /// 移除所有卡牌信息UI
    /// </summary>
    /// <param name="role"></param>
    public void RemoveAllCardUI()
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            PoolManager.Instance.cardDataUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }
    }
}
