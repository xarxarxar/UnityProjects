using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class TowerCard : MonoBehaviour
{
    //[SerializeField] private Text _upgradeDescription;//升级描述的Text
    [SerializeField] private Text _maxlevelText;//已满级的Text
    [SerializeField] private Text _levelText;//等级Text
    [SerializeField] private GameObject _unlockMask;//未解锁的遮罩
    [SerializeField] private Button _upgradeButton;//升级按钮
    [SerializeField] private RewardStruct _upgradeCost;//升级花费
    [SerializeField] private TowerType _type;//此卡片对应的炮塔类型
    [SerializeField] private CanvasGroup _canvasGroup;//此卡片对应的炮塔类型


    /// <summary>
    /// 此卡片对应的炮塔类型
    /// </summary>
    public TowerType Type { get => _type;}
    /// <summary>
    /// 此类型的炮塔的数据
    /// </summary>
    //public TowerData TowerData { get => _towerData;}

    /// <summary>
    /// 卡片被选中，激活卡片,同时也可用于更新卡片的状态
    /// </summary>
    public void ActivateCard()
    {
        if (TowerDataManager.Instance.GetTowerData(_type).Level>0)//已解锁
        {
            _unlockMask.SetActive(false);
            _canvasGroup.alpha = 1;
            _upgradeCost.Init(RewardType.Crown, 1);
            _levelText.gameObject.SetActive(true);
            _levelText.text = $"Lv. {TowerDataManager.Instance.GetTowerData(_type).Level}";
        }
        else
        {
            _unlockMask.SetActive(true);
            _canvasGroup.alpha = 0.3f;
            _levelText.gameObject.SetActive(false);
        }
        UpdateUpgradeButton();


    }

    /// <summary>
    /// 卡片没被旋转，失活
    /// </summary>
    public void DeactivateCard()
    {
        
    }

    //升级该塔
    private void UpGradeTower()
    {
        if(MetaCurrencyManager.Instance.HasEnoughMoney(_upgradeCost.type, _upgradeCost.count))
        {
            TowerDataManager.Instance.UpgradeTower(_type);
            MetaCurrencyManager.Instance.SpendMetaCoin(_upgradeCost.type, _upgradeCost.count);
        }
        _levelText.text = $"Lv. {TowerDataManager.Instance.GetTowerData(_type).Level}";
        UpdateUpgradeButton();
    }
    //更新升级按钮的状态
    private void UpdateUpgradeButton()
    {
        _upgradeButton.onClick.RemoveAllListeners();
        //未达到满级
        if (TowerDataManager.Instance.GetTowerData(_type).Level< TowerData.MaxLevel)
        {
            _upgradeButton.interactable = true;
            //是否有足够的金币
            _upgradeCost.countText.color = MetaCurrencyManager.Instance.HasEnoughMoney(_upgradeCost.type, _upgradeCost.count)
                ? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255);
            _maxlevelText.gameObject.SetActive(false);
            _upgradeButton.onClick.AddListener(UpGradeTower);
        }
        else//达到满级
        {
            _upgradeButton.interactable = false;
            _upgradeButton.gameObject.SetActive(false);
            _maxlevelText.gameObject.SetActive(true);
        }
    }
}
