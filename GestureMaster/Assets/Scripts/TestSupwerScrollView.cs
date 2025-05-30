using SuperScrollView;
using UnityEngine;

public class TestSupwerScrollView : MonoBehaviour
{
    public LoopListView2 mLoopListView;
    public int mLoopCount = 10;

    private void Start()
    {
        //mLoopListView.InitListView(mLoopCount, OnGetItemByIndex);
    }

    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView,int index)
    {
        LoopListViewItem2 item = listView.NewListViewItem("Image");
        return item;
    }
}
