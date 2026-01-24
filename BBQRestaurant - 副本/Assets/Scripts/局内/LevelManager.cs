using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 用于局内的管理,此处是整个局内游戏的起点
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public readonly float totalTime = 120;//单局游戏多少秒
    //---事件
    public static UnityAction OnInit;
    public static UnityAction OnStartBattle;
    public static UnityAction OnDead;
    public static UnityAction OnRelive;
    public static UnityAction OnEndBattle;

    //---属性---
    public bool isPaused { get; private set; }=false;//对局是否在暂停中


    //---变量---
    private float CurrentTime = 0;//当前的时间
    private Coroutine CountDownCoro =null;//倒计时的协程
    [SerializeField] private Text text;
    [SerializeField] private GameObject levelObject;//关卡物体


    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CurrentTime = 0;
        if(CountDownCoro!=null )
        {
            StopCoroutine(CountDownCoro);
        }
        StartBattle();//开始对局
        OnInit?.Invoke();
        levelObject.SetActive(true);
    }

    /// <summary>
    /// 开始一天，开始倒计时
    /// </summary>
    public void StartBattle()
    {
        isPaused = false;
        CountDownCoro=StartCoroutine(CountDownIe());//开始倒计时
        OnStartBattle?.Invoke();
    }

    /// <summary>
    /// 结束了，但是此时并不算真正的结束，可以给予玩家看广告复活的机会
    /// </summary>
    public void Dead()
    {
        isPaused=true;
        EndBattle();
        OnDead?.Invoke();
    }

    /// <summary>
    /// 玩家复活了
    /// </summary>
    public void Relive()
    {
        OnRelive?.Invoke();
    }

    /// <summary>
    /// 一天真正结束
    /// </summary>
    public void EndBattle()
    {
        levelObject.SetActive(false);
        OnEndBattle?.Invoke();
    }


    //=====私有方法=====
    
    //一天的倒计时
    private IEnumerator CountDownIe()
    {
        while (true)
        {
            if (isPaused) { yield return null; }

            if(CurrentTime < totalTime)
            {
                CurrentTime += Time.deltaTime;
            }
            else
            {
                Dead();
                CurrentTime = totalTime;
            }
            text.text=((int)(totalTime- CurrentTime)).ToString() ;
            yield return null;
        }
    }
}
