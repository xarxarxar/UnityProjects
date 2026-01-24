using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntrence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayDoing());
        
    }

    IEnumerator delayDoing()
    {
        yield return new WaitForSeconds(1);
        DataManager.Instance.InitOrLoadPlayerData();
    }
}
