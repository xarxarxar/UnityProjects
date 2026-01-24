using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUIManager : MonoBehaviour
{
    public static IngredientUIManager Instance;
    private Camera mainCam;
    public Canvas canvas;
    public GameObject ingredientUIPrefab;
    private ObjectPool<RectTransform> ingredientUIPool;
    private List<IngredientUIData> uiList = new List<IngredientUIData>();
    [System.Serializable]
    class IngredientUIData
    {
        public Transform ingredient;
        public Slider slider;
        public RectTransform container;
        public Vector3 worldOffset;
    }

    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    private void Start()
    {
        if (ingredientUIPool == null)
        {
            ingredientUIPool = new ObjectPool<RectTransform>(ingredientUIPrefab.GetComponent<RectTransform>(), 10, canvas.transform);
        }
    }

    void LateUpdate()
    {

        for (int i = 0; i < uiList.Count; i++)
        {
            var data = uiList[i];
            if (data.ingredient == null)
            {
                ingredientUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());

                uiList.RemoveAt(i);
                i--;
                continue;
            }

            // 位置更新
            Vector3 worldPos = data.ingredient.position + data.worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

            Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, (Vector2)screenPos, uiCam, out Vector2 localPoint))
            {
                data.container.anchoredPosition = localPoint;
            }

        }
    }

    /// <summary>
    /// 注册食材 UI
    /// </summary>
    public void RegisterIngredientUI(Ingredient ingredient, Vector3 offset)
    {
        if (uiList.Any(d => d.ingredient == ingredient.transform))
        {
            Debug.LogWarning($"重复注册 {ingredient.name} 的UI，忽略");
            return;
        }

        GameObject uiObj = ingredientUIPool.Get().gameObject;
        uiObj.transform.SetAsFirstSibling();

        IngredientUIData newData = new IngredientUIData
        {
            ingredient = ingredient.transform,
            container = uiObj.GetComponent<RectTransform>(),
            slider = uiObj.transform.Find("熟食度Slider").GetComponent<Slider>(),
            worldOffset = offset,
        };
        uiList.Add(newData);
    }

    /// <summary>
    /// 移除一个食材的UI
    /// </summary>
    public void RemoveIngredientUI(Ingredient ingredient)
    {
        var data = uiList.Find(d => d.ingredient == ingredient.transform);
        if (data != null)
        {
            ingredientUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }

    }

    /// <summary>
    /// 更新敌人血量（受击时调用）,是否是真伤，是否是暴击
    /// </summary>
    public void UpdateIngredientUI(Ingredient ingredient)
    {
        var data = uiList.Find(d => d.ingredient == ingredient.transform);
        if (data != null)
        {
            Image fill = data.slider.transform
                .Find("Fill Area/Fill")
                .GetComponent<Image>();

            if (ingredient.currentCook >=100  )
            {
                data.slider.value = ingredient.currentCook-100;
                fill.color = new Color32(229, 115, 115, 255); //Red
            }
            else
            {
                data.slider.value = ingredient.currentCook;
                fill.color = new Color32(129, 199, 132, 255); // Green
            }
        }

    }
}
