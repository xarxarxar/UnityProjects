using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public enum RoleEnum
{
    Self,
    Npc
}


public class Role : MonoBehaviour
{
    public int currentHp;
    public int maxHp;
    public int maxPatience;//最大耐心值
    public int ShieldCount;//护盾次数
    public int coinCount;//金币数量
    
    /// <summary>
    /// 金币数量变化事件，前一个参数为变化前，后一个参数为变化后
    /// </summary>
    public event UnityAction<int, int> OnCoinChange;


    public void Init(AnimalData animalData, UnityAction callback=null)
    {
        maxPatience = animalData.Patience;
        maxHp = 100;
        //填充数值
        currentHp =maxHp;
        ShieldCount = 0;
        coinCount = 0;

        RoleDataUIManager.Instance.RegisterRoleUI(this,Vector3.zero);
        callback?.Invoke();
    }

    public void ResetRole()
    {
        //转为值类型
        maxHp = 0;
        maxPatience = 1;

        //填充数值
        currentHp = maxHp;
        ShieldCount = 0;
        coinCount = 0;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (ShieldCount > 0)
        {
            ShieldCount--;
            return;
        }

        currentHp = Mathf.Max(currentHp - damage, 0);
        RoleDataUIManager.Instance.UpdateRoleHealth(this);
    }
    /// <summary>
    /// 回血
    /// </summary>
    /// <param name="hp"></param>
    public void RecoverHp(int hp)
    {
        currentHp = Mathf.Min(currentHp + hp, maxHp);
        RoleDataUIManager.Instance.UpdateRoleHealth(this);
    }
    /// <summary>
    /// 添加护盾次数
    /// </summary>
    /// <param name="count"></param>
    public void AddShieldCount(int count)
    {
        ShieldCount += count;
    }
    /// <summary>
    /// 添加金币
    /// </summary>
    /// <param name="count"></param>
    public void AddCoin(int count)
    {
        if (count <= 0) return;

        int newCoinCount = coinCount + count;
        OnCoinChange?.Invoke(coinCount, newCoinCount);
        coinCount = newCoinCount;

    }
    /// <summary>
    /// 直接减少金币，适合对方调用此方法来偷取我的金币，返回具体减少了多少金币
    /// </summary>
    /// <param name="count"></param>
    public int ReduceCoin(int count)
    {
        if (count < 0) return 0;

        int newCoinCount;
        int deltaCount;
        if (coinCount >= count)
        {
            newCoinCount = coinCount - count;
            deltaCount = count;
        }
        else
        {
            newCoinCount = 0;
            deltaCount = coinCount;
        }
        OnCoinChange?.Invoke(coinCount, newCoinCount);
        coinCount = newCoinCount;
        return deltaCount;
    }
    /// <summary>
    /// 花费金币，适合自己购买东西时调用
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool SpendCoin(int count)
    {
        if (count < 0) return false;

        if (coinCount>= count)
        {
            int newCoinCount = coinCount - count;
            OnCoinChange?.Invoke(coinCount, newCoinCount);
            coinCount = newCoinCount;
            return true;
        }
        return false;
    }
}
