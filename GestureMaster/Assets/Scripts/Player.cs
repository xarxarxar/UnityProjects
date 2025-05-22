// Player.cs
// 控制玩家手势的切换
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Button[] fingerButtons; // 控制每根手指的按钮
    private Hand myHand => HandControl.instance.myHand;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            fingerButtons[i].onClick.AddListener(() => ToggleFinger(index));
        }
    }

    //按钮切换手指的状态
    private void ToggleFinger(int index)
    {
        myHand.ToggleOneFinger(index);//切换手指状态
    }
}