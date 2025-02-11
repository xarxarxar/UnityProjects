using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 在关卡控制器中调用
public class LevelController : MonoBehaviour
{
    public DeckManager deckManager;

    private void Start()
    {
        deckManager.roundOver +=(round)=>
        {
            StartRound();
        };

        deckManager.levelOver += (totalScore) =>
        {
            if(totalScore <deckManager.config.needScore)
            {
                Debug.Log("本关卡失败");
            }
            else
            {
                Debug.Log("通关");
            }
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartRound();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            IncreaseHandLimit(20,10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayCards();
        }

    }

    void StartRound()
    {
        // 每回合抽取3张字母牌，尝试抽1张特殊牌
        deckManager.DrawCards(3, 1);
    }

    //出牌
    void PlayCards()
    {
        deckManager.PlayCard();
    }

    // 临时提升手牌上限（示例方法）
    public void IncreaseHandLimit(int normalBonus, int specialBonus)
    {
        // 这里可以修改DeckManager的内部状态
        deckManager.config.maxNormalCards = normalBonus;
        deckManager.config.maxSpecialCards = specialBonus;

    }
}
