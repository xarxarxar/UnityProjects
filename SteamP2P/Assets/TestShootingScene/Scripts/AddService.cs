using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AppIsSeverΪTrueʱ������������Ƿ���ˣ���Ҫ�ڷ����������У������ǿͻ���
/// </summary>
public class AddService : MonoBehaviour
{
    public bool AppIsServer = false;
    private NetworkManager networkManager;
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        if (AppIsServer == true)
        {
            networkManager.StartServer();
        }
        else
        {
            networkManager.StartClient();
        }

    }

}
