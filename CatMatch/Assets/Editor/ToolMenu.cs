using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolMenu
{
    [MenuItem("MyTools/Clear/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteKey(CrushStringHelper.OpenSlotAmount);
        PlayerPrefs.DeleteKey(CrushStringHelper.CurrentTeam);
        PlayerPrefs.DeleteKey(CrushStringHelper.CurExp);
        PlayerPrefs.DeleteKey(CrushStringHelper.BeastDataCount);
        for (int k = 0; k < 14; k++)
        {
            PlayerPrefs.DeleteKey(CrushStringHelper.BeastData + k);
        }
        PlayerPrefs.DeleteKey(CrushStringHelper.Team);
        for (int k = 0; k < 3; k++)
        {
            PlayerPrefs.DeleteKey(CrushStringHelper.TeamKey + k);
            PlayerPrefs.DeleteKey(CrushStringHelper.TeamValue + k);
        }
        PlayerPrefs.DeleteKey(CrushStringHelper.PlayStageAmount);
        PlayerPrefs.DeleteKey(CrushStringHelper.SweepAmount);
        PlayerPrefs.DeleteKey(CrushStringHelper.MaxStage);

        var count = PlayerPrefs.GetInt(CrushStringHelper.MyStageCount);
        for (int k = 0; k < count; k++)
        {
            PlayerPrefs.DeleteKey(CrushStringHelper.MyStage + k);
        }
        PlayerPrefs.DeleteKey(CrushStringHelper.MyStageCount);

        PlayerPrefs.DeleteKey(CrushStringHelper.LastOnLineDateTime);
        PlayerPrefs.DeleteKey(CrushStringHelper.LastTimeSecAgain);
        PlayerPrefs.DeleteKey(CrushStringHelper.LastReceiveMileStoneReward);

        PlayerPrefs.DeleteKey(CrushStringHelper.FirstWinStages);
        PlayerPrefs.DeleteKey(CrushStringHelper.MyShop);
    }

    [MenuItem("MyTools/Clear/Clear Somthing")]
    private static void ClearSomthing()
    {
        PlayerPrefs.DeleteKey(CrushStringHelper.LastReceiveMileStoneReward);
        PlayerPrefs.DeleteKey(CrushStringHelper.PlayStageAmount);
    }

    [MenuItem("MyTools/Clear/Clear All")]
    private static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("MyTools/MergeBeast Main Scene")]
    private static void MergeBeastMain()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Merge Beast/Scenes/Main.unity");
    }

    [MenuItem("MyTools/Crush Main Scene")]
    private static void CrushMainScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Crush/Scenes/Crush/InGame.unity");
    }

    [MenuItem("MyTools/Go To Crush")]
    private static void GoToCrush()
    {
        string path = "Assets/Crush/Scenes";

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

        Selection.activeObject = obj;

        EditorGUIUtility.PingObject(obj);
    }

    [MenuItem("MyTools/Go To MergeBeast")]
    private static void GoToMergeBeast()
    {
        string path = "Assets/Merge Beast/Scenes";

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

        Selection.activeObject = obj;

        EditorGUIUtility.PingObject(obj);
    }
}
