using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSteam : MonoBehaviour
{
    string id;
    // Start is called before the first frame update
    void Start()
    {
        if(SteamManager.Initialized)
        {
            string name=SteamFriends.GetPersonaName();
            Debug.Log("Hello"+name);

            id=SteamApps.GetAppOwner().ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUILayout.TextArea(id);
    }
}
