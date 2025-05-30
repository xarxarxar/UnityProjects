using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCalledFunction : MonoBehaviour
{
     private GameInfo gameInfo { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class GameInfo
{
    public string lastUpdateTime;//更新的时间，精确到秒


    public GameInfo()
    {
        Debug.Log("GameInfo Constructor Called");
    }
}
