using System;
using System.Collections;
using UnityEngine;

public class Hand:MonoBehaviour
{
    //分别是大拇指，食指，中指，无名指，小拇指
    [SerializeField]private bool[] fingers=new bool[5];
    public bool[] Fingers { get => fingers;}

    //伸直的手
    [SerializeField] public Transform[] fingerGestures;

    public void SetFingers(bool[] values)
    {
        if (values.Length != 5) return;
        Array.Copy(values, fingers, 5);//将fingers设置为setFingers的值

        for (int i = 0; i < 5; i++)
        {
            if (fingers[i])
            {
                SyncRecursive(fingerGestures[i], HandControl.instance.truefingerGestures[i]);
            }
            else
            {
                SyncRecursive(fingerGestures[i], HandControl.instance.falsefingerGestures[i]);
            }
        }
    }

    /// <summary>
    /// 设置单个手指状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void ToggleOneFinger(int index)
    {
        fingers[index] = !fingers[index];
        if (fingers[index])
        {
            SyncRecursive(fingerGestures[index], HandControl.instance.truefingerGestures[index]);
        }
        else
        {
            SyncRecursive(fingerGestures[index], HandControl.instance.falsefingerGestures[index]);
        }
    }

    // 递归方法：同步两个 transform
    private void SyncRecursive( Transform target,Transform source)
    {
        if (source == null || target == null)
        {
            Debug.LogWarning("结构不一致或对象为空");
            return;
        }

        target.localPosition = source.localPosition;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;

        // 如果子物体数量不同，忽略多余的
        int childCount = Mathf.Min(source.childCount, target.childCount);

        for (int i = 0; i < childCount; i++)
        {
            SyncRecursive( target.GetChild(i),source.GetChild(i));
        }
    }

    public IEnumerator RandomGesture()
    {
        int index = 0;
        while (true)
        {
            index = UnityEngine.Random.Range(0, 5);
            ToggleOneFinger(index);
            //if (UnityEngine.Random.value > 0.5f)
            //{
            //    SyncRecursive(fingerGestures[index], HandControl.instance.truefingerGestures[index]);
            //}
            //else
            //{
            //    SyncRecursive(fingerGestures[index], HandControl.instance.falsefingerGestures[index]);
            //}
            // 暂停处理
            while (LevelPlaying.isPaused) yield return null;
            yield return new WaitForSeconds(0.2f);
        }
        
    }
}
