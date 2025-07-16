using DanielLochner.Assets.SimpleScrollSnap; // 引入命名空间
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTowerPanel : MonoBehaviour
{
    //可序列化
    [SerializeField] private Button _unlockButton;//解锁新炮塔的按钮
    [SerializeField] private RewardStruct _unlockCost;//解锁新炮塔的花费
    [SerializeField] private Text _chooseThisTower;//使用该炮塔的提示文本
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;//
    
    //私有
    private TowerCard _currentTower;//当前的scrollview所在的Tower

    private void OnEnable()
    {
        _simpleScrollSnap.OnPanelCentered.AddListener(OnPanelSelected);
        Init();
    }
    private void OnDisable()
    {
        _simpleScrollSnap.OnPanelCentered.RemoveListener(OnPanelSelected);
    }

    //初始化所有卡片
    private void Init()
    {
        _unlockCost.Init(RewardType.Crown, 1);

        _simpleScrollSnap.StartingPanel = (int)TowerDataManager.Instance.CurrentTowerType;//起始位置
        _currentTower = _simpleScrollSnap.Content.GetChild(_simpleScrollSnap.StartingPanel).GetComponent<TowerCard>();
        //_currentTower.ActivateCard();
        
        for (int i=0;i< _simpleScrollSnap.NumberOfPanels; i++)
        {
            var towerCard = _simpleScrollSnap.Content.GetChild(i).GetComponent<TowerCard>();
            if (towerCard != null)
            {
                towerCard.ActivateCard();
            }
        }
        UpdateButtonState();
    }

    //更新解锁按钮的状态
    private void UpdateButtonState()
    {
        _unlockButton.onClick.RemoveAllListeners(); // 先移除旧监听器

        if (TowerDataManager.Instance.GetTowerData(_currentTower.Type).Level == 0)//未解锁
        {
            if (TowerDataManager.Instance.GetTowerData(TowerType.Basic).Level == 10)//基础形态到10级了可以解锁
            {
                _unlockButton.interactable = true;
            }
            else
            {
                _unlockButton.interactable = false;
            }
            
            _unlockCost.gameObject.SetActive(true);
            _chooseThisTower.gameObject.SetActive(false);
            //是否有足够的金币
            _unlockCost.countText.color= MetaCurrencyManager.Instance.HasEnoughMoney(_unlockCost.type, _unlockCost.count)
                ? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255);
            _unlockButton.onClick.AddListener(() => UnlockTower(_currentTower));
        }
        else//已解锁
        {
            _unlockCost.gameObject.SetActive(false);
            _chooseThisTower.gameObject.SetActive(true);
            _unlockButton.onClick.AddListener(() => ChooseThisTower(_currentTower));
            //当前选择的不是对局中使用的炮塔
            if (TowerDataManager.Instance.CurrentTowerType!= _currentTower.Type)
            {
                _unlockButton.interactable = true;
                _chooseThisTower.text = "选择";
                _chooseThisTower.color = new Color32(239, 241, 245, 255);
            }
            else//当前选择的就是对局中使用的炮塔
            {
                _unlockButton.interactable = false;
                _chooseThisTower.text = "已选择";
                _chooseThisTower.color = new Color32(76, 175, 80, 255);
            }
        }


    }

    //解锁炮塔
    private void UnlockTower(TowerCard towerCard)
    {
        //金币足够
        if (MetaCurrencyManager.Instance.HasEnoughMoney(_unlockCost.type, _unlockCost.count))
        {
            MetaCurrencyManager.Instance.SpendMetaCoin(_unlockCost.type, _unlockCost.count);
            TowerDataManager.Instance.UnlockTower(towerCard.Type);
        }
        
        towerCard.ActivateCard();
        UpdateButtonState();
    }

    //选定这个炮塔
    private void ChooseThisTower(TowerCard towerCard)
    {
        _unlockCost.gameObject.SetActive(false);
        TowerDataManager.Instance.CurrentTowerType= _currentTower.Type;
        UpdateButtonState();
    }


    //元素改变
    private void OnPanelSelected(int index,int preIndex)
    {
        
        _currentTower = _simpleScrollSnap.Content.GetChild(index).GetComponent<TowerCard>();
        _currentTower.ActivateCard();
        TowerCard preTower = _simpleScrollSnap.Content.GetChild(preIndex).GetComponent<TowerCard>();
        preTower.DeactivateCard();

        UpdateButtonState();//更新下方解锁按钮的逻辑
    }

}
