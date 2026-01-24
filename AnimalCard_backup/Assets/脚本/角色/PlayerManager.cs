using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理两个角色
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public RoleControl selfRoleControl;//自己
    public RoleControl npcRoleControl;//npc

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        selfRoleControl.Init();
        npcRoleControl.Init();
    }
}
