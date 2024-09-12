using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public List<Sprite> imageSprite=new List<Sprite>();

    public SerializableDictionary<string,Sprite> imageDict= new SerializableDictionary<string,Sprite>();

    public List<AudioClip> clickAudioList=new List<AudioClip>();
    public AudioClip clickAudio;
    public AudioSource clickAudioSource;
    private void Awake()
    {
        instance = this;
        //clickAudio = clickAudioList[0];
    }

    // ����һ��ö��
    public enum �����Ч
    {
        ��Ч1,
        ��Ч2,
        ��Ч3,
        ��Ч4,
        ��Ч5
    }

    // ˽���ֶ����洢��ǰ��ö��ֵ
    [SerializeField]
    private �����Ч _myClickAudio;

    // ���ԣ����ڼ��ö��ֵ�ı仯    
    public �����Ч MyClickAudio
    {
        get { return _myClickAudio; }
        set
        {
            if (_myClickAudio != value)
            {
                Debug.Log(_myClickAudio);
                Debug.Log(value);
                _myClickAudio = value;
                OnEnumChanged((int)_myClickAudio); // ��ö��ֵ�ı�ʱִ�з���,��������
            }
        }
    }

    // ö��ֵ�ı�ʱִ�еķ���
    private void OnEnumChanged(int clickAudioIndex)
    {
        clickAudio = clickAudioList[clickAudioIndex];
    }

    private void OnValidate()
    {
        // ȷ���ڱ༭����Ҳ�ܴ���
        // ֱ�ӵ��� OnEnumChanged ���鿴Ч��
        OnEnumChanged((int)_myClickAudio);
    }

    

    public void LoadImageFromUrl(string url, Action<Texture2D> success)
    {
        StartCoroutine(DownloadImage(url, success));
    }

     IEnumerator DownloadImage(string url,Action<Texture2D> success)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            success?.Invoke(texture);
        }
        else
        {
            Debug.LogError("Image download failed: " + request.error);
        }
    }
}
