using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "CookingGame/Customer Data")]
public class CustomerData : ScriptableObject
{
    [Header("长相")]
    public Sprite sprite;
    [Header("饱食度设置")]
    public float maxSatiety = 100f;
    public float satietyDecreasePerSec = 2f;

    [Header("耐心值设置")]
    public float maxPatience = 100f;
    public float patienceDecreasePerSec = 3f;

    [Header("小费比例")]
    public float fullPatienceTipMultiplier = 1.5f;   // 耐心 > 70% 时 +50%

    [Header("进店 -> 座位期间")]
    public string[] enterToSeat;

    [Header("耐心值高")]
    public string[] highPatience;

    [Header("耐心值中等")]
    public string[] midPatience;

    [Header("耐心值低")]
    public string[] lowPatience;

    [Header("耐心耗尽")]
    public string[] patienceZero;

    [Header("玩家扔生食物")]
    public string[] foodThrown;

    [Header("吃到喜欢的食物")]
    public string[] likedFood;

    [Header("店里有关系顾客时")]
    public string[] relationPresent;
}
