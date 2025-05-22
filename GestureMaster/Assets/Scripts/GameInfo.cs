using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameInfo
{
    private int level;//当前关卡
    public int coinCount;//金币数量
    public bool isMusicOn;//音乐是否开启
    public bool isSfxfOn;//音效是否开启

    public int Level 
    { 
        get => level;
        set
        {
            if(level != value)
            {
                level = value;
                GameManager.instance.levelText.text = $"关卡{level}";
            }
        }
    }
}
