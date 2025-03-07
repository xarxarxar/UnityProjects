using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentInfo : MonoBehaviour
{
    public static CurrentInfo Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Text currentScoreText;
    public Text nextScoreText;
    public Text letterCardMaxText;
    public Text specialCardMaxText;
    public Text continousCountText;
    public Text continousProbText;
    public Text extraScoreOnlyOneText;
    public Text canPlayZeroCardText;
    public Text addScoreWhenDeleteText;
    public Text extraScoreRounOverText;

    public void CloseCurrentInfoPanel()
    {
        transform.DOScale(0,0.2f).SetEase(Ease.OutQuart);
        GetComponent<Canvas>().enabled = false;
    }
}
