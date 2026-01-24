using DG.Tweening;
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
    // Q 弹相关
    private float squashAmount = 0.6f;  // 压缩比例
    private float stretchAmount = 1.4f; // 拉伸比例
    private float squashDuration = 0.2f; // Q 弹单程时长
    private Vector3 originalScale = Vector3.one;

    //闪红
    private Color shieldOriginalColor = new Color32(255,165,0,255);
    private Color hpOriginalColor = new Color32(135,188,79,255);


    /// <summary>
    /// 更新血条
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    /// <param name="currentShield"></param>
    /// <param name="maxShield"></param>
    /// <param name="isRealDamage">是否是真伤</param>
    /// <param name="isCri">是否暴击</param>
    public void UpdateBar(int currentHp,int maxHp,int currentShield, int maxShield,bool isRealDamage = false,bool isCri=false)
    {
        //有护盾
        if (!isRealDamage)
        {
            if (currentShield > 0)
            {
                shieldSlider.maxValue = maxShield;
                shieldSlider.gameObject.SetActive(true);
                if (isCri)
                {
                    FlashRed(shieldSlider.fillRect.transform.GetComponent<Image>(),shieldOriginalColor);
                    JellySquash(shieldSlider.transform);
                }

                hpSlider.gameObject.SetActive(false);
                shieldSlider.value = currentShield;
                return;
            }
            else
            {
                hpSlider.maxValue = maxHp;
                shieldSlider.gameObject.SetActive(false);
                hpSlider.gameObject.SetActive(true);
                if (isCri)
                {
                    FlashRed(hpSlider.fillRect.transform.GetComponent<Image>(), hpOriginalColor);
                    JellySquash(hpSlider.transform);
                }
                hpSlider.value = currentHp;
            }
        }
        else
        {
            hpSlider.maxValue = maxHp;
            shieldSlider.gameObject.SetActive(false);
            hpSlider.gameObject.SetActive(true);
            if (isCri)
            {
                FlashRed(hpSlider.fillRect.transform.GetComponent<Image>(), hpOriginalColor);
                JellySquash(hpSlider.transform);
            }
            hpSlider.value = currentHp;
        }
    }

    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash(Transform tran)
    {
        tran.DOKill();
        tran.localScale = originalScale;

        Sequence seq = DOTween.Sequence();
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        // 临时订阅
        void OnSpeedChanged(int speed)
        {
            if (seq != null) seq.timeScale = speed;
        }

        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        seq.Append(tran.DOScaleX(stretchAmount, squashDuration).SetEase(Ease.OutQuad));
        seq.Append(tran.DOScaleX(originalScale.y, squashDuration).SetEase(Ease.OutBounce));

        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }

    //闪红动画
    private void FlashRed(Image image,Color originalColor)
    {
        Color hitColor = new Color32(255, 102, 51, 255);
        image.DOKill();

        // 创建第一个红色过渡
        var toRed = image.DOColor(hitColor, 0.2f)
            .SetEase(Ease.Linear);

        // 创建第二个还原过渡
        var toNormal = image.DOColor(originalColor, 0.2f)
            .SetEase(Ease.Linear);

        // 合并为一个序列
        var seq = DOTween.Sequence();
        seq.Append(toRed);
        seq.Append(toNormal);

        // 初始化时同步 GameSpeed
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        // 临时订阅
        void OnSpeedChanged(int speed)
        {
            if (seq != null && seq.IsActive())
                seq.timeScale = speed;
        }
        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        // 动画结束后解绑
        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
}
