using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerDescriptionPanel : MonoBehaviour
{
    [SerializeField] private Text damageText;//伤害
    [SerializeField] private Text attackRateText;//攻速
    [SerializeField] private Text bulletCapText;//弹夹
    [SerializeField] private Text cirticalProbText;//暴击率
    [SerializeField] private Text cirticalMultiText;//暴击伤害
    [SerializeField] private Text reloadTimeText;//换弹时间
    /// <summary>
    /// 初始化炮塔属性面板
    /// </summary>
    public void Init(TowerData towerData)
    {
        damageText.text= towerData.BaseDamage.ToString();
        attackRateText.text= towerData.BaseAtkRate.ToString();
        bulletCapText.text= towerData.BaseCap.ToString();
        cirticalProbText.text = $"{Mathf.FloorToInt(towerData.BaseCritProb * 100)}%";//向下取整
        cirticalMultiText.text= towerData.BaseCritMult.ToString();
        reloadTimeText.text= towerData.BaseReload.ToString();
    }
}
