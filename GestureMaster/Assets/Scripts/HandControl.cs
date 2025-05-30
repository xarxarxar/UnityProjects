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
    /// 判断两个手势是否完全相同（每个手指状态都一致）
    /// </summary>
    public bool IsSame(bool[] otherGesture = null)
    {
        var compareTo = otherGesture ?? otherHand.Fingers;
        if (compareTo == null || compareTo.Length != 5 || myHand.Fingers == null || myHand.Fingers.Length != 5)
            return false;

        return Enumerable.Range(0, 5).All(i => myHand.Fingers[i] == compareTo[i]);
    }

    /// <summary>
    /// 判断两个手势是否完全相反（每个手指状态都不一致）
    /// </summary>
    public bool IsOpposite(bool[] otherGesture = null)
    {
        var compareTo = otherGesture ?? otherHand.Fingers;
        if (compareTo == null || compareTo.Length != 5 || myHand.Fingers == null || myHand.Fingers.Length != 5)
            return false;

        return Enumerable.Range(0, 5).All(i => myHand.Fingers[i] != compareTo[i]);
    }

    /// <summary>
    /// 剪刀石头布是否胜利
    /// </summary>
    /// <returns></returns>
    public bool IsRPSWin(bool[] otherGesture=null) // 判断石头剪刀布是否胜利
    {
        var rock = new[] { false, false, false, false, false };
        var scissor = new[] { false, true, true, false, false };
        var paper = new[] { true, true, true, true, true };
        var me = myHand.Fingers;
        var enemy = otherGesture??otherHand.Fingers;

        if (enemy.SequenceEqual(rock)) return me.SequenceEqual(paper);
        if (enemy.SequenceEqual(scissor)) return me.SequenceEqual(rock);
        if (enemy.SequenceEqual(paper)) return me.SequenceEqual(scissor);
        return false;
    }
}