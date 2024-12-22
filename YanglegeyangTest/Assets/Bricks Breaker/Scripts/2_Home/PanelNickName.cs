using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Text.RegularExpressions;

public class PanelNickName : MonoBehaviour
{
    public TMP_InputField textNickInput;
    public RectTransform myPanel;

    public void ClickOK()
    {
        if (textNickInput.text.Length < 3)
        {
            PlayManager.Instance.commonUI.SetToast("<color=#404252>Nickname length is short</color>");
            return;
        }

        GameData.NickName = textNickInput.text;
        PlayManager.Instance.commonUI.SetToast("Success!!");

        (PlayManager.Instance.currentBase as CtrHome)._MyInfo.SetMyInfo();

        Destroy(this.gameObject);
    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);

    }

    public void StartEditing()
    {
        myPanel.DOKill();
        myPanel.DOAnchorPosY(150f, 0.25f).SetEase(Ease.InOutFlash);
    }

    public void ValueChanged()
    {
        string tx = textNickInput.text;
        textNickInput.text = Regex.Replace(tx, @"[^0-9a-zA-Z]", "", RegexOptions.Singleline);
    }

    public void EndEditing()
    {
        string tx = textNickInput.text;
        textNickInput.text = Regex.Replace(tx, @"[^0-9a-zA-Z]", "", RegexOptions.Singleline);
        //PlayManager.Instance.commonUI.SetToast("<color=#404252>User name must be 3 - 10 characters</color>");

        myPanel.DOKill();
        myPanel.DOAnchorPosY(0f, 0.25f).SetEase(Ease.InOutFlash);
    }
}
