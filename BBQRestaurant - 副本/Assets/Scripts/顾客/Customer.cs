using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class Customer : MonoBehaviour
{
    public CustomerData data;
    public SpriteRenderer spriteRender;
    public CustomerSeat seat; // 客人当前的座位
    public bool isInStore=false;//客人是否在店铺里面
    [Header("饱食度设置")]
    public float maxSatiety = 100f;
    public float currentSatiety = 0;//当前的饱食度

    [Header("耐心值设置")]
    public float maxPatience = 100f;
    public float patienceToZeroTime = 45f;
    public float currentPatience = 100;//当前的耐心值

    [Header("小费比例")]
    public float fullPatienceTipMultiplier = 1.2f;   // 耐心 > 70% 时 +20%

    private Coroutine patienceCoro = null;//耐心值下降的协程
    private Coroutine currentDialogCoroutine=null;//说话的协程

    private int totalPrice=0;//该客人吃的所有食物的总售价

    /// <summary>
    /// 一开始设置顾客
    /// </summary>
    public void SetCustomer(CustomerData customerData, CustomerSeat targetSeat)
    {
        data=customerData;
        spriteRender.sprite=data.sprite;
        MoveToSeat(targetSeat);
    }


    /// <summary>
    /// 移动到座位
    /// </summary>
    /// <param name="targetSeat"></param>
    public void MoveToSeat(CustomerSeat targetSeat)
    {
        currentPatience = 100;
        //注册 UI
        CustomerUIManager.Instance.RegisterCustomerUI(this, Vector3.zero);
        //隐藏它的slider，到座位了再放出来
        CustomerUIManager.Instance.HideCustomerSlider(this);

        isInStore = true;//已经在店铺
        seat = targetSeat;
        seat.isOccupied = true;

        // 移动到 seatPoint，3秒到达
        transform.DOMove(targetSeat.seatPoint.position, 3.0f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Init(); // ← 到达后开始顾客逻辑
            });

        if (currentDialogCoroutine != null)
        {
            currentDialogCoroutine = null;
        }
        currentDialogCoroutine = StartCoroutine(AutoTalkRoutine());
    }

    /// <summary>
    /// 顾客开始离开店铺：释放座位并向右移动 10 距离
    /// </summary>
    public void MoveOut()
    {
        Say(data.patienceZero[Random.Range(0, data.patienceZero.Length)]);
        // 移除它的slider
        CustomerUIManager.Instance.HideCustomerSlider(this);
        StopCoroutine(patienceCoro);//停止协程

        PayMoney();//付钱
        
        // 释放座位,并从所有顾客列表中移除
        ReleaseSeat();

        transform.DOKill();
        Vector3 targetPos = transform.position + new Vector3(5f, 0, 0);

        transform.DOMove(targetPos, 3.0f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                End();
            });
    }

    /// <summary>
    /// 初始化顾客
    /// </summary>
    public void Init()
    {
        Debug.Log($"顾客 {name} 已经坐下，开始初始化逻辑");
        // 在这里启动顾客耐心、点餐逻辑……
        CustomerUIManager.Instance.ShowCustomerSlider(this);
        totalPrice =0;//清空食物售价
        //记录顾客
        AreaManager.Instance.AddCustomer(this);
        CustomerManager.Instance. AllCustomer.Add(this);
        if ( patienceCoro != null )
        {
            patienceCoro = null;
        }
        patienceCoro = StartCoroutine(PatienceIe());
    }

    /// <summary>
    /// 顾客移动完毕，返回对象池
    /// </summary>
    public void End()
    {
        // 从区域中移除
        isInStore = false;//已经不在店铺
        StopCoroutine(currentDialogCoroutine);//停止协程
        AreaManager.Instance.RemoveCustomer(this);
        CustomerUIManager.Instance.RemoveCustomerUI(this);
        // 回对象池
        CustomerManager.Instance.customerPool.Return(this);
    }

    /// <summary>
    /// 顾客离开时释放座位，并从所有顾客列表中移除
    /// </summary>
    public void ReleaseSeat()
    {
        if (seat != null)
        {
            seat.isOccupied = false;
            seat = null;
        }
        CustomerManager.Instance.RemoveCustomer(this);
    }

    /// <summary>
    /// 客人被喂
    /// </summary>
    public void BeFeeded(Ingredient ingredient)
    {
        totalPrice += ingredient.data.costPrice;//增加食物总售价
        AddSatiety(ingredient.data.satiety);//增加顾客的饱食度
    }

    /// <summary>
    /// 增加顾客饱食度
    /// </summary>
    public void AddSatiety(float satiety)
    {
        currentSatiety = Mathf.Min(currentSatiety+ satiety, maxSatiety);
        if(currentSatiety>= maxSatiety)
        {
            //Destroy(gameObject);
            MoveOut();//客人已吃饱
        }
    }

    //顾客付钱
    private void PayMoney()
    {
        //判断耐心值
        float ratio = currentPatience / maxPatience;
        if (ratio <= 0)//耐心为0
        {
            BattleGoldManager.Instance.AddGold(0);
        }
        else if (ratio <= CustomerManager.Instance.LowPatienceThreshold)//耐心值很低
        {
            //Debug.Log("付了很少的钱");
            BattleGoldManager.Instance.AddGold(Mathf.RoundToInt(totalPrice * 0.5f));//四舍五入
        }
        else if (ratio <= CustomerManager.Instance.MidPatienceThreshold)//耐心值较低
        {
            //Debug.Log("付了正常的钱");
            BattleGoldManager.Instance.AddGold(totalPrice);//四舍五入
        }
        else//耐心值很高
        {
            //Debug.Log("付了很多的钱");
            BattleGoldManager.Instance.AddGold(Mathf.RoundToInt(totalPrice * 1.5f));//四舍五入
        }
        totalPrice = 0;//清空食物售价
    }

    private IEnumerator PatienceIe()
    {
        currentPatience = 100f;

        while (currentPatience > 0)
        {
            // 暂停时等待，不更新耐心
            while (LevelManager.Instance.isPaused)
            {
                yield return null;
            }

            // 未暂停时才执行耐心下降
            currentPatience -= (100f / patienceToZeroTime) * Time.deltaTime;
            if (currentPatience < 0)
                currentPatience = 0;

            CustomerUIManager.Instance.UpdateCustomerSlider(this);

            yield return null;
        }

        // 耐心耗尽
        MoveOut();
    }


    /// <summary>
    /// 主动说话（由顾客自己触发，例如耐心值变化或随机闲聊）
    /// </summary>
    public void Say()
    {
        if (LevelManager.Instance.isPaused) return;

        // 如果当前正在说话，且不是被动打断的情况，主动台词就忽略
        if (CustomerUIManager.Instance.IsCustomerTalking(this))
        {
            return; // 忽略本次主动说话
        }

        // 根据顾客状态选择合适的台词池
        string txt = GetRandomDialogByState();
        if (!string.IsNullOrEmpty(txt))
        {
            CustomerUIManager.Instance.ShowCustomerDialog(this, txt, interrupt: false);
        }
    }


    /// <summary>
    /// 被动说话（外部事件触发，需要立即覆盖当前对话）
    /// </summary>
    /// <param name="txt">要说的话</param>
    public void Say(string txt)
    {
        if (LevelManager.Instance.isPaused) return;

        CustomerUIManager.Instance.ShowCustomerDialog(this, txt, interrupt: true);
    }

    IEnumerator AutoTalkRoutine()
    {
        while (isInStore)
        {
            if (LevelManager.Instance.isPaused)
            {
                yield return null;
                continue;
            }

            Debug.Log($"是否正在说话{CustomerUIManager.Instance.IsCustomerTalking(this)}");
            if (!CustomerUIManager.Instance.IsCustomerTalking(this))
            {
                // 50% 概率说话
                if (Random.value < 0.5f) // Random.value 返回 [0,1)
                {
                    Say();
                }
            }
            // 每隔一定时间尝试说话
            yield return new WaitForSeconds(Random.Range(4f, 6f));
            
        }
    }

    /// <summary>
    /// 根据耐心值或状态随机选择主动台词
    /// </summary>
    /// <returns></returns>
    private string GetRandomDialogByState()
    {
        // 耐心变成 0 或以下 → 不再返回主动对话，改用被动对话
        if (currentPatience <= 0)
        {
            return null; // 返回 null，让外层逻辑触发被动对话
                         // 或者你也可以直接 return GetPassiveDialog();
        }

        string[] pool;

        float ratio = currentPatience / data.maxPatience;

        if (ratio > CustomerManager.Instance.MidPatienceThreshold)
            pool = data.highPatience;
        else if (ratio > CustomerManager.Instance.LowPatienceThreshold)
            pool = data.midPatience;
        else
            pool = data.lowPatience;

        if (pool == null || pool.Length == 0) return null;

        return pool[Random.Range(0, pool.Length)];
    }
}
