using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MonsterAtributeType
{
    MoveType,
    GenderType,
    ElementType,
    ClassType,
    AbilityType
}

public interface IMonsterAtributeUpDown
{
    void PointerDown(Vector3 position, string title, string des);
    void PointerUp();
}

public class MonsterAtributeView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image image;
    public string title, des;
    public IMonsterAtributeUpDown monsterAtributeUpDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        monsterAtributeUpDown?.PointerDown(eventData.position, title, des);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        monsterAtributeUpDown?.PointerUp();
    }

    public void SetUp(IMonsterAtributeUpDown iM, MonsterAtributeType monsterAtribute, MoveType moveType = default, GenderType genderType = default,
        Element element = default, BeastClass beastClass = default, AbilityType abilityType = default)
    {
        monsterAtributeUpDown = iM;

        if (monsterAtribute == MonsterAtributeType.MoveType)
        {
            var data = GameData.Instance.monsterDesSprites.GetMoveDesSprite(moveType);
            title = data.title;
            des = data.info;
            image.sprite = data.sprite;
        }
        else if (monsterAtribute == MonsterAtributeType.GenderType)
        {
            var data = GameData.Instance.monsterDesSprites.GetGenderDesSprite(genderType);
            title = data.title;
            des = data.info;
            image.sprite = data.sprite;
        }
        else if (monsterAtribute == MonsterAtributeType.ElementType)
        {
            var data = GameData.Instance.monsterDesSprites.GetBeastElementDesSprite(element);
            title = data.title;
            des = data.info;
            image.sprite = data.sprite;
        }
        else if (monsterAtribute == MonsterAtributeType.ClassType)
        {
            var data = GameData.Instance.monsterDesSprites.GetBeastClassDesSprite(beastClass);
            title = data.title;
            des = data.info;
            image.sprite = data.sprite;
        }
        else if (monsterAtribute == MonsterAtributeType.AbilityType)
        {
            var data = GameData.Instance.monsterDesSprites.GetAbilityDesSprite(abilityType);
            title = data.title;
            des = data.info;
            image.sprite = data.sprite;
        }
    }
}
