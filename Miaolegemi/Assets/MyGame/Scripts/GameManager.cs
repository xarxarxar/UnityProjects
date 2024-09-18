using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏的主入口
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        // 确保 GameManager 在场景间不会被销毁,且仅有一个实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        SetMainSceneBGM();//设置主场景的背景音乐
    }

    /// <summary>
    /// 设置主场景的背景音乐
    /// </summary>
    private void SetMainSceneBGM()
    {
        AudioManager.Instance.PlayBackgroundMusic(BackgroundMusicType.MainMenu,0.5f);

    }
}
