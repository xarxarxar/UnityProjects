using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //属性
    //public int CurrentDay = 1;//游戏内的天数
    public int currentMonth =1;//游戏内的月份，向上取整，初始化的时候必须使用这个值，要不然会触发事件
    public int CurrentYear=> Mathf.CeilToInt(CurrentMonth / 12f);//游戏内的年份，向上取整
    //设置
    [SerializeField]private float SingleDayTime = 10;//游戏呢一个月的时长，默认为10秒，测试时可以修改

    public GameObject StartPanel;
    public GameObject BattleEndPanel;
    public Button StartButton;
    public Button RestartButton;

    public AnimalData CurrentAnimal;

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
        CurrentAnimal = AnimalManager.Instance.AllAnimalDatas[2];


        StartButton.onClick.RemoveAllListeners();
        StartButton.onClick.AddListener(() =>
        {
            StartPanel.SetActive(false);
            BattleManager.instance.Init(CurrentAnimal, null,120);
        });
        RestartButton.onClick.RemoveAllListeners();
        RestartButton.onClick.AddListener(() =>
        {
            BattleEndPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
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
