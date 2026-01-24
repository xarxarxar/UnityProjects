using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class RoleData 
{
    public int maxHp;
    public float maxStrength;
    public float Speed;
}


public class Role : MonoBehaviour
{
    public int currentHp;
    public int maxHp;
    public float currentStrength;//体力值，在战斗中可以加
    public float maxStrength;//最大体力值
    public float Speed;//生成金币的速度
    public int ShieldCount;//护盾次数
    public int coinCount;//金币数量

    private CardManager _cardManager;
    /// <summary>
    /// 金币数量变化事件，前一个参数为变化前，后一个参数为变化后
    /// </summary>
    public event UnityAction<int, int> OnCoinChange;

    private Coroutine SpawnCoinCoro = null;

    public void Init(CardManager cardManager, RoleData roleData, UnityAction callback=null)
    {
        _cardManager= cardManager;
        //转为值类型
        (int maxHp, float maxStrength, float Speed) paras = (roleData.maxHp, roleData.maxStrength, roleData.Speed);
        maxHp = paras.maxHp;
        maxStrength = paras.maxStrength;
        Speed = paras.Speed;

        //填充数值
        currentHp =maxHp;
        currentStrength=maxStrength;
        ShieldCount = 0;
        if (SpawnCoinCoro != null)
        {
            StopCoroutine(SpawnCoinCoro);
            SpawnCoinCoro=null;
        }
        coinCount = 0;
        SpawnCoinCoro = StartCoroutine(SpawnCoinIE());

        RoleDataUIManager.Instance.RegisterRoleUI(this,Vector3.zero);
        callback?.Invoke();
    }

    public void ResetRole()
    {
        _cardManager = null;
        //转为值类型
        maxHp = 0;
        maxStrength = 0;
        Speed = 0;

        //填充数值
        currentHp = maxHp;
        currentStrength = maxStrength;
        ShieldCount = 0;
        if (SpawnCoinCoro != null)
        {
            StopCoroutine(SpawnCoinCoro);
            SpawnCoinCoro = null;
        }
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
    /// 恢复体力
    /// </summary>
    /// <param name="strength"></param>
    public void RecoverStrength(float strength)
    {
        currentStrength = Mathf.Min(currentStrength + strength, maxStrength);
    }
    /// <summary>
    /// 减少体力
    /// </summary>
    /// <param name="strength"></param>
    public void ReduceStrength(float strength)
    {
        currentStrength = Mathf.Max(currentStrength - strength, 0);
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
    /// <summary>
    /// 召唤一张卡牌
    /// </summary>
    public SingleCard SummonACard()
    {
        return _cardManager.GetRandomCard();
    }


    private IEnumerator SpawnCoinIE()
    {
        if(Speed==0)
        {
            yield break;
        }
        while(true)
        {
            yield return new WaitForSecondsRealtime(1/Speed);//Speed越快，生成的金币越快
            AddCoin(1);
        }
    }
}
