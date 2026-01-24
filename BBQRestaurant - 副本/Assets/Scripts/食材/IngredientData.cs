using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "CookingGame/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    [Header("基础属性")]
    public string ingredientName;     // 名称

    [Header("数值属性")]
    public int satiety;               // 饱食度
    public int costPrice;             // 售价

    [Header("烤制时间 (秒)")]
    public float cookTime;            // 烧熟所需时间,食材的currentCook从0到100所需的时间
    public float burnTime;            // 烧焦所需时间（从熟开始计时，食材的currentCook从100到200所需的时间

    [Header("资源")]
    public Sprite ingredientSprite;   // 食材的 Sprite 图
}
