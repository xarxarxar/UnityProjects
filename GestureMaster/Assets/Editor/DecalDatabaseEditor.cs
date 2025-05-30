#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(DecalDatabase))]
public class DecalDatabaseEditor : Editor
{
    private ReorderableList list;
    private FieldInfo isDefaultField;
    private List<bool> foldouts;

    private static readonly List<string> meijiaDescription = new() { "每轮倒计时长+0.3秒" };
    private static readonly List<string> tiehuaDescription = new() { "每轮PK胜利时额外+1金币" };

    private void OnEnable()
    {
        isDefaultField = typeof(DecalData).GetField("isDefault", BindingFlags.NonPublic | BindingFlags.Instance);
        var db = (DecalDatabase)target;

        // 初始化折叠状态
        foldouts = new List<bool>();
        foreach (var _ in db.decalList) foldouts.Add(false);

        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty("decalList"), true, true, true, true);

        list.drawHeaderCallback = rect =>
            EditorGUI.LabelField(rect, "Decal 列表 (可拖拽排序，点击折叠) ");

        list.elementHeightCallback = index =>
        {
            bool exp = foldouts[index];
            int lines = exp ? 9 : 2; // 折叠时2行，展开时9行
            return lines * EditorGUIUtility.singleLineHeight + (lines + 1) * 4;
        };

        list.drawElementCallback = (rect, index, active, focused) =>
        {
            var prop = list.serializedProperty.GetArrayElementAtIndex(index);
            var dbInst = (DecalDatabase)target;
            var decal = dbInst.decalList[index];
            bool exp = foldouts[index];

            // 折叠控件
            Rect foldRect = new Rect(rect.x + 2, rect.y + 2, 12, EditorGUIUtility.singleLineHeight);
            foldouts[index] = EditorGUI.Foldout(foldRect, exp, GUIContent.none);

            float x = rect.x + 18;
            float w = rect.width - 36;
            float lh = EditorGUIUtility.singleLineHeight;
            float y = rect.y + 2;

            // ID
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(new Rect(x, y, w, lh), "ID", decal.id);
            EditorGUI.EndDisabledGroup();
            // 删除按钮
            Rect btnDel = new Rect(rect.x + rect.width - 32, y, 30, lh);
            if (GUI.Button(btnDel, "删"))
            {
                Undo.RecordObject(dbInst, "Delete Decal");
                dbInst.decalList.RemoveAt(index);
                foldouts.RemoveAt(index);
                EditorUtility.SetDirty(dbInst);
                return;
            }
            y += lh + 4;

            // 贴图
            decal.texture = (Texture2D)EditorGUI.ObjectField(new Rect(x, y, w, lh), "贴图", decal.texture, typeof(Texture2D), false);
            y += lh + 4;

            if (!exp) return;

            // 解锁方式
            var methodProp = prop.FindPropertyRelative("unlockMethod");
            UnlockMethod m = (UnlockMethod)methodProp.enumValueIndex;
            m = (UnlockMethod)EditorGUI.EnumPopup(new Rect(x, y, w, lh), "解锁方式", m);
            if ((int)m != methodProp.enumValueIndex)
            {
                Undo.RecordObject(dbInst, "Change Method");
                methodProp.enumValueIndex = (int)m;
                ApplyDescriptionAndPrice(decal, m);
                EditorUtility.SetDirty(dbInst);
            }
            y += lh + 4;

            // 解锁信息
            if (m != UnlockMethod.CoinPurchase)
            {
                var infoProp = prop.FindPropertyRelative("unlockInfo");
                infoProp.stringValue = EditorGUI.TextField(new Rect(x, y, w, lh), "信息", infoProp.stringValue);
                y += lh + 4;
            }

            // 描述
            EditorGUI.LabelField(new Rect(x, y, w, lh), "描述: " + decal.description);
            y += lh + 4;

            // 价格
            if (m == UnlockMethod.CoinPurchase)
            {
                EditorGUI.LabelField(new Rect(x, y, w, lh), "价格: " + decal.price);
                y += lh + 4;
            }

            // 默认
            bool def = (bool)isDefaultField.GetValue(decal);
            bool newDef = EditorGUI.Toggle(new Rect(x, y, w, lh), "默认贴花", def);
            if (newDef != def)
            {
                Undo.RecordObject(dbInst, "Toggle Default");
                if (newDef)
                    foreach (var o in dbInst.decalList)
                        if (o != decal && o.type == decal.type)
                            isDefaultField.SetValue(o, false);
                isDefaultField.SetValue(decal, newDef);
                EditorUtility.SetDirty(dbInst);
            }
            y += lh + 4;

            // 拥有
            decal.isOwned = EditorGUI.Toggle(new Rect(x, y, w, lh), "拥有", decal.isOwned);
            y += lh + 4;

            // 装备
            decal.isEquip = EditorGUI.Toggle(new Rect(x, y, w, lh), "装备", decal.isEquip);
            y += lh + 4;

            // 底线
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 2, rect.width, 1), Color.gray);
        };

        list.onAddCallback = l =>
        {
            var dbInst = (DecalDatabase)target;
            Undo.RecordObject(dbInst, "Add Decal");
            var d = new DecalData { type = dbInst.globalSkinType, unlockMethod = UnlockMethod.CoinPurchase };
            d.id = (d.type == SkinType.NailArt ? "mj_" : "th_") + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            ApplyDescriptionAndPrice(d, UnlockMethod.CoinPurchase);
            dbInst.decalList.Add(d);
            foldouts.Add(true);
            EditorUtility.SetDirty(dbInst);
        };

        list.onRemoveCallback = l =>
        {
            var dbInst = (DecalDatabase)target;
            Undo.RecordObject(dbInst, "Remove Decal");
            dbInst.decalList.RemoveAt(l.index);
            foldouts.RemoveAt(l.index);
            EditorUtility.SetDirty(dbInst);
        };

        list.onReorderCallback = l =>
        {
            var dbInst = (DecalDatabase)target;
            var newF = new List<bool>();
            foreach (var _ in dbInst.decalList) newF.Add(false);
            for (int i = 0; i < newF.Count && i < foldouts.Count; i++) newF[i] = foldouts[i];
            foldouts = newF;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var db = (DecalDatabase)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Decal Database", EditorStyles.boldLabel);
        db.globalSkinType = (SkinType)EditorGUILayout.EnumPopup("Global Skin Type", db.globalSkinType);
        EditorGUILayout.HelpBox("每种类型只能一个默认贴花。", MessageType.Info);

        list.DoLayoutList();
        EditorGUILayout.Space();
        if (GUILayout.Button("清除所有 isOwned 和 isEquip"))
        {
            Undo.RecordObject(db, "Clear Status");
            foreach (var d in db.decalList) { d.isOwned = false; d.isEquip = false; }
            EditorUtility.SetDirty(db);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ApplyDescriptionAndPrice(DecalData d, UnlockMethod m)
    {
        d.description = d.type == SkinType.NailArt ? meijiaDescription[0] : tiehuaDescription[0];
        d.price = m == UnlockMethod.CoinPurchase ? 100 : 0;
    }
}
#endif