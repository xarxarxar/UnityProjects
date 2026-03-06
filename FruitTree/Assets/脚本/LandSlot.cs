using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 地块脚本，只需要负责维护该地块是否已经有树
/// </summary>
public class LandSlot : MonoBehaviour
{
    public int landIndex = 0;//这块土地的编号
    public FruitType treeData=null;//这块土地上的果树类型
    public bool isOccupied = false;//是否已经占用

    //点击
    void OnMouseUp()
    {
        if (isOccupied) return;

        // 防止点击UI时触发土地点击
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                // 核心修改：如果遮挡物的名字是“地块”，或者是地块上的Text，就不算“被挡住”
                if (result.gameObject == this.gameObject)
                {
                    continue;
                }
                // 只有挡住的东西不是我自己时，才拦截
                Debug.Log("真正被UI挡住了: " + result.gameObject.name);
                return;
            }
        }

        Debug.Log("点击1 - 开始处理业务逻辑");
        if (treeData == null || string.IsNullOrEmpty(treeData.name))
        {
            // 没树，开商店
            PlantingPanel.Instance.OpenPanel(this);
        }
    }

    //种一棵树
    public void PlantTree(FruitType newData)
    {
        treeData = newData;
        FruitGameManager.Instance.PlantTree(newData,this);
        isOccupied = true;
    }
}