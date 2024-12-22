using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<int, string> mySerializableDict = new SerializableDictionary<int, string>();

    void Start()
    {
        // 初始化或操作字典
        mySerializableDict.Add(1, "Value1");
    }
}
