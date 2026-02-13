using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalBaseData", menuName = "Game/AnimalBaseData")]
public class AnimalBaseData : ScriptableObject
{
    public string RaceName;
    public string Race;
    public Sprite MaleSprite;
    public Sprite FemaleSprite;
}
