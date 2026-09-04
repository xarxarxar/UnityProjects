using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ITeamBuffTouch
{
    void OnTouch(Vector3 position, string title, string des);
    void ExitTouch(Vector3 position);
}

public class TeamBuffItemView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image effectType;
    private ITeamBuffTouch teamBuffTouch;

    private string title, des;

    public void OnPointerDown(PointerEventData eventData)
    {
        teamBuffTouch?.OnTouch(transform.position, this.title, this.des);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        teamBuffTouch?.ExitTouch(transform.position);
    }

    public void SetUp(ITeamBuffTouch teamBuffTouch,Sprite sprite, TeamBuffModel teamBuffModel)
    {
        if (teamBuffModel.teamBuff == TeamBuff.None)
        {
            teamBuffTouch = null;
        }
        else
        {
            string title = "";
            string des = "";
            int teamBuff = (int)teamBuffModel.teamBuff;
            if (teamBuff % 2 == 0)
            {
                title = "减益 " + (TeamBuff)(teamBuff - 1);
                des = "减益 " + GetElementString(teamBuffModel.buffForElement, teamBuffModel.buffForClass) + " " + teamBuffModel.value + "% " + (TeamBuff)(teamBuff - 1);
            }
            else
            {
                title = "增益 " + teamBuffModel.teamBuff;
                des = "增益 " + GetElementString(teamBuffModel.buffForElement, teamBuffModel.buffForClass) + " " + teamBuffModel.value + "% " + teamBuffModel.teamBuff;
            }

            this.title = title;
            this.des = des;
        }

        this.teamBuffTouch = teamBuffTouch;

        if (teamBuffModel.buffForElement != Element.None || teamBuffModel.buffForClass != BeastClass.None)
        {
            effectType.gameObject.SetActive(true);
            effectType.sprite = sprite;
        }
        else effectType.gameObject.SetActive(false);
    }

    string GetElementString(Element element, BeastClass buffForClass)
    {
        if(element != Element.None)
            return element.ToString();
        else
            return buffForClass.ToString();
        // return (element == Element.All ? "All" : element.ToString());
    }
}
