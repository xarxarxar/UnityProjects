using DanielLochner.Assets.SimpleScrollSnap; // 引入命名空间
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTowerPanel : MonoBehaviour
{
    //可序列化
    [SerializeField] private BindableButton _unlockButton;//解锁新炮塔的按钮
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

        //_simpleScrollSnap.StartingPanel = (int)DataManager.Instance.PlayerInfo.CurrentTowerType.Value;//起始位置
        //_simpleScrollSnap.GoToPanel((int)DataManager.Instance.PlayerInfo.CurrentTowerType.Value);//起始位置
        _currentTower = _simpleScrollSnap.Content.GetChild(_simpleScrollSnap.StartingPanel).GetComponent<TowerCard>();
        
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
        _unlockButton.RemoveAllListeners(); // 先移除旧监听器

        


    }

    //解锁炮塔
    private void UnlockTower(TowerCard towerCard)
    {
        //金币足够
        if (MetaCurrencyManager.Instance.HasEnoughMoney(_unlockCost.type, _unlockCost.count))
        {
            MetaCurrencyManager.Instance.SpendMetaCoin(_unlockCost.type, _unlockCost.count);
            
        }
        
        towerCard.ActivateCard();
        UpdateButtonState();
    }

    //选定这个炮塔
    private void ChooseThisTower(TowerCard towerCard)
    {
        Debug.Log("选择炮塔");
        

        _unlockCost.gameObject.SetActive(false);
        UpdateButtonState();
    }


    //元素改变
    private void OnPanelSelected(int index,int preIndex)
    {
        
        _currentTower = _simpleScrollSnap.Content.GetChild(index).GetComponent<TowerCard>();
        _currentTower.ActivateCard();

        UpdateButtonState();//更新下方解锁按钮的逻辑
    }

}
