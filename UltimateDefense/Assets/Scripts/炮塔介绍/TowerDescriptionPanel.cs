using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerDescriptionPanel : MonoBehaviour
{
    [SerializeField] private Text damageText;//…À∫¶
    [SerializeField] private Text attackRateText;//π•ÀŸ
    [SerializeField] private Text bulletCapText;//µØº–
    [SerializeField] private Text cirticalProbText;//±©ª˜¬ 
    [SerializeField] private Text cirticalMultiText;//±©ª˜…À∫¶
    [SerializeField] private Text reloadTimeText;//ªªµØ ±º‰
    /// <summary>
    /// ≥ı ºªØ≈⁄À˛ Ù–‘√Ê∞Â
    /// </summary>
    public void Init(TowerData towerData)
    {
        damageText.text= towerData.BaseDamage.ToString();
        attackRateText.text= towerData.BaseAtkRate.ToString();
        bulletCapText.text= towerData.BaseCap.ToString();
        cirticalProbText.text= towerData.BaseCritProb.ToString();
        cirticalMultiText.text= towerData.BaseCritMult.ToString();
        reloadTimeText.text= towerData.BaseReload.ToString();
    }
}
