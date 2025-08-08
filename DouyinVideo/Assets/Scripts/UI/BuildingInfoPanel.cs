using SuperScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableDictionary.Scripts;

public class BuildingInfoPanel : MonoBehaviour
{
    [SerializeField]private LoopGridView _allBuildingsGrid;
    [SerializeField]List<BuildingGridData> _allBuildings=new List<BuildingGridData>();

    private void Start()
    {

        _allBuildingsGrid.InitGridView(_allBuildings.Count, OnGetItemByRowColumn);
    }

    LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
    {
        if (index < 0)
        {
            return null;
        }
        if (index >= _allBuildings.Count) return null;

        LoopGridViewItem item = gridView.NewListViewItem("建筑");
        BuildingGrid buildingGrid= item.GetComponent<BuildingGrid>();

        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            buildingGrid.Init(_allBuildings[index],index);
            return item;
        }
        //否则只需要更新就行
        buildingGrid.UpdateUI(_allBuildings[index], index);
        return item;
    }
}
