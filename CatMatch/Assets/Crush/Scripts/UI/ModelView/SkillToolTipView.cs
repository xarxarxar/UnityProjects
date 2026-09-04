
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillToolTipView : MonoBehaviour
{
    public Text titleTxt;
    public Text statusTxt;
    public Text titleTxt2;
    public Text statusTxt2;

    public TextMeshProUGUI desTxt;

    public Image skillImg;
    public TextMeshProUGUI noteUnLockTxt;
    public TextMeshProUGUI noteLockTxt;

    public void SetUp(string title, bool isPassive, string des, Sprite skillSp, string noteUnLock, string noteLock)
    {
        if (isPassive)
        {
            skillImg.gameObject.SetActive(false);
            titleTxt.gameObject.SetActive(false);
            statusTxt.gameObject.SetActive(false);

            titleTxt2.gameObject.SetActive(true);
            statusTxt2.gameObject.SetActive(true);


            titleTxt2.text = title;
            statusTxt2.text = LangManager.Instance.Get("Passive");
        }
        else
        {
            skillImg.gameObject.SetActive(true);
            titleTxt.gameObject.SetActive(true);
            statusTxt.gameObject.SetActive(true);

            titleTxt2.gameObject.SetActive(false);
            statusTxt2.gameObject.SetActive(false);

            titleTxt.text = title;
            statusTxt.text = LangManager.Instance.Get("Active");
        }

        desTxt.text = des;

        if (skillSp != null) skillImg.sprite = skillSp;
        if (!string.IsNullOrEmpty(noteUnLock))
        {
            noteUnLockTxt.gameObject.SetActive(true);
            noteLockTxt.gameObject.SetActive(false);

            noteUnLockTxt.text = noteUnLock;
        }
        if (!string.IsNullOrEmpty(noteLock))
        {
            noteLockTxt.gameObject.SetActive(true);
            noteUnLockTxt.gameObject.SetActive(false);

            noteLockTxt.text = noteLock;
        }
    }
}