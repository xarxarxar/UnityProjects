using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public int _index;//该科技节点所对应的index

    private void OnEnable()
    {
        //_unlockButton.onClick.AddListener(UnlockButton);
        //_unlockButton.interactable = Index == ScienceManager.Instance.UnlockIndex+1;
        //_descriptionText.text = ScienceManager.Instance.GetScienceDataByIndex(Index).description;
    }

    private void OnDisable()
    {
        _unlockButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(ScienceNodeData scienceNode,int index)
    {
        _index=index;
        if(scienceNode == null )
        {
            Debug.Log("scienceNode is null");
            return;
        }
        _descriptionText.text=index.ToString()+ scienceNode.description;
        _unlockCost.Init(scienceNode.costType,scienceNode.cost);
        _unlockButton.interactable = _index == ScienceManager.Instance.UnlockIndex + 1;

        _unlockButton.onClick.AddListener(UnlockButton);
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void UpdateUI(ScienceNodeData scienceNode,int index)
    {
        _index = index;
        _descriptionText.text = index.ToString() + scienceNode.description;
        _unlockCost.Init(scienceNode.costType, scienceNode.cost);
        _unlockButton.interactable = _index == ScienceManager.Instance.UnlockIndex + 1;
    }

    private void UnlockButton()
    {
        ScienceManager.Instance.UnlockScience(_index);
        _unlockButton.onClick.RemoveListener(UnlockButton);
    }
}
