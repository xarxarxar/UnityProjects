using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : Card
{
    [SerializeField] private Image coinIcon;
    [SerializeField] private Image scoreIcon;

    [SerializeField] private Text countText;

    Color32 coinColor = new Color32(253, 151, 68, 255);
    Color32 scoreColor = new Color32(28, 176, 246, 255);
    public void CoinCard(int count)
    {
        coinIcon.gameObject.SetActive(true);
        scoreIcon.gameObject.SetActive(false);

        countText.text=$"+{count}";
        countText.color = coinColor;

    }

    public void ScoreCard(int count)
    {
        coinIcon.gameObject.SetActive(false);
        scoreIcon.gameObject.SetActive(true);

        countText.text = $"+{count}";
        countText.color = scoreColor;

    }
}
