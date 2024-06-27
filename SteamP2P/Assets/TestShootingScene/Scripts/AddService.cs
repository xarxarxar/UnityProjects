using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AppIsSever为True时，打包出来的是服务端，需要在服务器端运行，否则是客户端
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
