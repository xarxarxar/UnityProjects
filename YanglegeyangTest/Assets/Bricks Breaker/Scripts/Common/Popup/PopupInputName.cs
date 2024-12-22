using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupInputName : PopupBase
{

    public TMP_InputField textNickInput;

    /// <summary>
    /// Called when the OK button is clicked after entering the name
    /// </summary>
    public void ClickOK()
    {
        if (textNickInput.text.Length < 3)
        {

            //If the number of characters is less than the standard, a warning toast pop-up will appear.
            PlayManager.Instance.commonUI.SetToast("<color=#404252>Nickname length is short</color>");
            return;
        }

        //Save nickname in game data
        GameData.NickName = textNickInput.text;

        //Called when the OK button is clicked after entering the name
        PlayManager.Instance.commonUI.SetToast("Success!!");

        //Update information in the upper left
        (PlayManager.Instance.currentBase as CtrHome).SetMyInfoMyPage();

        Close();
    }

    /// <summary>
    /// Called at input start
    /// </summary>
    public void StartEditing()
    {
        textNickInput.placeholder.transform.SetAsFirstSibling();
        popup.DOKill();
        popup.DOAnchorPosY(150f, 0.25f).SetEase(Ease.InOutFlash);
    }

    /// <summary>
    /// Called when the content has changed
    /// </summary>
    public void ValueChanged()
    {
        string tx = textNickInput.text;
        textNickInput.text = Regex.Replace(tx, @"[^0-9a-zA-Z]", "", RegexOptions.Singleline);
    }

    /// <summary>
    /// alled when input is complete
    /// </summary>
    public void EndEditing()
    {
        string tx = textNickInput.text;
        textNickInput.text = Regex.Replace(tx, @"[^0-9a-zA-Z]", "", RegexOptions.Singleline);
        //PlayManager.Instance.commonUIe.SetToast("<color=#404252>User name must be 3 - 10 characters</color>");

        popup.DOKill();
        popup.DOAnchorPosY(0f, 0.25f).SetEase(Ease.InOutFlash);
    }
}
