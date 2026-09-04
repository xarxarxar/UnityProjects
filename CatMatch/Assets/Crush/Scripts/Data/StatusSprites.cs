using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusDict : SerializableDictionary<Status, Sprite> { }

[System.Serializable]
public class StatusEffectDict : SerializableDictionary<Status, GameObject> { }

[CreateAssetMenu(fileName = "StatusSprites", menuName = "CrushDatas/StatusSprites")]
public class StatusSprites : ScriptableObject
{
    public StatusDict statusSprites;
    public StatusEffectDict statusEffectPrefs;

    public Sprite GetStatusSprite(Status rewardType)
    {
        return statusSprites.ContainsKey(rewardType) ? statusSprites[rewardType] : null;
    }

    public GameObject GetStatusPref(Status rewardType)
    {
        return statusEffectPrefs.ContainsKey(rewardType) ? statusEffectPrefs[rewardType] : null;
    }
}
