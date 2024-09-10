using System;

[Serializable]
public class GameSettings
{
    // 音频设置
    public float MusicVolume { get; set; } = 0.5f;
    public float SFXVolume { get; set; } = 0.5f;

    // 其他游戏设置可以在这里添加
    public bool IsFullScreen { get; set; } = true;
    public int ScreenResolutionWidth { get; set; } = 1920;
    public int ScreenResolutionHeight { get; set; } = 1080;

    // 可以增加更多游戏设置...

    /// <summary>
    /// 用于保存设置到某种存储中，例如数据库或文件
    /// </summary>
    public void SaveSettings()
    {
        // 这里实现保存设置的逻辑
    }

    /// <summary>
    /// 用于从某种存储中加载设置，例如数据库或文件
    /// </summary>
    public void LoadSettings()
    {
        // 这里实现加载设置的逻辑
    }
}
