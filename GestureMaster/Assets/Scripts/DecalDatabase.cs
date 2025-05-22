using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "DecalDatabase", menuName = "Decals/DecalDatabase")]
public class DecalDatabase : ScriptableObject
{
    public SkinType globalSkinType = SkinType.NailArt;
    public List<DecalData> decalList = new List<DecalData>();

    private void OnValidate()
    {
        foreach (var decal in decalList)
        {
            decal.type = globalSkinType;
            decal.UpdateDescriptionAndPrice();
        }
    }
}

