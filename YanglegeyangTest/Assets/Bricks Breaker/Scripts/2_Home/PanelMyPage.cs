using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class PanelMyPage : PanelBase
{
    //Name, highest score, turn TMPro
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textBestScore;
    public TextMeshProUGUI textTurn;
    public TextMeshProUGUI textLevel;
    
    //Game record display TMPro
    public TextMeshProUGUI textCountPlay;
    public TextMeshProUGUI textCountBreakBricks;
    public TextMeshProUGUI textCountAllClear;
    public TextMeshProUGUI textCountLuckyBonus;
    public TextMeshProUGUI textCountHighestCombo;

    //Compenent
    public Slider sliderExpBar;
    public CanvasGroup myInfoGroup;
    
    public GameObject btnChange;
    public GameObject iconButtons;
    
    //panelMyPageScroll
    public PanelMyPageScroll panelMyPageScroll;


    /// <summary>
    /// My information page settings
    /// </summary>
    public void SetMyPage()
    {
        textName.text = GameData.NickName;

        textBestScore.text = Utility.ChangeThousandsSeparator(GameData.BestScore); //베스트 스코어 표시
        textTurn.text = string.Format("TURN {0}", GameData.BestTurn); //베스트 턴 표시

        textCountPlay.text = Utility.ChangeThousandsSeparator(GameData.CountPlay); //게임 횟수 표시
        textCountBreakBricks.text = Utility.ChangeThousandsSeparator(GameData.CountBreakBricks); //블럭 파괴 수 표시
        textCountAllClear.text = Utility.ChangeThousandsSeparator(GameData.CountAllClear); //AllClear 횟수 표시
        textCountLuckyBonus.text = Utility.ChangeThousandsSeparator(GameData.CountLuckyBonus); //럭키보너스 횟수 표시
        textCountHighestCombo.text = Utility.ChangeThousandsSeparator(GameData.CountHighestCombo); //최고 콤보 횟수 표시

        sliderExpBar.value = GameData.PlayerExp / GameData.PlayerMaxExp; //경험치바 설정
        textLevel.text = GameData.PlayerLevel.ToString(); // 현재 레벨 표시
    }



    /// <summary>
    /// Open my info page
    /// </summary>
    public override void Open()
    {
        panelMyPageScroll.SetData();
        panelMyPageScroll.OffScroll();
        base.Open();
    }



    /// <summary>
    /// Level name display settings
    /// </summary>
    public void SetInfoGroup(bool value)
    {
        if (!value)
        {
            PlayManager.Instance.commonUI._CoinGem.Show();

            myInfoGroup.DOKill();
            myInfoGroup.DOFade(0f, 0.2f).SetEase(Ease.OutCubic);

            iconButtons.transform.DOKill();
            iconButtons.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            PlayManager.Instance.commonUI._CoinGem.Hide();

            iconButtons.transform.DOKill();
            iconButtons.transform.DOScale(0f, 0f);

            myInfoGroup.DOKill();
            myInfoGroup.DOFade(1f, 0.25f).SetEase(Ease.OutCubic);
        }

        btnChange.SetActive(value);
    }


    /// <summary>
    /// Change icon
    /// </summary>
    public void Click_ChangeIcon()
    {
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);

        SetInfoGroup(false);
        panelMyPageScroll.OnScroll();
    }

    /// <summary>
    /// Close
    /// </summary>
    public override void Close()
    {
        if (panelMyPageScroll.isOn)
        {
            panelMyPageScroll.OffScroll();
            SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_close);

        }
        else
        {
            (PlayManager.Instance.currentBase as CtrHome)._MyInfo.SetMyInfo();
            base.Close();
        }
    }
}
