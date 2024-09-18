using System;

/// <summary>
/// bgm��������
/// </summary>
public enum BackgroundMusicType
{
    MainMenu,
    Gameplay,
    
}

/// <summary>
/// ��Ч����
/// </summary>
public enum SoundEffectType
{
    ClickButton,//�����ť
    ClickCard,//�������
    ComposeCard,//���ƺϳ�
    SwitchPage,//�л�ҳ��
    Victory,//�ɹ�
    Defeat//ʧ��
}

/// <summary>
/// ��Ϸ�����ã�����������С����Ч��С�ȵ�
/// </summary>
[Serializable]
public class GameSettings
{
    // ��Ƶ����
    public float MusicVolume { get; set; } = 0.5f;
    public float SFXVolume { get; set; } = 0.5f;

    // ������Ϸ���ÿ������������
    public bool IsFullScreen { get; set; } = true;
    public int ScreenResolutionWidth { get; set; } = 1920;
    public int ScreenResolutionHeight { get; set; } = 1080;

    // �������Ӹ�����Ϸ����...

    /// <summary>
    /// ���ڱ������õ�ĳ�ִ洢�У��������ݿ���ļ�
    /// </summary>
    public void SaveSettings()
    {
        // ����ʵ�ֱ������õ��߼�
    }

    /// <summary>
    /// ���ڴ�ĳ�ִ洢�м������ã��������ݿ���ļ�
    /// </summary>
    public void LoadSettings()
    {
        // ����ʵ�ּ������õ��߼�
    }
}
