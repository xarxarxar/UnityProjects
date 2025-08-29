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
    /// 初始化血条
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    /// <param name="currentShield"></param>
    public void Init(int currentHp, int maxHp, int currentShield)
    {
        shieldSlider.maxValue = currentShield;
        hpSlider.maxValue = maxHp;
        UpdateBar(currentHp, maxHp, currentShield);
    }

    /// <summary>
    /// 更新血条
    /// </summary>
    /// <param name="currentHp">当前血量</param>
    /// <param name="maxHp">最大血量</param>
    /// <param name="currentShield">当前护盾值</param>
    public void UpdateBar(int currentHp,int maxHp,int currentShield)
    {
        //有护盾
        if (currentShield > 0)
        {
            shieldSlider.gameObject.SetActive(true);
            hpSlider.gameObject.SetActive(false);
            shieldSlider.value = currentShield;
            return;
        }
        shieldSlider.gameObject.SetActive(false);
        hpSlider.gameObject.SetActive(true);
        hpSlider.value = currentHp;

    }
}
