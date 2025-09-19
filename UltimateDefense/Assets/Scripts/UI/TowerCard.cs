using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class TowerCard : MonoBehaviour
{
    [SerializeField] private Text _levelText;//等级Text
    [SerializeField] private Text _descriptionText;//描述Text
    [SerializeField] private GameObject _unlockMask;//未解锁的遮罩
    [SerializeField] private BindableButton _upgradeButton;//升级按钮
    [SerializeField] private RewardStruct _upgradeCost;//升级花费
    [SerializeField] private TowerType _type;//此卡片对应的炮塔类型
    [SerializeField] private Slider _levelSlider;//此卡片对应的炮塔类型


    /// <summary>
    /// 此卡片对应的炮塔类型
    /// </summary>
    public TowerType Type { get => _type;}

    /// <summary>
    /// 卡片被选中，激活卡片,同时也可用于更新卡片的状态
    /// </summary>
    public void ActivateCard()
    {
        if (TowerDataManager.Instance.GetTowerData(_type).Level>0)//已解锁
        {
            _unlockMask.SetActive(false);
            _upgradeCost.Init(RewardType.Crown, 1);
            _levelText.gameObject.SetActive(true);
            _levelText.text = $"Lv {TowerDataManager.Instance.GetTowerData(_type).Level}";
            
        }
        else
        {
            _unlockMask.SetActive(true);
            _levelText.gameObject.SetActive(false);
        }
        _descriptionText.text = TowerDataManager.Instance.TowerDiscription[_type];
        _levelSlider.value = TowerDataManager.Instance.GetTowerData(_type).Level;
        UpdateUpgradeButton();


    }


    //升级该塔
    private void UpGradeTower()
    {
        if (TowerDataManager.Instance.GetTowerData(_type).Level >= TowerData.MaxLevel)
        {
            AudioManager.Instance.PlaySFX("错误");
            GameUIManager.Instance.ShowQuickTip("已满级");
            return;
        }

        if (MetaCurrencyManager.Instance.HasEnoughMoney(_upgradeCost.type, _upgradeCost.count))
        {
            TowerDataManager.Instance.UpgradeTower(_type);
            MetaCurrencyManager.Instance.SpendMetaCoin(_upgradeCost.type, _upgradeCost.count);
            AudioManager.Instance.PlaySFX("炮塔升级");

            _levelText.text = $"Lv {TowerDataManager.Instance.GetTowerData(_type).Level}";
            _levelSlider.value = TowerDataManager.Instance.GetTowerData(_type).Level;
            _descriptionText.text = TowerDataManager.Instance.TowerDiscription[_type];
            UpdateUpgradeButton();
        }
        else
        {
            GameUIManager.Instance.ShowQuickTip("王冠不足");
            AudioManager.Instance.PlaySFX("错误");
        }
       
    }

    //更新升级按钮的状态
    private void UpdateUpgradeButton()
    {
        _upgradeButton.RemoveAllListeners();
        _upgradeButton.playSound = false;
        //未达到满级
        if (TowerDataManager.Instance.GetTowerData(_type).Level< TowerData.MaxLevel)
        {
            //是否有足够的金币
            _upgradeCost.countText.color = MetaCurrencyManager.Instance.HasEnoughMoney(_upgradeCost.type, _upgradeCost.count)
                ? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255);
            _upgradeButton.AddListener(UpGradeTower);
        }
        else//达到满级
        {
            _upgradeButton.gameObject.SetActive(false);
        }
    }
}
