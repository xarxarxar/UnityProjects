using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 双血条（护盾条和血条）
/// </summary>
public class HpSlider : MonoBehaviour
{
    [Header("UI 引用")]
    public Slider shieldSlider;       // 护盾条
    public Slider hpSlider;       // 血条


    /// <summary>
    /// 更新血条
    /// </summary>
    /// <param name="currentHp">当前血量</param>
    /// <param name="maxHp">最大血量</param>
    /// <param name="currentShield">当前护盾值</param>
    public void UpdateBar(int currentHp,int maxHp,int currentShield, int maxShield)
    {
        //有护盾
        if (currentShield > 0)
        {
            shieldSlider.maxValue = maxShield;
            shieldSlider.gameObject.SetActive(true);
            hpSlider.gameObject.SetActive(false);
            shieldSlider.value = currentShield;
            return;
        }
        hpSlider.maxValue = maxHp;
        shieldSlider.gameObject.SetActive(false);
        hpSlider.gameObject.SetActive(true);
        hpSlider.value = currentHp;

    }
}
