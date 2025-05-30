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
#if UNITY_EDITOR
        for (int i = decalList.Count - 1; i >= 0; i--)
        {
            var decal = decalList[i];
            if (decal.texture != null && !AssetDatabase.Contains(decal.texture))
            {
                Debug.LogWarning($"Decal [{i}] 的贴图已被删除，自动清理引用");
                decal.texture = null;
            }
        }
#endif

        foreach (var decal in decalList)
        {
            decal.type = globalSkinType;
        }
    }
}

