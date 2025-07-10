using SuperScrollView;
using UnityEngine;

public class SciencePanel : MonoBehaviour
{
    [SerializeField]private LoopListView2 listView;//列表

    private void Start()
    {
        // 初始化
        listView.InitListView(50, OnGetItemByIndex);
    }

    // 当需要显示新的 Item 时会调用
    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= 50) return null;

        // 从池中获取Item（name要与预制体名一致）
        LoopListViewItem2 item = listView.NewListViewItem("ScienceNode");

        // 设置数据
        ScienceNodeItem script = item.GetComponent<ScienceNodeItem>();

        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            script.Init(ScienceManager.Instance.GetScienceDataByIndex(index), index);
            return item;
        }
        //否则只需要更新就行
        script.UpdateUI(ScienceManager.Instance.GetScienceDataByIndex(index),index);

        return item;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.O)) 
        {
            listView.MovePanelToItemIndex(10,0);
        }
    }
}



