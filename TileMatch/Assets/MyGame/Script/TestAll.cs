using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TestAll : MonoBehaviour
{
    string teststr = "{\"data\": {\"UserName\": \"Player1\"}}";
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(JsonUtility.FromJson<LocalUserDataContioner>(teststr).data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
