using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingList : MonoBehaviour
{
    public Image[] medal;
    public TextMeshProUGUI textRank;
    public Image imageFlag;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textTurn;

    public void SetList(int rank, Sprite flag, string name, int score, int turn)
    {
        medal[0].gameObject.SetActive(false);
        medal[1].gameObject.SetActive(false);
        medal[2].gameObject.SetActive(false);

        //Medal set
        if (rank - 1 < 3 && rank != 0 && score > 0)
        {
            textRank.gameObject.SetActive(false);
            medal[rank - 1].gameObject.SetActive(true);
        }
        else
        {
            textRank.gameObject.SetActive(true);
            if (rank == 0)
            {
                textRank.text = "-";
            }
            else
            {
                if (score == 0)
                {
                    textRank.text = "-";
                }
                else
                {
                    textRank.text = rank.ToString();
                }
            }
        }

        //Flag set
        if (flag != null)
        {
            imageFlag.sprite = flag;
        }
        else
        {
            imageFlag.sprite = (PlayManager.Instance.currentBase as CtrHome).panelRanking.GetLangFlag("flag_default");
        }

        //Name setting
        textName.text = name;

        if (score != 0)
        {
            //Score setting
            textScore.text = Utility.ChangeThousandsSeparator(score);
            textTurn.text = turn.ToString();
        }
        else
        {
            textScore.text = "-";
            textTurn.text = "-";
        }

        this.gameObject.SetActive(true);
    }
}
