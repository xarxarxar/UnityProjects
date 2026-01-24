using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;
    public ObjectPool<Customer> customerPool;
    public Transform customerPoolParent;
    [Header("所有食材预制体")]
    public Customer customerPrefab;//顾客的预制体
    public List<Customer> AllCustomer=new List<Customer>();//所有场上的顾客
    public List<CustomerData> allCustomerDatas=new List<CustomerData>();//所有顾客的数据

    //顾客的设置
    public readonly float LowPatienceThreshold = 0.3f;//低耐心值比例
    public readonly float MidPatienceThreshold = 0.7f;//高耐心值比例

    [Header("所有落座点（6个）")]
    public List<CustomerSeat> seats = new List<CustomerSeat>();

    private Coroutine spawnCoro = null;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        LevelManager.OnInit += Init;
        LevelManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (customerPool == null)
        {
            customerPool = new ObjectPool<Customer>(customerPrefab, 10, customerPoolParent);
        }
        SpawnCustomer();
    }

    //关卡结束
    private void OnEndBattle()
    {
        for (int i = AllCustomer.Count - 1; i >= 0; i--)
        {
            AllCustomer[i].End();
        }
        AllCustomer.Clear(); // 可省略，看 End() 有没有 remove 全部

        if(spawnCoro != null)
        {
            StopCoroutine(spawnCoro);
            spawnCoro = null;
        }
    }

    //生成顾客
    public void SpawnCustomer()
    {
        spawnCoro= StartCoroutine(SpawnCustomerRoutine());
    }

    /// <summary>
    /// 将顾客从顾客列表中移除
    /// </summary>
    /// <param name="customer"></param>
    public void RemoveCustomer(Customer customer)
    {
        if (AllCustomer.Contains(customer))
        {
            AllCustomer.Remove(customer);
        }
    }

    private IEnumerator SpawnCustomerRoutine()
    {
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(10f);

        yield return new WaitForSecondsRealtime(3.0f);
        while (true)
        {
            // 暂停时每帧等待
            while (LevelManager.Instance.isPaused)
            {
                yield return null;
            }

            // 1. 找到一个空座位
            CustomerSeat seat = GetFreeSeat();
            if (seat == null)
            {
                Debug.LogWarning("没有空座位，无法生成顾客！");
                yield break;
            }

            // 2. 生成顾客（位置在座位右侧 5 距离处）
            Customer customer = customerPool.Get();
            Vector3 spawnPos = seat.seatPoint.position + new Vector3(5f, 0f, 0f);
            customer.transform.position = spawnPos;
            var randomData = allCustomerDatas[Random.Range(0, allCustomerDatas.Count)];
            // 3. 设置顾客
            customer.SetCustomer(randomData,seat);

            // 4. 等待下一次生成
            float elapsed = 0f;
            while (elapsed < 10f)
            {
                if (!LevelManager.Instance.isPaused)
                {
                    elapsed += Time.deltaTime;
                }
                yield return null;
            }
        }
    }


    /// <summary>
    /// 返回一个空闲的座位
    /// </summary>
    private CustomerSeat GetFreeSeat()
    {
        foreach (var seat in seats)
        {
            if (!seat.isOccupied)
                return seat;
        }
        return null;
    }

    
}
