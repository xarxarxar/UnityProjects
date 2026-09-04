using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum BeastInTeamSlotType
{
    BeastInfo,
    Empty,
    Lock
}

public class BeastInTeamItemView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject goBeastlot;
    public GameObject goEmptySlot;
    public GameObject goLockSlot;

    public Image upgradeImg;
    public Image beastIcon;

    public Text starAmountTxt;
    public Text gemValueTxt;

    public Action shortTouchAction;
    public Action longTouchAction;

    private float startTime, endTime;
    IEnumerator actionLongIE;

    public void OnPointerDown(PointerEventData eventData)
    {
        startTime = Time.time;
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

        endTime = Time.time;
        // Debug.Log("Haha:" + (endTime - startTime));
        var delTime = endTime - startTime;

        if (delTime > 0.05f)
        {
            if (delTime < 0.25f)
            {
                shortTouchAction?.Invoke();
            }
            // else
            // {
            //     longTouchAction?.Invoke();
            // }
        }
        startTime = 0f;
        endTime = 0f;
    }

    IEnumerator CountDown()
    {
        var time = Time.time;
        while (true)
        {
            yield return null;
            var newTime = Time.time;
            var delTime = newTime - time;
            if (delTime >= 0.25f)
            {
                longTouchAction?.Invoke();
                break;
            }
        }
    }

    public void SetUp(BeastInTeamSlotType type, BeastTeamInfo item, string gemValue, Action shortTouchAction, Action longTouchAction)
    {
        goBeastlot.SetActive(type == BeastInTeamSlotType.BeastInfo);
        goEmptySlot.SetActive(type == BeastInTeamSlotType.Empty);
        goLockSlot.SetActive(type == BeastInTeamSlotType.Lock);

        upgradeImg?.gameObject.SetActive(false);

        if (type == BeastInTeamSlotType.BeastInfo)
        {
            var sp = BeastPrefs.Instance.GetBeastPref(item.beastId).GetComponent<BeastBase>().icons[0];
            if (sp != null) beastIcon.sprite = sp;

            starAmountTxt.text = item.curStar.ToString();
        }
        else if (type == BeastInTeamSlotType.Lock)
        {
            gemValueTxt.text = gemValue;
        }

        this.shortTouchAction = shortTouchAction;
        this.longTouchAction = longTouchAction;
    }

    public void UpdateData(BeastTeamInfo item)
    {
        starAmountTxt.text = item.curStar.ToString();
    }
}
