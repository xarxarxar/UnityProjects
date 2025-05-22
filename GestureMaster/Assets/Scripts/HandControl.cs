// HandControl.cs
// 判断两个手的状态关系（相同、相反、胜负）
using UnityEngine;
using System.Linq;

public class HandControl : MonoBehaviour
{
    public static HandControl instance;
    public Hand myHand;
    public Hand otherHand;

    //伸直的手
    [SerializeField] public Transform[] truefingerGestures;

    //握紧的手
    [SerializeField] public Transform[] falsefingerGestures;

    private void Awake() => instance = this; // 单例模式

    /// <summary>
    /// 两个手势是否相同
    /// </summary>
    /// <returns></returns>
    public bool IsSame() =>
        Enumerable.Range(0, 5).All(i => myHand.Fingers[i] == otherHand.Fingers[i]); // 所有手指状态都相同

    /// <summary>
    /// 两个手势是否相反
    /// </summary>
    /// <returns></returns>
    public bool IsOpposite() =>
        Enumerable.Range(0, 5).All(i => myHand.Fingers[i] != otherHand.Fingers[i]); // 所有手指状态都相反

    /// <summary>
    /// 剪刀石头布是否胜利
    /// </summary>
    /// <returns></returns>
    public bool IsRPSWin() // 判断石头剪刀布是否胜利
    {
        var rock = new[] { false, false, false, false, false };
        var scissor = new[] { false, true, true, false, false };
        var paper = new[] { true, true, true, true, true };
        var me = myHand.Fingers;
        var enemy = otherHand.Fingers;

        if (enemy.SequenceEqual(rock)) return me.SequenceEqual(paper);
        if (enemy.SequenceEqual(scissor)) return me.SequenceEqual(rock);
        if (enemy.SequenceEqual(paper)) return me.SequenceEqual(scissor);
        return false;
    }
}