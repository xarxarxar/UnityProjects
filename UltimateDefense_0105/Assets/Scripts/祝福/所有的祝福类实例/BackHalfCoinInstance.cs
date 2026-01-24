using System.Collections;
using UnityEngine;


/// <summary>
/// n秒钟之后返还消耗金币的一半
/// </summary>
public class BackHalfCoinInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 0.5f, 0.7f, 0.9f };
    private int totalCoin;
    private Coroutine coroutine;

    public BackHalfCoinInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        Debug.Log("开始返还金币祝福");
        totalCoin = 0;
        CurrencyManager.OnCoinChange += OnCoinChange;
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private void OnCoinChange(int delta)
    {
        if (delta < 0)//只有小于0，才是消耗
        {
            totalCoin +=Mathf.Abs(delta);
        }
        
    }

    private IEnumerator CountDown()
    {
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, 60);
        yield return TimerUtility.WaitForGameSeconds(60);//固定等待60秒
        blessBuffShow.Close();
        CurrencyManager.Instance.AddCoin(Mathf.RoundToInt(totalCoin * multipliers[rarity]));//返还金币
        End();
        yield break;
    }

    public override void End()
    {
        CurrencyManager.OnCoinChange -= OnCoinChange;
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }

}

