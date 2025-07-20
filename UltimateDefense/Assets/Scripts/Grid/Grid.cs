using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Grid : MonoBehaviour, IPointerClickHandler
{
    [field:SerializeField]
    public Vector2Int GridPos { get; private set; } // 格子在网格中的位置，比如(2,3)
    public bool IsOccupied { get; private set; }    // 是否已被建筑占用
    public BuildingBase OccupyingObject { get; private set; }
    [SerializeField] private Text coordText; // 可选：Text组件用于显示坐标

    // 初始化格子位置
    public void Init(Vector2Int pos)
    {
        GridPos = pos;
        IsOccupied = false;

#if UNITY_EDITOR
        if (coordText != null)
        {
            coordText.text = $"({pos.x},{pos.y})";
        }
#endif
    }

    // 占用格子
    public void Occupy(BuildingBase obj)
    {
        IsOccupied = true;
        OccupyingObject = obj;
    }

    // 清空格子
    public void Clear()
    {
        IsOccupied = false;
        OccupyingObject = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"点击了格子：({GridPos.x}, {GridPos.y})");
        BuildManager.instance.HideBuildMenu();
        // 通知 GridManager 选择了哪个格子
        BuildManager.instance.ShowBuildMenu(this);
    }
}
