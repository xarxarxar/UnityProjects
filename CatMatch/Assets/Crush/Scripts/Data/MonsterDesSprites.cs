using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveDesSprite
{
    public MoveType moveType;
    public Sprite sprite;
    public string title;
    public string info;
}

[System.Serializable]
public class GenderDesSprite
{
    public GenderType genderType;
    public Sprite sprite;
    public string title;
    public string info;
}

[System.Serializable]
public class ElementDesSprite
{
    public Element element;
    public Sprite sprite;
    public string title;
    public string info;
}

[System.Serializable]
public class BeastClassDesSprite
{
    public BeastClass beastClass;
    public Sprite sprite;
    public string title;
    public string info;
}

[System.Serializable]
public class AbilityDesSprite
{
    public AbilityType abilityType;
    public Sprite sprite;
    public string title;
    public string info;
}

[CreateAssetMenu(fileName = "MonsterDesSprites", menuName = "CrushDatas/MonsterDesSprites")]
public class MonsterDesSprites : ScriptableObject
{
    public List<MoveDesSprite> moveDesSprites;
    public List<GenderDesSprite> genderDesSprites;
    public List<ElementDesSprite> elementDesSprites;
    public List<BeastClassDesSprite> beastClassDesSprites;
    public List<AbilityDesSprite> abilityDesSprites;

    public MoveDesSprite GetMoveDesSprite(MoveType moveType)
    {
        return moveDesSprites.Find(m => m.moveType == moveType);
    }

    public GenderDesSprite GetGenderDesSprite(GenderType moveType)
    {
        return genderDesSprites.Find(m => m.genderType == moveType);
    }

    public ElementDesSprite GetBeastElementDesSprite(Element element)
    {
        return elementDesSprites.Find(m => m.element == element);
    }

    public BeastClassDesSprite GetBeastClassDesSprite(BeastClass moveType)
    {
        return beastClassDesSprites.Find(m => m.beastClass == moveType);
    }

    public AbilityDesSprite GetAbilityDesSprite(AbilityType moveType)
    {
        return abilityDesSprites.Find(m => m.abilityType == moveType);
    }
}
