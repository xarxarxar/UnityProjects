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
        ScienceManager.Instance.UnlockIndex.OnValueChanged += SetInteractale;
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void UpdateUI(ScienceNodeData scienceNode,int index)
    {
        _index = index;
        _descriptionText.text =scienceNode.description;
        _unlockCost.Init(scienceNode.costType, scienceNode.cost);
        SetInteractale(_index);//设置该节点的可交互性
    }

    //解锁按钮的点击事件
    private void UnlockButton()
    {
        Debug.Log($"解锁的序号为{_index}");
        ScienceManager.Instance.UnlockScience(_index);
        //_unlockButton.onClick.RemoveListener(UnlockButton);
    }

    //设置这个节点的可交互性
    private void SetInteractale(int index)
    {
        _unlockMask.SetActive(_index > ScienceManager.Instance.UnlockIndex.Value + 1);
        _unlockButton.interactable = _index == ScienceManager.Instance.UnlockIndex.Value + 1;
    }
}
