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
    [SerializeField]private GameObject _unlockButtonShow;//解锁按钮的显示
    public int _index;//该科技节点所对应的index

    [SerializeField]private Image _image;//背景的光环
    [SerializeField]private Image _imageBackground;//背景的image
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(ScienceNodeData scienceNode,int index)
    {
        _index=index;
        _descriptionText.text=scienceNode.description;
        _unlockCost.Init(scienceNode.costType,scienceNode.cost);
        SetInteractale(0,_index);//设置该节点的可交互性

        _unlockButton.onClick.AddListener(UnlockButton);
        DataManager.Instance.PlayerInfo.OnUnlockCountChanged += SetInteractale;
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
        SetInteractale(0, _index);//设置该节点的可交互性
    }

    //解锁按钮的点击事件
    private void UnlockButton()
    {
        Debug.Log($"解锁的序号为{_index},记录下来的序号为{DataManager.Instance.PlayerInfo.UnlockCount}");
        //该科技已解锁
        if (DataManager.Instance.PlayerInfo.UnlockCount != _index) return;

        if(DataManager.Instance.PlayerInfo.UnlockCount < _index)
        {
            TipManager.Instance.ShowTip("前先解锁前一个科技");
            return;
        }

        //钱不够
        if (MetaCurrencyManager.Instance.SpendMetaCoin(_unlockCost.type, _unlockCost.count))
        {
            ScienceManager.Instance.UnlockScience(_index);
            AudioManager.Instance.PlaySFX("升级");
        }
        else
        {
            if (_unlockCost.type == RewardType.Diamond)
            {
                TipManager.Instance.ShowTip("钻石不足");
            }
            if (_unlockCost.type == RewardType.Crown)
            {
                TipManager.Instance.ShowTip("王冠不足");
            }
        }

    }

    //设置这个节点的可交互性
    private void SetInteractale(int oldValue,int index)
    {
        //Debug.Log($"解锁的序号为{_index},记录下来的序号为{DataManager.Instance.PlayerInfo.UnlockCount.Value},可见性为{_index < DataManager.Instance.PlayerInfo.UnlockCount.Value}");

        _lockedMask.gameObject.SetActive(_index < DataManager.Instance.PlayerInfo.UnlockCount);
        _unlockButtonShow.gameObject.SetActive(_index == DataManager.Instance.PlayerInfo.UnlockCount);
        _unlockMask.SetActive(_index > DataManager.Instance.PlayerInfo.UnlockCount);

        //已解锁
        if(_index < DataManager.Instance.PlayerInfo.UnlockCount)
        {
            _image.color = new Color32(238,220,121,0);
            _imageBackground.color = new Color32(61,49,65,255);
            _descriptionText.color = new Color32(119,119,119,255);
        }
        else if(_index == DataManager.Instance.PlayerInfo.UnlockCount)
        {
            _image.color = new Color32(238, 220, 121, 255);
            _imageBackground.color = new Color32(37, 29, 39, 255);
            _descriptionText.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            _image.color = new Color32(238, 220, 121, 0);
            _imageBackground.color = new Color32(61, 49, 65, 255);
            _descriptionText.color = new Color32(119, 119, 119, 255);
        }
        
    }
}
