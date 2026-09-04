using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillHomeInfoItemView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite skillBgOn;
    public Sprite skillBgOff;
    public Sprite skillTipSp;
    public Sprite skillLockSp;

    public int starAmount;
    public Text skillNameTxt;
    public Image skillBgImg;
    public Image skillTipImg;

    private BeastInfoHomePanel beastInfoHomePanel;
    private SkillDes skillDes;
    private BeastTeamInfo beastTeamInfo;

    public void OnPointerDown(PointerEventData eventData)
    {
        var skillOn = beastTeamInfo.curStar >= starAmount;
        bool isPassive = starAmount != 3;
        beastInfoHomePanel.TouchDownSkill(eventData.position, skillNameTxt.text, isPassive, BuildDesSkill(),
            !isPassive ? skillDes.skillSp : null,
            skillOn ? BuildNoteSkillOn() : "",
            skillOn ? "" : string.Format(LangManager.Instance.Get("NoteSkillOff"), starAmount));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        beastInfoHomePanel.TouchUpSkill(eventData.position);
    }

    private string BuildDesSkill()
    {
        var res = skillDes.skillDes;

        return res;
    }

    private string BuildNoteSkillOn()
    {
        var res = "";
        if (skillDes.percentDmgByStar > 0)
        {
            var nextStar = (beastTeamInfo.curStar - starAmount) + 1;
            if (nextStar > 5) return "";

            res = string.Format(LangManager.Instance.Get("NoteSkillOn"), skillDes.percentDmgByStar * nextStar + "% ") + LangManager.Instance.Get("Atk");
        }
        return res;
    }

    public void SetUp(BeastInfoHomePanel beastInfoHomePanel, BeastTeamInfo beastTeamInfo, SkillDes skillDes)
    {
        this.beastInfoHomePanel = beastInfoHomePanel;
        this.skillDes = skillDes;
        this.beastTeamInfo = beastTeamInfo;

        skillNameTxt.text = skillDes.skillName;
        skillDes.skillSp = skillDes.skillSp == null ? skillTipSp : skillDes.skillSp;

        var skillOn = beastTeamInfo.curStar >= starAmount;
        if (skillOn)
        {
            skillBgImg.sprite = skillBgOn;
            skillTipImg.sprite = skillDes.skillSp;
        }
        else
        {
            skillBgImg.sprite = skillBgOff;
            skillTipImg.sprite = skillLockSp;
        }
    }
}
