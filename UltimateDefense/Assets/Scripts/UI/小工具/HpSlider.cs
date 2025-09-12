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
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    /// <param name="currentShield"></param>
    /// <param name="maxShield"></param>
    /// <param name="targetSlider">显示哪个条，默认是先显示护盾条，再显示血条</param>
    public void UpdateBar(int currentHp,int maxHp,int currentShield, int maxShield,bool isRealDamage = false)
    {
        //有护盾
        if (!isRealDamage)
        {
            if (currentShield > 0)
            {
                shieldSlider.maxValue = maxShield;
                shieldSlider.gameObject.SetActive(true);
                hpSlider.gameObject.SetActive(false);
                shieldSlider.value = currentShield;
                return;
            }
            else
            {
                hpSlider.maxValue = maxHp;
                shieldSlider.gameObject.SetActive(false);
                hpSlider.gameObject.SetActive(true);
                hpSlider.value = currentHp;
            }
        }
        else
        {
            hpSlider.maxValue = maxHp;
            shieldSlider.gameObject.SetActive(false);
            hpSlider.gameObject.SetActive(true);
            hpSlider.value = currentHp;
        }
    }
}
