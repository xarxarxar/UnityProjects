using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class TowerCard : MonoBehaviour
{
    [SerializeField] private Text _levelText;//等级Text
    [SerializeField] private GameObject _unlockMask;//未解锁的遮罩
    [SerializeField] private BindableButton _upgradeButton;//升级按钮
    [SerializeField] private RewardStruct _upgradeCost;//升级花费
    [SerializeField] private Slider _levelSlider;//此卡片对应的炮塔类型



    /// <summary>
    /// 卡片被选中，激活卡片,同时也可用于更新卡片的状态
    /// </summary>
    public void ActivateCard()
    {
        
        //_descriptionText.text = TowerDataManager.Instance.TowerDiscription[_type];
        
        UpdateUpgradeButton();


    }


    //升级该塔
    private void UpGradeTower()
    {
        

        if (MetaCurrencyManager.Instance.HasEnoughMoney(_upgradeCost.type, _upgradeCost.count))
        {
            MetaCurrencyManager.Instance.SpendMetaCoin(_upgradeCost.type, _upgradeCost.count);
            AudioManager.Instance.PlaySFX("炮塔升级");

            
            UpdateUpgradeButton();
        }
        else
        {
            TipManager.Instance.ShowTip("王冠不足");
            AudioManager.Instance.PlaySFX("错误");
        }
       
    }

    //更新升级按钮的状态
    private void UpdateUpgradeButton()
    {
        _upgradeButton.RemoveAllListeners();
        _upgradeButton.playSound = false;
        //未达到满级
       
    }
}
