using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
        clickAudio = clickAudioList[0];
    }

    // 定义一个枚举
    public enum 点击音效
    {
        音效1,
        音效2,
        音效3,
        音效4,
        音效5
    }

    // 私有字段来存储当前的枚举值
    [SerializeField]
    private 点击音效 _myClickAudio;

    // 属性，用于检测枚举值的变化    
    public 点击音效 MyClickAudio
    {
        get { return _myClickAudio; }
        set
        {
            if (_myClickAudio != value)
            {
                Debug.Log(_myClickAudio);
                Debug.Log(value);
                _myClickAudio = value;
                OnEnumChanged((int)_myClickAudio); // 当枚举值改变时执行方法,传递索引
            }
        }
    }

    // 枚举值改变时执行的方法
    private void OnEnumChanged(int clickAudioIndex)
    {
        clickAudio = clickAudioList[clickAudioIndex];
    }

    private void OnValidate()
    {
        // 确保在编辑器中也能触发
        // 直接调用 OnEnumChanged 来查看效果
        OnEnumChanged((int)_myClickAudio);
    }
}
