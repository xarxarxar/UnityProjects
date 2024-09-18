using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�������
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        // ȷ�� GameManager �ڳ����䲻�ᱻ����,�ҽ���һ��ʵ��
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
        SetMainSceneBGM();//�����������ı�������
    }

    /// <summary>
    /// �����������ı�������
    /// </summary>
    private void SetMainSceneBGM()
    {
        AudioManager.Instance.PlayBackgroundMusic(BackgroundMusicType.MainMenu,0.5f);

    }
}
