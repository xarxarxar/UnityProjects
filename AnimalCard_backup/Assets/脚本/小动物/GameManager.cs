using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //属性
    //public int CurrentDay = 1;//游戏内的天数
    public int currentMonth =1;//游戏内的月份，向上取整，初始化的时候必须使用这个值，要不然会触发事件
    public int CurrentYear=> Mathf.CeilToInt(CurrentMonth / 12f);//游戏内的年份，向上取整

    //设置
    [SerializeField]private float SingleDayTime = 10;//游戏呢一个月的时长，默认为10秒，测试时可以修改

    public static event Action<int,int> OnMonthChanged;//月份增加的时候

    public int CurrentMonth
    {
        get => currentMonth;
        set
        {
            if(currentMonth != value)
            {
                OnMonthChanged?.Invoke(currentMonth,value);
                currentMonth = value;
            }
        }
    }


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //StartCoroutine(DayIe());
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.A))
        {
            CurrentMonth += 10;
        }
    }


    //天数增长
    private IEnumerator DayIe()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(SingleDayTime);
            CurrentMonth++;
            
        }
    }
}
