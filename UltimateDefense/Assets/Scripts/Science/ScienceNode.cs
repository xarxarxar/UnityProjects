using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 科技节点的UI
/// </summary>
public class ScienceNode : MonoBehaviour
{
    [SerializeField]private Text _descriptionText;//描述Text
    [SerializeField]private Button _unlockButton;//解锁该科技节点的按钮
    public int Index;//该科技节点所对应的index

    private void OnEnable()
    {
        _unlockButton.onClick.AddListener(UnlockButton);
        _unlockButton.interactable = Index == ScienceManager.Instance.UnlockIndex+1;
        _descriptionText.text = ScienceManager.Instance.GetScienceDataByIndex(Index).description;
    }

    private void OnDisable()
    {
        _unlockButton.onClick.RemoveAllListeners();
    }

    private void UnlockButton()
    {
        ScienceManager.Instance.UnlockScience(Index);
        _unlockButton.onClick.RemoveListener(UnlockButton);
    }
}
