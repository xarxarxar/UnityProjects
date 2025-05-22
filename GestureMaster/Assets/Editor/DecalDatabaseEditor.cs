#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DecalDatabase))]
public class DecalDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DecalDatabase db = (DecalDatabase)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Decal Database", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        db.globalSkinType = (SkinType)EditorGUILayout.EnumPopup("Global Skin Type", db.globalSkinType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Decal List", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 分组缓存
        Dictionary<SkinLevel, List<DecalData>> levelGroups = new Dictionary<SkinLevel, List<DecalData>>();
        foreach (SkinLevel level in System.Enum.GetValues(typeof(SkinLevel)))
        {
            levelGroups[level] = new List<DecalData>();
        }

        foreach (var decal in db.decalList)
        {
            levelGroups[decal.level].Add(decal);
        }

        // 修改 Level 缓存：key 是 DecalData，value 是新的 level
        Dictionary<DecalData, SkinLevel> levelChanges = new();

        // 删除缓存
        List<DecalData> deleteList = new();

        foreach (var group in levelGroups)
        {
            DrawGroupHeader(group.Key.ToString() + " 等级", GetColorByLevel(group.Key));
            List<DecalData> list = group.Value;

            foreach (var decal in list)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;

                decal.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", decal.texture, typeof(Texture2D), false);

                // 改为缓存 level 修改
                SkinLevel newLevel = (SkinLevel)EditorGUILayout.EnumPopup("Level", decal.level);
                if (newLevel != decal.level)
                {
                    levelChanges[decal] = newLevel;
                }

                decal.isOwned = EditorGUILayout.Toggle("Is Owned", decal.isOwned);
                decal.isEquip = EditorGUILayout.Toggle("Is Equip", decal.isEquip);
                decal.description = EditorGUILayout.TextField("Description", decal.description);
                decal.price = EditorGUILayout.IntField("Price", decal.price);

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("删除", GUILayout.Width(60), GUILayout.Height(60)))
                {
                    deleteList.Add(decal);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            // 添加按钮
            if (GUILayout.Button($"添加 {group.Key} 级别新贴花"))
            {
                Undo.RecordObject(db, "Add Decal");
                DecalData newDecal = new DecalData
                {
                    level = group.Key,
                    price = 0,
                    isOwned = false,
                    isEquip = false,
                    type = db.globalSkinType,
                    description = "",
                    texture = null
                };
                db.decalList.Add(newDecal);
                EditorUtility.SetDirty(db);
            }

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
        }

        // 应用 level 修改
        if (levelChanges.Count > 0)
        {
            Undo.RecordObject(db, "Change Decal Level");
            foreach (var pair in levelChanges)
            {
                pair.Key.level = pair.Value;
            }
            EditorUtility.SetDirty(db);
        }

        // 应用删除
        if (deleteList.Count > 0)
        {
            Undo.RecordObject(db, "Delete Decal");
            foreach (var decal in deleteList)
            {
                db.decalList.Remove(decal);
            }
            EditorUtility.SetDirty(db);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGroupHeader(string title, Color bgColor)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 25);
        EditorGUI.DrawRect(rect, bgColor);
        EditorGUI.LabelField(rect, title, new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        });
        EditorGUILayout.Space();
    }

    private Color GetColorByLevel(SkinLevel level)
    {
        return level switch
        {
            SkinLevel.Normal => new Color(0, 0, 0, 0.4f),
            SkinLevel.Purple => new Color(0.4f, 0.2f, 0.5f, 0.7f),
            SkinLevel.Golden => new Color(0.6f, 0.5f, 0.0f, 0.7f),
            _ => Color.gray
        };
    }
}
#endif
