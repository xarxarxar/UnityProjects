using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BeastTeamItemView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image upgradeImg;
    public Image beastIcon;

    public Slider slider;
    public Text medalAmountTxt;

    public GameObject starGo;
    public Text starAmountTxt;

    private Action shortTouchAction;
    private Action longTouchAction;

    private DateTime startTime, endTime;

    public bool longHold = false;

    IEnumerator actionLongIE;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("startTime:" + startTime);
        startTime = DateTime.Now;

        if (actionLongIE == null)
        {
            actionLongIE = CountDown();
            StartCoroutine(actionLongIE);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(actionLongIE);
        actionLongIE = null;

        endTime = DateTime.Now;
        var delTime = endTime - startTime;
        Debug.Log("Haha delTime:" + delTime);

        if (delTime.TotalSeconds > 0.05f)
        {
            if (delTime.TotalSeconds < 0.25f)
            {
                // Debug.Log("weeeeeeeeee:" + (this.shortTouchAction == null));
                this.shortTouchAction.Invoke();
            }
            // else
            // {
            //     longTouchAction?.Invoke();
            //     longHold = true;
            // }
        }
        // startTime = 0f;
        // endTime = 0f;
    }

    IEnumerator CountDown()
    {
        var time = DateTime.Now;
        // Debug.Log("quay len");
        while (true)
        {
            yield return null;
            var newTime = DateTime.Now;
            var delTime = newTime - time;
            if (delTime.TotalSeconds >= 0.25f)
            {
                // Debug.Log("heeeeeeeeeeeee:" + (this.longTouchAction == null));
                this.longTouchAction.Invoke();
                break;
            }
        }
    }

    public void SetUp(BeastTeamInfo item, Action shortTouchAction, Action longTouchAction)
    {
        longHold = false;

        UpdateData(item);


        this.shortTouchAction = shortTouchAction;
        this.longTouchAction = longTouchAction;
    }

    public void UpdateData(BeastTeamInfo item)
    {
        // var sp = BeastIcons.Instance.GetSprite(item.beastId);
        var sp = BeastPrefs.Instance.GetBeastPref(item.beastId).GetComponent<BeastBase>().icons[0];

        if (sp != null) beastIcon.sprite = sp;

        if (item.curStar == 0)
        {
            starGo.SetActive(false);
            slider.gameObject.SetActive(true);
            slider.maxValue = Constant.medalByStars[0];
            slider.value = item.curMedal > Constant.medalByStars[0] ? Constant.medalByStars[0] : (float)item.curMedal;

            medalAmountTxt.text = MergeBeast.Utils.FormatNumber(item.curMedal) + "/" + Constant.medalByStars[0];

            var tmpColor = beastIcon.color;
            tmpColor.a = 0.7f;
            beastIcon.color = tmpColor;
        }
        else
        {
            starGo.SetActive(true);
            slider.gameObject.SetActive(false);

            starAmountTxt.text = item.curStar.ToString();

            var tmpColor = beastIcon.color;
            tmpColor.a = 1f;
            beastIcon.color = tmpColor;
        }

        upgradeImg.gameObject.SetActive(false);

        for (int i = 0; i < Constant.medalByStars.Count; i++)
        {
            if (item.curStar < (i + 1) && item.curMedal >= Constant.medalByStars[i])
            {
                upgradeImg.gameObject.SetActive(true);
                break;
            }
        }
    }
}
