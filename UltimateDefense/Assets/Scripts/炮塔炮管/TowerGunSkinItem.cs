using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerGunSkinItem : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image skinImage;
    [SerializeField] private GameObject chosedObject;//选中时显示的框
    [SerializeField] private GameObject lockedObject;//未解锁时显示的框
    private int myIndex;
    private UnityAction<int> onClick;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(UnityAction<int> callback)
    {
        onClick = callback;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(myIndex));
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    public void SetItemData(Skin skin, int index, bool isSelected,bool isUnlocked)
    {
        myIndex = index;
        skinImage.sprite=skin.sprite;
        chosedObject.SetActive(isSelected);
        lockedObject.SetActive(!isUnlocked);
    }

}
