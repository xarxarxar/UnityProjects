using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回答数学题
/// </summary>
public class AnswerMathInstance : BlessInstance
{
    private int[] maxCoins = new int[3] { 2000, 5000, 10000 };
    private int coinCount;

    private Coroutine coroutine;


    public AnswerMathInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        BattleUIManager.Instance.AnswerMathQuesPanel.maxCoinCount = maxCoins[rarity];
        BattleUIManager.Instance.AnswerMathQuesPanel.maxTime = config.times[rarity];

        BattleUIManager.Instance.AnswerMathQuesPanel.gameObject.SetActive(true);

        BattleUIManager.Instance.AnswerMathQuesPanel.OnAnswerEnd -= OnAnswerEnd;
        BattleUIManager.Instance.AnswerMathQuesPanel.OnAnswerEnd += OnAnswerEnd;

        coroutine=BlessManager.Instance.StartCoroutine(DelayDo());

        IEnumerator DelayDo()
        {
            yield return null;
            BattleManager.Instance.PauseGame();
        }
    }

    public override void End()
    {
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        BattleManager.Instance.ResumeGame();
        CurrencyManager.Instance.AddCoin(coinCount);
        //BlessManager.Instance.RemoveBlessInstance(this);
    }


    private void OnAnswerEnd(int count)
    {
        coinCount = count;
        End();
    }
}
