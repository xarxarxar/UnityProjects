using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 科技节点的UI
/// </summary>
public class ScienceNodeItem : MonoBehaviour
{
    [SerializeField]private Text _descriptionText;//描述Text
    [SerializeField]private Button _unlockButton;//解锁该科技节点的按钮
    [SerializeField]private RewardStruct _unlockCost;//解锁该科技节点所需的花费
    [SerializeField]private GameObject _unlockMask;//该节点的未解锁时候的蒙版
    [SerializeField]private GameObject _lockedMask;//该节点的已解锁之后的蒙版
    public int _index;//该科技节点所对应的index

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(ScienceNodeData scienceNode,int index)
    {
        _index=index;
        _descriptionText.text=scienceNode.description;
        _unlockCost.Init(scienceNode.costType,scienceNode.cost);
        SetInteractale(_index);//设置该节点的可交互性

        _unlockButton.onClick.AddListener(UnlockButton);
        DataManager.Instance.PlayerInfo.UnlockCount.OnValueChanged += SetInteractale;
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void UpdateUI(ScienceNodeData scienceNode,int index)
    {
        Debug.Log("更新节点状态");
        _index = index;
        _descriptionText.text =scienceNode.description;
        _unlockCost.Init(scienceNode.costType, scienceNode.cost);
        SetInteractale(_index);//设置该节点的可交互性
    }

    //解锁按钮的点击事件
    private void UnlockButton()
    {
        Debug.Log($"解锁的序号为{_index},记录下来的序号为{DataManager.Instance.PlayerInfo.UnlockCount.Value}");
        //该科技已解锁
        if (DataManager.Instance.PlayerInfo.UnlockCount.Value >= _index) return;

        if(DataManager.Instance.PlayerInfo.UnlockCount.Value + 1 < _index)
        {
            GameUIManager.Instance.ShowQuickTip("前先解锁前一个科技");
            return;
        }

        //钱不够
        if (MetaCurrencyManager.Instance.SpendMetaCoin(_unlockCost.type, _unlockCost.count))
        {
            ScienceManager.Instance.UnlockScience(_index);
        }
        else
        {
            if (_unlockCost.type == RewardType.Diamond)
            {
                GameUIManager.Instance.ShowQuickTip("钻石不足");
            }
            if (_unlockCost.type == RewardType.Crown)
            {
                GameUIManager.Instance.ShowQuickTip("王冠不足");
            }
        }
        
        
        //_unlockButton.onClick.RemoveListener(UnlockButton);
    }

    //设置这个节点的可交互性
    private void SetInteractale(int index)
    {
        //Debug.Log($"解锁的序号为{_index},记录下来的序号为{DataManager.Instance.PlayerInfo.UnlockCount.Value},可见性为{_index < DataManager.Instance.PlayerInfo.UnlockCount.Value}");

        _lockedMask.gameObject.SetActive(_index <= DataManager.Instance.PlayerInfo.UnlockCount.Value);
        _unlockMask.SetActive(_index > DataManager.Instance.PlayerInfo.UnlockCount.Value + 1);
        //_unlockButton.interactable = _index == ScienceManager.Instance.UnlockIndex.Value + 1;
    }
}
