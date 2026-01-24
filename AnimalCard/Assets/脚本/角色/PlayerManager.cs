using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理两个角色
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        
    }
    
}
