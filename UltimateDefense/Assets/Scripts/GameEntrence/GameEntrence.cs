using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntrence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        DataManager.Instance.InitOrLoadPlayerData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
