using UnityEngine;
using UnityEngine.UI;

public class HPSliderFor2D : MonoBehaviour
{
    [Header("UI 引用")]
    public Image line;       // 分割条
    public Image hpBar;       // 血条（红色）
    public Image shieldBar;   // 护盾条（蓝色）

    private float baseWidth = 0f; // 最大血量时背景的基准宽度（可在 Inspector 设置）
    private Material mat = null;
    public int segments = 10;    // 想要的线段数量


    public void Init(int currentHp, int maxHp, int currentShield)
    {
        if (mat == null)
        {
            // 给Image创建一个实例化材质，避免修改到原始材质
            mat = Instantiate(line.material);
            line.material = mat;
        }
        if (baseWidth == 0f)
        {
            baseWidth = GetComponent<RectTransform>().sizeDelta.x;
        }

        UpdateBar(currentHp, maxHp, currentShield);
    }

    /// <summary>
    /// 更新血条
    /// </summary>
    /// <param name="currentHp">当前血量</param>
    /// <param name="maxHp">最大血量</param>
    /// <param name="currentShield">当前护盾值</param>
    public void UpdateBar(int currentHp, int maxHp, int currentShield)
    {
        // 背景的值：取 maxHP 和 (HP+盾) 的最大值
        int total = Mathf.Max(maxHp, currentHp + currentShield);
        //画黑线
        SetSegments(CalculateSegments(total));

        // 血条宽度：当前 HP / 背景最大值
        float hpWidth = baseWidth * ((float)currentHp / total);
        hpWidth = Mathf.Max(hpWidth, 0);
        hpBar.rectTransform.sizeDelta = new Vector2(hpWidth, hpBar.rectTransform.sizeDelta.y);

        // 填充护盾条（紧贴在血条后面）
        float shieldWidth = baseWidth * ((float)currentShield / total);
        shieldWidth = Mathf.Max(shieldWidth, 0);
        shieldBar.rectTransform.sizeDelta = new Vector2(shieldWidth, shieldBar.rectTransform.sizeDelta.y);
    }

    public void SetSegments(int count)
    {
        segments = count;
        mat.SetFloat("_Segments", segments);
    }

    int CalculateSegments(int total)
    {
        total = total / 10;
        if (total <= 1) return 0;

        if (total <= 5)
            return total - 1;

        if (total <= 20)
            return (5 + Mathf.RoundToInt((total - 5) / 3f)) - 1;

        // total > 20
        return (Mathf.Min(20, 10 + Mathf.RoundToInt((total - 20) / 5f))) - 1;
    }
}
